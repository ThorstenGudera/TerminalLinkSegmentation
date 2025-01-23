Option Strict On
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Public Class frmHSLRange
    Inherits Form
    Private _bitmap As New Bitmap(100, 100)
    Private _vals As New HSLValues()
    Private _bSrc As Bitmap = Nothing
    Private _left As Integer = 0
    Private _top As Integer = 0
    Private _x As Integer = 0
    Private _y As Integer = 0
    Private _tracking As Boolean = False
    Private _luminance As Single = 0F
    Private _saturation As Single = 0F
    Private _hue As Single = 0F
    Private _bloaded As Boolean = False
    Private _farb As Color = Color.Black
    Private _alpha As Integer
    Private _zoom As Single
    Shared static_Button6_Click_CustomColors As Integer() = {}

    Public ReadOnly Property Vals() As HSLValues
        Get
            Return _vals
        End Get
    End Property

    Public Sub New(imgIn As Bitmap)
        InitializeComponent()

        If AvailMem.AvailMem.checkAvailRam(imgIn.Width * imgIn.Height * 8L) Then
            _bitmap = New Bitmap(makeBitmap(imgIn))
            _bSrc = DirectCast(imgIn.Clone(), Bitmap)
        Else
            MessageBox.Show("Not enough Memory")
            Return
        End If

        Dim x As Integer = Me.panel1.ClientSize.Width - _bitmap.Width
        Dim y As Integer = Me.panel1.ClientSize.Height - _bitmap.Height
        x \= 2
        y \= 2

        pictureBox1.Left = x
        pictureBox1.Top = y
        pictureBox1.Image = DirectCast(_bitmap.Clone(), Bitmap)

        Me.AcceptButton = button2
        Me.CancelButton = button1

        Me.ComboBox1.SelectedItem = Me.ComboBox1.Items(0)
        Me.ComboBox2.SelectedItem = Me.ComboBox2.Items(0)
        Me.ComboBox3.SelectedItem = Me.ComboBox3.Items(0)
        Me.ComboBox4.SelectedItem = Me.ComboBox4.Items(0)

        AddHandler NumericUpDown1.ValueChanged, AddressOf NumericUpDown_ValueChanged
        AddHandler NumericUpDown2.ValueChanged, AddressOf NumericUpDown_ValueChanged

        AddHandler NumericUpDown3.ValueChanged, AddressOf NumericUpDown_ValueChanged
        AddHandler NumericUpDown4.ValueChanged, AddressOf NumericUpDown_ValueChanged
        AddHandler NumericUpDown5.ValueChanged, AddressOf NumericUpDown_ValueChanged
        AddHandler NumericUpDown6.ValueChanged, AddressOf NumericUpDown_ValueChanged

        HLSPic()
    End Sub

    Private Function makeBitmap(bmp As Bitmap) As Bitmap
        'Bitmap der passenden Grösse (für panel1) erstellen und zurückgeben
        Dim faktor As Double = CDbl(panel1.Width) / CDbl(panel1.Height)
        Dim multiplier As Double = CDbl(bmp.Width) / CDbl(bmp.Height)
        Dim zoom As Single = 1.0F

        If multiplier >= faktor Then
            zoom = CSng(panel1.ClientRectangle.Width) / CSng(bmp.Width)
        Else
            zoom = CSng(panel1.ClientRectangle.Height) / CSng(bmp.Height)
        End If

        Dim bitmap As Bitmap = Nothing
        Dim w2 As Integer = Convert.ToInt32(bmp.Width * zoom)
        Dim h2 As Integer = Convert.ToInt32(bmp.Height * zoom)
        If AvailMem.AvailMem.checkAvailRam(w2 * h2 * 4L) Then
            bitmap = New Bitmap(w2, h2)
        Else
            Return Nothing
        End If
        Dim gx As Graphics = Graphics.FromImage(bitmap)
        gx.SmoothingMode = SmoothingMode.AntiAlias
        gx.InterpolationMode = InterpolationMode.HighQualityBicubic
        'gx.InterpolationMode = InterpolationMode.HighQualityBilinear
        gx.DrawImage(bmp, 0, 0, Convert.ToInt32(bmp.Width * zoom), Convert.ToInt32(bmp.Height * zoom))
        gx.Dispose()

        _zoom = zoom

        Return bitmap
    End Function

    Private Sub HLSPic()
        If AvailMem.AvailMem.checkAvailRam(_bitmap.Width * _bitmap.Height * 8L) Then
            'Bild für PictureBox bearbeiten
            Dim bmpOut As Bitmap = DirectCast(_bitmap.Clone(), Bitmap)

            fipbmp.Bereich(bmpOut, CSng(Me.NumericUpDown1.Value), CSng(Me.NumericUpDown2.Value), Me._hue, Me._saturation,
                                    Me._luminance, Not Convert.ToBoolean(Me.ComboBox1.SelectedIndex),
                                    Not Convert.ToBoolean(Me.ComboBox2.SelectedIndex), CSng(Me.NumericUpDown3.Value),
                                    CSng(Me.NumericUpDown4.Value), CSng(Me.NumericUpDown5.Value), CSng(Me.NumericUpDown6.Value),
                                    Me.CheckBox2.Checked, Me._alpha, Not Convert.ToBoolean(Me.ComboBox4.SelectedIndex),
                                    CheckBox3.Checked, CDbl(NumericUpDown7.Value))

            Dim x As Integer = Me.panel1.ClientSize.Width - _bitmap.Width
            Dim y As Integer = Me.panel1.ClientSize.Height - _bitmap.Height
            x \= 2
            y \= 2

            pictureBox1.Left = x
            pictureBox1.Top = y

            Dim bmpOut2 As Bitmap = DirectCast(bmpOut.Clone(), Bitmap)

            'bmpOut wird mit Object "f" disposed
            'bmpOut.Dispose()
            pictureBox1.Image.Dispose()
            pictureBox1.Image = bmpOut2

            'Bild für Farbbereich zeichnen
            Dim bmp As New Bitmap(Me.Label7.Width, Me.Label7.Height)
            Dim g As Graphics = Graphics.FromImage(bmp)

            Dim color1 As Color = Nothing
            Dim color2 As Color = Nothing
            Dim color3 As Color = Nothing
            Dim color4 As Color = Nothing
            Dim color5 As Color = Nothing
            Dim color7 As Color = Nothing
            Dim color71 As Color = Nothing

            'nötige Farben ermittlen
            If Me.NumericUpDown2.Value > Me.NumericUpDown1.Value Then
                Dim p1 As PixelData = fipbmp.HSLtoRGB(CDbl(Me.NumericUpDown1.Value), 0.5F, 0.5F)
                color1 = Color.FromArgb(p1.red, p1.green, p1.blue)

                Dim p2 As PixelData = fipbmp.HSLtoRGB(CDbl(Me.NumericUpDown1.Value) + CDbl(Me.NumericUpDown2.Value - Me.NumericUpDown1.Value) / 6.0, 0.5F, 0.5F)
                color2 = Color.FromArgb(p2.red, p2.green, p2.blue)

                Dim p3 As PixelData = fipbmp.HSLtoRGB(CDbl(Me.NumericUpDown1.Value) + CDbl(Me.NumericUpDown2.Value - Me.NumericUpDown1.Value) / 3.0, 0.5F, 0.5F)
                color3 = Color.FromArgb(p3.red, p3.green, p3.blue)

                Dim p4 As PixelData = fipbmp.HSLtoRGB(CDbl(Me.NumericUpDown1.Value) + CDbl(Me.NumericUpDown2.Value - Me.NumericUpDown1.Value) / 2.0, 0.5F, 0.5F)
                color4 = Color.FromArgb(p4.red, p4.green, p4.blue)

                Dim p5 As PixelData = fipbmp.HSLtoRGB(CDbl(Me.NumericUpDown1.Value) + CDbl(Me.NumericUpDown2.Value - Me.NumericUpDown1.Value) * (2.0 / 3.0), 0.5F, 0.5F)
                color5 = Color.FromArgb(p5.red, p5.green, p5.blue)

                Dim p7 As PixelData = fipbmp.HSLtoRGB(CDbl(Me.NumericUpDown1.Value) + CDbl(Me.NumericUpDown2.Value - Me.NumericUpDown1.Value) * (5.0 / 6.0), 0.5F, 0.5F)
                color7 = Color.FromArgb(p7.red, p7.green, p7.blue)

                Dim p71 As PixelData = fipbmp.HSLtoRGB(CDbl(Me.NumericUpDown2.Value), 0.5F, 0.5F)
                color71 = Color.FromArgb(p71.red, p71.green, p71.blue)
            Else
                Dim p1 As PixelData = fipbmp.HSLtoRGB(CDbl(Me.NumericUpDown1.Value), 0.5F, 0.5F)
                color1 = Color.FromArgb(p1.red, p1.green, p1.blue)

                Dim z As Double = CDbl(Me.NumericUpDown1.Value) + CDbl(360.0F - CDbl(Me.NumericUpDown1.Value) + CDbl(Me.NumericUpDown2.Value)) / 6.0
                If z > 360.0F Then
                    z -= 360.0F
                End If
                Dim p2 As PixelData = fipbmp.HSLtoRGB(z, 0.5F, 0.5F)
                color2 = Color.FromArgb(p2.red, p2.green, p2.blue)

                z = CDbl(Me.NumericUpDown1.Value) + CDbl(360.0F - CDbl(Me.NumericUpDown1.Value) + CDbl(Me.NumericUpDown2.Value)) / 3.0
                If z > 360.0F Then
                    z -= 360.0F
                End If
                Dim p3 As PixelData = fipbmp.HSLtoRGB(z, 0.5F, 0.5F)
                color3 = Color.FromArgb(p3.red, p3.green, p3.blue)

                z = CDbl(Me.NumericUpDown1.Value) + CDbl(360.0F - CDbl(Me.NumericUpDown1.Value) + CDbl(Me.NumericUpDown2.Value)) / 2.0
                If z > 360.0F Then
                    z -= 360.0F
                End If
                Dim p4 As PixelData = fipbmp.HSLtoRGB(z, 0.5F, 0.5F)
                color4 = Color.FromArgb(p4.red, p4.green, p4.blue)

                z = CDbl(Me.NumericUpDown1.Value) + CDbl(360.0F - CDbl(Me.NumericUpDown1.Value) + CDbl(Me.NumericUpDown2.Value)) * (2.0 / 3.0)
                If z > 360.0F Then
                    z -= 360.0F
                End If
                Dim p5 As PixelData = fipbmp.HSLtoRGB(z, 0.5F, 0.5F)
                color5 = Color.FromArgb(p5.red, p5.green, p5.blue)

                z = CDbl(Me.NumericUpDown1.Value) + CDbl(360.0F - CDbl(Me.NumericUpDown1.Value) + CDbl(Me.NumericUpDown2.Value)) * (5.0 / 6.0)
                If z > 360.0F Then
                    z -= 360.0F
                End If
                Dim p7 As PixelData = fipbmp.HSLtoRGB(z, 0.5F, 0.5F)
                color7 = Color.FromArgb(p7.red, p7.green, p7.blue)

                Dim p71 As PixelData = fipbmp.HSLtoRGB(CDbl(Me.NumericUpDown2.Value), 0.5F, 0.5F)
                color71 = Color.FromArgb(p71.red, p71.green, p71.blue)
            End If

            'Brushes
            Dim b As New LinearGradientBrush(New RectangleF(0, 0, bmp.Width, CSng(bmp.Height) / 6.0F), color1, color2, LinearGradientMode.Vertical)
            Dim b2 As New LinearGradientBrush(New RectangleF(0, CSng(bmp.Height) / 6.0F, bmp.Width, CSng(bmp.Height) / 6.0F), color2, color3, LinearGradientMode.Vertical)
            Dim b3 As New LinearGradientBrush(New RectangleF(0, CSng(bmp.Height) / 6.0F * 2.0F, bmp.Width, CSng(bmp.Height) / 6.0F), color3, color4, LinearGradientMode.Vertical)
            Dim b4 As New LinearGradientBrush(New RectangleF(0, CSng(bmp.Height) / 6.0F * 3.0F, bmp.Width, CSng(bmp.Height) / 6.0F), color4, color5, LinearGradientMode.Vertical)
            Dim b5 As New LinearGradientBrush(New RectangleF(0, CSng(bmp.Height) / 6.0F * 4.0F, bmp.Width, CSng(bmp.Height) / 6.0F), color5, color7, LinearGradientMode.Vertical)
            Dim b7 As New LinearGradientBrush(New RectangleF(0, CSng(bmp.Height) / 6.0F * 5.0F, bmp.Width, CSng(bmp.Height) / 6.0F), color7, color71, LinearGradientMode.Vertical)

            'zeichnen
            g.FillRectangle(b, New Rectangle(0, 0, bmp.Width, Convert.ToInt32(CDbl(bmp.Height) / 6.0)))
            g.FillRectangle(b2, New Rectangle(0, Convert.ToInt32(CDbl(bmp.Height) / 6.0), bmp.Width, Convert.ToInt32(CDbl(bmp.Height) / 6.0)))
            g.FillRectangle(b3, New Rectangle(0, Convert.ToInt32(CDbl(bmp.Height) / 6.0 * 2.0), bmp.Width, Convert.ToInt32(CDbl(bmp.Height) / 6.0)))
            g.FillRectangle(b4, New Rectangle(0, Convert.ToInt32(CDbl(bmp.Height) / 6.0 * 3.0), bmp.Width, Convert.ToInt32(CDbl(bmp.Height) / 6.0)))
            g.FillRectangle(b5, New Rectangle(0, Convert.ToInt32(CDbl(bmp.Height) / 6.0 * 4.0), bmp.Width, Convert.ToInt32(CDbl(bmp.Height) / 6.0)))
            g.FillRectangle(b7, New Rectangle(0, Convert.ToInt32(CDbl(bmp.Height) / 6.0 * 5.0), bmp.Width, Convert.ToInt32(CDbl(bmp.Height) / 6.0)))

            'aufräumen
            b.Dispose()
            b2.Dispose()
            b3.Dispose()
            b4.Dispose()
            b5.Dispose()
            b7.Dispose()

            '... und darstellen
            If (Label7.BackgroundImage IsNot Nothing) Then
                Label7.BackgroundImage.Dispose()
            End If
            Label7.BackgroundImage = bmp

            'Variable für Property aktualisieren
            Me._vals = New HSLValues() With {
                .HueMin = CSng(Me.NumericUpDown1.Value),
                .HueMax = CSng(Me.NumericUpDown2.Value),
                .Hue = Me._hue,
                .SaturationMin = CSng(Me.NumericUpDown3.Value),
                .SaturationMax = CSng(Me.NumericUpDown4.Value),
                .Saturation = Me._saturation,
                .LuminanceMin = CSng(Me.NumericUpDown5.Value),
                .LuminanceMax = CSng(Me.NumericUpDown6.Value),
                .Luminance = Me._luminance,
                .AddSaturation = (If(Me.ComboBox1.SelectedIndex = 0, True, False)),
                .AddLuminance = (If(Me.ComboBox2.SelectedIndex = 0, True, False)),
                .DoAlpha = Me.CheckBox2.Checked,
                .Alpha = Me._alpha,
                .AddAlpha = (If(Me.ComboBox4.SelectedIndex = 0, True, False)),
                .UseRamp = Me.CheckBox3.Checked,
                .RampGamma = CDbl(Me.NumericUpDown7.Value)
        }
        End If
    End Sub

    Private Sub NumericUpDown_ValueChanged(sender As Object, e As EventArgs)
        HLSPic()
    End Sub

    Private Sub pictureBox1_MouseMove(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles pictureBox1.MouseMove
        If _tracking = False Then
            If e.X >= 0 And e.X < _bitmap.Width And e.Y >= 0 And e.Y < _bitmap.Height Then
                Dim ix As Integer = e.X
                Dim iy As Integer = e.Y

                Dim gfcolor As Color = _bitmap.GetPixel(ix, iy)
                Dim hls As HSLData = fipbmp.RGBtoHSL(gfcolor.R, gfcolor.G, gfcolor.B)

                Me.Label8.Text = "HLS: " + hls.Hue.ToString("0.#") + "; " + hls.Saturation.ToString("0.####") + "; " + hls.Luminance.ToString("0.####")
                Me.Label13.Text = " x = " & (_left + ix / _zoom).ToString("N0") & "; y = " & (_top + iy / _zoom).ToString("N0")
            End If
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox1.CheckedChanged
        'Darstellungsmodus des Bildes in pictureBox1 umschalten (gezoomt, bzw. "gecropped" (Ausschnitt der entsprechenden Grösse))
        If CheckBox1.Checked Then
            Me.pictureBox1.SizeMode = PictureBoxSizeMode.Normal
            Me.pictureBox1.Location = New Point(0, 0)
            Me.pictureBox1.Size = New Size(Me.panel1.Width, Me.panel1.Height)
            Me.pictureBox1.Cursor = Cursors.Hand

            _left = 0
            _top = 0

            AddHandler pictureBox1.MouseDown, AddressOf pictureBox1_MouseDown
            AddHandler pictureBox1.MouseMove, AddressOf pictureBox1_MouseMove_2
            AddHandler pictureBox1.MouseUp, AddressOf pictureBox1_MouseUp

            If AvailMem.AvailMem.checkAvailRam(Me.pictureBox1.ClientSize.Width * Me.pictureBox1.ClientSize.Height * 4L) Then
                _bitmap = New Bitmap(Me.pictureBox1.ClientSize.Width, Me.pictureBox1.ClientSize.Height)

                Dim g As Graphics = Graphics.FromImage(_bitmap)
                g.DrawImage(_bSrc, New Rectangle(0, 0, _bitmap.Width, _bitmap.Height), New Rectangle(_left, _top, Me.pictureBox1.ClientSize.Width, Me.pictureBox1.ClientSize.Height), GraphicsUnit.Pixel)
                g.Dispose()
            End If

            _zoom = 1
        Else
            Me.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize
            Me.pictureBox1.Cursor = Cursors.[Default]

            RemoveHandler pictureBox1.MouseDown, AddressOf pictureBox1_MouseDown
            RemoveHandler pictureBox1.MouseMove, AddressOf pictureBox1_MouseMove_2
            RemoveHandler pictureBox1.MouseUp, AddressOf pictureBox1_MouseUp

            Dim b2 As Bitmap = makeBitmap(_bSrc)
            If AvailMem.AvailMem.checkAvailRam(b2.Width * b2.Height * 4L) Then
                _bitmap = New Bitmap(b2)
            Else
                Return
            End If
        End If

        Me.pictureBox1.Image.Dispose()
        If AvailMem.AvailMem.checkAvailRam(_bitmap.Width * _bitmap.Height * 4L) Then
            Me.pictureBox1.Image = DirectCast(_bitmap.Clone(), Bitmap)
            HLSPic()
        End If
    End Sub

    Private Sub pictureBox1_MouseDown(sender As System.Object, e As System.Windows.Forms.MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            _x = e.X
            _y = e.Y
            _tracking = True
        End If
    End Sub

    Private Sub pictureBox1_MouseMove_2(sender As System.Object, e As System.Windows.Forms.MouseEventArgs)
        If _tracking Then
            _left = Math.Min(Math.Max(_left + (_x - e.X), 0), _bSrc.Width - Me.pictureBox1.ClientSize.Width)
            _top = Math.Min(Math.Max(_top + (_y - e.Y), 0), _bSrc.Height - Me.pictureBox1.ClientSize.Height)
        End If
    End Sub

    Private Sub pictureBox1_MouseUp(sender As System.Object, e As System.Windows.Forms.MouseEventArgs)
        'geänderten Auschnitt des Bildes (Verschiebung) darstellen
        If (_tracking) And (_bSrc.Width > pictureBox1.Width Or _bSrc.Height > pictureBox1.Height) Then
            If AvailMem.AvailMem.checkAvailRam(Me.pictureBox1.ClientSize.Width * Me.pictureBox1.ClientSize.Height * 4L) Then
                _bitmap = New Bitmap(Me.pictureBox1.ClientSize.Width, Me.pictureBox1.ClientSize.Height)
            Else
                Return
            End If
            Dim g As Graphics = Graphics.FromImage(_bitmap)
            g.DrawImage(_bSrc, New Rectangle(0, 0, _bitmap.Width, _bitmap.Height), New Rectangle(_left, _top, Me.pictureBox1.ClientSize.Width, Me.pictureBox1.ClientSize.Height), GraphicsUnit.Pixel)
            g.Dispose()

            If AvailMem.AvailMem.checkAvailRam(_bitmap.Width * _bitmap.Height * 4L) Then
                Me.pictureBox1.Image.Dispose()
                Me.pictureBox1.Image = DirectCast(_bitmap.Clone(), Bitmap)
                HLSPic()
            End If
            _tracking = False
        End If
    End Sub

    Private Sub TrackBar1_ValueChanged(sender As System.Object, e As System.EventArgs) Handles TrackBar1.ValueChanged
        TextBox1.Text = DirectCast(sender, TrackBar).Value.ToString()
        _hue = TrackBar1.Value
        HLSPic()
    End Sub

    Private Sub TextBox1_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBox1.TextChanged
        If IsNumeric(DirectCast(sender, TextBox).Text) Then
            Dim i As Integer = -1
            Int32.TryParse(DirectCast(sender, TextBox).Text, i)
            If i >= 0 AndAlso i <= 360 Then
                TrackBar1.Value = Convert.ToInt32(DirectCast(sender, TextBox).Text)
            End If
        End If
    End Sub

    Private Sub TrackBar2_ValueChanged(sender As System.Object, e As System.EventArgs) Handles trackBar2.ValueChanged
        textBox2.Text = DirectCast(sender, TrackBar).Value.ToString()
        _saturation = trackBar2.Value / 100.0F
        HLSPic()
    End Sub

    Private Sub TextBox2_TextChanged(sender As System.Object, e As System.EventArgs) Handles textBox2.TextChanged
        If IsNumeric(DirectCast(sender, TextBox).Text) Then
            Dim i As Integer = -101
            Int32.TryParse(DirectCast(sender, TextBox).Text, i)
            If i >= -100 AndAlso i <= 100 Then
                trackBar2.Value = Convert.ToInt32(DirectCast(sender, TextBox).Text)
            End If
        End If
    End Sub

    Private Sub TrackBar3_ValueChanged(sender As System.Object, e As System.EventArgs) Handles trackBar3.ValueChanged
        textBox3.Text = DirectCast(sender, TrackBar).Value.ToString()
        _luminance = trackBar3.Value / 100.0F
        HLSPic()
    End Sub

    Private Sub TextBox3_TextChanged(sender As System.Object, e As System.EventArgs) Handles textBox3.TextChanged
        If IsNumeric(DirectCast(sender, TextBox).Text) Then
            Dim i As Integer = -101
            Int32.TryParse(DirectCast(sender, TextBox).Text, i)
            If i >= -100 AndAlso i <= 100 Then
                trackBar3.Value = Convert.ToInt32(DirectCast(sender, TextBox).Text)
            End If
        End If
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex = 0 Then
            Me.trackBar2.Minimum = -100
        Else
            If Me.trackBar2.Value < 0 Then
                Me.trackBar2.Value = 0
            End If
            Me.trackBar2.Minimum = 0
        End If

        If _bloaded Then
            HLSPic()
        End If
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBox2.SelectedIndexChanged
        If ComboBox2.SelectedIndex = 0 Then
            Me.trackBar3.Minimum = -100
        Else
            If Me.trackBar3.Value < 0 Then
                Me.trackBar3.Value = 0
            End If
            Me.trackBar3.Minimum = 0
        End If
        If _bloaded Then
            HLSPic()
        End If
    End Sub

    Private Sub frmHLSBereich_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        _bloaded = True
    End Sub

    Private Sub TextBox1_Validating(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles TextBox1.Validating
        Dim tb As TextBox = DirectCast(sender, TextBox)
        If Not IsNumeric(tb.Text) Then
            tb.Text = "0"
        End If
        If Int32.Parse(tb.Text) > 360 Then
            tb.Text = "360"
        End If
        If Int32.Parse(tb.Text) < 0 Then
            tb.Text = "0"
        End If
    End Sub

    Private Sub textBox2_Validating(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles textBox3.Validating, textBox2.Validating
        Dim tb As TextBox = DirectCast(sender, TextBox)
        If Not IsNumeric(tb.Text) Then
            tb.Text = "0"
        End If
        If Int32.Parse(tb.Text) > 100 Then
            tb.Text = "100"
        End If
        If Int32.Parse(tb.Text) < -100 Then
            tb.Text = "-100"
        End If
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        'andere Hintergrundfarbe anzeigen
        If keyData = (Keys.B Or Keys.Control) Then
            Dim f As Color = Me.BackColor

            Me.BackColor = _farb

            For Each ct As Control In Me.Controls
                ct.BackColor = _farb
            Next

            _farb = f

            Return True
        End If

        'wenn Bild nur wenig opaq, nicht transparenten Bereich "suchen"
        If keyData = Keys.F2 Then
            If CheckBox1.Checked Then
                Dim pt As Point = fipbmp.ScrollToPic(_bSrc)
                _left = pt.X
                _top = pt.Y

                If (_bSrc.Width > pictureBox1.Width Or _bSrc.Height > pictureBox1.Height) Then
                    If AvailMem.AvailMem.checkAvailRam(Me.pictureBox1.ClientSize.Width * Me.pictureBox1.ClientSize.Height * 4L) Then
                        _bitmap = New Bitmap(Me.pictureBox1.ClientSize.Width, Me.pictureBox1.ClientSize.Height)
                    Else
                        Return True
                    End If
                    Dim g As Graphics = Graphics.FromImage(_bitmap)
                    g.DrawImage(_bSrc, New Rectangle(0, 0, _bitmap.Width, _bitmap.Height), New Rectangle(_left, _top, Me.pictureBox1.ClientSize.Width, Me.pictureBox1.ClientSize.Height), GraphicsUnit.Pixel)
                    g.Dispose()
                    If AvailMem.AvailMem.checkAvailRam(_bitmap.Width * _bitmap.Height * 4L) Then
                        If (Me.pictureBox1.Image IsNot Nothing) Then
                            Me.pictureBox1.Image.Dispose()
                        End If
                        Me.pictureBox1.Image = DirectCast(_bitmap.Clone(), Bitmap)
                    End If
                End If
            End If

            Return True
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub TextBox4_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles TextBox4.Validating
        Dim tb As TextBox = DirectCast(sender, TextBox)
        If Not IsNumeric(tb.Text) Then
            tb.Text = "0"
        End If
        If Int32.Parse(tb.Text) > 255 Then
            tb.Text = "255"
        End If
        If Int32.Parse(tb.Text) < -255 Then
            tb.Text = "-255"
        End If
    End Sub

    Private Sub TrackBar4_ValueChanged(sender As Object, e As EventArgs) Handles TrackBar4.ValueChanged
        TextBox4.Text = DirectCast(sender, TrackBar).Value.ToString()
        _alpha = TrackBar4.Value
        HLSPic()
    End Sub

    Private Sub ComboBox4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox4.SelectedIndexChanged
        If ComboBox4.SelectedIndex = 0 Then
            Me.TrackBar4.Minimum = -255
        Else
            If Me.TrackBar4.Value < 0 Then
                Me.TrackBar4.Value = 0
            End If
            Me.TrackBar4.Minimum = 0
        End If
        If _bloaded Then
            HLSPic()
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        Me.Label12.Enabled = Me.CheckBox2.Checked
        Me.TrackBar4.Enabled = Me.CheckBox2.Checked
        Me.TextBox4.Enabled = Me.CheckBox2.Checked
        Me.ComboBox4.Enabled = Me.CheckBox2.Checked
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If _bloaded Then
            HLSPic()
        End If
    End Sub

    Private Sub NumericUpDown7_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown7.ValueChanged
        If _bloaded Then
            HLSPic()
        End If
    End Sub

    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        Me.ColorDialog1.CustomColors = static_Button6_Click_CustomColors
        If Me.ColorDialog1.ShowDialog() = DialogResult.OK Then
            Me.panel1.BackColor = Me.ColorDialog1.Color
            static_Button6_Click_CustomColors = Me.ColorDialog1.CustomColors
        End If
    End Sub
End Class