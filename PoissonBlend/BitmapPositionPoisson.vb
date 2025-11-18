Option Strict On
Imports System.Drawing

Public Class BitmapPositionPoisson
    Implements IDisposable
    Public Property Bmp() As Bitmap
        Get
            Return m_Bmp
        End Get
        Set(value As Bitmap)
            m_Bmp = value
        End Set
    End Property
    Private m_Bmp As Bitmap

    Public Property Loc() As PointF
        Get
            Return m_Loc
        End Get
        Set(value As PointF)
            m_Loc = value
        End Set
    End Property
    Private m_Loc As PointF

    Public Property BmpSize() As Size

    Public Property BmpPath() As String
    Public Property DivergenceEnergy As Double
    Public Property ResultDivergenceEnergy As Double

    Public Function GetBmpFromPath(path As String) As Bitmap
        Dim bmpOut As Bitmap = Nothing
        Try
            Using img As Image = Image.FromFile(path, False)
                bmpOut = CType(img.Clone(), Bitmap)
            End Using
        Catch ex As Exception

        End Try
        Return bmpOut
    End Function

    Friend Sub SetBmp(bmpWork As Bitmap, saveToCache As Boolean)
        Dim bOld As Bitmap = Me.Bmp
        Me.Bmp = bmpWork

        If Not bOld Is Nothing AndAlso Not bOld.Equals(Me.Bmp) Then
            bOld.Dispose()
            bOld = Nothing
        End If

        If saveToCache Then
            Me.Bmp.Save(Me.BmpPath, Imaging.ImageFormat.Png)
        End If
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' Verwalteten Zustand löschen (verwaltete Objekte).
                If Bmp IsNot Nothing Then
                    Bmp.Dispose()
                End If
            End If

            ' Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() unten überschreiben.
            ' Große Felder auf NULL festlegen.

        End If
        Me.disposedValue = True
    End Sub

    ' Finalize() nur überschreiben, wenn Dispose(ByVal disposing As Boolean) oben über Code zum Freigeben von nicht verwalteten Ressourcen verfügt.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
