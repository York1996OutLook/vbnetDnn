Imports System.IO
Imports Microsoft.Office.Interop
Imports System.Runtime.Serialization.Formatters.Binary
Public Class DataManager
    ''' <summary>
    ''' 序列化到文件
    ''' </summary>
    ''' <param name="dnn">要序列化的神经网络对象</param>
    ''' <param name="num">要保存的文件名称的前缀</param>
    Shared Sub SerializeToFile(dnn As DeepNeuralNetWork, num As String)

        Dim binFormatter As New BinaryFormatter()


        Dim fs As New FileStream(Application.StartupPath & "\" & num & ".bin", FileMode.OpenOrCreate)
        binFormatter.Serialize(fs, dnn)

        fs.Flush()

        fs.Dispose()
    End Sub
    Shared Function deserializeFromFile() As DeepNeuralNetWork
        Dim mStream As New MemoryStream
        Dim fs As New FileStream(Application.StartupPath & "\" & 1 & ".bin", FileMode.Open, FileAccess.Read)
        Dim binFormatter As New BinaryFormatter
        Dim resultDnn As DeepNeuralNetWork = CType(binFormatter.Deserialize(fs), DeepNeuralNetWork)
        Return resultDnn
    End Function
    Shared Function readDateFromXls(xlsPath As String) As DnnData
        '格式是前5列是输入的x，最后一列是预计输出的y
        '假设输出的数字大小是位于0到9之间的 
        Dim xlsApp As New Excel.Application
        Dim xlsWorkbook As Excel.Workbook = xlsApp.Workbooks.Open(xlsPath)
        Dim trainSheet As Excel.Worksheet = xlsWorkbook.Worksheets("train")
        Dim testSheet As Excel.Worksheet = xlsWorkbook.Worksheets("test")
        Dim trainArr = trainSheet.UsedRange.Value
        Dim testArr = testSheet.UsedRange.Value
        Dim trainRowsCount As Integer = trainSheet.UsedRange.Rows.Count
        Dim trainColsCount As Integer = trainSheet.UsedRange.Columns.Count
        Dim testRowsCount As Integer = testSheet.UsedRange.Rows.Count
        Dim testColsCount As Integer = testSheet.UsedRange.Columns.Count
        xlsWorkbook.Save()
        xlsWorkbook.Close()
        xlsApp.Quit()
        xlsApp = Nothing
        Dim trainXMatrix As New Matrix(trainRowsCount, 5, False)
        Dim trainYMatrix As New Matrix(trainRowsCount, 10, False)
        Dim testXMatrix As New Matrix(testRowsCount, 5, False)
        Dim testYMatrix As New Matrix(testRowsCount, 10, False)

        For i As Integer = 0 To trainRowsCount - 1
            For j As Integer = 0 To 4
                trainXMatrix.array2D(i, j) = trainArr(i + 1, j + 1)
            Next
            trainYMatrix.array2D(i, trainArr(i + 1, 6)) = 1
        Next
        For i As Integer = 0 To testRowsCount - 1
            For j As Integer = 0 To 4
                testXMatrix.array2D(i, j) = testArr(i + 1, j + 1)
            Next
            testYMatrix.array2D(i, testArr(i + 1, 6)) = 1
        Next
        Dim resultData As New DnnData
        resultData.trainXMatrix = trainXMatrix
        resultData.trainYMatrix = trainYMatrix
        resultData.testXMatrix = testXMatrix
        resultData.testYMatrix = testYMatrix
        Return resultData
    End Function
    ''' <summary>
    ''' ，，
    ''' </summary>
    ''' <param name="pixelFile">文件的路径</param>
    ''' <param name="labelFile">标签的路径</param>
    ''' <param name="numImages">图片的数量</param>
    ''' <returns>返回一个图片数组</returns>
    Shared Function LoadData(ByVal pixelFile As String, ByVal labelFile As String, numImages As Integer) As DigitImage()
        '图片的大小是28*28的
        Dim result() As DigitImage = New DigitImage((numImages) - 1) {}
        Dim pixels(27, 27) As Single

        Dim ifsPixels As FileStream = New FileStream(pixelFile, FileMode.Open)
        Dim ifsLabels As FileStream = New FileStream(labelFile, FileMode.Open)
        Dim brImages As BinaryReader = New BinaryReader(ifsPixels)
        Dim brLabels As BinaryReader = New BinaryReader(ifsLabels)
        Dim magic1 As Integer = brImages.ReadInt32
        ' stored as big endian
        magic1 = ReverseBytes(magic1)
        ' convert to Intel format
        Dim imageCount As Integer = brImages.ReadInt32
        imageCount = ReverseBytes(imageCount)
        Dim numRows As Integer = brImages.ReadInt32
        numRows = ReverseBytes(numRows)
        Dim numCols As Integer = brImages.ReadInt32
        numCols = ReverseBytes(numCols)
        Dim magic2 As Integer = brLabels.ReadInt32
        magic2 = ReverseBytes(magic2)
        Dim numLabels As Integer = brLabels.ReadInt32
        numLabels = ReverseBytes(numLabels)
        Dim di As Integer = 0
        Do While (di < numImages)
            Dim i As Integer = 0
            Do While (i < 28)
                Dim j As Integer = 0
                Do While (j < 28)
                    Dim b As Byte = brImages.ReadByte

                    pixels(i, j) = b
                    j = (j + 1)
                Loop
                i = (i + 1)
            Loop

            Dim lbl As Byte = brLabels.ReadByte
            ' get the label
            Dim dImage As DigitImage = New DigitImage(28, 28, pixels, lbl)
            result(di) = dImage
            di = (di + 1)
        Loop

        ' Each image
        ifsPixels.Close()
        brImages.Close()
        ifsLabels.Close()
        brLabels.Close()
        Return result
    End Function
    Shared Function readMnistSet() As DnnData
        '加载mnisi数据集
        Dim trainImagePath As String = "E:\毕业设计用\数据集\train-images.idx3-ubyte"
        Dim trainLabelPath As String = "E:\毕业设计用\数据集\train-labels.idx1-ubyte"
        Dim testImagePath As String = "E:\毕业设计用\数据集\testImage.idx3-ubyte"
        Dim testLabelPath As String = "E:\毕业设计用\数据集\testLabel.idx1-ubyte"

        Dim trainImage() As DigitImage = LoadData(trainImagePath, trainLabelPath, 60000)
        Dim testImage() As DigitImage = LoadData(testImagePath, testLabelPath, 10000)
        Dim resultDnnData As New DnnData
        resultDnnData.trainXMatrix = Calculate.getXMatrixFromDigitImageArr(trainImage, 28, 28)
        resultDnnData.trainYMatrix = Calculate.getYMatrixFromDigitImageArr(trainImage)
        resultDnnData.testXMatrix = Calculate.getXMatrixFromDigitImageArr(testImage, 28, 28)
        resultDnnData.testYMatrix = Calculate.getYMatrixFromDigitImageArr(testImage)
        Return resultDnnData
    End Function
    Public Shared Function ReverseBytes(ByVal v As Integer) As Integer
        Dim intAsBytes() As Byte = BitConverter.GetBytes(v)
        Array.Reverse(intAsBytes)
        Return BitConverter.ToInt32(intAsBytes, 0)
    End Function


End Class
Public Class DnnData
    Private _trainXMatrix As Matrix
    Public Property trainXMatrix() As Matrix
        Get
            Return _trainXMatrix
        End Get
        Set(ByVal value As Matrix)
            _trainXMatrix = value
        End Set
    End Property
    Private _trainYMatrix As Matrix
    Public Property trainYMatrix() As Matrix
        Get
            Return _trainYMatrix
        End Get
        Set(ByVal value As Matrix)
            _trainYMatrix = value
        End Set
    End Property

    Private _testXMatrix As Matrix
    Public Property testXMatrix() As Matrix
        Get
            Return _testXMatrix
        End Get
        Set(ByVal value As Matrix)
            _testXMatrix = value
        End Set
    End Property
    Private _testYMatrix As Matrix
    Public Property testYMatrix() As Matrix
        Get
            Return _testYMatrix
        End Get
        Set(ByVal value As Matrix)
            _testYMatrix = value
        End Set
    End Property
    Sub New()

    End Sub
End Class
Public Class DigitImage

    Public width As Integer

    ' 28
    Public height As Integer

    ' 28
    Public pixels(27, 27) As Single

    ' 0(white) - 255(black)
    Public label As Byte

    ' '0' - '9'
    ''' <summary>
    ''' 构造函数
    ''' </summary>
    ''' <param name="width">宽度</param>
    ''' <param name="height">高度</param>
    ''' <param name="pixels">数组</param>
    ''' <param name="label">标签</param>
    Public Sub New(ByVal width As Integer, ByVal height As Integer, ByVal pixels(,) As Single, ByVal label As Byte)

        Me.width = width
        Me.height = height
        Dim i As Integer = 0
        Do While (i < Me.height)
            Dim j As Integer = 0
            Do While (j < Me.width)
                Me.pixels(i, j) = pixels(i, j)
                j = j + 1
            Loop
            i = i + 1
        Loop
        Me.label = label
    End Sub
End Class



