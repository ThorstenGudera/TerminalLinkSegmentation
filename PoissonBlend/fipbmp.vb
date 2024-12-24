Option Strict On

Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Threading.Tasks
Imports ChainCodeFinder

Public Class fipbmp
    Public Delegate Sub ProgressPlusEventHandler(sender As Object, e As ComponentModel.ProgressChangedEventArgs)
    Public Event ProgressPlus As ProgressPlusEventHandler

    Public Property BCancel() As Boolean
        Get
            Return m_BCancel
        End Get
        Set
            m_BCancel = Value
        End Set
    End Property
    Private m_BCancel As Boolean

    'some of these methods are ports of c# code provided here: http://stackoverflow.com/questions/36473340/unable-to-get-an-accurate-threshold-of-an-image-with-a-bright-spot
    Public Shared Sub GrayScaleImage(bmp As Bitmap)
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
                                       Dim value As Integer = CInt(CDbl(p(pos)) * 0.114 + CDbl(p(pos + 1)) * 0.587 + CDbl(p(pos + 2)) * 0.299)

                                       If value < 0 Then
                                           value = 0
                                       End If
                                       If value > 255 Then
                                           value = 255
                                       End If

                                       Dim v As Byte = CByte(value)
                                       p(pos) = v
                                       p(pos + 1) = v
                                       p(pos + 2) = v

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

    Public Shared Sub Invert(bmp As Bitmap)
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

            'for (int y = 0; y < bmData.Height; y++)
            Parallel.[For](0, h, Sub(y)
                                     Dim pos As Integer = y * stride

                                     For x As Integer = 0 To w - 1
                                         p(pos + 0) = CByte(255 - p(pos + 0))
                                         p(pos + 1) = CByte(255 - p(pos + 1))
                                         p(pos + 2) = CByte(255 - p(pos + 2))

                                         pos += 4
                                     Next

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

    Public Shared Function ExtractChannels(bmp As Bitmap, r As Boolean, g As Boolean, b As Boolean) As Bitmap
        Dim bmData As BitmapData = Nothing
        Dim bmData2 As BitmapData = Nothing

        If Not AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Return Nothing
        End If

        Dim bOut As Bitmap = Nothing

        Try
            bOut = New Bitmap(bmp.Width, bmp.Height)
            bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.[WriteOnly], PixelFormat.Format32bppArgb)
            bmData2 = bOut.LockBits(New Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

            Dim w As Integer = bmp.Width
            Dim h As Integer = bmp.Height
            Dim stride As Integer = bmData.Stride

            Dim p((bmData.Stride * bmData.Height) - 1) As Byte
            Marshal.Copy(bmData.Scan0, p, 0, p.Length)
            Dim p2((bmData2.Stride * bmData2.Height) - 1) As Byte
            Marshal.Copy(bmData2.Scan0, p2, 0, p2.Length)

            If r AndAlso Not g AndAlso Not b Then
                'for (int y = 0; y < bmData.Height; y++)
                Parallel.[For](0, h, Sub(y)
                                         Dim pos As Integer = y * stride

                                         For x As Integer = 0 To w - 1
                                             p2(pos + 0) = CByte(p(pos + 2))
                                             p2(pos + 1) = CByte(p(pos + 2))
                                             p2(pos + 2) = CByte(p(pos + 2))
                                             p2(pos + 3) = CByte(p(pos + 3))

                                             pos += 4
                                         Next

                                     End Sub)
            ElseIf g AndAlso Not r AndAlso Not b Then
                'for (int y = 0; y < bmData.Height; y++)
                Parallel.[For](0, h, Sub(y)
                                         Dim pos As Integer = y * stride

                                         For x As Integer = 0 To w - 1
                                             p2(pos + 0) = CByte(p(pos + 1))
                                             p2(pos + 1) = CByte(p(pos + 1))
                                             p2(pos + 2) = CByte(p(pos + 1))
                                             p2(pos + 3) = CByte(p(pos + 3))

                                             pos += 4
                                         Next

                                     End Sub)
            ElseIf b AndAlso Not g AndAlso Not r Then
                'for (int y = 0; y < bmData.Height; y++)
                Parallel.[For](0, h, Sub(y)
                                         Dim pos As Integer = y * stride

                                         For x As Integer = 0 To w - 1
                                             p2(pos + 0) = CByte(p(pos))
                                             p2(pos + 1) = CByte(p(pos))
                                             p2(pos + 2) = CByte(p(pos))
                                             p2(pos + 3) = CByte(p(pos + 3))

                                             pos += 4
                                         Next

                                     End Sub)
            ElseIf r AndAlso g AndAlso Not b Then
                'for (int y = 0; y < bmData.Height; y++)
                Parallel.[For](0, h, Sub(y)
                                         Dim pos As Integer = y * stride

                                         For x As Integer = 0 To w - 1
                                             Dim value As Integer = CInt(CDbl(p(pos + 1)) * 0.663 + CDbl(p(pos + 2)) * 0.337)

                                             If value < 0 Then
                                                 value = 0
                                             End If
                                             If value > 255 Then
                                                 value = 255
                                             End If

                                             Dim v As Byte = CByte(value)
                                             p2(pos) = v
                                             p2(pos + 1) = v
                                             p2(pos + 2) = v
                                             p2(pos + 3) = CByte(p(pos + 3))

                                             pos += 4
                                         Next

                                     End Sub)
            ElseIf r AndAlso b AndAlso Not g Then
                'for (int y = 0; y < bmData.Height; y++)
                Parallel.[For](0, h, Sub(y)
                                         Dim pos As Integer = y * stride

                                         For x As Integer = 0 To w - 1
                                             Dim value As Integer = CInt(CDbl(p(pos)) * 0.276 + CDbl(p(pos + 2)) * 0.724)

                                             If value < 0 Then
                                                 value = 0
                                             End If
                                             If value > 255 Then
                                                 value = 255
                                             End If

                                             Dim v As Byte = CByte(value)
                                             p2(pos) = v
                                             p2(pos + 1) = v
                                             p2(pos + 2) = v
                                             p2(pos + 3) = CByte(p(pos + 3))

                                             pos += 4
                                         Next

                                     End Sub)
            ElseIf g AndAlso b AndAlso Not r Then
                'for (int y = 0; y < bmData.Height; y++)
                Parallel.[For](0, h, Sub(y)
                                         Dim pos As Integer = y * stride

                                         For x As Integer = 0 To w - 1
                                             Dim value As Integer = CInt(CDbl(p(pos)) * 0.163 + CDbl(p(pos + 1)) * 0.837)

                                             If value < 0 Then
                                                 value = 0
                                             End If
                                             If value > 255 Then
                                                 value = 255
                                             End If

                                             Dim v As Byte = CByte(value)
                                             p2(pos) = v
                                             p2(pos + 1) = v
                                             p2(pos + 2) = v
                                             p2(pos + 3) = CByte(p(pos + 3))

                                             pos += 4
                                         Next

                                     End Sub)
            ElseIf r AndAlso g AndAlso b Then
                'for (int y = 0; y < bmData.Height; y++)
                Parallel.[For](0, h, Sub(y)
                                         Dim pos As Integer = y * stride

                                         For x As Integer = 0 To w - 1
                                             Dim value As Integer = CInt(CDbl(p(pos)) * 0.114 + CDbl(p(pos + 1)) * 0.587 + CDbl(p(pos + 2)) * 0.299)

                                             If value < 0 Then
                                                 value = 0
                                             End If
                                             If value > 255 Then
                                                 value = 255
                                             End If

                                             Dim v As Byte = CByte(value)
                                             p2(pos) = v
                                             p2(pos + 1) = v
                                             p2(pos + 2) = v
                                             p2(pos + 3) = CByte(p(pos + 3))

                                             pos += 4
                                         Next

                                     End Sub)
            End If

            Marshal.Copy(p2, 0, bmData2.Scan0, p2.Length)
            bmp.UnlockBits(bmData)
            bOut.UnlockBits(bmData2)

            p = Nothing
            p2 = Nothing
        Catch
            Try
                bmp.UnlockBits(bmData)

            Catch
            End Try
            Try
                bOut.UnlockBits(bmData2)

            Catch
            End Try
        End Try

        Return bOut
    End Function

    Public Shared Function Erode(bmp As Bitmap, radiusX As Integer, radiusY As Integer, returnBinary As Boolean, th As Integer, bgw As System.ComponentModel.BackgroundWorker, pe As ProgressEventArgs) As Bitmap
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 12L) Then
            Dim bmData As BitmapData = Nothing
            Dim bmData2 As BitmapData = Nothing
            Dim bmDataTmp As BitmapData = Nothing
            Dim bOut As Bitmap = Nothing
            Dim bTmp As Bitmap = Nothing
            Try
                bOut = New Bitmap(bmp.Width, bmp.Height)
                bTmp = New Bitmap(bmp.Width, bmp.Height)
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmData2 = bOut.LockBits(New Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataTmp = bTmp.LockBits(New Rectangle(0, 0, bTmp.Width, bTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)
                Dim p2((bmData2.Stride * bmData2.Height) - 1) As Byte
                Marshal.Copy(bmData2.Scan0, p2, 0, p2.Length)
                Dim pTmp((bmDataTmp.Stride * bmDataTmp.Height) - 1) As Byte
                Marshal.Copy(bmDataTmp.Scan0, pTmp, 0, pTmp.Length)

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height
                Dim stride As Integer = bmData.Stride

                If returnBinary Then
                    'For y As Integer = 0 To nHeight - 1
                    Parallel.For(0, nHeight, Sub(y)
                                                 For x As Integer = 0 To nWidth - 1
                                                     Dim b As Byte = 255
                                                     For i As Integer = x - radiusX To x + radiusX
                                                         If i >= 0 AndAlso i < nWidth Then
                                                             Dim b2 As Byte = 0
                                                             If p(y * stride + i * 4) > th Then
                                                                 b2 = 255
                                                             End If

                                                             b = Math.Min(b, b2)
                                                         End If
                                                     Next
                                                     pTmp(y * stride + x * 4) = b
                                                     pTmp(y * stride + x * 4 + 1) = b
                                                     pTmp(y * stride + x * 4 + 2) = b
                                                     pTmp(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3)
                                                 Next
                                             End Sub)
                    'Next

                    Marshal.Copy(pTmp, 0, bmDataTmp.Scan0, pTmp.Length)

                    'For x As Integer = 0 To nWidth - 1
                    Parallel.For(0, nWidth, Sub(x)
                                                For y As Integer = 0 To nHeight - 1
                                                    Dim b As Byte = 255
                                                    For i As Integer = y - radiusY To y + radiusY
                                                        If i >= 0 AndAlso i < nHeight Then
                                                            'Dim b2 As Byte = 0
                                                            'If pTmp(i * stride + x * 4) > th Then
                                                            '    b2 = 255
                                                            'End If
                                                            'If i >= 0 AndAlso i < nHeight Then
                                                            '    b = Math.Min(b, b2)
                                                            'End If   

                                                            b = Math.Min(b, pTmp(i * stride + x * 4))
                                                        End If
                                                    Next
                                                    p2(y * stride + x * 4) = b
                                                    p2(y * stride + x * 4 + 1) = b
                                                    p2(y * stride + x * 4 + 2) = b
                                                    p2(y * stride + x * 4 + 3) = pTmp(y * stride + x * 4 + 3)
                                                Next
                                            End Sub)
                    'Next
                Else
                    'For y As Integer = 0 To nHeight - 1
                    Parallel.For(0, nHeight, Sub(y)
                                                 For x As Integer = 0 To nWidth - 1
                                                     Dim b As Byte = 255
                                                     For i As Integer = x - radiusX To x + radiusX
                                                         If i >= 0 AndAlso i < nWidth Then
                                                             b = Math.Min(b, p(y * stride + i * 4))
                                                         End If
                                                     Next
                                                     pTmp(y * stride + x * 4) = b
                                                     pTmp(y * stride + x * 4 + 1) = b
                                                     pTmp(y * stride + x * 4 + 2) = b
                                                     pTmp(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3)
                                                 Next
                                             End Sub)
                    'Next

                    Marshal.Copy(pTmp, 0, bmDataTmp.Scan0, pTmp.Length)

                    'For x As Integer = 0 To nWidth - 1
                    Parallel.For(0, nWidth, Sub(x)
                                                For y As Integer = 0 To nHeight - 1
                                                    Dim b As Byte = 255
                                                    For i As Integer = y - radiusY To y + radiusY
                                                        If i >= 0 AndAlso i < nHeight Then
                                                            b = Math.Min(b, pTmp(i * stride + x * 4))
                                                        End If
                                                    Next
                                                    p2(y * stride + x * 4) = b
                                                    p2(y * stride + x * 4 + 1) = b
                                                    p2(y * stride + x * 4 + 2) = b
                                                    p2(y * stride + x * 4 + 3) = pTmp(y * stride + x * 4 + 3)
                                                Next
                                            End Sub)
                    'Next
                End If

                Marshal.Copy(p2, 0, bmData2.Scan0, p2.Length)

                bmp.UnlockBits(bmData)
                bOut.UnlockBits(bmData2)
                bTmp.UnlockBits(bmDataTmp)

                p = Nothing
                p2 = Nothing
                pTmp = Nothing
            Catch
                Try
                    bmp.UnlockBits(bmData)
                Catch

                End Try
                Try
                    bOut.UnlockBits(bmData2)
                Catch

                End Try
                Try
                    bTmp.UnlockBits(bmDataTmp)
                Catch

                End Try
                If Not bOut Is Nothing Then
                    bOut.Dispose()
                    bOut = Nothing
                End If
            Finally
                If Not bTmp Is Nothing Then
                    bTmp.Dispose()
                    bTmp = Nothing
                End If
            End Try

            Return bOut
        End If
        Return Nothing
    End Function

    Public Shared Function Dilate(bmp As Bitmap, radiusX As Integer, radiusy As Integer, returnBinary As Boolean, th As Integer, bgw As System.ComponentModel.BackgroundWorker, pe As ProgressEventArgs) As Bitmap
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 12L) Then
            Dim bmData As BitmapData = Nothing
            Dim bmData2 As BitmapData = Nothing
            Dim bmDataTmp As BitmapData = Nothing
            Dim bOut As Bitmap = Nothing
            Dim bTmp As Bitmap = Nothing
            Try
                bOut = New Bitmap(bmp.Width, bmp.Height)
                bTmp = New Bitmap(bmp.Width, bmp.Height)
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmData2 = bOut.LockBits(New Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataTmp = bTmp.LockBits(New Rectangle(0, 0, bTmp.Width, bTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)
                Dim p2((bmData2.Stride * bmData2.Height) - 1) As Byte
                Marshal.Copy(bmData2.Scan0, p2, 0, p2.Length)
                Dim pTmp((bmDataTmp.Stride * bmDataTmp.Height) - 1) As Byte
                Marshal.Copy(bmDataTmp.Scan0, pTmp, 0, pTmp.Length)

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height
                Dim stride As Integer = bmData.Stride

                If returnBinary Then
                    'For y As Integer = 0 To nHeight - 1
                    Parallel.For(0, nHeight, Sub(y)
                                                 For x As Integer = 0 To nWidth - 1
                                                     Dim b As Byte = 0
                                                     For i As Integer = x - radiusX To x + radiusX
                                                         If i >= 0 AndAlso i < nWidth Then
                                                             Dim b2 As Byte = 0
                                                             If p(y * stride + i * 4) > th Then
                                                                 b2 = 255
                                                             End If

                                                             b = Math.Max(b, b2)
                                                         End If
                                                     Next
                                                     pTmp(y * stride + x * 4) = b
                                                     pTmp(y * stride + x * 4 + 1) = b
                                                     pTmp(y * stride + x * 4 + 2) = b
                                                     pTmp(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3)
                                                 Next
                                             End Sub)
                    'Next

                    Marshal.Copy(pTmp, 0, bmDataTmp.Scan0, pTmp.Length)

                    'For x As Integer = 0 To nWidth - 1
                    Parallel.For(0, nWidth, Sub(x)
                                                For y As Integer = 0 To nHeight - 1
                                                    Dim b As Byte = 0
                                                    For i As Integer = y - radiusy To y + radiusy
                                                        If i >= 0 AndAlso i < nHeight Then
                                                            'Dim b2 As Byte = 0
                                                            'If pTmp(i * stride + x * 4) > th Then
                                                            '    b2 = 255
                                                            'End If
                                                            'If i >= 0 AndAlso i < nHeight Then
                                                            '    b = Math.Max(b, b2)
                                                            'End If

                                                            b = Math.Max(b, pTmp(i * stride + x * 4))
                                                        End If
                                                    Next
                                                    p2(y * stride + x * 4) = b
                                                    p2(y * stride + x * 4 + 1) = b
                                                    p2(y * stride + x * 4 + 2) = b
                                                    p2(y * stride + x * 4 + 3) = pTmp(y * stride + x * 4 + 3)
                                                Next
                                            End Sub)
                    'Next
                Else
                    'For y As Integer = 0 To nHeight - 1
                    Parallel.For(0, nHeight, Sub(y)
                                                 For x As Integer = 0 To nWidth - 1
                                                     Dim b As Byte = 0
                                                     For i As Integer = x - radiusX To x + radiusX
                                                         If i >= 0 AndAlso i < nWidth Then
                                                             b = Math.Max(b, p(y * stride + i * 4))
                                                         End If
                                                     Next
                                                     pTmp(y * stride + x * 4) = b
                                                     pTmp(y * stride + x * 4 + 1) = b
                                                     pTmp(y * stride + x * 4 + 2) = b
                                                     pTmp(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3)
                                                 Next
                                             End Sub)
                    'Next

                    Marshal.Copy(pTmp, 0, bmDataTmp.Scan0, pTmp.Length)

                    'For x As Integer = 0 To nWidth - 1
                    Parallel.For(0, nWidth, Sub(x)
                                                For y As Integer = 0 To nHeight - 1
                                                    Dim b As Byte = 0
                                                    For i As Integer = y - radiusy To y + radiusy
                                                        If i >= 0 AndAlso i < nHeight Then
                                                            b = Math.Max(b, pTmp(i * stride + x * 4))
                                                        End If
                                                    Next
                                                    p2(y * stride + x * 4) = b
                                                    p2(y * stride + x * 4 + 1) = b
                                                    p2(y * stride + x * 4 + 2) = b
                                                    p2(y * stride + x * 4 + 3) = pTmp(y * stride + x * 4 + 3)
                                                Next
                                            End Sub)
                    'Next
                End If

                Marshal.Copy(p2, 0, bmData2.Scan0, p2.Length)

                bmp.UnlockBits(bmData)
                bOut.UnlockBits(bmData2)
                bTmp.UnlockBits(bmDataTmp)

                p = Nothing
                p2 = Nothing
                pTmp = Nothing
            Catch
                Try
                    bmp.UnlockBits(bmData)
                Catch

                End Try
                Try
                    bOut.UnlockBits(bmData2)
                Catch

                End Try
                Try
                    bTmp.UnlockBits(bmDataTmp)
                Catch

                End Try
                If Not bOut Is Nothing Then
                    bOut.Dispose()
                    bOut = Nothing
                End If
            Finally
                If Not bTmp Is Nothing Then
                    bTmp.Dispose()
                    bTmp = Nothing
                End If
            End Try

            Return bOut
        End If
        Return Nothing
    End Function

    Public Shared Function Union(bmp As Bitmap, bmp2 As Bitmap, bgw As System.ComponentModel.BackgroundWorker, pe As ProgressEventArgs) As Bitmap
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Dim bmData As BitmapData = Nothing
            Dim bmData2 As BitmapData = Nothing
            Dim bmDataTmp As BitmapData = Nothing
            Dim bOut As Bitmap = Nothing

            Try
                bOut = New Bitmap(bmp.Width, bmp.Height)

                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmData2 = bOut.LockBits(New Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataTmp = bmp2.LockBits(New Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)
                Dim p2((bmData2.Stride * bmData2.Height) - 1) As Byte
                Marshal.Copy(bmData2.Scan0, p2, 0, p2.Length)
                Dim pTmp((bmDataTmp.Stride * bmDataTmp.Height) - 1) As Byte
                Marshal.Copy(bmDataTmp.Scan0, pTmp, 0, pTmp.Length)

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height
                Dim stride As Integer = bmData.Stride

                'For y As Integer = 0 To nHeight - 1
                Parallel.For(0, nHeight, Sub(y)
                                             For x As Integer = 0 To nWidth - 1
                                                 p2(y * stride + x * 4) = p(y * stride + x * 4) Or pTmp(y * stride + x * 4)
                                                 p2(y * stride + x * 4 + 1) = p(y * stride + x * 4 + 1) Or pTmp(y * stride + x * 4 + 1)
                                                 p2(y * stride + x * 4 + 2) = p(y * stride + x * 4 + 2) Or pTmp(y * stride + x * 4 + 2)
                                                 p2(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3)
                                             Next
                                         End Sub)
                'Next

                Marshal.Copy(p2, 0, bmData2.Scan0, p2.Length)

                bmp.UnlockBits(bmData)
                bOut.UnlockBits(bmData2)
                bmp2.UnlockBits(bmDataTmp)

                p = Nothing
                p2 = Nothing
                pTmp = Nothing
            Catch
                Try
                    bmp.UnlockBits(bmData)
                Catch

                End Try
                Try
                    bOut.UnlockBits(bmData2)
                Catch

                End Try
                Try
                    bmp2.UnlockBits(bmDataTmp)
                Catch

                End Try
                If Not bOut Is Nothing Then
                    bOut.Dispose()
                    bOut = Nothing
                End If
            End Try

            Return bOut
        End If
        Return Nothing
    End Function

    Public Shared Function Intersect(bmp As Bitmap, bmp2 As Bitmap, bgw As System.ComponentModel.BackgroundWorker, pe As ProgressEventArgs) As Bitmap
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Dim bmData As BitmapData = Nothing
            Dim bmData2 As BitmapData = Nothing
            Dim bmDataTmp As BitmapData = Nothing
            Dim bOut As Bitmap = Nothing

            Try
                bOut = New Bitmap(bmp.Width, bmp.Height)

                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmData2 = bOut.LockBits(New Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataTmp = bmp2.LockBits(New Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)
                Dim p2((bmData2.Stride * bmData2.Height) - 1) As Byte
                Marshal.Copy(bmData2.Scan0, p2, 0, p2.Length)
                Dim pTmp((bmDataTmp.Stride * bmDataTmp.Height) - 1) As Byte
                Marshal.Copy(bmDataTmp.Scan0, pTmp, 0, pTmp.Length)

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height
                Dim stride As Integer = bmData.Stride

                'For y As Integer = 0 To nHeight - 1
                Parallel.For(0, nHeight, Sub(y)
                                             For x As Integer = 0 To nWidth - 1
                                                 p2(y * stride + x * 4) = p(y * stride + x * 4) And pTmp(y * stride + x * 4)
                                                 p2(y * stride + x * 4 + 1) = p(y * stride + x * 4 + 1) And pTmp(y * stride + x * 4 + 1)
                                                 p2(y * stride + x * 4 + 2) = p(y * stride + x * 4 + 2) And pTmp(y * stride + x * 4 + 2)
                                                 p2(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3)
                                             Next
                                         End Sub)
                'Next

                Marshal.Copy(p2, 0, bmData2.Scan0, p2.Length)

                bmp.UnlockBits(bmData)
                bOut.UnlockBits(bmData2)
                bmp2.UnlockBits(bmDataTmp)

                p = Nothing
                p2 = Nothing
                pTmp = Nothing
            Catch
                Try
                    bmp.UnlockBits(bmData)
                Catch

                End Try
                Try
                    bOut.UnlockBits(bmData2)
                Catch

                End Try
                Try
                    bmp2.UnlockBits(bmDataTmp)
                Catch

                End Try
                If Not bOut Is Nothing Then
                    bOut.Dispose()
                    bOut = Nothing
                End If
            End Try

            Return bOut
        End If
        Return Nothing
    End Function

    Public Shared Function Subtract(bmp As Bitmap, bmp2 As Bitmap, bgw As System.ComponentModel.BackgroundWorker, pe As ProgressEventArgs) As Bitmap
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Dim bmData As BitmapData = Nothing
            Dim bmData2 As BitmapData = Nothing
            Dim bmDataTmp As BitmapData = Nothing
            Dim bOut As Bitmap = Nothing

            Try
                bOut = New Bitmap(bmp.Width, bmp.Height)

                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmData2 = bOut.LockBits(New Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataTmp = bmp2.LockBits(New Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)
                Dim p2((bmData2.Stride * bmData2.Height) - 1) As Byte
                Marshal.Copy(bmData2.Scan0, p2, 0, p2.Length)
                Dim pTmp((bmDataTmp.Stride * bmDataTmp.Height) - 1) As Byte
                Marshal.Copy(bmDataTmp.Scan0, pTmp, 0, pTmp.Length)

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height
                Dim stride As Integer = bmData.Stride

                'For y As Integer = 0 To nHeight - 1
                Parallel.For(0, nHeight, Sub(y)
                                             For x As Integer = 0 To nWidth - 1
                                                 p2(y * stride + x * 4) = CByte(Math.Max(CInt(p(y * stride + x * 4)) - pTmp(y * stride + x * 4), 0))
                                                 p2(y * stride + x * 4 + 1) = CByte(Math.Max(CInt(p(y * stride + x * 4 + 1)) - pTmp(y * stride + x * 4 + 1), 0))
                                                 p2(y * stride + x * 4 + 2) = CByte(Math.Max(CInt(p(y * stride + x * 4 + 2)) - pTmp(y * stride + x * 4 + 2), 0))
                                                 p2(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3) Or pTmp(y * stride + x * 4 + 2)
                                             Next
                                         End Sub)
                'Next

                Marshal.Copy(p2, 0, bmData2.Scan0, p2.Length)

                bmp.UnlockBits(bmData)
                bOut.UnlockBits(bmData2)
                bmp2.UnlockBits(bmDataTmp)

                p = Nothing
                p2 = Nothing
                pTmp = Nothing
            Catch
                Try
                    bmp.UnlockBits(bmData)
                Catch

                End Try
                Try
                    bOut.UnlockBits(bmData2)
                Catch

                End Try
                Try
                    bmp2.UnlockBits(bmDataTmp)
                Catch

                End Try
                If Not bOut Is Nothing Then
                    bOut.Dispose()
                    bOut = Nothing
                End If
            End Try

            Return bOut
        End If
        Return Nothing
    End Function

    Public Shared Function SubtractWithBias(bmp As Bitmap, bmp2 As Bitmap, bias As Integer, bgw As System.ComponentModel.BackgroundWorker, pe As ProgressEventArgs) As Bitmap
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Dim bmData As BitmapData = Nothing
            Dim bmData2 As BitmapData = Nothing
            Dim bmDataTmp As BitmapData = Nothing
            Dim bOut As Bitmap = Nothing

            Try
                bOut = New Bitmap(bmp.Width, bmp.Height)

                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmData2 = bOut.LockBits(New Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataTmp = bmp2.LockBits(New Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)
                Dim p2((bmData2.Stride * bmData2.Height) - 1) As Byte
                Marshal.Copy(bmData2.Scan0, p2, 0, p2.Length)
                Dim pTmp((bmDataTmp.Stride * bmDataTmp.Height) - 1) As Byte
                Marshal.Copy(bmDataTmp.Scan0, pTmp, 0, pTmp.Length)

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height
                Dim stride As Integer = bmData.Stride

                'For y As Integer = 0 To nHeight - 1
                Parallel.For(0, nHeight, Sub(y)
                                             For x As Integer = 0 To nWidth - 1
                                                 p2(y * stride + x * 4) = CByte(Math.Max(Math.Min(bias + CInt(p(y * stride + x * 4)) - pTmp(y * stride + x * 4), 255), 0))
                                                 p2(y * stride + x * 4 + 1) = CByte(Math.Max(Math.Min(CInt(bias + p(y * stride + x * 4 + 1)) - pTmp(y * stride + x * 4 + 1), 255), 0))
                                                 p2(y * stride + x * 4 + 2) = CByte(Math.Max(Math.Min(CInt(bias + p(y * stride + x * 4 + 2)) - pTmp(y * stride + x * 4 + 2), 255), 0))
                                                 p2(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3) Or pTmp(y * stride + x * 4 + 2)
                                             Next
                                         End Sub)
                'Next

                Marshal.Copy(p2, 0, bmData2.Scan0, p2.Length)

                bmp.UnlockBits(bmData)
                bOut.UnlockBits(bmData2)
                bmp2.UnlockBits(bmDataTmp)

                p = Nothing
                p2 = Nothing
                pTmp = Nothing
            Catch
                Try
                    bmp.UnlockBits(bmData)
                Catch

                End Try
                Try
                    bOut.UnlockBits(bmData2)
                Catch

                End Try
                Try
                    bmp2.UnlockBits(bmDataTmp)
                Catch

                End Try
                If Not bOut Is Nothing Then
                    bOut.Dispose()
                    bOut = Nothing
                End If
            End Try

            Return bOut
        End If
        Return Nothing
    End Function

    Friend Shared Sub BinarizeImg(bmp As Bitmap, fgVal As Integer)
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Dim bmData As BitmapData = Nothing

            Try
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height
                Dim stride As Integer = bmData.Stride

                'For y As Integer = 0 To nHeight - 1
                Parallel.For(0, nHeight, Sub(y)
                                             Dim pos As Integer = y * stride

                                             For x As Integer = 0 To nWidth - 1
                                                 Dim value As Integer = CInt(CDbl(p(pos)) * 0.114 + CDbl(p(pos + 1)) * 0.587 + CDbl(p(pos + 2)) * 0.299)

                                                 If value <= fgVal Then
                                                     value = 0
                                                 End If
                                                 If value > fgVal Then
                                                     value = 255
                                                 End If

                                                 p(pos) = CByte(value)
                                                 p(pos + 1) = CByte(value)
                                                 p(pos + 2) = CByte(value)
                                                 p(pos + 3) = CByte(255)

                                                 pos += 4
                                             Next
                                         End Sub)
                'Next

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

    'Public Function FastZGaussian_Blur_NxN(b As Bitmap, Length As Integer, bgw As System.ComponentModel.BackgroundWorker, f As FFT_Conv_DeconvVB.fipbmp, xStart As Integer, xWidth As Integer,
    '        yStart As Integer, yHeight As Integer) As Bitmap

    '    If (Length And &H1) <> 1 Then
    '        Return Nothing
    '    End If

    '    Dim bOut As Bitmap = Nothing

    '    Try
    '        Dim kernelVector As Double() = GetKernelF(Length, 1.0, -2, True, 1, 0.2,
    '            KernelTypeDeconvEnum.Gaussian, False, True, False)

    '        xStart = Math.Max(Math.Min(xStart, b.Width), 0)
    '        xWidth = Math.Max(Math.Min(xWidth, b.Width - xStart), 0)
    '        yStart = Math.Max(Math.Min(yStart, b.Height), 0)
    '        yHeight = Math.Max(Math.Min(yHeight, b.Height - yStart), 0)

    '        Dim pe As New ProgressEventArgs((b.Height + b.Width) * 4, 20, 1)
    '        bOut = f.ConvolveF(b, kernelVector, pe, bgw, FFT_Conv_DeconvVB.HorzVertEnum.hv)

    '        pe = Nothing
    '    Catch
    '        If bOut IsNot Nothing Then
    '            bOut.Dispose()
    '            bOut = Nothing
    '        End If
    '    End Try

    '    Return bOut
    'End Function

    Public Shared Function HitAndMiss(bmp As Bitmap, krnl(,) As Integer, bgw As System.ComponentModel.BackgroundWorker, pe As ProgressEventArgs) As Bitmap
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 16L) Then
            Dim bmData As BitmapData = Nothing
            Dim bmDataTmp As BitmapData = Nothing
            Dim bmDataTmp2 As BitmapData = Nothing
            Dim bTmp As Bitmap = Nothing
            Dim bTmp2 As Bitmap = Nothing
            Dim bOut As Bitmap = Nothing

            Try
                bTmp = New Bitmap(bmp.Width, bmp.Height)
                bTmp2 = New Bitmap(bmp.Width, bmp.Height)
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataTmp = bTmp.LockBits(New Rectangle(0, 0, bTmp.Width, bTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataTmp2 = bTmp2.LockBits(New Rectangle(0, 0, bTmp2.Width, bTmp2.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)
                Dim pTmp((bmDataTmp.Stride * bmDataTmp.Height) - 1) As Byte
                Marshal.Copy(bmDataTmp.Scan0, pTmp, 0, pTmp.Length)
                Dim pTmp2((bmDataTmp2.Stride * bmDataTmp2.Height) - 1) As Byte
                Marshal.Copy(bmDataTmp2.Scan0, pTmp2, 0, pTmp2.Length)

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height
                Dim stride As Integer = bmData.Stride

                Dim rH As Integer = krnl.GetLength(1) \ 2
                Dim cH As Integer = krnl.GetLength(0) \ 2

                'erode to bTmp
                'For y As Integer = 0 To nHeight - 1
                Parallel.For(0, nHeight, Sub(y)
                                             For x As Integer = 0 To nWidth - 1
                                                 Dim b As Byte = 255
                                                 For r = -rH To rH
                                                     If y + r >= 0 AndAlso y + r < nHeight Then
                                                         For c = -cH To cH
                                                             If x + c >= 0 AndAlso x + c < nWidth Then
                                                                 If krnl(c + cH, r + rH) = 1 Then
                                                                     b = Math.Min(b, p((r + y) * stride + (c + x) * 4))
                                                                 End If
                                                             End If
                                                         Next
                                                     End If
                                                 Next

                                                 pTmp(y * stride + x * 4) = b
                                                 pTmp(y * stride + x * 4 + 1) = b
                                                 pTmp(y * stride + x * 4 + 2) = b
                                                 pTmp(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3)
                                             Next
                                         End Sub)
                'Next

                Marshal.Copy(pTmp, 0, bmDataTmp.Scan0, pTmp.Length)
                bmp.UnlockBits(bmData)

                Invert(bmp)

                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)

                'erode complement to btmp2
                'For y As Integer = 0 To nHeight - 1
                Parallel.For(0, nHeight, Sub(y)
                                             For x As Integer = 0 To nWidth - 1
                                                 Dim b As Byte = 255
                                                 For r = -rH To rH
                                                     If y + r >= 0 AndAlso y + r < nHeight Then
                                                         For c = -cH To cH
                                                             If x + c >= 0 AndAlso x + c < nWidth Then
                                                                 If krnl(c + cH, r + rH) = -1 Then
                                                                     b = Math.Min(b, p((r + y) * stride + (c + x) * 4))
                                                                 End If
                                                             End If
                                                         Next
                                                     End If
                                                 Next

                                                 'since we're in the complement, we need to either take the complement of the byte, or invert the resulting image of this operation
                                                 pTmp2(y * stride + x * 4) = b
                                                 pTmp2(y * stride + x * 4 + 1) = b
                                                 pTmp2(y * stride + x * 4 + 2) = b
                                                 pTmp2(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3)
                                             Next
                                         End Sub)
                'Next
                Marshal.Copy(pTmp2, 0, bmDataTmp2.Scan0, pTmp2.Length)

                bmp.UnlockBits(bmData)
                bTmp.UnlockBits(bmDataTmp)
                bTmp2.UnlockBits(bmDataTmp2)

                p = Nothing
                pTmp = Nothing
                pTmp2 = Nothing

                'if you want to keep these images...
                'Invert(bmp)
                'Invert(bTmp2)
                'bOut = Subtract(bTmp, bTmp2, bgw, pe)

                bOut = Intersect(bTmp, bTmp2, bgw, pe)
            Catch ex As Exception
                Try
                    bmp.UnlockBits(bmData)
                Catch

                End Try
                Try
                    bTmp.UnlockBits(bmDataTmp)
                Catch

                End Try
                Try
                    bTmp2.UnlockBits(bmDataTmp2)
                Catch

                End Try
                If Not bOut Is Nothing Then
                    bOut.Dispose()
                    bOut = Nothing
                End If
            Finally
                If Not bTmp Is Nothing Then
                    bTmp.Dispose()
                    bTmp = Nothing
                End If
                If Not bTmp2 Is Nothing Then
                    bTmp2.Dispose()
                    bTmp2 = Nothing
                End If
            End Try

            Return bOut
        End If
        Return Nothing
    End Function

    Public Shared Function HitAndMissStep1(bmp As Bitmap, krnl(,) As Integer, bgw As System.ComponentModel.BackgroundWorker, pe As ProgressEventArgs) As Bitmap
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 8L) Then
            Dim bmData As BitmapData = Nothing
            Dim bmDataOut As BitmapData = Nothing

            Dim bOut As Bitmap = Nothing

            Try
                bOut = New Bitmap(bmp.Width, bmp.Height)
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataOut = bOut.LockBits(New Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)
                Dim pTmp((bmDataOut.Stride * bmDataOut.Height) - 1) As Byte
                Marshal.Copy(bmDataOut.Scan0, pTmp, 0, pTmp.Length)

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height
                Dim stride As Integer = bmData.Stride

                Dim rH As Integer = krnl.GetLength(1) \ 2
                Dim cH As Integer = krnl.GetLength(0) \ 2

                'erode to bTmp
                'For y As Integer = 0 To nHeight - 1
                Parallel.For(0, nHeight, Sub(y)
                                             For x As Integer = 0 To nWidth - 1
                                                 Dim b As Byte = 255
                                                 For r = -rH To rH
                                                     If y + r >= 0 AndAlso y + r < nHeight Then
                                                         For c = -cH To cH
                                                             If x + c >= 0 AndAlso x + c < nWidth Then
                                                                 If krnl(c + cH, r + rH) = 1 Then
                                                                     b = Math.Min(b, p((r + y) * stride + (c + x) * 4))
                                                                 End If
                                                             End If
                                                         Next
                                                     End If
                                                 Next

                                                 pTmp(y * stride + x * 4) = b
                                                 pTmp(y * stride + x * 4 + 1) = b
                                                 pTmp(y * stride + x * 4 + 2) = b
                                                 pTmp(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3)
                                             Next
                                         End Sub)
                'Next

                Marshal.Copy(pTmp, 0, bmDataOut.Scan0, pTmp.Length)
                bmp.UnlockBits(bmData)
                bOut.UnlockBits(bmDataOut)

                p = Nothing
                pTmp = Nothing
            Catch ex As Exception
                Try
                    bmp.UnlockBits(bmData)
                Catch

                End Try
                Try
                    bOut.UnlockBits(bmDataOut)
                Catch

                End Try

                If Not bOut Is Nothing Then
                    bOut.Dispose()
                    bOut = Nothing
                End If
            End Try

            Return bOut
        End If
        Return Nothing
    End Function

    Public Shared Function HitAndMissStep2(bmp As Bitmap, krnl(,) As Integer, bgw As System.ComponentModel.BackgroundWorker, pe As ProgressEventArgs) As Bitmap
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 8L) Then
            Dim bmData As BitmapData = Nothing

            Dim bmDataOut As BitmapData = Nothing
            Dim bOut As Bitmap = Nothing

            Try
                Invert(bmp)
                bOut = New Bitmap(bmp.Width, bmp.Height)
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataOut = bOut.LockBits(New Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)
                Dim pTmp2((bmDataOut.Stride * bmDataOut.Height) - 1) As Byte
                Marshal.Copy(bmDataOut.Scan0, pTmp2, 0, pTmp2.Length)

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height
                Dim stride As Integer = bmData.Stride

                Dim rH As Integer = krnl.GetLength(1) \ 2
                Dim cH As Integer = krnl.GetLength(0) \ 2

                'erode complement to btmp2
                'For y As Integer = 0 To nHeight - 1
                Parallel.For(0, nHeight, Sub(y)
                                             For x As Integer = 0 To nWidth - 1
                                                 Dim b As Byte = 255
                                                 For r = -rH To rH
                                                     If y + r >= 0 AndAlso y + r < nHeight Then
                                                         For c = -cH To cH
                                                             If x + c >= 0 AndAlso x + c < nWidth Then
                                                                 If krnl(c + cH, r + rH) = -1 Then
                                                                     b = Math.Min(b, p((r + y) * stride + (c + x) * 4))
                                                                 End If
                                                             End If
                                                         Next
                                                     End If
                                                 Next

                                                 'since we're in the complement, we need to either take the complement of the byte, or invert the resulting image of this operation
                                                 pTmp2(y * stride + x * 4) = b
                                                 pTmp2(y * stride + x * 4 + 1) = b
                                                 pTmp2(y * stride + x * 4 + 2) = b
                                                 pTmp2(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3)
                                             Next
                                         End Sub)
                'Next
                Marshal.Copy(pTmp2, 0, bmDataOut.Scan0, pTmp2.Length)

                bmp.UnlockBits(bmData)
                bOut.UnlockBits(bmDataOut)

                p = Nothing
                pTmp2 = Nothing

                'if you want to keep bmp...
                'Invert(bmp)

                Invert(bOut)

            Catch ex As Exception
                Try
                    bmp.UnlockBits(bmData)
                Catch

                End Try
                Try
                    bOut.UnlockBits(bmDataOut)
                Catch

                End Try
                If Not bOut Is Nothing Then
                    bOut.Dispose()
                    bOut = Nothing
                End If
            End Try

            Return bOut
        End If
        Return Nothing
    End Function

    Public Function GetOutline(bmp As Bitmap, fgVal As Integer) As Bitmap
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 8L) Then
            Dim b As Bitmap = Nothing
            Dim b2 As Bitmap = Nothing
            Dim fbits As BitArray = Nothing

            Dim zList As New List(Of AlphaChain)

            Try
                b = DirectCast(bmp.Clone(), Bitmap)
                b2 = New Bitmap(bmp.Width, bmp.Height)

                Using g As Graphics = Graphics.FromImage(b2)
                    g.Clear(Color.Black)
                End Using

                Dim prg As Integer = 0

                For i As Integer = 1 To 255
                    If Not BCancel Then
                        prg = CInt(CDbl(i) * 100.0 / 255)

                        Dim cf As New ChainFinder()
                        cf.AllowNullCells = True

                        Dim fList As List(Of ChainCode) = cf.GetOutline(b, fgVal + 1, True, 0, False)
                        'Dim l As New List(Of ChainCode)
                        'l.AddRange(fList)
                        zList.Add(New AlphaChain() With {.FList = fList, .Alpha = i})

                        'DrawOutlineToBmp(b2, fList, i)

                        RaiseEvent ProgressPlus(Me, New ComponentModel.ProgressChangedEventArgs(prg, Nothing))

                        RemoveOutline(b, fList)
                    Else
                        Exit For
                    End If
                Next

                DrawCompleteOutlineToBmp(b2, zList)

                If b IsNot Nothing Then
                    b.Dispose()
                End If

                b = Nothing

                Return b2
            Catch
                If fbits IsNot Nothing Then
                    fbits = Nothing
                End If
            End Try

            If b IsNot Nothing Then
                b.Dispose()
            End If

            b = Nothing
        End If
        Return Nothing
    End Function

    Public Function GetOutlineArrayGrayscale(bmp As Bitmap, fgVal As Integer, maxIterations As Integer) As Integer(,)
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 12L) Then
            Dim b2(,) As Integer = New Integer(bmp.Width, bmp.Height) {}
            Dim fbits As BitArray = Nothing

            Dim zList As New List(Of AlphaChain)

            Using b As Bitmap = New Bitmap(bmp)
                Dim prg As Integer = 0
                Dim i As Integer = 1
                Dim isDifferent As Boolean = True

                While isDifferent AndAlso i < maxIterations
                    If Not BCancel Then
                        prg = Math.Min(CInt(CDbl(i) * 100.0 / maxIterations), 100)

                        Dim cf As New ChainFinder()
                        cf.AllowNullCells = True

                        Dim fList As List(Of ChainCode) = cf.GetOutline(b, fgVal + 1, True, 0, False)
                        zList.Add(New AlphaChain() With {.FList = fList, .Alpha = i})

                        RaiseEvent ProgressPlus(Me, New ComponentModel.ProgressChangedEventArgs(prg, Nothing))

                        RemoveOutline(b, fList)

                        isDifferent = CheckIsDifferentGrayscale(b)
                        i += 1
                    Else
                        Exit While
                    End If
                End While

                SetCompleteOutlineToArray(b2, zList)
                Return b2
            End Using
        End If
        Return Nothing
    End Function

    Private Sub SetCompleteOutlineToArray(b2(,) As Integer, zList As List(Of AlphaChain))
        If zList IsNot Nothing AndAlso zList.Count > 0 Then
            For Each a As AlphaChain In zList
                For Each c As ChainCode In a.FList
                    For i As Integer = 0 To c.Coord.Count - 1
                        Dim x As Integer = c.Coord(i).X
                        Dim y As Integer = c.Coord(i).Y

                        b2(x, y) = CByte(Math.Min(a.Alpha, 255))
                    Next
                Next
            Next
        End If
    End Sub

    Private Function CheckIsDifferentGrayscale(bmp As Bitmap) As Boolean
        Dim bmData As BitmapData = Nothing

        Try
            bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.[WriteOnly], PixelFormat.Format32bppArgb)

            Dim w As Integer = bmp.Width
            Dim h As Integer = bmp.Height

            Dim stride As Integer = bmData.Stride

            Dim p((bmData.Stride * bmData.Height) - 1) As Byte
            Marshal.Copy(bmData.Scan0, p, 0, p.Length)
            bmp.UnlockBits(bmData)

            Dim f As Boolean = False

            'For y As Integer = 0 To bmData.Height - 1
            Parallel.For(0, h, Sub(y, loopState)
                                   Dim pos As Integer = y * stride
                                   Dim found As Boolean = False

                                   For x As Integer = 0 To w - 1
                                       Dim value As Integer = p(pos)

                                       If value > 0 Then
                                           found = True
                                           Exit For
                                       End If

                                       pos += 4
                                   Next

                                   If found Then
                                       f = True
                                       loopState.Break()
                                   End If
                                   'Next
                               End Sub)

            p = Nothing

            Return f
        Catch
            Try
                bmp.UnlockBits(bmData)

            Catch
            End Try
        End Try

        Return True
    End Function

    Private Sub DrawCompleteOutlineToBmp(b As Bitmap, zList As List(Of AlphaChain))
        If AvailMem.AvailMem.checkAvailRam(b.Width * b.Height * 4L) Then
            Dim bmData As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
            Dim Scan0 As System.IntPtr = bmData.Scan0

            Dim p((bmData.Stride * bmData.Height) - 1) As Byte
            Marshal.Copy(bmData.Scan0, p, 0, p.Length)

            Dim stride As Integer = bmData.Stride

            If zList IsNot Nothing AndAlso zList.Count > 0 Then
                For Each a As AlphaChain In zList
                    For Each c As ChainCode In a.FList
                        For i As Integer = 0 To c.Coord.Count - 1
                            Dim x As Integer = c.Coord(i).X
                            Dim y As Integer = c.Coord(i).Y

                            p(y * stride + x * 4) = CByte(a.Alpha)
                            p(y * stride + x * 4 + 1) = CByte(a.Alpha)
                            p(y * stride + x * 4 + 2) = CByte(a.Alpha)
                            p(y * stride + x * 4 + 3) = CByte(255)
                        Next
                    Next
                Next
            End If

            Marshal.Copy(p, 0, bmData.Scan0, p.Length)

            b.UnlockBits(bmData)

            p = Nothing
        End If
    End Sub

    Public Sub RemoveOutline(b As Bitmap, fList As List(Of ChainCode))
        If AvailMem.AvailMem.checkAvailRam(b.Width * b.Height * 4L) Then
            Dim bmData As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
            Dim stride As Integer = bmData.Stride

            Dim Scan0 As System.IntPtr = bmData.Scan0

            Dim p((bmData.Stride * bmData.Height) - 1) As Byte
            Marshal.Copy(bmData.Scan0, p, 0, p.Length)

            If fList IsNot Nothing AndAlso fList.Count > 0 Then
                For Each c As ChainCode In fList
                    For i As Integer = 0 To c.Coord.Count - 1
                        Dim x As Integer = c.Coord(i).X
                        Dim y As Integer = c.Coord(i).Y

                        p(y * stride + x * 4) = CType(0, [Byte])
                        p(y * stride + x * 4 + 1) = CType(0, [Byte])
                        p(y * stride + x * 4 + 2) = CType(0, [Byte])
                        p(y * stride + x * 4 + 3) = CType(255, [Byte])
                    Next
                Next
            End If

            Marshal.Copy(p, 0, bmData.Scan0, p.Length)
            b.UnlockBits(bmData)

            p = Nothing
        End If
    End Sub

    Private Sub DrawOutlineToBmp(b As Bitmap, fList As List(Of ChainCode), alpha As Integer)
        If AvailMem.AvailMem.checkAvailRam(b.Width * b.Height * 4L) Then
            Dim bmData As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
            Dim Scan0 As System.IntPtr = bmData.Scan0

            Dim p((bmData.Stride * bmData.Height) - 1) As Byte
            Marshal.Copy(bmData.Scan0, p, 0, p.Length)

            Dim stride As Integer = bmData.Stride

            If fList IsNot Nothing AndAlso fList.Count > 0 Then
                For Each c As ChainCode In fList
                    For i As Integer = 0 To c.Coord.Count - 1
                        Dim x As Integer = c.Coord(i).X
                        Dim y As Integer = c.Coord(i).Y

                        p(y * stride + x * 4) = CByte(alpha)
                        p(y * stride + x * 4 + 1) = CByte(alpha)
                        p(y * stride + x * 4 + 2) = CByte(alpha)
                        p(y * stride + x * 4 + 3) = CByte(255)
                    Next
                Next
            End If

            Marshal.Copy(p, 0, bmData.Scan0, p.Length)

            b.UnlockBits(bmData)

            p = Nothing
        End If
    End Sub

    Public Shared Function Erode(bmp As Bitmap, krnl(,) As Integer, returnBinary As Boolean, th As Integer, bgw As System.ComponentModel.BackgroundWorker, pe As ProgressEventArgs) As Bitmap
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 8L) Then
            Dim bmData As BitmapData = Nothing
            Dim bmDataOut As BitmapData = Nothing

            Dim bOut As Bitmap = Nothing

            Try
                bOut = New Bitmap(bmp.Width, bmp.Height)
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataOut = bOut.LockBits(New Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)
                Dim pTmp((bmDataOut.Stride * bmDataOut.Height) - 1) As Byte
                Marshal.Copy(bmDataOut.Scan0, pTmp, 0, pTmp.Length)

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height
                Dim stride As Integer = bmData.Stride

                Dim rH As Integer = krnl.GetLength(1) \ 2
                Dim cH As Integer = krnl.GetLength(0) \ 2

                If returnBinary Then
                    'For y As Integer = 0 To nHeight - 1
                    Parallel.For(0, nHeight, Sub(y)
                                                 For x As Integer = 0 To nWidth - 1
                                                     Dim b As Byte = 255
                                                     For r = -rH To rH
                                                         If y + r >= 0 AndAlso y + r < nHeight Then
                                                             For c = -cH To cH
                                                                 If x + c >= 0 AndAlso x + c < nWidth Then
                                                                     If krnl(c + cH, r + rH) = 1 Then
                                                                         Dim b2 As Byte = 0
                                                                         If p((r + y) * stride + (c + x) * 4) > th Then
                                                                             b2 = 255
                                                                         End If
                                                                         b = Math.Min(b, p((r + y) * stride + (c + x) * 4))
                                                                     End If
                                                                 End If
                                                             Next
                                                         End If
                                                     Next

                                                     pTmp(y * stride + x * 4) = b
                                                     pTmp(y * stride + x * 4 + 1) = b
                                                     pTmp(y * stride + x * 4 + 2) = b
                                                     pTmp(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3)
                                                 Next
                                             End Sub)
                    'Next
                Else
                    'erode to bTmp
                    'For y As Integer = 0 To nHeight - 1
                    Parallel.For(0, nHeight, Sub(y)
                                                 For x As Integer = 0 To nWidth - 1
                                                     Dim b As Byte = 255
                                                     For r = -rH To rH
                                                         If y + r >= 0 AndAlso y + r < nHeight Then
                                                             For c = -cH To cH
                                                                 If x + c >= 0 AndAlso x + c < nWidth Then
                                                                     If krnl(c + cH, r + rH) = 1 Then
                                                                         b = Math.Min(b, p((r + y) * stride + (c + x) * 4))
                                                                     End If
                                                                 End If
                                                             Next
                                                         End If
                                                     Next

                                                     pTmp(y * stride + x * 4) = b
                                                     pTmp(y * stride + x * 4 + 1) = b
                                                     pTmp(y * stride + x * 4 + 2) = b
                                                     pTmp(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3)
                                                 Next
                                             End Sub)
                    'Next
                End If

                Marshal.Copy(pTmp, 0, bmDataOut.Scan0, pTmp.Length)
                bmp.UnlockBits(bmData)
                bOut.UnlockBits(bmDataOut)

                p = Nothing
                pTmp = Nothing
            Catch ex As Exception
                Try
                    bmp.UnlockBits(bmData)
                Catch

                End Try
                Try
                    bOut.UnlockBits(bmDataOut)
                Catch

                End Try

                If Not bOut Is Nothing Then
                    bOut.Dispose()
                    bOut = Nothing
                End If
            End Try

            Return bOut
        End If
        Return Nothing
    End Function

    Public Shared Function Dilate(bmp As Bitmap, krnl(,) As Integer, returnBinary As Boolean, th As Integer, bgw As System.ComponentModel.BackgroundWorker, pe As ProgressEventArgs) As Bitmap
        If AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 8L) Then
            Dim bmData As BitmapData = Nothing
            Dim bmDataOut As BitmapData = Nothing

            Dim bOut As Bitmap = Nothing

            Try
                bOut = New Bitmap(bmp.Width, bmp.Height)
                bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataOut = bOut.LockBits(New Rectangle(0, 0, bOut.Width, bOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)
                Dim pTmp((bmDataOut.Stride * bmDataOut.Height) - 1) As Byte
                Marshal.Copy(bmDataOut.Scan0, pTmp, 0, pTmp.Length)

                Dim nWidth As Integer = bmp.Width
                Dim nHeight As Integer = bmp.Height
                Dim stride As Integer = bmData.Stride

                Dim rH As Integer = krnl.GetLength(1) \ 2
                Dim cH As Integer = krnl.GetLength(0) \ 2

                If returnBinary Then
                    'For y As Integer = 0 To nHeight - 1
                    Parallel.For(0, nHeight, Sub(y)
                                                 For x As Integer = 0 To nWidth - 1
                                                     Dim b As Byte = 0
                                                     For r = -rH To rH
                                                         If y + r >= 0 AndAlso y + r < nHeight Then
                                                             For c = -cH To cH
                                                                 If x + c >= 0 AndAlso x + c < nWidth Then
                                                                     If krnl(c + cH, r + rH) = 1 Then
                                                                         Dim b2 As Byte = 0
                                                                         If p((r + y) * stride + (c + x) * 4) > th Then
                                                                             b2 = 255
                                                                         End If
                                                                         b = Math.Max(b, p((r + y) * stride + (c + x) * 4))
                                                                     End If
                                                                 End If
                                                             Next
                                                         End If
                                                     Next

                                                     pTmp(y * stride + x * 4) = b
                                                     pTmp(y * stride + x * 4 + 1) = b
                                                     pTmp(y * stride + x * 4 + 2) = b
                                                     pTmp(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3)
                                                 Next
                                             End Sub)
                    'Next
                Else
                    'For y As Integer = 0 To nHeight - 1
                    Parallel.For(0, nHeight, Sub(y)
                                                 For x As Integer = 0 To nWidth - 1
                                                     Dim b As Byte = 0
                                                     For r = -rH To rH
                                                         If y + r >= 0 AndAlso y + r < nHeight Then
                                                             For c = -cH To cH
                                                                 If x + c >= 0 AndAlso x + c < nWidth Then
                                                                     If krnl(c + cH, r + rH) = 1 Then
                                                                         b = Math.Max(b, p((r + y) * stride + (c + x) * 4))
                                                                     End If
                                                                 End If
                                                             Next
                                                         End If
                                                     Next

                                                     pTmp(y * stride + x * 4) = b
                                                     pTmp(y * stride + x * 4 + 1) = b
                                                     pTmp(y * stride + x * 4 + 2) = b
                                                     pTmp(y * stride + x * 4 + 3) = p(y * stride + x * 4 + 3)
                                                 Next
                                             End Sub)
                    'Next
                End If

                Marshal.Copy(pTmp, 0, bmDataOut.Scan0, pTmp.Length)
                bmp.UnlockBits(bmData)
                bOut.UnlockBits(bmDataOut)

                p = Nothing
                pTmp = Nothing
            Catch ex As Exception
                Try
                    bmp.UnlockBits(bmData)
                Catch

                End Try
                Try
                    bOut.UnlockBits(bmDataOut)
                Catch

                End Try

                If Not bOut Is Nothing Then
                    bOut.Dispose()
                    bOut = Nothing
                End If
            End Try

            Return bOut
        End If
        Return Nothing
    End Function
End Class
