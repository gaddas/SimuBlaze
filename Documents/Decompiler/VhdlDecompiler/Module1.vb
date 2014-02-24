Imports System.IO

Module Module1

    Const format As String = "attribute INIT_{0:X2} of ram_1024_x_18  : label is """
    Const formatP As String = "attribute INITP_{0:X2} of ram_1024_x_18 : label is """
    Sub Main()
        Dim values As New Queue(Of Char)
        Dim instructions As New Queue(Of Char)

        Dim sr As New StreamReader("input.vhd")

        Dim counter As Integer = 0
        Dim line As String = sr.ReadLine

        While Not line.StartsWith(String.Format(format, counter))
            line = sr.ReadLine
        End While

        counter = 0
        For i As Integer = 0 To &H3F
            line = line.Replace(String.Format(format, i), "").Replace(""";", "")
            For j As Integer = line.Length - 1 To 0 Step -1
                values.Enqueue(line(j))
            Next

            Console.WriteLine(line)
            line = sr.ReadLine
            counter += 1
        Next

        For i As Integer = 0 To &H7
            line = line.Replace(String.Format(formatP, i), "").Replace(""";", "")
            For j As Integer = line.Length - 1 To 0 Step -1
                Dim b As Byte = Val("&H" & line(j))
                instructions.Enqueue((b And 3).ToString())
                instructions.Enqueue(((b And &HC) >> 2).ToString())
            Next

            Console.WriteLine(line)
            line = sr.ReadLine
            counter += 1
        Next
        sr.Close()


        Dim sw As New StreamWriter("output.hex")
        While values.Count > 0
            sw.WriteLine("{0}{4}{3}{2}{1}", instructions.Dequeue, values.Dequeue, values.Dequeue, values.Dequeue, values.Dequeue)
        End While
        sw.Flush()
        sw.Close()
    End Sub

End Module
