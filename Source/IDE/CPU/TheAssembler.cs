using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDE.CPU.Commands;
using System.IO;

namespace IDE.CPU
{
    public class TheAssembler
    {
        public byte[,] INIT;
        public byte[,] INITP;

        public TheCpu CPU;

        public Dictionary<int, int> LineMappings;

        public void Instruction(int address, string bits)
        {
            var bitsINITP = bits.Substring(0, 2);
            var bitsINIT1 = bits.Substring(2, 8);
            var bitsINIT2 = bits.Substring(10, 8);

            var byteINITP = BitsToByte2(bitsINITP);
            var byteINIT1 = BitsToByte8(bitsINIT1);
            var byteINIT2 = BitsToByte8(bitsINIT2);

            INIT[address / 16, (address % 16) * 2 + 0] = byteINIT2;
            INIT[address / 16, (address % 16) * 2 + 1] = byteINIT1;

            //INITP[(address % 4) / 32, (address % 4) % 32] |= (byte)(byteINITP << ((address % 4) * 2));

            var address2 = address / 4;
            switch (address % 4)
            {
                case 0:
                    INITP[address2 / 32, address2 % 32] |= (byte)(byteINITP << 0);
                    break;
                case 1:
                    INITP[address2 / 32, address2 % 32] |= (byte)(byteINITP << 2);
                    break;
                case 2:
                    INITP[address2 / 32, address2 % 32] |= (byte)(byteINITP << 4);
                    break;
                case 3:
                    INITP[address2 / 32, address2 % 32] |= (byte)(byteINITP << 6);
                    break;
            }
        }

        // 0 to 1024 (64*16)
        public int address = 0;

        public TheAssembler(TheCpu CPU)
        {
            this.INIT = new byte[64, 32];
            this.INITP = new byte[8, 32];

            this.CPU = CPU;
            this.LineMappings = new Dictionary<int, int>();
        }

        #region [Methods for HDL]

        private string[] ParseHdlToHex(string inputFilename, string format, string formatp, string formate)
        {
            var values = new Queue<char>();
            var instructions = new Queue<char>();

            using (var sr = new StreamReader(inputFilename))
            {
                var line = "";

                while (!line.StartsWith(string.Format(format, 0)))
                {
                    line = sr.ReadLine();
                }

                for (int i = 0; i <= 0x3F; i++)
                {
                    line = line.Replace(string.Format(format, i), "").Replace(formate, "");
                    for (int j = line.Length - 1; j >= 0; j--)
                    {
                        values.Enqueue(line[j]);
                    }
                    line = sr.ReadLine();
                }

                for (int i = 0; i <= 0x07; i++)
                {
                    line = line.Replace(string.Format(formatp, i), "").Replace(formate, "");
                    for (int j = line.Length - 1; j >= 0; j--)
                    {
                        byte b = byte.Parse("" + line[j], System.Globalization.NumberStyles.HexNumber);
                        instructions.Enqueue((b & 0x3).ToString()[0]);
                        instructions.Enqueue(((b & 0xC) >> 2).ToString()[0]);
                    }
                    line = sr.ReadLine();
                }
            }

            var result = new List<string>();
            while (values.Count > 0)
            {
                result.Add(string.Format("{0}{4}{3}{2}{1}\n", instructions.Dequeue(), values.Dequeue(), values.Dequeue(), values.Dequeue(), values.Dequeue()));
            }
            return result.ToArray();
        }

        private string[] ParseVhdlToHex(string inputFilename)
        {
            var format = @"attribute INIT_{0:X2} of ram_1024_x_18  : label is """;
            var formatp = @"attribute INITP_{0:X2} of ram_1024_x_18 : label is """;
            var formate = @""";";

            return ParseHdlToHex(inputFilename, format, formatp, formate);
        }

        private string[] ParseVerilogToHex(string inputFilename)
        {
            var format = @"defparam ram_1024_x_18.INIT_{0:X2}  = 256'h";
            var formatp = @"defparam ram_1024_x_18.INITP_{0:X2} = 256'h";
            var formate = @";";

            return ParseHdlToHex(inputFilename, format, formatp, formate);
        }

        #endregion
        #region [Methods]

        private void WriteCommand(StreamWriter sw, string format, params string[] args)
        {
            var label = string.Format("{0:X3}:   ", this.address);
            var command = string.Format(format, args);

            sw.WriteLine(label + command);

            this.address++;
        }

        public string Disassemble(string inputFilename, string outputFilename)
        {
            this.address = 0;
            string[] hexLines = null;

            try
            {
                if (inputFilename.ToUpper().EndsWith(".VHD"))
                {
                    hexLines = ParseVhdlToHex(inputFilename);
                }
                else
                {
                    hexLines = ParseVerilogToHex(inputFilename);
                }
            }
            catch
            {
                return "Невалиден файл";
            }

            using (var sw = new StreamWriter(outputFilename, false))
            {
                foreach (var line in hexLines)
                {
                    var bits = HexToBits18(line);

                    var sX = "s" + BitsToHex4(bits.Substring(6, 4));
                    var sY = "s" + BitsToHex4(bits.Substring(10, 4));
                    var kk = BitsToHex4(bits.Substring(10, 4)) + BitsToHex4(bits.Substring(14, 4));
                    var aaa = BitsToHex4("00" + bits.Substring(8, 2)) + BitsToHex4(bits.Substring(10, 4)) + BitsToHex4(bits.Substring(14, 4));

                    switch (bits.Substring(0, 6))
                    {
                        //case "000000000000000000":
                        //    this.WriteCommand(sw, "");
                        //    break;
                        case "000000":
                            this.WriteCommand(sw, "LOAD {0}, {1}", sX, kk);
                            break;
                        case "000001":
                            this.WriteCommand(sw, "LOAD {0}, {1}", sX, sY);
                            break;
                        case "001010":
                            this.WriteCommand(sw, "AND {0}, {1}", sX, kk);
                            break;
                        case "001011":
                            this.WriteCommand(sw, "AND {0}, {1}", sX, sY);
                            break;
                        case "001100":
                            this.WriteCommand(sw, "OR {0}, {1}", sX, kk);
                            break;
                        case "001101":
                            this.WriteCommand(sw, "OR {0}, {1}", sX, sY);
                            break;
                        case "001110":
                            this.WriteCommand(sw, "XOR {0}, {1}", sX, kk);
                            break;
                        case "001111":
                            this.WriteCommand(sw, "XOR {0}, {1}", sX, sY);
                            break;
                        case "010010":
                            this.WriteCommand(sw, "TEST {0}, {1}", sX, kk);
                            break;
                        case "010011":
                            this.WriteCommand(sw, "TEST {0}, {1}", sX, sY);
                            break;
                        case "011000":
                            this.WriteCommand(sw, "ADD {0}, {1}", sX, kk);
                            break;
                        case "011001":
                            this.WriteCommand(sw, "ADD {0}, {1}", sX, sY);
                            break;
                        case "011010":
                            this.WriteCommand(sw, "ADDCY {0}, {1}", sX, kk);
                            break;
                        case "011011":
                            this.WriteCommand(sw, "ADDCY {0}, {1}", sX, sY);
                            break;
                        case "011100":
                            this.WriteCommand(sw, "SUB {0}, {1}", sX, kk);
                            break;
                        case "011101":
                            this.WriteCommand(sw, "SUB {0}, {1}", sX, sY);
                            break;
                        case "011110":
                            this.WriteCommand(sw, "SUBCY {0}, {1}", sX, kk);
                            break;
                        case "011111":
                            this.WriteCommand(sw, "SUBCY  {0}, {1}", sX, sY);
                            break;
                        case "010100":
                            this.WriteCommand(sw, "COMPARE {0}, {1}", sX, kk);
                            break;
                        case "010101":
                            this.WriteCommand(sw, "COMPARE {0}, {1}", sX, sY);
                            break;
                        case "100000":
                            switch (bits.Substring(14, 4))
                            {
                                case "0110":
                                    this.WriteCommand(sw, "SL0 {0}", sX);
                                    break;
                                case "0111":
                                    this.WriteCommand(sw, "SL1 {0}", sX);
                                    break;
                                case "0100":
                                    this.WriteCommand(sw, "SLX {0}", sX);
                                    break;
                                case "0000":
                                    this.WriteCommand(sw, "SLA {0}", sX);
                                    break;
                                case "0010":
                                    this.WriteCommand(sw, "RL {0}", sX);
                                    break;
                                case "1110":
                                    this.WriteCommand(sw, "SR0 {0}", sX);
                                    break;
                                case "1111":
                                    this.WriteCommand(sw, "SR1 {0}", sX);
                                    break;
                                case "1010":
                                    this.WriteCommand(sw, "SRX {0}", sX);
                                    break;
                                case "1000":
                                    this.WriteCommand(sw, "SRA {0}", sX);
                                    break;
                                case "1100":
                                    this.WriteCommand(sw, "RR {0}", sX);
                                    break;
                            }
                            break;
                        case "101100":
                            this.WriteCommand(sw, "OUTPUT {0}, {1}", sX, kk);
                            break;
                        case "101101":
                            this.WriteCommand(sw, "OUTPUT {0}, ({1})", sX, sY);
                            break;
                        case "000100":
                            this.WriteCommand(sw, "INPUT {0}, {1}", sX, kk);
                            break;
                        case "000101":
                            this.WriteCommand(sw, "INPUT {0}, ({1})", sX, sY);
                            break;
                        case "101110":
                            this.WriteCommand(sw, "STORE {0}, {1}", sX, kk);
                            break;
                        case "101111":
                            this.WriteCommand(sw, "STORE {0}, ({1})", sX, sY);
                            break;
                        case "000110":
                            this.WriteCommand(sw, "FETCH {0}, {1}", sX, kk);
                            break;
                        case "000111":
                            this.WriteCommand(sw, "FETCH {0}, ({1})", sX, sY);
                            break;
                        case "110100":
                            this.WriteCommand(sw, "JUMP {0}", aaa);
                            break;
                        case "110101":
                            switch (bits.Substring(6, 2))
                            {
                                case "00":
                                    this.WriteCommand(sw, "JUMP Z, {0}", aaa);
                                    break;
                                case "01":
                                    this.WriteCommand(sw, "JUMP NZ, {0}", aaa);
                                    break;
                                case "10":
                                    this.WriteCommand(sw, "JUMP C, {0}", aaa);
                                    break;
                                case "11":
                                    this.WriteCommand(sw, "JUMP MC, {0}", aaa);
                                    break;
                            }
                            break;
                        case "110000":
                            this.WriteCommand(sw, "CALL {0}", aaa);
                            break;
                        case "110001":
                            switch (bits.Substring(6, 2))
                            {
                                case "00":
                                    this.WriteCommand(sw, "CALL Z, {0}", aaa);
                                    break;
                                case "01":
                                    this.WriteCommand(sw, "CALL NZ, {0}", aaa);
                                    break;
                                case "10":
                                    this.WriteCommand(sw, "CALL C, {0}", aaa);
                                    break;
                                case "11":
                                    this.WriteCommand(sw, "CALL NC, {0}", aaa);
                                    break;
                            }
                            break;
                        case "101010":
                            this.WriteCommand(sw, "RETURN");
                            break;
                        case "101011":
                            switch (bits.Substring(6, 2))
                            {
                                case "00":
                                    this.WriteCommand(sw, "RETURN Z");
                                    break;
                                case "01":
                                    this.WriteCommand(sw, "RETURN NZ");
                                    break;
                                case "10":
                                    this.WriteCommand(sw, "RETURN C");
                                    break;
                                case "11":
                                    this.WriteCommand(sw, "RETURN NC");
                                    break;
                            }
                            break;
                        case "111000":
                            switch (bits.Substring(17, 1))
                            {
                                case "0":
                                    this.WriteCommand(sw, "RETURNI DISABLE");
                                    break;
                                case "1":
                                    this.WriteCommand(sw, "RETURNI ENABLE");
                                    break;
                            }
                            break;
                        case "111100":
                            switch (bits.Substring(17, 1))
                            {
                                case "0":
                                    this.WriteCommand(sw, "DISABLE INTERRUPT");
                                    break;
                                case "1":
                                    this.WriteCommand(sw, "ENABLE INTERRUPT");
                                    break;
                            }
                            break;
                        default:
                            return "Неразпозната инструкция " + line;
                    }
                }
            }

            return null;
        }

        public string Assemble(string[] lines, TheCpu CPU, string outputFilename)
        {
            this.address = 0;
            string error;

            // Prepare line mappings
            for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                string lineText = lines[lineNumber];

                if (lineText.Trim() == string.Empty) continue;

                string[] lineParsed;
                CommandBase.Parse(lineText, lineNumber, CPU, out lineParsed);
                CommandBase command = CommandManager.RecognizeCommand(lineText, lineParsed, CPU, out error);

                if (command == null || error != null)
                {
                    return error;
                }
                else
                {
                    LineMappings.Add(lineNumber, address);

                    if (command.GetType() == typeof(dirADDRESS) ||
                        command.GetType() == typeof(dirCONSTANT) ||
                        command.GetType() == typeof(dirNAMEREG) ||
                        command.GetType() == typeof(EmptyLine))
                    {
                        command.Assemble(lineParsed, CPU, this);
                    }
                    else
                    {
                        address++;
                    }
                }
            }

            this.address = 0;

            // Assemble
            for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                string lineText = lines[lineNumber];

                if (lineText.Trim() == string.Empty) continue;

                string[] lineParsed;
                CommandBase.Parse(lineText, lineNumber, CPU, out lineParsed);
                CommandBase command = CommandManager.RecognizeCommand(lineText, lineParsed, CPU, out error);

                if (command == null || error != null)
                {
                    return error;
                }
                else
                {
                    command.Assemble(lineParsed, CPU, this);
                }
            }

            // prepare strings
            Dictionary<string, string> output = new Dictionary<string, string>();
            output.Add("{name}", "RAM");
            output.Add("{timestamp}", DateTime.Now.ToString());

            for (int i = 0; i < 64; i++)
            {
                var data = new StringBuilder();
                var placeholder = "{" + string.Format("INIT_{0:X2}", i) + "}";

                for (int j = 31; j >= 0; j--)
                {
                    data.AppendFormat("{0:X2}", INIT[i, j]);
                }

                output.Add(placeholder, data.ToString());
            }

            for (int i = 0; i < 8; i++)
            {
                var data = new StringBuilder();
                var placeholder = "{" + string.Format("INITP_{0:X2}", i) + "}";

                for (int j = 31; j >= 0; j--)
                {
                    data.AppendFormat("{0:X2}", INITP[i, j]);
                }

                output.Add(placeholder, data.ToString());
            }

            string templateFilename;
            if (outputFilename.ToUpper().EndsWith(".VHD"))
            {
                templateFilename = AppDomain.CurrentDomain.BaseDirectory + @"Templates\ROM_form.vhd";
            }
            else
            {
                templateFilename = AppDomain.CurrentDomain.BaseDirectory + @"Templates\ROM_form.v";
            }
            
            using (StreamReader sr = new StreamReader(templateFilename))
            {
                using (StreamWriter sw = new StreamWriter(outputFilename, false))
                {
                    bool flag = false;

                    while (sr.EndOfStream == false)
                    {
                        var line = sr.ReadLine();

                        if (flag)
                        {
                            foreach (KeyValuePair<string, string> kvp in output)
                            {
                                line = line.Replace(kvp.Key, kvp.Value);
                            }
                            sw.WriteLine(line);
                        }

                        if (line.Contains("{begin template}"))
                        {
                            flag = true;
                        }
                    }
                }
            }

            return null;
        }

        public string AddressToBits10(string address)
        {
            var lineNumber = CPU.GetAddress(address);
            var addressNumber = string.Format("{0:x3}", LineMappings[lineNumber]);

            return HexToBits10(addressNumber);
        }

        public string RegisterToBits4(string register)
        {
            return HexToBits4(this.GetRegisterCode(register));
        }

        public string LiteralToBits8(string literal)
        {
            var value = CPU.GetLiteral(literal);
            var valueHex = string.Format("{0:x2}", value);
            return HexToBits8(valueHex);
        }

        private string GetRegisterCode(string register)
        {
            switch (register.Trim().ToLower())
            {
                case "s0":
                    return "0";
                case "s1":
                    return "1";
                case "s2":
                    return "2";
                case "s3":
                    return "3";
                case "s4":
                    return "4";
                case "s5":
                    return "5";
                case "s6":
                    return "6";
                case "s7":
                    return "7";
                case "s8":
                    return "8";
                case "s9":
                    return "9";
                case "sa":
                    return "A";
                case "sb":
                    return "B";
                case "sc":
                    return "C";
                case "sd":
                    return "D";
                case "se":
                    return "E";
                case "sf":
                    return "F";
                default:
                    if (this.CPU.NamedRegisters.ContainsKey(register.Trim().ToLower()))
                    {
                        return this.GetRegisterCode(this.CPU.NamedRegisters[register.Trim().ToLower()]);
                    }
                    throw new ArgumentException("Няма такъв регистър.");
            }
        }

        #endregion
        #region [Methods for Parsing Binary Data]

        public static byte BitsToByte2(string bits)
        {
            var byte1 = 0;

            byte1 += ((bits[1] == '1' ? 1 : 0) << 0);
            byte1 += ((bits[0] == '1' ? 1 : 0) << 1);

            return (byte)byte1;
        }

        public static byte BitsToByte8(string bits)
        {
            var byte1 = 0;

            byte1 += ((bits[7] == '1' ? 1 : 0) << 0);
            byte1 += ((bits[6] == '1' ? 1 : 0) << 1);
            byte1 += ((bits[5] == '1' ? 1 : 0) << 2);
            byte1 += ((bits[4] == '1' ? 1 : 0) << 3);
            byte1 += ((bits[3] == '1' ? 1 : 0) << 4);
            byte1 += ((bits[2] == '1' ? 1 : 0) << 5);
            byte1 += ((bits[1] == '1' ? 1 : 0) << 6);
            byte1 += ((bits[0] == '1' ? 1 : 0) << 7);

            return (byte)byte1;
        }

        public static string BitsToHex4(string bits)
        {
            var byte1 = 0;

            byte1 += ((bits[3] == '1' ? 1 : 0) << 0);
            byte1 += ((bits[2] == '1' ? 1 : 0) << 1);
            byte1 += ((bits[1] == '1' ? 1 : 0) << 2);
            byte1 += ((bits[0] == '1' ? 1 : 0) << 3);

            return string.Format("{0:X1}", byte1);
        }

        public static string BitsToHex8(string bits)
        {
            var byte1 = 0;

            byte1 += ((bits[7] == '1' ? 1 : 0) << 0);
            byte1 += ((bits[6] == '1' ? 1 : 0) << 1);
            byte1 += ((bits[5] == '1' ? 1 : 0) << 2);
            byte1 += ((bits[4] == '1' ? 1 : 0) << 3);
            byte1 += ((bits[3] == '1' ? 1 : 0) << 4);
            byte1 += ((bits[2] == '1' ? 1 : 0) << 5);
            byte1 += ((bits[1] == '1' ? 1 : 0) << 6);
            byte1 += ((bits[0] == '1' ? 1 : 0) << 7);

            return string.Format("{1:X2}", byte1);
        }

        public static string BitsToHex18(string bits)
        {
            var byte1 = 0;
            var byte2 = 0;
            var byte3 = 0;

            byte1 += ((bits[17] == '1' ? 1 : 0) << 0);
            byte1 += ((bits[16] == '1' ? 1 : 0) << 1);
            byte1 += ((bits[15] == '1' ? 1 : 0) << 2);
            byte1 += ((bits[14] == '1' ? 1 : 0) << 3);
            byte1 += ((bits[13] == '1' ? 1 : 0) << 4);
            byte1 += ((bits[12] == '1' ? 1 : 0) << 5);
            byte1 += ((bits[11] == '1' ? 1 : 0) << 6);
            byte1 += ((bits[10] == '1' ? 1 : 0) << 7);

            byte2 += ((bits[9] == '1' ? 1 : 0) << 0);
            byte2 += ((bits[8] == '1' ? 1 : 0) << 1);
            byte2 += ((bits[7] == '1' ? 1 : 0) << 2);
            byte2 += ((bits[6] == '1' ? 1 : 0) << 3);
            byte2 += ((bits[5] == '1' ? 1 : 0) << 4);
            byte2 += ((bits[4] == '1' ? 1 : 0) << 5);
            byte2 += ((bits[3] == '1' ? 1 : 0) << 6);
            byte2 += ((bits[2] == '1' ? 1 : 0) << 7);

            byte3 += ((bits[1] == '1' ? 1 : 0) << 0);
            byte3 += ((bits[0] == '1' ? 1 : 0) << 1);

            return string.Format("{0:X1}{1:X2}{2:X2}", byte3, byte2, byte1);
        }

        public static string HexToBits4(string hex)
        {
            var result = new StringBuilder();

            var bits0123 = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);

            result.Append((bits0123 & 8) >> 3);
            result.Append((bits0123 & 4) >> 2);
            result.Append((bits0123 & 2) >> 1);
            result.Append((bits0123 & 1) >> 0);

            return result.ToString();
        }

        public static string HexToBits8(string hex)
        {
            var result = new StringBuilder();

            var bits0to7 = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);

            result.Append((bits0to7 & 128) >> 7);
            result.Append((bits0to7 & 64) >> 6);
            result.Append((bits0to7 & 32) >> 5);
            result.Append((bits0to7 & 16) >> 4);
            result.Append((bits0to7 & 8) >> 3);
            result.Append((bits0to7 & 4) >> 2);
            result.Append((bits0to7 & 2) >> 1);
            result.Append((bits0to7 & 1) >> 0);

            return result.ToString();
        }

        public static string HexToBits10(string hex)
        {
            var result = new StringBuilder();

            var bits0to2 = byte.Parse(hex.Substring(0, 1), System.Globalization.NumberStyles.HexNumber);
            var bits3456 = byte.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);

            result.Append((bits0to2 & 2) >> 1);
            result.Append((bits0to2 & 1) >> 0);

            result.Append((bits3456 & 128) >> 7);
            result.Append((bits3456 & 64) >> 6);
            result.Append((bits3456 & 32) >> 5);
            result.Append((bits3456 & 16) >> 4);
            result.Append((bits3456 & 8) >> 3);
            result.Append((bits3456 & 4) >> 2);
            result.Append((bits3456 & 2) >> 1);
            result.Append((bits3456 & 1) >> 0);

            return result.ToString();
        }

        public static string HexToBits18(string hex)
        {
            var result = new StringBuilder();

            var bits0to2 = byte.Parse(hex.Substring(0, 1), System.Globalization.NumberStyles.HexNumber);
            var bits3456 = byte.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            var bits6789 = byte.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);

            result.Append((bits0to2 & 2) >> 1);
            result.Append((bits0to2 & 1) >> 0);

            result.Append((bits3456 & 128) >> 7);
            result.Append((bits3456 & 64) >> 6);
            result.Append((bits3456 & 32) >> 5);
            result.Append((bits3456 & 16) >> 4);
            result.Append((bits3456 & 8) >> 3);
            result.Append((bits3456 & 4) >> 2);
            result.Append((bits3456 & 2) >> 1);
            result.Append((bits3456 & 1) >> 0);

            result.Append((bits6789 & 128) >> 7);
            result.Append((bits6789 & 64) >> 6);
            result.Append((bits6789 & 32) >> 5);
            result.Append((bits6789 & 16) >> 4);
            result.Append((bits6789 & 8) >> 3);
            result.Append((bits6789 & 4) >> 2);
            result.Append((bits6789 & 2) >> 1);
            result.Append((bits6789 & 1) >> 0);

            return result.ToString();
        }

        #endregion
    }
}
