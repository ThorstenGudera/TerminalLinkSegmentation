Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Class FFT_related
    Public Shared Function Convolve(ByVal bWork As Bitmap, ByVal fKernel As Double(,), shift As Integer, bgw As System.ComponentModel.BackgroundWorker) As Bitmap
        If bWork Is Nothing OrElse fKernel Is Nothing Then Return Nothing
        Dim bWorkInner As Bitmap = Nothing
        Dim bmpSize As Size = New Size()

        Try
            bWorkInner = MakeBitmap2(CType(bWork, Bitmap), shift)
        Catch
            If bWorkInner IsNot Nothing Then
                bWorkInner.Dispose()
                bWorkInner = Nothing
            End If

            Return Nothing
        End Try

        bmpSize = bWorkInner.Size

        If bgw IsNot Nothing AndAlso bgw.WorkerSupportsCancellation AndAlso bgw.CancellationPending Then
            Return bWorkInner
        End If

        If bgw IsNot Nothing AndAlso bgw.WorkerReportsProgress Then
            bgw.ReportProgress(5)
        End If

        Dim HR As Double(,) = Nothing
        Dim HI As Double(,) = Nothing
        Dim XR As Double(,) = Nothing
        Dim XI As Double(,) = Nothing
        Dim cont As Boolean = False
        XR = GetLuminanceTable(bWorkInner)
        XI = New Double(XR.GetLength(0) - 1, XR.GetLength(1) - 1) {}

        If bgw IsNot Nothing AndAlso bgw.WorkerSupportsCancellation AndAlso bgw.CancellationPending Then
            Return bWorkInner
        End If

        If bgw IsNot Nothing AndAlso bgw.WorkerReportsProgress Then
            bgw.ReportProgress(20)
        End If

        TransformImage(XR, XI)

        If bgw IsNot Nothing AndAlso bgw.WorkerSupportsCancellation AndAlso bgw.CancellationPending Then
            Return bWorkInner
        End If

        If bgw IsNot Nothing AndAlso bgw.WorkerReportsProgress Then
            bgw.ReportProgress(30)
        End If

        If HR Is Nothing AndAlso HI Is Nothing Then
            If fKernel IsNot Nothing Then
                HR = New Double(XR.GetLength(0) - 1, XR.GetLength(1) - 1) {}
                HI = New Double(XR.GetLength(0) - 1, XR.GetLength(1) - 1) {}
                SetupKernel(fKernel, HR, HI)
                TransformKernel(HR, HI)
            End If
        End If

        If bgw IsNot Nothing AndAlso bgw.WorkerSupportsCancellation AndAlso bgw.CancellationPending Then
            Return bWorkInner
        End If

        If bgw IsNot Nothing AndAlso bgw.WorkerReportsProgress Then
            bgw.ReportProgress(40)
        End If

        For y As Integer = 0 To XR.GetLength(1) - 1
            For x As Integer = 0 To XR.GetLength(0) - 1
                Dim tmp As Double = XR(x, y) * HR(x, y) - XI(x, y) * HI(x, y)
                XI(x, y) = XI(x, y) * HR(x, y) + XR(x, y) * HI(x, y)
                XR(x, y) = tmp
            Next
        Next

        ReTransform(XR, XI)

        If bgw IsNot Nothing AndAlso bgw.WorkerSupportsCancellation AndAlso bgw.CancellationPending Then
            Return bWorkInner
        End If

        If bgw IsNot Nothing AndAlso bgw.WorkerReportsProgress Then
            bgw.ReportProgress(80)
        End If

        SetPixelsByHSL(XR, bWorkInner, False)

        If bgw IsNot Nothing AndAlso bgw.WorkerReportsProgress Then
            bgw.ReportProgress(100)
        End If

        Return bWorkInner
    End Function

    Public Shared Function MakeBitmap2(bitmap As Bitmap, shift As Integer) As Bitmap
        Dim w As Integer = bitmap.Width
        Dim h As Integer = bitmap.Height
        Dim bmp As Bitmap = Nothing

        Try
            Dim w2 As Integer = w + shift * 2
            Dim h2 As Integer = h + shift * 2
            If AvailMem.AvailMem.checkAvailRam(w2 * h2 * 4L) Then
                bmp = New Bitmap(w2, h2)
            End If

            Using g As Graphics = Graphics.FromImage(bmp)
                Using tb As New TextureBrush(bitmap)
                    tb.WrapMode = System.Drawing.Drawing2D.WrapMode.TileFlipXY
                    tb.TranslateTransform(shift, shift)
                    g.FillRectangle(tb, New Rectangle(0, 0, bmp.Width, bmp.Height))
                End Using
            End Using

            'Dim ffff As New Form
            'ffff.Text = "makeBitmap2"
            'ffff.BackgroundImage = bmp
            'ffff.BackgroundImageLayout = ImageLayout.Zoom
            'ffff.ShowDialog()
        Catch
            If Not bmp Is Nothing Then
                bmp.Dispose()
                bmp = Nothing
            End If
        End Try
        Return bmp
    End Function

    Public Shared Function GetLuminanceTable(bitmap As Bitmap) As Double(,)
        Dim f As Integer = CheckWH(bitmap)

        Dim d As Double(,) = New Double(f - 1, f - 1) {}
        Dim w As Integer = d.GetLength(0)
        Dim h As Integer = d.GetLength(1)

        'for(int y = 0; y < d.GetLength(1); y++)
        Parallel.[For](0, h, Sub(y)
                                 For x As Integer = 0 To w - 1
                                     d(x, y) = 0
                                 Next
                             End Sub)

        For y As Integer = 0 To bitmap.Height - 1
            For x As Integer = 0 To bitmap.Width - 1
                d(x, y) = bitmap.GetPixel(x, y).GetBrightness()
            Next
        Next

        Return d
    End Function

    Public Shared Function CheckWH(bitmap As Bitmap) As Integer
        Dim i As Integer = Math.Max(bitmap.Width, bitmap.Height)
        Dim z As Integer = CInt(Math.Ceiling(Math.Log(i, 2)))

        Return CInt(Math.Pow(2, z))
    End Function

    Public Shared Function CheckWH(sz As Size) As Integer
        Dim i As Integer = Math.Max(sz.Width, sz.Height)
        Dim z As Integer = CInt(Math.Ceiling(Math.Log(i, 2)))

        Return CInt(Math.Pow(2, z))
    End Function

    Public Shared Sub TransformImage(XR As Double(,), XI As Double(,))
        Dim w As Integer = XI.GetLength(0)
        Dim h As Integer = XI.GetLength(1)

        'for (int l = 0; l < XI.GetLength(1); l++)
        Parallel.[For](0, h, Sub(l)
                                 For j As Integer = 0 To w - 1
                                     XI(j, l) = 0
                                 Next
                             End Sub)

        'for (int l = 0; l < XR.GetLength(1); l++)
        Parallel.[For](0, h, Sub(l)
                                 Dim r As Double() = New Double(XR.GetLength(0) - 1) {}
                                 Dim i As Double() = New Double(XR.GetLength(0) - 1) {}

                                 For j As Integer = 0 To r.Length - 1
                                     r(j) = XR(j, l)
                                     i(j) = 0
                                 Next

                                 FFT(r, i, False)

                                 For j As Integer = 0 To r.Length - 1
                                     XR(j, l) = r(j)
                                     XI(j, l) = i(j)
                                 Next
                             End Sub)

        'for (int j = 0; j < XR.GetLength(0); j++)
        Parallel.[For](0, w, Sub(j)
                                 Dim r As Double() = New Double(XR.GetLength(1) - 1) {}
                                 Dim i As Double() = New Double(XR.GetLength(1) - 1) {}

                                 For l As Integer = 0 To r.Length - 1
                                     r(l) = XR(j, l)
                                     i(l) = XI(j, l)
                                 Next

                                 FFT(r, i, False)

                                 For l As Integer = 0 To r.Length - 1
                                     XR(j, l) = r(l)
                                     XI(j, l) = i(l)
                                 Next

                             End Sub)
    End Sub

    Private Shared Sub SetupKernel(kernel2F As Double(,), HR As Double(,), HI As Double(,))
        Dim n2 As Integer = kernel2F.GetLength(0) \ 2

        'shift the kernel-parts to the corners of the HR-array,
        'so the output will not get shifted, the kernel is symmetric on sample 0
        For i As Integer = 0 To HI.GetLength(0) - 1
            For ii As Integer = 0 To HI.GetLength(1) - 1
                HR(i, ii) = 0

                If (kernel2F.GetLength(0) And &H1) = 1 Then
                    'odd * odd kenrels
                    If i <= n2 AndAlso ii <= n2 Then
                        HR(i, ii) = kernel2F(i + n2, ii + n2)
                    End If

                    If i > n2 AndAlso i >= HR.GetLength(0) - n2 AndAlso ii < n2 Then
                        HR(i, ii) = kernel2F(i + n2 - HR.GetLength(0), ii + n2)
                    End If

                    If i <= n2 AndAlso ii > n2 AndAlso ii >= HR.GetLength(1) - n2 Then
                        HR(i, ii) = kernel2F(i + n2, ii + n2 - HR.GetLength(1))
                    End If

                    If i > n2 AndAlso i >= HR.GetLength(0) - n2 AndAlso ii > n2 AndAlso ii >= HR.GetLength(1) - n2 Then
                        HR(i, ii) = kernel2F(i + n2 - HR.GetLength(0), ii + n2 - HR.GetLength(1))
                    End If
                Else
                    'even * even kernels

                    'lefttop centered kernels
                    'if (i <= n2 && ii <= n2)
                    '    HR[i, ii] = kernel2F[i + n2 - 1, ii + n2 - 1];

                    'if (i > n2 && i >= HR.GetLength(0) - n2 + 1 &&
                    '    ii <= n2)
                    '    HR[i, ii] = kernel2F[i + n2 - 1 - HR.GetLength(0), ii + n2 - 1];

                    'if (i <= n2 && ii > n2 &&
                    '    ii >= HR.GetLength(1) - n2 + 1)
                    '    HR[i, ii] = kernel2F[i + n2 - 1, ii + n2 - 1 - HR.GetLength(1)];

                    'if (i > n2 && i >= HR.GetLength(0) - n2 + 1 &&
                    '    ii > n2 && ii >= HR.GetLength(1) - n2 + 1)
                    '    HR[i, ii] = kernel2F[i + n2 - 1 - HR.GetLength(0), ii + n2 - 1 - HR.GetLength(1)];

                    'normal centered kernels
                    If i < n2 AndAlso ii < n2 Then
                        HR(i, ii) = kernel2F(i + n2, ii + n2)
                    End If

                    If i >= n2 AndAlso i >= HR.GetLength(0) - n2 AndAlso ii < n2 Then
                        HR(i, ii) = kernel2F(i + n2 - HR.GetLength(0), ii + n2)
                    End If

                    If i < n2 AndAlso ii >= n2 AndAlso ii >= HR.GetLength(1) - n2 Then
                        HR(i, ii) = kernel2F(i + n2, ii + n2 - HR.GetLength(1))
                    End If

                    If i >= n2 AndAlso i >= HR.GetLength(0) - n2 AndAlso ii >= n2 AndAlso ii >= HR.GetLength(1) - n2 Then
                        HR(i, ii) = kernel2F(i + n2 - HR.GetLength(0), ii + n2 - HR.GetLength(1))
                    End If
                End If

                HI(i, ii) = 0
            Next
        Next
    End Sub

    Public Shared Sub TransformKernel(kernel As Double(,), kernelI As Double(,))
        Dim w As Integer = kernel.GetLength(0)
        Dim h As Integer = kernel.GetLength(1)

        'for (int l = 0; l < XR.GetLength(1); l++)
        Parallel.[For](0, h, Sub(l)
                                 Dim r As Double() = New Double(kernel.GetLength(0) - 1) {}
                                 Dim i As Double() = New Double(kernel.GetLength(0) - 1) {}

                                 For j As Integer = 0 To r.Length - 1
                                     r(j) = kernel(j, l)
                                     i(j) = 0
                                 Next

                                 FFT(r, i, False)

                                 For j As Integer = 0 To r.Length - 1
                                     kernel(j, l) = r(j)
                                     kernelI(j, l) = i(j)
                                 Next

                             End Sub)

        'for (int j = 0; j < XR.GetLength(0); j++)
        Parallel.[For](0, w, Sub(j)
                                 Dim r As Double() = New Double(kernel.GetLength(1) - 1) {}
                                 Dim i As Double() = New Double(kernel.GetLength(1) - 1) {}

                                 For l As Integer = 0 To r.Length - 1
                                     r(l) = kernel(j, l)
                                     i(l) = kernelI(j, l)
                                 Next

                                 FFT(r, i, False)

                                 For l As Integer = 0 To r.Length - 1
                                     kernel(j, l) = r(l)
                                     kernelI(j, l) = i(l)
                                 Next

                             End Sub)
    End Sub

    Public Shared Sub ReTransform(XR As Double(,), XI As Double(,))
        Dim w As Integer = XR.GetLength(0)
        Dim h As Integer = XR.GetLength(1)

        'for (int j = 0; j < XR.GetLength(0); j++)
        Parallel.[For](0, w, Sub(j)
                                 Dim r As Double() = New Double(XR.GetLength(1) - 1) {}
                                 Dim i As Double() = New Double(XR.GetLength(1) - 1) {}

                                 For l As Integer = 0 To r.Length - 1
                                     r(l) = XR(j, l)
                                     i(l) = XI(j, l)
                                 Next

                                 FFT(r, i, True)

                                 For l As Integer = 0 To r.Length - 1
                                     XR(j, l) = r(l)
                                     XI(j, l) = i(l)
                                 Next

                             End Sub)

        'for (int l = 0; l < XR.GetLength(1); l++)
        Parallel.[For](0, h, Sub(l)
                                 Dim r As Double() = New Double(XR.GetLength(0) - 1) {}
                                 Dim i As Double() = New Double(XR.GetLength(0) - 1) {}

                                 For j As Integer = 0 To r.Length - 1
                                     r(j) = XR(j, l)
                                     i(j) = XI(j, l)
                                 Next

                                 FFT(r, i, True)

                                 For j As Integer = 0 To r.Length - 1
                                     XR(j, l) = r(j)
                                     XI(j, l) = i(j)
                                 Next

                             End Sub)
    End Sub

    'This is a port of the FFT-Method shown in the book
    '"The Scientist and Engineer's Guide to Digital Signal Processing". 
    'A textbook of DSP techniques by Steven W. Smith, 1997, Chapter 12, Page 235 (f) 
    'http://www.dspguide.com/
    'Notifier from the downloadable Textfile containing the programs:
    '(http://www.dspguide.com/programs.txt)
    '"Programs from: "The Scientist and Engineer's Guide to Digital Signal
    'Processing." Visit our website at:  www.dspguide.com.  All these programs may
    'be copied, distributed, and used for any noncommercial purpose."
    Public Shared Sub FFT(Re As Double(), Im As Double(), reverse As Boolean)
        If Re.Length <> Im.Length Then
            Throw New Exception("Arrays must be of same length")
        End If

        'we repeatedly rewrite Sum(n, 0, N-1)[x[n]*e^(-2Piikn/N)] (where k runs over all frequencies 0 to N-1) to Sum(n, even)[x[n]*e^(-2Piikn/N)] + Sum(n, odd)[x[n]*e^(-2Piikn/N)];
        'which is the same as Sum(r, 0, N/2-1)[x[2r]*e^(-2Piik(2r)/N)] + Sum(r, 0, N/2-1)[x[2r + 1]*e^(-2Piik(2r + 1)/N)], where of the last term
        'we can factor out of(2r + 1) the "1"part: Sum(r, 0, N/2-1)[x[2r + 1]*e^(-2Piik(2r)/N)*e^(-2Piik/N)], which we can pull before the sum, 
        'since its not related to n (or here: r): e^(-2Piik/N) * Sum(r, 0, N/2-1)[x[2r + 1]*e^(-2Piik(2r)/N)], so we get:
        'Sum(n, 0, N-1)[x[n]*e^(-2Piikn/N)] = Sum(r, 0, N/2-1)[x[2r]*e^(-2Piik(2r)/N)] + e^(-2Piik/N) * Sum(r, 0, N/2-1)[x[2r + 1]*e^(-2Piik(2r)/N)]
        'now e^(-2Piik(2r)/N) is just e^(-2Pii/N*2*kr), which is, (take the 2 from the numerator and put it into the denominator), e^(-2Pii/(N/2) * kr).
        'so finally we get: 
        'X[k] = Sum(n, 0, N-1)[x[n]*e^(-2Piikn/N)] = Sum(r, 0, N/2-1)[x[2r]*e^(-2Piikr/(N/2)) + e^(-2Piik/N) * Sum(r, 0, N/2-1)[x[2r + 1]*e^(-2Piikr/(N/2))]]
        'But Sum(r, 0, N/2-1)[x[2r]*e^(-2Piikr/(N/2)) is just the (N / 2) DFT for the even samples and so for the odd ones (times the factored out factor)
        'So the DFT X[k] = Sum(n, 0, N-1)[x[n]*e^(-2Piikn/N)] is just the sum of X_even[k] + e^(-2Piik/N) * X_odd[k].
        '(A sum of 2 N/2 point DFTs represents the N point DFT)
        'We repeat this (from 2 N/2 point DFTs to 4 N/4 point DFTs etc.) until r is constant (= a sum of N 1 point DFTs), so the sum vanishes, 
        'and we are already in the frequency domain, since we get terms like coeffizient1*BasisvectorK1 + coeffizient2*BasisvectorK2 etc 
        'which is the target representation (of the DFT).

        'Another interresting point: Remember linear Algebra: The terms coeffizient1*BasisvectorK1 just stretch the Amplitude of the BaseVector,
        'so this indicates that we might really have a Spectrum, think of Eigenvalues and Eigenvectors. 

        'In the algorithm we use the trick with the bit-reversal of the order of samples: (0 0 0) + (1 0 0) + (0 1 0) + (1 1 0) for 4 even samples (0, 4, 2, 6)
        'and (0 0 1) + (1 0 1) + (0 1 1) + (1 1 1) for the odd ones (1, 5, 3, 7)

        'Philosophically we reduce complexity by increasing entropie -> therefore the second part, we must "clean up" and bring the samples into
        'its initial sequence by applying the "Butterfly" for each sample, for each chunk, for each stage (no. stages = log2N).

        Dim nmi As Integer = Re.Length - 1
        Dim nd2 As Integer = Re.Length \ 2

        Dim z As Integer = CInt(Math.Ceiling(Math.Log(Re.Length, 2)))
        Dim j2 As Integer = nd2
        'MessageBox.Show(z.ToString() + "\r\n" + Math.Log(Re.Length, 2).ToString());

        If Re.Length < Math.Pow(2, z) Then
            FillWithZeros(Re, Im, z)
        End If

        If reverse Then
            For k As Integer = 0 To Re.Length - 1
                Im(k) = -Im(k)
            Next
        End If

        'bit reversal
        For i As Integer = 1 To Re.Length - 2
            If i < j2 Then
                Dim tr As Double = Re(j2)
                Dim ti As Double = Im(j2)

                Re(j2) = Re(i)
                Im(j2) = Im(i)

                Re(i) = tr
                Im(i) = ti
            End If

            Dim k As Integer = nd2

            While k <= j2
                j2 -= k
                k \= 2
            End While

            j2 += k
        Next

        'reorder
        'What we do here, is to dilute the two N/2 point DFTs with zeros and shift the odd ones 1 sample to the right.
        'Then we add both (and repeat this for all samples, for all chunks, for all stages).
        'Even: 0246 -> 00204060, Odd: 1357 -> 01030507, add even and odd 00204060 + 01030507 = 01234567. 
        'This way we restore the initial order of the samples.
        'Of course, the above has to be done in freuquncy domain, since we already are in it.
        'For this, we double the samples in frequency and mutliply the odd samples with a sinusoid.

        'Recall: 
        'Diluting with zeros in time is duplicating the samples in frequency domain. (A 2 point signal [1, -1] corresponds to a frequency of 1. 
        'With zeros [1, 0, -1, 0] it represents a frequency of 1 plus a frequency of "three" (= -1) (phase shifted by Pi/2)).

        '[
        '4 pt signal: [1, 0, -1, 0](Math.Cos(2pi*n/4*f) or negative:(Math.Cos(2pi*(1-n)/4)*f))(f=1 or f=-1)[neg. phase shifted] -> 
        '[1, 0, 0, 0, -1, 0, 0, 0] (f=1 + f=3 or neg. f=-1 + f=-3) = Math.Cos(2 * Math.PI * n / 8.0 * 1) + Math.Cos(2 * Math.PI * n / 8.0 * 3) / 2.0 or
        'neg. Math.Cos(2 * Math.PI * (1-n) / 8.0 * 1) + Math.Cos(2 * Math.PI * (1-n) / 8.0 * 3) / 2.0 [neg. phase shifted]
        ']

        'A shift in time domain is done by convolving with a shifted delta kernel. 
        '(Just look, what convolving with a kernel 0 [0 0 0 0 1] does to your signal and in which direction it gets shifted).
        'A Delta impuls in time corresponds to a sinusoid in frequency. 
        'Because its easy to see that a sinusoid in time corresponds to a delta function in frequency (a single frequency) 
        'and the DFT is in both directions the same (except of a factor), this just has to be so.
        'Remember that a sinusoid in freuquency is a multi frequency signal, just wave - shaped (not just one single oscillating wave)!

        'So we have to duplicate the samples and multiply the odd ones by a sinusoid of correct frequencies. Then add both.
        'Since duplicating will add or subtract from existing values because we add two chunks of
        'the same amount of samples in one "per samples" pass, this will result in "the Butterfly":
        'even: 0246 -> dupl -> 02460246, odd:  1357 -> mult by sinusoid -> dupl -> 13571357 and add.

        '2 point input
        ' Odd        Even
        '  *sin       |
        '   |    x    |
        '  *-1        |
        '   +         +
        '2 point output ("x" means to pass overcross (the odd result to the even flow-thread and vice versa)).

        For l As Integer = 1 To z
            Dim lv As Integer = CInt(Math.Pow(2, l))
            Dim lv2 As Integer = lv \ 2

            Dim ur As Double = 1
            Dim ui As Double = 0

            Dim sr As Double = Math.Cos(Math.PI / lv2)
            Dim si As Double = -Math.Sin(Math.PI / lv2)

            For j As Integer = 1 To lv2
                Dim j1 As Integer = j - 1
                Dim i As Integer = j1
                While i <= nmi 'https://en.wikipedia.org/wiki/Butterfly_diagram
                    Dim ip As Integer = i + lv2
                    Dim tr As Double = Re(ip) * ur - Im(ip) * ui 'sinusoid multiplication (= shift of C and D in time domain [AB -> A0B0] + CD -> 0C0D) (AB -> A0B0 and diluting with zeros of CD is doubling in frequency domain)
                    Dim ti As Double = Re(ip) * ui + Im(ip) * ur 'shift in time domain corresponds to convolving with a shifted delta function (which in frequency domain is multiplying by a sinusoid)

                    Re(ip) = Re(i) - tr
                    Im(ip) = Im(i) - ti

                    Re(i) = Re(i) + tr
                    Im(i) = Im(i) + ti
                    i += lv
                End While

                Dim t As Double = ur
                ur = t * sr - ui * si 'sinusoid *value* (or sample) of correct frequencies 
                ui = t * si + ui * sr
            Next
        Next

        If reverse Then
            For i As Integer = 0 To Re.Length - 1
                Re(i) /= Re.Length
                Im(i) /= -Re.Length
            Next
        End If
    End Sub

    Public Shared Sub FillWithZeros(Re As Double(), Im As Double(), z As Integer)
        Throw New NotImplementedException()
    End Sub

    Public Shared Sub SetPixelsByHSL(XR As Double(,), bmp As Bitmap, setOpaque As Boolean)
        Dim bmData As BitmapData = Nothing

        Try
            If Not AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
                Return
            End If

            bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

            Dim w As Integer = bmp.Width
            Dim h As Integer = bmp.Height

            Dim p((bmData.Stride * bmData.Height) - 1) As Byte
            Marshal.Copy(bmData.Scan0, p, 0, p.Length)

            'for (int y = 0; y < bmp.Height; y++)
            Parallel.[For](0, h, Sub(y)
                                     For x As Integer = 0 To w - 1
                                         Dim l As Double = XR(x, y)

                                         If Double.IsInfinity(l) OrElse Double.IsNaN(l) Then
                                             l = 0
                                         End If

                                         Dim hsl As HSLData = RGBtoHSL(p(y * bmData.Stride + x * 4 + 2), p(y * bmData.Stride + x * 4 + 1), p(y * bmData.Stride + x * 4))
                                         Dim pid As PixelData = HSLtoRGB(hsl.Hue, hsl.Saturation, Math.Max(Math.Min(l, 1.0), 0))

                                         If setOpaque Then
                                             p(y * bmData.Stride + x * 4 + 3) = CByte(255)
                                         End If
                                         p(y * bmData.Stride + x * 4 + 2) = pid.red
                                         p(y * bmData.Stride + x * 4 + 1) = pid.green
                                         p(y * bmData.Stride + x * 4) = pid.blue
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

        RGB.red = CByte(CInt(Math.Floor(r)))
        RGB.green = CByte(CInt(Math.Floor(g)))
        RGB.blue = CByte(CInt(Math.Floor(b)))

        Return RGB
    End Function

    Public Structure PixelData
        Public blue As Byte
        Public green As Byte
        Public red As Byte
        Public alpha As Byte
    End Structure

    Public Structure HSLData
        Public Hue As Single
        Public Saturation As Single
        Public Luminance As Single
    End Structure
End Class
