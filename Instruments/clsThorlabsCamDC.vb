' Ralf Mouthaan
' University of Cambridge
' June 2016
'
' Class to control Thorlabs camera
'
' If using multiple cameras then each camera must have a unique CameraID, which needs to be set beforehand.

Option Explicit On
Option Strict On

Public Class clsThorlabsCamDC
    Inherits clsCamera

    Private cam As uc480.Camera
    Private pMemory As Integer
    Private _ImageWidth As Integer
    Private _ImageHeight As Integer
    Private _Exposure As Double

    Public Property intCameraID As Integer
    Public Property intTriggerMode As Integer
    Public Property portSLMClock As IO.Ports.SerialPort
    Public Property bolActive As Boolean

    Public Sub New()

        _strInstrumentName = "Thorlabs DC Camera"
        cam = New uc480.Camera()
        _intCameraID = 1
        _Exposure = 6.86
        bolActive = False

    End Sub
    Public Sub New(ByVal strFilename As String)

        cam = New uc480.Camera()
        _strInstrumentName = "Thorlabs DC Camera"
        Load(strFilename)

    End Sub

    Public Overrides Sub Save(strFilename As String)

        MyBase.Save(strFilename)

        Dim writer As New IO.StreamWriter(strFilename, True)

        writer.WriteLine("Thorlabs Camera")
        writer.WriteLine("Camera ID" + vbTab + intCameraID.ToString)
        writer.WriteLine("Trigger Mode" + vbTab + intTriggerMode.ToString)

        If portSLMClock Is Nothing Then
            writer.WriteLine("SLM Clock Port = " + vbTab + "Nothing")
        Else
            writer.WriteLine("SLM Clock Port = " + vbTab + portSLMClock.PortName)
        End If

        writer.Close()

    End Sub

    Public Overrides Sub Load(strFilename As String)

        MyBase.Load(strFilename)

        Dim reader As New IO.StreamReader(strFilename)

        'Read to relevant bit
        Do
        Loop Until reader.ReadLine.StartsWith("Thorlabs Camera")

        intCameraID = CInt(Split(reader.ReadLine, vbTab)(1))
        _intTriggerMode = CInt(Split(reader.ReadLine, vbTab)(1))
        If intTriggerMode = 2 Then
            portSLMClock = New IO.Ports.SerialPort(Split(reader.ReadLine, vbTab)(1))
        End If
        bolActive = True

        reader.Close()

    End Sub

    Public Overrides Sub Startup(Optional bolShow As Boolean = False)

        If bolActive = False Then Exit Sub

        Dim info As uc480.Types.SensorInfo

        bolConnectionOpen = True

        cam.Init(intCameraID)
        If cam.IsOpened = False Then
            Call Err.Raise(-1, "Thorlabs Camera", "Could not open camera")
        End If
        cam.Parameter.ResetToDefault()

        'Note: DC1545 camera only has 8 bit resolution.
        cam.PixelFormat.Set(uc480.Defines.ColorMode.Mono8)
        cam.Timing.PixelClock.Set(30)
        cam.Timing.Framerate.Set(15)
        Exposure = _Exposure

        cam.Information.GetSensorInfo(info)
        _ImageWidth = info.MaxSize.Width
        _ImageHeight = info.MaxSize.Height

        If intTriggerMode = 0 Then
            'Free run
            cam.Trigger.Set(uc480.Defines.TriggerMode.Continuous)
        ElseIf intTriggerMode = 1 Then
            If info.SensorName.EndsWith("M") = False Then Call Err.Raise(-1, strInstrumentName, "Instrument does not support hirose trigger")
            cam.Trigger.Set(uc480.Defines.TriggerMode.Lo_Hi)
        ElseIf intTriggerMode = 2 Then
            If portSLMClock Is Nothing Then Call Err.Raise(-1, strInstrumentName, "Instrument does not support Arduino trigger")
        End If

        cam.Memory.Allocate(pMemory, True)

        MyBase.Startup()

    End Sub

    Public Overrides Sub Shutdown()
        If bolConnectionOpen = False Then Exit Sub
        If cam IsNot Nothing Then
            If pMemory <> 0 Then cam.Memory.Free(pMemory)
            cam.Exit()
        End If
        MyBase.Shutdown()
    End Sub

    Public Overrides ReadOnly Property ImageWidth As Integer

        Get
            Return _ImageWidth
        End Get
    End Property

    Public Overrides ReadOnly Property ImageHeight As Integer
        Get
            Return _ImageHeight
        End Get
    End Property

    Public Overrides Property Exposure As Double
        Get

            Dim RetVal As Double
            If bolConnectionOpen = False Then
                RetVal = _Exposure
            Else
                cam.Timing.Exposure.Get(RetVal)
            End If

            Return RetVal

        End Get
        Set(value As Double)

            _Exposure = value
            If bolConnectionOpen = False Then Exit Property

            If bolConnectionOpen = True Then
                cam.Timing.Exposure.Set(value)
            End If

        End Set
    End Property

    Public Overrides Sub SaveImage(Filename As String)
        cam.Acquisition.Freeze(uc480.Defines.DeviceParameter.Wait)
        cam.Image.Save(Filename, pMemory)
    End Sub

    Public Overrides Function GetIntegerImage(Optional NoFrames As Integer = -1) As Integer(,)

        Dim RetVal(,) As Integer = Nothing
        Dim ImageBuff() As Integer = Nothing
        Dim max As Integer

        If intTriggerMode = 2 Then
            portSLMClock.WriteLine("ping")
            portSLMClock.ReadLine()
        End If

        cam.Memory.Clear(pMemory, uc480.Defines.ColorMode.Mono8)
        cam.Acquisition.Freeze(uc480.Defines.DeviceParameter.Wait)
        cam.Memory.CopyToArray(pMemory, ImageBuff)

        ReDim RetVal(ImageWidth - 1, ImageHeight - 1)

        'Unflatten image, convert to integer
        max = 0
        For j = 0 To ImageHeight - 1
            For i = 0 To ImageWidth - 1
                RetVal(i, j) = ImageBuff(j * ImageWidth + i)
                If RetVal(i, j) > max Then max = RetVal(i, j)
            Next
        Next
        If max = 255 Then
            Console.WriteLine("Thorlabs camera saturated")
        End If

        Return RetVal

    End Function

End Class
