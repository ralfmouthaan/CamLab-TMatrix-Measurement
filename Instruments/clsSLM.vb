' Ralf Mouthaan
' University of Cambridge
' March 2018
'
' Class to operate SLM. Assumes SLM is connected as a second or third screen and all that needs to be done is for a hologram to be
' displayed in full-screen mode on this screen. Uses the GDI+ library to quickly update the hologram. This is significantly quicker, 
' but Is less legible As it makes calls To unmanaged code, requires some pointer manipulations, And operates on a flattened array.

Option Explicit On
Option Strict On

Public Class clsSLM
    Inherits clsInstrument

    Private frmDisplay As Form
    Private picDisplay As PictureBox

    Public ReadOnly Property intResolutionx As Integer
    Public ReadOnly Property intResolutiony As Integer
    Public Property intScreenNo As Integer

    Public lstHolograms As List(Of clsHologram)

    Private bolCalibrationLoaded As Boolean
    Private bmapCheckerboard As Bitmap

    Public Sub New()

        _strInstrumentName = "SLM"
        intResolutionx = -999
        intResolutiony = -999
        intScreenNo = -999
        bolConnectionOpen = False
        bolCalibrationLoaded = False

        'Set up a full-screen form
        frmDisplay = New Form
        frmDisplay.FormBorderStyle = FormBorderStyle.None
        frmDisplay.WindowState = FormWindowState.Maximized

        'Add picturebox to form, stretch to fill form
        picDisplay = New PictureBox
        frmDisplay.Controls.Add(picDisplay)
        picDisplay.Dock = DockStyle.Fill

        lstHolograms = New List(Of clsHologram)

        ReDim _LevelIndexedByPhase(CInt(2 * Math.PI * 100))
        For i = 0 To CInt(2 * Math.PI * 100)
            _LevelIndexedByPhase(i) = CInt(i / (2 * Math.PI * 100) * 255)
        Next

    End Sub

    Public Sub StartUp()

        'Error checks
        If intScreenNo = -999 Then Call Err.Raise(-1, strInstrumentName, "Screen Number not defined")
        If intScreenNo > Screen.AllScreens.Count Then Call Err.Raise(-1, strInstrumentName, "Screen Number > Number of screens")

        'Get target screen
        Dim scrSLM As Screen = Screen.AllScreens(intScreenNo - 1)

        'GDI+-related declarations
        Dim bmdataLockedBits As Imaging.BitmapData
        Dim bLockedBits() As Byte
        Dim z As Integer

        'Check resolution of target screen
        _intResolutionx = scrSLM.Bounds.Width
        _intResolutiony = scrSLM.Bounds.Height

        'Create checkerboard background
        bmapCheckerboard = New Bitmap(intResolutionx, intResolutiony)

        'Lock bits
        bmdataLockedBits = bmapCheckerboard.LockBits(New Rectangle(0, 0, intResolutionx, intResolutiony),
                                               Imaging.ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)
        ReDim bLockedBits(bmdataLockedBits.Stride * bmdataLockedBits.Height - 1)
        Runtime.InteropServices.Marshal.Copy(bmdataLockedBits.Scan0, bLockedBits, 0, bmdataLockedBits.Stride * bmdataLockedBits.Height)

        'Update bits
        For i = 0 To intResolutionx - 1
            For j = 0 To intResolutiony - 1
                z = j * bmdataLockedBits.Stride + i * 3
                If (i + j) Mod 2 = 0 Then
                    bLockedBits(z) = PhaseToByte(0)
                    bLockedBits(z + 1) = bLockedBits(z)
                    bLockedBits(z + 2) = bLockedBits(z)
                Else
                    bLockedBits(z) = PhaseToByte(Math.PI)
                    bLockedBits(z + 1) = bLockedBits(z)
                    bLockedBits(z + 2) = bLockedBits(z)
                End If
            Next
        Next

        'Unlock bits
        Runtime.InteropServices.Marshal.Copy(bLockedBits, 0, bmdataLockedBits.Scan0, bLockedBits.Length)
        bmapCheckerboard.UnlockBits(bmdataLockedBits)

        'Position form on screen and show
        frmDisplay.StartPosition = FormStartPosition.Manual
        frmDisplay.Location = New Point(scrSLM.Bounds.X, scrSLM.Bounds.Y)

        'Display in a threadsafe manner, as this may be called from another thread
        If frmDisplay.InvokeRequired Then
            frmDisplay.Invoke(Sub() frmDisplay.Show())
        Else
            frmDisplay.Show()
        End If

        bolConnectionOpen = True

    End Sub
    Public Sub ShutDown()
        If frmDisplay.InvokeRequired Then
            frmDisplay.Invoke(Sub() frmDisplay.Hide())
        Else
            frmDisplay.Hide()
        End If
        bolConnectionOpen = False
    End Sub

    Public Function GetCurrentBitmap() As Bitmap

        'Return a bitmap for the current hologram based on the hologram classes in lstHolograms

        If bolConnectionOpen = False Then StartUp()

        Dim Hologram As clsHologram
        Dim bmapDisplay As Bitmap
        Dim arrHologram(,) As Double
        Dim intHoloWidth As Integer, intHoloHeight As Integer
        Dim intLockWidth As Integer, intLockHeight As Integer
        Dim intLockLeft As Integer, intLockTop As Integer, intLockRight As Integer, intLockBottom As Integer
        Dim intOffsetx As Integer, intOffsety As Integer 'Measured from top-left of SLM display to top-left of hologram
        Dim bmdataLockedBits As Imaging.BitmapData
        Dim bLockedbits() As Byte

        'Set background to checkerboard
        bmapDisplay = New Bitmap(bmapCheckerboard)

        'Add holograms, account for Zernikes
        For indHologram = 0 To lstHolograms.Count - 1

            Hologram = lstHolograms(indHologram)

            If Hologram.bolVisible = False Then Continue For

            'Determine width and height of hologram
            intHoloWidth = Hologram.Width
            intHoloHeight = Hologram.Width

            'Windows uses top-left of images as a reference point.
            'To match JC code, the hologram positions are relative to the bottom-left of the SLM display and the center of the hologram.
            'So we correct for this.
            intOffsetx = Hologram.intOffsetx - CInt(intHoloWidth / 2)
            intOffsety = intResolutiony - Hologram.intOffsety - CInt(intHoloWidth / 2)

            'Determine width and height of lock region
            intLockLeft = intOffsetx
            intLockTop = intOffsety
            intLockRight = intOffsetx + intHoloWidth
            intLockBottom = intOffsety + intHoloHeight
            If intLockLeft < 0 Then intLockLeft = 0
            If intLockTop < 0 Then intLockTop = 0
            If intLockRight > intResolutionx - 1 Then intLockRight = intResolutionx - 1
            If intLockBottom > intResolutiony - 1 Then intLockBottom = intResolutiony - 1
            intLockWidth = intLockRight - intLockLeft
            intLockHeight = intLockBottom - intLockTop

            If intLockWidth > 0 And intLockHeight > 0 Then

                'Get bytes corresponding to hologram region
                'This returns a one-dimensional byte array indexed by x, y and (B,G,R)
                bmdataLockedBits = bmapDisplay.LockBits(New Rectangle(intLockLeft, intLockTop, intLockWidth, intLockHeight),
                                               Imaging.ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)
                ReDim bLockedbits(bmdataLockedBits.Stride * bmdataLockedBits.Height - 1)
                Runtime.InteropServices.Marshal.Copy(bmdataLockedBits.Scan0, bLockedbits, 0, bmdataLockedBits.Stride * bmdataLockedBits.Height)

                arrHologram = Hologram.arrHologram

                ' Only outer for is parallelised else the overhead of declaring x, y, z becomes considerable.
                ' It may be possible to have x, y, z persistent in a vb.net thread, but I have not figured out how yet.
                Parallel.For(0, intHoloWidth,
                Sub(i)

                    Dim x As Integer, y As Integer, z As Integer

                    For j = 0 To intHoloHeight - 1

                        'Calculate x and y, positions in lock region
                        x = i
                        y = j
                        If intOffsetx < 0 Then x += intOffsetx
                        If intOffsety < 0 Then y += intOffsety

                        'Check if this is within the display region
                        If x < 0 Or y < 0 Or x > intLockWidth - 1 Or y > intLockHeight - 1 Then Continue For

                        'Set pixel
                        z = y * bmdataLockedBits.Stride + x * 3
                        If Double.IsNaN(arrHologram(i, j)) Then
                            bLockedbits(z) = PhaseToByte(((i + j) Mod 2) * Math.PI)
                        Else
                            bLockedbits(z) = PhaseToByte(arrHologram(i, j))
                        End If
                        bLockedbits(z + 1) = bLockedbits(z)
                        bLockedbits(z + 2) = bLockedbits(z)

                    Next

                End Sub)

                'Unlock bits
                Runtime.InteropServices.Marshal.Copy(bLockedbits, 0, bmdataLockedBits.Scan0, bLockedbits.Length)
                bmapDisplay.UnlockBits(bmdataLockedBits)

            End If

        Next

        Return bmapDisplay

    End Function
    Public Sub Refresh()

        'Display holograms as currently set is lstHolograms

        Dim bmapdisplay As Bitmap
        bmapdisplay = GetCurrentBitmap()

        'Show on SLM
        'Do this in a threadsafe manner
        If frmDisplay.InvokeRequired Then
            frmDisplay.Invoke(Sub()
                                  picDisplay.Image = bmapdisplay
                                  picDisplay.Refresh()
                              End Sub)
        Else
            picDisplay.Image = bmapdisplay
            picDisplay.Refresh()
        End If

        Threading.Thread.Sleep(100)

    End Sub
    Public Sub Refresh(bmapDisplay As Bitmap)

        'Display bitmap passed in.
        'Note, if this is used then the displayed hologram may have no bearing on lstHolograms

        'Show on SLM
        'Do this in a threadsafe manner
        If frmDisplay.InvokeRequired Then
            frmDisplay.Invoke(Sub()
                                  picDisplay.Image = bmapDisplay
                                  picDisplay.Refresh()
                              End Sub)
        Else
            picDisplay.Image = bmapDisplay
            picDisplay.Refresh()
        End If

        Threading.Thread.Sleep(100)

    End Sub

#Region "Calibration-Related"
    Private _LevelIndexedByPhase(CInt(2 * Math.PI * 100)) As Integer
    Private Function PhaseToByte(ByVal Phase As Double) As Byte

        Dim index As Integer = CInt(Math.Round(Phase * 100))
        index = index Mod CInt(2 * Math.PI * 100)
        If index < 0 Then index += CInt(2 * Math.PI * 100)
        Dim i As Integer = _LevelIndexedByPhase(index)
        i = i Mod 255
        Return CByte(i)

    End Function
    Public Sub LoadCalibrationFile(ByVal strFilename As String)

        'Some error checks
        If IO.File.Exists(strFilename) = False Then Call Err.Raise(-1, strInstrumentName, "Calibration file does not exist")
        Dim reader As New IO.StreamReader(strFilename)
        If reader.ReadLine <> "SLM Calibration" Then
            reader.Close()
            Call Err.Raise(-1, strInstrumentName, "File is not an SLM calibration file")
        End If

        'Read data into array PhaseIndexedByLevel
        Dim splitstr() As String
        Dim PhaseIndexedByLevel(256) As Double
        For i = 0 To 255
            If reader.EndOfStream = True Then
                reader.Close()
                Call Err.Raise(-1, strInstrumentName, "Seemingly not enough lines in calibration file")
            End If
            splitstr = Split(reader.ReadLine(), vbTab)
            If CInt(splitstr(0)) <> i Then
                reader.Close()
                Call Err.Raise(-1, strInstrumentName, "Could not find calibration for level " & CStr(i))
            End If
            PhaseIndexedByLevel(i) = CDbl(splitstr(1))
            PhaseIndexedByLevel(i) = PhaseIndexedByLevel(i) - PhaseIndexedByLevel(0)
        Next
        PhaseIndexedByLevel(256) = 2 * Math.PI
        reader.Close()

        'Interpolate to get LevelIndexedByPhase array
        Dim j As Integer = 0
        For i = 0 To CInt(2 * Math.PI * 100)

            While i / 100 < PhaseIndexedByLevel(j)
                j = j - 1
            End While
            While i / 100 >= PhaseIndexedByLevel(j + 1)
                j = j + 1
            End While

            If Math.Abs(i / 100 - PhaseIndexedByLevel(j)) < Math.Abs(i / 100 - PhaseIndexedByLevel(j + 1)) Then
                _LevelIndexedByPhase(i) = j
            Else
                _LevelIndexedByPhase(i) = j + 1
            End If

            If _LevelIndexedByPhase(i) = 256 Then _LevelIndexedByPhase(i) = 0

        Next

        bolCalibrationLoaded = True

    End Sub
#End Region

End Class
