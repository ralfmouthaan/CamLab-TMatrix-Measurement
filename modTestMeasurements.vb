' Ralf Mouthaan
' University of Cambridge
' ??? 2020
'
' Scripts for making test measurements.
' Essentially, a known hologram is displayed and the output speckle pattern measured

Option Explicit On
Option Strict On

Namespace TestMeasurements
    Module modTestMeasurements

        Public Sub DisplayHologramRecordImage(ByVal HologramFilename As String, ByVal SaveFilename As String)

            'Reads in the matlab-generated hologram.
            'Displays the hologram
            'Takes an image
            'Stores the image
            'Turns off the camera so I can use the camera object elsewhere.

            Dim Img(,) As Integer
            Dim CmxImg(,) As Numerics.Complex
            Dim Holo As clsMacropixelHologram
            Dim max As Integer

            MeasurementSetup = New clsMeasurementSetup
            MeasurementSetup.Startup()

            Holo = New clsMacropixelHologram
            Holo.RawWidth = MeasurementSetup.ObjHoloWidth
            Holo.LoadZernikes("C:\Instrument Setup\Tx1 Zernikes.txt")
            Holo.bolApplyZernikes = True
            Holo.bolVisible = True
            Holo.bolCircularAperture = False
            Holo.LoadFromFile(HologramFilename)

            MeasurementSetup.SLM.lstHolograms.Add(Holo)
            MeasurementSetup.SLM.Refresh()

            Do
                Img = MeasurementSetup.Camera.GetOffAxisCroppedImage
                MeasurementSetup.Camera.Exposure = MeasurementSetup.Camera.Exposure - 2
                ImageProcessing.Maximum(Img, max)
            Loop Until max < 250

            CmxImg = MeasurementSetup.Camera.GetOffAxisComplexImage
            ImageProcessing.SaveImgToFile(CmxImg, SaveFilename)
            MeasurementSetup.Camera.Shutdown()

        End Sub

        Public Sub DisplayHologram(ByVal HologramFilename As String)

            'Reads in the matlab-generated hologram.
            'Displays the hologram

            Dim Holo As clsMacropixelHologram

            MeasurementSetup = New clsMeasurementSetup
            MeasurementSetup.SLM.StartUp()

            Holo = New clsMacropixelHologram
            Holo.RawWidth = MeasurementSetup.ObjHoloWidth
            Holo.LoadZernikes("C:\Instrument Setup\Tx1 Zernikes.txt")
            Holo.bolApplyZernikes = True
            Holo.bolVisible = True
            Holo.bolCircularAperture = False
            Holo.LoadFromFile(HologramFilename)

            MeasurementSetup.SLM.lstHolograms.Add(Holo)
            MeasurementSetup.SLM.Refresh()

        End Sub
        Public Sub DisplayRandomHologram(ByVal NoMacropixels As Integer, ByVal ImgFilename As String, ByVal HoloFilename As String)

            Dim Rnd As New Random
            Dim Img(,) As Integer
            Dim CmxImg(,) As Numerics.Complex
            Dim Holo As clsMacropixelHologram
            Dim max As Integer

            MeasurementSetup = New clsMeasurementSetup
            MeasurementSetup.Startup()

            Holo = New clsMacropixelHologram
            Holo.RawWidth = MeasurementSetup.ObjHoloWidth
            Holo.LoadZernikes("C:\Instrument Setup\Tx1 Zernikes.txt")
            Holo.bolApplyZernikes = True
            Holo.bolVisible = True
            Holo.bolCircularAperture = False
            ReDim Holo.arrMacroPixels(NoMacropixels)
            Holo.SetToRandom()

            MeasurementSetup.SLM.lstHolograms.Add(Holo)
            MeasurementSetup.SLM.Refresh()

            Do
                Img = MeasurementSetup.Camera.GetOffAxisCroppedImage
                MeasurementSetup.Camera.Exposure = MeasurementSetup.Camera.Exposure - 2
                ImageProcessing.Maximum(Img, max)
            Loop Until max < 250

            CmxImg = MeasurementSetup.Camera.GetOffAxisComplexImage
            ImageProcessing.SaveImgToFile(CmxImg, ImgFilename)
            MeasurementSetup.Camera.Shutdown()

            Call Err.Raise(-1, "Still need to save hologram to file")


        End Sub

    End Module
End Namespace