Option Strict On

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports HelplineRulerControl
Imports SegmentsListLib

Public Class frmColorCurves
    Inherits System.Windows.Forms.Form

    Private alphaMappings(255) As Integer
    Private ptsalpha As Point() = New Point() {New Point(0, 0), New Point(255, 255)}
    Dim ptsalpha4 As Point() = New Point() {New Point(0, 0), New Point(255, 255)}
    Dim ptsalpha2 As Point() = New Point() {New Point(0, 0), New Point(255, 255)}

    Dim _alphas As Boolean
    Dim _alphap As Integer

    Private _bitmap As Bitmap = Nothing
    Private _bSrc As Bitmap = Nothing

    Private _tracking As Boolean = False
    Private _tracking2 As Boolean = False

    Private _loaded As Boolean = False

    Private _left As Integer = 0
    Private _top As Integer = 0
    Private _x As Integer = 0
    Private _y As Integer = 0

    Private _ix As Integer = 0
    Private _iy As Integer = 0
    Private _bgPic As Bitmap

    Private _zoom As Single = 1.0F
    Private _dontSetValues As Boolean

    Public Sub New(ByVal imgIn As Bitmap, ByVal Farbe As String, ByVal Tolerance As Byte)
        MyBase.New()

        Me.InitializeComponent()

        If AvailMem.AvailMem.checkAvailRam(imgIn.Width * imgIn.Height * 8L) Then
            _bitmap = makeBitmap(imgIn)
            _bSrc = CType(imgIn.Clone, Bitmap)
        Else
            MessageBox.Show("Not enough Memory")
            Return
        End If

        Me.btnGetBmp.Visible = False

        pictureBox1.Image = CType(_bitmap.Clone, Bitmap)

        display(Me.ListBox4, ptsalpha)

        Me.ComboBox2.SelectedItem = Me.ComboBox2.Items(0)
        button10.Enabled = False

        Me.TextBox1.Text = Farbe
        Me.NumericUpDown10.Value = CType(Tolerance, Decimal)
    End Sub

    Public Property FBitmap() As Bitmap
        Get
            Return _bitmap
        End Get

        Set(ByVal value As Bitmap)
            _bitmap = value
        End Set
    End Property

    Public ReadOnly Property MappingsAlpha() As Integer()
        Get
            Return alphaMappings
        End Get
    End Property

    Public Property alphaPoints2() As Point()
        Get
            Return ptsalpha
        End Get

        Set(ByVal value As Point())
            ptsalpha4 = value
        End Set
    End Property

    Private Sub Panel3_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles panel3.Paint
        Using _fPen As New Pen(SystemColors.ControlText, 1)
            Dim mx As Matrix = New Matrix(1, 0, 0, -1, 0, 304)
            e.Graphics.Transform = mx

            e.Graphics.DrawLine(_fPen, 30, 50, 285, 50)
            e.Graphics.DrawLine(_fPen, 30, 50, 30, 305)

            Dim x As Integer

            For x = 0 To 5
                e.Graphics.DrawLine(_fPen, 25, 50 + (50 * x), 30, 50 + (50 * x))
                e.Graphics.DrawLine(_fPen, 30 + (50 * x), 45, 30 + (50 * x), 50)
            Next

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

            For x = 1 To 9
                Dim fpen As Pen = New Pen(Color.Black, 1)
                fpen.DashStyle = DashStyle.Custom
                fpen.DashPattern = New Single() {1, 4}
                e.Graphics.DrawLine(fpen, 30, 50 + (50 * x), 30 + (50 * x), 50)
                fpen.Dispose()
            Next

            Dim gs As GraphicsState = e.Graphics.Save()

            Dim mx1 As Matrix = mx.Clone()
            mx1.Translate(30, 50)
            e.Graphics.Transform = mx1

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

            Dim b As Integer = 0

            If ptsalpha.Length > 0 Then
                e.Graphics.DrawLine(Pens.DarkGray, 0, ptsalpha(0).Y, ptsalpha(0).X, ptsalpha(0).Y)
            End If
            If ptsalpha.Length > 1 Then
                e.Graphics.DrawCurve(Pens.Black, ptsalpha, CType(NumericUpDown11.Value, Single))
            Else
                b += 8
            End If
            If ptsalpha.Length > 0 Then
                e.Graphics.DrawLine(Pens.DarkGray, ptsalpha(ptsalpha.Length - 1).X, ptsalpha(ptsalpha.Length - 1).Y, 255, ptsalpha(ptsalpha.Length - 1).Y)
            End If

            For Each ptf As Point In ptsalpha
                e.Graphics.DrawRectangle(Pens.Black, ptf.X - 2, ptf.Y - 2, 4, 4)
            Next

            e.Graphics.Restore(gs)

            e.Graphics.Transform = New Matrix(1, 0, 0, 1, 40, 0)

            Dim ffont As Font = New Font("Arial", 12)

            If (b And 8) = 8 Then
                e.Graphics.DrawString("Zuwenig Punkte für Luminace", ffont, Brushes.White, 0, 48)
            End If

            ffont.Dispose()

        End Using
    End Sub

    Private Sub sortArray(ByVal a As Point())
        Array.Sort(a, New PointsComparer())
    End Sub

    Private Sub display(ByVal lb As ListBox, ByVal a As Point())
        lb.Items.Clear()

        Dim i As Integer = 0

        For i = 0 To a.Length - 1
            lb.Items.Add(a(i))
        Next
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button9.Click
        If CheckBox2.Checked Then
            ReDim ptsalpha2(ptsalpha.Length - 1)
            ptsalpha.CopyTo(ptsalpha2, 0)
            sortArray(ptsalpha2)
        End If

        display(Me.ListBox4, ptsalpha)

        panel3.Invalidate()

        DrawPic()
    End Sub

    Private Sub DrawPic()
        Dim bmp2 As Bitmap = Nothing
        If AvailMem.AvailMem.checkAvailRam(_bitmap.Width * _bitmap.Height * 4L) Then
            bmp2 = CType(_bitmap.Clone, Bitmap)
        Else
            Return
        End If
        Dim fPath As GraphicsPath = New GraphicsPath()

        Dim cuSgmt As New CurveSegment

        Gamma_reset_alpha()

        If RadioButton1.Checked Then
            If ptsalpha.Length > 0 Then
                fPath.AddLine(0, ptsalpha(0).Y, ptsalpha(0).X, ptsalpha(0).Y)
                If ptsalpha.Length > 1 Then
                    fPath.AddCurve(ptsalpha, CType(NumericUpDown11.Value, Single))
                End If
                fPath.AddLine(ptsalpha(ptsalpha.Length - 1).X, ptsalpha(ptsalpha.Length - 1).Y, 255, ptsalpha(ptsalpha.Length - 1).Y)
            Else
                If RadioButton1.Checked Then
                    fPath.AddLine(0, 0, 255, 255)
                Else
                    fPath.AddLine(0, 255, 255, 255)
                End If
            End If
        Else

        End If

        fPath.Reset()

        Dim bz As List(Of BezierSegment) = cuSgmt.CalcBezierSegments(ptsalpha, CType(NumericUpDown11.Value, Single))
        Dim pts As List(Of PointF) = cuSgmt.GetAllPoints(bz, 256, 0, 255)
        cuSgmt.MapPoints(pts, alphaMappings)

        If Me.RadioButton1.Checked Then
            fipbmp.gradColors(bmp2, alphaMappings, Me.CheckBox1.Checked)
        Else
            fipbmp.gradLumToAlpha(bmp2, alphaMappings, Me.CheckBox4.Checked, Me.CheckBox1.Checked)
        End If

        Dim bmpOut2 As Bitmap = Nothing

        If AvailMem.AvailMem.checkAvailRam(bmp2.Width * bmp2.Height * 4L) Then
            bmpOut2 = CType(bmp2.Clone(), Bitmap)
        End If

        bmp2.Dispose()
        If Not pictureBox1.Image Is Nothing Then
            pictureBox1.Image.Dispose()
        End If
        pictureBox1.Image = bmpOut2


        button10.Enabled = True

    End Sub

    Private Function makeBitmap(ByVal bmp As Bitmap) As Bitmap
        Dim faktor As Double = panel2.Width / panel2.Height
        Dim multiplier As Double = bmp.Width / bmp.Height
        Dim zoom As Single = 0.0F

        If multiplier >= faktor Then
            zoom = CType(panel2.ClientRectangle.Width / bmp.Width, Single)
        Else
            zoom = CType(panel2.ClientRectangle.Height / bmp.Height, Single)
        End If

        Dim bitmap As Bitmap = Nothing
        Dim w2 As Integer = CType(bmp.Width * zoom, Integer)
        Dim h2 As Integer = CType(bmp.Height * zoom, Integer)
        If AvailMem.AvailMem.checkAvailRam(w2 * h2 * 4L) Then
            bitmap = New Bitmap(w2, h2)
        Else
            Return Nothing
        End If
        Dim gx As Graphics = Graphics.FromImage(bitmap)
        gx.SmoothingMode = SmoothingMode.AntiAlias
        gx.InterpolationMode = InterpolationMode.HighQualityBicubic
        'gx.InterpolationMode = InterpolationMode.HighQualityBilinear
        gx.DrawImage(bmp, 0, 0, CType(bmp.Width * zoom, Integer), CType(bmp.Height * zoom, Integer))
        gx.Dispose()

        Return bitmap
    End Function

    Private Sub Gamma_reset_alpha()
        For i As Integer = 0 To 255
            alphaMappings(i) = 0
        Next
    End Sub

    Private Sub panel3_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles panel3.MouseMove
        Dim ix As Integer = e.X - 30
        Dim iy As Integer = 254 - (e.Y)

        ix = Math.Max(Math.Min(ix, 255), 0)
        iy = Math.Max(Math.Min(iy, 255), 0)

        Me.ToolStripStatusLabel1.Text = ix.ToString & ";" & iy.ToString

        If _tracking = True Then
            'If (ix >= 0) And (ix <= 255) And (iy >= 0) And (iy <= 255) Then
            If _alphas Then
                Me.ptsalpha(_alphap) = New Point(ix, iy)
            End If

            If CheckBox2.Checked Then
                ReDim ptsalpha2(ptsalpha.Length - 1)
                ptsalpha.CopyTo(ptsalpha2, 0)
                sortArray(ptsalpha2)
            End If

            display(Me.ListBox4, ptsalpha)

            panel3.Invalidate()

            If Timer1.Enabled Then
                Timer1.Stop()
            End If

            Me.Timer1.Start()
            'End If
        End If

    End Sub

    Private Sub panel3_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles panel3.MouseUp
        Dim ix As Integer = e.X - 30
        Dim iy As Integer = 254 - (e.Y)

        ix = Math.Max(Math.Min(ix, 255), 0)
        iy = Math.Max(Math.Min(iy, 255), 0)

        Dim pt As Point = New Point(ix, iy)

        If e.Button = MouseButtons.Right Then
            'If (pt.X >= 0) And (pt.X <= 255) And (pt.Y >= 0) And (pt.Y <= 255) Then
            Dim i As Integer = 0
            Dim f As Boolean = False

            For i = 0 To Me.ptsalpha.Length - 1
                If Me.ptsalpha(i).X = pt.X Then
                    f = True
                    Me.ptsalpha(i) = pt
                    Exit For
                End If
            Next

            If f = False Then
                ReDim Preserve ptsalpha(ptsalpha.Length)
                ptsalpha(ptsalpha.Length - 1) = pt
                sortArray(ptsalpha)
                display(Me.ListBox4, ptsalpha)
            End If


            Button9_Click(button9, EventArgs.Empty)
        End If
        'End If

        _tracking = False

        Me.panel3.Capture = False
    End Sub

    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        Gamma_reset_alpha()
        If RadioButton1.Checked Then
            ptsalpha = New Point() {New Point(0, 0), New Point(255, 255)}
        Else
            ptsalpha = New Point() {New Point(0, 255), New Point(255, 255)}
        End If

        Button9_Click(button9, EventArgs.Empty)
    End Sub

    Private Sub panel3_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles panel3.MouseDown
        Dim ix As Integer = e.X - 30
        Dim iy As Integer = 254 - (e.Y)

        ix = Math.Max(Math.Min(ix, 255), 0)
        iy = Math.Max(Math.Min(iy, 255), 0)

        Dim pt As Point = New Point(ix, iy)
        _alphas = False

        If e.Button = MouseButtons.Left Then
            For i As Integer = 0 To Me.ListBox4.Items.Count - 1
                Dim ptf As Point = CType(Me.ListBox4.Items(i), Point)
                If (pt.X > (ptf.X - 4)) AndAlso (pt.X < (ptf.X + 4)) AndAlso
                    (pt.Y > (ptf.Y - 4)) AndAlso (pt.Y < (ptf.Y + 4)) Then
                    Me.ListBox4.SelectedItem = Me.ListBox4.Items(i)
                    _alphas = True
                    _alphap = i
                    _tracking = True
                End If
            Next
        End If


        If _tracking Then
            Me.panel3.Capture = True
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox2.CheckedChanged
        If _loaded Then
            Button9_Click(button9, EventArgs.Empty)
        End If
    End Sub

    Private Sub frmGradColors3_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        _loaded = True
    End Sub

    Private Sub numericUpDown7_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If _loaded Then
            Button9_Click(button9, EventArgs.Empty)
        End If
    End Sub

    Private Sub numericUpDown8_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If _loaded Then
            Button9_Click(button9, EventArgs.Empty)
        End If
    End Sub

    Private Sub numericUpDown9_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If _loaded Then
            Button9_Click(button9, EventArgs.Empty)
        End If
    End Sub

    Private Sub CheckBoxPic_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBoxPic.CheckedChanged
        If CheckBoxPic.Checked Then
            Me.pictureBox1.SizeMode = PictureBoxSizeMode.Normal
            Me.pictureBox1.Location = New Point(0, 0)
            Me.pictureBox1.Size = New Size(Me.panel2.Width, Me.panel2.Height)
            Me.pictureBox1.Cursor = Cursors.Hand

            _left = 0
            _top = 0

            AddHandler pictureBox1.MouseDown, AddressOf pictureBox1_MouseDown
            AddHandler pictureBox1.MouseMove, AddressOf pictureBox1_MouseMove_2
            AddHandler pictureBox1.MouseUp, AddressOf pictureBox1_MouseUp

            If Not IsNothing(_bitmap) Then
                _bitmap.Dispose()
            End If

            If AvailMem.AvailMem.checkAvailRam(Me.pictureBox1.ClientSize.Width * Me.pictureBox1.ClientSize.Height * 4L) Then
                _bitmap = New Bitmap(Me.pictureBox1.ClientSize.Width, Me.pictureBox1.ClientSize.Height)
            Else
                Return
            End If
            Dim g As Graphics = Graphics.FromImage(_bitmap)
            g.DrawImage(_bSrc, New Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
            New Rectangle(_left, _top, Me.pictureBox1.ClientSize.Width, Me.pictureBox1.ClientSize.Height), GraphicsUnit.Pixel)
            g.Dispose()
        Else
            Me.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize
            Me.pictureBox1.Cursor = Cursors.Default

            RemoveHandler pictureBox1.MouseDown, AddressOf pictureBox1_MouseDown
            RemoveHandler pictureBox1.MouseMove, AddressOf pictureBox1_MouseMove_2
            RemoveHandler pictureBox1.MouseUp, AddressOf pictureBox1_MouseUp

            If Not IsNothing(_bitmap) AndAlso _bitmap.Equals(_bSrc) = False Then
                _bitmap.Dispose()
            End If
            _bitmap = New Bitmap(makeBitmap(_bSrc))
        End If

        If Not IsNothing(Me.pictureBox1.Image) Then
            Me.pictureBox1.Image.Dispose()
        End If
        If AvailMem.AvailMem.checkAvailRam(_bitmap.Width * _bitmap.Height * 4L) Then
            Me.pictureBox1.Image = CType(_bitmap.Clone, Bitmap)

            If Me._bgPic IsNot Nothing Then
                Dim pOld As Image = Me.pictureBox1.BackgroundImage
                Me.pictureBox1.BackgroundImage = MakeBGImage()
                If pOld IsNot Nothing Then
                    pOld.Dispose()
                    pOld = Nothing
                End If
            End If

            Button9_Click(button9, EventArgs.Empty)
        End If
    End Sub

    Private Sub pictureBox1_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            _x = e.X
            _y = e.Y
            _tracking2 = True
        End If
    End Sub

    Private Sub pictureBox1_MouseMove_2(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If _tracking2 Then
            _left = Math.Min(Math.Max(_left + (_x - e.X), 0), _bSrc.Width - Me.pictureBox1.ClientSize.Width)
            _top = Math.Min(Math.Max(_top + (_y - e.Y), 0), _bSrc.Height - Me.pictureBox1.ClientSize.Height)
        End If
    End Sub

    Private Sub pictureBox1_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If (_tracking2) AndAlso (_bSrc.Width > pictureBox1.Width Or _bSrc.Height > pictureBox1.Height) Then
            If AvailMem.AvailMem.checkAvailRam(Me.pictureBox1.ClientSize.Width * Me.pictureBox1.ClientSize.Height * 4L) Then
                If Not IsNothing(_bitmap) Then
                    _bitmap.Dispose()
                End If
                _bitmap = New Bitmap(Me.pictureBox1.ClientSize.Width, Me.pictureBox1.ClientSize.Height)
            Else
                Return
            End If
            Dim g As Graphics = Graphics.FromImage(_bitmap)
            g.DrawImage(_bSrc, New Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
            New Rectangle(_left, _top, Me.pictureBox1.ClientSize.Width, Me.pictureBox1.ClientSize.Height), GraphicsUnit.Pixel)
            g.Dispose()
            If Not IsNothing(Me.pictureBox1.Image) Then
                Me.pictureBox1.Image.Dispose()
            End If
            If AvailMem.AvailMem.checkAvailRam(_bitmap.Width * _bitmap.Height * 4L) Then
                Me.pictureBox1.Image = CType(_bitmap.Clone, Bitmap)
                If Me._bgPic IsNot Nothing Then
                    Dim pOld As Image = Me.pictureBox1.BackgroundImage
                    Me.pictureBox1.BackgroundImage = MakeBGImage()
                    If pOld IsNot Nothing Then
                        pOld.Dispose()
                        pOld = Nothing
                    End If
                End If
                Button9_Click(button9, EventArgs.Empty)
            End If
            _tracking2 = False
        End If
    End Sub

    Private Sub pictureBox1_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pictureBox1.MouseMove
        If _tracking = False Then
            If e.X >= 0 AndAlso e.X < _bitmap.Width AndAlso e.Y >= 0 AndAlso e.Y < _bitmap.Height Then
                Dim ix As Integer = e.X
                Dim iy As Integer = e.Y
                _ix = e.X
                _iy = e.Y

                Dim gfcolor As Color = _bitmap.GetPixel(ix, iy)
                Dim gfcolor2 As Color = CType(pictureBox1.Image, Bitmap).GetPixel(ix, iy)
                Me.ToolStripStatusLabel1.Text = "ARGB: " & gfcolor.A.ToString() & "; " & gfcolor.R.ToString() & "; " & gfcolor.G.ToString() & "; " & gfcolor.B.ToString() &
                    "; NEW: ARGB: " & gfcolor2.A.ToString() & "; " & gfcolor2.R.ToString() & "; " & gfcolor2.G.ToString() & "; " & gfcolor2.B.ToString()

                Me.ToolStripStatusLabel2.Text = "Lum: " & gfcolor.GetBrightness().ToString("N2") & "; Alpha: " & gfcolor.A.ToString() & "; NEW: Alpha: " & gfcolor2.A.ToString()
            End If
        End If
    End Sub

    Private Sub pictureBox1_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pictureBox1.DoubleClick
        If _tracking = False Then
            If _ix >= 0 AndAlso _ix < _bitmap.Width AndAlso _iy >= 0 AndAlso _iy < _bitmap.Height Then
                Dim gfcolor As Color = _bitmap.GetPixel(_ix, _iy)
                Me.TextBox1.Text = gfcolor.A.ToString() & ";" & gfcolor.R.ToString() & ";" & gfcolor.G.ToString() & ";" & gfcolor.B.ToString()
            End If
        End If
    End Sub

    Private Sub HandleKeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TextBox1.KeyPress
        If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> Char.Parse("-") AndAlso e.KeyChar <> Char.Parse(",") _
         AndAlso e.KeyChar <> Char.Parse(".") AndAlso Not Char.IsControl(e.KeyChar) AndAlso e.KeyChar <> Char.Parse(";") Then
            e.Handled = True
        End If
    End Sub

    Private Sub CheckBox3_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CheckBox3.CheckedChanged
        If _loaded Then
            Me.Button13.Enabled = Me.CheckBox3.Checked
            DrawPic()
        End If
    End Sub

    Private Sub NumericUpDown10_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles NumericUpDown10.ValueChanged
        If _loaded Then
            DrawPic()
        End If
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TextBox1.TextChanged
        If _loaded Then
            DrawPic()
        End If
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
        If keyData = Keys.F2 Then
            If CheckBoxPic.Checked Then
                Dim pt As Point = fipbmp.ScrollToPic(_bSrc)
                _left = pt.X
                _top = pt.Y

                If (_bSrc.Width > pictureBox1.Width Or _bSrc.Height > pictureBox1.Height) Then
                    If Not IsNothing(_bitmap) Then
                        _bitmap.Dispose()
                    End If
                    If AvailMem.AvailMem.checkAvailRam(Me.pictureBox1.ClientSize.Width * Me.pictureBox1.ClientSize.Height * 4L) Then
                        _bitmap = New Bitmap(Me.pictureBox1.ClientSize.Width, Me.pictureBox1.ClientSize.Height)
                    Else
                        Return True
                    End If
                    Dim g As Graphics = Graphics.FromImage(_bitmap)
                    g.DrawImage(_bSrc, New Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
                     New Rectangle(_left, _top, Me.pictureBox1.ClientSize.Width, Me.pictureBox1.ClientSize.Height), GraphicsUnit.Pixel)
                    g.Dispose()
                    If Not IsNothing(Me.pictureBox1.Image) Then
                        Me.pictureBox1.Image.Dispose()
                    End If
                    If AvailMem.AvailMem.checkAvailRam(_bitmap.Width * _bitmap.Height * 4L) Then
                        Me.pictureBox1.Image = CType(_bitmap.Clone, Bitmap)
                        If Me._bgPic IsNot Nothing Then
                            Dim pOld As Image = Me.pictureBox1.BackgroundImage
                            Me.pictureBox1.BackgroundImage = MakeBGImage()
                            If pOld IsNot Nothing Then
                                pOld.Dispose()
                                pOld = Nothing
                            End If
                        End If
                    End If
                End If
            End If

            Return True
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        Timer1.Stop()

        Button9_Click(button9, EventArgs.Empty)
    End Sub

    Private Sub Button8_Click_1(sender As System.Object, e As System.EventArgs) Handles Button8.Click
        Dim pt As Point = New Point(CType(Me.NumericUpDown13.Value, Integer), CType(Me.NumericUpDown12.Value, Integer))

        Dim i As Integer = 0
        Dim f As Boolean = False

        For i = 0 To Me.ListBox4.Items.Count - 1
            If CType(Me.ListBox4.Items(i), Point).X = pt.X Then
                f = True
                Exit For
            End If
        Next

        If f = False Then
            ReDim Preserve ptsalpha(ptsalpha.Length)
            ptsalpha(ptsalpha.Length - 1) = pt
            sortArray(ptsalpha)
            display(Me.ListBox4, ptsalpha)
        End If
        Button9_Click(button9, EventArgs.Empty)
    End Sub

    Private Sub Button7_Click(sender As System.Object, e As System.EventArgs) Handles Button7.Click
        Try
            Me.ListBox4.Items.Remove(Me.ListBox4.SelectedItem)
            ptsalpha = New Point() {}
            ReDim ptsalpha(Me.ListBox4.Items.Count - 1)

            Dim i As Integer = 0

            For i = 0 To Me.ListBox4.Items.Count - 1
                ptsalpha(i) = CType(Me.ListBox4.Items(i), Point)
            Next
        Catch

        End Try
        Button9_Click(button9, EventArgs.Empty)
    End Sub

    Private Sub NumericUpDown11_ValueChanged(sender As System.Object, e As System.EventArgs) Handles NumericUpDown11.ValueChanged
        If _loaded Then
            Button9_Click(button9, EventArgs.Empty)
        End If
    End Sub

    Function ProcessColors() As Bitmap
        Dim bWork As Bitmap = Nothing

        Try
            If AvailMem.AvailMem.checkAvailRam(Me._bSrc.Width * Me._bSrc.Height * 4L) Then
                bWork = CType(Me._bSrc.Clone, Bitmap)
            End If

            Dim aalpha(255) As Integer

            Array.Copy(MappingsAlpha, aalpha, MappingsAlpha.Length)
            fipbmp.gradColors(bWork, aalpha, Me.CheckBox1.Checked)
        Catch
            If Not bWork Is Nothing Then
                bWork.Dispose()
                bWork = Nothing
            End If
        End Try
        Return bWork
    End Function

    Private Sub CheckBox12_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox12.CheckedChanged
        If Me.CheckBox12.Checked Then
            Me.pictureBox1.BackColor = SystemColors.ControlDarkDark
        Else
            Me.pictureBox1.BackColor = SystemColors.Control
        End If
    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        If Me.ColorDialog1.ShowDialog() = DialogResult.OK Then
            Dim c As Color = Me.ColorDialog1.Color
            Me.TextBox1.Text = c.A & ";" & c.R & ";" & c.G & ";" & c.B
        End If
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        If _loaded Then
            Dim b As Boolean = True

            If MessageBox.Show("All current values will be ignored. Continue?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = DialogResult.Yes Then
                b = True
            Else
                b = False
            End If

            If b Then
                If Me.RadioButton1.Checked Then
                    ptsalpha = New Point() {New Point(0, 0), New Point(255, 255)}
                    ptsalpha4 = New Point() {New Point(0, 0), New Point(255, 255)}
                    ptsalpha2 = New Point() {New Point(0, 0), New Point(255, 255)}

                    Me.NumericUpDown10.Enabled = True
                    Me.Label10.Enabled = True
                    Me.TextBox1.Enabled = True
                    Me.Button13.Enabled = True
                    Me.CheckBox3.Enabled = True
                Else
                    ptsalpha = New Point() {New Point(0, 255), New Point(255, 255)}
                    ptsalpha4 = New Point() {New Point(0, 255), New Point(255, 255)}
                    ptsalpha2 = New Point() {New Point(0, 255), New Point(255, 255)}

                    Me.NumericUpDown10.Enabled = False
                    Me.Label10.Enabled = False
                    Me.TextBox1.Enabled = False
                    Me.Button13.Enabled = False
                    Me.CheckBox3.Enabled = False
                End If

                display(Me.ListBox4, ptsalpha)
                panel3.Invalidate()

                DrawPic()
            End If
        End If
    End Sub

    Private Sub ListBox4_DoubleClick(sender As Object, e As EventArgs) Handles ListBox4.DoubleClick
        Try
            Dim pt As Point = CType(CType(sender, ListBox).SelectedItem, Point)
            NumericUpDown13.Value = CType(pt.X, Decimal)
            NumericUpDown12.Value = CType(pt.Y, Decimal)
        Catch

        End Try
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If _loaded Then
            Button9_Click(button9, EventArgs.Empty)
        End If
    End Sub

    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        If _loaded Then
            Button9_Click(button9, EventArgs.Empty)
        End If
    End Sub

    Private Sub btnLoadBG_Click(sender As Object, e As EventArgs) Handles btnLoadBG.Click
        If Me.openFileDialog1.ShowDialog() = DialogResult.OK Then
            Dim bmp As Bitmap = Nothing
            Using img As Image = Image.FromFile(Me.openFileDialog1.FileName)
                bmp = New Bitmap(img)
            End Using
            Me.SetBitmap(Me._bgPic, bmp)

            Me._dontSetValues = True
            Me.numericUpDown1.Value = CDec(0)
            Me.numericUpDown2.Value = CDec(0)
            Me.numericUpDown3.Value = CDec(Me._bSrc.Width)
            Me.numericUpDown4.Value = CDec(Me._bSrc.Height)
            Me._dontSetValues = False

            If Me._bgPic IsNot Nothing Then
                Dim pOld As Image = Me.pictureBox1.BackgroundImage
                Me.pictureBox1.BackgroundImage = MakeBGImage()
                If pOld IsNot Nothing Then
                    pOld.Dispose()
                    pOld = Nothing
                End If
            End If
        End If
    End Sub

    Private Sub SetBitmap(ByRef bitmapToSet As Bitmap, ByRef bitmapToBeSet As Bitmap)
        Dim bOld As Bitmap = bitmapToSet

        bitmapToSet = bitmapToBeSet

        If bOld IsNot Nothing AndAlso bOld.Equals(bitmapToBeSet) = False Then
            bOld.Dispose()
        End If
    End Sub

    Private Sub frmColorCurves_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Me._bgPic IsNot Nothing Then
            Me._bgPic.Dispose()
        End If
    End Sub

    Private Function MakeBGImage() As Bitmap
        Dim res As Bitmap = Nothing
        Dim z As Single = 1.0F
        If CheckBoxPic.Checked Then
            res = New Bitmap(Me.pictureBox1.Width, Me.pictureBox1.Height)
        Else
            Dim r As Rectangle = GetImageRectangle()
            res = New Bitmap(r.Width, r.Height)

            Dim faktor As Double = System.Convert.ToDouble(r.Width) / System.Convert.ToDouble(r.Height)
            Dim multiplier As Double = System.Convert.ToDouble(Me._bSrc.Width) / System.Convert.ToDouble(Me._bSrc.Height)
            If (multiplier >= faktor) Then
                z = System.Convert.ToSingle(System.Convert.ToDouble(r.Width) / System.Convert.ToDouble(Me._bSrc.Width))
            Else
                z = System.Convert.ToSingle(System.Convert.ToDouble(r.Height) / System.Convert.ToDouble(Me._bSrc.Height))
            End If
        End If

        Using gx As Graphics = Graphics.FromImage(res)
            If CheckBoxPic.Checked Then
                gx.TranslateTransform(-Me.numericUpDown1.Value - _left, -Me.numericUpDown2.Value - _top)
            End If
            Dim w As Single = CSng(Me.numericUpDown3.Value) / _bSrc.Width
            Dim h As Single = CSng(Me.numericUpDown4.Value) / _bSrc.Height
            gx.ScaleTransform(1.0F / w * z, 1.0F / h * z)
            gx.DrawImage(Me._bgPic, 0, 0)
        End Using

        Return res
    End Function

    Private Function GetImageRectangle() As Rectangle
        Dim pboxType As Type = pictureBox1.GetType()
        Dim irProperty As System.Reflection.PropertyInfo = pboxType.GetProperty(
                    "ImageRectangle",
                    System.Reflection.BindingFlags.GetProperty Or System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance)

        Dim r As Rectangle = CType(irProperty.GetValue(pictureBox1, Nothing), Rectangle)

        Me._zoom = CSng(r.Width / Me.pictureBox1.Image.Width)

        Return r
    End Function

    Private Sub numericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles numericUpDown1.ValueChanged, numericUpDown2.ValueChanged, numericUpDown3.ValueChanged, numericUpDown4.ValueChanged
        If Not Me._dontSetValues AndAlso Me._bgPic IsNot Nothing Then
            Dim pOld As Image = Me.pictureBox1.BackgroundImage
            Me.pictureBox1.BackgroundImage = MakeBGImage()
            If pOld IsNot Nothing Then
                pOld.Dispose()
                pOld = Nothing
            End If
        End If

        Me.pictureBox1.Refresh()
    End Sub

    Private Sub btnOrigSize_Click(sender As Object, e As EventArgs) Handles btnOrigSize.Click
        Me.numericUpDown3.Value = CDec(Me._bSrc.Width)
        Me.numericUpDown4.Value = CDec(Me._bSrc.Height)
    End Sub

    Private Sub btnClearBG_Click(sender As Object, e As EventArgs) Handles btnClearBG.Click
        If Me._bgPic IsNot Nothing Then
            Me._bgPic.Dispose()
        End If

        Me._bgPic = Nothing

        If Me.pictureBox1.BackgroundImage IsNot Nothing Then
            Me.pictureBox1.BackgroundImage.Dispose()
        End If

        Me.pictureBox1.BackgroundImage = Nothing
    End Sub
End Class
