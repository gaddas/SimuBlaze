Imports System.IO.Ports

Module Com

    Public ReadOnly Property IsDataAvailable() As Boolean
        Get
            Return Data.Count > 0
        End Get
    End Property
    Public Data As New Queue(Of Byte)

    Private port As SerialPort

    Sub New()
        port = New SerialPort("COM2", 9600, Parity.None, 8, StopBits.One)
        AddHandler port.DataReceived, AddressOf OnPortData

        port.Open()
    End Sub

    Public Sub Send(ByVal b As Byte)
        Console.WriteLine("Writing to com: {0}", b)

        Dim buffer As Byte() = New Byte() {b}
        port.Write(buffer, 0, 1)
    End Sub

    Private Sub OnPortData(ByVal sender As Object, ByVal e As SerialDataReceivedEventArgs)
        Dim bytes As Integer = port.BytesToRead

        For i As Integer = 0 To bytes - 1
            Data.Enqueue(port.ReadByte())
        Next

    End Sub


End Module
