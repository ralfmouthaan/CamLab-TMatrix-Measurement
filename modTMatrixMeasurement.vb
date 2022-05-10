' Ralf Mouthaan
' University of Cambridge
' April 2020
'
' Measurement routines for taking measurements in presence of phase drift

Option Explicit On
Option Strict On

Module modTMatrixMeasurement

    Public NoMeasurements As Integer
    Public NoModes As Integer
    Public bw As ComponentModel.BackgroundWorker

#Region "T-Matrix Measurement Routines"
    Public Sub TMatrixMeasurement_MacropixelBasis(sender As Object, e As ComponentModel.DoWorkEventArgs)

        Dim cmxRef(,) As Numerics.Complex
        Dim cmxNewRef(,) As Numerics.Complex
        Dim intObj(,) As Integer
        Dim intObjRef(,) As Integer
        Dim i As Integer
        Dim Holo As clsMacropixelHologram
        Dim RefPixelNo As Integer
        Dim ProcessingTask As Task
        Dim sw As Stopwatch

        'Startup
        sw = New Stopwatch
        If lstMeasurementData Is Nothing Then lstMeasurementData = New List(Of clsMeasurementDataContainer)
        bolMeasurementsDone = False
        ProcessingTask = Task.Factory.StartNew(AddressOf ProcessDataAndSave)
        MeasurementSetup = New clsMeasurementSetup
        MeasurementSetup.Startup()
        MeasurementSetup.SLM.lstHolograms.Clear()
        RefPixelNo = CInt(NoModes / 2 + Math.Sqrt(NoModes) / 2)
        cmxRef = Nothing

        'Set up hologram
        Holo = New clsMacropixelHologram With {
             .RawWidth = MeasurementSetup.ObjHoloWidth,
            .bolCircularAperture = False,
            .bolVisible = True
        }
        ReDim Holo.arrMacroPixels(NoModes - 1)
        For i = 0 To NoModes - 1
            Holo.arrMacroPixels(i) = Double.NaN
        Next
        Holo.LoadZernikes("D:\RPM Data Files\Tx1 Zernikes.txt")
        MeasurementSetup.SLM.lstHolograms.Add(Holo)

        ' Main loop
        i = 0
        sw.Start()
        While i <= NoModes - 1


            'Update reference?
            If i Mod 50 = 0 Then
                While lstMeasurementData.Count > 50
                    Threading.Thread.Sleep(100)
                End While
                Holo.arrMacroPixels(RefPixelNo) = 0
                MeasurementSetup.SLM.Refresh()
                cmxNewRef = MeasurementSetup.Camera.GetOffAxisComplexImage
                Holo.arrMacroPixels(RefPixelNo) = Double.NaN
                If i <> 0 Then
                    PhaseDriftCorrection.PhaseDriftCorrection(cmxRef, cmxNewRef, cmxNewRef)
                    Console.WriteLine("Reference Refreshed - " + PhaseDriftCorrection.ErrorMetric(cmxRef, cmxNewRef, cmxNewRef, 0).ToString)
                Else
                    ReDim cmxRef(cmxNewRef.GetLength(0) - 1, cmxNewRef.GetLength(1) - 1)
                End If
                Array.Copy(cmxNewRef, cmxRef, cmxNewRef.Length)
            End If

            'Measure + store reference + object mode
            Holo.arrMacroPixels(RefPixelNo) = 0
            Holo.arrMacroPixels(i) = 0
            MeasurementSetup.SLM.Refresh()
            intObjRef = MeasurementSetup.Camera.GetIntegerImage
            Holo.arrMacroPixels(RefPixelNo) = Double.NaN
            Holo.arrMacroPixels(i) = Double.NaN

            'Measure + store Object mode
            Holo.arrMacroPixels(i) = 0
            Holo.arrMacroPixels(RefPixelNo) = Double.NaN
            MeasurementSetup.SLM.Refresh()
            intObj = MeasurementSetup.Camera.GetIntegerImage

            'Add measurement to processing queue
            SyncLock lstMeasurementData
                lstMeasurementData.Add(New clsMeasurementDataContainer(Holo.arrMacroPixels, cmxRef, intObj, intObjRef))
            End SyncLock

            Holo.arrMacroPixels(i) = Double.NaN

            'Update GUI
            i = i + 1
            If NoMeasurements = 0 And i > NoModes - 1 Then
                i = 0
                bw.ReportProgress(CInt(i / NoModes * 1000))
            Else
                bw.ReportProgress(CInt(i / NoModes * 1000))
            End If

        End While

        sw.Stop()
        MeasurementSetup.Shutdown()

        'Wait for data processing to complete
        bolMeasurementsDone = True
        ProcessingTask.Wait()

        Console.WriteLine("Measurement time = " + (sw.ElapsedMilliseconds / 1000 / 60).ToString("F2") + " minutes")

    End Sub
    Public Sub TMatrixMeasurement_FourierBasis(sender As Object, e As ComponentModel.DoWorkEventArgs)

        Dim cmxRef(,) As Numerics.Complex
        Dim cmxNewRef(,) As Numerics.Complex
        Dim intObj(,) As Integer
        Dim intObjRef(,) As Integer
        Dim i As Integer
        Dim RefHolo As clsBlankHologram
        Dim ObjHolo As clsFourierMacropixelHologram
        Dim ProcessingTask As Task
        Dim sw As Stopwatch
        Dim bolInfiniteMeasurements As Boolean

        If NoMeasurements = 0 Then
            NoMeasurements = NoModes
            bolInfiniteMeasurements = True
        Else
            bolInfiniteMeasurements = False
        End If

        'Startup
        sw = New Stopwatch
        If lstMeasurementData Is Nothing Then lstMeasurementData = New List(Of clsMeasurementDataContainer)
        bolMeasurementsDone = False
        ProcessingTask = Task.Factory.StartNew(AddressOf ProcessDataAndSave)
        MeasurementSetup = New clsMeasurementSetup
        MeasurementSetup.Startup()
        MeasurementSetup.SLM.lstHolograms.Clear()
        cmxRef = Nothing

        'Set up reference hologram
        RefHolo = New clsBlankHologram With {
            .bolCircularAperture = False,
            .RawWidth = MeasurementSetup.RefHoloWidth
        }
        RefHolo.LoadZernikes("D:\RPM Data Files\Tx1 Zernikes.txt")
        MeasurementSetup.SLM.lstHolograms.Add(RefHolo)

        'Set up object hologram
        ObjHolo = New clsFourierMacropixelHologram With {
            .bolCircularAperture = False,
            .RawWidth = MeasurementSetup.ObjHoloWidth,
            .intNoMeasurements = NoMeasurements,
            .intNoMacropixels = NoModes,
            .bolApplyZernikes = True,
            .dblMaxTilt = 100
        }
        ObjHolo.LoadZernikes("D:\RPM Data Files\Tx1 Zernikes.txt")
        MeasurementSetup.SLM.lstHolograms.Add(ObjHolo)

        ' Main loop
        i = 0
        sw.Start()
        While i <= NoModes - 1

            ObjHolo.MeasurementNo = i

            'Update reference
            If i Mod 50 = 0 Then
                While lstMeasurementData.Count > 50
                    Threading.Thread.Sleep(100)
                End While
                ObjHolo.bolCheckerboard = True
                RefHolo.bolCheckerboard = False
                MeasurementSetup.SLM.Refresh()
                cmxNewRef = MeasurementSetup.Camera.GetOffAxisComplexImage
                If i <> 0 Then
                    PhaseDriftCorrection.PhaseDriftCorrection(cmxRef, cmxNewRef, cmxNewRef)
                    Console.WriteLine("Reference Refreshed - " + PhaseDriftCorrection.ErrorMetric(cmxRef, cmxNewRef, cmxNewRef, 0).ToString)
                Else
                    ReDim cmxRef(cmxNewRef.GetLength(0) - 1, cmxNewRef.GetLength(1) - 1)
                End If
                Array.Copy(cmxNewRef, cmxRef, cmxNewRef.Length)
            End If

            'reference + object mode
            ObjHolo.bolCheckerboard = False
            RefHolo.bolCheckerboard = False
            MeasurementSetup.SLM.Refresh()
            intObjRef = MeasurementSetup.Camera.GetIntegerImage

            'Object mode
            ObjHolo.bolCheckerboard = False
            RefHolo.bolCheckerboard = True
            MeasurementSetup.SLM.Refresh()
            intObj = MeasurementSetup.Camera.GetIntegerImage

            'Process mode
            SyncLock lstMeasurementData
                lstMeasurementData.Add(New clsMeasurementDataContainer(ObjHolo.arrMacropixels, cmxRef, intObj, intObjRef))
            End SyncLock

            'Update GUI
            i += 1
            If bolInfiniteMeasurements = True And i > NoMeasurements - 1 Then
                i = 0
            End If
            bw.ReportProgress(CInt(i / NoModes * 1000))

        End While

        sw.Stop()
        MeasurementSetup.Shutdown()

        'Wait for data processing to complete
        bolMeasurementsDone = True
        ProcessingTask.Wait()

        Console.WriteLine("Measurement time = " + (sw.ElapsedMilliseconds / 1000 / 60).ToString("F2") + " minutes")

    End Sub
    Public Sub TMatrixMeasurement_RandomBasis(sender As Object, e As ComponentModel.DoWorkEventArgs)

        Dim RefHolo As clsBlankHologram
        Dim ObjHolo As clsMacropixelHologram
        Dim cmxRef(,) As Numerics.Complex
        Dim cmxNewRef(,) As Numerics.Complex
        Dim intObj(,) As Integer
        Dim intObjRef(,) As Integer
        Dim i As Integer
        Dim ProcessingTask As Task
        Dim sw As Stopwatch
        Dim bolInfiniteMeasurements As Boolean

        If NoMeasurements = 0 Then
            NoMeasurements = NoModes
            bolInfiniteMeasurements = True
        Else
            bolInfiniteMeasurements = False
        End If

        'Startup
        sw = New Stopwatch
        If lstMeasurementData Is Nothing Then lstMeasurementData = New List(Of clsMeasurementDataContainer)
        bolMeasurementsDone = False
        ProcessingTask = Task.Factory.StartNew(AddressOf ProcessDataAndSave)
        MeasurementSetup = New clsMeasurementSetup
        MeasurementSetup.Startup()
        MeasurementSetup.SLM.lstHolograms.Clear()
        cmxRef = Nothing

        'Set up reference hologram
        RefHolo = New clsBlankHologram With {
            .bolCircularAperture = False,
            .RawWidth = MeasurementSetup.RefHoloWidth
        }
        RefHolo.LoadZernikes("D:\RPM Data Files\Tx1 Zernikes.txt")
        MeasurementSetup.SLM.lstHolograms.Add(RefHolo)

        'Set up object hologram
        ObjHolo = New clsMacropixelHologram With {
            .bolCircularAperture = False,
            .RawWidth = MeasurementSetup.ObjHoloWidth
        }
        ReDim ObjHolo.arrMacroPixels(NoModes - 1)
        ObjHolo.LoadZernikes("D:\RPM Data Files\Tx1 Zernikes.txt")
        MeasurementSetup.SLM.lstHolograms.Add(ObjHolo)

        ' Main loop
        i = 0
        sw.Start()
        While i <= NoMeasurements - 1 Or bolInfiniteMeasurements = True

            ObjHolo.SetToRandom()

            'Refresh reference
            If i Mod 20 = 0 Then
                While lstMeasurementData.Count > 50
                    Threading.Thread.Sleep(100)
                End While
                ObjHolo.bolCheckerboard = True
                RefHolo.bolCheckerboard = False
                MeasurementSetup.SLM.Refresh()
                cmxNewRef = MeasurementSetup.Camera.GetOffAxisComplexImage
                If i <> 0 Then
                    PhaseDriftCorrection.PhaseDriftCorrection(cmxRef, cmxNewRef, cmxNewRef)
                    Console.WriteLine("Reference Refreshed - " + PhaseDriftCorrection.ErrorMetric(cmxRef, cmxNewRef, cmxNewRef, 0).ToString)
                Else
                    ReDim cmxRef(cmxNewRef.GetLength(0) - 1, cmxNewRef.GetLength(1) - 1)
                End If
                Array.Copy(cmxNewRef, cmxRef, cmxNewRef.Length)
            End If

            'reference + object mode
            ObjHolo.bolCheckerboard = False
            RefHolo.bolCheckerboard = False
            MeasurementSetup.SLM.Refresh()
            intObjRef = MeasurementSetup.Camera.GetIntegerImage

            'Object mode
            ObjHolo.bolCheckerboard = False
            RefHolo.bolCheckerboard = True
            MeasurementSetup.SLM.Refresh()
            intObj = MeasurementSetup.Camera.GetIntegerImage

            'Process mode
            SyncLock lstMeasurementData
                lstMeasurementData.Add(New clsMeasurementDataContainer(ObjHolo.arrMacroPixels, cmxRef, intObj, intObjRef))
            End SyncLock

            i += 1
            bw.ReportProgress(CInt((i Mod NoMeasurements) / NoMeasurements * 1000))

        End While

        sw.Stop()
        MeasurementSetup.Shutdown()

        'Wait for data processing to complete
        bolMeasurementsDone = True
        ProcessingTask.Wait()

        Console.WriteLine("Measurement time = " + (sw.ElapsedMilliseconds / 1000 / 60).ToString("F2") + " minutes")

    End Sub
    Public Sub TMatrixMeasurement_HadamardBasis(sender As Object, e As ComponentModel.DoWorkEventArgs)

        Dim cmxRef(,) As Numerics.Complex
        Dim cmxNewRef(,) As Numerics.Complex
        Dim intObj(,) As Integer
        Dim intObjRef(,) As Integer
        Dim i As Integer
        Dim Holo As clsMacropixelHologram
        Dim ProcessingTask As Task
        Dim sw As Stopwatch
        Dim bolInfiniteMeasurements As Boolean

        If NoMeasurements = 0 Then
            NoMeasurements = NoModes
            bolInfiniteMeasurements = True
        Else
            bolInfiniteMeasurements = False
        End If

        'Startup
        sw = New Stopwatch
        If lstMeasurementData Is Nothing Then lstMeasurementData = New List(Of clsMeasurementDataContainer)
        bolMeasurementsDone = False
        ProcessingTask = Task.Factory.StartNew(AddressOf ProcessDataAndSave)
        MeasurementSetup = New clsMeasurementSetup
        MeasurementSetup.Startup()
        MeasurementSetup.SLM.lstHolograms.Clear()
        cmxRef = Nothing

        'Set up hologram
        Holo = New clsMacropixelHologram With {
            .RawWidth = MeasurementSetup.ObjHoloWidth,
            .bolVisible = True,
            .bolCircularAperture = False
        }
        Holo.LoadZernikes("D:\RPM Data Files\Tx1 Zernikes.txt")
        MeasurementSetup.SLM.lstHolograms.Add(Holo)
        ReDim Holo.arrMacroPixels(NoModes - 1)

        ' Main loop
        i = 0
        sw.Start()
        While i <= NoMeasurements - 1 Or bolInfiniteMeasurements = True

            'Update reference?
            If i Mod 20 = 0 Then
                While lstMeasurementData.Count > 50
                    Threading.Thread.Sleep(100)
                End While
                Holo.SetToHadamard(0)
                MeasurementSetup.SLM.Refresh()
                cmxNewRef = MeasurementSetup.Camera.GetOffAxisComplexImage
                If i <> 0 Then
                    PhaseDriftCorrection.PhaseDriftCorrection(cmxRef, cmxNewRef, cmxNewRef)
                    Console.WriteLine("Reference Refreshed - " + PhaseDriftCorrection.ErrorMetric(cmxRef, cmxNewRef, cmxNewRef, 0).ToString)
                Else
                    ReDim cmxRef(cmxNewRef.GetLength(0) - 1, cmxNewRef.GetLength(1) - 1)
                End If
                Array.Copy(cmxNewRef, cmxRef, cmxNewRef.Length)
            End If

            'reference + object mode
            Holo.SetToHadamardSum(0, i Mod NoModes)
            MeasurementSetup.SLM.Refresh()
            intObjRef = MeasurementSetup.Camera.GetIntegerImage

            'Object mode
            Holo.SetToHadamard(i Mod NoModes)
            MeasurementSetup.SLM.Refresh()
            intObj = MeasurementSetup.Camera.GetIntegerImage

            'Process mode
            SyncLock lstMeasurementData
                lstMeasurementData.Add(New clsMeasurementDataContainer(Holo.arrMacroPixels, cmxRef, intObj, intObjRef))
            End SyncLock

            'Update GUI
            i += 1
            bw.ReportProgress(CInt((i Mod NoMeasurements) / NoMeasurements * 1000))

        End While

        sw.Stop()
        MeasurementSetup.Shutdown()

        'Wait for data processing to complete
        bolMeasurementsDone = True
        ProcessingTask.Wait()

        Console.WriteLine("Measurement time = " + (sw.ElapsedMilliseconds / 1000 / 60).ToString("F2") + " minutes")

    End Sub
#End Region

#Region "Data Processing"

    'Done on separate thread

    Private bolMeasurementsDone As Boolean

    Class clsMeasurementDataContainer
        Public Sub New(ByVal _arrHolo() As Double, ByVal _cmxRef(,) As Numerics.Complex,
                       ByVal _intObj(,) As Integer, ByVal _intObjRef(,) As Integer)

            ReDim arrHolo(_arrHolo.GetLength(0) - 1)
            ReDim cmxRef(_cmxRef.GetLength(0) - 1, _cmxRef.GetLength(1) - 1)
            ReDim intObj(_intObj.GetLength(0) - 1, _intObj.GetLength(1) - 1)
            ReDim intObjRef(_intObjRef.GetLength(0) - 1, _intObjRef.GetLength(1) - 1)

            Array.Copy(_arrHolo, arrHolo, _arrHolo.Length)
            Array.Copy(_cmxRef, cmxRef, _cmxRef.Length)
            Array.Copy(_intObj, intObj, _intObj.Length)
            Array.Copy(_intObjRef, intObjRef, _intObjRef.Length)

        End Sub
        Public arrHolo() As Double
        Public cmxRef(,) As Numerics.Complex
        Public intObj(,) As Integer
        Public intObjRef(,) As Integer
    End Class
    Private lstMeasurementData As List(Of clsMeasurementDataContainer)
    Private Sub ProcessDataAndSave()

        Dim cmxObj(,) As Numerics.Complex
        Dim cmxObjRef(,) As Numerics.Complex
        Dim CurrMeasurement As clsMeasurementDataContainer
        Dim Overlap As Double
        Dim MinOverlap As Double

        Dim HoloWriter As New IO.StreamWriter("Holo In.txt")
        Dim ImgWriter As New IO.StreamWriter("Image Out.txt")
        'Dim RawDataWriter As New IO.StreamWriter("Raw Images.txt")
        If lstMeasurementData Is Nothing Then lstMeasurementData = New List(Of clsMeasurementDataContainer)
        MinOverlap = 1

        'Run while measurements are still being taken.
        'Once measurements have finished, process all data in the list.
        While bolMeasurementsDone = False Or lstMeasurementData.Count > 0
            If lstMeasurementData.Count > 0 Then

                'Pop a measurement off the list.
                SyncLock lstMeasurementData
                    CurrMeasurement = lstMeasurementData.Item(0)
                    lstMeasurementData.RemoveAt(0)
                End SyncLock

                'Get complex images from any unprocessed images
                cmxObj = MeasurementSetup.Camera.CalculateOffAxisComplexImage(CurrMeasurement.intObj)
                cmxObjRef = MeasurementSetup.Camera.CalculateOffAxisComplexImage(CurrMeasurement.intObjRef)

                'Phase drift correction
                PhaseDriftCorrection.PhaseDriftCorrection(CurrMeasurement.cmxRef, cmxObj, cmxObjRef)
                Overlap = PhaseDriftCorrection.ErrorMetric(CurrMeasurement.cmxRef, cmxObj, cmxObjRef, 0)
                If Overlap < MinOverlap Then MinOverlap = Overlap
                Console.WriteLine(Overlap)

                'Save data
                HoloWriter.WriteLine(TypeConversions.HoloToString(CurrMeasurement.arrHolo))
                ImgWriter.WriteLine(TypeConversions.ImgToString(cmxObj))

                ImageProcessing.Crop(CurrMeasurement.intObj, MeasurementSetup.Camera.OffAxisImageViewport)
                'RawDataWriter.WriteLine(TypeConversions.ImgToString(CurrMeasurement.intObj))

            Else
                HoloWriter.Flush()
                ImgWriter.Flush()
                'RawDataWriter.Flush()
            End If
        End While

        'Finish up
        HoloWriter.Close()
        ImgWriter.Close()
        ' RawDataWriter.Close()
        Console.WriteLine("Min Overlap = " + CStr(MinOverlap))

    End Sub

#End Region

#Region "Hologram Generation"

    'This was an attempt at pre-computing the holograms.
    'It works, but we run out of memory.
    'It runs about 50% faster. Measurement of 36 holograms goes from ~0.6 minutes to ~0.4 minutes.
    'But, we run out of memory when we try to do more than 50 measuremnts.
    'Oh well, was worth a try.
    'I suppose we could have several tasks set up that adds to lstHologramData when the number of holograms gets low,
    '   and have it work in parallel in this manner, but this is likely not going to give an order of magnitude improvement.

    Private lstHologramData As List(Of clsHologramDataContainer)

    Public Sub TMatrixMeasurement_General(sender As Object, e As ComponentModel.DoWorkEventArgs)

        Dim ProcessingTask As Task
        Dim sw As Stopwatch
        Dim HoloData As clsHologramDataContainer

        Dim cmxRef(,) As Numerics.Complex
        Dim cmxNewRef(,) As Numerics.Complex
        Dim intObj(,) As Integer
        Dim intObjRef(,) As Integer
        Dim i As Integer

        'Startup
        sw = New Stopwatch
        If lstMeasurementData Is Nothing Then lstMeasurementData = New List(Of clsMeasurementDataContainer)
        bolMeasurementsDone = False
        ProcessingTask = Task.Factory.StartNew(AddressOf ProcessDataAndSave)
        MeasurementSetup = New clsMeasurementSetup
        MeasurementSetup.Startup()
        GenerateHadamardHolograms()
        cmxRef = Nothing

        ' Main loop
        sw.Start()
        i = 0
        While lstHologramData.Count > 0

            HoloData = lstHologramData(0)
            lstHologramData.RemoveAt(0)

            'Update reference?
            If i Mod 50 = 0 Then
                MeasurementSetup.SLM.Refresh(HoloData.RefHolo)
                cmxNewRef = MeasurementSetup.Camera.GetOffAxisComplexImage
                If i <> 0 Then
                    PhaseDriftCorrection.PhaseDriftCorrection(cmxRef, cmxNewRef, cmxNewRef)
                    Console.WriteLine("Reference Refreshed - " + PhaseDriftCorrection.ErrorMetric(cmxRef, cmxNewRef, cmxNewRef, 0).ToString)
                Else
                    ReDim cmxRef(cmxNewRef.GetLength(0) - 1, cmxNewRef.GetLength(1) - 1)
                End If
                Array.Copy(cmxNewRef, cmxRef, cmxNewRef.Length)
            End If

            'reference + object mode
            MeasurementSetup.SLM.Refresh(HoloData.ObjRefHolo)
            intObjRef = MeasurementSetup.Camera.GetIntegerImage

            'Object mode
            MeasurementSetup.SLM.Refresh(HoloData.ObjHolo)
            intObj = MeasurementSetup.Camera.GetIntegerImage

            'Process mode
            SyncLock lstMeasurementData
                lstMeasurementData.Add(New clsMeasurementDataContainer(HoloData.arrHolo, cmxRef, intObj, intObjRef))
            End SyncLock

            'Update GUI
            bw.ReportProgress(CInt(i / NoModes * 1000))
            i = i + 1

        End While

        sw.Stop()
        MeasurementSetup.Shutdown()

        'Wait for data processing to complete
        bolMeasurementsDone = True
        ProcessingTask.Wait()

        Console.WriteLine("Measurement time = " + (sw.ElapsedMilliseconds / 1000 / 60).ToString("F2") + " minutes")

    End Sub

    Private Class clsHologramDataContainer
        Public RefHolo As Bitmap
        Public ObjHolo As Bitmap
        Public ObjRefHolo As Bitmap
        Public arrHolo() As Double
    End Class

    Private Sub GenerateHadamardHolograms()

        Dim Holo As clsMacropixelHologram
        Dim bmapRef As Bitmap
        Dim bmapObj As Bitmap
        Dim bmapObjRef As Bitmap
        Dim arrHolo() As Double

        MeasurementSetup.Startup()
        lstHologramData = New List(Of clsHologramDataContainer)

        'Set up hologram
        Holo = New clsMacropixelHologram With {
            .RawWidth = MeasurementSetup.ObjHoloWidth,
            .bolVisible = True,
            .bolCircularAperture = False
        }
        Holo.LoadZernikes("D:\RPM Data Files\Tx1 Zernikes.txt")
        MeasurementSetup.SLM.lstHolograms.Add(Holo)
        ReDim Holo.arrMacroPixels(NoModes - 1)
        ReDim arrHolo(NoModes - 1)

        'reference hologram
        Holo.SetToHadamard(0)
        bmapRef = MeasurementSetup.SLM.GetCurrentBitmap

        ' Main loop
        For i = 0 To NoModes - 1

            Console.WriteLine("Generating Hologram " + i.ToString)

            'Object hologram
            Holo.SetToHadamard(i)
            bmapObj = MeasurementSetup.SLM.GetCurrentBitmap
            Array.Copy(Holo.arrMacroPixels, arrHolo, Holo.arrMacroPixels.Length)

            'reference + object hologram
            Holo.SetToHadamardSum(0, i)
            bmapObjRef = MeasurementSetup.SLM.GetCurrentBitmap

            lstHologramData.Add(New clsHologramDataContainer With {
                                .ObjHolo = bmapObj,
                                .RefHolo = bmapRef,
                                .ObjRefHolo = bmapObjRef,
                                .arrHolo = arrHolo})

        Next

    End Sub
    Private Sub GenerateRandomHolograms()

        Dim RefHolo As clsBlankHologram
        Dim ObjHolo As clsMacropixelHologram
        Dim bmapRef As Bitmap
        Dim bmapObj As Bitmap
        Dim bmapObjRef As Bitmap
        Dim arrHolo() As Double

        MeasurementSetup.Startup()
        lstHologramData = New List(Of clsHologramDataContainer)

        'Set up reference hologram
        RefHolo = New clsBlankHologram With {
            .bolCircularAperture = False,
            .RawWidth = MeasurementSetup.RefHoloWidth
        }
        RefHolo.LoadZernikes("D:\RPM Data Files\Tx1 Zernikes.txt")
        MeasurementSetup.SLM.lstHolograms.Add(RefHolo)

        'Set up object hologram
        ObjHolo = New clsMacropixelHologram With {
            .bolCircularAperture = False,
            .RawWidth = MeasurementSetup.ObjHoloWidth
        }
        ReDim ObjHolo.arrMacroPixels(NoModes - 1)
        ReDim arrHolo(NoModes - 1)
        ObjHolo.LoadZernikes("D:\RPM Data Files\Tx1 Zernikes.txt")
        MeasurementSetup.SLM.lstHolograms.Add(ObjHolo)

        'reference hologram
        ObjHolo.bolCheckerboard = True
        RefHolo.bolCheckerboard = False
        bmapRef = MeasurementSetup.SLM.GetCurrentBitmap

        ' Main loop
        For i = 0 To NoMeasurements - 1

            Console.WriteLine("Generating Hologram " + i.ToString)

            'Object hologram
            ObjHolo.SetToRandom()
            ObjHolo.bolCheckerboard = False
            RefHolo.bolCheckerboard = True
            bmapObjRef = MeasurementSetup.SLM.GetCurrentBitmap
            Array.Copy(ObjHolo.arrMacroPixels, arrHolo, ObjHolo.arrMacroPixels.Length)

            'reference + object hologram
            ObjHolo.bolCheckerboard = False
            RefHolo.bolCheckerboard = False
            bmapObj = MeasurementSetup.SLM.GetCurrentBitmap

            lstHologramData.Add(New clsHologramDataContainer With {
                    .ObjHolo = bmapObj,
                    .RefHolo = bmapRef,
                    .ObjRefHolo = bmapObjRef,
                    .arrHolo = arrHolo})

        Next

    End Sub
    Private Sub GenerateMacropixelHolograms()

        Dim Holo As clsMacropixelHologram
        Dim RefPixelNo As Integer
        Dim bmapRef As Bitmap
        Dim bmapObj As Bitmap
        Dim bmapObjRef As Bitmap
        Dim arrHolo() As Double

        'Startup
        MeasurementSetup.Startup()
        lstHologramData = New List(Of clsHologramDataContainer)
        RefPixelNo = CInt(NoModes / 2 + Math.Sqrt(NoModes) / 2)

        'Set up hologram
        Holo = New clsMacropixelHologram With {
             .RawWidth = MeasurementSetup.ObjHoloWidth,
            .bolCircularAperture = False,
            .bolVisible = True
        }
        ReDim Holo.arrMacroPixels(NoModes - 1)
        For i = 0 To NoModes - 1
            Holo.arrMacroPixels(i) = Double.NaN
        Next
        ReDim arrHolo(NoModes - 1)
        Holo.LoadZernikes("D:\RPM Data Files\Tx1 Zernikes.txt")
        MeasurementSetup.SLM.lstHolograms.Add(Holo)

        'Reference hologram
        Holo.arrMacroPixels(RefPixelNo) = 0
        bmapRef = MeasurementSetup.SLM.GetCurrentBitmap
        Holo.arrMacroPixels(RefPixelNo) = 0

        For i = 0 To NoModes - 1

            Console.WriteLine("Generating Hologram " + i.ToString)

            'Measure + store Object mode
            Holo.arrMacroPixels(i) = 0
            Holo.arrMacroPixels(RefPixelNo) = Double.NaN
            bmapObjRef = MeasurementSetup.SLM.GetCurrentBitmap
            Array.Copy(Holo.arrMacroPixels, arrHolo, Holo.arrMacroPixels.Length)

            'Measure + store reference + object mode
            Holo.arrMacroPixels(RefPixelNo) = 0
            bmapObj = MeasurementSetup.SLM.GetCurrentBitmap

            'Reset hologram
            Holo.arrMacroPixels(RefPixelNo) = Double.NaN
            Holo.arrMacroPixels(i) = Double.NaN

            lstHologramData.Add(New clsHologramDataContainer With {
                    .ObjHolo = bmapObj,
                    .RefHolo = bmapRef,
                    .ObjRefHolo = bmapObjRef,
                    .arrHolo = arrHolo})

        Next

    End Sub
    Private Sub GenerateFourierHolograms()

        Dim RefHolo As clsBlankHologram
        Dim ObjHolo As clsFourierMacropixelHologram
        Dim bmapRef As Bitmap
        Dim bmapObj As Bitmap
        Dim bmapObjRef As Bitmap
        Dim arrHolo() As Double

        'Startup
        MeasurementSetup.Startup()
        lstHologramData = New List(Of clsHologramDataContainer)

        'Set up reference hologram
        RefHolo = New clsBlankHologram With {
            .bolCircularAperture = False,
            .RawWidth = MeasurementSetup.RefHoloWidth
        }
        RefHolo.LoadZernikes("D:\RPM Data Files\Tx1 Zernikes.txt")
        MeasurementSetup.SLM.lstHolograms.Add(RefHolo)

        'Set up object hologram
        ObjHolo = New clsFourierMacropixelHologram With {
            .bolCircularAperture = False,
            .RawWidth = MeasurementSetup.ObjHoloWidth,
            .intNoMeasurements = NoMeasurements,
            .intNoMacropixels = NoModes,
            .bolApplyZernikes = True,
            .dblMaxTilt = 4
        }
        ReDim arrHolo(NoModes - 1)
        ObjHolo.LoadZernikes("D:\RPM Data Files\Tx1 Zernikes.txt")
        MeasurementSetup.SLM.lstHolograms.Add(ObjHolo)

        'Reference hologram
        ObjHolo.bolCheckerboard = True
        RefHolo.bolCheckerboard = False
        bmapRef = MeasurementSetup.SLM.GetCurrentBitmap

        ' Main loop
        For i = 0 To NoMeasurements - 1

            Console.WriteLine("Generating Hologram " + i.ToString)
            ObjHolo.MeasurementNo = i

            'Object mode
            ObjHolo.bolCheckerboard = False
            RefHolo.bolCheckerboard = True
            bmapObjRef = MeasurementSetup.SLM.GetCurrentBitmap
            Array.Copy(ObjHolo.arrMacropixels, arrHolo, ObjHolo.arrMacropixels.Length)

            'reference + object mode
            ObjHolo.bolCheckerboard = False
            RefHolo.bolCheckerboard = False
            bmapObj = MeasurementSetup.SLM.GetCurrentBitmap

            lstHologramData.Add(New clsHologramDataContainer With {
                .ObjHolo = bmapObj,
                .RefHolo = bmapRef,
                .ObjRefHolo = bmapObjRef,
                .arrHolo = arrHolo})

        Next

    End Sub
#End Region
End Module
