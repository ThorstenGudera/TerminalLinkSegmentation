Option Strict On

Imports System.Runtime.InteropServices
Imports System.Drawing.Imaging
Imports System.Drawing
Imports System.Threading.Tasks

Public Class fipbmp
    Shared Sub GradColors(bmp As Bitmap, redValues As Byte(), greenValues As Byte(), blueValues As Byte())
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Dim bmData As BitmapData = Nothing

            Try
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim stride As Integer = bmData.Stride
                Dim Scan0 As System.IntPtr = bmData.Scan0

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)

                Parallel.[For](0, nHeight, Sub(y)
                                               'For y As Integer = 0 To nHeight - 1
                                               Dim pos As Integer = y * stride

                                               For x As Integer = 0 To nWidth - 1
                                                   Dim value As Integer = redValues(p(pos + 2))
                                                   If value < 0 Then
                                                       value = 0
                                                   End If
                                                   If value > 255 Then
                                                       value = 255
                                                   End If
                                                   p(pos + 2) = CByte(value)

                                                   Dim value2 As Integer = greenValues(p(pos + 1))
                                                   If value2 < 0 Then
                                                       value2 = 0
                                                   End If
                                                   If value2 > 255 Then
                                                       value2 = 255
                                                   End If
                                                   p(pos + 1) = CByte(value2)

                                                   Dim value3 As Integer = blueValues(p(pos))
                                                   If value3 < 0 Then
                                                       value3 = 0
                                                   End If
                                                   If value3 > 255 Then
                                                       value3 = 255
                                                   End If
                                                   p(pos) = CByte(value3)

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
        End If
    End Sub

    Shared Sub GradColors(bmp As Bitmap, redValues As Byte(), greenValues As Byte(), blueValues As Byte(), fColor As Color, tolerance As Integer)
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Dim bmData As BitmapData = Nothing

            Try
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim stride As Integer = bmData.Stride
                Dim Scan0 As System.IntPtr = bmData.Scan0

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)

                Parallel.[For](0, nHeight, Sub(y)
                                               'For y As Integer = 0 To nHeight - 1
                                               Dim pos As Integer = y * stride

                                               For x As Integer = 0 To nWidth - 1
                                                   If (p(pos + 3) = 0 AndAlso fColor.A = 0) OrElse
                                                       ((p(pos + 3) >= (CInt(fColor.A) - tolerance)) AndAlso (CInt(p(pos + 3)) <= (CInt(fColor.A) + tolerance)) _
                                                        AndAlso (CInt(p(pos + 2)) >= (CInt(fColor.R) - tolerance)) AndAlso (CInt(p(pos + 2)) <= (CInt(fColor.R) + tolerance)) _
                                                        AndAlso (CInt(p(pos + 1)) >= (CInt(fColor.G) - tolerance)) AndAlso (CInt(p(pos + 1)) <= (CInt(fColor.G) + tolerance)) _
                                                        AndAlso (CInt(p(pos)) >= (CInt(fColor.B) - tolerance)) AndAlso (CInt(p(pos)) <= (CInt(fColor.B) + tolerance))) Then

                                                       Dim value As Integer = redValues(p(pos + 2))
                                                       If value < 0 Then
                                                           value = 0
                                                       End If
                                                       If value > 255 Then
                                                           value = 255
                                                       End If
                                                       p(pos + 2) = CByte(value)

                                                       Dim value2 As Integer = greenValues(p(pos + 1))
                                                       If value2 < 0 Then
                                                           value2 = 0
                                                       End If
                                                       If value2 > 255 Then
                                                           value2 = 255
                                                       End If
                                                       p(pos + 1) = CByte(value2)

                                                       Dim value3 As Integer = blueValues(p(pos))
                                                       If value3 < 0 Then
                                                           value3 = 0
                                                       End If
                                                       If value3 > 255 Then
                                                           value3 = 255
                                                       End If
                                                       p(pos) = CByte(value3)
                                                   End If
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
        End If
    End Sub


    Public Shared Sub gradColors(bmp As Bitmap, lumValues As Integer(), Multiplicator As Integer)
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Dim bmData As BitmapData = Nothing

            Try
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim stride As Integer = bmData.Stride
                Dim Scan0 As System.IntPtr = bmData.Scan0

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)

                Parallel.[For](0, nHeight, Sub(y)
                                               'For y As Integer = 0 To nHeight - 1
                                               Dim pos As Integer = y * stride

                                               For x As Integer = 0 To nWidth - 1
                                                   Dim hsl As HSLData = RGBtoHSL(CInt(p(pos + 2)), CInt(p(pos + 1)), CInt(p(pos)))

                                                   Dim lValue As Double = hsl.Luminance * Multiplicator
                                                   Dim rest As Double = lValue - CInt(lValue)

                                                   Dim lTmpValue As Integer = CInt(lValue * (CDbl(lumValues.Length - 1) / CDbl(Multiplicator)))

                                                   If lTmpValue < 0 Then
                                                       lTmpValue = 0
                                                   End If
                                                   If lTmpValue > lumValues.Length - 1 Then
                                                       lTmpValue = lumValues.Length - 1
                                                   End If
                                                   lTmpValue = lumValues(lTmpValue)

                                                   Dim val_luminance As Single = CSng(CDbl(lTmpValue) * (CDbl(Multiplicator) / CDbl(lumValues.Length)) + rest)

                                                   val_luminance /= Multiplicator

                                                   If val_luminance > 1.0F Then
                                                       val_luminance = 1.0F
                                                   End If

                                                   If val_luminance < 0.0F Then
                                                       val_luminance = 0.0F
                                                   End If

                                                   Dim nPixel As PixelData = HSLtoRGB(hsl.Hue, hsl.Saturation, val_luminance)

                                                   Dim value As Integer = nPixel.red
                                                   If value < 0 Then
                                                       value = 0
                                                   End If
                                                   If value > 255 Then
                                                       value = 255
                                                   End If
                                                   p(pos + 2) = CByte(value)
                                                   Dim value2 As Integer = nPixel.green
                                                   If value2 < 0 Then
                                                       value2 = 0
                                                   End If
                                                   If value2 > 255 Then
                                                       value2 = 255
                                                   End If
                                                   p(pos + 1) = CByte(value2)
                                                   Dim value3 As Integer = nPixel.blue
                                                   If value3 < 0 Then
                                                       value3 = 0
                                                   End If
                                                   If value3 > 255 Then
                                                       value3 = 255
                                                   End If
                                                   p(pos) = CByte(value3)
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
        End If
    End Sub

    Public Shared Function RGBtoHSL(Red As Integer, Green As Integer, Blue As Integer) As HSLData
        Dim hsl As New HSLData()

        Dim c As Color = Color.FromArgb(255, Red, Green, Blue)
        hsl.Hue = c.GetHue()
        hsl.Saturation = c.GetSaturation()
        hsl.Luminance = c.GetBrightness()

        Return hsl
    End Function

    Public Shared Function HSLtoRGB(H As Double, S As Double, L As Double) As PixelData
        Dim Temp1 As Double = 0.0, Temp2 As Double = 0.0
        Dim r As Double = 0.0, g As Double = 0.0, b As Double = 0.0

        If S = 0 Then
            r = L
            g = L
            b = L
        Else
            If L < 0.5 Then
                Temp2 = L * (1.0 + S)
            Else
                Temp2 = (L + S) - (S * L)
            End If

            Temp1 = 2.0 * L - Temp2

            'bischen Spaghetti hier, evtl. in eigene Funktion auslagern

            Dim hTmp As Double = H / 360.0
            Dim rTmp As Double, gTmp As Double, bTmp As Double

            rTmp = hTmp + (1.0 / 3.0)
            gTmp = hTmp
            bTmp = hTmp - (1.0 / 3.0)

            If rTmp < 0.0 Then
                rTmp += 1.0
            End If
            If gTmp < 0.0 Then
                gTmp += 1.0
            End If
            If bTmp < 0.0 Then
                bTmp += 1.0
            End If

            If rTmp > 1.0 Then
                rTmp -= 1.0
            End If
            If gTmp > 1.0 Then
                gTmp -= 1.0
            End If
            If bTmp > 1.0 Then
                bTmp -= 1.0
            End If

            If 6.0 * rTmp < 1.0 Then
                r = Temp1 + (Temp2 - Temp1) * 6.0 * rTmp
            ElseIf 2.0 * rTmp < 1.0 Then
                r = Temp2
            ElseIf 3.0 * rTmp < 2.0 Then
                r = Temp1 + (Temp2 - Temp1) * ((2.0 / 3.0) - rTmp) * 6.0
            Else
                r = Temp1
            End If

            If 6.0 * gTmp < 1.0 Then
                g = Temp1 + (Temp2 - Temp1) * 6.0 * gTmp
            ElseIf 2.0 * gTmp < 1.0 Then
                g = Temp2
            ElseIf 3.0 * gTmp < 2.0 Then
                g = Temp1 + (Temp2 - Temp1) * ((2.0 / 3.0) - gTmp) * 6.0
            Else
                g = Temp1
            End If

            If 6.0 * bTmp < 1.0 Then
                b = Temp1 + (Temp2 - Temp1) * 6.0 * bTmp
            ElseIf 2.0 * bTmp < 1.0 Then
                b = Temp2
            ElseIf 3.0 * bTmp < 2.0 Then
                b = Temp1 + (Temp2 - Temp1) * ((2.0 / 3.0) - bTmp) * 6.0
            Else
                b = Temp1
            End If
        End If

        Dim RGB As New PixelData()

        r *= 255.0
        g *= 255.0
        b *= 255.0

        RGB.red = CByte(CInt(r))
        RGB.green = CByte(CInt(g))
        RGB.blue = CByte(CInt(b))

        Return RGB
    End Function

    Public Shared Function ScrollToPic(bmp As Bitmap) As Point
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            ' GDI+ still lies to us - the return format is BGR, NOT RGB.
            Dim bmData As BitmapData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

            Dim scanline As Integer = bmData.Stride

            Dim Scan0 As System.IntPtr = bmData.Scan0

            Dim lefttop As New Point()
            Dim complete As Boolean = False

            Dim p((bmData.Stride * bmData.Height) - 1) As Byte
            Marshal.Copy(bmData.Scan0, p, 0, p.Length)

            Dim pos As Integer = 0

            For y As Integer = 0 To bmp.Height - 1
                For x As Integer = 0 To bmp.Width - 1
                    If p(pos + 3) <> 0 Then
                        lefttop = New Point(x, y)
                        complete = True
                        Exit For
                    End If

                    pos += 4
                Next
                If complete Then
                    Exit For
                End If
            Next

            Marshal.Copy(p, 0, bmData.Scan0, p.Length)
            bmp.UnlockBits(bmData)

            p = Nothing

            Return lefttop
        End If

        Return New Point(0, 0)
    End Function

    Public Shared Sub Hue(bmp As Bitmap, fHue As Single)
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Dim nWidth As Integer = bmp.Width
            Dim nHeight As Integer = bmp.Height

            Dim bmData As BitmapData = Nothing

            Try
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                Dim scan0 As IntPtr = bmData.Scan0
                Dim stride As Integer = bmData.Stride

                Dim pf((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, pf, 0, pf.Length)

                Dim pos As Integer = 0

                'for (int y = 0; y < size.Y; y++)
                Parallel.[For](0, nHeight, Sub(y)
                                               Dim pfpos As Integer = y * stride
                                               For x As Integer = 0 To nWidth - 1
                                                   Dim red As Byte = pf(pfpos + 2)
                                                   Dim green As Byte = pf(pfpos + 1)
                                                   Dim blue As Byte = pf(pfpos)

                                                   Dim p As HSLData = RGBtoHSL(red, green, blue)
                                                   Dim val_hue As Single = fHue + p.Hue

                                                   If val_hue > 360 Then
                                                       val_hue -= 360
                                                   End If

                                                   Dim nPixel As PixelData = HSLtoRGB(val_hue, p.Saturation, p.Luminance)

                                                   pf(pfpos + 2) = nPixel.red
                                                   pf(pfpos + 1) = nPixel.green
                                                   pf(pfpos) = nPixel.blue

                                                   pfpos += 4
                                               Next

                                           End Sub)

                Marshal.Copy(pf, 0, bmData.Scan0, pf.Length)
                bmp.UnlockBits(bmData)

                pf = Nothing
            Catch
                Try
                    bmp.UnlockBits(bmData)

                Catch
                End Try
            End Try
        End If
    End Sub

    Public Shared Sub Saturation(bmp As Bitmap, fSaturation As Single)
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Dim nWidth As Integer = bmp.Width
            Dim nHeight As Integer = bmp.Height

            Dim bmData As BitmapData = Nothing

            Try
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                Dim scan0 As IntPtr = bmData.Scan0
                Dim stride As Integer = bmData.Stride

                Dim pf((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, pf, 0, pf.Length)

                Dim pos As Integer = 0

                'for (int y = 0; y < size.Y; y++)
                Parallel.[For](0, nHeight, Sub(y)
                                               Dim pfpos As Integer = y * stride
                                               For x As Integer = 0 To nWidth - 1
                                                   Dim red As Byte = pf(pfpos + 2)
                                                   Dim green As Byte = pf(pfpos + 1)
                                                   Dim blue As Byte = pf(pfpos)

                                                   Dim p As HSLData = RGBtoHSL(red, green, blue)
                                                   Dim val_saturation As Single = fSaturation + p.Saturation

                                                   If val_saturation > 1.0F Then
                                                       val_saturation = 1.0F
                                                   End If

                                                   If val_saturation < 0F Then
                                                       val_saturation = 0F
                                                   End If

                                                   Dim nPixel As PixelData = HSLtoRGB(p.Hue, val_saturation, p.Luminance)

                                                   pf(pfpos + 2) = nPixel.red
                                                   pf(pfpos + 1) = nPixel.green
                                                   pf(pfpos) = nPixel.blue

                                                   pfpos += 4
                                               Next

                                           End Sub)
                Marshal.Copy(pf, 0, bmData.Scan0, pf.Length)
                bmp.UnlockBits(bmData)

                pf = Nothing
            Catch
                Try
                    bmp.UnlockBits(bmData)

                Catch
                End Try
            End Try
        End If
    End Sub

    Public Shared Sub Luminance(bmp As Bitmap, fLuminance As Single)
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Dim nWidth As Integer = bmp.Width
            Dim nHeight As Integer = bmp.Height

            Dim bmData As BitmapData = Nothing

            Try
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                Dim scan0 As IntPtr = bmData.Scan0
                Dim stride As Integer = bmData.Stride

                Dim pf((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, pf, 0, pf.Length)

                Dim pos As Integer = 0

                'for (int y = 0; y < size.Y; y++)
                Parallel.[For](0, nHeight, Sub(y)
                                               Dim pfpos As Integer = y * stride
                                               For x As Integer = 0 To nWidth - 1
                                                   Dim red As Byte = pf(pfpos + 2)
                                                   Dim green As Byte = pf(pfpos + 1)
                                                   Dim blue As Byte = pf(pfpos)

                                                   Dim p As HSLData = RGBtoHSL(red, green, blue)
                                                   Dim val_luminance As Single = fLuminance + p.Luminance

                                                   If val_luminance > 1.0F Then
                                                       val_luminance = 1.0F
                                                   End If

                                                   If val_luminance < 0F Then
                                                       val_luminance = 0F
                                                   End If

                                                   Dim nPixel As PixelData = HSLtoRGB(p.Hue, p.Saturation, val_luminance)

                                                   pf(pfpos + 2) = nPixel.red
                                                   pf(pfpos + 1) = nPixel.green
                                                   pf(pfpos) = nPixel.blue

                                                   pfpos += 4
                                               Next

                                           End Sub)
                Marshal.Copy(pf, 0, bmData.Scan0, pf.Length)
                bmp.UnlockBits(bmData)

                pf = Nothing
            Catch
                Try
                    bmp.UnlockBits(bmData)

                Catch
                End Try
            End Try
        End If
    End Sub

    Public Shared Sub Bereich(bmp As Bitmap, HueMin As Single, HueMax As Single, Hue As Single, Saturation As Single, Luminance As Single, AddSaturation As Boolean,
        AddLuminance As Boolean, SaturationMin As Single, SaturationMax As Single, LuminanceMin As Single, LuminanceMax As Single, setAlpha As Boolean, alphaVal As Integer,
                              addAlpha As Boolean, useRamp As Boolean, rampGamma As Double)
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Dim nWidth As Integer = bmp.Width
            Dim nHeight As Integer = bmp.Height

            Dim bmData As BitmapData = Nothing

            Dim dist As Integer = CInt(Math.Ceiling(HueMax - HueMin))

            If (dist And &H1) = 1 Then
                dist += 1
            End If

            If HueMin > HueMax Then
                dist += 360
            End If

            Dim hueRamp(dist) As Double
            For i As Integer = 0 To hueRamp.Length - 1
                hueRamp(i) = 1
            Next

            If useRamp Then
                Dim l As Integer = hueRamp.Length \ 2

                For i As Integer = 0 To l
                    hueRamp(i) = Math.Pow(i / l, 1.0 / rampGamma)
                Next

                For i As Integer = l + 1 To hueRamp.Length - 1
                    hueRamp(i) = hueRamp(l - (i - l))
                Next
            End If

            Try
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                Dim scan0 As IntPtr = bmData.Scan0
                Dim stride As Integer = bmData.Stride

                Dim pf((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, pf, 0, pf.Length)

                Dim pos As Integer = 0

                'For y As Integer = 0 To nHeight - 1
                Parallel.[For](0, nHeight, Sub(y)
                                               Dim pfpos As Integer = y * stride
                                               For x As Integer = 0 To nWidth - 1
                                                   Dim red As Byte = pf(pfpos + 2)
                                                   Dim green As Byte = pf(pfpos + 1)
                                                   Dim blue As Byte = pf(pfpos)

                                                   Dim p As HSLData = RGBtoHSL(red, green, blue)

                                                   Dim fSat As New HSLRange()
                                                   If SaturationMin < SaturationMax Then
                                                       fSat.Range1.Min = SaturationMin
                                                       fSat.Range1.Max = SaturationMax
                                                       fSat.Range2.Min = 0
                                                       fSat.Range2.Max = 0
                                                   Else
                                                       fSat.Range1.Min = SaturationMax
                                                       fSat.Range1.Max = 1.0F
                                                       fSat.Range2.Min = 0
                                                       fSat.Range2.Max = SaturationMin
                                                   End If

                                                   Dim fLum As New HSLRange()
                                                   If LuminanceMin < LuminanceMax Then
                                                       fLum.Range1.Min = LuminanceMin
                                                       fLum.Range1.Max = LuminanceMax
                                                       fLum.Range2.Min = 0
                                                       fLum.Range2.Max = 0
                                                   Else
                                                       fLum.Range1.Min = LuminanceMax
                                                       fLum.Range1.Max = 1.0F
                                                       fLum.Range2.Min = 0
                                                       fLum.Range2.Max = LuminanceMin
                                                   End If

                                                   If HueMin < HueMax Then
                                                       If (p.Hue >= HueMin) AndAlso (p.Hue <= HueMax) Then
                                                           If fSat.Contains(p.Saturation) AndAlso fLum.Contains(p.Luminance) Then
                                                               If setAlpha Then
                                                                   If addAlpha Then
                                                                       pf(pfpos + 3) = CByte(Math.Max(Math.Min(CInt(pf(pfpos + 3)) + alphaVal * hueRamp(Math.Max(Math.Min(CInt(p.Hue - HueMin), hueRamp.Length - 1), 0)), 255), 0))
                                                                   Else
                                                                       pf(pfpos + 3) = CByte(alphaVal)
                                                                   End If
                                                               End If
                                                               Dim val_hue As Single = p.Hue + CSng(Hue * hueRamp(Math.Max(Math.Min(CInt(p.Hue - HueMin), hueRamp.Length - 1), 0)))

                                                               If val_hue > 360 Then
                                                                   val_hue -= 360
                                                               End If

                                                               p.Hue = val_hue

                                                               If AddSaturation Then
                                                                   p.Saturation += CSng(Saturation * hueRamp(Math.Max(Math.Min(CInt(p.Hue - HueMin), hueRamp.Length - 1), 0)))
                                                               Else
                                                                   p.Saturation = Saturation
                                                               End If

                                                               If p.Saturation > 1.0F Then
                                                                   p.Saturation = 1.0F
                                                               End If

                                                               If p.Saturation < 0F Then
                                                                   p.Saturation = 0F
                                                               End If

                                                               If AddLuminance Then
                                                                   p.Luminance += CSng(Luminance * hueRamp(Math.Max(Math.Min(CInt(p.Hue - HueMin), hueRamp.Length - 1), 0)))
                                                               Else
                                                                   p.Luminance = Luminance
                                                               End If

                                                               If p.Luminance > 1.0F Then
                                                                   p.Luminance = 1.0F
                                                               End If

                                                               If p.Luminance < 0F Then
                                                                   p.Luminance = 0F
                                                               End If

                                                               Dim nPixel As PixelData = HSLtoRGB(p.Hue, p.Saturation, p.Luminance)

                                                               pf(pfpos + 2) = nPixel.red
                                                               pf(pfpos + 1) = nPixel.green
                                                               pf(pfpos) = nPixel.blue
                                                           End If
                                                       End If
                                                   Else
                                                       If (p.Hue >= HueMin) OrElse (p.Hue <= HueMax) Then
                                                           If fSat.Contains(p.Saturation) AndAlso fLum.Contains(p.Luminance) Then
                                                               Dim ahue As Single = p.Hue - HueMin

                                                               If ahue < 0 Then
                                                                   ahue += 360
                                                               End If

                                                               ahue = CInt(Math.Max(Math.Min(ahue, hueRamp.Length - 1), 0))

                                                               Dim fhue As Integer = CInt(ahue)
                                                               If setAlpha Then
                                                                   If addAlpha Then
                                                                       pf(pfpos + 3) = CByte(Math.Max(Math.Min(CInt(pf(pfpos + 3)) + alphaVal * hueRamp(fhue), 255), 0))
                                                                   Else
                                                                       pf(pfpos + 3) = CByte(alphaVal)
                                                                   End If
                                                               End If

                                                               Dim val_hue As Single = p.Hue + CSng(Hue * hueRamp(fhue))

                                                               If val_hue > 360 Then
                                                                   val_hue -= 360
                                                               End If

                                                               p.Hue = val_hue

                                                               If AddSaturation Then
                                                                   p.Saturation += CSng(Saturation * hueRamp(fhue))
                                                               Else
                                                                   p.Saturation = Saturation
                                                               End If

                                                               If p.Saturation > 1.0F Then
                                                                   p.Saturation = 1.0F
                                                               End If

                                                               If p.Saturation < 0F Then
                                                                   p.Saturation = 0F
                                                               End If

                                                               If AddLuminance Then
                                                                   p.Luminance += CSng(Luminance * hueRamp(fhue))
                                                               Else
                                                                   p.Luminance = Luminance
                                                               End If

                                                               If p.Luminance > 1.0F Then
                                                                   p.Luminance = 1.0F
                                                               End If

                                                               If p.Luminance < 0F Then
                                                                   p.Luminance = 0F
                                                               End If

                                                               Dim nPixel As PixelData = HSLtoRGB(p.Hue, p.Saturation, p.Luminance)

                                                               pf(pfpos + 2) = nPixel.red
                                                               pf(pfpos + 1) = nPixel.green
                                                               pf(pfpos) = nPixel.blue
                                                           End If
                                                       End If
                                                   End If

                                                   pfpos += 4
                                               Next
                                           End Sub)
                'Next

                Marshal.Copy(pf, 0, bmData.Scan0, pf.Length)
                bmp.UnlockBits(bmData)

                pf = Nothing
            Catch
                Try
                    bmp.UnlockBits(bmData)

                Catch
                End Try
            End Try
        End If
    End Sub

    'Public Shared Function RGBtoHSL(Red As Integer, Green As Integer, Blue As Integer) As HSLData
    '    Dim hsl As New HSLData()

    '    Dim c As Color = Color.FromArgb(255, Red, Green, Blue)
    '    hsl.Hue = c.GetHue()
    '    hsl.Saturation = c.GetSaturation()
    '    hsl.Luminance = c.GetBrightness()

    '    Return hsl
    'End Function

    'Public Shared Function HSLtoRGB(H As Double, S As Double, L As Double) As PixelData
    '    Dim Temp1 As Double = 0.0, Temp2 As Double = 0.0
    '    Dim r As Double = 0.0, g As Double = 0.0, b As Double = 0.0

    '    If S = 0 Then
    '        r = L
    '        g = L
    '        b = L
    '    Else
    '        If L < 0.5 Then
    '            Temp2 = L * (1.0 + S)
    '        Else
    '            Temp2 = (L + S) - (S * L)
    '        End If

    '        Temp1 = 2.0 * L - Temp2

    '        'bischen Spaghetti hier, evtl. in eigene Funktion auslagern

    '        Dim hTmp As Double = H / 360.0
    '        Dim rTmp As Double, gTmp As Double, bTmp As Double

    '        rTmp = hTmp + (1.0 / 3.0)
    '        gTmp = hTmp
    '        bTmp = hTmp - (1.0 / 3.0)

    '        If rTmp < 0.0 Then
    '            rTmp += 1.0
    '        End If
    '        If gTmp < 0.0 Then
    '            gTmp += 1.0
    '        End If
    '        If bTmp < 0.0 Then
    '            bTmp += 1.0
    '        End If

    '        If rTmp > 1.0 Then
    '            rTmp -= 1.0
    '        End If
    '        If gTmp > 1.0 Then
    '            gTmp -= 1.0
    '        End If
    '        If bTmp > 1.0 Then
    '            bTmp -= 1.0
    '        End If

    '        If 6.0 * rTmp < 1.0 Then
    '            r = Temp1 + (Temp2 - Temp1) * 6.0 * rTmp
    '        ElseIf 2.0 * rTmp < 1.0 Then
    '            r = Temp2
    '        ElseIf 3.0 * rTmp < 2.0 Then
    '            r = Temp1 + (Temp2 - Temp1) * ((2.0 / 3.0) - rTmp) * 6.0
    '        Else
    '            r = Temp1
    '        End If

    '        If 6.0 * gTmp < 1.0 Then
    '            g = Temp1 + (Temp2 - Temp1) * 6.0 * gTmp
    '        ElseIf 2.0 * gTmp < 1.0 Then
    '            g = Temp2
    '        ElseIf 3.0 * gTmp < 2.0 Then
    '            g = Temp1 + (Temp2 - Temp1) * ((2.0 / 3.0) - gTmp) * 6.0
    '        Else
    '            g = Temp1
    '        End If

    '        If 6.0 * bTmp < 1.0 Then
    '            b = Temp1 + (Temp2 - Temp1) * 6.0 * bTmp
    '        ElseIf 2.0 * bTmp < 1.0 Then
    '            b = Temp2
    '        ElseIf 3.0 * bTmp < 2.0 Then
    '            b = Temp1 + (Temp2 - Temp1) * ((2.0 / 3.0) - bTmp) * 6.0
    '        Else
    '            b = Temp1
    '        End If
    '    End If

    '    Dim RGB As New PixelData()

    '    r *= 255.0
    '    g *= 255.0
    '    b *= 255.0

    '    RGB.red = CByte(CInt(r))
    '    RGB.green = CByte(CInt(g))
    '    RGB.blue = CByte(CInt(b))

    '    Return RGB
    'End Function
End Class
