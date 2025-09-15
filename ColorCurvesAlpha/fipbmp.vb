Option Strict On

Imports System.Runtime.InteropServices
Imports System.Drawing.Imaging
Imports System.Drawing
Imports System.Threading.Tasks

Public Class fipbmp
    Public Shared Sub gradColors(bmp As Bitmap, alphaValues As Integer(), ignoreAlpha0 As Boolean)
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
                                                   If p(pos + 3) > 0 OrElse Not ignoreAlpha0 Then
                                                       Dim value As Integer = alphaValues(p(pos + 3))
                                                       If value < 0 Then
                                                           value = 0
                                                       End If
                                                       If value > 255 Then
                                                           value = 255
                                                       End If
                                                       p(pos + 3) = CByte(value)
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

    Public Shared Sub gradLumToAlpha(bmp As Bitmap, alphaValues As Integer(), doAlpha As Boolean, ignoreAlpha0 As Boolean)
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
                                                   If p(pos + 3) > 0 OrElse Not ignoreAlpha0 Then
                                                       Dim c As Color = Color.FromArgb(p(pos + 3), p(pos + 2), p(pos + 1), p(pos))
                                                       Dim lum As Single = Math.Max(Math.Min(c.GetBrightness() * 255.0F, 255), 0)

                                                       If doAlpha Then
                                                           lum *= CSng(CDbl(p(pos + 3)) / 255.0)
                                                       End If

                                                       Dim value As Integer = alphaValues(CInt(lum))
                                                       If value < 0 Then
                                                           value = 0
                                                       End If
                                                       If value > 255 Then
                                                           value = 255
                                                       End If

                                                       p(pos + 3) = CByte(value)
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
End Class
