Option Strict On
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports ChainCodeFinder

Public Class GradAddFixedWeightBVectorAlgorithm
    Implements IBVectorComputingAlgorithm

    Public Property LowerImgWeight As Double = 1
    Public Property UpperImgWeight As Double = 1

    Public Sub SetValuesSilent(lowerImgWeight As Double, upperImgWeight As Double) Implements IBVectorComputingAlgorithm.SetValuesSilent
        Me.LowerImgWeight = lowerImgWeight
        Me.UpperImgWeight = upperImgWeight
    End Sub

    Public Function GetValue(x As Integer, y As Integer, plane As Integer, col As Integer, colL As Integer, p() As Byte,
                             pL() As Byte, scanline As Integer, scanlineL As Integer, rc As Rectangle, center As PointF,
                             curX As Integer, curY As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue
        Dim value As Double = (col - p(x * 4 + y * scanline + plane)) * Me.UpperImgWeight +
                (colL - pL((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) * Me.LowerImgWeight

        Return value
    End Function

    Public Function GetValue(x As Integer, y As Integer, x2 As Integer, y2 As Integer, plane As Integer,
                             pX() As Byte, pY() As Byte, pLX() As Byte, pLY() As Byte, scanline As Integer,
                             scanlineL As Integer, rc As Rectangle, center As PointF, j As Integer,
                             curX As Integer, curY As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue
        Dim value As Double = 0

        If j < 2 Then
            value = ((CDbl(pX(x * 4 + y * scanline + plane) - 127) - CDbl(pX(x2 * 4 + y2 * scanline + plane) - 127)) * Me.UpperImgWeight) +
                         ((CDbl(pLX((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane) - 127) - CDbl(pLX((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane) - 127)) * Me.LowerImgWeight)
        Else
            value = ((CDbl(pY(x * 4 + y * scanline + plane) - 127) - CDbl(pY(x2 * 4 + y2 * scanline + plane) - 127)) * Me.UpperImgWeight) +
                         ((CDbl(pLY((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane) - 127) - CDbl(pLY((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane) - 127)) * Me.LowerImgWeight)
        End If

        Return value / 4.0
    End Function

    Public Function GetValue(x As Integer, y As Integer, x2 As Integer, y2 As Integer, plane As Integer,
                             pX() As Short, pY() As Short, pLX() As Short, pLY() As Short, scanline As Integer,
                             scanlineL As Integer, rc As Rectangle, center As PointF, j As Integer,
                             curX As Integer, curY As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue
        Dim value As Double = 0

        If j < 2 Then
            value = ((CDbl(pX(x * 4 + y * scanline + plane)) - CDbl(pX(x2 * 4 + y2 * scanline + plane))) * Me.UpperImgWeight) +
                         ((CDbl(pLX((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) - CDbl(pLX((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane))) * Me.LowerImgWeight)
        Else
            value = ((CDbl(pY(x * 4 + y * scanline + plane)) - CDbl(pY(x2 * 4 + y2 * scanline + plane))) * Me.UpperImgWeight) +
                         ((CDbl(pLY((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) - CDbl(pLY((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane))) * Me.LowerImgWeight)
        End If

        Return value / 8.0
    End Function

    Public Function GetValue(x As Integer, y As Integer, x2 As Integer, y2 As Integer, plane As Integer,
                     pX() As Byte, pY() As Byte, scanline As Integer, j As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue
        Dim value As Double = 0
        If j < 2 Then
            value = (CDbl(pX(x * 4 + y * scanline + plane) - 127) - CDbl(pX(x2 * 4 + y2 * scanline + plane) - 127))
        Else
            value = (CDbl(pY(x * 4 + y * scanline + plane) - 127) - CDbl(pY(x2 * 4 + y2 * scanline + plane) - 127))
        End If

        Return value / 4.0
    End Function

    Public Function GetValue(xA1 As Integer, yA1 As Integer, xA2 As Integer, yA2 As Integer, plane As Integer, pX() As Byte, pY() As Byte, pB() As Byte, scanline As Integer, scanlineL As Integer, rc As Rectangle, ptC As PointF, j As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue
        Throw New NotImplementedException()
    End Function
End Class

