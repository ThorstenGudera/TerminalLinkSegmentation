Option Strict On
Imports System.Drawing

Public Interface IExtendedBVectorComputingAlgorithm
    Property MaxPixelDist As Integer
    Property DistanceArray As Integer(,)
    Property MaxDistInArray As Integer
    Property Gamma As Double
    Function GetPixelDist(x As Integer, y As Integer) As Integer
    'Function GetMaxDistInArray() As Integer
    Sub Setup(bmp As Bitmap, maxPixelDist As Integer, gamma As Double)
End Interface
