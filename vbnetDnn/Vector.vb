<Serializable>
Public Class Vector(Of T) '里面用到了向量类
    Private _array As T()
    Default Public Property VectorValues(index As Integer) As T
        Get
            Return _array(index)
        End Get
        Set(ByVal value As T)
            _array(index) = value
        End Set
    End Property

    Private _length As Integer
    Public ReadOnly Property length() As Integer
        Get
            Return _length
        End Get

    End Property
    ''' <summary>
    ''' 构造函数
    ''' </summary>
    ''' <param name="count">指定向量内部的数组</param>

    Sub New(ParamArray count() As T)
        _length = count.Length
        _array = count


    End Sub
    ''' <summary>
    ''' 返回向量中最大元素所在的下标
    ''' </summary>
    ''' <returns></returns>
    Function getMaxIndex() As Integer
        Dim max As Single = Single.MinValue
        Dim index As Integer = 0
        For i As Integer = 0 To Me.length - 1
            If Single.Parse(Me(i).ToString) > max Then
                max = Single.Parse(Me(i).ToString)
                index = i
            End If
        Next
        Return index
    End Function
    ''' <summary>
    ''' 构造函数
    ''' </summary>
    ''' <param name="m_lenOfVector">向量长度</param>
    ''' <param name="isRand">如果这个参数是True那么用随机数来填充这个向量</param>
    Sub New(m_lenOfVector As Integer, isRand As Boolean)
        _length = m_lenOfVector
        Dim tempArray(m_lenOfVector - 1) As T
        If isRand = True Then
            Dim rand As New Random(Guid.NewGuid.GetHashCode)
            '这里先用均匀分布代替了
            For i As Integer = 0 To m_lenOfVector - 1
                '用box muller算法生成一个标准正态分布
                Dim u1 As Single = rand.NextDouble()
                Dim u2 As Single = rand.NextDouble()
                Dim z As Single = Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2)
                tempArray(i) = Convert.ChangeType(z, GetType(T))
                'tempArray(i) = Convert.ChangeType(0, GetType(T))
            Next
        End If
        _array = tempArray

    End Sub

End Class
