Imports System.Diagnostics.Stopwatch
Module test

    Sub Main()
        Dim a As New Matrix(2, 2, True)
        Dim b As New Matrix(2, 2, True)
        Dim s As New Stopwatch
        s.Start()

        Dim c As Matrix = Calculate.multiplyTwoMatrix(a, b)
        Console.Write(s.ElapsedMilliseconds)
        Console.ReadKey()

    End Sub

End Module
