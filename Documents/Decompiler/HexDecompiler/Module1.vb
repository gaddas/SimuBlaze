Imports System.IO
Imports System.Text

Module Module1

    Sub Main()
        Dim sr As New StreamReader("input.hex")
        Dim sw As New StreamWriter("output.psm")
        Dim line As String

        While Not sr.EndOfStream
            line = sr.ReadLine
            line = ParseInstructions(line)

            Dim sx_decode As String = "s" & ParseHex(line.Substring(6, 4))
            Dim sy_decode As String = "s" & ParseHex(line.Substring(10, 4))
            Dim kk_decode As String = ParseHex(line.Substring(10, 4)) & ParseHex(line.Substring(14, 4))
            Dim aaa_decode As String = ParseHex("00" & line.Substring(8, 2)) & ParseHex(line.Substring(10, 4)) & ParseHex(line.Substring(14, 4))

            Select Case line.Substring(0, 6)
                Case "000000"
                    sw.WriteLine("LOAD {0} {1}", sx_decode, kk_decode)
                    '=> kcpsm3_opcode := "LOAD " & sx_decode & ',' & kk_decode & "         ";
                Case "000001"
                    sw.WriteLine("LOAD {0} {1}", sx_decode, sy_decode)
                    '=> kcpsm3_opcode := "LOAD " & sx_decode & ',' & sy_decode & "         ";
                Case "001010"
                    sw.WriteLine("AND {0} {1}", sx_decode, kk_decode)
                    '=> kcpsm3_opcode := "AND " & sx_decode & ',' & kk_decode & "          ";
                Case "001011"
                    sw.WriteLine("AND {0} {1}", sx_decode, sy_decode)
                    '=> kcpsm3_opcode := "AND " & sx_decode & ',' & sy_decode & "          ";
                Case "001100"
                    sw.WriteLine("OR {0} {1}", sx_decode, kk_decode)
                    '=> kcpsm3_opcode := "OR " & sx_decode & ',' & kk_decode & "           ";
                Case "001101"
                    sw.WriteLine("OR {0} {1}", sx_decode, sy_decode)
                    '=> kcpsm3_opcode := "OR " & sx_decode & ',' & sy_decode & "           ";
                Case "001110"
                    sw.WriteLine("XOR {0} {1}", sx_decode, kk_decode)
                    '=> kcpsm3_opcode := "XOR " & sx_decode & ',' & kk_decode & "          ";
                Case "001111"
                    sw.WriteLine("XOR {0} {1}", sx_decode, sy_decode)
                    '=> kcpsm3_opcode := "XOR " & sx_decode & ',' & sy_decode & "          ";
                Case "010010"
                    sw.WriteLine("TEST {0} {1}", sx_decode, kk_decode)
                    '=> kcpsm3_opcode := "TEST " & sx_decode & ',' & kk_decode & "         ";
                Case "010011"
                    sw.WriteLine("TEST {0} {1}", sx_decode, sy_decode)
                    '=> kcpsm3_opcode := "TEST " & sx_decode & ',' & sy_decode & "         ";
                Case "011000"
                    sw.WriteLine("ADD {0} {1}", sx_decode, kk_decode)
                    '=> kcpsm3_opcode := "ADD " & sx_decode & ',' & kk_decode & "          ";
                Case "011001"
                    sw.WriteLine("ADD {0} {1}", sx_decode, sy_decode)
                    '=> kcpsm3_opcode := "ADD " & sx_decode & ',' & sy_decode & "          ";
                Case "011010"
                    sw.WriteLine("ADDCY {0} {1}", sx_decode, kk_decode)
                    '=> kcpsm3_opcode := "ADDCY " & sx_decode & ',' & kk_decode & "        ";
                Case "011011"
                    sw.WriteLine("ADDCY {0} {1}", sx_decode, sy_decode)
                    '=> kcpsm3_opcode := "ADDCY " & sx_decode & ',' & sy_decode & "        ";
                Case "011100"
                    sw.WriteLine("SUB {0} {1}", sx_decode, kk_decode)
                    '=> kcpsm3_opcode := "SUB " & sx_decode & ',' & kk_decode & "          ";
                Case "011101"
                    sw.WriteLine("SUB {0} {1}", sx_decode, sy_decode)
                    '=> kcpsm3_opcode := "SUB " & sx_decode & ',' & sy_decode & "          ";
                Case "011110"
                    sw.WriteLine("SUBCY {0} {1}", sx_decode, kk_decode)
                    '=> kcpsm3_opcode := "SUBCY " & sx_decode & ',' & kk_decode & "        ";
                Case "011111"
                    sw.WriteLine("SUBCY  {0} {1}", sx_decode, sy_decode)
                    '=> kcpsm3_opcode := "SUBCY " & sx_decode & ',' & sy_decode & "        ";
                Case "010100"
                    sw.WriteLine("COMPARE {0} {1}", sx_decode, kk_decode)
                    '=> kcpsm3_opcode := "COMPARE " & sx_decode & ',' & kk_decode & "      ";
                Case "010101"
                    sw.WriteLine("COMPARE {0} {1}", sx_decode, sy_decode)
                    '=> kcpsm3_opcode := "COMPARE " & sx_decode & ',' & sy_decode & "      ";

                Case "100000"
                    Select Case line.Substring(14, 4)
                        Case "0110"
                            sw.WriteLine("SL0 {0}", sx_decode)
                            '=> kcpsm3_opcode := "SL0 " & sx_decode & "             ";
                        Case "0111"
                            sw.WriteLine("SL1 {0}", sx_decode)
                            '=> kcpsm3_opcode := "SL1 " & sx_decode & "             ";
                        Case "0100"
                            sw.WriteLine("SLX {0}", sx_decode)
                            '=> kcpsm3_opcode := "SLX " & sx_decode & "             ";
                        Case "0000"
                            sw.WriteLine("SLA {0}", sx_decode)
                            '=> kcpsm3_opcode := "SLA " & sx_decode & "             ";
                        Case "0010"
                            sw.WriteLine("RL {0}", sx_decode)
                            '=> kcpsm3_opcode := "RL " & sx_decode & "              ";
                        Case "1110"
                            sw.WriteLine("SR0 {0}", sx_decode)
                            '=> kcpsm3_opcode := "SR0 " & sx_decode & "             ";
                        Case "1111"
                            sw.WriteLine("SR1 {0}", sx_decode)
                            '=> kcpsm3_opcode := "SR1 " & sx_decode & "             ";
                        Case "1010"
                            sw.WriteLine("SRX {0}", sx_decode)
                            '=> kcpsm3_opcode := "SRX " & sx_decode & "             ";
                        Case "1000"
                            sw.WriteLine("SRA {0}", sx_decode)
                            '=> kcpsm3_opcode := "SRA " & sx_decode & "             ";
                        Case "1100"
                            sw.WriteLine("RR {0}", sx_decode)
                            '=> kcpsm3_opcode := "RR " & sx_decode & "              ";
                    End Select

                Case "101100"
                    sw.WriteLine("OUTPUT {0} {1}", sx_decode, kk_decode)
                    '=> kcpsm3_opcode := "OUTPUT " & sx_decode & ',' & kk_decode & "       ";
                Case "101101"
                    sw.WriteLine("OUTPUT {0} {1}", sx_decode, sy_decode)
                    '=> kcpsm3_opcode := "OUTPUT " & sx_decode & ",(" & sy_decode & ")     ";
                Case "000100"
                    sw.WriteLine("INPUT {0} {1}", sx_decode, kk_decode)
                    '=> kcpsm3_opcode := "INPUT " & sx_decode & ',' & kk_decode & "        ";
                Case "000101"
                    sw.WriteLine("INPUT {0} {1}", sx_decode, sy_decode)
                    '=> kcpsm3_opcode := "INPUT " & sx_decode & ",(" & sy_decode & ")      ";
                Case "101110"
                    sw.WriteLine("STORE {0} {1}", sx_decode, kk_decode)
                    '=> kcpsm3_opcode := "STORE " & sx_decode & ',' & kk_decode & "        ";
                Case "101111"
                    sw.WriteLine("STORE {0} {1}", sx_decode, sy_decode)
                    '=> kcpsm3_opcode := "STORE " & sx_decode & ",(" & sy_decode & ")      ";
                Case "000110"
                    sw.WriteLine("FETCH {0} {1}", sx_decode, kk_decode)
                    '=> kcpsm3_opcode := "FETCH " & sx_decode & ',' & kk_decode & "        ";
                Case "000111"
                    sw.WriteLine("FETCH {0} {1}", sx_decode, sy_decode)
                    '=> kcpsm3_opcode := "FETCH " & sx_decode & ",(" & sy_decode & ")      ";
                Case "110100"
                    sw.WriteLine("JUMP {0}", aaa_decode)
                    '=> kcpsm3_opcode := "JUMP " & aaa_decode & "           ";
                Case "110101"
                    Select Case line.Substring(6, 2)
                        Case "00"
                            sw.WriteLine("JUMP Z, {0}", aaa_decode)
                            '=> kcpsm3_opcode := "JUMP Z," & aaa_decode & "         ";
                        Case "01"
                            sw.WriteLine("JUMP NZ, {0}", aaa_decode)
                            '=> kcpsm3_opcode := "JUMP NZ," & aaa_decode & "        ";
                        Case "10"
                            sw.WriteLine("JUMP C, {0}", aaa_decode)
                            '=> kcpsm3_opcode := "JUMP C," & aaa_decode & "         ";
                        Case "11"
                            sw.WriteLine("JUMP MC, {0}", aaa_decode)
                            '=> kcpsm3_opcode := "JUMP NC," & aaa_decode & "        ";
                    End Select

                Case "110000"
                    sw.WriteLine("CALL {0}", aaa_decode)
                    '=> kcpsm3_opcode := "CALL " & aaa_decode & "           ";
                Case "110001"
                    Select Case line.Substring(6, 2)
                        Case "00"
                            sw.WriteLine("CALL Z, {0}", aaa_decode)
                            '=> kcpsm3_opcode := "CALL Z," & aaa_decode & "         ";
                        Case "01"
                            sw.WriteLine("CALL NZ, {0}", aaa_decode)
                            '=> kcpsm3_opcode := "CALL NZ," & aaa_decode & "        ";
                        Case "10"
                            sw.WriteLine("CALL C, {0}", aaa_decode)
                            '=> kcpsm3_opcode := "CALL C," & aaa_decode & "         ";
                        Case "11"
                            sw.WriteLine("CALL NC, {0}", aaa_decode)
                            '=> kcpsm3_opcode := "CALL NC," & aaa_decode & "        ";
                    End Select

                Case "101010"
                    sw.WriteLine("RETURN")
                    '=> kcpsm3_opcode := "RETURN             ";
                Case "101011"
                    Select Case line.Substring(6, 2)
                        Case "00"
                            sw.WriteLine("RETURN Z")
                            '=> kcpsm3_opcode := "RETURN Z           ";
                        Case "01"
                            sw.WriteLine("RETURN NZ")
                            '=> kcpsm3_opcode := "RETURN NZ          ";
                        Case "10"
                            sw.WriteLine("RETURN C")
                            '=> kcpsm3_opcode := "RETURN C           ";
                        Case "11"
                            sw.WriteLine("RETURN NC")
                            '=> kcpsm3_opcode := "RETURN NC          ";
                    End Select

                Case "111000" '=>
                    Select Case line.Substring(17, 1)
                        Case "0"
                            sw.WriteLine("RETURNI DISABLE")
                            '=> kcpsm3_opcode := "RETURNI DISABLE    ";
                        Case "1" '
                            sw.WriteLine("RETURNI ENABLE")
                            '=> kcpsm3_opcode := "RETURNI ENABLE     ";
                    End Select

                Case "111100" '=>
                    Select Case line.Substring(17, 1)
                        Case "0"
                            sw.WriteLine("DISABLE INTERRUPT")
                            '=> kcpsm3_opcode := "DISABLE INTERRUPT  ";
                        Case "1"
                            sw.WriteLine("ENABLE INTERRUPT")
                            ' => kcpsm3_opcode := "ENABLE INTERRUPT   ";
                    End Select

                Case Else
                    sw.WriteLine(line)
            End Select



        End While
        sr.Close()
        sw.Flush()
        sw.Close()

        sr = New StreamReader("output.psm")
        sw = New StreamWriter("output.log")

        Dim counter As Integer = 0
        While Not sr.EndOfStream
            line = sr.ReadLine()
            sw.WriteLine(String.Format(" {0:X3}                     {1}", counter, line))
            counter += 1
        End While

        sw.Flush()
        sw.Close()
        sr.Close()
    End Sub

    Public Function ParseInstructions(ByVal hexString As String) As String
        Dim result As New StringBuilder

        Dim bits0to2 As Byte = Byte.Parse(hexString.Substring(0, 1), Globalization.NumberStyles.HexNumber)
        Dim bits1 As Byte = Byte.Parse(hexString.Substring(1, 2), Globalization.NumberStyles.HexNumber)
        Dim bits2 As Byte = Byte.Parse(hexString.Substring(3, 2), Globalization.NumberStyles.HexNumber)

        result.Append((bits0to2 And 2) >> 1)
        result.Append((bits0to2 And 1) >> 0)

        result.Append((bits1 And 128) >> 7)
        result.Append((bits1 And 64) >> 6)
        result.Append((bits1 And 32) >> 5)
        result.Append((bits1 And 16) >> 4)
        result.Append((bits1 And 8) >> 3)
        result.Append((bits1 And 4) >> 2)
        result.Append((bits1 And 2) >> 1)
        result.Append((bits1 And 1) >> 0)

        result.Append((bits2 And 128) >> 7)
        result.Append((bits2 And 64) >> 6)
        result.Append((bits2 And 32) >> 5)
        result.Append((bits2 And 16) >> 4)
        result.Append((bits2 And 8) >> 3)
        result.Append((bits2 And 4) >> 2)
        result.Append((bits2 And 2) >> 1)
        result.Append((bits2 And 1) >> 0)

        Return result.ToString()
    End Function
       
    Public Function ParseHex(ByVal s As String) As Char
        Select Case s
            Case "0000"
                Return "0"
            Case "0001"
                Return "1"
            Case "0010"
                Return "2"
            Case "0011"
                Return "3"
            Case "0100"
                Return "4"
            Case "0101"
                Return "5"
            Case "0110"
                Return "6"
            Case "0111"
                Return "7"
            Case "1000"
                Return "8"
            Case "1001"
                Return "9"
            Case "1010"
                Return "A"
            Case "1011"
                Return "B"
            Case "1100"
                Return "C"
            Case "1101"
                Return "D"
            Case "1110"
                Return "E"
            Case "1111"
                Return "F"
        End Select

    End Function

End Module
