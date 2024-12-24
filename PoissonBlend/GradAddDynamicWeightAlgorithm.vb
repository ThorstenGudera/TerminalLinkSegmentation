Option Strict On
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows.Forms
Imports ChainCodeFinder

Public Class GradAddDynamicWeightAlgorithm
    Implements IBVectorComputingAlgorithm, IExtendedBVectorComputingAlgorithm

    Public Property LowerImgWeight As Double = 1
    Public Property UpperImgWeight As Double = 1

    Public Property MaxPixelDist As Integer Implements IExtendedBVectorComputingAlgorithm.MaxPixelDist
    Public Property Gamma As Double = 1.0 Implements IExtendedBVectorComputingAlgorithm.Gamma
    Public Property DistanceArray As Integer(,) Implements IExtendedBVectorComputingAlgorithm.DistanceArray
    Public Property MaxDistInArray As Integer Implements IExtendedBVectorComputingAlgorithm.MaxDistInArray

    Public Sub SetValuesSilent(lowerImgWeight As Double, upperImgWeight As Double) Implements IBVectorComputingAlgorithm.SetValuesSilent
        Me.LowerImgWeight = lowerImgWeight
        Me.UpperImgWeight = upperImgWeight
    End Sub

    Public Function GetValue(x As Integer, y As Integer, plane As Integer, col As Integer, colL As Integer, p() As Byte,
                             pL() As Byte, scanline As Integer, scanlineL As Integer, rc As Rectangle, center As PointF,
                             curX As Integer, curY As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue

        Dim pixelDist As Integer = Me.GetPixelDist(curX + rc.X, curY + rc.Y) '+rc.X + rc.Y für tiles
        Dim relDist As Double = Math.Pow(CDbl(pixelDist) / CDbl(Me.MaxPixelDist), Me.Gamma) ' 0.0 to 1.0++

        Dim value As Double = 0.0

        If relDist > 1.0 Then
            value = (col - p(x * 4 + y * scanline + plane)) * Me.UpperImgWeight +
                    (colL - pL((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) * Me.LowerImgWeight
        Else
            value = (col - p(x * 4 + y * scanline + plane)) * relDist * Me.UpperImgWeight +
                (colL - pL((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) * (1.0 - relDist) * Me.LowerImgWeight
        End If

        Return value
    End Function

    Public Function GetValue(x As Integer, y As Integer, x2 As Integer, y2 As Integer, plane As Integer,
                             pX() As Byte, pY() As Byte, pLX() As Byte, pLY() As Byte, scanline As Integer,
                             scanlineL As Integer, rc As Rectangle, center As PointF, j As Integer,
                             curX As Integer, curY As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue

        Dim pixelDist As Integer = Me.GetPixelDist(curX + rc.X, curY + rc.Y) '+rc.X + rc.Y für tiles
        Dim relDist As Double = Math.Pow(CDbl(pixelDist) / CDbl(Me.MaxPixelDist), Me.Gamma) ' 0.0 to 1.0++

        Dim value As Double = 0.0

        If relDist > 1.0 Then
            If j < 2 Then
                value = ((CDbl(pX(x * 4 + y * scanline + plane) - 127) - CDbl(pX(x2 * 4 + y2 * scanline + plane) - 127)) * Me.UpperImgWeight) +
                             ((CDbl(pLX((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane) - 127) - CDbl(pLX((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane) - 127)) * Me.LowerImgWeight)
            Else
                value = ((CDbl(pY(x * 4 + y * scanline + plane) - 127) - CDbl(pY(x2 * 4 + y2 * scanline + plane) - 127)) * Me.UpperImgWeight) +
                             ((CDbl(pLY((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane) - 127) - CDbl(pLY((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane) - 127)) * Me.LowerImgWeight)
            End If
        Else
            If j < 2 Then
                value = ((CDbl(pX(x * 4 + y * scanline + plane) - 127) - CDbl(pX(x2 * 4 + y2 * scanline + plane) - 127)) * relDist * Me.UpperImgWeight) +
                         ((CDbl(pLX((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane) - 127) - CDbl(pLX((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane) - 127)) * (1.0 - relDist) * Me.LowerImgWeight)
            Else
                value = ((CDbl(pY(x * 4 + y * scanline + plane) - 127) - CDbl(pY(x2 * 4 + y2 * scanline + plane) - 127)) * relDist * Me.UpperImgWeight) +
                         ((CDbl(pLY((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane) - 127) - CDbl(pLY((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane) - 127)) * (1.0 - relDist) * Me.LowerImgWeight)
            End If
        End If

        Return value / 4.0
    End Function

    Public Function GetValue(x As Integer, y As Integer, x2 As Integer, y2 As Integer, plane As Integer,
                             pX() As Short, pY() As Short, pLX() As Short, pLY() As Short, scanline As Integer,
                             scanlineL As Integer, rc As Rectangle, center As PointF, j As Integer,
                             curX As Integer, curY As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue

        Dim pixelDist As Integer = Me.GetPixelDist(curX + rc.X, curY + rc.Y) '+rc.X + rc.Y für tiles
        Dim relDist As Double = Math.Pow(CDbl(pixelDist) / CDbl(Me.MaxPixelDist), Me.Gamma) ' 0.0 to 1.0++

        Dim value As Double = 0.0

        If relDist > 1.0 Then
            If j < 2 Then
                value = ((CDbl(pX(x * 4 + y * scanline + plane)) - CDbl(pX(x2 * 4 + y2 * scanline + plane))) * Me.UpperImgWeight) +
                             ((CDbl(pLX((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) - CDbl(pLX((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane))) * Me.LowerImgWeight)
            Else
                value = ((CDbl(pY(x * 4 + y * scanline + plane)) - CDbl(pY(x2 * 4 + y2 * scanline + plane))) * Me.UpperImgWeight) +
                             ((CDbl(pLY((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) - CDbl(pLY((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane))) * Me.LowerImgWeight)
            End If
        Else
            If j < 2 Then
                value = ((CDbl(pX(x * 4 + y * scanline + plane)) - CDbl(pX(x2 * 4 + y2 * scanline + plane))) * relDist * Me.UpperImgWeight) +
                         ((CDbl(pLX((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) - CDbl(pLX((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane))) * (1.0 - relDist) * Me.LowerImgWeight)
            Else
                value = ((CDbl(pY(x * 4 + y * scanline + plane)) - CDbl(pY(x2 * 4 + y2 * scanline + plane))) * relDist * Me.UpperImgWeight) +
                         ((CDbl(pLY((x + rc.X) * 4 + (y + rc.Y) * scanlineL + plane)) - CDbl(pLY((x2 + rc.X) * 4 + (y2 + rc.Y) * scanlineL + plane))) * (1.0 - relDist) * Me.LowerImgWeight)
            End If
        End If

        Return value / 8.0
    End Function

    Public Function GetValue(x As Integer, y As Integer, x2 As Integer, y2 As Integer, plane As Integer,
                     pX() As Byte, pY() As Byte, scanline As Integer, j As Integer) As Double Implements IBVectorComputingAlgorithm.GetValue

        Dim value As Double = 0.0

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

    Public Function GetPixelDist(x As Integer, y As Integer) As Integer Implements IExtendedBVectorComputingAlgorithm.GetPixelDist
        Return Me.DistanceArray(x, y)
    End Function

    Public Function GetMaxArrayDist() As Integer
        Dim z As Integer = 0
        Dim w As Integer = Me.DistanceArray.GetLength(0)
        Dim h As Integer = Me.DistanceArray.GetLength(1)

        For y As Integer = 0 To h - 1
            For x As Integer = 0 To w - 1
                If Me.DistanceArray(x, y) > z Then
                    z = Me.DistanceArray(x, y)
                End If
            Next
        Next

        Return z
    End Function

    Public Sub Setup(bmp As Bitmap, maxPixelDist As Integer, gamma As Double) Implements IExtendedBVectorComputingAlgorithm.Setup
        fipbmp.GrayScaleImage(bmp)
        SetRegion(bmp)

        Dim fip As New fipbmp()
        Me.DistanceArray = fip.GetOutlineArrayGrayscale(bmp, 0, 10000)
        Me.MaxDistInArray = GetMaxArrayDist()
        Me.MaxPixelDist = maxPixelDist
        Me.Gamma = gamma
    End Sub

    Private Sub SetRegion(bmp As Bitmap)
        Dim bmData As BitmapData = Nothing

        If Not AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Return
        End If

        Try
            bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.[WriteOnly], PixelFormat.Format32bppArgb)

            Dim w As Integer = bmp.Width
            Dim h As Integer = bmp.Height

            Dim stride As Integer = bmData.Stride

            Dim p((bmData.Stride * bmData.Height) - 1) As Byte
            Marshal.Copy(bmData.Scan0, p, 0, p.Length)

            'For y As Integer = 0 To bmData.Height - 1
            Parallel.For(0, h, Sub(y)
                                   Dim pos As Integer = y * stride

                                   For x As Integer = 0 To w - 1
                                       Dim v As Byte = If(p(pos + 3) > 0, CByte(255), CByte(0))
                                       p(pos) = v
                                       p(pos + 1) = v
                                       p(pos + 2) = v
                                       p(pos + 3) = 255

                                       pos += 4
                                   Next
                                   'Next
                               End Sub)

            Marshal.Copy(p, 0, bmData.Scan0, p.Length)
            bmp.UnlockBits(bmData)

            p = Nothing
        Catch
            Try
                bmp.UnlockBits(bmData)

            Catch
            End Try
        End Try
    End Sub
End Class
