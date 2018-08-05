Imports System.Drawing.Drawing2D
Imports System.Math

Public Class Form1
    Dim dnn As DeepNeuralNetWork
    Dim dnnData As DnnData
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
#Region "测试xls数据"
        'dnnData = DataManager.readDateFromXls(Application.StartupPath & "/test.xls")

        'dnn = New DeepNeuralNetWork(5, New Vector(Of Integer)(20)， 10, 0.1, DeepNeuralNetWork.CostFunctionTypes.crossEntropCostFunction)
        'dnn.SGD(dnnData, 10, 200)
        'PictureBox1.Image = dnn.getBitmap
        'PictureBox2.Image = dnn.getEvaluateImage

#End Region
#Region "测试手写数字"
        dnn = New DeepNeuralNetWork(28 * 28, New Vector(Of Integer)(31), 10, 0.05, DeepNeuralNetWork.CostFunctionTypes.crossEntropCostFunction)
        dnnData = DataManager.readMnistSet()
        dnn.SGD(dnnData, 10, 30)
        PictureBox1.Image = dnn.getBitmap
        PictureBox2.Image = dnn.getEvaluateImage

#End Region
#Region "读取本地的权重和偏置"
        'dnn = New DeepNeuralNetWork("1")
#End Region


    End Sub

    ''' <summary>
    ''' 逐个加载测试数据,并且画图
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        '
        Static i As Integer = 0
        i = i + 1

        dnn.feedForward(Calculate.matrixRowToVector(dnnData.testXMatrix, i))

        'showImage(dnnData.testXMatrix, i)
        PictureBox1.Image = dnn.getBitmap
    End Sub
    Sub showImage(matrix As Matrix, index As Integer)
        Dim bitmap As New Bitmap(28, 28)
        Dim g As Graphics = Graphics.FromImage(bitmap)
        g.Clear(Color.White)
        Dim pointer As Integer = 0
        For i As Integer = 0 To 27
            For j As Integer = 0 To 27
                Dim grayScale As Integer = matrix.array2D(index, pointer)
                pointer = pointer + 1
                g.FillRectangle(New SolidBrush(Color.FromArgb(grayScale * 255 Mod 256, grayScale * 255 Mod 256, 0)), New Rectangle(j, i, 1, 1))
            Next
        Next
        PictureBox2.Image = bitmap
    End Sub


End Class
