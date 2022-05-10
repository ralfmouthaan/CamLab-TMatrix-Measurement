' Ralf Mouthaan
' University of Cambridge
' ??? mid-2020
'
' Code for correcting phase drift.
' A measurement of a reference signal, an object signal and reference signal + object signal is taken.
' These three measurements are used to correct for any phase drift in the system.
' The measurements are not taken in this module, but the data is processed in this module.
' See my thesis for details, but essentially, it is recognised that if the phase drift is corrected for then
' ref_measured + obj_measured = (ref + obj)_measured

Option Explicit On
Option Strict On

Namespace PhaseDriftCorrection

    Module modPhaseDriftCorrection

        Public Function ErrorMetric(ByVal Img1(,) As Numerics.Complex, ByVal Img2(,) As Numerics.Complex, ByVal Img3(,) As Numerics.Complex, ByVal theta As Double) As Double

            'Calculates the overlap integral of Img1 + Img2*exp(1i*theta) and Img3.

            If UBound(Img1, 1) <> UBound(Img2, 1) Then Call Err.Raise(-1, "", "Images should be same size")
            If UBound(Img1, 1) <> UBound(Img3, 1) Then Call Err.Raise(-1, "", "Images should be same size")
            If UBound(Img1, 2) <> UBound(Img1, 2) Then Call Err.Raise(-1, "", "Images should be same size")
            If UBound(Img1, 2) <> UBound(Img3, 2) Then Call Err.Raise(-1, "", "Images should be same size")

            Dim M1(UBound(Img1, 1), UBound(Img1, 2)) As Numerics.Complex
            Dim Rotation As Numerics.Complex
            Dim RetVal As Double

            Rotation = New Numerics.Complex(Math.Cos(theta), Math.Sin(theta))

            For i = 0 To UBound(Img1, 1)
                For j = 0 To UBound(Img1, 2)
                    M1(i, j) = Img1(i, j) + Img2(i, j) * Rotation
                Next
            Next

            ImageProcessing.OverlapIntegral(M1, Img3, RetVal)
            Return RetVal

        End Function
        Public Sub PhaseDriftCorrection(ByRef Img1(,) As Numerics.Complex, ByRef Img2(,) As Numerics.Complex, ByRef Img3(,) As Numerics.Complex)

            Dim temp(3) As Double
            Dim R As Double = 0.61803399
            Dim C As Double = 1 - R
            Dim x0 As Double, x1 As Double, x2 As Double, x3 As Double
            Dim f1 As Double, f2 As Double

            ' Evaluate the function at 4 different points
            temp(0) = ErrorMetric(Img1, Img2, Img3, 0)
            temp(1) = ErrorMetric(Img1, Img2, Img3, Math.PI / 2)
            temp(2) = ErrorMetric(Img1, Img2, Img3, Math.PI)
            temp(3) = ErrorMetric(Img1, Img2, Img3, 3 * Math.PI / 2)

            ' Assign x0 to x4 depending on what the maximum was.
            For i = 0 To 3
                If temp(i) = temp.Max Then
                    x0 = (i - 1) * Math.PI / 2
                    x1 = i * Math.PI / 2
                    x3 = (i + 1) * Math.PI / 2
                    x2 = x1 + C * (x3 - x1)
                End If
            Next

            'Evaluate at x1 and x2
            f1 = ErrorMetric(Img1, Img2, Img3, x1)
            f2 = ErrorMetric(Img1, Img2, Img3, x2)

            'Main loop
            While Math.Abs(x3 - x0) > 0.001
                If f2 > f1 Then
                    x0 = x1
                    x1 = x2
                    x2 = R * x2 + C * x3
                    f1 = f2
                    f2 = ErrorMetric(Img1, Img2, Img3, x2)
                Else
                    x3 = x2
                    x2 = x1
                    x1 = R * x1 + C * x0
                    f2 = f1
                    f1 = ErrorMetric(Img1, Img2, Img3, x1)
                End If
            End While

            'Correct Img2 to be in phase with Img1
            ImageProcessing.Multiply(Img2, New Numerics.Complex(Math.Cos(x1), Math.Sin(x1)))

            'CurrOverlap = ErrorMetric(Img1, Img2, Img3, 0)
            'If Math.Abs(CurrOverlap - 1) > 0.005 Then
            ' Call Err.Raise(-1, "Could not correct phase drift. Overlap = " + CurrOverlap.ToString)
            ' End If

        End Sub

    End Module

End Namespace
