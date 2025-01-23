Option Strict On
Imports System.Drawing

Public Class GradMaxBVectorAlgorithm
    Implements IBVectorComputingAlgorithm

    Friend fieldChangeAbs As Double = 0
    Friend fieldChangeComp As Double = 0
    Friend count As Integer = 0

    Public Sub SetValuesSilent(lowerImgWeight As Double, upperImgWeight As Double) Implements IBVectorComputingAlgorithm.SetValuesSilent

    End Sub

    Public Function GetValue(x As Integer, y As Integer, plane As Integer, col As Integer, colL As Integer, p() As Byte,
                             pL() As Byte, scanline As Integer, scanlineL As Integer, rc As Rectangle, center As PointF,
                             curX As Integer, curY As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue
        Dim v1 As Double = Math.Abs(col - CDbl(p(x * 4 + y * scanline + plane)))
        Dim v2 As Double = Math.Abs(colL - CDbl(pL((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)))
        Dim vMax As Double = Math.Max(v1, v2)

        Dim value As Double = (col - CDbl(p(x * 4 + y * scanline + plane)))

        If vMax = v2 Then
            Dim f1 As Double = value
            value = (colL - CDbl(pL((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)))

            fieldChangeComp += (value - f1)
            fieldChangeAbs += Math.Abs(value - f1)
        End If

        count += 1

        Return value
    End Function

    'following two mehtods need revision
    Public Function GetValue(x As Integer, y As Integer, x2 As Integer, y2 As Integer, plane As Integer,
                             pX() As Byte, pY() As Byte, pLX() As Byte, pLY() As Byte, scanline As Integer,
                             scanlineL As Integer, rc As Rectangle, center As PointF, j As Integer,
                             curX As Integer, curY As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue
        Dim value As Double = 0
        If j < 2 Then
            Dim v1 As Double = Math.Abs(CDbl(pX(x * 4 + y * scanline + plane)) - CDbl(pX(x2 * 4 + y2 * scanline + plane)))
            Dim v2 As Double = Math.Abs(CDbl(pLX((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) - CDbl(pLX((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane)))

            Dim vMax As Double = Math.Max(v1, v2)

            value = (CDbl(pX(x * 4 + y * scanline + plane)) - CDbl(pX(x2 * 4 + y2 * scanline + plane))) / 2.0

            If vMax = v2 Then
                Dim f1 As Double = value
                value = (CDbl(pLX((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) - CDbl(pLX((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane))) / 2.0 * value / 127.0

                fieldChangeComp += (value - f1)
                fieldChangeAbs += Math.Abs(value - f1)
            End If
        Else
            Dim v1 As Double = Math.Abs(CDbl(pY(x * 4 + y * scanline + plane)) - CDbl(pY(x2 * 4 + y2 * scanline + plane)))
            Dim v2 As Double = Math.Abs(CDbl(pLY((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) - CDbl(pLY((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane)))

            Dim vMax As Double = Math.Max(v1, v2)

            value = (CDbl(pY(x * 4 + y * scanline + plane)) - CDbl(pY(x2 * 4 + y2 * scanline + plane))) / 2.0

            If vMax = v2 Then
                Dim f1 As Double = value
                value = (CDbl(pLY((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) - CDbl(pLY((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane))) / 2.0 * value / 127.0

                fieldChangeComp += (value - f1)
                fieldChangeAbs += Math.Abs(value - f1)
            End If
        End If

        count += 1

        Return value / 4.0
    End Function

    Public Function GetValue(x As Integer, y As Integer, x2 As Integer, y2 As Integer, plane As Integer,
                         pX() As Short, pY() As Short, pLX() As Short, pLY() As Short, scanline As Integer,
                         scanlineL As Integer, rc As Rectangle, center As PointF, j As Integer,
                         curX As Integer, curY As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue
        Dim value As Double = 0
        If j < 2 Then
            Dim v1 As Double = Math.Abs((CDbl(pX(x * 4 + y * scanline + plane))) - (CDbl(pX(x2 * 4 + y2 * scanline + plane))))
            Dim v2 As Double = Math.Abs((CDbl(pLX((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane))) - (CDbl(pLX((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane))))

            Dim vMax As Double = Math.Max(v1, v2)

            value = (CDbl(pX(x * 4 + y * scanline + plane)) - CDbl(pX(x2 * 4 + y2 * scanline + plane))) / 2.0

            If vMax = v2 Then
                Dim f1 As Double = value
                value = (CDbl(pLX((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) - CDbl(pLX((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane))) / 2.0

                fieldChangeComp += (value - f1)
                fieldChangeAbs += Math.Abs(value - f1)
            End If
        Else
            Dim v1 As Double = Math.Abs((CDbl(pY(x * 4 + y * scanline + plane))) - (CDbl(pY(x2 * 4 + y2 * scanline + plane))))
            Dim v2 As Double = Math.Abs((CDbl(pLY((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane))) - (CDbl(pLY((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane))))

            Dim vMax As Double = Math.Max(v1, v2)

            value = (CDbl(pY(x * 4 + y * scanline + plane)) - CDbl(pY(x2 * 4 + y2 * scanline + plane))) / 2.0

            If vMax = v2 Then
                Dim f1 As Double = value
                value = (CDbl(pLY((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) - CDbl(pLY((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane))) / 2.0

                fieldChangeComp += (value - f1)
                fieldChangeAbs += Math.Abs(value - f1)
            End If
        End If

        count += 1

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
