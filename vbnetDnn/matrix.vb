<Serializable>
Public Class Matrix
    ''' <summary>
    ''' 行数
    ''' </summary>
    Private _rowsCount As Integer
    Public Property rowsCount() As Integer
        Get
            Return _rowsCount
        End Get
        Set(ByVal value As Integer)
            _rowsCount = value
        End Set
    End Property
    ''' <summary>
    ''' 列数
    ''' </summary>
    Private _columnsCount As Integer
    Public Property colunmsCount() As Integer
        Get
            Return _columnsCount
        End Get
        Set(ByVal value As Integer)
            _columnsCount = value
        End Set
    End Property

    ''' <summary>
    ''' 内部的二维数组
    ''' </summary>
    Private _array2D(,) As Single
    Default Public Property array2D(rowIndex As Integer, columnIndex As Integer) As Single
        Get
            Return _array2D(rowIndex, columnIndex)
        End Get
        Set(ByVal value As Single)
            _array2D(rowIndex, columnIndex) = value
        End Set
    End Property
    ''' <summary>
    ''' 构造函数
    ''' </summary>
    ''' <param name="m_rowsCount">行数</param>
    ''' <param name="m_colunmsCount">列数</param>
    ''' <param name="isRand">如果这个是True就用随机数来填充这个矩阵</param>
    Sub New(m_rowsCount As Integer, m_colunmsCount As Integer, isRand As Boolean)
        Me.rowsCount = m_rowsCount
        Me.colunmsCount = m_colunmsCount
        Dim tempArray2D(rowsCount - 1, colunmsCount - 1) As Single
        _array2D = tempArray2D
        If isRand = True Then
            Dim rand As New Random(Guid.NewGuid.GetHashCode)

            For i As Integer = 0 To m_rowsCount - 1
                For j As Integer = 0 To m_colunmsCount - 1
                    '用box muller算法生成一个标准正态分布
                    Dim u1 As Single = rand.NextDouble()
                    Dim u2 As Single = rand.NextDouble()
                    Dim z As Single = Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2)
                    Me.array2D(i, j) = z
                    'Me.array2D(i, j) = 0
                Next
            Next
        End If

    End Sub
    Sub New(rawArr(,) As Single)
        Dim colsUpper As Integer = rawArr.GetUpperBound(1)
        Dim rowsUpper As Integer = rawArr.GetUpperBound(0)
        For i As Integer = 0 To rowsUpper
            For j As Integer = 0 To colsUpper
                Me.array2D(i, j) = rawArr(i, j)
            Next
        Next
    End Sub
End Class



