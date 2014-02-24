Module Module1

    Sub Main()
        Dim c2 As New Threading.Thread(AddressOf Code2.Start)
        Dim c3 As New Threading.Thread(AddressOf Code3.Start)
        Dim c4 As New Threading.Thread(AddressOf Code4.Start)

        c2.Start()
        c3.Start()
        c4.Start()
        Code1.Start()

    End Sub

End Module
