Option Strict On

Public Class HSLValues
    Public Property HueMin() As Single
        Get
            Return m_HueMin
        End Get
        Set
            m_HueMin = Value
        End Set
    End Property
    Private m_HueMin As Single
    Public Property HueMax() As Single
        Get
            Return m_HueMax
        End Get
        Set
            m_HueMax = Value
        End Set
    End Property
    Private m_HueMax As Single
    Public Property Hue() As Single
        Get
            Return m_Hue
        End Get
        Set
            m_Hue = Value
        End Set
    End Property
    Private m_Hue As Single
    Public Property SaturationMin() As Single
        Get
            Return m_SaturationMin
        End Get
        Set
            m_SaturationMin = Value
        End Set
    End Property
    Private m_SaturationMin As Single
    Public Property SaturationMax() As Single
        Get
            Return m_SaturationMax
        End Get
        Set
            m_SaturationMax = Value
        End Set
    End Property
    Private m_SaturationMax As Single
    Public Property Saturation() As Single
        Get
            Return m_Saturation
        End Get
        Set
            m_Saturation = Value
        End Set
    End Property
    Private m_Saturation As Single
    Public Property LuminanceMin() As Single
        Get
            Return m_LuminanceMin
        End Get
        Set
            m_LuminanceMin = Value
        End Set
    End Property
    Private m_LuminanceMin As Single
    Public Property LuminanceMax() As Single
        Get
            Return m_LuminanceMax
        End Get
        Set
            m_LuminanceMax = Value
        End Set
    End Property
    Private m_LuminanceMax As Single
    Public Property Luminance() As Single
        Get
            Return m_Luminance
        End Get
        Set
            m_Luminance = Value
        End Set
    End Property
    Private m_Luminance As Single
    Public Property AddSaturation() As Boolean
        Get
            Return m_AddSaturation
        End Get
        Set
            m_AddSaturation = Value
        End Set
    End Property
    Private m_AddSaturation As Boolean
    Public Property AddLuminance() As Boolean
        Get
            Return m_AddLuminance
        End Get
        Set
            m_AddLuminance = Value
        End Set
    End Property

    Public Property DoAlpha As Boolean
    Public Property Alpha As Integer
    Public Property AddAlpha As Boolean
    Private m_AddLuminance As Boolean

    Public Property UseRamp As Boolean
    Public Property RampGamma As Double
End Class
