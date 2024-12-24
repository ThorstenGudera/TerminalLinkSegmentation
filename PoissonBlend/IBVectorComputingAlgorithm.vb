Option Strict On
Imports System.Drawing

Public Interface IBVectorComputingAlgorithm
    Function GetValue(x As Integer, y As Integer, plane As Integer, col As Integer, colL As Integer,
                      p As Byte(), pL As Byte(), scanline As Integer, scanlineL As Integer, rc As Rectangle, center As PointF,
                      curX As Integer, curY As Integer) As Double

    Function GetValue(x As Integer, y As Integer, x2 As Integer, y2 As Integer, plane As Integer,
                      pX As Byte(), pY As Byte(), pLX As Byte(), pLY As Byte(), scanline As Integer, scanlineL As Integer,
                      rc As Rectangle, center As PointF, j As Integer, curX As Integer, curY As Integer) As Double

    Function GetValue(x As Integer, y As Integer, x2 As Integer, y2 As Integer, plane As Integer,
                      pX As Short(), pY As Short(), pLX As Short(), pLY As Short(), scanline As Integer, scanlineL As Integer,
                      rc As Rectangle, center As PointF, j As Integer, curX As Integer, curY As Integer) As Double

    Sub SetValuesSilent(lowerImgWeight As Double, upperImgWeight As Double)

    'temp
    Function GetValue(xA1 As Integer, yA1 As Integer, xA2 As Integer, yA2 As Integer, plane As Integer, pX() As Byte, pY() As Byte, scanline As Integer, j As Integer) As Double
    Function GetValue(xA1 As Integer, yA1 As Integer, xA2 As Integer, yA2 As Integer, plane As Integer, pX() As Byte, pY() As Byte, pB() As Byte, scanline As Integer, scanlineL As Integer, rc As Rectangle, ptC As PointF, j As Integer) As Double
End Interface
