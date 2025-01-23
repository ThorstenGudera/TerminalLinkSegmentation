Option Strict On
Imports System.Drawing

Public Class PathInfo
    Public Property Points As List(Of PointF)
    Public Property Offset As Point
    Public Property RoundCaps As Boolean
    Public Property DrawWidth As Single
    Public Property SourcePt As Point
    Public Property DestPt As Point
    Public Property BVAlg As Integer
    Public Property UpperWeight As Double
    Public Property LowerWeight As Double

    Public Sub Setup()
        If Me.Points Is Nothing Then
            Me.Points = New List(Of PointF)
        End If
    End Sub

    Public Function Count() As Integer
        Setup()
        Return Me.Points.Count
    End Function

    Public Function ToArray() As PointF()
        Setup()
        Return Me.Points.ToArray()
    End Function

    Public Sub Add(pt As PointF)
        Setup()
        Me.Points.Add(pt)
    End Sub

    Public Sub AddRange(pts() As PointF)
        Setup()
        Me.Points.AddRange(pts)
    End Sub
End Class
