' Ralf Mouthaan
' University of Cambridge
' March 2020

Option Explicit On
Option Strict On

Module modMeasurementSetup

    Public MeasurementSetup As clsMeasurementSetup

End Module

Public Class clsMeasurementSetup

    Public Camera As clsCamera
    Public SLM As clsSLM
    Public ReadOnly ObjHoloWidth As Integer = 500
    Public ReadOnly RefHoloWidth As Integer = 500

    Public Sub New()

        ' Set up SLM
        SLM = New clsSLM
        SLM.intScreenNo = 2

        'Set up Camera
        Camera = New clsBlackflyCam
        Camera.Load("C:\Instrument Setup\Input Camera.txt")

    End Sub
    Public Sub Startup()
        Camera.Startup()
        SLM.StartUp()
    End Sub
    Public Sub Shutdown()
        Camera.Shutdown()
        SLM.ShutDown()
    End Sub

End Class