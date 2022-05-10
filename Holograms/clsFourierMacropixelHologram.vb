' Ralf Mouthaan
' University of Cambridge
' August 2020

Option Explicit On
Option Strict On

Public Class clsFourierMacropixelHologram
    Inherits clsHologram

    Private _dblMacroTiltx As Double
    Private _dblMacroTilty As Double
    Private _arrMacropixels As Double()
    Private _intNoMacropixels As Integer
    Private _intNoMeasurements As Integer
    Private _Holowidth_MPx As Integer
    Private _MacropixelWidth As Integer

    Public Property dblMaxTilt As Double
    Public WriteOnly Property MeasurementNo As Integer
        Set(value As Integer)

            If value > _intNoMeasurements Then Call Err.Raise(-1, "Fourier Macropixel Hologram", "Measurement No > No Measuremens")

            Dim dtilt As Double = 2 * _dblMaxTilt / (_Holowidth_MPx - 1)

            Dim idx_x As Integer = value Mod _Holowidth_MPx
            Dim idx_y As Integer = CInt(Math.Floor(value / _Holowidth_MPx))

            dblMacroTiltx = -dblMaxTilt + idx_x * dtilt
            dblMacroTilty = -dblMaxTilt + idx_y * dtilt

        End Set
    End Property
    Public Property intNoMeasurements As Integer
        Get
            Return _intNoMeasurements
        End Get
        Set(value As Integer)

            _intNoMeasurements = value
            dblMacroTiltx = _dblMacroTiltx 'Refresh arrMacropixels array

        End Set
    End Property
    Public Property dblMacroTiltx As Double
        Get
            Return _dblMacroTiltx
        End Get
        Set(value As Double)

            _dblMacroTiltx = value
            Call RefreshMacropixels()

        End Set
    End Property
    Public Property dblMacroTilty As Double
        Get
            Return _dblMacroTilty
        End Get
        Set(value As Double)

            _dblMacroTilty = value
            Call RefreshMacropixels()

        End Set
    End Property
    Public Property intNoMacropixels As Integer
        Get
            Return _intNoMacropixels
        End Get
        Set(value As Integer)

            If value = 0 Then Exit Property
            If Math.Sqrt(value) <> Int(Math.Sqrt(value)) Then
                Call Err.Raise(-1, "Fourier Macropixel Hologram", "No Macropixels must be a square number")
            End If

            _intNoMacropixels = value
            _Holowidth_MPx = CInt(Math.Sqrt(_intNoMacropixels))
            _MacropixelWidth = CInt(RawWidth / _Holowidth_MPx)

            dblMacroTiltx = _dblMacroTiltx 'Refresh arrMacropixels array

        End Set
    End Property
    Public Overrides Property RawWidth As Integer
        Get
            Return MyBase.RawWidth
        End Get
        Set(value As Integer)
            MyBase.RawWidth = value
            intNoMeasurements = intNoMeasurements 'Refresh no macropixels
        End Set
    End Property

    Public ReadOnly Property arrMacropixels As Double()
        Get
            Return _arrMacropixels
        End Get
    End Property
    Private Sub RefreshMacropixels()

        ReDim _arrMacropixels(intNoMacropixels - 1)

        For i = 0 To _Holowidth_MPx - 1
            For j = 0 To _Holowidth_MPx - 1
                _arrMacropixels(i + j * _Holowidth_MPx) = i * dblMacroTiltx * _MacropixelWidth / 10 + j * dblMacroTilty * _MacropixelWidth / 10
            Next
        Next

    End Sub

    Public Overrides Property arrRawHologram(i As Integer, j As Integer) As Double
        Get

            Dim idx As Integer '= CInt(Math.Floor(j / _MacropixelWidth) * _Holowidth_MPx + Math.Floor(i / _MacropixelWidth))

            Dim idx_x As Integer
            Dim idx_y As Integer

            idx_x = CInt(Math.Floor(j / _MacropixelWidth))
            idx_y = CInt(Math.Floor(i / _MacropixelWidth))

            If idx_x > _Holowidth_MPx - 1 Then idx_x = _Holowidth_MPx - 1
            If idx_y > _Holowidth_MPx - 1 Then idx_y = _Holowidth_MPx - 1

            idx = idx_x * _Holowidth_MPx + idx_y

            Return arrMacropixels(idx)

        End Get
        Set(value As Double)
            Throw New NotImplementedException()
        End Set
    End Property

End Class
