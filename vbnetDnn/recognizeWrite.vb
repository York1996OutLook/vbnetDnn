Public Class recognizeWrite
    Dim wi As wrtieImg
    Private Sub recognizeWrite_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim b As Bitmap = New Bitmap(280, 280)
        Me.isMouseDown = False
        wi = New wrtieImg
        dnn = New DeepNeuralNetWork(" ")
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        If Me.isMouseDown = False Then Exit Sub
        wi.pixels.array2D(Int((e.Y - 1) / 10), Int((e.X - 1) / 10)) = 255

        wi.draw()
        Me.PictureBox1.Image = wi.bitmap

    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        If e.Button = MouseButtons.Left Then
            Me.isMouseDown = False
        End If
    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown
        If e.Button = MouseButtons.Left Then
            Me.isMouseDown = True
        End If
    End Sub
    Private _isMouseDown As Boolean
    Public Property isMouseDown() As Boolean
        Get
            Return _isMouseDown
        End Get
        Set(ByVal value As Boolean)
            _isMouseDown = value
        End Set
    End Property

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        wi.clear()
        PictureBox1.Image = wi.bitmap
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim outputYVector As Vector(Of Single) = dnn.feedForward(Calculate.scalarMultiplicationVector((Calculate.matrixToVector(wi.pixels)), 1 / 256))
        Me.Label1.Text = outputYVector.getMaxIndex.ToString
    End Sub
    Private _dnn As DeepNeuralNetWork
    Public Property dnn() As DeepNeuralNetWork
        Get
            Return _dnn
        End Get
        Set(ByVal value As DeepNeuralNetWork)
            _dnn = value
        End Set
    End Property
End Class
Public Class wrtieImg
    Private _bitmap As Bitmap
    Public Property bitmap() As Bitmap
        Get
            Return _bitmap
        End Get
        Set(ByVal value As Bitmap)
            _bitmap = value
        End Set
    End Property

    Private _pixels As Matrix
    Public Property pixels() As Matrix
        Get
            Return _pixels
        End Get
        Set(ByVal value As Matrix)
            _pixels = value
        End Set
    End Property
    Sub New()
        Me.pixels = New Matrix(28, 28, False)
        Me.bitmap = New Bitmap(280, 280)
    End Sub
    Sub draw()
        Dim g As Graphics = Graphics.FromImage(Me.bitmap)
        g.Clear(Color.White)
        For i As Integer = 0 To Me.pixels.rowsCount - 1 'i代表行
            For j As Integer = 0 To Me.pixels.colunmsCount - 1
                If Me.pixels.array2D(i, j) = 0 Then Continue For
                Dim rect As New Rectangle(j * 10, i * 10, 10, 10)
                g.FillRectangle(Brushes.Black, rect)
            Next
        Next

    End Sub
    Sub clear()
        Me.pixels = New Matrix(28, 28, False)
        draw()
    End Sub
End Class