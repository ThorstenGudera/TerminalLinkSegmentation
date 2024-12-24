Option Strict On
Imports System.Drawing

Public Class GradAddBVectorAlgorithm
    Implements IBVectorComputingAlgorithm

    Public Sub SetValuesSilent(lowerImgWeight As Double, upperImgWeight As Double) Implements IBVectorComputingAlgorithm.SetValuesSilent

    End Sub

    Public Function GetValue(x As Integer, y As Integer, plane As Integer, col As Integer, colL As Integer, p() As Byte,
                             pL() As Byte, scanline As Integer, scanlineL As Integer, rc As Rectangle, center As PointF,
                             curX As Integer, curY As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue

        Dim value As Double = (((col - CDbl(p(x * 4 + y * scanline + plane))) * 0.5) +
                     ((colL - CDbl(pL((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane))) * 0.5))

        Return value
    End Function

    Public Function GetValue(x As Integer, y As Integer, x2 As Integer, y2 As Integer, plane As Integer,
                             pX() As Byte, pY() As Byte, pLX() As Byte, pLY() As Byte, scanline As Integer,
                             scanlineL As Integer, rc As Rectangle, center As PointF, j As Integer,
                             curX As Integer, curY As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue
        Dim value As Double = 0

        If j < 2 Then
            value = (((CDbl(pX(x * 4 + y * scanline + plane) - 127) - CDbl(pX(x2 * 4 + y2 * scanline + plane) - 127)) * 0.5) +
                     ((CDbl(pLX((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane) - 127) - CDbl(pLX((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane) - 127)) * 0.5))
        Else
            value = (((CDbl(pY(x * 4 + y * scanline + plane) - 127) - CDbl(pY(x2 * 4 + y2 * scanline + plane) - 127)) * 0.5) +
                     ((CDbl(pLY((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane) - 127) - CDbl(pLY((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane) - 127)) * 0.5))
        End If

        Return value / 4.0
    End Function

    Public Function GetValue(x As Integer, y As Integer, x2 As Integer, y2 As Integer, plane As Integer,
                             pX() As Short, pY() As Short, pLX() As Short, pLY() As Short, scanline As Integer,
                             scanlineL As Integer, rc As Rectangle, center As PointF, j As Integer,
                             curX As Integer, curY As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue
        Dim value As Double = 0

        If j < 2 Then
            value = (((CDbl(pX(x * 4 + y * scanline + plane)) - CDbl(pX(x2 * 4 + y2 * scanline + plane))) * 0.5) +
                         ((CDbl(pLX((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) - CDbl(pLX((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane))) * 0.5))
        Else
            value = (((CDbl(pY(x * 4 + y * scanline + plane)) - CDbl(pY(x2 * 4 + y2 * scanline + plane))) * 0.5) +
                         ((CDbl(pLY((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) - CDbl(pLY((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane))) * 0.5))
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

    Public Function GetValue(x As Integer, y As Integer, x2 As Integer, y2 As Integer, plane As Integer, pX() As Byte, pY() As Byte, pB() As Byte, scanline As Integer, scanlineL As Integer, rc As Rectangle, ptC As PointF, j As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue
        Dim value As Double = 0
        If j < 2 Then
            value = ((CDbl(pX(x * 4 + y * scanline + plane) - 127) - CDbl(pX(x2 * 4 + y2 * scanline + plane) - 127) * 0.5) +
                         ((CDbl(pB((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane) - 127) - CDbl(pB((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane) - 127)) * 0.5))
        Else
            value = ((CDbl(pY(x * 4 + y * scanline + plane) - 127) - CDbl(pY(x2 * 4 + y2 * scanline + plane) - 127) * 0.5) +
                         ((CDbl(pB((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane) - 127) - CDbl(pB((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane) - 127)) * 0.5))
        End If

        Return value
    End Function
End Class
