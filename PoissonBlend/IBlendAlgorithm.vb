Option Strict On
Imports System.ComponentModel
Imports System.Drawing
Imports ChainCodeFinder

Public Interface IBlendAlgorithm
    Event ShowInfo As EventHandler(Of String)
    Event ShowProgess As EventHandler(Of ProgressEventArgs)
    Event BoundaryError As EventHandler(Of String)
    Property BlendParameters As BlendParameters
    Property CancellationPending As Boolean
    Property BGW As BackgroundWorker
    Sub Apply(rc As Rectangle)
    Sub ApplyD(rc As Rectangle)
    Sub ApplyA(rc As Rectangle)
    Sub ApplyTile(upperImg As Bitmap, rc As Rectangle, rr As Rectangle, glBounds As List(Of ChainCode), xx As Integer, yy As Integer)
    Sub ApplyTileMinOvelap(upperImg As Bitmap, rc As Rectangle, rr As Rectangle, glBounds As List(Of ChainCode), xx As Integer, yy As Integer)
    Sub ApplyInterpolationTile(bUXY As Bitmap, upperImgOrg As Bitmap, amountTilesPerRow As Integer, rc As Rectangle, doubleSizeInterpolationTile As Boolean)
    Sub ApplyTileD(bUX As Bitmap, bUY As Bitmap, rc As Rectangle, rr As Rectangle, glBounds As List(Of ChainCode), xx As Integer, yy As Integer)
    Sub ApplyInterpolationTileD(bUXY As Bitmap, upperImgOrg As Bitmap, amountTilesPerRow As Integer, rc As Rectangle, doubleSizeInterpolationTile As Boolean)
    Sub ApplyTileMinOverlapD(bUX As Bitmap, bUY As Bitmap, rc As Rectangle, rr As Rectangle, glBounds As List(Of ChainCode), xx As Integer, yy As Integer)
    Sub ApplyDForAlpha(rc As Rectangle)

    Sub ApplyG(rc As Rectangle)
    Sub ApplyG2(rc As Rectangle)
End Interface
