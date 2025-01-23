Option Strict On

Imports System.Drawing
Imports ChainCodeFinder

Public Class BlendParameters
    Implements IDisposable

    Public Property LowerImg As Bitmap = Nothing
    Public Property UpperImg As Bitmap = Nothing
    Public Property BlendAlgorithm As IBlendAlgorithm
    Public Property BVectorAlg As IBVectorComputingAlgorithm = New GradNormalBVectorAlgorithm

    Public Property JacobiGSParams As JacobiGSParams
    Public Property GMRESParams As GMRESParams
    Public Property MinAlpha As Integer = 254
    Public Property MinPixelAmount As Integer = 12
    Public Property UpperImgWeight As Double
    Public Property LowerImgWeight As Double
    Public Property DrawHQ As Boolean = True
    Public Property AutoSOR As Boolean
    Public Property AutoSORMode As AutoSORMode
    Public Property CopyLowerImageRect As Boolean
    Public Property UseIntRects As Boolean

    Public Property Pixels As List(Of Point) = Nothing
    Public Property RC As Rectangle
    Public Property Results As Double()()
    Friend Property DiagonalElements As Integer()()
    Friend Property OtherElements As Integer()()()
    Friend Property BVector As Double()()

    Public Property PE As ProgressEventArgs = Nothing

    Public Property LowerImgDX As Bitmap = Nothing
    Public Property UpperImgDX As Bitmap = Nothing
    Public Property LowerImgDY As Bitmap = Nothing
    Public Property UpperImgDY As Bitmap = Nothing
    Public Property UpperImgOrg As Bitmap = Nothing
    Public Property UpperADX As Short()
    Public Property UpperADY As Short()
    Public Property LowerADX As Short()
    Public Property LowerADY As Short()

    Public Property ResultsTiles As List(Of Double()())
    Public Property BmpsUX As BitmapPositionPoisson(,)
    Public Property BmpsUY As BitmapPositionPoisson(,)
    Public Property CurY As Integer
    Public Property CurX As Integer
    Public Property BVectorAlgType As Integer
    Public Property Overlap As Integer
    Public Property BmpsLX As BitmapPositionPoisson(,)
    Public Property BmpsLY As BitmapPositionPoisson(,)
    Public Property UseDblSzInterpolationTile As Boolean
    Public Property UsePPP As Boolean
    Public Property PppPic As Bitmap
    Public Property BitmapBlend As Bitmap
    Public Property UseBoundary As Boolean
    Public Property GrayscaleIntermediatePic As Boolean
    Public Property FeatherIntermediatePics As Boolean
    Public Property Gamma As Double
    Public Property MaxPixelDist As Integer
    Public Property UpperImgMixRel As Double
    Public Property UseOrigForFeathering As Boolean
    Public Property CurrentBoundary As List(Of ChainCode)

    Public Function SetValues(minALpha As Integer, maxIterations As Integer, maxLinError As Double,
                              innerIterations As Integer, preRelax As Boolean, rc As Rectangle,
                              numRestart As Integer, autoSOR As Boolean, autoSORMode As AutoSORMode,
                              minPixelAmount As Integer, upperImgWeight As Double,
                              lowerImgWeight As Double, algorithm As IBlendAlgorithm,
                              bVectorAlgorithm As IBVectorComputingAlgorithm,
                              drawHQ As Boolean, pe As ProgressEventArgs) As Boolean

        Me.MinAlpha = minALpha
        Me.MinPixelAmount = minPixelAmount
        Me.UpperImgWeight = upperImgWeight
        Me.LowerImgWeight = lowerImgWeight
        Me.BlendAlgorithm = algorithm
        Me.BVectorAlg = bVectorAlgorithm
        Me.DrawHQ = drawHQ
        Me.AutoSOR = autoSOR
        Me.AutoSORMode = autoSORMode
        Me.RC = rc
        Me.PE = pe

        Me.JacobiGSParams = New JacobiGSParams() With {
            .MaxIterations = maxIterations,
            .DesiredMaxLinearError = maxLinError}

        Me.GMRESParams = New GMRESParams() With {
            .MaxInnerIterationsGMRES = innerIterations,
            .DesiredMaxLinearError = maxLinError,
            .PreRelax = preRelax,
            .MaxRestartAmountGMRES = numRestart}

        If Me.Results Is Nothing Then
            Dim results(2)() As Double
            Me.Results = results
        End If
        If Me.DiagonalElements Is Nothing Then
            Dim diagonalElements(2)() As Integer
            Me.DiagonalElements = diagonalElements
        End If
        If Me.OtherElements Is Nothing Then
            Dim otherElements(2)()() As Integer
            Me.OtherElements = otherElements
        End If
        If Me.BVector Is Nothing Then
            Dim bVector(2)() As Double
            Me.BVector = bVector
        End If

        Return True
    End Function

    Public Function GetBVectorAlg(n As Integer) As IBVectorComputingAlgorithm
        Select Case n
            Case 0
                Return New GradAddBVectorAlgorithm()
            Case 1
                Return New GradNormalBVectorAlgorithm()
            Case 2
                Return New GradMaxBVectorAlgorithm()
            Case 3
                Return New GradAddFixedWeightBVectorAlgorithm()
            Case 4
                Return New GradNormalDynamicWeightAlgorithm()
            Case 5
                Return New GradAddDynamicWeightAlgorithm()
            Case Else
                Return New GradAddBVectorAlgorithm()
        End Select
    End Function

    Public Function GetCalcAlg(n As Integer) As IBlendAlgorithm
        Select Case n
            Case 0
                Return New BlendAlgorithmGaussSeidelSOR() With {.BlendParameters = Me}
            Case 1
                Return New BlendAlgorithmGMRES_restarted() With {.BlendParameters = Me}
            Case Else
                Return New BlendAlgorithmGaussSeidelSOR() With {.BlendParameters = Me}
        End Select
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                If Not Me.UpperImg Is Nothing Then
                    Me.UpperImg.Dispose()
                    Me.UpperImg = Nothing
                End If
                If Not Me.LowerImg Is Nothing Then
                    Me.LowerImg.Dispose()
                    Me.LowerImg = Nothing
                End If
                If Not Me.UpperImgDX Is Nothing Then
                    Me.UpperImgDX.Dispose()
                    Me.UpperImgDX = Nothing
                End If
                If Not Me.LowerImgDX Is Nothing Then
                    Me.LowerImgDX.Dispose()
                    Me.LowerImgDX = Nothing
                End If

                If Not Me.BitmapBlend Is Nothing Then
                    Me.BitmapBlend.Dispose()
                    Me.BitmapBlend = Nothing
                End If

                If Not Me.UpperImgDY Is Nothing Then
                    Me.UpperImgDY.Dispose()
                    Me.UpperImgDY = Nothing
                End If
                If Not Me.LowerImgDY Is Nothing Then
                    Me.LowerImgDY.Dispose()
                    Me.LowerImgDY = Nothing
                End If
                If Not Me.PppPic Is Nothing Then
                    Me.PppPic.Dispose()
                    Me.PppPic = Nothing
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
