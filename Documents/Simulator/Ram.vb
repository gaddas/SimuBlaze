Module Ram

    Private RAM As New Dictionary(Of Integer, Byte)

    Public Function Read(ByVal core As Integer, ByVal address0 As Byte, ByVal address1 As Byte) As Byte
        Dim address As Integer = (CInt(address1) << 8) + address0

        If RAM.ContainsKey(address) Then
            'Console.WriteLine("Core {2} read at {0:X4} values {1:X2}", address, RAM(address), core)
            Return RAM(address)
        Else
            'Console.WriteLine("Core {2} at {0:X4} unknown value", address, 0, core)
            'Throw New ApplicationException("RAM Address is free")
            Return 0
        End If
    End Function

    Public Sub Write(ByVal core As Integer, ByVal address0 As Byte, ByVal address1 As Byte, ByVal data As Byte)
        Dim address As Integer = (CInt(address1) << 8) + address0
        RAM(address) = data

        Console.WriteLine("Core {2} write at {0:X4} values {1:X2}", address, data, core)
    End Sub

End Module
