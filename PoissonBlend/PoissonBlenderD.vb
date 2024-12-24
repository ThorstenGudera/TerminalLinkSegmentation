Option Strict On

Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports ChainCodeFinder

Public Class PoissonBlenderD
    Public Property BlendParameters As BlendParameters = Nothing
    Public Property TileSize As Integer

    Public Sub New(lowerImg As Bitmap, upperImg As Bitmap)
        Me.BlendParameters = New BlendParameters()
        Me.BlendParameters.LowerImg = lowerImg
        Me.BlendParameters.UpperImg = upperImg

        Dim upperImgDX As Bitmap = Nothing
        Dim upperImgDY As Bitmap = Nothing
        Dim lowerImgDX As Bitmap = Nothing
        Dim lowerImgDY As Bitmap = Nothing

        If AvailMem.AvailMem.checkAvailRam(lowerImg.Width * lowerImg.Height * 8L +
                                           upperImg.Width * upperImg.Height * 8L) Then
            upperImgDX = GetXDerivative(upperImg)
            upperImgDY = GetYDerivative(upperImg)
            lowerImgDX = GetXDerivative(lowerImg)
            lowerImgDY = GetYDerivative(lowerImg)

            Me.BlendParameters.UpperImgDX = upperImgDX
            Me.BlendParameters.UpperImgDY = upperImgDY
            Me.BlendParameters.LowerImgDX = lowerImgDX
            Me.BlendParameters.LowerImgDY = lowerImgDY
        Else
            Throw New Exception("Not enough memory.")
        End If
    End Sub

    Public Sub New(lowerImg As Bitmap)
        Me.BlendParameters = New BlendParameters()
        Me.BlendParameters.LowerImg = lowerImg
        Me.BlendParameters.UpperImg = New Bitmap(lowerImg)
    End Sub

    Private Function GetXDerivative(b As Bitmap) As Bitmap
        Dim bRes As Bitmap = New Bitmap(b.Width, b.Height)

        Dim bmData As BitmapData = bRes.LockBits(New Rectangle(0, 0, bRes.Width, bRes.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
        Dim bmSrc As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

        Dim stride As Integer = bmData.Stride

        Dim p((bmData.Stride * bmData.Height) - 1) As Byte
        Marshal.Copy(bmData.Scan0, p, 0, p.Length)

        Dim pSrc((bmSrc.Stride * bmSrc.Height) - 1) As Byte
        Marshal.Copy(bmSrc.Scan0, pSrc, 0, pSrc.Length)
        b.UnlockBits(bmSrc)

        Dim pos As Integer = 0

        For y As Integer = 0 To bRes.Height - 1
            pos = y * stride
            For x As Integer = 0 To bRes.Width - 1
                If x > 0 AndAlso x < bRes.Width - 1 Then
                    p(pos) = CByte(((CInt(pSrc(pos + 4)) - CInt(pSrc(pos - 4))) / 2 + 127))
                    p(pos + 1) = CByte(((CInt(pSrc(pos + 4 + 1)) - CInt(pSrc(pos - 4 + 1))) / 2 + 127))
                    p(pos + 2) = CByte(((CInt(pSrc(pos + 4 + 2)) - CInt(pSrc(pos - 4 + 2))) / 2 + 127))
                ElseIf x = 0 AndAlso x < bRes.Width - 1 Then
                    p(pos) = CByte(((CInt(pSrc(pos + 4)) - CInt(pSrc(pos))) / 2 + 127))
                    p(pos + 1) = CByte(((CInt(pSrc(pos + 4 + 1)) - CInt(pSrc(pos + 1))) / 2 + 127))
                    p(pos + 2) = CByte(((CInt(pSrc(pos + 4 + 2)) - CInt(pSrc(pos + 2))) / 2 + 127))
                ElseIf x > 0 AndAlso x = bRes.Width - 1 Then
                    p(pos) = CByte(((CInt(pSrc(pos)) - CInt(pSrc(pos - 4))) / 2 + 127))
                    p(pos + 1) = CByte(((CInt(pSrc(pos + 1)) - CInt(pSrc(pos - 4 + 1))) / 2 + 127))
                    p(pos + 2) = CByte(((CInt(pSrc(pos + 2)) - CInt(pSrc(pos - 4 + 2))) / 2 + 127))
                End If

                p(pos + 3) = 255
                pos += 4
            Next
        Next

        Marshal.Copy(p, 0, bmData.Scan0, p.Length)
        bRes.UnlockBits(bmData)
        pSrc = Nothing
        p = Nothing

        Return bRes
    End Function

    Private Function GetYDerivative(b As Bitmap) As Bitmap
        Dim bRes As Bitmap = New Bitmap(b.Width, b.Height)

        Dim bmData As BitmapData = bRes.LockBits(New Rectangle(0, 0, bRes.Width, bRes.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
        Dim bmSrc As BitmapData = b.LockBits(New Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

        Dim stride As Integer = bmData.Stride

        Dim p((bmData.Stride * bmData.Height) - 1) As Byte
        Marshal.Copy(bmData.Scan0, p, 0, p.Length)

        Dim pSrc((bmSrc.Stride * bmSrc.Height) - 1) As Byte
        Marshal.Copy(bmSrc.Scan0, pSrc, 0, pSrc.Length)
        b.UnlockBits(bmSrc)

        Dim pos As Integer = 0

        For y As Integer = 0 To bRes.Height - 1
            pos = y * stride
            For x As Integer = 0 To bRes.Width - 1
                If y > 0 AndAlso y < bRes.Height - 1 Then
                    p(pos) = CByte(((CInt(pSrc(pos + stride)) - CInt(pSrc(pos - stride))) / 2 + 127))
                    p(pos + 1) = CByte(((CInt(pSrc(pos + stride + 1)) - CInt(pSrc(pos - stride + 1))) / 2 + 127))
                    p(pos + 2) = CByte(((CInt(pSrc(pos + stride + 2)) - CInt(pSrc(pos - stride + 2))) / 2 + 127))
                ElseIf y = 0 AndAlso y < bRes.Height - 1 Then
                    p(pos) = CByte(((CInt(pSrc(pos + stride)) - CInt(pSrc(pos))) / 2 + 127))
                    p(pos + 1) = CByte(((CInt(pSrc(pos + stride + 1)) - CInt(pSrc(pos + 1))) / 2 + 127))
                    p(pos + 2) = CByte(((CInt(pSrc(pos + stride + 2)) - CInt(pSrc(pos + 2))) / 2 + 127))
                ElseIf y > 0 AndAlso y = bRes.Height - 1 Then
                    p(pos) = CByte(((CInt(pSrc(pos)) - CInt(pSrc(pos - stride))) / 2 + 127))
                    p(pos + 1) = CByte(((CInt(pSrc(pos + 1)) - CInt(pSrc(pos - stride + 1))) / 2 + 127))
                    p(pos + 2) = CByte(((CInt(pSrc(pos + 2)) - CInt(pSrc(pos - stride + 2))) / 2 + 127))
                End If

                p(pos + 3) = 255
                pos += 4
            Next
        Next

        Marshal.Copy(p, 0, bmData.Scan0, p.Length)
        bRes.UnlockBits(bmData)
        pSrc = Nothing
        p = Nothing

        Return bRes
    End Function

    Public Function SetParameters(minAlpha As Integer, maxIterations As Integer, maxLinError As Double,
                              innerIterations As Integer, preRelax As Boolean, rc As Rectangle,
                              numRestart As Integer, autoSOR As Boolean, autoSORMode As AutoSORMode,
                              minPixelAmount As Integer, upperImgWeight As Double,
                              lowerImgWeight As Double, blendAlgorithm As IBlendAlgorithm,
                              bVectorAlgorithm As IBVectorComputingAlgorithm,
                              drawHQ As Boolean, pe As ProgressEventArgs) As Boolean

        Return Me.BlendParameters.SetValues(minAlpha, maxIterations, maxLinError,
                              innerIterations, preRelax, rc, numRestart,
                              autoSOR, autoSORMode,
                              minPixelAmount, upperImgWeight,
                              lowerImgWeight, blendAlgorithm,
                              bVectorAlgorithm,
                              drawHQ, pe)
    End Function

    Public Function Apply() As Bitmap
        Me.BlendParameters.BlendAlgorithm.ApplyD(Me.BlendParameters.RC)

        Return New Bitmap(Me.BlendParameters.LowerImg)
    End Function

    Public Sub ApplyTileD(bUX As Bitmap, bUY As Bitmap, rc As Rectangle, rr As Rectangle, glBounds As List(Of ChainCode), xx As Integer, yy As Integer)
        Me.BlendParameters.BlendAlgorithm.ApplyTileD(bUX, bUY, rc, rr, glBounds, xx, yy)
    End Sub

    Public Sub ApplyTileMinOverlap(bUXY As Bitmap, rc As Rectangle, rr As Rectangle, glBounds As List(Of ChainCode), xx As Integer, yy As Integer)
        Me.BlendParameters.BlendAlgorithm.ApplyTileMinOvelap(bUXY, rc, rr, glBounds, xx, yy)
    End Sub

    Public Sub ApplyTileMinOverlapD(bUX As Bitmap, bUY As Bitmap, rc As Rectangle, rr As Rectangle, glBounds As List(Of ChainCode), xx As Integer, yy As Integer)
        Me.BlendParameters.BlendAlgorithm.ApplyTileMinOverlapD(bUX, bUY, rc, rr, glBounds, xx, yy)
    End Sub

    Public Sub ApplyDForAlpha()
        Me.BlendParameters.BlendAlgorithm.ApplyDForAlpha(Me.BlendParameters.RC)
    End Sub
End Class

