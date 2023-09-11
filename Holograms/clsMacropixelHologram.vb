' Ralf Mouthaan
' University of Cambridge
' March 2020

Option Explicit On
Option Strict On

Public Class clsMacropixelHologram
    Inherits clsHologram

    Private _arrMacropixels As Double()
    Private _NoMacropixels As Integer
    Private _Holowidth_MPx As Integer
    Private _MacropixelWidth As Integer
    Private _arrHadamard(,) As Integer

    Public Property arrMacroPixels As Double()
        Get
            Return _arrMacropixels
        End Get
        Set(value As Double())
            If value Is Nothing Then Exit Property
            If value.Count = 0 Then Exit Property
            _arrMacropixels = value
            _NoMacropixels = arrMacroPixels.GetLength(0)
            _MacropixelWidth = CInt(Math.Round(RawWidth / Math.Sqrt(_NoMacropixels)))
            _Holowidth_MPx = CInt(Math.Sqrt(_NoMacropixels))
        End Set
    End Property
    Public Overrides Property RawWidth As Integer
        Get
            Return MyBase.RawWidth
        End Get
        Set(value As Integer)
            MyBase.RawWidth = value
            arrMacroPixels = _arrMacropixels
        End Set
    End Property
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

            Return _arrMacropixels(idx)

        End Get
        Set(value As Double)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Sub SetToRandom()

        Static Rnd As New Random

        For i = 0 To arrMacroPixels.Length - 1
            If Rnd.NextDouble <= 1 Then
                arrMacroPixels(i) = 2 * Math.PI * Rnd.NextDouble
            Else
                arrMacroPixels(i) = Double.NaN
            End If
        Next

    End Sub
    Public Sub SetToHadamard(ByVal RowNo As Integer)

        'RowNo is the row we wish to extract. NB this is base-zero, so the first row is row 0.

        If _arrHadamard Is Nothing Then PopulateHadamardArray()
        If _arrHadamard.GetLength(0) < RowNo Then PopulateHadamardArray()
        If _arrHadamard.GetLength(0) < RowNo Then Call Err.Raise(-1, "Get Hadamard Row", "Seem to be accessing a row > max number of rows")

        For i = 0 To arrMacroPixels.Count - 1
            arrMacroPixels(i) = _arrHadamard(RowNo, i) * Math.PI / 2
        Next

    End Sub
    Public Sub SetToHadamardSum(ByVal RowNo1 As Integer, ByVal RowNo2 As Integer)

        If _arrHadamard Is Nothing Then PopulateHadamardArray()
        If _arrHadamard.GetLength(0) < RowNo1 Then PopulateHadamardArray()
        If _arrHadamard.GetLength(0) < RowNo1 Then Call Err.Raise(-1, "Get Hadamard Row", "Seem to be accessing a row > max number of rows")
        If _arrHadamard.GetLength(0) < RowNo2 Then PopulateHadamardArray()
        If _arrHadamard.GetLength(0) < RowNo2 Then Call Err.Raise(-1, "Get Hadamard Row", "Seem to be accessing a row > max number of rows")

        For i = 0 To arrMacroPixels.Count - 1

            If _arrHadamard(RowNo1, i) <> _arrHadamard(RowNo2, i) Then
                arrMacroPixels(i) = Double.NaN
            Else
                arrMacroPixels(i) = _arrHadamard(RowNo1, i) * Math.PI / 2
            End If

        Next

    End Sub
    Private Sub PopulateHadamardArray()

        Dim reader As New IO.StreamReader("C:\Instrument Setup\Hadamard.txt")
        Dim splitstr() As String

        ReDim _arrHadamard(NoModes - 1, NoModes - 1)

        For i = 0 To NoModes - 1
            splitstr = Split(reader.ReadLine, ",")
            For j = 0 To NoModes - 1
                _arrHadamard(i, j) = CInt(splitstr(j))
            Next
        Next

        reader.Close()

    End Sub
    Public Sub LoadFromFile(ByVal Filename As String)

        Dim reader As New System.IO.StreamReader(Filename)

        arrMacroPixels = TypeConversions.StringToHolo(reader.ReadLine)

        reader.Close()

    End Sub

End Class
