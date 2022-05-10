' Ralf Mouthaan
' University of Cambridge
' March 2018
' 
' Container class for hologram information
' Any calculations are generally done in SLM class.

Option Explicit On
Option Strict On

Public MustInherit Class clsHologram

    Public Sub New()
        bolVisible = True
        bolCircularAperture = True
        bolApplyZernikes = True
        ReDim _arrZernikes(8)
    End Sub

    Public Property bolVisible As Boolean
    Public Property bolCircularAperture As Boolean
    Public Property bolApplyZernikes As Boolean
    Public Property bolCheckerboard As Boolean

    Public ReadOnly Property arrHologramCmx As Numerics.Complex(,)
        Get
            Dim phase(,) As Double = arrHologram

            Dim RetVal(Width - 1, Width - 1) As Numerics.Complex

            For i = 0 To Width - 1
                For j = 0 To Width - 1
                    RetVal(i, j) = New Numerics.Complex(Math.Cos(phase(i, j)), Math.Sin(phase(i, j)))
                Next
            Next

            Return RetVal

        End Get
    End Property
    Public ReadOnly Property arrHologram As Double(,)
        Get

            If RawWidth = 0 Then Call Err.Raise(-1, "Hologram Class", "Width has not yet been defined")

            Dim RetVal(Width - 1, Width - 1) As Double

            Parallel.For(0, Width,
            Sub(i)
                For j = 0 To Width - 1

                    If bolVisible = False Then
                        RetVal(i, j) = Double.NaN
                        Continue For
                    End If

                    If bolCircularAperture = True And lutnormradius(i, j) > 1 Then
                        RetVal(i, j) = Double.NaN
                        Continue For
                    End If

                    If bolCheckerboard = True Then
                        RetVal(i, j) = Double.NaN
                        Continue For
                    End If

                    If bolApplyZernikes = False Then
                        RetVal(i, j) = arrRawHologram(i, j)
                        Continue For
                    End If

                    'RetVal(i, j) = arrRawHologram(CInt(Math.Round(i / dblScale)), CInt(Math.Round(j / dblScale)))
                    RetVal(i, j) = arrRawHologram(i, j)

                    'Correct for Zernikes
                    RetVal(i, j) += dblPiston
                    RetVal(i, j) += dblTiltx * Math.PI * lutradius(i, j) * lutsintheta(i, j)
                    RetVal(i, j) += dblTilty * Math.PI * lutradius(i, j) * lutcostheta(i, j)
                    'RetVal(i, j) += dblFocus * Math.PI * (2 * lutradius(i, j) * lutradius(i, j) - 1)
                    'RetVal(i, j) += dblAstigmatismx * Math.PI * lutradius(i, j) * lutradius(i, j) * 2 * lutsintheta(i, j) * lutcostheta(i, j)
                    'RetVal(i, j) += dblAstigmatismy * Math.PI * lutradius(i, j) * lutradius(i, j) * (2 * lutcostheta(i, j) * lutcostheta(i, j) - 1)

                Next
            End Sub)

                Return RetVal

        End Get
    End Property
    Public ReadOnly Property arrHologram(ByVal i As Integer, ByVal j As Integer) As Double
        Get

            If RawWidth = 0 Then Call Err.Raise(-1, "Hologram Class", "Width has not yet been defined")
            If bolVisible = False Then Return Double.NaN
            If bolCircularAperture = True And lutnormradius(i, j) > 1 Then Return Double.NaN
            If bolCheckerboard = True Then Return Double.NaN

            If bolApplyZernikes = False Then Return arrRawHologram(i, j)

            Dim RetVal As Double
            RetVal = arrRawHologram(CInt(Math.Round(i / dblScale)), CInt(Math.Round(j / dblScale)))

            'Correct for Zernikes
            RetVal += dblPiston
            RetVal += dblTiltx * Math.PI * lutradius(i, j) * lutsintheta(i, j)
            RetVal += dblTilty * Math.PI * lutradius(i, j) * lutcostheta(i, j)
            RetVal += dblFocus * Math.PI * (2 * lutradius(i, j) * lutradius(i, j) - 1)
            RetVal += dblAstigmatismx * Math.PI * lutradius(i, j) * lutradius(i, j) * 2 * lutsintheta(i, j) * lutcostheta(i, j)
            RetVal += dblAstigmatismy * Math.PI * lutradius(i, j) * lutradius(i, j) * (2 * lutcostheta(i, j) * lutcostheta(i, j) - 1)

            Return RetVal

        End Get
    End Property

#Region "Zernikes"

    Public Property dblScale As Double
        Get
            Return arrZernikes(8)
        End Get
        Set(value As Double)
            arrZernikes(8) = value
            Call RedefineLUTs()
        End Set
    End Property
    Public Property intOffsetx As Integer
        Get
            Return CInt(arrZernikes(1))
        End Get
        Set(value As Integer)
            arrZernikes(1) = CDbl(value)
        End Set
    End Property
    Public Property intOffsety As Integer
        Get
            Return CInt(arrZernikes(2))
        End Get
        Set(value As Integer)
            arrZernikes(2) = CDbl(value)
        End Set
    End Property
    Public Property dblPiston As Double
        Get
            Return arrZernikes(0)
        End Get
        Set(value As Double)
            arrZernikes(0) = value
        End Set
    End Property
    Public Property dblTiltx As Double
        Get
            Return arrZernikes(3)
        End Get
        Set(value As Double)
            arrZernikes(3) = value
        End Set
    End Property
    Public Property dblTilty As Double
        Get
            Return arrZernikes(4)
        End Get
        Set(value As Double)
            arrZernikes(4) = value
        End Set
    End Property
    Public Property dblFocus As Double
        Get
            Return arrZernikes(5)
        End Get
        Set(value As Double)
            arrZernikes(5) = value
        End Set
    End Property
    Public Property dblAstigmatismx As Double
        Get
            Return arrZernikes(6)
        End Get
        Set(value As Double)
            arrZernikes(6) = value
        End Set
    End Property
    Public Property dblAstigmatismy As Double
        Get
            Return arrZernikes(7)
        End Get
        Set(value As Double)
            arrZernikes(7) = value
        End Set
    End Property

    Private _arrZernikes() As Double
    Public Property arrZernikes() As Double()
        Get
            Return _arrZernikes
        End Get
        Set(value As Double())
            _arrZernikes = value
        End Set
    End Property

    Public Sub SaveZernikes(ByVal Filename As String)

        Dim writer As New IO.StreamWriter(Filename)

        writer.WriteLine("SAVED ZERNIKES FILE")
        writer.WriteLine("")

        writer.WriteLine("  Piston:" & vbTab & dblPiston.ToString)
        writer.WriteLine("  Offset x:" & vbTab & intOffsetx.ToString)
        writer.WriteLine("  Offset y:" & vbTab & intOffsety.ToString)
        writer.WriteLine("  Tilt x:" & vbTab & dblTiltx.ToString)
        writer.WriteLine("  Tilt y:" & vbTab & dblTilty.ToString)
        writer.WriteLine("  Focus:" & vbTab & dblFocus.ToString)
        writer.WriteLine("  Astigmatism x:" & vbTab & dblAstigmatismx.ToString)
        writer.WriteLine("  Astigmatism y:" & vbTab & dblAstigmatismy.ToString)
        writer.WriteLine("  Scale :" & vbTab & dblScale.ToString)

        writer.Close()

    End Sub
    Public Sub LoadZernikes(ByVal Filename As String)

        Dim reader As New IO.StreamReader(Filename)
        reader.ReadLine() 'SAVED ZERNIKES FILE
        reader.ReadLine()

        dblPiston = CInt(Split(reader.ReadLine, vbTab)(1))
        intOffsetx = CInt(Split(reader.ReadLine, vbTab)(1))
        intOffsety = CInt(Split(reader.ReadLine, vbTab)(1))
        dblTiltx = CDbl(Split(reader.ReadLine, vbTab)(1))
        dblTilty = CDbl(Split(reader.ReadLine, vbTab)(1))
        dblFocus = CDbl(Split(reader.ReadLine, vbTab)(1))
        dblAstigmatismx = CDbl(Split(reader.ReadLine, vbTab)(1))
        dblAstigmatismy = CDbl(Split(reader.ReadLine, vbTab)(1))
        dblScale = CDbl(Split(reader.ReadLine, vbTab)(1))

        reader.Close()

    End Sub

#End Region

#Region "Height, Width and LUTs"

    Protected Friend lutsintheta(,) As Double
    Protected Friend lutcostheta(,) As Double
    Protected Friend lutnormradius(,) As Double
    Protected Friend lutradius(,) As Double

    Private _RawWidth As Integer
    Public Property Width As Integer
        Get
            Return CInt(Math.Round(RawWidth * dblScale)) 'Should I round, ceiling or floor instead?
        End Get
        Set(value As Integer)
            RawWidth = value
        End Set
    End Property

    Public Overridable Property RawWidth As Integer
        Get
            Return _RawWidth
        End Get
        Set(value As Integer)
            If _RawWidth <> value Then
                _RawWidth = value
                Call RedefineLUTs()
            End If
        End Set
    End Property
    Protected Friend Sub RedefineLUTs()

        ReDim lutsintheta(Width - 1, Width - 1)
        ReDim lutcostheta(Width - 1, Width - 1)
        ReDim lutnormradius(Width - 1, Width - 1)
        ReDim lutradius(Width - 1, Width - 1)

        Dim theta As Double

        For i = 0 To Width - 1
            For j = 0 To Width - 1

                'Calculate r and theta, make hologram circular
                lutnormradius(i, j) = 2 * Math.Sqrt((i - Width / 2) ^ 2 + (j - Width / 2) ^ 2) / Math.Min(Width, Width)
                lutradius(i, j) = 2 * Math.Sqrt((i - Width / 2) ^ 2 + (j - Width / 2) ^ 2) / 1000
                theta = -Math.Atan2(j - Width / 2, i - Width / 2) - Math.PI / 2
                lutsintheta(i, j) = Math.Sin(theta)
                lutcostheta(i, j) = Math.Cos(theta)

            Next
        Next

    End Sub

#End Region


    'Values are phase values between 0 and 2 pi
    Public MustOverride Property arrRawHologram(i As Integer, j As Integer) As Double 'Returns hologram elements corrected for Zernikes

    'Saves hologram, including Zernikes.
    Public Sub SaveHologram(ByVal strFilename As String)

        Dim writer As New IO.StreamWriter(strFilename)
        Dim arr(,) As Double = arrHologram


        For i = 0 To Width - 1
            For j = 0 To Width - 1

                If j <> 0 Then writer.Write(vbTab)
                writer.Write(arr(i, j))

            Next
            writer.WriteLine()
        Next

        writer.Close()

    End Sub

End Class
