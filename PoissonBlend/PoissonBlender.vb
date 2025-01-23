Option Strict On

Imports System.Drawing
Imports ChainCodeFinder

Public Class PoissonBlender
    Public Property BlendParameters As BlendParameters = Nothing
    Public Property TileSize As Integer

    Public Sub New(lowerImg As Bitmap, upperImg As Bitmap)
        Me.BlendParameters = New BlendParameters()
        Me.BlendParameters.LowerImg = lowerImg
        Me.BlendParameters.UpperImg = upperImg
    End Sub

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
        Me.BlendParameters.BlendAlgorithm.Apply(Me.BlendParameters.RC)

        Return New Bitmap(Me.BlendParameters.LowerImg)
    End Function

    Public Function ApplyG() As Bitmap
        Me.BlendParameters.BlendAlgorithm.ApplyG(Me.BlendParameters.RC)

        Return New Bitmap(Me.BlendParameters.LowerImg)
    End Function

    Public Function ApplyG2() As Bitmap
        Me.BlendParameters.BlendAlgorithm.ApplyG2(Me.BlendParameters.RC)

        Return New Bitmap(Me.BlendParameters.LowerImg)
    End Function

    Public Sub ApplyTile(bUXY As Bitmap, rc As Rectangle, rr As Rectangle, glBounds As List(Of ChainCode), xx As Integer, yy As Integer)
        Me.BlendParameters.BlendAlgorithm.ApplyTile(bUXY, rc, rr, glBounds, xx, yy)
    End Sub

    Public Sub ApplyTileMinOverlap(bUXY As Bitmap, rc As Rectangle, rr As Rectangle, glBounds As List(Of ChainCode), xx As Integer, yy As Integer)
        Me.BlendParameters.BlendAlgorithm.ApplyTileMinOvelap(bUXY, rc, rr, glBounds, xx, yy)
    End Sub
End Class
