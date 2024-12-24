Option Strict On
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports ChainCodeFinder

Public Class BlendAlgorithmGaussSeidelSOR
    Implements IBlendAlgorithm

    Public Event ShowInfo As EventHandler(Of String) Implements IBlendAlgorithm.ShowInfo
    Public Event ShowProgess As EventHandler(Of ProgressEventArgs) Implements IBlendAlgorithm.ShowProgess
    Public Event BoundaryError As EventHandler(Of String) Implements IBlendAlgorithm.BoundaryError

    Private _lockObject As New Object

    Public Sub New()

    End Sub

    Public Property CancellationPending As Boolean Implements IBlendAlgorithm.CancellationPending
        Get
            Return _cancellationPending
        End Get
        Set(value As Boolean)
            _cancellationPending = value
        End Set
    End Property
    Private _cancellationPending As Boolean = False

    Public Property BlendParameters As BlendParameters Implements IBlendAlgorithm.BlendParameters
        Get
            Return _blendParameters
        End Get
        Set(value As BlendParameters)
            _blendParameters = value
        End Set
    End Property
    Private _blendParameters As BlendParameters = Nothing

    Public Property BGW As BackgroundWorker Implements IBlendAlgorithm.BGW
        Get
            Return Me._bgw
        End Get
        Set(value As BackgroundWorker)
            Me._bgw = value
        End Set
    End Property

    Private _bgw As BackgroundWorker

    Private Function ComputeWForSOR(computeWeightForSOR As Boolean, autoSORMode As AutoSORMode, pixelsCount As Integer, upperImgWidth As Integer, upperImgHeight As Integer, minPxAmount As Integer) As Double
        If Not computeWeightForSOR Then
            Dim upperImgAmount As Double = upperImgWidth * 4.0 / 3.0
            Return Math.Max(Math.Min(2 / (1 + Math.Sin(Math.PI / (Math.Sqrt(upperImgAmount)))), 2), 1)
        Else
            If autoSORMode = AutoSORMode.OneAndAHalf Then
                Return 1.5
            Else
                'note that this is stronger than in the Multigrid implementation
                If minPxAmount = 0 Then
                    minPxAmount = 12
                End If
                pixelsCount = Math.Max(pixelsCount, minPxAmount)
                Dim ratio As Double = pixelsCount / (upperImgWidth * upperImgHeight)
                Dim upperImgAmount As Double = upperImgWidth * ratio
                If autoSORMode = AutoSORMode.SqrtWidthRelated Then
                    upperImgAmount = Math.Sqrt(upperImgAmount)
                    upperImgAmount *= 12
                End If
                Return Math.Max(Math.Min(2 / (1 + Math.Sin(Math.PI / upperImgAmount)), 2), 1)
            End If
        End If
    End Function

    Public Sub Apply(rc As Rectangle) Implements IBlendAlgorithm.Apply
        'Jacobi/GaussSeidel

        'mask: Stores the coordinates and the state (inside, boundary or outside of the region).
        'Region is the arbitrary shape of the upper image (based on alpha values. Alpha = 0 is outside)
        'We need this mapping of the coordinates just *because* of the arbitrary shape, since the vector that contains
        'all pixels to process is not a rectangular mapping, and so, the length a "row" or "column" could differ 
        'from the length of other rows or columns.
        Dim mask(_blendParameters.UpperImg.Width * _blendParameters.UpperImg.Height - 1) As Integer

        'get the set of pixels to process
        Dim pixels As List(Of Point) = GetPixels(_blendParameters.UpperImg, _blendParameters.LowerImg, rc, mask, _blendParameters.MinAlpha)

        Dim bmData As BitmapData = Nothing
        Dim bmSrc As BitmapData = Nothing

        'note, I use fields for these here in this project
        'these store the system to solve
        'Dim results(2)() As Double 'keep the results for further processing
        'Dim diagonalElements(2)() As Integer '4 for inner pixels, <4 for pixels that touch the boundary
        'Dim otherElements(2)()() As Integer 'the indices for the -1 entries

        'the algorithm to compute the differences of the "b" vector
        Dim bVectorAlg As IBVectorComputingAlgorithm = _blendParameters.BVectorAlg 'BVectorComputingAlgorithm.GetAlgorithm(_blendParameters.Params.Algorithm)

        Dim c2 As List(Of ChainCode) = GetBoundary(_blendParameters.UpperImg, _blendParameters.MinAlpha)
        bVectorAlg.SetValuesSilent(_blendParameters.LowerImgWeight, _blendParameters.UpperImgWeight)

        _blendParameters.Pixels = pixels
        _blendParameters.RC = rc

        'Dim w As Double = 2 / (1 + Math.Sin(Math.PI / _blendParameters.UpperImg.Width))
        Dim w As Double = Me.ComputeWForSOR(_blendParameters.AutoSOR, _blendParameters.AutoSORMode, pixels.Count,
                                            _blendParameters.UpperImg.Width, _blendParameters.UpperImg.Height, _blendParameters.MinPixelAmount)

        OnShowProgress(20)

        Try
            If AvailMem.AvailMem.checkAvailRam(_blendParameters.UpperImg.Width * _blendParameters.UpperImg.Height * 4L + _blendParameters.LowerImg.Width * _blendParameters.LowerImg.Height * 4L) Then
                bmData = _blendParameters.UpperImg.LockBits(New Rectangle(0, 0, _blendParameters.UpperImg.Width, _blendParameters.UpperImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmSrc = _blendParameters.LowerImg.LockBits(New Rectangle(0, 0, _blendParameters.LowerImg.Width, _blendParameters.LowerImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim scanline As Integer = bmData.Stride
                Dim nWidth As Integer = _blendParameters.UpperImg.Width
                Dim nHeight As Integer = _blendParameters.UpperImg.Height

                Dim scanlineL As Integer = bmSrc.Stride
                Dim nWidthL As Integer = _blendParameters.LowerImg.Width
                Dim nHeightL As Integer = _blendParameters.LowerImg.Height

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)

                Dim pL((bmSrc.Stride * bmSrc.Height) - 1) As Byte
                Marshal.Copy(bmSrc.Scan0, pL, 0, pL.Length)

                'For plane As Integer = 0 To 2
                Parallel.For(0, 3, Sub(plane, loopState)
                                       'matrix  elements
                                       Dim n(pixels.Count - 1) As Integer
                                       Dim r(pixels.Count - 1)() As Integer

                                       'b-vector
                                       Dim b(pixels.Count - 1) As Double

                                       'here we setup the system, put all boundary values and the known-function values (differences between pixels, "finite differences") to b.
                                       'and also set up the sparse system by putting the factor for the central pixel to the n-array and the off diagonal elements 
                                       '(the indices of the surrounding pixels that in the matrix have a value of -1) to the r-array.
                                       GetSystem(p, pL, scanline, scanlineL, pixels, nWidth, nHeight, mask, n, r, b, rc, plane, bVectorAlg)

                                       'jacobi method to solve
                                       Dim result As Double() = GaussSeidelForPoissonEq_Div(b, n, r, _blendParameters.JacobiGSParams.MaxIterations, True, _blendParameters.JacobiGSParams.DesiredMaxLinearError,
                                                                                   _blendParameters.PE, w, plane)
                                       'make the results re-usable for further iterations
                                       _blendParameters.Results(plane) = result
                                       _blendParameters.DiagonalElements(plane) = n
                                       _blendParameters.OtherElements(plane) = r

                                       _blendParameters.BVector(plane) = b
                                   End Sub)
                'Next

                OnShowProgress(_blendParameters.PE.ImgWidthHeight)

                'write the values back to the image
                InsertPixelsForAllPlanes(pL, scanlineL, _blendParameters.Results, pixels, _blendParameters.RC)

                Marshal.Copy(pL, 0, bmSrc.Scan0, pL.Length)
                _blendParameters.UpperImg.UnlockBits(bmData)
                _blendParameters.LowerImg.UnlockBits(bmSrc)
                p = Nothing
                pL = Nothing
            End If
        Catch exc As Exception
            Try
                _blendParameters.UpperImg.UnlockBits(bmData)
            Catch

            End Try
            Try
                _blendParameters.LowerImg.UnlockBits(bmSrc)
            Catch

            End Try
        Finally
            _blendParameters.UpperImg.Dispose()
            _blendParameters.UpperImg = Nothing
        End Try
    End Sub

    Public Sub ApplyG(rc As Rectangle) Implements IBlendAlgorithm.ApplyG
        'Jacobi/GaussSeidel

        Using uI As Bitmap = New Bitmap(_blendParameters.UpperImg)
            Grayscale(uI)

            'mask: Stores the coordinates and the state (inside, boundary or outside of the region).
            'Region is the arbitrary shape of the upper image (based on alpha values. Alpha = 0 is outside)
            'We need this mapping of the coordinates just *because* of the arbitrary shape, since the vector that contains
            'all pixels to process is not a rectangular mapping, and so, the length a "row" or "column" could differ 
            'from the length of other rows or columns.
            Dim mask(uI.Width * uI.Height - 1) As Integer

            'get the set of pixels to process
            Dim pixels As List(Of Point) = GetPixels(uI, _blendParameters.LowerImg, rc, mask, _blendParameters.MinAlpha)

            Dim bmData As BitmapData = Nothing
            Dim bmSrc As BitmapData = Nothing

            'note, I use fields for these here in this project
            'these store the system to solve
            'Dim results(2)() As Double 'keep the results for further processing
            'Dim diagonalElements(2)() As Integer '4 for inner pixels, <4 for pixels that touch the boundary
            'Dim otherElements(2)()() As Integer 'the indices for the -1 entries

            'the algorithm to compute the differences of the "b" vector
            Dim bVectorAlg As IBVectorComputingAlgorithm = _blendParameters.BVectorAlg 'BVectorComputingAlgorithm.GetAlgorithm(_blendParameters.Params.Algorithm)

            Dim c2 As List(Of ChainCode) = GetBoundary(uI, _blendParameters.MinAlpha)
            bVectorAlg.SetValuesSilent(_blendParameters.LowerImgWeight, _blendParameters.UpperImgWeight)

            _blendParameters.Pixels = pixels
            _blendParameters.RC = rc

            'Dim w As Double = 2 / (1 + Math.Sin(Math.PI / uI.Width))
            Dim w As Double = Me.ComputeWForSOR(_blendParameters.AutoSOR, _blendParameters.AutoSORMode, pixels.Count,
                                                uI.Width, uI.Height, _blendParameters.MinPixelAmount)

            OnShowProgress(20)

            Try
                If AvailMem.AvailMem.checkAvailRam(uI.Width * uI.Height * 4L + _blendParameters.LowerImg.Width * _blendParameters.LowerImg.Height * 4L) Then
                    bmData = uI.LockBits(New Rectangle(0, 0, uI.Width, uI.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                    bmSrc = _blendParameters.LowerImg.LockBits(New Rectangle(0, 0, _blendParameters.LowerImg.Width, _blendParameters.LowerImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                    Dim scanline As Integer = bmData.Stride
                    Dim nWidth As Integer = uI.Width
                    Dim nHeight As Integer = uI.Height

                    Dim scanlineL As Integer = bmSrc.Stride
                    Dim nWidthL As Integer = _blendParameters.LowerImg.Width
                    Dim nHeightL As Integer = _blendParameters.LowerImg.Height

                    Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                    Marshal.Copy(bmData.Scan0, p, 0, p.Length)

                    Dim pL((bmSrc.Stride * bmSrc.Height) - 1) As Byte
                    Marshal.Copy(bmSrc.Scan0, pL, 0, pL.Length)

                    'For plane As Integer = 0 To 2
                    Parallel.For(0, 3, Sub(plane, loopState)
                                           'matrix  elements
                                           Dim n(pixels.Count - 1) As Integer
                                           Dim r(pixels.Count - 1)() As Integer

                                           'b-vector
                                           Dim b(pixels.Count - 1) As Double

                                           'here we setup the system, put all boundary values and the known-function values (differences between pixels, "finite differences") to b.
                                           'and also set up the sparse system by putting the factor for the central pixel to the n-array and the off diagonal elements 
                                           '(the indices of the surrounding pixels that in the matrix have a value of -1) to the r-array.
                                           GetSystem(p, pL, scanline, scanlineL, pixels, nWidth, nHeight, mask, n, r, b, rc, plane, bVectorAlg)

                                           'jacobi method to solve
                                           Dim result As Double() = GaussSeidelForPoissonEq_Div(b, n, r, _blendParameters.JacobiGSParams.MaxIterations, True, _blendParameters.JacobiGSParams.DesiredMaxLinearError,
                                                                                       _blendParameters.PE, w, plane)
                                           'make the results re-usable for further iterations
                                           _blendParameters.Results(plane) = result
                                           _blendParameters.DiagonalElements(plane) = n
                                           _blendParameters.OtherElements(plane) = r

                                           _blendParameters.BVector(plane) = b
                                       End Sub)
                    'Next

                    OnShowProgress(_blendParameters.PE.ImgWidthHeight)

                    'write the values back to the image
                    InsertPixelsForAllPlanes(pL, scanlineL, _blendParameters.Results, pixels, _blendParameters.RC)

                    Marshal.Copy(pL, 0, bmSrc.Scan0, pL.Length)
                    uI.UnlockBits(bmData)
                    _blendParameters.LowerImg.UnlockBits(bmSrc)
                    p = Nothing
                    pL = Nothing
                End If
            Catch exc As Exception
                Try
                    uI.UnlockBits(bmData)
                Catch

                End Try
                Try
                    _blendParameters.LowerImg.UnlockBits(bmSrc)
                Catch

                End Try
            Finally
                _blendParameters.UpperImg.Dispose()
                _blendParameters.UpperImg = Nothing
            End Try
        End Using
    End Sub

    Public Sub ApplyG2(rc As Rectangle) Implements IBlendAlgorithm.ApplyG2
        'Jacobi/GaussSeidel

        If _blendParameters.FeatherIntermediatePics AndAlso
            (_blendParameters.BVectorAlg.GetType().Equals(GetType(GradAddDynamicWeightAlgorithm)) OrElse
            _blendParameters.BVectorAlg.GetType().Equals(GetType(GradNormalDynamicWeightAlgorithm))) AndAlso
            Not _blendParameters.UseOrigForFeathering Then

            ApplyG4(rc)
        Else
            Using uI As Bitmap = New Bitmap(_blendParameters.UpperImg), lI As Bitmap = New Bitmap(_blendParameters.LowerImg)
                If _blendParameters.GrayscaleIntermediatePic Then
                    Grayscale(uI)
                End If

                'mask: Stores the coordinates and the state (inside, boundary or outside of the region).
                'Region is the arbitrary shape of the upper image (based on alpha values. Alpha = 0 is outside)
                'We need this mapping of the coordinates just *because* of the arbitrary shape, since the vector that contains
                'all pixels to process is not a rectangular mapping, and so, the length a "row" or "column" could differ 
                'from the length of other rows or columns.
                Dim mask(uI.Width * uI.Height - 1) As Integer

                'get the set of pixels to process
                Dim pixels As List(Of Point) = GetPixels(uI, lI, rc, mask, _blendParameters.MinAlpha)

                Dim bmData As BitmapData = Nothing
                Dim bmSrc As BitmapData = Nothing

                'note, I use fields for these here in this project
                'these store the system to solve
                'Dim results(2)() As Double 'keep the results for further processing
                'Dim diagonalElements(2)() As Integer '4 for inner pixels, <4 for pixels that touch the boundary
                'Dim otherElements(2)()() As Integer 'the indices for the -1 entries

                'the algorithm to compute the differences of the "b" vector
                Dim bVectorAlg As IBVectorComputingAlgorithm = _blendParameters.BVectorAlg 'BVectorComputingAlgorithm.GetAlgorithm(_blendParameters.Params.Algorithm)

                Dim c2 As List(Of ChainCode) = GetBoundary(uI, _blendParameters.MinAlpha)
                bVectorAlg.SetValuesSilent(_blendParameters.LowerImgWeight, _blendParameters.UpperImgWeight)

                _blendParameters.Pixels = pixels
                _blendParameters.RC = rc

                'Dim w As Double = 2 / (1 + Math.Sin(Math.PI / uI.Width))
                Dim w As Double = Me.ComputeWForSOR(_blendParameters.AutoSOR, _blendParameters.AutoSORMode, pixels.Count,
                                                    uI.Width, uI.Height, _blendParameters.MinPixelAmount)

                OnShowProgress(20)

                Try
                    If AvailMem.AvailMem.checkAvailRam(uI.Width * uI.Height * 4L + lI.Width * lI.Height * 4L) Then
                        bmData = uI.LockBits(New Rectangle(0, 0, uI.Width, uI.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                        bmSrc = lI.LockBits(New Rectangle(0, 0, lI.Width, lI.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                        Dim scanline As Integer = bmData.Stride
                        Dim nWidth As Integer = uI.Width
                        Dim nHeight As Integer = uI.Height

                        Dim scanlineL As Integer = bmSrc.Stride
                        Dim nWidthL As Integer = lI.Width
                        Dim nHeightL As Integer = lI.Height

                        Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                        Marshal.Copy(bmData.Scan0, p, 0, p.Length)

                        Dim pL((bmSrc.Stride * bmSrc.Height) - 1) As Byte
                        Marshal.Copy(bmSrc.Scan0, pL, 0, pL.Length)

                        'For plane As Integer = 0 To 2
                        'Parallel.For(0, 3, Sub(plane, loopState)
                        'matrix  elements

                        Dim plane As Integer = 0
                        Dim n(pixels.Count - 1) As Integer
                        Dim r(pixels.Count - 1)() As Integer

                        'b-vector
                        Dim b(pixels.Count - 1) As Double

                        'here we setup the system, put all boundary values and the known-function values (differences between pixels, "finite differences") to b.
                        'and also set up the sparse system by putting the factor for the central pixel to the n-array and the off diagonal elements 
                        '(the indices of the surrounding pixels that in the matrix have a value of -1) to the r-array.
                        GetSystem(p, pL, scanline, scanlineL, pixels, nWidth, nHeight, mask, n, r, b, rc, plane, bVectorAlg)

                        'jacobi method to solve
                        Dim result As Double() = GaussSeidelForPoissonEq_Div(b, n, r, _blendParameters.JacobiGSParams.MaxIterations, True, _blendParameters.JacobiGSParams.DesiredMaxLinearError,
                                                                    _blendParameters.PE, w, plane)
                        'make the results re-usable for further iterations
                        _blendParameters.Results(plane) = result
                        _blendParameters.DiagonalElements(plane) = n
                        _blendParameters.OtherElements(plane) = r

                        _blendParameters.BVector(plane) = b
                        'End Sub)
                        'Next

                        OnShowProgress(_blendParameters.PE.ImgWidthHeight)

                        'write the values back to the image
                        InsertPixelsForAllPlanes(pL, scanlineL, _blendParameters.Results, pixels, _blendParameters.RC)

                        Marshal.Copy(pL, 0, bmSrc.Scan0, pL.Length)
                        uI.UnlockBits(bmData)
                        lI.UnlockBits(bmSrc)
                        p = Nothing
                        pL = Nothing

                        FeatherResults(_blendParameters.LowerImg, _blendParameters.UpperImg, lI)
                    End If
                Catch exc As Exception
                    Try
                        uI.UnlockBits(bmData)
                    Catch

                    End Try
                    Try
                        lI.UnlockBits(bmSrc)
                    Catch

                    End Try
                Finally
                    _blendParameters.UpperImg.Dispose()
                    _blendParameters.UpperImg = Nothing
                End Try
            End Using
        End If
    End Sub

    Public Shared Sub Grayscale(bmp As Bitmap)
        Dim bmData As BitmapData = Nothing

        If Not AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L) Then
            Return
        End If

        Try
            bmData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.[WriteOnly], PixelFormat.Format32bppArgb)

            Dim w As Integer = bmp.Width
            Dim h As Integer = bmp.Height

            Dim p((bmData.Stride * bmData.Height) - 1) As Byte
            Marshal.Copy(bmData.Scan0, p, 0, p.Length)

            'For y As Integer = 0 To bmData.Height - 1
            Parallel.For(0, h, Sub(y)
                                   Dim pos As Integer = y * bmData.Stride

                                   For x As Integer = 0 To w - 1
                                       Dim value As Integer = CInt(CDbl(p(pos)) * 0.11 + CDbl(p(pos + 1)) * 0.59 + CDbl(p(pos + 2)) * 0.3)

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

    Private Sub FeatherResults(lowerImg As Bitmap, upperImg As Bitmap, lI As Bitmap)
        Dim bmData As BitmapData = Nothing
        Dim bmSrc As BitmapData = Nothing
        Dim bmData2 As BitmapData = Nothing
        Dim distanceArray As Integer(,) = Nothing

        Dim gamma As Double = _blendParameters.Gamma
        Dim maxPixelDist As Integer = _blendParameters.MaxPixelDist

        Using bC As New Bitmap(upperImg)
            RaiseEvent ShowInfo(Me, "Generating DistanceArray...")
            Grayscale(bC)
            SetRegion(bC)
            Dim fip As New fipbmp()
            distanceArray = fip.GetOutlineArrayGrayscale(bC, 0, 10000)
        End Using

        RaiseEvent ShowInfo(Me, "Writing Pixels...")
        If AvailMem.AvailMem.checkAvailRam(lowerImg.Width * lowerImg.Height * 4L + lI.Width * lI.Height * 4L) Then
            bmData = lowerImg.LockBits(New Rectangle(0, 0, lowerImg.Width, lowerImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
            bmSrc = lI.LockBits(New Rectangle(0, 0, lI.Width, lI.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
            bmData2 = upperImg.LockBits(New Rectangle(0, 0, upperImg.Width, upperImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

            Dim scanline As Integer = bmData.Stride
            Dim nWidth As Integer = lowerImg.Width
            Dim nHeight As Integer = lowerImg.Height

            Dim mixRelation As Double = _blendParameters.UpperImgMixRel
            Dim oneMinusMixRelation As Double = 1.0 - mixRelation

            Dim p((bmData.Stride * bmData.Height) - 1) As Byte
            Marshal.Copy(bmData.Scan0, p, 0, p.Length)

            Dim pL((bmSrc.Stride * bmSrc.Height) - 1) As Byte
            Marshal.Copy(bmSrc.Scan0, pL, 0, pL.Length)

            Dim p2((bmData2.Stride * bmData2.Height) - 1) As Byte
            Marshal.Copy(bmData2.Scan0, p2, 0, p2.Length)

            Parallel.For(0, nHeight, Sub(y)
                                         'For y As Integer = 0 To nHeight - 1
                                         Dim pos As Integer = y * scanline
                                         For x As Integer = 0 To nWidth - 1
                                             If p2(pos + 3) > 0 Then
                                                 Dim pixelDist As Integer = distanceArray(x, y)
                                                 Dim relDist As Double = Math.Pow(CDbl(pixelDist) / CDbl(maxPixelDist), gamma) ' 0.0 to 1.0
                                                 Dim oneMinusRelDist As Double = 1.0 - relDist

                                                 If relDist > 1.0 Then
                                                     p(pos) = CByte(Math.Max(Math.Min(pL(pos) * oneMinusMixRelation + p2(pos) * mixRelation, 255), 0))
                                                     p(pos + 1) = CByte(Math.Max(Math.Min(pL(pos + 1) * oneMinusMixRelation + p2(pos + 1) * mixRelation, 255), 0))
                                                     p(pos + 2) = CByte(Math.Max(Math.Min(pL(pos + 2) * oneMinusMixRelation + p2(pos + 2) * mixRelation, 255), 0))
                                                 Else
                                                     p(pos) = CByte(Math.Max(Math.Min((pL(pos) * oneMinusRelDist + p2(pos) * relDist) * mixRelation + pL(pos) * oneMinusMixRelation, 255), 0))
                                                     p(pos + 1) = CByte(Math.Max(Math.Min((pL(pos + 1) * oneMinusRelDist + p2(pos + 1) * relDist) * mixRelation + pL(pos + 1) * oneMinusMixRelation, 255), 0))
                                                     p(pos + 2) = CByte(Math.Max(Math.Min((pL(pos + 2) * oneMinusRelDist + p2(pos + 2) * relDist) * mixRelation + pL(pos + 2) * oneMinusMixRelation, 255), 0))
                                                 End If

                                             End If
                                             pos += 4
                                         Next
                                         'Next
                                     End Sub)

            Marshal.Copy(p, 0, bmData.Scan0, p.Length)

            lowerImg.UnlockBits(bmData)
            lI.UnlockBits(bmSrc)
            upperImg.UnlockBits(bmData2)
            p = Nothing
            pL = Nothing
        End If
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

    Public Sub ApplyG4(rc As Rectangle)
        'GMRES_r
        Dim bParams As BlendParameters = CloneBlendParameters(Me._blendParameters)

        RaiseEvent ShowInfo(Me, "Applying normal mode...")
        Apply(rc)

        Using uI As Bitmap = New Bitmap(bParams.UpperImg)
            RaiseEvent ShowInfo(Me, "Grayscale upper Image...")
            Grayscale(uI)

            'mask: Stores the coordinates and the state (inside, boundary or outside of the region).
            'Region is the arbitrary shape of the upper image (based on alpha values. Alpha = 0 is outside)
            'We need this mapping of the coordinates just *because* of the arbitrary shape, since the vector that contains
            'all pixels to process is not a rectangular mapping, and so, the length a "row" or "column" could differ 
            'from the length of other rows or columns.
            Dim mask(uI.Width * uI.Height - 1) As Integer

            'get the set of pixels to process
            Dim pixels As List(Of Point) = GetPixels(uI, bParams.LowerImg, rc, mask, _blendParameters.MinAlpha)

            Dim bmData As BitmapData = Nothing
            Dim bmSrc As BitmapData = Nothing

            'note, I use fields for these here in this project
            'these store the system to solve
            'Dim results(2)() As Double 'keep the results for further processing
            'Dim diagonalElements(2)() As Integer '4 for inner pixels, <4 for pixels that touch the boundary
            'Dim otherElements(2)()() As Integer 'the indices for the -1 entries

            'the algorithm to compute the differences of the "b" vector
            Dim bVectorAlg As IBVectorComputingAlgorithm = _blendParameters.BVectorAlg 'BVectorComputingAlgorithm.GetAlgorithm(_blendParameters.Params.Algorithm)

            Dim c2 As List(Of ChainCode) = GetBoundary(uI, _blendParameters.MinAlpha)
            bVectorAlg.SetValuesSilent(_blendParameters.LowerImgWeight, _blendParameters.UpperImgWeight)

            bParams.Pixels = pixels
            _blendParameters.RC = rc

            'Dim w As Double = 2 / (1 + Math.Sin(Math.PI / uI.Width))
            Dim w As Double = Me.ComputeWForSOR(_blendParameters.AutoSOR, _blendParameters.AutoSORMode, pixels.Count,
                                                uI.Width, uI.Height, _blendParameters.MinPixelAmount)

            Dim innerIterations As Integer = Me.BlendParameters.GMRESParams.MaxInnerIterationsGMRES
            Dim maxError As Double = Me.BlendParameters.GMRESParams.DesiredMaxLinearError
            Dim preRelax As Boolean = Me.BlendParameters.GMRESParams.PreRelax
            Dim restart As Integer = Me.BlendParameters.GMRESParams.MaxRestartAmountGMRES

            OnShowProgress(20)

            Try
                If AvailMem.AvailMem.checkAvailRam(uI.Width * uI.Height * 4L + bParams.LowerImg.Width * bParams.LowerImg.Height * 4L) Then
                    bmData = uI.LockBits(New Rectangle(0, 0, uI.Width, uI.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                    bmSrc = bParams.LowerImg.LockBits(New Rectangle(0, 0, bParams.LowerImg.Width, bParams.LowerImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                    Dim scanline As Integer = bmData.Stride
                    Dim nWidth As Integer = uI.Width
                    Dim nHeight As Integer = uI.Height

                    Dim scanlineL As Integer = bmSrc.Stride
                    Dim nWidthL As Integer = bParams.LowerImg.Width
                    Dim nHeightL As Integer = bParams.LowerImg.Height

                    Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                    Marshal.Copy(bmData.Scan0, p, 0, p.Length)

                    Dim pL((bmSrc.Stride * bmSrc.Height) - 1) As Byte
                    Marshal.Copy(bmSrc.Scan0, pL, 0, pL.Length)

                    'For plane As Integer = 0 To 2
                    Parallel.For(0, 3, Sub(plane, loopState)
                                           'matrix  elements
                                           Dim n(pixels.Count - 1) As Integer
                                           Dim r(pixels.Count - 1)() As Integer

                                           'b-vector
                                           Dim b(pixels.Count - 1) As Double

                                           'here we setup the system, put all boundary values and the known-function values (differences between pixels, "finite differences") to b.
                                           'and also set up the sparse system by putting the factor for the central pixel to the n-array and the off diagonal elements 
                                           '(the indices of the surrounding pixels that in the matrix have a value of -1) to the r-array.
                                           GetSystem(p, pL, scanline, scanlineL, pixels, nWidth, nHeight, mask, n, r, b, rc, plane, bVectorAlg)

                                           'jacobi method to solve
                                           Dim result As Double() = GaussSeidelForPoissonEq_Div(b, n, r, _blendParameters.JacobiGSParams.MaxIterations, True, _blendParameters.JacobiGSParams.DesiredMaxLinearError,
                                                                                       _blendParameters.PE, w, plane)
                                           'make the results re-usable for further iterations
                                           _blendParameters.Results(plane) = result
                                           _blendParameters.DiagonalElements(plane) = n
                                           _blendParameters.OtherElements(plane) = r

                                           _blendParameters.BVector(plane) = b
                                       End Sub)
                    'Next

                    OnShowProgress(_blendParameters.PE.ImgWidthHeight)

                    'write the values back to the image
                    InsertPixelsForAllPlanes(pL, scanlineL, bParams.Results, pixels, _blendParameters.RC)

                    Marshal.Copy(pL, 0, bmSrc.Scan0, pL.Length)

                    ''test ############################
                    'Marshal.Copy(pL, 0, bmData.Scan0, pL.Length)
                    ''end test ############################

                    uI.UnlockBits(bmData)
                    bParams.LowerImg.UnlockBits(bmSrc)
                    p = Nothing
                    pL = Nothing

                    InsertPixels(bParams.LowerImg, _blendParameters.LowerImg, bParams.UpperImg,
                                 _blendParameters.MaxPixelDist, _blendParameters.Gamma, _blendParameters.UpperImgMixRel)
                    _blendParameters.LowerImg.Dispose()
                    _blendParameters.LowerImg = New Bitmap(bParams.LowerImg)

                    ''test ############################
                    'SetMask(uI, _blendParameters.UpperImg, 200, 0.4F)

                    'Using gx As Graphics = Graphics.FromImage(_blendParameters.LowerImg)
                    '    gx.DrawImage(uI, 0, 0)
                    'End Using
                    ''end test ############################
                End If
            Catch exc As Exception
                Try
                    uI.UnlockBits(bmData)
                Catch

                End Try
                Try
                    bParams.LowerImg.UnlockBits(bmSrc)
                Catch

                End Try
            Finally
                bParams.Dispose()
            End Try
        End Using
    End Sub

    Private Sub InsertPixels(bmp As Bitmap, bmpF As Bitmap, upperImg As Bitmap, maxDist As Integer, gamma As Double, mixRelation As Double)
        If Not AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 15L) Then
            Return
        End If

        RaiseEvent ShowInfo(Me, "Generating DistanceArray...")
        Dim distanceArray As Integer(,) = Nothing

        Using bC As New Bitmap(upperImg)
            Grayscale(bC)
            SetRegion(bC)
            Dim fip As New fipbmp()
            distanceArray = fip.GetOutlineArrayGrayscale(bC, 0, 10000)
        End Using

        Dim bmData As BitmapData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
        Dim bmSrc As BitmapData = bmpF.LockBits(New Rectangle(0, 0, bmpF.Width, bmpF.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
        Dim bmMask As BitmapData = upperImg.LockBits(New Rectangle(0, 0, upperImg.Width, upperImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        Dim w As Integer = bmp.Width
        Dim h As Integer = bmp.Height
        Dim stride As Integer = bmData.Stride

        Dim p((bmData.Stride * bmData.Height) - 1) As Byte
        Marshal.Copy(bmData.Scan0, p, 0, p.Length)

        Dim pSrc((bmSrc.Stride * bmSrc.Height) - 1) As Byte
        Marshal.Copy(bmSrc.Scan0, pSrc, 0, pSrc.Length)

        Dim pMask((bmMask.Stride * bmMask.Height) - 1) As Byte
        Marshal.Copy(bmMask.Scan0, pMask, 0, pMask.Length)

        Dim oneMinusMixRelation As Double = 1.0 - mixRelation

        'For y As Integer = 0 To h - 1
        Parallel.[For](0, h, Sub(y)
                                 Dim pos As Integer = y * stride
                                 For x As Integer = 0 To w - 1
                                     If pMask(pos + 3) > 0 Then
                                         Dim pixelDist As Integer = distanceArray(x, y)

                                         Dim relDist As Double = Math.Pow(CDbl(pixelDist) / CDbl(maxDist), gamma)

                                         If relDist > 1.0 Then
                                             p(pos) = CByte(Math.Max(Math.Min(p(pos) * oneMinusMixRelation + pSrc(pos) * mixRelation, 255), 0))
                                             p(pos + 1) = CByte(Math.Max(Math.Min(p(pos + 1) * oneMinusMixRelation + pSrc(pos + 1) * mixRelation, 255), 0))
                                             p(pos + 2) = CByte(Math.Max(Math.Min(p(pos + 2) * oneMinusMixRelation + pSrc(pos + 2) * mixRelation, 255), 0))
                                             p(pos + 3) = pSrc(pos + 3)
                                         Else
                                             Dim blue As Double = (CDbl(p(pos)) * (1.0 - relDist) + CDbl(pSrc(pos)) * relDist) * mixRelation + p(pos) * oneMinusMixRelation
                                             Dim green As Double = (CDbl(p(pos + 1)) * (1.0 - relDist) + CDbl(pSrc(pos + 1)) * relDist) * mixRelation + p(pos + 1) * oneMinusMixRelation
                                             Dim red As Double = (CDbl(p(pos + 2)) * (1.0 - relDist) + CDbl(pSrc(pos + 2)) * relDist) * mixRelation + p(pos + 2) * oneMinusMixRelation

                                             p(pos) = CByte(Math.Max(Math.Min(blue, 255), 0))
                                             p(pos + 1) = CByte(Math.Max(Math.Min(green, 255), 0))
                                             p(pos + 2) = CByte(Math.Max(Math.Min(red, 255), 0))
                                             p(pos + 3) = pSrc(pos + 3)
                                         End If
                                     End If

                                     pos += 4
                                 Next
                             End Sub)
        'Next

        Marshal.Copy(p, 0, bmData.Scan0, p.Length)

        bmp.UnlockBits(bmData)
        bmpF.UnlockBits(bmSrc)
        upperImg.UnlockBits(bmMask)

        p = Nothing
        pSrc = Nothing

        RaiseEvent ShowInfo(Me, "Done.")
    End Sub

    Private Function CloneBlendParameters(blendParameters As BlendParameters) As BlendParameters
        Dim params As New BlendParameters()

        params.UpperImg = New Bitmap(blendParameters.UpperImg)
        params.LowerImg = New Bitmap(blendParameters.LowerImg)

        Dim results(2)() As Double
        params.Results = results
        Dim diagonalElements(2)() As Integer
        params.DiagonalElements = diagonalElements
        Dim otherElements(2)()() As Integer
        params.OtherElements = otherElements
        Dim bVector(2)() As Double
        params.BVector = bVector

        Return params
    End Function

    Public Sub ApplyD(rc As Rectangle) Implements IBlendAlgorithm.ApplyD
        'Jacobi/GaussSeidel

        'mask: Stores the coordinates and the state (inside, boundary or outside of the region).
        'Region is the arbitrary shape of the upper image (based on alpha values. Alpha = 0 is outside)
        'We need this mapping of the coordinates just *because* of the arbitrary shape, since the vector that contains
        'all pixels to process is not a rectangular mapping, and so, the length a "row" or "column" could differ 
        'from the length of other rows or columns.
        Dim mask(_blendParameters.UpperImg.Width * _blendParameters.UpperImg.Height - 1) As Integer

        'get the set of pixels to process
        Dim pixels As List(Of Point) = GetPixels(_blendParameters.UpperImg, _blendParameters.LowerImg, rc, mask, _blendParameters.MinAlpha)

        Dim bmDataX As BitmapData = Nothing
        Dim bmDataY As BitmapData = Nothing
        Dim bmSrc As BitmapData = Nothing
        Dim bmSrcX As BitmapData = Nothing
        Dim bmSrcY As BitmapData = Nothing

        'note, I use fields for these here in this project
        'these store the system to solve
        'Dim results(2)() As Double 'keep the results for further processing
        'Dim diagonalElements(2)() As Integer '4 for inner pixels, <4 for pixels that touch the boundary
        'Dim otherElements(2)()() As Integer 'the indices for the -1 entries

        'the algorithm to compute the differences of the "b" vector
        Dim bVectorAlg As IBVectorComputingAlgorithm = _blendParameters.BVectorAlg 'BVectorComputingAlgorithm.GetAlgorithm(_blendParameters.Params.Algorithm)

        Dim c2 As List(Of ChainCode) = GetBoundary(_blendParameters.UpperImg, _blendParameters.MinAlpha)
        bVectorAlg.SetValuesSilent(_blendParameters.LowerImgWeight, _blendParameters.UpperImgWeight)

        _blendParameters.Pixels = pixels
        _blendParameters.RC = rc

        'Dim w As Double = 2 / (1 + Math.Sin(Math.PI / _blendParameters.UpperImg.Width))
        Dim w As Double = Me.ComputeWForSOR(_blendParameters.AutoSOR, _blendParameters.AutoSORMode, pixels.Count,
                                            _blendParameters.UpperImg.Width, _blendParameters.UpperImg.Height, _blendParameters.MinPixelAmount)

        OnShowProgress(20)

        Try
            If AvailMem.AvailMem.checkAvailRam(_blendParameters.UpperImg.Width * _blendParameters.UpperImg.Height * 4L + _blendParameters.LowerImg.Width * _blendParameters.LowerImg.Height * 4L) Then
                bmDataX = _blendParameters.UpperImgDX.LockBits(New Rectangle(0, 0, _blendParameters.UpperImgDX.Width, _blendParameters.UpperImgDX.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataY = _blendParameters.UpperImgDY.LockBits(New Rectangle(0, 0, _blendParameters.UpperImgDY.Width, _blendParameters.UpperImgDY.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmSrc = _blendParameters.LowerImg.LockBits(New Rectangle(0, 0, _blendParameters.LowerImg.Width, _blendParameters.LowerImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmSrcX = _blendParameters.LowerImgDX.LockBits(New Rectangle(0, 0, _blendParameters.LowerImgDX.Width, _blendParameters.LowerImgDX.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmSrcY = _blendParameters.LowerImgDY.LockBits(New Rectangle(0, 0, _blendParameters.LowerImgDY.Width, _blendParameters.LowerImgDY.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim scanline As Integer = bmDataX.Stride
                Dim nWidth As Integer = _blendParameters.UpperImgDX.Width
                Dim nHeight As Integer = _blendParameters.UpperImgDX.Height

                Dim scanlineL As Integer = bmSrc.Stride
                Dim nWidthL As Integer = _blendParameters.LowerImg.Width
                Dim nHeightL As Integer = _blendParameters.LowerImg.Height

                Dim pX((bmDataX.Stride * bmDataX.Height) - 1) As Byte
                Marshal.Copy(bmDataX.Scan0, pX, 0, pX.Length)

                Dim pY((bmDataY.Stride * bmDataY.Height) - 1) As Byte
                Marshal.Copy(bmDataY.Scan0, pY, 0, pY.Length)

                Dim pL((bmSrc.Stride * bmSrc.Height) - 1) As Byte
                Marshal.Copy(bmSrc.Scan0, pL, 0, pL.Length)

                Dim pLX((bmSrcX.Stride * bmSrcX.Height) - 1) As Byte
                Marshal.Copy(bmSrcX.Scan0, pLX, 0, pLX.Length)

                Dim pLY((bmSrcY.Stride * bmSrcY.Height) - 1) As Byte
                Marshal.Copy(bmSrcY.Scan0, pLY, 0, pLY.Length)

                'For plane As Integer = 0 To 2
                Parallel.For(0, 3, Sub(plane, loopState)
                                       'matrix  elements
                                       Dim n(pixels.Count - 1) As Integer
                                       Dim r(pixels.Count - 1)() As Integer

                                       'b-vector
                                       Dim b(pixels.Count - 1) As Double

                                       'here we setup the system, put all boundary values and the known-function values (differences between pixels, "finite differences") to b.
                                       'and also set up the sparse system by putting the factor for the central pixel to the n-array and the off diagonal elements 
                                       '(the indices of the surrounding pixels that in the matrix have a value of -1) to the r-array.
                                       GetSystemD(pX, pY, pL, pLX, pLY, scanline, scanlineL, pixels, nWidth, nHeight, mask, n, r, b, rc, plane, bVectorAlg)

                                       'jacobi method to solve
                                       Dim result As Double() = GaussSeidelForPoissonEq_Div(b, n, r, _blendParameters.JacobiGSParams.MaxIterations, True, _blendParameters.JacobiGSParams.DesiredMaxLinearError,
                                                                                       _blendParameters.PE, w, plane)
                                       'make the results re-usable for further iterations
                                       _blendParameters.Results(plane) = result
                                       _blendParameters.DiagonalElements(plane) = n
                                       _blendParameters.OtherElements(plane) = r

                                       _blendParameters.BVector(plane) = b
                                   End Sub)
                'Next

                OnShowProgress(_blendParameters.PE.ImgWidthHeight)

                'write the values back to the image
                InsertPixelsForAllPlanes(pL, scanlineL, _blendParameters.Results, pixels, _blendParameters.RC)

                Marshal.Copy(pL, 0, bmSrc.Scan0, pL.Length)
                _blendParameters.UpperImgDX.UnlockBits(bmDataX)
                _blendParameters.UpperImgDY.UnlockBits(bmDataY)
                _blendParameters.LowerImg.UnlockBits(bmSrc)
                _blendParameters.LowerImgDX.UnlockBits(bmSrcX)
                _blendParameters.LowerImgDY.UnlockBits(bmSrcY)
                pX = Nothing
                pY = Nothing
                pL = Nothing
                pLX = Nothing
                pLY = Nothing
            End If
        Catch exc As Exception
            Try
                _blendParameters.UpperImgDX.UnlockBits(bmDataX)
            Catch

            End Try
            Try
                _blendParameters.UpperImgDY.UnlockBits(bmDataY)
            Catch

            End Try
            Try
                _blendParameters.LowerImg.UnlockBits(bmSrc)
            Catch

            End Try
            Try
                _blendParameters.LowerImgDX.UnlockBits(bmSrcX)
            Catch

            End Try
            Try
                _blendParameters.LowerImgDY.UnlockBits(bmSrcY)
            Catch

            End Try
        Finally
            _blendParameters.UpperImgDX.Dispose()
            _blendParameters.UpperImgDX = Nothing
            _blendParameters.UpperImgDY.Dispose()
            _blendParameters.UpperImgDY = Nothing
            _blendParameters.LowerImgDX.Dispose()
            _blendParameters.LowerImgDX = Nothing
            _blendParameters.LowerImgDY.Dispose()
            _blendParameters.LowerImgDY = Nothing
        End Try
    End Sub

    Public Sub ApplyA(rc As Rectangle) Implements IBlendAlgorithm.ApplyA
        'Jacobi/GaussSeidel

        'mask: Stores the coordinates and the state (inside, boundary or outside of the region).
        'Region is the arbitrary shape of the upper image (based on alpha values. Alpha = 0 is outside)
        'We need this mapping of the coordinates just *because* of the arbitrary shape, since the vector that contains
        'all pixels to process is not a rectangular mapping, and so, the length a "row" or "column" could differ 
        'from the length of other rows or columns.
        Dim mask(_blendParameters.UpperImg.Width * _blendParameters.UpperImg.Height - 1) As Integer

        'get the set of pixels to process
        Dim pixels As List(Of Point) = GetPixels(_blendParameters.UpperImg, _blendParameters.LowerImg, rc, mask, _blendParameters.MinAlpha)

        Dim bmSrc As BitmapData = Nothing

        'note, I use fields for these here in this project
        'these store the system to solve
        'Dim results(2)() As Double 'keep the results for further processing
        'Dim diagonalElements(2)() As Integer '4 for inner pixels, <4 for pixels that touch the boundary
        'Dim otherElements(2)()() As Integer 'the indices for the -1 entries

        'the algorithm to compute the differences of the "b" vector
        Dim bVectorAlg As IBVectorComputingAlgorithm = _blendParameters.BVectorAlg 'BVectorComputingAlgorithm.GetAlgorithm(_blendParameters.Params.Algorithm)

        Dim c2 As List(Of ChainCode) = GetBoundary(_blendParameters.UpperImg, _blendParameters.MinAlpha)
        bVectorAlg.SetValuesSilent(_blendParameters.LowerImgWeight, _blendParameters.UpperImgWeight)

        _blendParameters.Pixels = pixels
        _blendParameters.RC = rc

        'Dim w As Double = 2 / (1 + Math.Sin(Math.PI / _blendParameters.UpperImg.Width))
        Dim w As Double = Me.ComputeWForSOR(_blendParameters.AutoSOR, _blendParameters.AutoSORMode, pixels.Count,
                                            _blendParameters.UpperImg.Width, _blendParameters.UpperImg.Height, _blendParameters.MinPixelAmount)

        OnShowProgress(20)

        Try
            If AvailMem.AvailMem.checkAvailRam(_blendParameters.UpperImg.Width * _blendParameters.UpperImg.Height * 4L + _blendParameters.LowerImg.Width * _blendParameters.LowerImg.Height * 4L) Then
                bmSrc = _blendParameters.LowerImg.LockBits(New Rectangle(0, 0, _blendParameters.LowerImg.Width, _blendParameters.LowerImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim scanline As Integer = bmSrc.Stride
                Dim nWidth As Integer = _blendParameters.UpperImg.Width
                Dim nHeight As Integer = _blendParameters.UpperImg.Height

                Dim scanlineL As Integer = bmSrc.Stride
                Dim nWidthL As Integer = _blendParameters.LowerImg.Width
                Dim nHeightL As Integer = _blendParameters.LowerImg.Height

                Dim pX() As Short = _blendParameters.UpperADX
                Dim pY() As Short = _blendParameters.UpperADY

                Dim pL((bmSrc.Stride * bmSrc.Height) - 1) As Byte
                Marshal.Copy(bmSrc.Scan0, pL, 0, pL.Length)

                Dim pLX() As Short = _blendParameters.LowerADX
                Dim pLY() As Short = _blendParameters.LowerADY

                'For plane As Integer = 0 To 2
                Parallel.For(0, 3, Sub(plane, loopState)
                                       'matrix  elements
                                       Dim n(pixels.Count - 1) As Integer
                                       Dim r(pixels.Count - 1)() As Integer

                                       'b-vector
                                       Dim b(pixels.Count - 1) As Double

                                       'here we setup the system, put all boundary values and the known-function values (differences between pixels, "finite differences") to b.
                                       'and also set up the sparse system by putting the factor for the central pixel to the n-array and the off diagonal elements 
                                       '(the indices of the surrounding pixels that in the matrix have a value of -1) to the r-array.
                                       GetSystemA(pX, pY, pL, pLX, pLY, scanline, scanlineL, pixels, nWidth, nHeight, mask, n, r, b, rc, plane, bVectorAlg)

                                       'jacobi method to solve
                                       Dim result As Double() = GaussSeidelForPoissonEq_Div(b, n, r, _blendParameters.JacobiGSParams.MaxIterations, True, _blendParameters.JacobiGSParams.DesiredMaxLinearError,
                                                                                   _blendParameters.PE, w, plane)
                                       'make the results re-usable for further iterations
                                       _blendParameters.Results(plane) = result
                                       _blendParameters.DiagonalElements(plane) = n
                                       _blendParameters.OtherElements(plane) = r

                                       _blendParameters.BVector(plane) = b
                                   End Sub)
                'Next

                OnShowProgress(_blendParameters.PE.ImgWidthHeight)

                'write the values back to the image
                InsertPixelsForAllPlanes(pL, scanlineL, _blendParameters.Results, pixels, _blendParameters.RC)

                Marshal.Copy(pL, 0, bmSrc.Scan0, pL.Length)
                _blendParameters.LowerImg.UnlockBits(bmSrc)
                pL = Nothing
            End If
        Catch exc As Exception
            Try
                _blendParameters.LowerImg.UnlockBits(bmSrc)
            Catch

            End Try
        Finally
            _blendParameters.UpperADX = Nothing
            _blendParameters.UpperADY = Nothing
            _blendParameters.LowerADX = Nothing
            _blendParameters.LowerADY = Nothing
        End Try
    End Sub

    Private Sub OnShowProgress(v As Integer)
        _blendParameters.PE.CurrentProgress = v
        RaiseEvent ShowProgess(Me, _blendParameters.PE)
    End Sub

    'get the pixels for the selected region and map the coordinates of them to the indices in the vector
    Private Function GetPixels(upperImg As Bitmap, lowerImg As Bitmap, rc As Rectangle, ByRef mask As Integer(),
                               minAlpha As Integer) As List(Of Point)
        Dim bmData As BitmapData = Nothing
        Dim points As New List(Of Point)

        'Dim fff As New Form()
        'fff.BackgroundImage = upperImg
        'fff.BackgroundImageLayout = ImageLayout.Zoom
        'fff.ShowDialog()

        Try
            Dim nWidth As Integer = upperImg.Width
            Dim nHeight As Integer = upperImg.Height

            Dim l As List(Of ChainCode) = GetBoundary(upperImg, minAlpha)

            Me._blendParameters.CurrentBoundary = l

            For i As Integer = 0 To l.Count - 1
                Dim coords As List(Of Point) = l(i).Coord

                For j As Integer = 0 To coords.Count - 1
                    mask(coords(j).Y * nWidth + coords(j).X) = -1
                Next
            Next

            If AvailMem.AvailMem.checkAvailRam(upperImg.Width * upperImg.Height * 4L) Then
                bmData = upperImg.LockBits(New Rectangle(0, 0, upperImg.Width, upperImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim scanline As Integer = bmData.Stride
                Dim Scan0 As System.IntPtr = bmData.Scan0

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)

                Dim add As Point() = {New Point(-1, 0), New Point(1, 0), New Point(0, -1), New Point(0, 1)}

                For y As Integer = 0 To nHeight - 1
                    If Me.CancellationPending Then
                        Exit For
                    End If
                    Dim pos As Integer = y * scanline

                    For x As Integer = 0 To nWidth - 1
                        Dim alpha As Double = p(pos + 3)
                        If Not mask(y * nWidth + x) = -1 Then
                            mask(y * nWidth + x) = Int32.MinValue 'set outside
                        End If
                        If alpha > minAlpha Then
                            If Not mask(y * nWidth + x) = -1 Then
                                points.Add(New Point(x, y))
                                mask(y * nWidth + x) = points.Count - 1
                            End If
                        End If
                        pos += 4
                    Next
                Next

                upperImg.UnlockBits(bmData)
                p = Nothing
            End If

            'MessageBox.Show(mask(421 * nWidth + 364).ToString())
        Catch
            Try
                upperImg.UnlockBits(bmData)
            Catch

            End Try

        End Try

        Return points
    End Function

    'we need to pass a copy of upperImg to the method, since this may be called from a parallel loop
    Private Function GetBoundary(upperImg As Bitmap, minAlpha As Integer) As List(Of ChainCode)
        Dim l As List(Of ChainCode) = Nothing
        Dim bmpTmp As Bitmap = Nothing
        Try
            If AvailMem.AvailMem.checkAvailRam(upperImg.Width * upperImg.Height * 4L) Then
                bmpTmp = New Bitmap(upperImg)
            Else
                Throw New Exception("Not enough memory.")
            End If
            Dim nWidth As Integer = bmpTmp.Width
            Dim nHeight As Integer = bmpTmp.Height
            Dim cf As New ChainFinder()
            SyncLock Me._lockObject
                l = cf.GetOutline(bmpTmp, nWidth, nHeight, minAlpha, False, 0, False, 0, False)
            End SyncLock
        Catch exc As Exception
            OnBoundaryError(exc.Message)
        Finally
            If Not bmpTmp Is Nothing Then
                bmpTmp.Dispose()
                bmpTmp = Nothing
            End If
        End Try
        Return l
    End Function

    Private Sub OnBoundaryError(message As String)
        RaiseEvent BoundaryError(Me, message)
    End Sub

    'setup the matrix and the b vector
    Private Sub GetSystem(p As Byte(), pL As Byte(), scanline As Integer, scanlineL As Integer, pixels As List(Of Point), nWidth As Integer, nHeight As Integer,
                          mask As Integer(), n() As Integer, r As Integer()(), b() As Double, rc As Rectangle, plane As Integer, alg As IBVectorComputingAlgorithm)

        Dim add As Point() = {New Point(-1, 0), New Point(1, 0), New Point(0, -1), New Point(0, 1)}
        Dim ptC As New PointF(rc.Width / 2.0F, rc.Height / 2.0F)

        For i As Integer = 0 To pixels.Count - 1
            If Me.CancellationPending Then
                Exit For
            End If
            Dim x As Integer = pixels(i).X
            Dim y As Integer = pixels(i).Y

            'If x > 0 AndAlso x < nWidth - 1 AndAlso y > 0 AndAlso y < nHeight - 1 Then
            Dim xx As Integer = rc.X + x
            Dim yy As Integer = rc.Y + y
            Dim col As Integer = p(y * scanline + x * 4 + plane)
            Dim colL As Integer = pL((y + rc.Y) * scanlineL + (x + rc.X) * 4 + plane)
            Dim pos As Integer = y * scanline + x * 4
            Dim posL As Integer = yy * scanlineL + xx * 4
            Dim neighbors As Integer = 0

            Dim rr() As Integer = {-1, -1, -1, -1, mask(y * nWidth + x)}

            For j As Integer = 0 To add.Length - 1
                Dim xA As Integer = x + add(j).X
                Dim yA As Integer = y + add(j).Y

                If xA >= 0 AndAlso xA < nWidth AndAlso yA >= 0 AndAlso yA < nHeight Then
                    Dim indx As Integer = mask(yA * nWidth + xA)

                    If indx > Int32.MinValue Then
                        neighbors += 1
                        If indx = -1 Then 'boundary
                            'b(i) += pL(posL + add(j).Y * scanlineL + add(j).X * 4 + plane)
                            b(i) += pL((yA + rc.Y) * scanlineL + (xA + rc.X) * 4 + plane)
                            'rr(j) = -indx - 1
                        End If
                        If indx > -1 Then
                            'b(i) += (col - p(pos + add(j).Y * scanline + add(j).X * 4 + plane))
                            If alg Is Nothing Then
                                b(i) += (col - (p(yA * scanline + xA * 4 + plane)))
                            Else
                                b(i) += alg.GetValue(xA, yA, plane, col, colL, p, pL, scanline, scanlineL, rc, ptC, x, y)
                            End If
                            rr(j) = indx
                        End If
                    End If
                End If
            Next

            n(i) = neighbors
            r(i) = rr
            'End If
        Next
    End Sub

    'setup the matrix and the b vector
    Private Sub GetSystemD(pX As Byte(), pY As Byte(), pL As Byte(), pLX As Byte(), pLY As Byte(), scanline As Integer, scanlineL As Integer, pixels As List(Of Point), nWidth As Integer, nHeight As Integer,
                          mask As Integer(), n() As Integer, r As Integer()(), b() As Double, rc As Rectangle, plane As Integer, alg As IBVectorComputingAlgorithm)

        Dim add As Point() = {New Point(-1, 0), New Point(1, 0), New Point(0, -1), New Point(0, 1)}
        Dim addD As Point() = {New Point(1, 0), New Point(1, 0), New Point(0, 1), New Point(0, 1)}
        Dim ptC As New PointF(rc.Width / 2.0F, rc.Height / 2.0F)

        For i As Integer = 0 To pixels.Count - 1
            If Me.CancellationPending Then
                Exit For
            End If
            Dim x As Integer = pixels(i).X
            Dim y As Integer = pixels(i).Y

            'If x > 0 AndAlso x < nWidth - 1 AndAlso y > 0 AndAlso y < nHeight - 1 Then
            Dim xx As Integer = rc.X + x
            Dim yy As Integer = rc.Y + y
            Dim pos As Integer = y * scanline + x * 4
            Dim posL As Integer = yy * scanlineL + xx * 4
            Dim neighbors As Integer = 0

            Dim rr() As Integer = {-1, -1, -1, -1, mask(y * nWidth + x)}

            For j As Integer = 0 To addD.Length - 1
                Dim xA As Integer = x + add(j).X
                Dim yA As Integer = y + add(j).Y
                Dim xA1 As Integer = x + addD(j).X
                Dim yA1 As Integer = y + addD(j).Y
                Dim xA2 As Integer = x - addD(j).X
                Dim yA2 As Integer = y - addD(j).Y

                If xA >= 0 AndAlso xA < nWidth AndAlso yA >= 0 AndAlso yA < nHeight Then
                    Dim indx As Integer = mask(yA * nWidth + xA)

                    If indx > Int32.MinValue Then
                        neighbors += 1
                        If indx = -1 Then 'boundary
                            b(i) += pL((yA + rc.Y) * scanlineL + (xA + rc.X) * 4 + plane)
                        End If
                        If indx > -1 Then
                            If alg Is Nothing Then
                                'derivatives are computed normally (not reversed as the Laplacian), so we need to subtract from b
                                If j < 2 Then
                                    b(i) -= ((pX(yA1 * scanline + xA1 * 4 + plane) - 127) - (pX(yA2 * scanline + xA2 * 4 + plane) - 127))
                                Else
                                    b(i) -= ((pY(yA1 * scanline + xA1 * 4 + plane) - 127) - (pY(yA2 * scanline + xA2 * 4 + plane) - 127))
                                End If
                            Else
                                b(i) -= alg.GetValue(xA1, yA1, xA2, yA2, plane, pX, pY, pLX, pLY, scanline, scanlineL, rc, ptC, j, x, y)
                            End If
                            rr(j) = indx
                        End If
                    End If
                End If
            Next

            n(i) = neighbors
            r(i) = rr
            'End If
        Next

        'test debug, makes only sense when applyD run not parallel
        'If alg.GetType().Equals(GetType(GradMaxBVectorAlgorithm)) Then
        '    Dim a As GradMaxBVectorAlgorithm = CType(alg, GradMaxBVectorAlgorithm)
        '    Console.WriteLine("fieldChange sum: " & a.fieldChangeComp & " - fieldChange abs: " & a.fieldChangeAbs & " - OperationsCount: " & a.count)
        'End If
    End Sub

    Private Sub GetSystemA(pX As Short(), pY As Short(), pL As Byte(), pLX As Short(), pLY As Short(), scanline As Integer, scanlineL As Integer, pixels As List(Of Point), nWidth As Integer, nHeight As Integer,
                          mask As Integer(), n() As Integer, r As Integer()(), b() As Double, rc As Rectangle, plane As Integer, alg As IBVectorComputingAlgorithm)

        Dim add As Point() = {New Point(-1, 0), New Point(1, 0), New Point(0, -1), New Point(0, 1)}
        Dim addD As Point() = {New Point(1, 0), New Point(1, 0), New Point(0, 1), New Point(0, 1)}
        Dim ptC As New PointF(rc.Width / 2.0F, rc.Height / 2.0F)

        For i As Integer = 0 To pixels.Count - 1
            If Me.CancellationPending Then
                Exit For
            End If
            Dim x As Integer = pixels(i).X
            Dim y As Integer = pixels(i).Y

            'If x > 0 AndAlso x < nWidth - 1 AndAlso y > 0 AndAlso y < nHeight - 1 Then
            Dim xx As Integer = rc.X + x
            Dim yy As Integer = rc.Y + y
            Dim pos As Integer = y * scanline + x * 4
            Dim posL As Integer = yy * scanlineL + xx * 4
            Dim neighbors As Integer = 0

            Dim rr() As Integer = {-1, -1, -1, -1, mask(y * nWidth + x)}

            For j As Integer = 0 To addD.Length - 1
                Dim xA As Integer = x + add(j).X
                Dim yA As Integer = y + add(j).Y
                Dim xA1 As Integer = x + addD(j).X
                Dim yA1 As Integer = y + addD(j).Y
                Dim xA2 As Integer = x - addD(j).X
                Dim yA2 As Integer = y - addD(j).Y

                If xA >= 0 AndAlso xA < nWidth AndAlso yA >= 0 AndAlso yA < nHeight Then
                    Dim indx As Integer = mask(yA * nWidth + xA)

                    If indx > Int32.MinValue Then
                        neighbors += 1
                        If indx = -1 Then 'boundary
                            b(i) += pL((yA + rc.Y) * scanlineL + (xA + rc.X) * 4 + plane)
                        End If
                        If indx > -1 Then
                            If alg Is Nothing Then
                                'derivatives are computed normally (not reversed as the Laplacian), so we need to subtract from b
                                If j < 2 Then
                                    b(i) -= ((pX(yA1 * scanline + xA1 * 4 + plane)) - (pX(yA2 * scanline + xA2 * 4 + plane))) / 4.0
                                Else
                                    b(i) -= ((pY(yA1 * scanline + xA1 * 4 + plane)) - (pY(yA2 * scanline + xA2 * 4 + plane))) / 4.0
                                End If
                            Else
                                b(i) -= alg.GetValue(xA1, yA1, xA2, yA2, plane, pX, pY, pLX, pLY, scanline, scanlineL, rc, ptC, j, x, y)
                            End If
                            rr(j) = indx
                        End If
                    End If
                End If
            Next

            n(i) = neighbors
            r(i) = rr
            'End If
        Next

        'test debug, makes only sense when applyA run not parallel
        'If alg.GetType().Equals(GetType(GradMaxBVectorAlgorithm)) Then
        '    Dim a As GradMaxBVectorAlgorithm = CType(alg, GradMaxBVectorAlgorithm)
        '    Console.WriteLine("fieldChange sum: " & a.fieldChangeComp & " - fieldChange abs: " & a.fieldChangeAbs & " - OperationsCount: " & a.count)
        'End If
    End Sub

    Private Function GaussSeidelForPoissonEq_Div(b As Double(), n As Integer(), r As Integer()(), maxIterations As Integer, autoStop As Boolean,
                                            autoStopTolerance As Double, pe As ProgressEventArgs, w As Double, plane As Integer) As Double()
        Dim xVector As Double() = (Enumerable.Repeat(0.0, b.Length).ToArray())
        Dim xVectorOld As Double() = (Enumerable.Repeat(0.0, b.Length).ToArray())

        Dim l As Integer = b.Length
        'Dim w As Double = 3.0 / 2.0 'w_opt = 2 / ( 1 + sin(pi/(n+1)) )

        For p As Integer = 0 To maxIterations - 1
            If Me.CancellationPending Then
                Exit For
            End If
            For i As Integer = 0 To l - 1 'rows
                Dim sigma As Double = 0
                Dim center As Double = xVectorOld(r(i)(4))

                'red point 
                If (r(i)(0) > -1) Then
                    sigma += xVector(r(i)(0))
                End If
                'black point 
                If (r(i)(1) > -1) Then
                    sigma += xVectorOld(r(i)(1))
                End If
                'red point 
                If (r(i)(2) > -1) Then
                    sigma += xVector(r(i)(2))
                End If
                'black point 
                If (r(i)(3) > -1) Then
                    sigma += xVectorOld(r(i)(3))
                End If

                'red point
                xVector(i) = center + (w * (b(i) + sigma - (n(i) * center))) / n(i)
            Next

            Dim maxErr As Double = Double.MaxValue

            If autoStop Then
                If CheckResult(xVector, xVectorOld, autoStopTolerance, maxErr) Then
                    '#If DEBUG Then
                    '                        Console.WriteLine("AutoStop at iteration: " & p.ToString())
                    '#End If
                    Return xVector
                End If
            End If

            xVector.CopyTo(xVectorOld, 0)

            SyncLock Me._lockObject
                If Not pe Is Nothing Then
                    If pe.ImgWidthHeight < Int32.MaxValue Then
                        pe.CurrentProgress += 1
                    End If
                    Try
                        If CInt(pe.CurrentProgress) Mod pe.PrgInterval = 0 Then
                            OnShowProgress(pe.CurrentProgress)
                            RaiseEvent ShowInfo(Me, "### --- ### Info: " & p.ToString() & " Iterations of plane " & plane.ToString() & " finished. Max Error: " & maxErr.ToString())
                        End If
                    Catch
                    End Try
                End If
            End SyncLock
        Next

        Return xVector
    End Function

    Private Function CheckResult(solvedVector() As Double, solvedVectorA() As Double, autoStopTolerance As Double) As Boolean
        Dim b As Boolean = False
        Dim max As Double = 0

        'CheckResult2(solvedVector, solvedVectorA)

        For i As Integer = 0 To solvedVector.Length - 1
            Dim e As Double = Math.Abs(solvedVector(i) - solvedVectorA(i))

            If e > autoStopTolerance Then
                b = True
                Exit For
            End If

            If e > max Then
                max = e
            End If
        Next
        '#If DEBUG Then
        '        Console.WriteLine("max error at current iteration: " & max.ToString())
        '#End If
        Return Not b
    End Function

    Private Function CheckResult(solvedVector() As Double, solvedVectorA() As Double, autoStopTolerance As Double, ByRef err As Double) As Boolean
        Dim b As Boolean = False
        Dim max As Double = 0

        'CheckResult2(solvedVector, solvedVectorA)

        For i As Integer = 0 To solvedVector.Length - 1
            Dim e As Double = Math.Abs(solvedVector(i) - solvedVectorA(i))

            If e > autoStopTolerance Then
                max = e
                b = True
                Exit For
            End If

            If e > max Then
                max = e
            End If
        Next

        err = max

        '#If DEBUG Then
        '        Console.WriteLine("max error at current iteration: " & max.ToString())
        '#End If
        Return Not b
    End Function

    'write back the results to the lower image
    Private Sub InsertPixelsForAllPlanes(pL As Byte(), scanlineL As Integer, results As Double()(), pixels As List(Of Point), rc As Rectangle)
        For i As Integer = 0 To pixels.Count - 1
            Dim x As Integer = pixels(i).X + rc.X
            Dim y As Integer = pixels(i).Y + rc.Y

            pL(y * scanlineL + x * 4) = CByte(Math.Max(Math.Min(results(0)(i), 255), 0))
            pL(y * scanlineL + x * 4 + 1) = CByte(Math.Max(Math.Min(results(1)(i), 255), 0))
            pL(y * scanlineL + x * 4 + 2) = CByte(Math.Max(Math.Min(results(2)(i), 255), 0))
        Next
    End Sub

    'write back the results to the lower image
    Private Sub InsertPixelsForAllPlanesG(pL As Byte(), scanlineL As Integer, results As Double()(), pixels As List(Of Point), rc As Rectangle)
        For i As Integer = 0 To pixels.Count - 1
            Dim x As Integer = pixels(i).X + rc.X
            Dim y As Integer = pixels(i).Y + rc.Y

            pL(y * scanlineL + x * 4) = CByte(Math.Max(Math.Min(results(0)(i), 255), 0))
            pL(y * scanlineL + x * 4 + 1) = CByte(Math.Max(Math.Min(results(0)(i), 255), 0))
            pL(y * scanlineL + x * 4 + 2) = CByte(Math.Max(Math.Min(results(0)(i), 255), 0))
        Next
    End Sub

    'crop so that at least a one px border around bmpWork fits fully into lowerImg
    Private Function CropForProcessingUpperImg(upperImg1 As Bitmap, size As Size, ByRef rc As Rectangle) As Bitmap
        Dim rc2 As Rectangle = rc
        rc2.Intersect(New Rectangle(New Point(0, 0), size))

        Dim rCrop As Rectangle = New Rectangle(Math.Max(-rc.X, 0), Math.Max(-rc.Y, 0), rc2.Width, rc2.Height)

        If rCrop.X + rCrop.Width > upperImg1.Width Then rCrop.Width = upperImg1.Width - rCrop.X
        If rCrop.Y + rCrop.Height > upperImg1.Height Then rCrop.Height = upperImg1.Height - rCrop.Y

        Dim bmpOut As Bitmap = Nothing

        If AvailMem.AvailMem.checkAvailRam(rCrop.Width * rCrop.Height * 4L) Then
            bmpOut = upperImg1.Clone(rCrop, PixelFormat.Format32bppArgb)
        End If

        rc = New Rectangle(New Point(Math.Max(rc.X, 0), Math.Max(rc.Y, 0)), rCrop.Size)

        Return bmpOut
    End Function

    Private Function CropForProcessingLowerImg(lowerImg As Bitmap, ByRef rc As Rectangle) As Bitmap
        Dim bmpOut As Bitmap = Nothing

        If AvailMem.AvailMem.checkAvailRam(rc.Width * rc.Height * 4L) Then
            bmpOut = lowerImg.Clone(rc, PixelFormat.Format32bppArgb)
        End If

        Return bmpOut
    End Function

    'self explaining
    Private Function SetToIntRect(fLocation As RectangleF) As Rectangle
        Dim r As New Rectangle(CInt(Math.Floor(fLocation.X)),
                               CInt(Math.Floor(fLocation.Y)),
                               CInt(Math.Ceiling(fLocation.Width)),
                               CInt(Math.Ceiling(fLocation.Height)))

        Return r
    End Function

    Public Sub ApplyTile(upperImg As Bitmap, rc As Rectangle, rr As Rectangle, glBounds As List(Of ChainCode), xx As Integer, yy As Integer) Implements IBlendAlgorithm.ApplyTile
        'Jacobi/GaussSeidel

        'mask: Stores the coordinates and the state (inside, boundary or outside of the region).
        'Region is the arbitrary shape of the upper image (based on alpha values. Alpha = 0 is outside)
        'We need this mapping of the coordinates just *because* of the arbitrary shape, since the vector that contains
        'all pixels to process is not a rectangular mapping, and so, the length a "row" or "column" could differ 
        'from the length of other rows or columns.
        Dim mask(upperImg.Width * upperImg.Height - 1) As Integer

        'get the set of pixels to process
        Dim pixels As List(Of Point) = GetPixels(upperImg, _blendParameters.LowerImg, rr, mask, _blendParameters.MinAlpha, glBounds)

        Dim bmDataXY As BitmapData = Nothing
        Dim bmSrc As BitmapData = Nothing
        Dim bmRes As BitmapData = Nothing
        Dim bmSrcPPP As BitmapData = Nothing

        'the algorithm to compute the differences of the "b" vector
        Dim bVectorAlg As IBVectorComputingAlgorithm = _blendParameters.BVectorAlg 'BVectorComputingAlgorithm.GetAlgorithm(_blendData.Params.Algorithm)

        Dim rrc As New Rectangle(rr.X, rr.Y, rr.Width, rr.Height)

        Try
            If AvailMem.AvailMem.checkAvailRam(upperImg.Width * upperImg.Height * 8L) Then
                bmDataXY = upperImg.LockBits(New Rectangle(0, 0, upperImg.Width, upperImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmSrc = _blendParameters.LowerImg.LockBits(New Rectangle(0, 0, _blendParameters.LowerImg.Width, _blendParameters.LowerImg.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
                If _blendParameters.PppPic IsNot Nothing Then
                    bmSrcPPP = _blendParameters.PppPic.LockBits(New Rectangle(0, 0, _blendParameters.PppPic.Width, _blendParameters.PppPic.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
                End If

                Dim scanline As Integer = bmDataXY.Stride
                Dim nWidth As Integer = upperImg.Width
                Dim nHeight As Integer = upperImg.Height

                Dim scanlineL As Integer = bmSrc.Stride

                Dim pXY((bmDataXY.Stride * bmDataXY.Height) - 1) As Byte
                Marshal.Copy(bmDataXY.Scan0, pXY, 0, pXY.Length)

                Dim pL((bmSrc.Stride * bmSrc.Height) - 1) As Byte
                Marshal.Copy(bmSrc.Scan0, pL, 0, pL.Length)

                Dim pppL() As Byte = {}

                If bmSrcPPP IsNot Nothing Then
                    Dim pppL2((bmSrcPPP.Stride * bmSrcPPP.Height) - 1) As Byte
                    Marshal.Copy(bmSrcPPP.Scan0, pppL2, 0, pppL2.Length)
                    pppL = pppL2
                End If

                'Dim results(2)() As Double

                Dim w As Double = Me.ComputeWForSOR(_blendParameters.AutoSOR, _blendParameters.AutoSORMode, pixels.Count,
                                    _blendParameters.LowerImg.Width, _blendParameters.LowerImg.Height, _blendParameters.MinPixelAmount)

                OnShowProgress(20)

                Dim results(2)() As Double

                'For plane As Integer = 0 To 2
                Parallel.For(0, 3, Sub(plane, loopState)
                                       'matrix  elements
                                       Dim n(pixels.Count - 1) As Integer
                                       Dim r(pixels.Count - 1)() As Integer

                                       'b-vector
                                       Dim b(pixels.Count - 1) As Double

                                       If _blendParameters.UsePPP Then
                                           'here we setup the system, put all boundary values and the known-function values (differences between pixels, "finite differences") to b.
                                           'and also set up the sparse system by putting the factor for the central pixel to the n-array and the off diagonal elements 
                                           '(the indices of the surrounding pixels that in the matrix have a value of -1) to the r-array.
                                           GetSystemForTilePPP(pXY, pL, pppL, scanline, scanlineL, pixels, nWidth, nHeight, mask, n, r, b, rrc, plane, bVectorAlg)
                                       Else
                                           'here we setup the system, put all boundary values and the known-function values (differences between pixels, "finite differences") to b.
                                           'and also set up the sparse system by putting the factor for the central pixel to the n-array and the off diagonal elements 
                                           '(the indices of the surrounding pixels that in the matrix have a value of -1) to the r-array.
                                           GetSystemForTile(pXY, pL, scanline, scanlineL, pixels, nWidth, nHeight, mask, n, r, b, rrc, plane, bVectorAlg)
                                       End If

                                       'jacobi method to solve
                                       Dim result As Double() = GaussSeidelForPoissonEq_Div(b, n, r, _blendParameters.JacobiGSParams.MaxIterations, True, _blendParameters.JacobiGSParams.DesiredMaxLinearError,
                                                                                                  _blendParameters.PE, w, plane)

                                       results(plane) = result

                                       'make the results re-usable for further iterations
                                       _blendParameters.DiagonalElements(plane) = n
                                       _blendParameters.OtherElements(plane) = r

                                       _blendParameters.BVector(plane) = b
                                   End Sub)
                'Next

                OnShowProgress(_blendParameters.PE.ImgWidthHeight)

                upperImg.UnlockBits(bmDataXY)
                _blendParameters.LowerImg.UnlockBits(bmSrc)
                pXY = Nothing
                pL = Nothing

                If bmSrcPPP IsNot Nothing Then
                    _blendParameters.PppPic.UnlockBits(bmSrcPPP)
                    pppL = Nothing
                End If

                'write the values back to the image
                CropDrawAndSetLowerImage(_blendParameters.LowerImg, results, pixels, _blendParameters.BmpsUX(_blendParameters.CurX, _blendParameters.CurY), rr, rc, False)

                _blendParameters.ResultsTiles.Add(results)
            End If
        Catch exc As Exception
            Try
                upperImg.UnlockBits(bmDataXY)
            Catch

            End Try
            Try
                _blendParameters.LowerImg.UnlockBits(bmSrc)
            Catch

            End Try
            Try
                If bmSrcPPP IsNot Nothing Then
                    _blendParameters.PppPic.UnlockBits(bmSrcPPP)
                End If
            Catch

            End Try
        Finally

        End Try
    End Sub

    Private Sub CropDrawAndSetLowerImage(lowerImg As Bitmap, uVector As Double()(), pxls As List(Of Point),
                                              bitmapPosition As BitmapPositionPoisson, rr As Rectangle, rc As Rectangle, saveToCache As Boolean)
        Dim bmpWork As Bitmap = Nothing
        Dim bmData As BitmapData = Nothing

        Try
            'evtl cast auf int und rectangle statt rectangleF
            Dim rrc As New Rectangle(rr.X, rr.Y, rr.Width, rr.Height)
            bmpWork = lowerImg.Clone(rrc, lowerImg.PixelFormat)
            bmData = bmpWork.LockBits(New Rectangle(0, 0, bmpWork.Width, bmpWork.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

            Dim p(bmData.Stride * bmData.Height - 1) As Byte

            Dim scanline As Integer = bmData.Stride
            Marshal.Copy(bmData.Scan0, p, 0, p.Length)

            InsertPixelsForAllPlanesTile(p, scanline, uVector, pxls, rr)
            bmpWork.UnlockBits(bmData)

            Marshal.Copy(p, 0, bmData.Scan0, p.Length)
            p = Nothing

            bitmapPosition.SetBmp(bmpWork, saveToCache)
        Catch ex As Exception
            Try
                bmpWork.UnlockBits(bmData)
            Catch

            End Try

            If Not bmpWork Is Nothing Then
                bmpWork.Dispose()
                bmpWork = Nothing
            End If
        End Try
    End Sub

    Private Sub InsertPixelsForAllPlanesTile(pL As Byte(), scanlineL As Integer, results As Double()(),
                                             pixels As List(Of Point), rc As Rectangle)
        For i As Integer = 0 To pixels.Count - 1
            Dim x As Integer = pixels(i).X '+ rc.X
            Dim y As Integer = pixels(i).Y '+ rc.Y

            Try
                If x >= 0 AndAlso x * 4 < scanlineL AndAlso y >= 0 AndAlso y * scanlineL < pL.Length - scanlineL Then
                    pL(y * scanlineL + x * 4) = CByte(Math.Max(Math.Min(results(0)(i), 255), 0))
                    pL(y * scanlineL + x * 4 + 1) = CByte(Math.Max(Math.Min(results(1)(i), 255), 0))
                    pL(y * scanlineL + x * 4 + 2) = CByte(Math.Max(Math.Min(results(2)(i), 255), 0))
                End If
            Catch exc As Exception
                'Me.Invoke(New Action(Sub()
                '                         MessageBox.Show(exc.Message & vbCrLf & results(0)(i).ToString() & vbCrLf & results(1)(i).ToString() & vbCrLf & results(2)(i).ToString())
                '                     End Sub))
            End Try
        Next
    End Sub

    Private Function GetPixels(upperImg As Bitmap, lowerImg As Bitmap, rc As Rectangle, ByRef mask As Integer(),
                               minAlpha As Integer, globalBoundary As List(Of ChainCode)) As List(Of Point)
        Dim bmData As BitmapData = Nothing
        Dim points As New List(Of Point)

        Try
            Dim nWidth As Integer = upperImg.Width
            Dim nHeight As Integer = upperImg.Height

            Dim l As List(Of ChainCode) = GetBoundary(upperImg, minAlpha)

            Me._blendParameters.CurrentBoundary = l

            For i As Integer = 0 To l.Count - 1
                Dim coords As List(Of Point) = l(i).Coord

                For j As Integer = 0 To coords.Count - 1
                    Dim cropBoundary As Boolean = False

                    For z As Integer = 0 To globalBoundary.Count - 1
                        Dim jj As Integer = j
                        If Not globalBoundary(z).Coord.Exists(Function(a) a.X = coords(jj).X AndAlso a.Y = coords(jj).Y) Then
                            cropBoundary = True
                            Exit For
                        End If
                    Next

                    If cropBoundary Then
                        mask(coords(j).Y * nWidth + coords(j).X) = -2
                    Else
                        mask(coords(j).Y * nWidth + coords(j).X) = -1
                    End If
                Next
            Next

            If AvailMem.AvailMem.checkAvailRam(upperImg.Width * upperImg.Height * 4L) Then
                bmData = upperImg.LockBits(New Rectangle(0, 0, upperImg.Width, upperImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim scanline As Integer = bmData.Stride
                Dim Scan0 As System.IntPtr = bmData.Scan0

                Dim p((bmData.Stride * bmData.Height) - 1) As Byte
                Marshal.Copy(bmData.Scan0, p, 0, p.Length)

                Dim add As Point() = {New Point(-1, 0), New Point(1, 0), New Point(0, -1), New Point(0, 1)}

                For y As Integer = 0 To nHeight - 1
                    If Not BGW Is Nothing AndAlso BGW.WorkerSupportsCancellation AndAlso BGW.CancellationPending Then
                        Exit For
                    End If
                    Dim pos As Integer = y * scanline

                    For x As Integer = 0 To nWidth - 1
                        Dim alpha As Double = p(pos + 3)
                        If Not mask(y * nWidth + x) = -1 AndAlso Not mask(y * nWidth + x) = -2 Then
                            mask(y * nWidth + x) = Int32.MinValue 'set outside
                        End If
                        If alpha > minAlpha Then
                            If Not mask(y * nWidth + x) = -1 AndAlso Not mask(y * nWidth + x) = -2 Then
                                points.Add(New Point(x, y))
                                mask(y * nWidth + x) = points.Count - 1
                            End If
                        End If
                        pos += 4
                    Next
                Next

                upperImg.UnlockBits(bmData)
                p = Nothing
            End If
        Catch
            Try
                upperImg.UnlockBits(bmData)
            Catch

            End Try

        End Try

        Return points
    End Function

    Private Sub GetSystemForTile(p As Byte(), pL As Byte(), scanline As Integer, scanlineL As Integer, pixels As List(Of Point), nWidth As Integer, nHeight As Integer,
                          mask As Integer(), n() As Integer, r As Integer()(), b() As Double, rrc As Rectangle, plane As Integer, alg As IBVectorComputingAlgorithm)

        Dim add As Point() = {New Point(-1, 0), New Point(1, 0), New Point(0, -1), New Point(0, 1)}
        Dim ptC As New PointF(rrc.Width / 2.0F, rrc.Height / 2.0F)

        For i As Integer = 0 To pixels.Count - 1
            If Me.BGW Is Nothing AndAlso Me.BGW.WorkerSupportsCancellation AndAlso Me.BGW.CancellationPending Then
                Exit For
            End If
            Dim x As Integer = pixels(i).X
            Dim y As Integer = pixels(i).Y

            Dim col As Integer = p(y * scanline + x * 4 + plane)
            Dim pos As Integer = y * scanline + x * 4

            Dim neighbors As Integer = 0

            Dim rr() As Integer = {-1, -1, -1, -1, mask(y * nWidth + x)}

            For j As Integer = 0 To add.Length - 1
                Dim xA As Integer = x + add(j).X
                Dim yA As Integer = y + add(j).Y

                If xA >= 0 AndAlso xA < nWidth AndAlso yA >= 0 AndAlso yA < nHeight Then
                    Dim colL As Integer = pL((y + rrc.Y) * scanlineL + (x + rrc.X) * 4 + plane)

                    Dim indx As Integer = mask(yA * nWidth + xA)

                    If indx > Int32.MinValue Then
                        neighbors += 1
                        If indx = -1 Then 'boundary
                            b(i) += pL((yA + rrc.Y) * scanlineL + (xA + rrc.X) * 4 + plane)
                        End If
                        If indx = -2 Then 'crop boundary
                            If alg Is Nothing Then
                                b(i) += pL((yA + rrc.Y) * scanlineL + (xA + rrc.X) * 4 + plane)
                            Else
                                'just take the lowerImgValue here until I can compute a better value...
                                b(i) += pL((yA + rrc.Y) * scanlineL + (xA + rrc.X) * 4 + plane) 'alg.GetCropBoundaryValue(xA, yA, plane, col, colL, p, pL, scanline, scanlineL, rrc, ptC)
                            End If
                            'rr(j) = indx
                        End If
                        If indx > -1 Then
                            If alg Is Nothing Then
                                b(i) += (col - (p(yA * scanline + xA * 4 + plane)))
                            Else
                                b(i) += alg.GetValue(xA, yA, plane, col, colL, p, pL, scanline, scanlineL, rrc, ptC, x, y)
                            End If
                            rr(j) = indx
                        End If
                    End If
                    'End If 
                End If
            Next

            n(i) = neighbors
            r(i) = rr
        Next
    End Sub

    Private Sub GetSystemForTilePPP(p As Byte(), pL As Byte(), pppL As Byte(), scanline As Integer, scanlineL As Integer, pixels As List(Of Point), nWidth As Integer, nHeight As Integer,
                          mask As Integer(), n() As Integer, r As Integer()(), b() As Double, rrc As Rectangle, plane As Integer, alg As IBVectorComputingAlgorithm)

        Dim add As Point() = {New Point(-1, 0), New Point(1, 0), New Point(0, -1), New Point(0, 1)}
        Dim ptC As New PointF(rrc.Width / 2.0F, rrc.Height / 2.0F)

        For i As Integer = 0 To pixels.Count - 1
            If Me.BGW Is Nothing AndAlso Me.BGW.WorkerSupportsCancellation AndAlso Me.BGW.CancellationPending Then
                Exit For
            End If
            Dim x As Integer = pixels(i).X
            Dim y As Integer = pixels(i).Y

            Dim col As Integer = p(y * scanline + x * 4 + plane)
            Dim pos As Integer = y * scanline + x * 4

            Dim neighbors As Integer = 0

            Dim rr() As Integer = {-1, -1, -1, -1, mask(y * nWidth + x)}

            For j As Integer = 0 To add.Length - 1
                Dim xA As Integer = x + add(j).X
                Dim yA As Integer = y + add(j).Y

                If xA >= 0 AndAlso xA < nWidth AndAlso yA >= 0 AndAlso yA < nHeight Then
                    Dim colL As Integer = pL((y + rrc.Y) * scanlineL + (x + rrc.X) * 4 + plane)

                    Dim indx As Integer = mask(yA * nWidth + xA)

                    If indx > Int32.MinValue Then
                        neighbors += 1
                        If indx = -1 Then 'boundary
                            b(i) += pL((yA + rrc.Y) * scanlineL + (xA + rrc.X) * 4 + plane)
                        End If
                        If indx = -2 Then 'crop boundary
                            If alg Is Nothing Then
                                b(i) += pppL((yA + rrc.Y) * scanlineL + (xA + rrc.X) * 4 + plane)
                            Else
                                'just take the lowerImgValue here until I can compute a better value...
                                b(i) += pppL((yA + rrc.Y) * scanlineL + (xA + rrc.X) * 4 + plane) 'alg.GetCropBoundaryValue(xA, yA, plane, col, colL, p, pL, scanline, scanlineL, rrc, ptC)
                            End If
                            'rr(j) = indx
                        End If
                        If indx > -1 Then
                            If alg Is Nothing Then
                                b(i) += (col - (p(yA * scanline + xA * 4 + plane)))
                            Else
                                b(i) += alg.GetValue(xA, yA, plane, col, colL, p, pL, scanline, scanlineL, rrc, ptC, x, y)
                            End If
                            rr(j) = indx
                        End If
                    End If
                    'End If 
                End If
            Next

            n(i) = neighbors
            r(i) = rr
        Next
    End Sub

    Public Sub ApplyTileMinOvelap(upperImg As Bitmap, rc As Rectangle, rr As Rectangle, glBounds As List(Of ChainCode), xx As Integer, yy As Integer) Implements IBlendAlgorithm.ApplyTileMinOvelap
        'Jacobi/GaussSeidel

        'mask: Stores the coordinates and the state (inside, boundary or outside of the region).
        'Region is the arbitrary shape of the upper image (based on alpha values. Alpha = 0 is outside)
        'We need this mapping of the coordinates just *because* of the arbitrary shape, since the vector that contains
        'all pixels to process is not a rectangular mapping, and so, the length a "row" or "column" could differ 
        'from the length of other rows or columns.
        Dim mask(upperImg.Width * upperImg.Height - 1) As Integer

        'get the set of pixels to process
        Dim pixels As List(Of Point) = GetPixels(upperImg, _blendParameters.LowerImg, rr, mask, _blendParameters.MinAlpha, glBounds)

        Dim bmDataXY As BitmapData = Nothing
        Dim bmSrc As BitmapData = Nothing
        Dim bmRes As BitmapData = Nothing

        'the algorithm to compute the differences of the "b" vector
        Dim bVectorAlg As IBVectorComputingAlgorithm = _blendParameters.BVectorAlg 'BVectorComputingAlgorithm.GetAlgorithm(_blendData.Params.Algorithm)

        Dim rrc As New Rectangle(rr.X, rr.Y, rr.Width, rr.Height)

        Try
            If AvailMem.AvailMem.checkAvailRam(upperImg.Width * upperImg.Height * 8L) Then
                bmDataXY = upperImg.LockBits(New Rectangle(0, 0, upperImg.Width, upperImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmSrc = _blendParameters.LowerImg.LockBits(New Rectangle(0, 0, _blendParameters.LowerImg.Width, _blendParameters.LowerImg.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

                Dim scanline As Integer = bmDataXY.Stride
                Dim nWidth As Integer = upperImg.Width
                Dim nHeight As Integer = upperImg.Height

                Dim scanlineL As Integer = bmSrc.Stride

                Dim pXY((bmDataXY.Stride * bmDataXY.Height) - 1) As Byte
                Marshal.Copy(bmDataXY.Scan0, pXY, 0, pXY.Length)

                Dim pL((bmSrc.Stride * bmSrc.Height) - 1) As Byte
                Marshal.Copy(bmSrc.Scan0, pL, 0, pL.Length)

                'Dim results(2)() As Double

                Dim w As Double = Me.ComputeWForSOR(_blendParameters.AutoSOR, _blendParameters.AutoSORMode, pixels.Count,
                                            _blendParameters.LowerImg.Width, _blendParameters.LowerImg.Height, _blendParameters.MinPixelAmount)

                OnShowProgress(20)

                Dim results(2)() As Double

                'For plane As Integer = 0 To 2
                Parallel.For(0, 3, Sub(plane, loopState)
                                       'matrix  elements
                                       Dim n(pixels.Count - 1) As Integer
                                       Dim r(pixels.Count - 1)() As Integer

                                       'b-vector
                                       Dim b(pixels.Count - 1) As Double

                                       'here we setup the system, put all boundary values and the known-function values (differences between pixels, "finite differences") to b.
                                       'and also set up the sparse system by putting the factor for the central pixel to the n-array and the off diagonal elements 
                                       '(the indices of the surrounding pixels that in the matrix have a value of -1) to the r-array.
                                       GetSystemForTileMinOverlap(pXY, pL, scanline, scanlineL, pixels, nWidth, nHeight, mask, n, r, b, rrc, plane, bVectorAlg)

                                       'jacobi method to solve
                                       Dim result As Double() = GaussSeidelForPoissonEq_Div(b, n, r, _blendParameters.JacobiGSParams.MaxIterations, True, _blendParameters.JacobiGSParams.DesiredMaxLinearError,
                                                                                                          _blendParameters.PE, w, plane)

                                       results(plane) = result

                                       'make the results re-usable for further iterations
                                       _blendParameters.DiagonalElements(plane) = n
                                       _blendParameters.OtherElements(plane) = r

                                       _blendParameters.BVector(plane) = b
                                   End Sub)
                'Next

                OnShowProgress(_blendParameters.PE.ImgWidthHeight)

                upperImg.UnlockBits(bmDataXY)
                _blendParameters.LowerImg.UnlockBits(bmSrc)
                pXY = Nothing
                pL = Nothing

                'write the values back to the image
                CropDrawAndSetLowerImage(_blendParameters.LowerImg, results, pixels, _blendParameters.BmpsUX(_blendParameters.CurX, _blendParameters.CurY), rr, rc, False)

                _blendParameters.ResultsTiles.Add(results)
            End If
        Catch exc As Exception
            Try
                upperImg.UnlockBits(bmDataXY)
            Catch

            End Try
            Try
                _blendParameters.LowerImg.UnlockBits(bmSrc)
            Catch

            End Try
        Finally
            'bUX.Dispose()
            'bUX = Nothing
            'bUY.Dispose()
            'bUY = Nothing
        End Try
    End Sub

    Private Sub GetSystemForTileMinOverlap(p As Byte(), pL As Byte(), scanline As Integer, scanlineL As Integer, pixels As List(Of Point), nWidth As Integer, nHeight As Integer,
                      mask As Integer(), n() As Integer, r As Integer()(), b() As Double, rrc As Rectangle, plane As Integer, alg As IBVectorComputingAlgorithm)

        Dim add As Point() = {New Point(-1, 0), New Point(1, 0), New Point(0, -1), New Point(0, 1)}
        Dim ptC As New PointF(rrc.Width / 2.0F, rrc.Height / 2.0F)

        For i As Integer = 0 To pixels.Count - 1
            If Me.CancellationPending Then
                Exit For
            End If
            Dim x As Integer = pixels(i).X
            Dim y As Integer = pixels(i).Y
            Dim col As Integer = p(y * scanline + x * 4 + plane)

            Dim neighbors As Integer = 0

            Dim rr() As Integer = {-1, -1, -1, -1, mask(y * nWidth + x)}
            Dim added As Boolean = False

            For j As Integer = 0 To add.Length - 1
                Dim xA As Integer = x + add(j).X
                Dim yA As Integer = y + add(j).Y

                If xA >= 0 AndAlso xA < nWidth AndAlso yA >= 0 AndAlso yA < nHeight Then
                    Dim colL As Integer = pL((y + rrc.Y) * scanlineL + (x + rrc.X) * 4 + plane)

                    Dim indx As Integer = mask(yA * nWidth + xA)

                    If indx > Int32.MinValue Then
                        neighbors += 1
                        If indx = -1 Then 'boundary
                            b(i) += pL((yA + rrc.Y) * scanlineL + (xA + rrc.X) * 4 + plane)
                        End If
                        If indx = -2 Then 'crop boundary
                            b(i) += p(yA * scanline + xA * 4 + plane)
                        End If
                        If indx > -1 Then
                            If alg Is Nothing Then
                                b(i) += (col - (p(yA * scanline + xA * 4 + plane)))
                            Else
                                b(i) += alg.GetValue(xA, yA, plane, col, colL, p, pL, scanline, scanlineL, rrc, ptC, x, y)
                            End If
                            rr(j) = indx
                        End If
                    End If
                End If
            Next

            n(i) = neighbors
            r(i) = rr
        Next
    End Sub

    Public Sub ApplyInterpolationTile(bUXY As Bitmap, lowerImg As Bitmap, amountTilesPerRow As Integer, rc As Rectangle, doubleSizeInterpolationTile As Boolean) Implements IBlendAlgorithm.ApplyInterpolationTile
        If amountTilesPerRow > 1 Then
            If doubleSizeInterpolationTile Then
                Dim amountTilesHalf As Integer = amountTilesPerRow \ 2

                Dim wUT As Integer = bUXY.Width \ amountTilesHalf
                Dim hUT As Integer = bUXY.Height \ amountTilesHalf
                Dim wLT As Integer = lowerImg.Width \ amountTilesHalf
                Dim hLT As Integer = lowerImg.Height \ amountTilesHalf

                Dim tmpXY As New Bitmap(wUT, hUT)
                Dim tmpUI As New Bitmap(wLT, hLT)
                Using g As Graphics = Graphics.FromImage(tmpXY)
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.DrawImage(bUXY, 0, 0, wUT, hUT)
                End Using
                Using g As Graphics = Graphics.FromImage(tmpUI)
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.DrawImage(lowerImg, 0, 0, wLT, hLT)
                End Using

                Dim rcL As New Rectangle(rc.X \ amountTilesHalf, rc.Y \ amountTilesHalf, rc.Width \ amountTilesHalf, rc.Height \ amountTilesHalf)

                Dim mI As Integer = 5000
                Dim preRel As Boolean = _blendParameters.GMRESParams.PreRelax
                Dim maxRstrs As Integer = _blendParameters.GMRESParams.MaxRestartAmountGMRES
                Dim desError As Double = 0.0001
                Dim minAlpha As Integer = _blendParameters.MinAlpha
                Dim innerItrts As Integer = _blendParameters.GMRESParams.MaxInnerIterationsGMRES
                Dim autoSOR As Boolean = _blendParameters.AutoSOR
                Dim autoSORMode As AutoSORMode = _blendParameters.AutoSORMode
                Dim postProc As Boolean = False
                Dim postProcMultiplier As Double = 1.0

                Dim pb As New PoissonBlender(New Bitmap(tmpUI), New Bitmap(tmpXY))
                Dim pe As New PoissonBlend.ProgressEventArgs(mI * 3, 0)
                pe.PrgInterval = mI \ 20

                Dim bAlg As IBVectorComputingAlgorithm = pb.BlendParameters.GetBVectorAlg(_blendParameters.BVectorAlgType) 'addb etc
                Dim cAlg As IBlendAlgorithm = pb.BlendParameters.GetCalcAlg(0) 'gssor

                AddHandler cAlg.ShowProgess, AddressOf ShowProgress

                Dim rcSmall As New Rectangle(0, 0, tmpXY.Width, tmpXY.Height)

                pb.SetParameters(minAlpha, mI, desError, innerItrts, preRel, New Rectangle(0, 0, rcSmall.Width, rcSmall.Height),
                     maxRstrs, autoSOR, autoSORMode, 12, 1, 1, cAlg, bAlg, True, pe)

                Dim bmp As Bitmap = Nothing

                Try
                    If pb IsNot Nothing Then
                        cAlg.BGW = Me.BGW
                        bmp = pb.Apply()
                    End If
                Catch exc As OutOfMemoryException
                    MessageBox.Show(exc.Message)
                Catch exc As Exception
                    Console.WriteLine(exc.Message)
                End Try

                pb.BlendParameters.Dispose()

                Dim bOLdL As Bitmap = tmpUI
                tmpUI = bmp.Clone(rcL, bmp.PixelFormat)
                If Not bOLdL Is Nothing Then
                    bOLdL.Dispose()
                    bOLdL = Nothing
                End If

                RemoveHandler cAlg.ShowProgess, AddressOf ShowProgress

                'insert results to upper images
                For y As Integer = 0 To amountTilesPerRow - 1
                    For x As Integer = 0 To amountTilesPerRow - 1
                        Using g As Graphics = Graphics.FromImage(_blendParameters.BmpsUX(x, y).Bmp)
                            Dim st As Integer = If(x = 0, 1, 0)
                            Dim en As Integer = If(x = amountTilesPerRow - 1, 1, 0)
                            If y > 0 Then
                                g.DrawImage(tmpUI, New RectangleF(st, 0, _blendParameters.BmpsUX(x, y).Bmp.Width - st - en, 1),
                                New RectangleF((_blendParameters.BmpsUX(x, y).Loc.X + st) / amountTilesHalf,
                                               _blendParameters.BmpsUX(x, y).Loc.Y / amountTilesHalf,
                                               CSng((_blendParameters.BmpsUX(x, y).Bmp.Width - st - en) / amountTilesHalf),
                                               1.0F / amountTilesHalf), GraphicsUnit.Pixel)
                            End If
                            If y < amountTilesPerRow - 1 Then
                                g.DrawImage(tmpUI, New RectangleF(st, _blendParameters.BmpsUX(x, y).Bmp.Height - 1, _blendParameters.BmpsUX(x, y).Bmp.Width - st - en, 1),
                                           New RectangleF((_blendParameters.BmpsUX(x, y).Loc.X + st) / amountTilesHalf,
                                                          (_blendParameters.BmpsUX(x, y).Loc.Y + _blendParameters.BmpsUX(x, y).Bmp.Height - 1) / amountTilesHalf,
                                                          CSng((_blendParameters.BmpsUX(x, y).Bmp.Width - st - en) / amountTilesHalf),
                                                          1.0F / amountTilesHalf), GraphicsUnit.Pixel)
                            End If
                        End Using
                    Next
                    For x As Integer = 0 To amountTilesPerRow - 1
                        Using g As Graphics = Graphics.FromImage(_blendParameters.BmpsUX(x, y).Bmp)
                            Dim st As Integer = If(y = 0, 1, 0)
                            Dim en As Integer = If(y = amountTilesPerRow - 1, 1, 0)
                            If x > 0 Then
                                g.DrawImage(tmpUI, New RectangleF(0, st, 1, _blendParameters.BmpsUX(x, y).Bmp.Height - st - en),
                                New RectangleF(_blendParameters.BmpsUX(x, y).Loc.X / amountTilesHalf,
                                               (_blendParameters.BmpsUX(x, y).Loc.Y + st) / amountTilesHalf,
                                               1.0F / amountTilesHalf,
                                               CSng((_blendParameters.BmpsUX(x, y).Bmp.Height - st - en) / amountTilesHalf)), GraphicsUnit.Pixel)
                            End If
                            If x < amountTilesPerRow - 1 Then
                                g.DrawImage(tmpUI, New RectangleF(_blendParameters.BmpsUX(x, y).Bmp.Width - 1, st, 1, _blendParameters.BmpsUX(x, y).Bmp.Height - st - en),
                                           New RectangleF((_blendParameters.BmpsUX(x, y).Loc.X + _blendParameters.BmpsUX(x, y).Bmp.Width - 1) / amountTilesHalf,
                                                          (_blendParameters.BmpsUX(x, y).Loc.Y + st) / amountTilesHalf,
                                                          1.0F / amountTilesHalf,
                                                          CSng((_blendParameters.BmpsUX(x, y).Bmp.Height - st - en) / amountTilesHalf)), GraphicsUnit.Pixel)
                            End If
                        End Using
                    Next
                Next

                If tmpXY IsNot Nothing Then
                    tmpXY.Dispose()
                    tmpXY = Nothing
                End If
                If tmpUI IsNot Nothing Then
                    tmpUI.Dispose()
                    tmpUI = Nothing
                End If
                If bmp IsNot Nothing Then
                    bmp.Dispose()
                    bmp = Nothing
                End If
            Else
                Dim wUT As Integer = bUXY.Width \ amountTilesPerRow
                Dim hUT As Integer = bUXY.Height \ amountTilesPerRow
                Dim wLT As Integer = lowerImg.Width \ amountTilesPerRow
                Dim hLT As Integer = lowerImg.Height \ amountTilesPerRow

                Dim tmpXY As New Bitmap(wUT, hUT)
                Dim tmpUI As New Bitmap(wLT, hLT)
                Using g As Graphics = Graphics.FromImage(tmpXY)
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.DrawImage(bUXY, 0, 0, wUT, hUT)
                End Using
                Using g As Graphics = Graphics.FromImage(tmpUI)
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.DrawImage(lowerImg, 0, 0, wLT, hLT)
                End Using

                Dim rcL As New Rectangle(rc.X \ amountTilesPerRow, rc.Y \ amountTilesPerRow, rc.Width \ amountTilesPerRow, rc.Height \ amountTilesPerRow)

                Dim mI As Integer = 5000
                Dim preRel As Boolean = _blendParameters.GMRESParams.PreRelax
                Dim maxRstrs As Integer = _blendParameters.GMRESParams.MaxRestartAmountGMRES
                Dim desError As Double = 0.0001
                Dim minAlpha As Integer = _blendParameters.MinAlpha
                Dim innerItrts As Integer = _blendParameters.GMRESParams.MaxInnerIterationsGMRES
                Dim autoSOR As Boolean = _blendParameters.AutoSOR
                Dim autoSORMode As AutoSORMode = _blendParameters.AutoSORMode
                Dim postProc As Boolean = False
                Dim postProcMultiplier As Double = 1.0

                Dim pb As New PoissonBlender(New Bitmap(tmpUI), New Bitmap(tmpXY))
                Dim pe As New PoissonBlend.ProgressEventArgs(mI * 3, 0)
                pe.PrgInterval = mI \ 20

                Dim bAlg As IBVectorComputingAlgorithm = pb.BlendParameters.GetBVectorAlg(_blendParameters.BVectorAlgType) 'addb etc
                Dim cAlg As IBlendAlgorithm = pb.BlendParameters.GetCalcAlg(0) 'gssor

                AddHandler cAlg.ShowProgess, AddressOf ShowProgress

                Dim rcSmall As New Rectangle(0, 0, tmpXY.Width, tmpXY.Height)

                pb.SetParameters(minAlpha, mI, desError, innerItrts, preRel, New Rectangle(0, 0, rcSmall.Width, rcSmall.Height),
                     maxRstrs, autoSOR, autoSORMode, 12, 1, 1, cAlg, bAlg, True, pe)

                Dim bmp As Bitmap = Nothing

                Try
                    If pb IsNot Nothing Then
                        cAlg.BGW = Me.BGW
                        bmp = pb.Apply()
                    End If
                Catch exc As OutOfMemoryException
                    MessageBox.Show(exc.Message)
                Catch exc As Exception
                    Console.WriteLine(exc.Message)
                End Try

                pb.BlendParameters.Dispose()

                Dim bOLdL As Bitmap = tmpUI
                tmpUI = bmp.Clone(rcL, bmp.PixelFormat)
                If Not bOLdL Is Nothing Then
                    bOLdL.Dispose()
                    bOLdL = Nothing
                End If

                RemoveHandler cAlg.ShowProgess, AddressOf ShowProgress

                'insert results to upper images
                For y As Integer = 0 To amountTilesPerRow - 1
                    For x As Integer = 0 To amountTilesPerRow - 1
                        Using g As Graphics = Graphics.FromImage(_blendParameters.BmpsUX(x, y).Bmp)
                            Dim st As Integer = If(x = 0, 1, 0)
                            Dim en As Integer = If(x = amountTilesPerRow - 1, 1, 0)
                            If y > 0 Then
                                g.DrawImage(tmpUI, New RectangleF(st, 0, _blendParameters.BmpsUX(x, y).Bmp.Width - st - en, 1),
                                New RectangleF((_blendParameters.BmpsUX(x, y).Loc.X + st) / amountTilesPerRow,
                                               _blendParameters.BmpsUX(x, y).Loc.Y / amountTilesPerRow,
                                               CSng((_blendParameters.BmpsUX(x, y).Bmp.Width - st - en) / amountTilesPerRow),
                                               1.0F / amountTilesPerRow), GraphicsUnit.Pixel)
                            End If
                            If y < amountTilesPerRow - 1 Then
                                g.DrawImage(tmpUI, New RectangleF(st, _blendParameters.BmpsUX(x, y).Bmp.Height - 1, _blendParameters.BmpsUX(x, y).Bmp.Width - st - en, 1),
                                           New RectangleF((_blendParameters.BmpsUX(x, y).Loc.X + st) / amountTilesPerRow,
                                                          (_blendParameters.BmpsUX(x, y).Loc.Y + _blendParameters.BmpsUX(x, y).Bmp.Height - 1) / amountTilesPerRow,
                                                          CSng((_blendParameters.BmpsUX(x, y).Bmp.Width - st - en) / amountTilesPerRow),
                                                          1.0F / amountTilesPerRow), GraphicsUnit.Pixel)
                            End If
                        End Using
                    Next
                    For x As Integer = 0 To amountTilesPerRow - 1
                        Using g As Graphics = Graphics.FromImage(_blendParameters.BmpsUX(x, y).Bmp)
                            Dim st As Integer = If(y = 0, 1, 0)
                            Dim en As Integer = If(y = amountTilesPerRow - 1, 1, 0)
                            If x > 0 Then
                                g.DrawImage(tmpUI, New RectangleF(0, st, 1, _blendParameters.BmpsUX(x, y).Bmp.Height - st - en),
                                New RectangleF(_blendParameters.BmpsUX(x, y).Loc.X / amountTilesPerRow,
                                               (_blendParameters.BmpsUX(x, y).Loc.Y + st) / amountTilesPerRow,
                                               1.0F / amountTilesPerRow,
                                               CSng((_blendParameters.BmpsUX(x, y).Bmp.Height - st - en) / amountTilesPerRow)), GraphicsUnit.Pixel)
                            End If
                            If x < amountTilesPerRow - 1 Then
                                g.DrawImage(tmpUI, New RectangleF(_blendParameters.BmpsUX(x, y).Bmp.Width - 1, st, 1, _blendParameters.BmpsUX(x, y).Bmp.Height - st - en),
                                           New RectangleF((_blendParameters.BmpsUX(x, y).Loc.X + _blendParameters.BmpsUX(x, y).Bmp.Width - 1) / amountTilesPerRow,
                                                          (_blendParameters.BmpsUX(x, y).Loc.Y + st) / amountTilesPerRow,
                                                          1.0F / amountTilesPerRow,
                                                          CSng((_blendParameters.BmpsUX(x, y).Bmp.Height - st - en) / amountTilesPerRow)), GraphicsUnit.Pixel)
                            End If
                        End Using
                    Next
                Next

                If tmpXY IsNot Nothing Then
                    tmpXY.Dispose()
                    tmpXY = Nothing
                End If
                If tmpUI IsNot Nothing Then
                    tmpUI.Dispose()
                    tmpUI = Nothing
                End If
                If bmp IsNot Nothing Then
                    bmp.Dispose()
                    bmp = Nothing
                End If
            End If
        End If
    End Sub

    Public Sub ApplyInterpolationTileD(bUXY As Bitmap, lowerImg As Bitmap, amountTilesPerRow As Integer, rc As Rectangle, doubleSizeInterpolationTile As Boolean) Implements IBlendAlgorithm.ApplyInterpolationTileD
        If amountTilesPerRow > 1 Then
            If doubleSizeInterpolationTile Then
                Dim amountTilesHalf As Integer = amountTilesPerRow \ 2

                Dim wUT As Integer = bUXY.Width \ amountTilesHalf
                Dim hUT As Integer = bUXY.Height \ amountTilesHalf
                Dim wLT As Integer = lowerImg.Width \ amountTilesHalf
                Dim hLT As Integer = lowerImg.Height \ amountTilesHalf

                Dim tmpXY As New Bitmap(wUT, hUT)
                Dim tmpUI As New Bitmap(wLT, hLT)
                Using g As Graphics = Graphics.FromImage(tmpXY)
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.DrawImage(bUXY, 0, 0, wUT, hUT)
                End Using
                Using g As Graphics = Graphics.FromImage(tmpUI)
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.DrawImage(lowerImg, 0, 0, wLT, hLT)
                End Using

                Dim rcL As New Rectangle(rc.X \ amountTilesHalf, rc.Y \ amountTilesHalf, rc.Width \ amountTilesHalf, rc.Height \ amountTilesHalf)

                Dim mI As Integer = 5000
                Dim preRel As Boolean = _blendParameters.GMRESParams.PreRelax
                Dim maxRstrs As Integer = _blendParameters.GMRESParams.MaxRestartAmountGMRES
                Dim desError As Double = 0.0001
                Dim minAlpha As Integer = _blendParameters.MinAlpha
                Dim innerItrts As Integer = _blendParameters.GMRESParams.MaxInnerIterationsGMRES
                Dim autoSOR As Boolean = _blendParameters.AutoSOR
                Dim autoSORMode As AutoSORMode = _blendParameters.AutoSORMode
                Dim postProc As Boolean = False
                Dim postProcMultiplier As Double = 1.0

                Dim pb As New PoissonBlenderD(New Bitmap(tmpUI), New Bitmap(tmpXY))
                Dim pe As New PoissonBlend.ProgressEventArgs(mI * 3, 0)
                pe.PrgInterval = mI \ 20

                Dim bAlg As IBVectorComputingAlgorithm = pb.BlendParameters.GetBVectorAlg(_blendParameters.BVectorAlgType) 'addb etc
                Dim cAlg As IBlendAlgorithm = pb.BlendParameters.GetCalcAlg(0) 'gssor

                AddHandler cAlg.ShowProgess, AddressOf ShowProgress

                Dim rcSmall As New Rectangle(0, 0, tmpXY.Width, tmpXY.Height)

                pb.SetParameters(minAlpha, mI, desError, innerItrts, preRel, New Rectangle(0, 0, rcSmall.Width, rcSmall.Height),
                     maxRstrs, autoSOR, autoSORMode, 12, 1, 1, cAlg, bAlg, True, pe)

                Dim bmp As Bitmap = Nothing

                Try
                    If pb IsNot Nothing Then
                        cAlg.BGW = Me.BGW
                        bmp = pb.Apply()
                    End If
                Catch exc As OutOfMemoryException
                    MessageBox.Show(exc.Message)
                Catch exc As Exception
                    Console.WriteLine(exc.Message)
                End Try

                pb.BlendParameters.Dispose()

                Dim bOLdL As Bitmap = tmpUI
                tmpUI = bmp.Clone(rcL, bmp.PixelFormat)
                If Not bOLdL Is Nothing Then
                    bOLdL.Dispose()
                    bOLdL = Nothing
                End If

                RemoveHandler cAlg.ShowProgess, AddressOf ShowProgress

                'insert results to upper images
                For y As Integer = 0 To amountTilesPerRow - 1
                    For x As Integer = 0 To amountTilesPerRow - 1
                        Using g As Graphics = Graphics.FromImage(_blendParameters.BmpsUX(x, y).Bmp)
                            Dim st As Integer = If(x = 0, 1, 0)
                            Dim en As Integer = If(x = amountTilesPerRow - 1, 1, 0)
                            If y > 0 Then
                                g.DrawImage(tmpUI, New RectangleF(st, 0, _blendParameters.BmpsUX(x, y).Bmp.Width - st - en, 1),
                                New RectangleF((_blendParameters.BmpsUX(x, y).Loc.X + st) / amountTilesHalf,
                                               _blendParameters.BmpsUX(x, y).Loc.Y / amountTilesHalf,
                                               CSng((_blendParameters.BmpsUX(x, y).Bmp.Width - st - en) / amountTilesHalf),
                                               1.0F / amountTilesHalf), GraphicsUnit.Pixel)
                            End If
                            If y < amountTilesPerRow - 1 Then
                                g.DrawImage(tmpUI, New RectangleF(st, _blendParameters.BmpsUX(x, y).Bmp.Height - 1, _blendParameters.BmpsUX(x, y).Bmp.Width - st - en, 1),
                                           New RectangleF((_blendParameters.BmpsUX(x, y).Loc.X + st) / amountTilesHalf,
                                                          (_blendParameters.BmpsUX(x, y).Loc.Y + _blendParameters.BmpsUX(x, y).Bmp.Height - 1) / amountTilesHalf,
                                                          CSng((_blendParameters.BmpsUX(x, y).Bmp.Width - st - en) / amountTilesHalf),
                                                          1.0F / amountTilesHalf), GraphicsUnit.Pixel)
                            End If
                        End Using
                    Next
                    For x As Integer = 0 To amountTilesPerRow - 1
                        Using g As Graphics = Graphics.FromImage(_blendParameters.BmpsUX(x, y).Bmp)
                            Dim st As Integer = If(y = 0, 1, 0)
                            Dim en As Integer = If(y = amountTilesPerRow - 1, 1, 0)
                            If x > 0 Then
                                g.DrawImage(tmpUI, New RectangleF(0, st, 1, _blendParameters.BmpsUX(x, y).Bmp.Height - st - en),
                                New RectangleF(_blendParameters.BmpsUX(x, y).Loc.X / amountTilesHalf,
                                               (_blendParameters.BmpsUX(x, y).Loc.Y + st) / amountTilesHalf,
                                               1.0F / amountTilesHalf,
                                               CSng((_blendParameters.BmpsUX(x, y).Bmp.Height - st - en) / amountTilesHalf)), GraphicsUnit.Pixel)
                            End If
                            If x < amountTilesPerRow - 1 Then
                                g.DrawImage(tmpUI, New RectangleF(_blendParameters.BmpsUX(x, y).Bmp.Width - 1, st, 1, _blendParameters.BmpsUX(x, y).Bmp.Height - st - en),
                                           New RectangleF((_blendParameters.BmpsUX(x, y).Loc.X + _blendParameters.BmpsUX(x, y).Bmp.Width - 1) / amountTilesHalf,
                                                          (_blendParameters.BmpsUX(x, y).Loc.Y + st) / amountTilesHalf,
                                                          1.0F / amountTilesHalf,
                                                          CSng((_blendParameters.BmpsUX(x, y).Bmp.Height - st - en) / amountTilesHalf)), GraphicsUnit.Pixel)
                            End If
                        End Using
                    Next
                Next

                If tmpXY IsNot Nothing Then
                    tmpXY.Dispose()
                    tmpXY = Nothing
                End If
                If tmpUI IsNot Nothing Then
                    tmpUI.Dispose()
                    tmpUI = Nothing
                End If
                If bmp IsNot Nothing Then
                    bmp.Dispose()
                    bmp = Nothing
                End If
            Else
                Dim wUT As Integer = bUXY.Width \ amountTilesPerRow
                Dim hUT As Integer = bUXY.Height \ amountTilesPerRow
                Dim wLT As Integer = lowerImg.Width \ amountTilesPerRow
                Dim hLT As Integer = lowerImg.Height \ amountTilesPerRow

                Dim tmpXY As New Bitmap(wUT, hUT)
                Dim tmpUI As New Bitmap(wLT, hLT)
                Using g As Graphics = Graphics.FromImage(tmpXY)
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.DrawImage(bUXY, 0, 0, wUT, hUT)
                End Using
                Using g As Graphics = Graphics.FromImage(tmpUI)
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.DrawImage(lowerImg, 0, 0, wLT, hLT)
                End Using

                Dim rcL As New Rectangle(rc.X \ amountTilesPerRow, rc.Y \ amountTilesPerRow, rc.Width \ amountTilesPerRow, rc.Height \ amountTilesPerRow)

                Dim mI As Integer = 5000
                Dim preRel As Boolean = _blendParameters.GMRESParams.PreRelax
                Dim maxRstrs As Integer = _blendParameters.GMRESParams.MaxRestartAmountGMRES
                Dim desError As Double = 0.0001
                Dim minAlpha As Integer = _blendParameters.MinAlpha
                Dim innerItrts As Integer = _blendParameters.GMRESParams.MaxInnerIterationsGMRES
                Dim autoSOR As Boolean = _blendParameters.AutoSOR
                Dim autoSORMode As AutoSORMode = _blendParameters.AutoSORMode
                Dim postProc As Boolean = False
                Dim postProcMultiplier As Double = 1.0

                Dim pb As New PoissonBlenderD(New Bitmap(tmpUI), New Bitmap(tmpXY))
                Dim pe As New PoissonBlend.ProgressEventArgs(mI * 3, 0)
                pe.PrgInterval = mI \ 20

                Dim bAlg As IBVectorComputingAlgorithm = pb.BlendParameters.GetBVectorAlg(_blendParameters.BVectorAlgType) 'addb etc
                Dim cAlg As IBlendAlgorithm = pb.BlendParameters.GetCalcAlg(0) 'gssor

                AddHandler cAlg.ShowProgess, AddressOf ShowProgress

                Dim rcSmall As New Rectangle(0, 0, tmpXY.Width, tmpXY.Height)

                pb.SetParameters(minAlpha, mI, desError, innerItrts, preRel, New Rectangle(0, 0, rcSmall.Width, rcSmall.Height),
                     maxRstrs, autoSOR, autoSORMode, 12, 1, 1, cAlg, bAlg, True, pe)

                Dim bmp As Bitmap = Nothing

                Try
                    If pb IsNot Nothing Then
                        cAlg.BGW = Me.BGW
                        bmp = pb.Apply()
                    End If
                Catch exc As OutOfMemoryException
                    MessageBox.Show(exc.Message)
                Catch exc As Exception
                    Console.WriteLine(exc.Message)
                End Try

                pb.BlendParameters.Dispose()

                Dim bOLdL As Bitmap = tmpUI
                tmpUI = bmp.Clone(rcL, bmp.PixelFormat)
                If Not bOLdL Is Nothing Then
                    bOLdL.Dispose()
                    bOLdL = Nothing
                End If

                RemoveHandler cAlg.ShowProgess, AddressOf ShowProgress

                'insert results to upper images
                For y As Integer = 0 To amountTilesPerRow - 1
                    For x As Integer = 0 To amountTilesPerRow - 1
                        Using g As Graphics = Graphics.FromImage(_blendParameters.BmpsUX(x, y).Bmp)
                            Dim st As Integer = If(x = 0, 1, 0)
                            Dim en As Integer = If(x = amountTilesPerRow - 1, 1, 0)
                            If y > 0 Then
                                g.DrawImage(tmpUI, New RectangleF(st, 0, _blendParameters.BmpsUX(x, y).Bmp.Width - st - en, 1),
                                New RectangleF((_blendParameters.BmpsUX(x, y).Loc.X + st) / amountTilesPerRow,
                                               _blendParameters.BmpsUX(x, y).Loc.Y / amountTilesPerRow,
                                               CSng((_blendParameters.BmpsUX(x, y).Bmp.Width - st - en) / amountTilesPerRow),
                                               1.0F / amountTilesPerRow), GraphicsUnit.Pixel)
                            End If
                            If y < amountTilesPerRow - 1 Then
                                g.DrawImage(tmpUI, New RectangleF(st, _blendParameters.BmpsUX(x, y).Bmp.Height - 1, _blendParameters.BmpsUX(x, y).Bmp.Width - st - en, 1),
                                           New RectangleF((_blendParameters.BmpsUX(x, y).Loc.X + st) / amountTilesPerRow,
                                                          (_blendParameters.BmpsUX(x, y).Loc.Y + _blendParameters.BmpsUX(x, y).Bmp.Height - 1) / amountTilesPerRow,
                                                          CSng((_blendParameters.BmpsUX(x, y).Bmp.Width - st - en) / amountTilesPerRow),
                                                          1.0F / amountTilesPerRow), GraphicsUnit.Pixel)
                            End If
                        End Using
                    Next
                    For x As Integer = 0 To amountTilesPerRow - 1
                        Using g As Graphics = Graphics.FromImage(_blendParameters.BmpsUX(x, y).Bmp)
                            Dim st As Integer = If(y = 0, 1, 0)
                            Dim en As Integer = If(y = amountTilesPerRow - 1, 1, 0)
                            If x > 0 Then
                                g.DrawImage(tmpUI, New RectangleF(0, st, 1, _blendParameters.BmpsUX(x, y).Bmp.Height - st - en),
                                New RectangleF(_blendParameters.BmpsUX(x, y).Loc.X / amountTilesPerRow,
                                               (_blendParameters.BmpsUX(x, y).Loc.Y + st) / amountTilesPerRow,
                                               1.0F / amountTilesPerRow,
                                               CSng((_blendParameters.BmpsUX(x, y).Bmp.Height - st - en) / amountTilesPerRow)), GraphicsUnit.Pixel)
                            End If
                            If x < amountTilesPerRow - 1 Then
                                g.DrawImage(tmpUI, New RectangleF(_blendParameters.BmpsUX(x, y).Bmp.Width - 1, st, 1, _blendParameters.BmpsUX(x, y).Bmp.Height - st - en),
                                           New RectangleF((_blendParameters.BmpsUX(x, y).Loc.X + _blendParameters.BmpsUX(x, y).Bmp.Width - 1) / amountTilesPerRow,
                                                          (_blendParameters.BmpsUX(x, y).Loc.Y + st) / amountTilesPerRow,
                                                          1.0F / amountTilesPerRow,
                                                          CSng((_blendParameters.BmpsUX(x, y).Bmp.Height - st - en) / amountTilesPerRow)), GraphicsUnit.Pixel)
                            End If
                        End Using
                    Next
                Next

                If tmpXY IsNot Nothing Then
                    tmpXY.Dispose()
                    tmpXY = Nothing
                End If
                If tmpUI IsNot Nothing Then
                    tmpUI.Dispose()
                    tmpUI = Nothing
                End If
                If bmp IsNot Nothing Then
                    bmp.Dispose()
                    bmp = Nothing
                End If
            End If
        End If
    End Sub

    Public Sub ApplyTileMinOverlapD(bUX As Bitmap, bUY As Bitmap, rc As Rectangle, rr As Rectangle, glBounds As List(Of ChainCode), xx As Integer, yy As Integer) Implements IBlendAlgorithm.ApplyTileMinOverlapD
        'Jacobi/GaussSeidel

        'mask: Stores the coordinates and the state (inside, boundary or outside of the region).
        'Region is the arbitrary shape of the upper image (based on alpha values. Alpha = 0 is outside)
        'We need this mapping of the coordinates just *because* of the arbitrary shape, since the vector that contains
        'all pixels to process is not a rectangular mapping, and so, the length a "row" or "column" could differ 
        'from the length of other rows or columns.
        Dim mask(bUX.Width * bUX.Height - 1) As Integer

        'get the set of pixels to process
        Dim pixels As List(Of Point) = GetPixels(bUX, bUY, rr, mask, _blendParameters.MinAlpha, glBounds) 'correct this

        Dim bmDataX As BitmapData = Nothing
        Dim bmDataY As BitmapData = Nothing
        Dim bmSrc As BitmapData = Nothing
        Dim bmRes As BitmapData = Nothing
        Dim bmSrcX As BitmapData = Nothing
        Dim bmSrcY As BitmapData = Nothing

        'the algorithm to compute the differences of the "b" vector
        Dim bVectorAlg As IBVectorComputingAlgorithm = _blendParameters.BVectorAlg 'BVectorComputingAlgorithm.GetAlgorithm(_blendData.Params.Algorithm)

        Dim rrc As New Rectangle(0, 0, rr.Width, rr.Height)

        Try
            If AvailMem.AvailMem.checkAvailRam(bUX.Width * bUX.Height * 8L) Then
                bmDataX = bUX.LockBits(New Rectangle(0, 0, bUX.Width, bUX.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataY = bUY.LockBits(New Rectangle(0, 0, bUY.Width, bUY.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmSrc = _blendParameters.LowerImg.LockBits(New Rectangle(0, 0, _blendParameters.LowerImg.Width, _blendParameters.LowerImg.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
                bmSrcX = _blendParameters.BmpsLX(xx, yy).Bmp.LockBits(New Rectangle(0, 0, _blendParameters.BmpsLX(xx, yy).Bmp.Width, _blendParameters.BmpsLX(xx, yy).Bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmSrcY = _blendParameters.BmpsLY(xx, yy).Bmp.LockBits(New Rectangle(0, 0, _blendParameters.BmpsLY(xx, yy).Bmp.Width, _blendParameters.BmpsLY(xx, yy).Bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim scanline As Integer = bmDataX.Stride
                Dim nWidth As Integer = bUX.Width
                Dim nHeight As Integer = bUX.Height

                Dim scanlineL As Integer = bmSrcX.Stride

                Dim pX((bmDataX.Stride * bmDataX.Height) - 1) As Byte
                Marshal.Copy(bmDataX.Scan0, pX, 0, pX.Length)

                Dim pY((bmDataY.Stride * bmDataY.Height) - 1) As Byte
                Marshal.Copy(bmDataY.Scan0, pY, 0, pY.Length)

                Dim pL((bmSrc.Stride * bmSrc.Height) - 1) As Byte
                Marshal.Copy(bmSrc.Scan0, pL, 0, pL.Length)

                Dim pLX((bmSrcX.Stride * bmSrcX.Height) - 1) As Byte
                Marshal.Copy(bmSrcX.Scan0, pLX, 0, pLX.Length)

                Dim pLY((bmSrcY.Stride * bmSrcY.Height) - 1) As Byte
                Marshal.Copy(bmSrcY.Scan0, pLY, 0, pLY.Length)

                Dim w As Double = Me.ComputeWForSOR(_blendParameters.AutoSOR, _blendParameters.AutoSORMode, pixels.Count,
                                            _blendParameters.LowerImg.Width, _blendParameters.LowerImg.Height, _blendParameters.MinPixelAmount)

                OnShowProgress(20)

                Dim results(2)() As Double

                'For plane As Integer = 0 To 2
                Parallel.For(0, 3, Sub(plane, loopState)
                                       'matrix  elements
                                       Dim n(pixels.Count - 1) As Integer
                                       Dim r(pixels.Count - 1)() As Integer

                                       'b-vector
                                       Dim b(pixels.Count - 1) As Double

                                       'here we setup the system, put all boundary values and the known-function values (differences between pixels, "finite differences") to b.
                                       'and also set up the sparse system by putting the factor for the central pixel to the n-array and the off diagonal elements 
                                       '(the indices of the surrounding pixels that in the matrix have a value of -1) to the r-array.
                                       GetSystemForTileMinOverlapD(pX, pY, pL, pLX, pLY, scanline, scanlineL, pixels, nWidth, nHeight, mask, n, r, b, rrc, plane, bVectorAlg)

                                       'jacobi method to solve
                                       Dim result As Double() = GaussSeidelForPoissonEq_Div(b, n, r, _blendParameters.JacobiGSParams.MaxIterations, True, _blendParameters.JacobiGSParams.DesiredMaxLinearError,
                                                                                   _blendParameters.PE, w, plane)

                                       results(plane) = result

                                       'make the results re-usable for further iterations
                                       _blendParameters.DiagonalElements(plane) = n
                                       _blendParameters.OtherElements(plane) = r

                                       _blendParameters.BVector(plane) = b
                                   End Sub)
                'Next

                OnShowProgress(_blendParameters.PE.ImgWidthHeight)

                bUX.UnlockBits(bmDataX)
                bUY.UnlockBits(bmDataY)
                _blendParameters.BmpsLX(xx, yy).Bmp.UnlockBits(bmSrcX)
                _blendParameters.BmpsLY(xx, yy).Bmp.UnlockBits(bmSrcY)
                _blendParameters.LowerImg.UnlockBits(bmSrc)
                pX = Nothing
                pY = Nothing
                pL = Nothing
                pLX = Nothing
                pLY = Nothing

                'write the values back to the image
                CropDrawAndSetLowerImage(_blendParameters.LowerImg, results, pixels, _blendParameters.BmpsUX(_blendParameters.CurX, _blendParameters.CurY), rr, rc, False)

                _blendParameters.ResultsTiles.Add(results)
            End If
        Catch exc As Exception
            Try
                bUX.UnlockBits(bmDataX)
            Catch

            End Try
            Try
                bUY.UnlockBits(bmDataY)
            Catch

            End Try
            Try
                _blendParameters.LowerImg.UnlockBits(bmSrc)
            Catch

            End Try
            Try
                _blendParameters.BmpsLX(xx, yy).Bmp.UnlockBits(bmSrcX)
            Catch

            End Try
            Try
                _blendParameters.BmpsLY(xx, yy).Bmp.UnlockBits(bmSrcY)
            Catch

            End Try
        Finally

        End Try
    End Sub

    Private Sub GetSystemForTileMinOverlapD(pX As Byte(), pY As Byte(), pL As Byte(), pLX As Byte(), pLY As Byte(), scanline As Integer, scanlineL As Integer, pixels As List(Of Point), nWidth As Integer, nHeight As Integer,
                          mask As Integer(), n() As Integer, r As Integer()(), b() As Double, rrc As Rectangle, plane As Integer, alg As IBVectorComputingAlgorithm)

        Dim add As Point() = {New Point(-1, 0), New Point(1, 0), New Point(0, -1), New Point(0, 1)}
        Dim addD As Point() = {New Point(1, 0), New Point(1, 0), New Point(0, 1), New Point(0, 1)}
        Dim ptC As New PointF(rrc.Width / 2.0F, rrc.Height / 2.0F)

        For i As Integer = 0 To pixels.Count - 1
            If Me.CancellationPending Then
                Exit For
            End If
            Dim x As Integer = pixels(i).X
            Dim y As Integer = pixels(i).Y

            'If x > 0 AndAlso x < nWidth - 1 AndAlso y > 0 AndAlso y < nHeight - 1 Then
            Dim xx As Integer = rrc.X + x
            Dim yy As Integer = rrc.Y + y
            Dim pos As Integer = y * scanline + x * 4
            Dim posL As Integer = yy * scanlineL + xx * 4
            Dim neighbors As Integer = 0

            Dim rr() As Integer = {-1, -1, -1, -1, mask(y * nWidth + x)}

            For j As Integer = 0 To addD.Length - 1
                Dim xA As Integer = x + add(j).X
                Dim yA As Integer = y + add(j).Y
                Dim xA1 As Integer = x + addD(j).X
                Dim yA1 As Integer = y + addD(j).Y
                Dim xA2 As Integer = x - addD(j).X
                Dim yA2 As Integer = y - addD(j).Y

                If xA >= 0 AndAlso xA < nWidth AndAlso yA >= 0 AndAlso yA < nHeight Then
                    Dim indx As Integer = mask(yA * nWidth + xA)

                    If indx > Int32.MinValue Then
                        neighbors += 1
                        If indx = -1 Then 'boundary
                            b(i) += pL((yA + rrc.Y) * scanlineL + (xA + rrc.X) * 4 + plane)
                        End If
                        If indx = -2 Then 'crop boundary
                            If alg Is Nothing Then
                                b(i) += pL((yA + rrc.Y) * scanlineL + (xA + rrc.X) * 4 + plane)
                            Else
                                'just take the lowerImgValue here until I can compute a better value...
                                b(i) += pL((yA + rrc.Y) * scanlineL + (xA + rrc.X) * 4 + plane) 'alg.GetCropBoundaryValue(xA, yA, plane, col, colL, p, pL, scanline, scanlineL, rrc, ptC)
                            End If
                            'rr(j) = indx
                        End If
                        If indx > -1 Then
                            If alg Is Nothing Then
                                'derivatives are computed normally (not reversed as the Laplacian), so we need to subtract from b
                                If j < 2 Then
                                    b(i) -= ((pX(yA1 * scanline + xA1 * 4 + plane) - 127) - (pX(yA2 * scanline + xA2 * 4 + plane) - 127))
                                Else
                                    b(i) -= ((pY(yA1 * scanline + xA1 * 4 + plane) - 127) - (pY(yA2 * scanline + xA2 * 4 + plane) - 127))
                                End If
                            Else
                                b(i) -= alg.GetValue(xA1, yA1, xA2, yA2, plane, pX, pY, pLX, pLY, scanline, scanlineL, rrc, ptC, j, x, y)
                            End If
                            rr(j) = indx
                        End If
                    End If
                End If
            Next

            n(i) = neighbors
            r(i) = rr
            'End If
        Next

        'test debug, makes only sense when applyD runs not parallel
        'If alg.GetType().Equals(GetType(GradMaxBVectorAlgorithm)) Then
        '    Dim a As GradMaxBVectorAlgorithm = CType(alg, GradMaxBVectorAlgorithm)
        '    Console.WriteLine("fieldChange sum: " & a.fieldChangeComp & " - fieldChange abs: " & a.fieldChangeAbs & " - OperationsCount: " & a.count)
        'End If
    End Sub


    Private Sub ShowProgress(sender As Object, e As ProgressEventArgs)
        OnShowProgress(e.CurrentProgress)
    End Sub

    Public Sub ApplyTileD(bUX As Bitmap, bUY As Bitmap, rc As Rectangle, rr As Rectangle, glBounds As List(Of ChainCode), xx As Integer, yy As Integer) Implements IBlendAlgorithm.ApplyTileD
        'Jacobi/GaussSeidel

        'mask: Stores the coordinates and the state (inside, boundary or outside of the region).
        'Region is the arbitrary shape of the upper image (based on alpha values. Alpha = 0 is outside)
        'We need this mapping of the coordinates just *because* of the arbitrary shape, since the vector that contains
        'all pixels to process is not a rectangular mapping, and so, the length a "row" or "column" could differ 
        'from the length of other rows or columns.
        Dim mask(bUX.Width * bUX.Height - 1) As Integer

        'get the set of pixels to process
        Dim pixels As List(Of Point) = GetPixels(bUX, _blendParameters.LowerImg, rr, mask, _blendParameters.MinAlpha, glBounds)

        Dim bmDataX As BitmapData = Nothing
        Dim bmDataY As BitmapData = Nothing
        Dim bmSrc As BitmapData = Nothing
        Dim bmRes As BitmapData = Nothing
        Dim bmSrcX As BitmapData = Nothing
        Dim bmSrcY As BitmapData = Nothing

        'the algorithm to compute the differences of the "b" vector
        Dim bVectorAlg As IBVectorComputingAlgorithm = _blendParameters.BVectorAlg 'BVectorComputingAlgorithm.GetAlgorithm(_blendData.Params.Algorithm)

        Dim rrc As New Rectangle(rr.X, rr.Y, rr.Width, rr.Height)

        Try
            If AvailMem.AvailMem.checkAvailRam(bUX.Width * bUX.Height * 8L) Then
                bmDataX = bUX.LockBits(New Rectangle(0, 0, bUX.Width, bUX.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataY = bUY.LockBits(New Rectangle(0, 0, bUY.Width, bUY.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmSrc = _blendParameters.LowerImg.LockBits(New Rectangle(0, 0, _blendParameters.LowerImg.Width, _blendParameters.LowerImg.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
                bmSrcX = _blendParameters.LowerImgDX.LockBits(New Rectangle(0, 0, _blendParameters.LowerImgDX.Width, _blendParameters.LowerImgDX.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmSrcY = _blendParameters.LowerImgDY.LockBits(New Rectangle(0, 0, _blendParameters.LowerImgDY.Width, _blendParameters.LowerImgDY.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim scanline As Integer = bmDataX.Stride
                Dim nWidth As Integer = bUX.Width
                Dim nHeight As Integer = bUX.Height

                Dim scanlineL As Integer = bmSrc.Stride

                Dim pX((bmDataX.Stride * bmDataX.Height) - 1) As Byte
                Marshal.Copy(bmDataX.Scan0, pX, 0, pX.Length)

                Dim pY((bmDataY.Stride * bmDataY.Height) - 1) As Byte
                Marshal.Copy(bmDataY.Scan0, pY, 0, pY.Length)

                Dim pL((bmSrc.Stride * bmSrc.Height) - 1) As Byte
                Marshal.Copy(bmSrc.Scan0, pL, 0, pL.Length)

                Dim pLX((bmSrcX.Stride * bmSrcX.Height) - 1) As Byte
                Marshal.Copy(bmSrcX.Scan0, pLX, 0, pLX.Length)

                Dim pLY((bmSrcY.Stride * bmSrcY.Height) - 1) As Byte
                Marshal.Copy(bmSrcY.Scan0, pLY, 0, pLY.Length)

                Dim w As Double = Me.ComputeWForSOR(_blendParameters.AutoSOR, _blendParameters.AutoSORMode, pixels.Count,
                                            _blendParameters.LowerImg.Width, _blendParameters.LowerImg.Height, _blendParameters.MinPixelAmount)

                OnShowProgress(20)

                Dim results(2)() As Double

                'For plane As Integer = 0 To 2
                Parallel.For(0, 3, Sub(plane, loopState)
                                       'matrix  elements
                                       Dim n(pixels.Count - 1) As Integer
                                       Dim r(pixels.Count - 1)() As Integer

                                       'b-vector
                                       Dim b(pixels.Count - 1) As Double

                                       'here we setup the system, put all boundary values and the known-function values (differences between pixels, "finite differences") to b.
                                       'and also set up the sparse system by putting the factor for the central pixel to the n-array and the off diagonal elements 
                                       '(the indices of the surrounding pixels that in the matrix have a value of -1) to the r-array.
                                       GetSystemForTileD(pX, pY, pL, pLX, pLY, scanline, scanlineL, pixels, nWidth, nHeight, mask, n, r, b, rrc, plane, bVectorAlg)

                                       'jacobi method to solve
                                       Dim result As Double() = GaussSeidelForPoissonEq_Div(b, n, r, _blendParameters.JacobiGSParams.MaxIterations, True, _blendParameters.JacobiGSParams.DesiredMaxLinearError,
                                                                                                          _blendParameters.PE, w, plane)

                                       results(plane) = result

                                       'make the results re-usable for further iterations
                                       _blendParameters.DiagonalElements(plane) = n
                                       _blendParameters.OtherElements(plane) = r

                                       _blendParameters.BVector(plane) = b
                                   End Sub)
                'Next

                OnShowProgress(_blendParameters.PE.ImgWidthHeight)

                bUX.UnlockBits(bmDataX)
                bUY.UnlockBits(bmDataY)
                _blendParameters.LowerImgDX.UnlockBits(bmSrcX)
                _blendParameters.LowerImgDY.UnlockBits(bmSrcY)
                _blendParameters.LowerImg.UnlockBits(bmSrc)
                pX = Nothing
                pY = Nothing
                pL = Nothing
                pLX = Nothing
                pLY = Nothing

                'write the values back to the image
                CropDrawAndSetLowerImage(_blendParameters.LowerImg, results, pixels, _blendParameters.BmpsUX(_blendParameters.CurX, _blendParameters.CurY), rr, rc, False)

                _blendParameters.ResultsTiles.Add(results)
            End If
        Catch exc As Exception
            Try
                bUX.UnlockBits(bmDataX)
            Catch

            End Try
            Try
                bUY.UnlockBits(bmDataY)
            Catch

            End Try
            Try
                _blendParameters.LowerImg.UnlockBits(bmSrc)
            Catch

            End Try
            Try
                _blendParameters.LowerImgDX.UnlockBits(bmSrcX)
            Catch

            End Try
            Try
                _blendParameters.LowerImgDY.UnlockBits(bmSrcY)
            Catch

            End Try
        Finally

        End Try
    End Sub

    'setup the matrix and the b vector
    Private Sub GetSystemForTileD(pX As Byte(), pY As Byte(), pL As Byte(), pLX As Byte(), pLY As Byte(), scanline As Integer, scanlineL As Integer, pixels As List(Of Point), nWidth As Integer, nHeight As Integer,
                          mask As Integer(), n() As Integer, r As Integer()(), b() As Double, rrc As Rectangle, plane As Integer, alg As IBVectorComputingAlgorithm)

        Dim add As Point() = {New Point(-1, 0), New Point(1, 0), New Point(0, -1), New Point(0, 1)}
        Dim addD As Point() = {New Point(1, 0), New Point(1, 0), New Point(0, 1), New Point(0, 1)}
        Dim ptC As New PointF(rrc.Width / 2.0F, rrc.Height / 2.0F)

        For i As Integer = 0 To pixels.Count - 1
            If Me.CancellationPending Then
                Exit For
            End If
            Dim x As Integer = pixels(i).X
            Dim y As Integer = pixels(i).Y

            'If x > 0 AndAlso x < nWidth - 1 AndAlso y > 0 AndAlso y < nHeight - 1 Then
            Dim xx As Integer = rrc.X + x
            Dim yy As Integer = rrc.Y + y
            Dim pos As Integer = y * scanline + x * 4
            Dim posL As Integer = yy * scanlineL + xx * 4
            Dim neighbors As Integer = 0

            Dim rr() As Integer = {-1, -1, -1, -1, mask(y * nWidth + x)}

            For j As Integer = 0 To addD.Length - 1
                Dim xA As Integer = x + add(j).X
                Dim yA As Integer = y + add(j).Y
                Dim xA1 As Integer = x + addD(j).X
                Dim yA1 As Integer = y + addD(j).Y
                Dim xA2 As Integer = x - addD(j).X
                Dim yA2 As Integer = y - addD(j).Y

                If xA >= 0 AndAlso xA < nWidth AndAlso yA >= 0 AndAlso yA < nHeight Then
                    Dim indx As Integer = mask(yA * nWidth + xA)

                    If indx > Int32.MinValue Then
                        neighbors += 1
                        If indx = -1 Then 'boundary
                            b(i) += pL((yA + rrc.Y) * scanlineL + (xA + rrc.X) * 4 + plane)
                        End If
                        If indx = -2 Then 'crop boundary
                            If alg Is Nothing Then
                                b(i) += pL((yA + rrc.Y) * scanlineL + (xA + rrc.X) * 4 + plane)
                            Else
                                'just take the lowerImgValue here until I can compute a better value...
                                b(i) += pL((yA + rrc.Y) * scanlineL + (xA + rrc.X) * 4 + plane) 'alg.GetCropBoundaryValue(xA, yA, plane, col, colL, p, pL, scanline, scanlineL, rrc, ptC)
                            End If
                            'rr(j) = indx
                        End If
                        If indx > -1 Then
                            If alg Is Nothing Then
                                'derivatives are computed normally (not reversed as the Laplacian), so we need to subtract from b
                                If j < 2 Then
                                    b(i) -= ((pX(yA1 * scanline + xA1 * 4 + plane) - 127) - (pX(yA2 * scanline + xA2 * 4 + plane) - 127))
                                Else
                                    b(i) -= ((pY(yA1 * scanline + xA1 * 4 + plane) - 127) - (pY(yA2 * scanline + xA2 * 4 + plane) - 127))
                                End If
                            Else
                                b(i) -= alg.GetValue(xA1, yA1, xA2, yA2, plane, pX, pY, pLX, pLY, scanline, scanlineL, rrc, ptC, j, x, y)
                            End If
                            rr(j) = indx
                        End If
                    End If
                End If
            Next

            n(i) = neighbors
            r(i) = rr
            'End If
        Next

        'test debug, makes only sense when applyD runs not parallel
        'If alg.GetType().Equals(GetType(GradMaxBVectorAlgorithm)) Then
        '    Dim a As GradMaxBVectorAlgorithm = CType(alg, GradMaxBVectorAlgorithm)
        '    Console.WriteLine("fieldChange sum: " & a.fieldChangeComp & " - fieldChange abs: " & a.fieldChangeAbs & " - OperationsCount: " & a.count)
        'End If
    End Sub

    Public Sub ApplyDForAlpha(rc As Rectangle) Implements IBlendAlgorithm.ApplyDForAlpha 'upperimg als trimap->unknown only mit alphaval 255, lowerimg als trimap, values fg = 255, unknon = 127, bg = 0
        'Jacobi/GaussSeidel

        'mask: Stores the coordinates and the state (inside, boundary or outside of the region).
        'Region is the arbitrary shape of the upper image (based on alpha values. Alpha = 0 is outside)
        'We need this mapping of the coordinates just *because* of the arbitrary shape, since the vector that contains
        'all pixels to process is not a rectangular mapping, and so, the length a "row" or "column" could differ 
        'from the length of other rows or columns.
        Dim mask(_blendParameters.UpperImg.Width * _blendParameters.UpperImg.Height - 1) As Integer

        'get the set of pixels to process
        Dim pixels As List(Of Point) = GetPixels(_blendParameters.UpperImg, _blendParameters.LowerImg, rc, mask, _blendParameters.MinAlpha)

        Dim bmDataX As BitmapData = Nothing
        Dim bmDataY As BitmapData = Nothing
        Dim bmSrc As BitmapData = Nothing
        Dim bmPB As BitmapData = Nothing

        'note, I use fields for these here in this project
        'these store the system to solve
        'Dim results(2)() As Double 'keep the results for further processing
        'Dim diagonalElements(2)() As Integer '4 for inner pixels, <4 for pixels that touch the boundary
        'Dim otherElements(2)()() As Integer 'the indices for the -1 entries

        'the algorithm to compute the differences of the "b" vector
        Dim bVectorAlg As IBVectorComputingAlgorithm = _blendParameters.BVectorAlg 'BVectorComputingAlgorithm.GetAlgorithm(_blendParameters.Params.Algorithm)

        Dim c2 As List(Of ChainCode) = GetBoundary(_blendParameters.UpperImg, _blendParameters.MinAlpha)
        bVectorAlg.SetValuesSilent(_blendParameters.LowerImgWeight, _blendParameters.UpperImgWeight)

        _blendParameters.Pixels = pixels
        _blendParameters.RC = rc

        Dim useBoundary As Boolean = _blendParameters.UseBoundary

        'Dim w As Double = 2 / (1 + Math.Sin(Math.PI / _blendParameters.UpperImg.Width))
        Dim w As Double = Me.ComputeWForSOR(_blendParameters.AutoSOR, _blendParameters.AutoSORMode, pixels.Count,
                                            _blendParameters.UpperImg.Width, _blendParameters.UpperImg.Height, _blendParameters.MinPixelAmount)

        OnShowProgress(20)

        Try
            If AvailMem.AvailMem.checkAvailRam(_blendParameters.UpperImg.Width * _blendParameters.UpperImg.Height * 4L + _blendParameters.LowerImg.Width * _blendParameters.LowerImg.Height * 4L) Then
                bmDataX = _blendParameters.UpperImgDX.LockBits(New Rectangle(0, 0, _blendParameters.UpperImgDX.Width, _blendParameters.UpperImgDX.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmDataY = _blendParameters.UpperImgDY.LockBits(New Rectangle(0, 0, _blendParameters.UpperImgDY.Width, _blendParameters.UpperImgDY.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmSrc = _blendParameters.LowerImg.LockBits(New Rectangle(0, 0, _blendParameters.LowerImg.Width, _blendParameters.LowerImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
                bmPB = _blendParameters.BitmapBlend.LockBits(New Rectangle(0, 0, _blendParameters.BitmapBlend.Width, _blendParameters.BitmapBlend.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

                Dim scanline As Integer = bmDataX.Stride
                Dim nWidth As Integer = _blendParameters.UpperImgDX.Width
                Dim nHeight As Integer = _blendParameters.UpperImgDX.Height

                Dim scanlineL As Integer = bmSrc.Stride
                Dim nWidthL As Integer = _blendParameters.LowerImg.Width
                Dim nHeightL As Integer = _blendParameters.LowerImg.Height

                Dim pX((bmDataX.Stride * bmDataX.Height) - 1) As Byte
                Marshal.Copy(bmDataX.Scan0, pX, 0, pX.Length)

                Dim pY((bmDataY.Stride * bmDataY.Height) - 1) As Byte
                Marshal.Copy(bmDataY.Scan0, pY, 0, pY.Length)

                Dim pL((bmSrc.Stride * bmSrc.Height) - 1) As Byte
                Marshal.Copy(bmSrc.Scan0, pL, 0, pL.Length)

                Dim pB((bmPB.Stride * bmPB.Height) - 1) As Byte
                Marshal.Copy(bmPB.Scan0, pB, 0, pB.Length)

                'For plane As Integer = 0 To 2
                Parallel.For(0, 3, Sub(plane, loopState)

                                       'matrix  elements
                                       Dim n(pixels.Count - 1) As Integer
                                       Dim r(pixels.Count - 1)() As Integer

                                       'b-vector
                                       Dim b(pixels.Count - 1) As Double

                                       'here we setup the system, put all boundary values and the known-function values (differences between pixels, "finite differences") to b.
                                       'and also set up the sparse system by putting the factor for the central pixel to the n-array and the off diagonal elements 
                                       '(the indices of the surrounding pixels that in the matrix have a value of -1) to the r-array.
                                       GetSystemDForAlpha(pX, pY, pL, pB, scanline, scanlineL, pixels, nWidth, nHeight, mask, n, r, b, rc, plane, bVectorAlg, useBoundary)

                                       'jacobi method to solve
                                       Dim result As Double() = GaussSeidelForPoissonEq_Div(b, n, r, _blendParameters.JacobiGSParams.MaxIterations, True, _blendParameters.JacobiGSParams.DesiredMaxLinearError,
                                                                _blendParameters.PE, w, plane)
                                       'make the results re-usable for further iterations
                                       _blendParameters.Results(plane) = result
                                       _blendParameters.DiagonalElements(plane) = n
                                       _blendParameters.OtherElements(plane) = r

                                       _blendParameters.BVector(plane) = b
                                   End Sub)
                'Next

                OnShowProgress(_blendParameters.PE.ImgWidthHeight)

                'write the values back to the image
                InsertPixelsForAllPlanesForAlpha(pL, scanlineL, _blendParameters.Results, pixels, _blendParameters.RC, 0, nWidthL, nHeightL)

                Marshal.Copy(pL, 0, bmSrc.Scan0, pL.Length)
                _blendParameters.UpperImgDX.UnlockBits(bmDataX)
                _blendParameters.UpperImgDY.UnlockBits(bmDataY)
                _blendParameters.LowerImg.UnlockBits(bmSrc)
                _blendParameters.BitmapBlend.UnlockBits(bmPB)
                pX = Nothing
                pY = Nothing
                pL = Nothing
                pB = Nothing
            End If
        Catch exc As Exception
            Try
                _blendParameters.UpperImgDX.UnlockBits(bmDataX)
            Catch

            End Try
            Try
                _blendParameters.UpperImgDY.UnlockBits(bmDataY)
            Catch

            End Try
            Try
                _blendParameters.LowerImg.UnlockBits(bmSrc)
            Catch

            End Try
            Try
                _blendParameters.BitmapBlend.UnlockBits(bmPB)
            Catch

            End Try
        Finally
            _blendParameters.UpperImgDX.Dispose()
            _blendParameters.UpperImgDX = Nothing
            _blendParameters.UpperImgDY.Dispose()
            _blendParameters.UpperImgDY = Nothing
            If _blendParameters.LowerImgDX IsNot Nothing Then
                _blendParameters.LowerImgDX.Dispose()
                _blendParameters.LowerImgDX = Nothing
            End If
            If _blendParameters.LowerImgDY IsNot Nothing Then
                _blendParameters.LowerImgDY.Dispose()
                _blendParameters.LowerImgDY = Nothing
            End If
        End Try
    End Sub

    Private Sub GetSystemDForAlpha(pX As Byte(), pY As Byte(), pL As Byte(), pB As Byte(), scanline As Integer, scanlineL As Integer, pixels As List(Of Point), nWidth As Integer, nHeight As Integer,
                          mask As Integer(), n() As Integer, r As Integer()(), b() As Double, rc As Rectangle, plane As Integer, alg As IBVectorComputingAlgorithm, useBoundary As Boolean)

        Dim add As Point() = {New Point(-1, 0), New Point(1, 0), New Point(0, -1), New Point(0, 1)}
        Dim addD As Point() = {New Point(1, 0), New Point(1, 0), New Point(0, 1), New Point(0, 1)}
        Dim ptC As New PointF(rc.Width / 2.0F, rc.Height / 2.0F)

        For i As Integer = 0 To pixels.Count - 1
            If Me.CancellationPending Then
                Exit For
            End If
            Dim x As Integer = pixels(i).X
            Dim y As Integer = pixels(i).Y

            'If x > 0 AndAlso x < nWidth - 1 AndAlso y > 0 AndAlso y < nHeight - 1 Then
            Dim xx As Integer = rc.X + x
            Dim yy As Integer = rc.Y + y
            Dim pos As Integer = y * scanline + x * 4
            Dim posL As Integer = yy * scanlineL + xx * 4
            Dim neighbors As Integer = 0

            Dim rr() As Integer = {-1, -1, -1, -1, mask(y * nWidth + x)}
            Dim added As Boolean = False

            For j As Integer = 0 To addD.Length - 1
                Dim xA As Integer = x + add(j).X
                Dim yA As Integer = y + add(j).Y
                Dim xA1 As Integer = x + addD(j).X
                Dim yA1 As Integer = y + addD(j).Y
                Dim xA2 As Integer = x - addD(j).X
                Dim yA2 As Integer = y - addD(j).Y

                If xA >= 0 AndAlso xA < nWidth AndAlso yA >= 0 AndAlso yA < nHeight Then
                    Dim indx As Integer = mask(yA * nWidth + xA)

                    If indx > Int32.MinValue Then
                        neighbors += 1
                        If useBoundary AndAlso indx = -1 Then 'boundary
                            b(i) += pL((yA + rc.Y) * scanlineL + (xA + rc.X) * 4 + plane)
                        End If
                        If indx > -1 Then
                            If alg Is Nothing Then
                                'derivatives are computed normally (not reversed as the Laplacian), so we need to subtract from b
                                If j < 2 Then
                                    b(i) -= ((pX(yA1 * scanline + xA1 * 4 + plane) - 127) - (pX(yA2 * scanline + xA2 * 4 + plane) - 127))
                                Else
                                    b(i) -= ((pY(yA1 * scanline + xA1 * 4 + plane) - 127) - (pY(yA2 * scanline + xA2 * 4 + plane) - 127))
                                End If
                            Else
                                b(i) -= alg.GetValue(xA1, yA1, xA2, yA2, plane, pX, pY, pB, scanline, scanlineL, rc, ptC, j)
                            End If
                            rr(j) = indx
                        End If

                        'If indx = -1 Then 'boundary
                        '    b(i) += 0 'pL((yA + rc.Y) * scanlineL + (xA + rc.X) * 4 + plane)
                        'End If
                        'If indx > -1 Then
                        '    If Not added Then
                        '        b(i) -= pB(y * scanline + x * 4 + plane) - 127.0
                        '        added = True
                        '    End If
                        '    'b(i) += pXY(y * scanline + x * 4 + plane) / 4.0
                        '    rr(j) = indx
                        'End If
                    End If
                End If
            Next

            n(i) = neighbors
            r(i) = rr
            'End If
        Next
    End Sub

    Private Sub InsertPixelsForAllPlanesForAlpha(pL As Byte(), scanlineL As Integer, results As Double()(),
                                                 pixels As List(Of Point), rc As Rectangle, subt As Integer, w As Integer, h As Integer)
        Parallel.For(0, h, Sub(y)
                               Dim pos As Integer = y * scanlineL
                               For x As Integer = 0 To w - 1
                                   pL(pos + 3) = 0
                                   pos += 4
                               Next
                           End Sub)

        For i As Integer = 0 To pixels.Count - 1
            Dim x As Integer = pixels(i).X + rc.X
            Dim y As Integer = pixels(i).Y + rc.Y

            pL(y * scanlineL + x * 4) = CByte(Math.Max(Math.Min(results(0)(i) - subt, 255), 0))
            pL(y * scanlineL + x * 4 + 1) = CByte(Math.Max(Math.Min(results(1)(i) - subt, 255), 0))
            pL(y * scanlineL + x * 4 + 2) = CByte(Math.Max(Math.Min(results(2)(i) - subt, 255), 0))
            pL(y * scanlineL + x * 4 + 3) = CByte(255)
        Next
    End Sub
End Class
