Option Strict On
Friend Class HSLRange
    Public Zrange1 As FRange
    Public Zrange2 As FRange

    Public Sub New()
        Zrange1 = New FRange()
        Zrange2 = New FRange()
    End Sub

    Public Property Range1() As FRange
        Get
            Return Zrange1
        End Get
        Set
            Zrange1 = Value
        End Set
    End Property

    Public Property Range2() As FRange
        Get
            Return Zrange2
        End Get
        Set
            Zrange2 = Value
        End Set
    End Property

    Public Function Contains(Value As Single) As Boolean
        If Range1.Min <> Range1.Max AndAlso Value >= Range1.Min AndAlso Value <= Range1.Max Then
            Return True
        End If
        If Range2.Min <> Range2.Max AndAlso Value >= Range2.Min AndAlso Value <= Range2.Max Then
            Return True
        End If
        Return False
    End Function
End Class
