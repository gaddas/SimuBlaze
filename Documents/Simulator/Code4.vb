﻿Module Code4

    Private s0 As Byte
    Private s1 As Byte
    Private s2 As Byte
    Private s3 As Byte
    Private s4 As Byte
    Private s5 As Byte
    Private s6 As Byte
    Private s7 As Byte
    Private s8 As Byte
    Private s9 As Byte
    Private sA As Byte
    Private sB As Byte
    Private sC As Byte
    Private sD As Byte
    Private sE As Byte
    Private sF As Byte

    Private CARRY As Boolean = False
    Private ZERO As Boolean = False

    Private ScratchPadMemory As New Dictionary(Of Byte, Byte)

    Private Sub FETCH(ByRef register As Byte, ByVal value As Byte)
        If ScratchPadMemory.ContainsKey(value) Then
            register = ScratchPadMemory(value)
        Else
            Throw New ApplicationException("FETCH not existing address from scratch pad memory")
        End If
    End Sub

    Private Sub STORE(ByRef register As Byte, ByVal value As Byte)
        ScratchPadMemory(value) = register
    End Sub

    Private Sub _LOAD(ByRef register As Byte, ByVal value As Byte)
        register = value
    End Sub

    Private Sub __ADD(ByRef register As Byte, ByVal value As Byte)
        CARRY = (CInt(register) + CInt(value) > Byte.MaxValue)
        register = (CInt(register) + CInt(value)) Mod (Byte.MaxValue + 1)
        ZERO = (register = 0)
    End Sub

    Private Sub ADDCY(ByRef register As Byte, ByVal value As Byte)
        Dim bonus As Integer = 0
        If CARRY Then bonus = 1

        CARRY = (CInt(register) + CInt(value) > Byte.MaxValue)
        register = (CInt(register) + CInt(value) + bonus) Mod (Byte.MaxValue + 1)
        ZERO = (register = 0)
    End Sub

    Private Sub __SUB(ByRef register As Byte, ByVal value As Byte)
        CARRY = ((CInt(register) - CInt(value)) < 0)

        If CARRY Then
            register = (&HFF + CInt(register) - CInt(value) + 1)
        Else
            register = (CInt(register) - CInt(value))
        End If

        ZERO = (register = 0)
    End Sub

    Private Sub SUBCY(ByRef register As Byte, ByVal value As Byte)
        Dim bonus As Integer = 0
        If CARRY Then bonus = 1

        CARRY = ((CInt(register) - CInt(value)) < 0)

        If CARRY Then
            register = (&HFF + CInt(register) - CInt(value) + 1 - bonus)
        Else
            register = (CInt(register) - CInt(value))
        End If

        ZERO = (register = 0)
    End Sub

    Private Sub TEST(ByRef register As Byte, ByVal value As Byte)
        Dim result As Byte = register And value
        ZERO = (result = 0)

        Dim bitCount As Byte = 0
        If result And 1 = 1 Then bitCount += 1
        If result And 2 = 2 Then bitCount += 1
        If result And 4 = 4 Then bitCount += 1
        If result And 8 = 8 Then bitCount += 1
        If result And 16 = 16 Then bitCount += 1
        If result And 32 = 32 Then bitCount += 1
        If result And 64 = 64 Then bitCount += 1
        If result And 128 = 128 Then bitCount += 1
        CARRY = (bitCount Mod 2) = 1
    End Sub

    Private RAM_RADDR1 As Byte
    Private RAM_RADDR0 As Byte
    Private RAM_WADDR0 As Byte
    Private RAM_WADDR1 As Byte
    Private RAM_DIN As Byte

    Private Sub OUTPUT(ByRef register As Byte, ByVal address As Byte)
        '75 RAM R_ADDR LSB
        '74 RAM R_ADDR МSB
        '76 RAM DOUT

        '73 RAM W_ADDR LSB
        '72 RAM W_ADDR МSB
        '71 RAM DIN
        '70 RAM WE

        Select Case address
            Case &H75
                RAM_RADDR0 = register
            Case &H74
                RAM_RADDR1 = register
            Case &H73
                RAM_WADDR0 = register
            Case &H72
                RAM_WADDR1 = register
            Case &H71
                RAM_DIN = register
            Case &H70
                If register = 1 Then
                    Ram.Write(4, RAM_WADDR0, RAM_WADDR1, RAM_DIN)
                End If
            Case LED_port
                Console.WriteLine("led_port => {0:X2}", register)

            Case tx_uart_port
                Console.WriteLine("tx_uart_port => {0:X2}", register)

            Case Else
                Throw New ApplicationException("Unhandled INPUT port address")
        End Select
    End Sub

    Private Sub INPUT(ByRef register As Byte, ByVal address As Byte)
        '75 RAM R_ADDR LSB
        '74 RAM R_ADDR МSB
        '76 RAM DIN

        '73 RAM W_ADDR LSB
        '72 RAM W_ADDR МSB
        '71 RAM DOUT
        '70 RAM WE

        Select Case address
            Case &H76
                register = Ram.Read(4, RAM_RADDR0, RAM_RADDR1)
            Case rx_uart_port
                Console.WriteLine("Reading from RX_UART_PORT:")
                register = CByte(Console.ReadLine())
            Case uart_state_port
                Console.WriteLine("Reading from UART_STATE_PORT (F = tx_full, D = data):")
                Console.WriteLine(" F = tx_full")
                Console.WriteLine(" D = rx_data_present")
                Select Case Console.ReadLine.ToUpper.Trim
                    Case "F"
                        register = tx_full
                    Case "D"
                        register = rx_data_present
                    Case Else
                        register = 0
                End Select

            Case Else
                Throw New ApplicationException("Unhandled INPUT port address")
        End Select
    End Sub


    Const rx_uart_port = &H2
    Const tx_uart_port = &H4

    Const UARTinDATA = &H11             ' клетка в рамта на процесора
    Const UARToutDATA = &H12            ' клетка в рамта на процесора
    Const uart_state_port = &H3
    Const rx_data_present = &H10
    Const tx_full = &H2
    Const SIZE = &H17
    Const LED_port = &H5                '8 simple LEDs

    'RAM constants
    Const RAMaddLSBstart = &H18
    Const RAMaddMSBstart = &H19
    Const RAMaddLSBneed = &H22
    Const RAMaddMSBneed = &H23
    Const RAMaddLSB = &H13
    Const RAMaddMSB = &H14
    Const RAMwriteDATA = &H15
    Const RAMreadDATA = &H16

    Const RAM_out_port = &H73

    Const AIJ = &HA
    Const AIK = &HB
    Const AKJ = &HC
    Const sum = &HD
    Const delay_1us_constant = &HB

    Public Sub Start()
cold_start:
warm_start:
        _LOAD(s0, &HFD)
        STORE(s0, RAMaddLSB)
        _LOAD(s0, 3)
        STORE(s0, RAMaddMSB)
        _LOAD(s0, 0)
        STORE(s0, RAMwriteDATA)
        Call writeRAM()

        'тук трябва да сме записали 1+н*н числа в паметта
        'Флойд
        _LOAD(s0, 0)
        _LOAD(s1, 0)
        STORE(s0, RAMaddLSBstart)
        STORE(s0, RAMaddMSBstart)
        __ADD(s0, 1)
        ADDCY(s1, 0)
        STORE(s0, RAMaddLSBstart)
        STORE(s1, RAMaddMSBstart)
        'we have the start address of the matrix
wait:
        _LOAD(s0, &HFD) ' това трбява да е същия вид чакане
        STORE(s0, RAMaddLSB)
        _LOAD(s0, 3)
        STORE(s0, RAMaddMSB)
        Call readRAM()
        FETCH(s0, RAMreadDATA) '
        TEST(s0, &HFF)
        Threading.Thread.Sleep(500)
        If ZERO Then GoTo wait 'JUMP(Z, wait)
        _LOAD(s0, &HFC)
        STORE(s0, RAMaddLSB)
        _LOAD(s0, 3)
        STORE(s0, RAMaddMSB)
        Call readRAM()
        FETCH(s0, RAMreadDATA) '
        STORE(s0, SIZE)
        FETCH(s5, SIZE) ' k iterations
loopk:
loopWait:
        _LOAD(s0, &HFD) ' very beginning, set HINT4 to 0
        STORE(s0, RAMaddLSB)
        _LOAD(s0, 3)
        STORE(s0, RAMaddMSB)
        Call readRAM()
        FETCH(s0, RAMreadDATA)
        TEST(s0, &HFF)
        If ZERO Then GoTo loopWait 'JUMP(Z, loopWait)

        FETCH(sB, SIZE) ' i iterations
        __SUB(sB, 1) ' i-1
        If ZERO Then GoTo loopi_end 'JUMP(Z, loopi_end)
loopi:
        FETCH(sA, SIZE) ' j iterations
        __SUB(sA, 1) ' j-1
        If ZERO Then GoTo loopj_end 'JUMP(Z, loopj_end)
loopj:
        FETCH(s0, RAMaddLSBstart)
        FETCH(s1, RAMaddMSBstart)
        __ADD(s0, sA) ' __ADD j to the start adress
        ADDCY(s1, 0) ' __ADD with carry
        __SUB(s0, 1) ' subtract 1 for offbyone
        SUBCY(s1, 0) ' subtract with carry, now in s0 we have the start + j-1
        FETCH(sC, SIZE) ' in sC we get size again
        _LOAD(sF, sB) ' in sF we get i (sB)
        __SUB(sF, 1) ' we make i-1, in order to prevent offbyone error
        _LOAD(sD, 0) ' sD 0
        _LOAD(sE, 0) ' sE 0
loopMult:
        __ADD(sD, sF) ' cycle for multiplying, we make in sD i size times (sF)
        ADDCY(sE, 0)
        __SUB(sC, 1)
        If Not ZERO Then GoTo loopMult 'JUMP(NZ, loopMult)

        __ADD(s0, sD) ' here we __ADD the mult and we have the address
        ADDCY(s1, sE)
        STORE(s0, RAMaddLSB)
        STORE(s1, RAMaddMSB)
        STORE(s0, RAMaddLSBneed)
        STORE(s1, RAMaddMSBneed)
        Call readRAM()
        FETCH(s0, RAMreadDATA) ' s0=D[i,j] address=i*SIZE+j, OK
        STORE(s0, AIJ)

        'now getting the D[i,k] element address=i*SIZE+k
        FETCH(s0, RAMaddLSBstart)
        FETCH(s1, RAMaddMSBstart)
        __ADD(s0, s5) ' __ADD k
        ADDCY(s1, 0)
        __SUB(s0, 1)
        SUBCY(s1, 0)
        FETCH(sC, SIZE)
        _LOAD(sF, sB)
        __SUB(sF, 1)
        _LOAD(sD, 0)
        _LOAD(sE, 0)
loopMult2:
        __ADD(sD, sF)
        ADDCY(sE, 0)
        __SUB(sC, 1)
        If Not ZERO Then GoTo loopMult2 'JUMP(NZ, loopMult2)

        __ADD(s0, sD)
        ADDCY(s1, sE)
        STORE(s0, RAMaddLSB)
        STORE(s1, RAMaddMSB)
        Call readRAM()
        FETCH(s0, RAMreadDATA) ' D[i,k] element address=i*SIZE+k
        STORE(s0, AIK)

        'now getting the D[k,j] element address=k*SIZE+j
        FETCH(s0, RAMaddLSBstart)
        FETCH(s1, RAMaddMSBstart)
        __ADD(s0, sA)
        ADDCY(s1, 0)
        __SUB(s0, 1)
        __SUB(s1, 0)
        FETCH(sC, SIZE)
        _LOAD(sF, s5)
        __SUB(sF, 1)
        _LOAD(sD, 0)
        _LOAD(sE, 0)
loopMult3:
        __ADD(sD, sF)
        ADDCY(sE, 0)
        __SUB(sC, 1)
        If Not ZERO Then GoTo loopMult3 'JUMP(NZ, loopMult3)

        __ADD(s0, sD)
        ADDCY(s1, sE)
        STORE(s0, RAMaddLSB)
        STORE(s1, RAMaddMSB)
        Call readRAM()
        FETCH(s0, RAMreadDATA) ' s0=D[i,j] address=i*SIZE+j
        STORE(s0, AKJ)

        ' now we have all of them and we have to compare AIJ with (AIK+AKJ)					
        FETCH(s0, AKJ)
        FETCH(s1, AIK)
        __ADD(s0, s1) ' s0=(AIK+AKJ)
        If Not CARRY Then GoTo OK 'JUMP(NC, OK)
        _LOAD(s0, &HFF)

OK:
        STORE(s0, sum)
        FETCH(s1, AIJ)
        __SUB(s0, s1)
        If CARRY Then Call replace() 'call C, replace
        __SUB(sA, 1) ' j
        If ZERO Then GoTo loopj_end 'JUMP(Z, loopj_end)
        __SUB(sA, 1)
        If Not ZERO Then GoTo loopj 'JUMP(NZ, loopj)
loopj_end:
        __SUB(sB, 1) 'i
        If ZERO Then GoTo loopi_end 'JUMP(Z, loopi_end)
        __SUB(sB, 1)
        If Not ZERO Then GoTo loopi 'JUMP(NZ, loopi)
loopi_end:
        _LOAD(s0, &HFD) 'end of k iteration waiting for HINT4 to be 0
        STORE(s0, RAMaddLSB)
        _LOAD(s0, 3)
        STORE(s0, RAMaddMSB)
        _LOAD(s0, 0)
        STORE(s0, RAMwriteDATA)
        Call writeRAM() ' set HINT4 to 0
        __SUB(s5, 1)
        If Not ZERO Then GoTo loopk 'JUMP(NZ, loopk)
        GoTo warm_start 'JUMP(warm_start)
    End Sub

    Public Sub readRAM()
readRAM:
        FETCH(s6, RAMaddLSB)
        FETCH(s7, RAMaddMSB)
        OUTPUT(s6, &H75) ' запис на съдържанието на s6 в изходен порт 75 (LSB на адреса)
        OUTPUT(s7, &H74) ' запис на съдържанието на s7 в изходен порт 74 (МSB на адреса)
        INPUT(s8, &H76) ' прочитане на входен порт 76 в регистър s8
        STORE(s8, RAMreadDATA)
        Return
    End Sub

    Public Sub writeRAM()
writeRAM:
        FETCH(sC, RAMaddLSB) ' зареждане на регистър sC със стойността на LSB на адреса
        FETCH(sD, RAMaddMSB) ' зареждане на регистър sD със стойността на МSB на адреса 
        OUTPUT(sC, &H73) ' запис на съдържанието на sС в изходен порт 73 (LSB на адреса)
        OUTPUT(sD, &H72) ' запис на съдържанието на sD в изходен порт 72 (МSB на адреса)
        FETCH(s9, RAMwriteDATA)
        OUTPUT(s9, &H71) ' запис на съдържанието на s9 в изходен порт 71 (данни)
        _LOAD(sE, 1) ' запис на команда за запис в регистър Е (RAM_WRITE_STROBE) 1
        OUTPUT(sE, &H70) ' издаване на командата към паметта (RAM_WRITE_STROBE = 1)
        _LOAD(sE, 0) ' нулиране съдържанието на sЕ
        OUTPUT(sE, &H70) ' RAM_WRITE_STROBE = 0
        Return
    End Sub

    Public Sub read()
read:
        'INPUT(s0, uart_state_port)
        'OUTPUT(s0, LED_port)
        'TEST(s0, rx_data_present)
        'If ZERO Then GoTo read 'JUMP(Z, read)
        INPUT(s1, rx_uart_port)
        STORE(s1, UARTinDATA)
        Return
    End Sub

    Public Sub write()
write:
        INPUT(s0, uart_state_port)
        OUTPUT(s0, LED_port)
        TEST(s0, tx_full)
        If Not ZERO Then GoTo write 'JUMP(nZ, write)
        FETCH(s1, UARToutDATA)
        OUTPUT(s1, tx_uart_port)
        Return
    End Sub

    Public Sub replace()
replace:
        FETCH(s0, RAMaddLSBneed)
        FETCH(s1, RAMaddMSBneed)
        STORE(s0, RAMaddLSB)
        STORE(s1, RAMaddMSB)
        FETCH(s0, sum)
        STORE(s0, RAMwriteDATA)
        Call writeRAM()
        Return
    End Sub

End Module
