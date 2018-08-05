
<Serializable>
Public Class DeepNeuralNetWork
    Public Enum CostFunctionTypes
        QuadraticCostFunction = 0
        crossEntropCostFunction = 1
    End Enum
    ''' <summary>
    ''' 指的是Cost function的梯度
    ''' </summary>
    Private Structure delta
        Dim deltaWeights As deltaWeights
        Dim deltaBias As deltaBias
    End Structure
    Private Structure deltaWeights
        Dim hiddenLayerDeltaWeightsList As List(Of Matrix)
        Dim outputLayerDeltaWeightsMatrix As Matrix
    End Structure
    Private Structure deltaBias
        Dim hiddenLayerDeltaBiasList As List(Of Vector(Of Single))
        Dim outputLayerDeltaBiasVector As Vector(Of Single)
    End Structure

    Private _inputLayer_NeuralCount As Integer
    ''' <summary>
    ''' 输入层神经元的个数
    ''' </summary>
    ''' <returns></returns>
    Public Property inputLayer_NeuralCount() As Integer
        Get
            Return _inputLayer_NeuralCount
        End Get
        Set(ByVal value As Integer)
            _inputLayer_NeuralCount = value
        End Set
    End Property

    Private _hiddenLayer_NueralCountVector As Vector(Of Integer)
    ''' <summary>
    ''' 隐藏层神经元个数,用一个向量来存储起来
    ''' </summary>
    ''' <returns></returns>
    Public Property hiddenLayer_NueralCountVector() As Vector(Of Integer)
        Get
            Return _hiddenLayer_NueralCountVector
        End Get
        Set(ByVal value As Vector(Of Integer))
            _hiddenLayer_NueralCountVector = value
        End Set
    End Property
    Private _outputLayer_NeuralCount As Integer
    ''' <summary>
    ''' 输出层的神经元个数
    ''' </summary>
    ''' <returns></returns>
    Public Property outputLayer_NeuralCount() As Integer
        Get
            Return _outputLayer_NeuralCount
        End Get
        Set(ByVal value As Integer)
            _outputLayer_NeuralCount = value
        End Set
    End Property
    Private _studyRate As Single
    ''' <summary>
    ''' 学习率
    ''' </summary>
    ''' <returns></returns>
    Public Property studyRate() As Single
        Get
            Return _studyRate
        End Get
        Set(ByVal value As Single)
            _studyRate = value
        End Set
    End Property

    Private _costFunctionType As CostFunctionTypes
    Public Property costFunctionType() As CostFunctionTypes
        Get
            Return _costFunctionType
        End Get
        Set(ByVal value As CostFunctionTypes)
            _costFunctionType = value
        End Set
    End Property
    ''' <summary>
    ''' 构造函数
    ''' </summary>
    ''' <param name="m_inputLayer_NeuralCount">输入层神经元个数</param>
    ''' <param name="m_hiddenLayer_NueralCountVector">隐藏层神经元个数用向量表示,比如(5,6,7)代表三层隐藏层,长度分别是5,6,7</param>
    ''' <param name="m_outputLayer_NeuralCount">输出层神经元个数</param>
    ''' <param name="m_studyRate">学习率</param>

    Sub New(m_inputLayer_NeuralCount As Integer, m_hiddenLayer_NueralCountVector As Vector(Of Integer), m_outputLayer_NeuralCount As Integer， m_studyRate As Single, costType As CostFunctionTypes)
        If m_inputLayer_NeuralCount <= 0 Then Throw New Exception("输入层神经元数量不可以小于1")
        For i As Integer = 0 To m_hiddenLayer_NueralCountVector.length - 1
            If m_hiddenLayer_NueralCountVector(i) <= 0 Then
                Throw New Exception("隐藏层神经元个数不能小于1")
            End If
        Next
        If m_outputLayer_NeuralCount <= 0 Then Throw New Exception("输出层神经元数量不能小于1")
        Me.inputLayer_NeuralCount = m_inputLayer_NeuralCount
        Me.hiddenLayer_NueralCountVector = m_hiddenLayer_NueralCountVector
        Me.outputLayer_NeuralCount = m_outputLayer_NeuralCount
        Me.studyRate = m_studyRate
        Me.costFunctionType = CostFunctionTypes.crossEntropCostFunction
        initialWeightsAndBiasAndErrorsAndActivationsAndBitmap()

    End Sub
    ''' <summary>
    ''' 构造函数，用一个本地文件来初始化一个神经网络
    ''' 文本的格式是前面是bs,后面是ws中间有个@，一个数据一行
    ''' </summary>
    ''' <param name="wbTxt">训练好的网络的文本文件路径</param>
    Sub New(wbTxt As String)
        wbTxt = Strings.Trim(Strings.Replace(IO.File.ReadAllText(Application.StartupPath & "\wb.txt"), vbLf, ""))
        Dim tempSplit() As String = wbTxt.Split("@")
        Dim t() As String = Trim(tempSplit(0)).Split(vbCrLf)
        Dim bs(39) As Single
        For i As Integer = 0 To 39

            bs(i) = Single.Parse(t(i))
        Next

        Dim wtxt As String = tempSplit(1)
        Dim wStr() As String = wtxt.Split(vbCr)
        Dim ws(784 * 30 + 30 * 10 - 1) As Single
        For i As Integer = 0 To 23819
            ws(i) = Single.Parse(wStr(i))
        Next
        Me.inputLayer_NeuralCount = 784
        Me.hiddenLayer_NueralCountVector = New Vector(Of Integer)(30)
        Me.outputLayer_NeuralCount = 10
        Me.studyRate = 3.0
        initialWeightsAndBiasAndErrorsAndActivationsAndBitmap()

        Dim bVector As New Vector(Of Single)(30, False)
        For i As Integer = 0 To 29
            bVector(i) = bs(i)
        Next
        bVector = New Vector(Of Single)(10, False)
        For i As Integer = 30 To 39
            bVector(i - 30) = bs(i)
        Next
        Me.outputLayerBiasVector = bVector

        For i As Integer = 0 To 784 * 30 - 1
            Me.hiddenLayerWeightsList(0).array2D(i Mod 784, Int(i / 784)) = ws(i)
        Next
        Dim pointer As Integer = 0

        Me.outputLayerWeightsMatrix = New Matrix(30, 10, False)

        For i As Integer = 784 * 30 To 784 * 30 + 30 * 10 - 1

            Me.outputLayerWeightsMatrix.array2D(pointer Mod 30, Int(pointer / 30)) = ws(i)
            pointer += 1
        Next

    End Sub
    '所有的隐藏层和它之前的层组成的权重矩阵，第一个权重矩阵是输入层和第一个隐藏层建立的
    '权重矩阵

    Private _hiddenLayerWeightsList As List(Of Matrix)

    Public Property hiddenLayerWeightsList() As List(Of Matrix)
        Get
            Return _hiddenLayerWeightsList
        End Get
        Set(ByVal value As List(Of Matrix))
            _hiddenLayerWeightsList = value
        End Set
    End Property
    '所有的隐藏层才会拥有偏执向量
    Private _hiddenLayerBiasList As List(Of Vector(Of Single))
    Public Property hiddenLayerBiasList() As List(Of Vector(Of Single))
        Get
            Return _hiddenLayerBiasList
        End Get
        Set(ByVal value As List(Of Vector(Of Single)))
            _hiddenLayerBiasList = value
        End Set
    End Property

    '输出层和最后一层隐藏层之间的权重矩阵
    Private _outputLayerWeightsMatrix As Matrix
    Public Property outputLayerWeightsMatrix() As Matrix
        Get
            Return _outputLayerWeightsMatrix
        End Get
        Set(ByVal value As Matrix)
            _outputLayerWeightsMatrix = value
        End Set
    End Property
    <NonSerialized>
    Private _outputLayerZVector As Vector(Of Single)

    ''' <summary>
    ''' 输出层的没有经过激活函数的向量
    ''' </summary>
    ''' <returns></returns>
    Public Property outputLayerZVector() As Vector(Of Single)
        Get
            Return _outputLayerZVector
        End Get
        Set(ByVal value As Vector(Of Single))
            _outputLayerZVector = value
        End Set
    End Property
    ''' <summary>
    ''' 初始化权重们和偏执们和误差们和激活值们和bitmap
    ''' </summary>
    Private Sub initialWeightsAndBiasAndErrorsAndActivationsAndBitmap()
        Me.hiddenLayerWeightsList = New List(Of Matrix)
        Me.hiddenLayerBiasList = New List(Of Vector(Of Single))
        Me.hiddenLayerErrorsList = New List(Of Vector(Of Single))
        Me.hiddenLayerZVectorList = New List(Of Vector(Of Single))
        Me.outputLayerWeightsMatrix = New Matrix(hiddenLayer_NueralCountVector(hiddenLayer_NueralCountVector.length - 1), outputLayer_NeuralCount, True)
        Me.outputLayerBiasVector = New Vector(Of Single)(Me.outputLayer_NeuralCount, True)
        '所有的隐藏层和它之前的层组成的权重矩阵，第一个权重矩阵是输入层和第一个隐藏层建立的
        Me.hiddenLayerWeightsList.Add(New Matrix(inputLayer_NeuralCount, hiddenLayer_NueralCountVector(0), True))
        Me.hiddenLayerActivationsList = New List(Of Vector(Of Single))
        For i As Integer = 0 To hiddenLayer_NueralCountVector.length - 1
            Me.hiddenLayerActivationsList.Add(Nothing)
            Me.hiddenLayerBiasList.Add(New Vector(Of Single)(hiddenLayer_NueralCountVector(i), True))
            Me.hiddenLayerErrorsList.Add(Nothing)
            Me.hiddenLayerZVectorList.Add(Nothing)
            If i = 0 Then Continue For
            Me.hiddenLayerWeightsList.Add(New Matrix(hiddenLayer_NueralCountVector(i - 1), hiddenLayer_NueralCountVector(i), True))
        Next
        Me.bitmap = New Bitmap(1000, 1000)

    End Sub
    <NonSerialized>
    Private _inputXVector As Vector(Of Single)
    ''' <summary>
    ''' 输入层的向量
    ''' </summary>
    ''' <returns></returns>
    Public Property inputXVector() As Vector(Of Single)
        Get
            Return _inputXVector
        End Get
        Set(ByVal value As Vector(Of Single))
            _inputXVector = value
        End Set
    End Property
    <NonSerialized>
    Private _outputActivationsVector As Vector(Of Single)
    ''' <summary>
    ''' 输出层的向量
    ''' </summary>
    ''' <returns></returns>
    Public Property outputActivationsVector() As Vector(Of Single)
        Get
            Return _outputActivationsVector
        End Get
        Set(ByVal value As Vector(Of Single))
            _outputActivationsVector = value
        End Set
    End Property
    <NonSerialized>
    Private _outputYVector As Vector(Of Single)
    ''' <summary>
    ''' 期望输出的Y值向量
    ''' </summary>
    ''' <returns></returns>
    Public Property outputYVector() As Vector(Of Single)
        Get
            Return _outputYVector
        End Get
        Set(ByVal value As Vector(Of Single))
            _outputYVector = value
        End Set
    End Property
    Private _outputLayerBiasVector As Vector(Of Single)

    ''' <summary>
    ''' 输出层的偏置向量
    ''' </summary>
    ''' <returns></returns>
    Public Property outputLayerBiasVector() As Vector(Of Single)
        Get
            Return _outputLayerBiasVector
        End Get
        Set(ByVal value As Vector(Of Single))
            _outputLayerBiasVector = value
        End Set
    End Property
    <NonSerialized>
    Private _hiddenLayerZVectorList As List(Of Vector(Of Single))
    ''' <summary>
    ''' 隐藏层的z向量 们
    ''' </summary>
    ''' <returns></returns>
    Public Property hiddenLayerZVectorList() As List(Of Vector(Of Single))
        Get
            Return _hiddenLayerZVectorList
        End Get
        Set(ByVal value As List(Of Vector(Of Single)))
            _hiddenLayerZVectorList = value
        End Set
    End Property
    <NonSerialized>
    Private _outputErrorVector As Vector(Of Single)
    ''' <summary>
    ''' 输出层的error向量
    ''' </summary>
    ''' <returns></returns>
    Public Property outputErrorVector() As Vector(Of Single)
        Get
            Return _outputErrorVector
        End Get
        Set(ByVal value As Vector(Of Single))
            _outputErrorVector = value
        End Set
    End Property
    <NonSerialized>
    Private _hiddenLayerErrorList As List(Of Vector(Of Single))
    ''' <summary>
    ''' 隐藏层的误差们
    ''' </summary>
    ''' <returns></returns>
    Public Property hiddenLayerErrorsList() As List(Of Vector(Of Single))
        Get
            Return _hiddenLayerErrorList
        End Get
        Set(ByVal value As List(Of Vector(Of Single)))
            _hiddenLayerErrorList = value
        End Set
    End Property
    <NonSerialized>
    Private _hiddenLayerActivationsList As List(Of Vector(Of Single))
    Public Property hiddenLayerActivationsList() As List(Of Vector(Of Single))
        Get
            Return _hiddenLayerActivationsList
        End Get
        Set(ByVal value As List(Of Vector(Of Single)))
            _hiddenLayerActivationsList = value
        End Set
    End Property
    ''' <summary>
    ''' 前馈，由一组输入经过各个层的传播得到一组输出,输出是激活后的向量
    ''' </summary>
    ''' <param name="m_inputXVector">输入向量</param>
    ''' <returns>输出激活之后的向量</returns>
    Function feedForward(m_inputXVector As Vector(Of Single)) As Vector(Of Single)
#Region "计算第一个隐藏层的未激活和激活值向量"
        Me.inputXVector = m_inputXVector '输入的向量是一个属性
        '计算x*w+b,x是输入层向量
        Dim WmulXaddB As Vector(Of Single) = Calculate.addTwoVectors(
            Calculate.multiplyMatrixAndVector(hiddenLayerWeightsList(0), inputXVector),
            hiddenLayerBiasList(0))
        Me.hiddenLayerZVectorList(0) = WmulXaddB '隐藏层的z向量属性
        WmulXaddB = Calculate.activationFunction(WmulXaddB) '激活一下
        Me.hiddenLayerActivationsList(0) = WmulXaddB
#End Region
#Region "'计算每个第2个隐藏层到最后一个隐藏层的z和a属性,z是没有激活的权重*输入+偏置,a是激活之后的"
        For i As Integer = 1 To hiddenLayer_NueralCountVector.length - 1
            WmulXaddB = Calculate.addTwoVectors(Calculate.multiplyMatrixAndVector(hiddenLayerWeightsList(i), WmulXaddB), hiddenLayerBiasList(i))
            Me.hiddenLayerZVectorList(i) = WmulXaddB
            WmulXaddB = Calculate.activationFunction(WmulXaddB)
            Me.hiddenLayerActivationsList(i) = WmulXaddB
        Next
#End Region

#Region "计算输出层的z和a向量,z是没有激活的权重*输入+偏置,a是激活之后的"
        Dim m_outputActivationsVector As New Vector(Of Single)
        Dim outputZVector As Vector(Of Single) = Calculate.addTwoVectors(Calculate.multiplyMatrixAndVector(Me.outputLayerWeightsMatrix, WmulXaddB), Me.outputLayerBiasVector)
        Me.outputLayerZVector = outputZVector '这是输出层没有经过激活的向量
        m_outputActivationsVector = Calculate.activationFunction(outputZVector) '激活之后
        Me.outputActivationsVector = m_outputActivationsVector
#End Region
        Return m_outputActivationsVector
    End Function
    <NonSerialized>
    Private _bitmap As Bitmap
    ''' <summary>
    ''' 可视化
    ''' </summary>
    ''' <returns>返回一个图片</returns>
    Private Property bitmap() As Bitmap
        Get

            Return _bitmap
        End Get
        Set(ByVal value As Bitmap)
            _bitmap = value
        End Set
    End Property
    ''' <summary>
    ''' 调用函数的时候就会显示一个神经网络的图片
    ''' </summary>
    ''' <returns>返回一个bitmap图片10000*1000</returns>
    Function getBitmap() As Bitmap
        Me.draw()
        Return Me.bitmap
    End Function
    ''' <summary>
    ''' 调用这个方法就可以画出来当前运行之后的 网络
    ''' </summary>
    Sub draw()
#Region "初始化一些绘图参数,包括背景颜色,〇的大小,间距,开始位置等等"


        Dim g As Graphics = Graphics.FromImage(bitmap)
        g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        g.Clear(Color.Lavender)
        Dim circleRadius As Single = 10
        Dim circleInterval As Single = 15
        Dim layerInterval As Single = bitmap.Width * 0.618 / (hiddenLayer_NueralCountVector.length + 1)
        Dim startY As Integer = (bitmap.Size.Height - inputLayer_NeuralCount * (2 * circleRadius + circleInterval)) / 2
        Dim startX As Integer = bitmap.Width * (1 - 0.618) / 2
        Dim pointList As New List(Of Point())
#End Region
#Region "画输入层的输入数据和 〇"


        Dim points(inputLayer_NeuralCount - 1) As Point
        For i As Integer = 0 To inputLayer_NeuralCount - 1

            Dim x As Single = startX
            Dim y As Single = startY + i * circleRadius * 2 + i * circleInterval
            g.DrawString(inputXVector(i).ToString & "→", New Font("黑体"， 15), Brushes.Green, x - 60, y)
            Dim rect As New Rectangle(x, y, circleRadius * 2, circleRadius * 2)
            g.FillEllipse(Brushes.Green, rect)
            g.DrawString(Me.inputXVector(i), New Font("黑体", 8), Brushes.Yellow, x + 5, y + circleRadius - 5)
            points(i) = New Point(x, y)
        Next
        pointList.Add(points)
#End Region
#Region "画隐藏层的〇"


        For i As Integer = 0 To hiddenLayer_NueralCountVector.length - 1
            ReDim points(hiddenLayer_NueralCountVector(i) - 1)
            startX += layerInterval
            startY = (bitmap.Size.Height - hiddenLayer_NueralCountVector(i) * (2 * circleRadius + circleInterval)) / 2
            For j As Integer = 0 To hiddenLayer_NueralCountVector(i) - 1
                Dim x As Single = startX
                Dim y As Single = startY + j * circleRadius * 2 + j * circleInterval

                Dim rect As New Rectangle(x, y, circleRadius * 2, circleRadius * 2)
                g.DrawEllipse(Pens.Black, rect)
                g.DrawString(Me.hiddenLayerBiasList(i)(j), New Font("黑体", 9), Brushes.Red, x + 5, y + circleRadius - 5)
                points(j) = New Point(x, y)
            Next
            pointList.Add(points)
        Next
#End Region
#Region "输出层的〇"


        startX += layerInterval
        ReDim points(outputLayer_NeuralCount - 1)
        For i As Integer = 0 To outputLayer_NeuralCount - 1

            startY = (bitmap.Size.Height - outputLayer_NeuralCount * (2 * circleRadius + circleInterval)) / 2
            Dim x As Single = startX
            Dim y As Single = startY + i * circleRadius * 2 + i * circleInterval
            Dim rect As New Rectangle(x, y, circleRadius * 2, circleRadius * 2)
            g.DrawString("→" & Math.Round(outputActivationsVector(i), 2).ToString, New Font("黑体"， 15), Brushes.Red, x + 50, y)

            points(i) = New Point(x, y)
            g.FillEllipse(Brushes.Red, rect)

        Next
        pointList.Add(points)
#End Region
#Region "画〇之间的连线和"
        For i As Integer = 0 To pointList.Count - 2
            For j As Integer = 0 To pointList(i).Count - 1
                For k As Integer = 0 To pointList(i + 1).Count - 1
                    Dim pStart As Point = pointList(i)(j)
                    Dim pEnd As Point = pointList(i + 1)(k)
                    g.DrawLine(Pens.Green, New Point(pStart.X + 2 * circleRadius, pStart.Y + circleRadius), New Point(pEnd.X, pEnd.Y + circleRadius))
                Next
            Next
        Next
#End Region
    End Sub
    ''' <summary>
    ''' 产生隐藏层和输出层的误差向量
    ''' 向前传播
    ''' </summary>
    Private Function backPropagateTheError(m_outputYVector As Vector(Of Single)) As delta

#Region "定义  一次bp产生的梯度，初始化一些梯度的数据结构"
        Me.outputYVector = m_outputYVector
        Dim lenOfHiddenLayers As Integer = hiddenLayer_NueralCountVector.length

        Dim resultDelta As New delta
        resultDelta.deltaBias.hiddenLayerDeltaBiasList = New List(Of Vector(Of Single))

        resultDelta.deltaWeights.hiddenLayerDeltaWeightsList = New List(Of Matrix)
        For i As Integer = 0 To lenOfHiddenLayers - 1
            resultDelta.deltaBias.hiddenLayerDeltaBiasList.Add(Nothing)
            resultDelta.deltaWeights.hiddenLayerDeltaWeightsList.Add(Nothing)
        Next

#End Region




#Region "计算输出层and隐藏层的误差向量"
        '输出层的误差
        If Me.costFunctionType = CostFunctionTypes.QuadraticCostFunction Then
            Me.outputErrorVector =
                        Calculate.multiplyTwoVectors(
                            Calculate.derivativeOfCost(Me.outputActivationsVector, Me.outputYVector),
                            Calculate.derivativeOfActivationFunction(Me.outputLayerZVector))
        ElseIf Me.costFunctionType = CostFunctionTypes.crossEntropCostFunction Then
            Me.outputErrorVector = Calculate.minusTwoVectors(Me.outputActivationsVector, Me.outputYVector)
        End If

        '隐藏层们的误差
        hiddenLayerErrorsList(lenOfHiddenLayers - 1) = Calculate.multiplyTwoVectors(
                                            Calculate.multiplyMatrixAndVector(Calculate.transposition(Me.outputLayerWeightsMatrix), Me.outputErrorVector),
                                            Calculate.derivativeOfActivationFunction(hiddenLayerZVectorList(lenOfHiddenLayers - 1)))

        For i As Integer = lenOfHiddenLayers - 2 To 0 Step -1
            hiddenLayerErrorsList(i) = Calculate.multiplyTwoVectors(
                                            Calculate.multiplyMatrixAndVector(Calculate.transposition(Me.hiddenLayerWeightsList(i + 1)), Me.hiddenLayerErrorsList(i + 1)),
                                            Calculate.derivativeOfActivationFunction(hiddenLayerZVectorList(i)))
        Next

#End Region

#Region "反向传播误差,权重和偏置"


        Dim deltaWeightsMatrix As Matrix = Calculate.transposition(Calculate.multiplyVectorAndMatrix(Me.outputErrorVector, Calculate.transposition(Calculate.transformVectorToMatrix(Me.hiddenLayerActivationsList(lenOfHiddenLayers - 1)))))
        Dim deltaBiasVector As Vector(Of Single) = outputErrorVector

        resultDelta.deltaWeights.outputLayerDeltaWeightsMatrix = deltaWeightsMatrix
        resultDelta.deltaBias.outputLayerDeltaBiasVector = deltaBiasVector

        For i As Integer = lenOfHiddenLayers - 1 To 1 Step -1
            deltaWeightsMatrix = Calculate.transposition(Calculate.multiplyVectorAndMatrix(Me.hiddenLayerErrorsList(i), Calculate.transposition(Calculate.transformVectorToMatrix(Me.hiddenLayerActivationsList(i - 1)))))
            deltaBiasVector = Me.hiddenLayerErrorsList(i)

            resultDelta.deltaBias.hiddenLayerDeltaBiasList(i) = deltaBiasVector
            resultDelta.deltaWeights.hiddenLayerDeltaWeightsList(i) = deltaWeightsMatrix
        Next
#End Region
#Region "由第一层隐藏层反向传播到输入层"

        deltaWeightsMatrix = Calculate.transposition(Calculate.multiplyVectorAndMatrix(Me.hiddenLayerErrorsList(0), Calculate.transposition(Calculate.transformVectorToMatrix(Me.inputXVector))))
        deltaBiasVector = Me.hiddenLayerErrorsList(0)

        resultDelta.deltaWeights.hiddenLayerDeltaWeightsList(0) = deltaWeightsMatrix
        resultDelta.deltaBias.hiddenLayerDeltaBiasList(0) = deltaBiasVector

#End Region
        Return resultDelta
    End Function
    Sub SGD(dnnDatas As DnnData, miniBatchSize As Integer, iteratorCount As Integer)
        ReDim Me._testPoints(iteratorCount - 1)
        Dim xWidth As Integer = dnnDatas.trainXMatrix.colunmsCount
        Dim yWidth As Integer = dnnDatas.trainYMatrix.colunmsCount
        For k As Integer = 1 To iteratorCount
            Calculate.shuffleMatrix(dnnDatas.trainXMatrix, dnnDatas.trainYMatrix)
            For i As Integer = 0 To dnnDatas.trainXMatrix.rowsCount - 1 Step miniBatchSize
                If i + miniBatchSize > dnnDatas.trainXMatrix.rowsCount Then
                    Exit For
                End If
                Dim xBatchMatrix As Matrix = Calculate.clipMatrix(dnnDatas.trainXMatrix, i, (i + miniBatchSize) - 1, 0, xWidth - 1)
                Dim yBatchMatrix As Matrix = Calculate.clipMatrix(dnnDatas.trainYMatrix, i, (i + miniBatchSize) - 1, 0, yWidth - 1)
                Me.updateMiniBatch(xBatchMatrix, yBatchMatrix)
            Next
            Dim accuracy As Single = Me.test(dnnDatas.testXMatrix, dnnDatas.testYMatrix)
            Me.testPoints(k - 1) = New Point((k - 1) * 500 / iteratorCount, 500 - accuracy * 500)
            Debug.Print("第" & k & "次迭代,正确率为:  " & accuracy)
            DataManager.SerializeToFile(Me, k.ToString)
        Next
    End Sub
    Private _evalueImage As Bitmap
    Private Property evaluaImage() As Bitmap
        Get
            Return _evalueImage
        End Get
        Set(ByVal value As Bitmap)
            _evalueImage = value
        End Set
    End Property
    Function getEvaluateImage() As Bitmap
        drawEvaluateImage()
        Return Me.evaluaImage
    End Function
    Private Sub drawEvaluateImage()
        evaluaImage = New Bitmap(500, 500)
        Dim g As Graphics = Graphics.FromImage(evaluaImage)
        g.Clear(Color.Wheat)
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        Dim pCount As Integer = Me.testPoints.Count
        g.DrawLine(Pens.Green, 0, 0, 0, 500)
        g.DrawLine(Pens.Green, 0, 500, 500, 500)

        g.DrawCurve(Pens.Black, Me.testPoints)
    End Sub
    Private _testPoints() As Point
    Private Property testPoints As Point()
        Get
            Return _testPoints
        End Get
        Set(ByVal value As Point())
            _testPoints = value
        End Set
    End Property
    Sub updateMiniBatch(miniBatchXMatrix As Matrix, miniBatchYMatrix As Matrix)
        Dim batchSize As Integer = miniBatchXMatrix.rowsCount
        feedForward(Calculate.matrixRowToVector(miniBatchXMatrix, 0)) '前馈神经网络，会更新dnn的一些内部数据。
        Dim resultDelta As delta = backPropagateTheError(Calculate.matrixRowToVector(miniBatchYMatrix, 0)) '对Y反向传播一次误差，得到第一次bp的梯度

        Dim tempDelta As delta
        '对miniBatch其他的数据进行反向传播。就算小批量带来的梯度总和
        For i As Integer = 1 To miniBatchXMatrix.rowsCount - 1
            feedForward(Calculate.matrixRowToVector(miniBatchXMatrix, i))
            tempDelta = backPropagateTheError(Calculate.matrixRowToVector(miniBatchYMatrix, i))
            resultDelta.deltaBias.outputLayerDeltaBiasVector = Calculate.addTwoVectors(resultDelta.deltaBias.outputLayerDeltaBiasVector, tempDelta.deltaBias.outputLayerDeltaBiasVector)
            resultDelta.deltaWeights.outputLayerDeltaWeightsMatrix = Calculate.addTwoMatrix(resultDelta.deltaWeights.outputLayerDeltaWeightsMatrix, tempDelta.deltaWeights.outputLayerDeltaWeightsMatrix)
            For j As Integer = 0 To Me.hiddenLayer_NueralCountVector.length - 1
                resultDelta.deltaBias.hiddenLayerDeltaBiasList(j) = Calculate.addTwoVectors(resultDelta.deltaBias.hiddenLayerDeltaBiasList(j), tempDelta.deltaBias.hiddenLayerDeltaBiasList(j))
                resultDelta.deltaWeights.hiddenLayerDeltaWeightsList(j) = Calculate.addTwoMatrix(resultDelta.deltaWeights.hiddenLayerDeltaWeightsList(j), tempDelta.deltaWeights.hiddenLayerDeltaWeightsList(j))
            Next
        Next
        '接下来是更新权重和偏执，公式是w=w-w均*studyRate
        Me.outputLayerBiasVector = Calculate.minusTwoVectors(Me.outputLayerBiasVector, Calculate.scalarMultiplicationVector(resultDelta.deltaBias.outputLayerDeltaBiasVector, Me.studyRate / batchSize))
        Me.outputLayerWeightsMatrix = Calculate.minusTwoMatrix(Me.outputLayerWeightsMatrix, Calculate.scalarMultiplicationMatrix(resultDelta.deltaWeights.outputLayerDeltaWeightsMatrix, Me.studyRate / batchSize))
        For i As Integer = 0 To Me.hiddenLayer_NueralCountVector.length - 1
            Me.hiddenLayerBiasList(i) = Calculate.minusTwoVectors(Me.hiddenLayerBiasList(i), Calculate.scalarMultiplicationVector(resultDelta.deltaBias.hiddenLayerDeltaBiasList(i), Me.studyRate / batchSize))
            Me.hiddenLayerWeightsList(i) = Calculate.minusTwoMatrix(Me.hiddenLayerWeightsList(i), Calculate.scalarMultiplicationMatrix(resultDelta.deltaWeights.hiddenLayerDeltaWeightsList(i), Me.studyRate / batchSize))
        Next

    End Sub

    ''' <summary>
    ''' 对数据进行前馈 得到结果a然后和和预期y进行比较,得到测试数据的准确率
    ''' </summary>
    ''' <param name="testXMatrix">输入矩阵</param>
    ''' <param name="testYMatrix">输出矩阵</param>
    ''' <returns>返回准确率,介于0到1</returns>
    Function test(testXMatrix As Matrix, testYMatrix As Matrix) As Single
        Dim lenOfTest As Integer = testXMatrix.rowsCount
        Dim count As Integer = 0
        For i As Integer = 0 To lenOfTest - 1
            If feedForward(Calculate.matrixRowToVector(testXMatrix, i)).getMaxIndex = (Calculate.matrixRowToVector(testYMatrix, i)).getMaxIndex Then
                count += 1 '如果前馈得到的结果和预期一致的话
            End If
        Next
        Return count / lenOfTest
    End Function


End Class
