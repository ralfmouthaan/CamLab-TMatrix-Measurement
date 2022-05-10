' Ralf Mouthaan
' University of Cambridge
' June 2019
'
' Blank hologram class

Option Explicit On
Option Strict On

Public Class clsBlankHologram
    Inherits clsHologram

    Public Overrides Property arrRawHologram(i As Integer, j As Integer) As Double
        Get
            Return 0
        End Get
        Set(value As Double)
            Call Err.Raise(-1, "Blank Hologram Class", "Cannot set individual pixel values for blank hologram")
        End Set
    End Property

End Class
