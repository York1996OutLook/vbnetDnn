Imports System.Math

Public Class Calculate
    ''' <summary>
    ''' 矩阵变成一个向量
    ''' </summary>
    ''' <param name="rawMatrix">原矩阵</param>
    ''' <returns>返回一个大小和矩阵大小一样的向量（cols*rows）</returns>
    Shared Function matrixToVector(rawMatrix As Matrix) As Vector(Of Single)

        Dim colsNumber As Integer = rawMatrix.colunmsCount
        Dim rowsNumber As Integer = rawMatrix.rowsCount

        Dim resultVector As New Vector(Of Single)(colsNumber * rowsNumber, False)
        Dim pointer As Integer = 0
        For i As Integer = 0 To rowsNumber - 1
            For j As Integer = 0 To colsNumber - 1
                resultVector(pointer) = rawMatrix.array2D(i, j)
                pointer += 1
            Next
        Next

        Return resultVector
    End Function
    ''' <summary>
    ''' 求两个同型矩阵的和
    ''' </summary>
    ''' <param name="matrix_1">第一个矩阵</param>
    ''' <param name="matrix_2">第二个矩阵</param>
    ''' <returns>返回两个矩阵的和</returns>
    Shared Function addTwoMatrix(matrix_1 As Matrix, matrix_2 As Matrix) As Matrix
        If matrix_1 Is Nothing Then
            Throw New Exception("matrix_1矩阵为空")
        End If
        If matrix_1 Is Nothing Then
            Throw New Exception("matrix_2矩阵为空")
        End If
        If matrix_1.rowsCount <> matrix_2.rowsCount Then
            Throw New Exception("行数不一致,不能进行加法运算")
        End If
        If matrix_1.colunmsCount <> matrix_2.colunmsCount Then
            Throw New Exception("列数不一致,不能进行加法运算")
        End If

        Dim rowsCount As Integer = matrix_1.rowsCount
        Dim colsCount As Integer = matrix_2.colunmsCount
        Dim resultMatrix As New Matrix(rowsCount, colsCount, False)
        For r As Integer = 0 To rowsCount - 1
            For c As Integer = 0 To colsCount - 1
                resultMatrix.array2D(r, c) = matrix_1.array2D(r, c) + matrix_2.array2D(r, c)
            Next
        Next
        Return resultMatrix
    End Function
    ''' <summary>
    ''' 返回矩阵1减去矩阵2的值,对应元素相减,需要行数列数一致
    ''' </summary>
    ''' <param name="matrix_1">第一个矩阵</param>
    ''' <param name="matrix_2">被减去的矩阵</param>
    ''' <returns>返回两个矩阵的差值</returns>
    Shared Function minusTwoMatrix(matrix_1 As Matrix, matrix_2 As Matrix) As Matrix
        If matrix_1 Is Nothing Then
            Throw New Exception("matrix_1矩阵为空")
        End If
        If matrix_1 Is Nothing Then
            Throw New Exception("matrix_2矩阵为空")
        End If
        If matrix_1.rowsCount <> matrix_2.rowsCount Then
            Throw New Exception("行数不一致,不能进行减法运算")
        End If
        If matrix_1.colunmsCount <> matrix_2.colunmsCount Then
            Throw New Exception("列数不一致,不能进行减法运算")
        End If

        Dim rowsCount As Integer = matrix_1.rowsCount
        Dim colsCount As Integer = matrix_2.colunmsCount
        Dim resultMatrix As New Matrix(rowsCount, colsCount, False)
        For r As Integer = 0 To rowsCount - 1
            For c As Integer = 0 To colsCount - 1
                resultMatrix.array2D(r, c) = matrix_1.array2D(r, c) - matrix_2.array2D(r, c)
            Next
        Next
        Return resultMatrix
    End Function
    ''' <summary>
    ''' 矩阵相乘,左矩阵的列数等于右边矩阵的行数
    ''' </summary>
    ''' <param name="matrix_1">左矩阵</param>
    ''' <param name="matrix_2">右矩阵</param>
    ''' <returns>返回乘起来的矩阵</returns>
    Shared Function multiplyTwoMatrix(matrix_1 As Matrix, matrix_2 As Matrix) As Matrix
        If matrix_1 Is Nothing Then
            Throw New Exception("matrix_1矩阵为空")
        End If
        If matrix_1 Is Nothing Then
            Throw New Exception("matrix_2矩阵为空")
        End If
        If matrix_1.colunmsCount <> matrix_2.rowsCount Then
            Throw New Exception("左矩阵的列数 不等于 右边矩阵的行数,不能进行乘法运算")
        End If


        Dim resultMatrix As New Matrix(matrix_1.rowsCount, matrix_2.colunmsCount, False)
        '模型,m*n*n*s的矩阵是m*s的
        Dim sum As Single
        For m As Integer = 0 To matrix_1.rowsCount - 1
            For s As Integer = 0 To matrix_2.colunmsCount - 1
                sum = 0
                For i As Integer = 0 To matrix_1.colunmsCount - 1
                    sum += matrix_1.array2D(m, i) * matrix_2.array2D(i, s)
                Next
                resultMatrix.array2D(m, s) = sum
            Next
        Next
        Return resultMatrix

    End Function
    Shared Function multiplyTwoMatrix1(matrix_1 As Matrix, matrix_2 As Matrix) As Matrix
        If matrix_1 Is Nothing Then
            Throw New Exception("matrix_1矩阵为空")
        End If
        If matrix_1 Is Nothing Then
            Throw New Exception("matrix_2矩阵为空")
        End If
        If matrix_1.colunmsCount <> matrix_2.rowsCount Then
            Throw New Exception("左矩阵的列数 不等于 右边矩阵的行数,不能进行乘法运算")
        End If

        Dim resultMatrix As New Matrix(matrix_1.rowsCount, matrix_2.colunmsCount, False)
        '模型,m*n*n*s的矩阵是m*s的
        Dim sum As Single
        Dim T As Matrix = Calculate.transposition(matrix_2)

        For m As Integer = 0 To matrix_1.rowsCount - 1
            For s As Integer = 0 To matrix_2.colunmsCount - 1
                For i As Integer = 0 To matrix_1.colunmsCount - 1
                    sum = 0
                    For j As Integer = 0 To matrix_2.rowsCount - 1
                        sum += matrix_1.array2D(m, j) * T.array2D(i, s)
                    Next
                    resultMatrix.array2D(m, s) = sum
                Next
            Next
        Next
        Return resultMatrix
    End Function
    ''' <summary>
    ''' 矩阵的数乘运算
    ''' </summary>
    ''' <param name="rawMatrix">原始矩阵</param>
    ''' <param name="scalarNum">矩阵要乘的一个数字</param>
    ''' <returns>矩阵数乘的结果</returns>
    Shared Function scalarMultiplicationMatrix(rawMatrix As Matrix, scalarNum As Single) As Matrix
        If rawMatrix Is Nothing Then
            Throw New Exception("原矩阵为空")
        End If
        Dim rowsCount As Integer = rawMatrix.rowsCount
        Dim colsCount As Integer = rawMatrix.colunmsCount
        Dim resultMatrix As New Matrix(rowsCount, colsCount, False)
        For r As Integer = 0 To rowsCount - 1
            For c As Integer = 0 To colsCount - 1
                resultMatrix.array2D(r, c) = rawMatrix.array2D(r, c) * scalarNum
            Next
        Next
        Return resultMatrix
    End Function
    ''' <summary>
    ''' 数乘向量
    ''' </summary>
    ''' <param name="rawVector">原始向量</param>
    ''' <param name="scalarNum">数</param>
    ''' <returns>返回这个数乘以向量的每一个元素</returns>
    Shared Function scalarMultiplicationVector(rawVector As Vector(Of Single), scalarNum As Single) As Vector(Of Single)
        Dim lenOfRawVector As Integer = rawVector.length
        Dim resultVector As New Vector(Of Single)(rawVector.length, False)
        For i As Integer = 0 To lenOfRawVector - 1
            resultVector(i) = rawVector(i) * scalarNum
        Next
        Return resultVector
    End Function
    ''' <summary>
    ''' 矩阵转置
    ''' </summary>
    ''' <param name="rawMatrix">原矩阵</param>
    ''' <returns>返回转置之后的矩阵</returns>
    Shared Function transposition(rawMatrix As Matrix) As Matrix
        If rawMatrix Is Nothing Then
            Throw New Exception("原矩阵为空")
        End If
        Dim rowsCount As Integer = rawMatrix.rowsCount
        Dim colsCount As Integer = rawMatrix.colunmsCount
        Dim resultMatrix As New Matrix(colsCount, rowsCount, False)
        For r As Integer = 0 To rowsCount - 1
            For c As Integer = 0 To colsCount - 1
                resultMatrix.array2D(c, r) = rawMatrix.array2D(r, c)
            Next
        Next
        Return resultMatrix
    End Function
    ''' <summary>
    ''' 求矩阵中所有元素的和
    ''' </summary>
    ''' <param name="rawMatrix">被求和的矩阵</param>
    ''' <returns>矩阵元素的和</returns>
    Shared Function sumOfMatrixItems(rawMatrix As Matrix) As Single
        Dim sum As Single = 0
        Dim rowsCount As Integer = rawMatrix.rowsCount
        Dim colsCount As Integer = rawMatrix.colunmsCount
        For r As Integer = 0 To rowsCount - 1
            For c As Integer = 0 To colsCount - 1
                sum += rawMatrix.array2D(r, c)
            Next
        Next
        Return sum
    End Function
    ''' <summary>
    ''' 从原矩阵上剪裁一个矩阵
    ''' </summary>
    ''' <param name="rawMatrix">原始矩阵</param>
    ''' <param name="startRow">开始行</param>
    ''' <param name="endRow">结束行</param>
    ''' <param name="startColumn">开始列</param>
    ''' <param name="endColumn">结束列</param>
    ''' <returns>返回矩形区域代表的矩阵</returns>
    Shared Function clipMatrix(
                        rawMatrix As Matrix,
                        startRow As Integer,
                        endRow As Integer,
                        startColumn As Integer,
                        endColumn As Integer) As Matrix
        If startRow < 0 Then Throw New Exception("开始行数不能小于0")
        If startColumn < 0 Then Throw New Exception("开始列数不能小于0")
        Dim rowsCount As Integer = rawMatrix.rowsCount
        Dim colsCount As Integer = rawMatrix.colunmsCount
        If endRow > rowsCount - 1 Then Throw New Exception("结束行数不能大于原来矩阵的行数")
        If endColumn > colsCount - 1 Then Throw New Exception("结束列数不能大于原来矩阵的列数")

        Dim resultMatrix As New Matrix(endRow - startRow + 1, endColumn - startColumn + 1, False)

        For r As Integer = 0 To endRow - startRow
            For c As Integer = 0 To endColumn - startColumn
                resultMatrix.array2D(r, c) = rawMatrix.array2D(r + startRow, c + startColumn)
            Next
        Next
        Return resultMatrix
    End Function
    ''' <summary>
    ''' 计算两个向量对应元素相乘，要求两个向量长度一致[Hadamard 乘积]
    ''' </summary>
    ''' <param name="vector1">第一个向量</param>
    ''' <param name="vector2">第二个向量</param>
    ''' <returns>返回一个和参数中的向量同长度的向量</returns>
    Shared Function multiplyTwoVectors(vector1 As Vector(Of Single), vector2 As Vector(Of Single)) As Vector(Of Single)
        If vector1.length <> vector2.length Then
            Throw New Exception("两个向量元素个数相同才可以相乘")
        End If
        Dim resultVector As New Vector(Of Single)(vector1.length, False)
        For i As Integer = 0 To vector1.length - 1

            resultVector(i) = vector1(i) * vector2(i)

        Next
        Return resultVector
    End Function
    ''' <summary>
    ''' 计算向量和矩阵的乘积1*n*n*s=1*s
    ''' </summary>
    ''' <param name="rawVector">原来的向量n*1</param>
    ''' <param name="rawMatrix">原来的矩阵n*s</param>
    ''' <returns>返回一个s*1的向量</returns>
    Shared Function multiplyMatrixAndVector(rawMatrix As Matrix, rawVector As Vector(Of Single)) As Vector(Of Single)
        Dim lenOfVector As Integer = rawVector.length
        Dim matrixRowsCount As Integer = rawMatrix.rowsCount
        Dim matrixColsCount As Integer = rawMatrix.colunmsCount
        If lenOfVector <> matrixRowsCount Then
            Throw New Exception("向量的列数和矩阵的行数不一致")
        End If
        Dim resultVector As New Vector(Of Single)(matrixColsCount, False)
        Dim sum As Single
        For c As Integer = 0 To matrixColsCount - 1
            sum = 0
            For i As Integer = 0 To lenOfVector - 1
                sum += rawMatrix.array2D(i, c) * rawVector(i)
            Next
            resultVector(c) = sum
        Next
        Return resultVector
    End Function
    ''' <summary>
    ''' 用s型函数来作为激活函数
    ''' </summary>
    ''' <param name="rawNum">被处理的单精度数字</param>
    ''' <returns>返回一个经过sigmoid函数处理之后的数字</returns>
    Private Shared Function sigmoid(rawNum As Single) As Single
        Return 1.0 / (1.0 + Exp(-rawNum))
    End Function
    ''' <summary>
    ''' 用s型函数来作为激活函数
    ''' </summary>
    ''' <param name="rawVector"></param>
    ''' <returns>返回s型激活后的向量</returns>
    Private Shared Function sigmoid(rawVector As Vector(Of Single)) As Vector(Of Single)
        Dim resultVector As New Vector(Of Single)(rawVector.length, False)
        For i As Integer = 0 To rawVector.length - 1
            resultVector(i) = sigmoid(rawVector(i))
        Next
        Return resultVector
    End Function
    ''' <summary>
    ''' 激活函数
    ''' </summary>
    ''' <param name="rawVector"></param>
    ''' <returns></returns>
    Shared Function activationFunction(rawVector As Vector(Of Single)) As Vector(Of Single)

        Return sigmoid(rawVector)
    End Function
    ''' <summary>
    ''' 激活函数的导数
    ''' </summary>
    ''' <param name="rawVector"></param>
    ''' <returns></returns>
    Shared Function derivativeOfActivationFunction(rawVector As Vector(Of Single)) As Vector(Of Single)
        'Dim resultVector As New Vector(Of Single)(rawVector.length, False)

        Return multiplyTwoVectors(sigmoid(rawVector), minusTwoVectors(createOnesVector(rawVector.length), sigmoid(rawVector)))
    End Function
    ''' <summary>
    ''' sigmoid函数的导数
    ''' </summary>
    ''' <param name="rawVector"></param>
    ''' <returns></returns>
    Private Shared Function derivativeOfSigmoid(rawVector As Vector(Of Single)) As Vector(Of Single)
        'Dim resultVector As New Vector(Of Single)(rawVector.length, False)

        Return multiplyTwoVectors(sigmoid(rawVector), minusTwoVectors(createOnesVector(rawVector.length), sigmoid(rawVector)))
    End Function
    ''' <summary>
    ''' 代价函数求导
    ''' </summary>
    ''' <param name="outputActivations">实际输出的激活值向量</param>
    ''' <param name="outputYVector">预期输出的激活值向量</param>
    ''' <returns>返回代价函数的导数</returns>
    Shared Function derivativeOfCost(outputActivations As Vector(Of Single), outputYVector As Vector(Of Single)) As Vector(Of Single)

        Return derivativeQuadraticCostFunction(outputActivations, outputYVector)
    End Function
    Private Shared Function crossEntropCostFunction(outputActivations As Vector(Of Single), outputYVector As Vector(Of Single)) As Single
        Dim lenOfVector As Integer = outputYVector.length
        Dim onePart As Vector(Of Single) = multiplyTwoVectors(outputYVector, Calculate.logE(outputActivations))
        Dim twoPart As Vector(Of Single) = multiplyTwoVectors(minusTwoVectors(createOnesVector(lenOfVector), outputYVector), logE(minusTwoVectors(createOnesVector(lenOfVector), outputActivations)))
        Dim result As Single = sumOfVector(addTwoVectors(onePart, twoPart), 1)

        Return result
    End Function
    ''' <summary>
    ''' 对原向量中的每个元素取对数
    ''' </summary>
    ''' <param name="rawVector"></param>
    ''' <returns></returns>
    Private Shared Function logE(rawVector As Vector(Of Single)) As Vector(Of Single)
        Dim lenOfVector As Single = rawVector.length
        Dim resultVector As New Vector(Of Single)
        For i As Integer = 0 To lenOfVector - 1
            resultVector(i) = Log(rawVector(i))
        Next
        Return resultVector
    End Function
    ''' <summary>
    ''' 二次代价函数的表达式
    ''' </summary>
    ''' <param name="outputActivations"></param>
    ''' <param name="outputYVector"></param>
    ''' <returns></returns>
    Private Shared Function quadraticCostFunction(outputActivations As Vector(Of Single), outputYVector As Vector(Of Single)) As Single
        Dim lenOfVector As Integer = outputActivations.length
        Dim resultVector As New Vector(Of Single)(lenOfVector, False)

        Dim tempMinusVector As Vector(Of Single) = minusTwoVectors(outputActivations, outputYVector)
        resultVector = multiplyTwoVectors(tempMinusVector, tempMinusVector)

        Return sumOfVector(resultVector, 0.5)
    End Function
    ''' <summary>
    ''' 二次代价函数的表达式
    ''' </summary>
    ''' <param name="outputActivations"></param>
    ''' <param name="outputYVector"></param>
    ''' <returns></returns>
    Private Shared Function derivativeQuadraticCostFunction(outputActivations As Vector(Of Single), outputYVector As Vector(Of Single)) As Vector(Of Single)
        '代价函数是一个确定的函数,所以导师也是一个确定的值
        'a-y,如果是期望大于激活值,那么激活值应该变大,这个导数值是负数,
        Return minusTwoVectors(outputActivations, outputYVector)
    End Function
    ''' <summary>
    ''' 创建一个向量,向量元素都是1
    ''' </summary>
    ''' <param name="lenOfVector">向量的长度</param>
    ''' <returns>返回一个元素值全是1的向量</returns>

    Shared Function createOnesVector(lenOfVector As Integer) As Vector(Of Single)
        Dim resultVector As New Vector(Of Single)(lenOfVector, False)
        For i As Integer = 0 To lenOfVector - 1
            resultVector(i) = 1
        Next
        Return resultVector
    End Function

    ''' <summary>
    ''' 返回两个同型向量的差
    ''' </summary>
    ''' <param name="vector1">第一个向量</param>
    ''' <param name="vector2">第二个向量</param>
    ''' <returns>返回vector1-vector2代表的向量</returns>
    Shared Function minusTwoVectors(vector1 As Vector(Of Single), vector2 As Vector(Of Single)) As Vector(Of Single)
        Dim lenOfVector As Integer = vector1.length
        If vector2.length <> lenOfVector Then Throw New Exception("两个向量长度不一致,不能做减法")
        Dim resultVector As New Vector(Of Single)(lenOfVector, False)
        For i As Integer = 0 To lenOfVector - 1
            resultVector(i) = vector1(i) - vector2(i)
        Next
        Return resultVector
    End Function
    ''' <summary>
    ''' 求向量每个元素的加和是多少,和 在与scalarNum相乘
    ''' </summary>
    ''' <param name="rawVector">原始向量</param>
    ''' <param name="scalarNum">数</param>
    ''' <returns></returns>
    Private Shared Function sumOfVector(rawVector As Vector(Of Single), scalarNum As Single) As Single

        Dim lenOfVector As Integer = rawVector.length
        Dim resultSingle As Single = 0
        For i As Integer = 0 To lenOfVector - 1
            resultSingle += rawVector(i)
        Next
        resultSingle *= scalarNum
        Return resultSingle
    End Function
    ''' <summary>
    ''' 向量求和，对应元素相加
    ''' </summary>
    ''' <param name="vector1">向量1</param>
    ''' <param name="vector2">向量2</param>
    ''' <returns>返回一个同型向量</returns>

    Shared Function addTwoVectors(vector1 As Vector(Of Single), vector2 As Vector(Of Single)) As Vector(Of Single)
        Dim lenOfVector As Integer = vector1.length
        Dim resultVector As New Vector(Of Single)(lenOfVector, False)
        For i As Integer = 0 To lenOfVector - 1
            resultVector(i) = vector1(i) + vector2(i)
        Next
        Return resultVector
    End Function
    ''' <summary>
    ''' 把一个向量转换成一个矩阵
    ''' </summary>
    ''' <param name="rawVector"></param>
    ''' <returns></returns>
    Shared Function transformVectorToMatrix(rawVector As Vector(Of Single)) As Matrix
        Dim lenOfVector As Integer = rawVector.length
        Dim resultMatrix As New Matrix(lenOfVector, 1, False)
        For i As Integer = 0 To lenOfVector - 1
            resultMatrix.array2D(i, 0) = rawVector(i)
        Next
        Return resultMatrix
    End Function
    ''' <summary>
    ''' 向量和矩阵相乘
    ''' </summary>
    ''' <param name="rawVector"></param>
    ''' <param name="rawMatrix"></param>
    ''' <returns>返回乘积</returns>
    Shared Function multiplyVectorAndMatrix(rawVector As Vector(Of Single), rawMatrix As Matrix) As Matrix
        Dim lenOfVector As Integer = rawVector.length
        Dim matrixColsCount As Integer = rawMatrix.colunmsCount
        If rawMatrix.rowsCount <> 1 Then Throw New Exception("矩阵的行数不是1,不能和向量乘起来")
        Dim resultMatrix As New Matrix(lenOfVector, matrixColsCount, False)
        For i As Integer = 0 To lenOfVector - 1
            For j As Integer = 0 To matrixColsCount - 1
                resultMatrix.array2D(i, j) = rawVector(i) * rawMatrix.array2D(0, j)
            Next
        Next
        Return resultMatrix
    End Function
    ''' <summary>
    ''' 对矩阵按行洗牌，打乱其中的元素,xy的打乱规则是一样的
    ''' </summary>
    ''' <param name="rawXMatrix">打乱的X矩阵</param>
    ''' <param name="rawYMatrix">打乱的Y矩阵</param>

    Shared Sub shuffleMatrix(ByRef rawXMatrix As Matrix, ByRef rawYMatrix As Matrix)
        Dim rowsCount As Integer = rawXMatrix.rowsCount
        Dim colsCount As Integer = rawXMatrix.colunmsCount

        Dim rand As New Random(Environment.TickCount)
        Dim exchangeOne As Integer
        Dim exchangeTwo As Integer
        For i As Integer = 0 To rowsCount - 1
            exchangeOne = rand.Next(0, rowsCount)
            exchangeTwo = rand.Next(0, rowsCount)
            exchangeMatrixTwoRows(rawXMatrix, exchangeOne, exchangeTwo)
            exchangeMatrixTwoRows(rawYMatrix, exchangeOne, exchangeTwo)
        Next

    End Sub
    ''' <summary>
    ''' 对向量洗牌，打乱其中的元素
    ''' </summary>
    ''' <param name="rawVector">原始向量不会受影响</param>
    ''' <returns>返回另一个被打乱的向量</returns>
    Shared Function shuffleVector(rawVector As Vector(Of Single)) As Vector(Of Single)
        Dim resultVector As Vector(Of Single) = rawVector
        Dim lenOfRawVector As Integer = rawVector.length
        Dim rand As New Random(Environment.TickCount)
        Dim exchangeOne As Integer
        Dim exchangeTwo As Integer
        For i As Integer = 0 To lenOfRawVector - 1
            exchangeOne = rand.Next(0, lenOfRawVector)
            exchangeTwo = rand.Next(0, lenOfRawVector)
            exchangeVectorTwoItems(resultVector, exchangeOne, exchangeTwo)
        Next
        Return resultVector
    End Function
    ''' <summary>
    ''' 交换矩阵的两行
    ''' </summary>
    ''' <param name="rawMatrix">原始矩阵</param>
    ''' <param name="one">第一个位置</param>
    ''' <param name="two">第二个位置</param>
    Shared Sub exchangeMatrixTwoRows(ByRef rawMatrix As Matrix, one As Integer, two As Integer)

        Dim colsCount As Integer = rawMatrix.colunmsCount
        Dim temp As New Vector(Of Single)(colsCount, False)
        For i As Integer = 0 To colsCount - 1
            temp(i) = rawMatrix.array2D(one, i)
            rawMatrix.array2D(one, i) = rawMatrix.array2D(two, i)
            rawMatrix.array2D(two, i) = temp(i)
        Next
    End Sub
    ''' <summary>
    ''' 交换向量两个位置的元素
    ''' </summary>
    ''' <param name="rawVector">原始向量</param>
    ''' <param name="one">第一个位置</param>
    ''' <param name="two">第二个位置</param>
    Shared Sub exchangeVectorTwoItems(ByRef rawVector As Vector(Of Single), one As Integer, two As Integer)
        Dim temp As Integer = rawVector(one)
        rawVector(one) = rawVector(two)
        rawVector(two) = temp
    End Sub
    ''' <summary>
    ''' 把矩阵的某一行变成一个列向量
    ''' </summary>
    ''' <param name="rawMatrix">原始矩阵</param>
    ''' <param name="rowNum">矩阵的这个行</param>
    ''' <returns>返回一个列向量</returns>
    Shared Function matrixRowToVector(rawMatrix As Matrix, rowNum As Integer) As Vector(Of Single)
        Dim colsCount As Integer = rawMatrix.colunmsCount
        Dim resultVector As New Vector(Of Single)(colsCount, False)
        For i As Integer = 0 To colsCount - 1
            resultVector(i) = rawMatrix.array2D(rowNum, i)
        Next
        Return resultVector
    End Function

    ''' <summary>
    ''' 从数组中获取一个矩阵
    ''' </summary>
    ''' <param name="rawArray"></param>
    ''' <param name="colsNumber"></param>
    ''' <returns></returns>
    Function getMatrixFromBytesArray(rawArray() As Byte, colsNumber As Integer) As Matrix
        Dim resultMatrix As New Matrix(rawArray.Length / colsNumber, colsNumber, False)
        Dim pointer As Integer = 0
        For i As Integer = 0 To resultMatrix.rowsCount - 1
            For j As Integer = 0 To colsNumber - 1
                resultMatrix.array2D(i, j) = rawArray(pointer)
                pointer += 1
            Next
        Next
        Return resultMatrix
    End Function
    ''' <summary>
    ''' 从图片数组中获得训练矩阵
    ''' </summary>
    ''' <param name="digitImageArr">一维数组，每个元素都是一个图片</param>
    ''' <param name="imageHeight">图片的高度</param>
    ''' <param name="imageWidth">图片的宽度</param>
    ''' <returns>返回一个矩阵,都是像素，arr.length*(h*w)</returns>
    Shared Function getXMatrixFromDigitImageArr(digitImageArr() As DigitImage, imageHeight As Integer, imageWidth As Integer) As Matrix

        Dim lenOfArr As Integer = digitImageArr.Length
        Dim resultMatrix As New Matrix(lenOfArr, imageHeight * imageWidth, False)
        For i As Integer = 0 To lenOfArr - 1
            Dim count As Integer = 0
            For r As Integer = 0 To imageHeight - 1
                For c As Integer = 0 To imageWidth - 1
                    Dim b As Single = digitImageArr(i).pixels(r, c)
                    resultMatrix.array2D(i, count) = b
                    count += 1
                Next
            Next
        Next
        Return resultMatrix
    End Function
    ''' <summary>
    ''' 返回一个代表训练矩阵中的期望输出，就是图片的标签
    ''' </summary>
    ''' <param name="digitImageArr"></param>
    ''' <returns></returns>
    Shared Function getYMatrixFromDigitImageArr(digitImageArr() As DigitImage) As Matrix
        '标签表示的列是1，其他是0，比如标签是1，就会得到这样一行{0，1，0，0，0，0，0，0，0，0}
        Dim lenOfArr As Integer = digitImageArr.Length
        Dim resultMatrix As New Matrix(lenOfArr, 10, False)
        For i As Integer = 0 To lenOfArr - 1
            resultMatrix.array2D(i, digitImageArr(i).label) = 1
        Next
        Return resultMatrix
    End Function
End Class
