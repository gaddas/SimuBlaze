using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDE.CPU.Commands;
using System.ComponentModel;
using System.Windows.Forms;

namespace IDE.CPU
{
    public class TheCpu : INotifyPropertyChanged
    {
        #region [Fields]

        private frmMain _parent;

        private int _pc = 0;

        private Stack<int> _stack = new Stack<int>();

        private byte[] _scratchpad = new byte[64];

        private Dictionary<string, byte> literals = new Dictionary<string, byte>();

        private Dictionary<string, int> labels = new Dictionary<string, int>();

        private Dictionary<string, string> namedRegisters = new Dictionary<string, string>();

        private bool _zero;

        private bool _carry;

        private bool _interruptEnable;

        private byte _s0, _s1, _s2, _s3, _s4, _s5, _s6, _s7, _s8, _s9, _sA, _sB, _sC, _sD, _sE, _sF;

        #endregion
        #region [Properties]

        /// <summary>
        /// The Program Counter
        /// </summary>
        public int PC
        {
            get
            {
                return this._pc;
            }

            set
            {
                _pc = value;

                if (this._pc > 0x3FF)
                {
                    this._pc = (this._pc & 0x3FF);
                }
            }
        }

        // Flags
        public bool ZERO
        {
            get
            {
                return this._zero;
            }

            set
            {
                this._zero = value;
                this.OnPropertyChanged(this.PropertyName(() => this.ZERO));
            }
        }

        public bool CARRY
        {
            get
            {
                return this._carry;
            }

            set
            {
                this._carry = value;
                this.OnPropertyChanged(this.PropertyName(() => this.CARRY));
            }
        }

        public bool INTERRUPT_ENABLE
        {
            get
            {
                return this._interruptEnable;
            }

            set
            {
                this._interruptEnable = value;
                this.OnPropertyChanged(this.PropertyName(() => this.INTERRUPT_ENABLE));
            }
        }

        public bool PRESERVED_ZERO { get; set; }
        public bool PRESERVED_CARRY { get; set; }

        // Registers
        public byte s0
        {
            get
            {
                return this._s0;
            }

            set
            {
                this._s0 = value;
                this.OnPropertyChanged(this.PropertyName(() => this.s0));
            }
        }
        public byte s1
        {
            get
            {
                return this._s1;
            }

            set
            {
                this._s1 = value;
                this.OnPropertyChanged(this.PropertyName(() => this.s1));
            }
        }
        public byte s2
        {
            get
            {
                return this._s2;
            }

            set
            {
                this._s2 = value;
                this.OnPropertyChanged(this.PropertyName(() => this.s2));
            }
        }
        public byte s3
        {
            get
            {
                return this._s3;
            }

            set
            {
                this._s3 = value;
                this.OnPropertyChanged(this.PropertyName(() => this.s3));
            }
        }
        public byte s4
        {
            get
            {
                return this._s4;
            }

            set
            {
                this._s4 = value;
                this.OnPropertyChanged(this.PropertyName(() => this.s4));
            }
        }
        public byte s5
        {
            get
            {
                return this._s5;
            }

            set
            {
                this._s5 = value;
                this.OnPropertyChanged(this.PropertyName(() => this.s5));
            }
        }
        public byte s6
        {
            get
            {
                return this._s6;
            }

            set
            {
                this._s6 = value;
                this.OnPropertyChanged(this.PropertyName(() => this.s6));
            }
        }
        public byte s7
        {
            get
            {
                return this._s7;
            }

            set
            {
                this._s7 = value;
                this.OnPropertyChanged(this.PropertyName(() => this.s7));
            }
        }
        public byte s8
        {
            get
            {
                return this._s8;
            }

            set
            {
                this._s8 = value;
                this.OnPropertyChanged(this.PropertyName(() => this.s8));
            }
        }
        public byte s9
        {
            get
            {
                return this._s9;
            }

            set
            {
                this._s9 = value;
                this.OnPropertyChanged(this.PropertyName(() => this.s9));
            }
        }
        public byte sA
        {
            get
            {
                return this._sA;
            }

            set
            {
                this._sA = value;
                this.OnPropertyChanged(this.PropertyName(() => this.sA));
            }
        }
        public byte sB
        {
            get
            {
                return this._sB;
            }

            set
            {
                this._sB = value;
                this.OnPropertyChanged(this.PropertyName(() => this.sB));
            }
        }
        public byte sC
        {
            get
            {
                return this._sC;
            }

            set
            {
                this._sC = value;
                this.OnPropertyChanged(this.PropertyName(() => this.sC));
            }
        }
        public byte sD
        {
            get
            {
                return this._sD;
            }

            set
            {
                this._sD = value;
                this.OnPropertyChanged(this.PropertyName(() => this.sD));
            }
        }
        public byte sE
        {
            get
            {
                return this._sE;
            }

            set
            {
                this._sE = value;
                this.OnPropertyChanged(this.PropertyName(() => this.sE));
            }
        }
        public byte sF
        {
            get
            {
                return this._sF;
            }

            set
            {
                this._sF = value;
                this.OnPropertyChanged(this.PropertyName(() => this.sF));
            }
        }

        /// <summary>
        /// The Function Stack
        /// </summary>
        public Stack<int> Stack
        {
            get
            {
                return this._stack;
            }
        }

        /// <summary>
        /// The Scratchpad RAM
        /// </summary>
        public byte[] Scratchpad
        {
            get
            {
                return this._scratchpad;
            }

            set
            {
                this._scratchpad = value;
            }
        }

        public Dictionary<string, byte> Literals
        {
            get
            {
                return this.literals;
            }
        }

        public Dictionary<string, int> Labels
        {
            get
            {
                return this.labels;
            }
        }

        public Dictionary<string, string> NamedRegisters
        {
            get
            {
                return this.namedRegisters;
            }
        }

        #endregion
        #region [Methods]

        public bool ValidRegister(string register)
        {
            switch (register.Trim().ToLower())
            {
                case "s0":
                case "s1":
                case "s2":
                case "s3":
                case "s4":
                case "s5":
                case "s6":
                case "s7":
                case "s8":
                case "s9":
                case "sa":
                case "sb":
                case "sc":
                case "sd":
                case "se":
                case "sf":
                    return true;
                default:
                    if (this.namedRegisters.ContainsKey(register.Trim().ToLower()))
                    {
                        return true;
                    }
                    return false;
            }
        }

        public byte GetRegister(string register)
        {
            switch (register.Trim().ToLower())
            {
                case "s0":
                    return this.s0;
                case "s1":
                    return this.s1;
                case "s2":
                    return this.s2;
                case "s3":
                    return this.s3;
                case "s4":
                    return this.s4;
                case "s5":
                    return this.s5;
                case "s6":
                    return this.s6;
                case "s7":
                    return this.s7;
                case "s8":
                    return this.s8;
                case "s9":
                    return this.s9;
                case "sa":
                    return this.sA;
                case "sb":
                    return this.sB;
                case "sc":
                    return this.sC;
                case "sd":
                    return this.sD;
                case "se":
                    return this.sE;
                case "sf":
                    return this.sF;
                default:
                    if (this.namedRegisters.ContainsKey(register.Trim().ToLower()))
                    {
                        return this.GetRegister(this.namedRegisters[register.Trim().ToLower()]);
                    }
                    throw new ArgumentException("Няма такъв регистър.");
            }
        }

        public void SetRegister(string register, byte value)
        {
            switch (register.Trim().ToLower())
            {
                case "s0":
                    this.s0 = value;
                    return;
                case "s1":
                    this.s1 = value;
                    return;
                case "s2":
                    this.s2 = value;
                    return;
                case "s3":
                    this.s3 = value;
                    return;
                case "s4":
                    this.s4 = value;
                    return;
                case "s5":
                    this.s5 = value;
                    return;
                case "s6":
                    this.s6 = value;
                    return;
                case "s7":
                    this.s7 = value;
                    return;
                case "s8":
                    this.s8 = value;
                    return;
                case "s9":
                    this.s9 = value;
                    return;
                case "sa":
                    this.sA = value;
                    return;
                case "sb":
                    this.sB = value;
                    return;
                case "sc":
                    this.sC = value;
                    return;
                case "sd":
                    this.sD = value;
                    return;
                case "se":
                    this.sE = value;
                    return;
                case "sf":
                    this.sF = value;
                    return;
                default:
                    if (this.namedRegisters.ContainsKey(register.Trim().ToLower()))
                    {
                        this.SetRegister(this.namedRegisters[register.Trim().ToLower()], value);
                        return;
                    }
                    throw new ArgumentException("Няма такъв регистър.");
            }
        }

        public bool ValidLiteral(string literal)
        {
            byte result;

            if (this.literals.ContainsKey(literal.ToUpper()))
            {
                return true;
            }
            else if (byte.TryParse(literal, System.Globalization.NumberStyles.HexNumber, null, out result))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public byte GetLiteral(string literal)
        {
            byte result;

            if (this.literals.ContainsKey(literal.ToUpper()))
            {
                result = this.literals[literal.ToUpper()];
                return result;
            }
            else if (byte.TryParse(literal, System.Globalization.NumberStyles.HexNumber, null, out result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException("Константа не може да бъде намерена.");
            }
        }

        public bool ValidAddress(string address)
        {
            int result;

            if (this.labels.ContainsKey(address.ToUpper()))
            {
                return true;
            }
            else if (int.TryParse(address, System.Globalization.NumberStyles.HexNumber, null, out result))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetAddress(string address)
        {
            int result;

            if (this.labels.ContainsKey(address.ToUpper()))
            {
                result = this.labels[address.ToUpper()];
                return result;
            }
            else if (int.TryParse(address, System.Globalization.NumberStyles.HexNumber, null, out result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException("Невалиден адрес.");
            }
        }

        public byte ReadRAM(byte address)
        {
            if (address >= _scratchpad.Length)
            {
                throw new ArgumentException("Четене извън scratchpad паметта.");
            }
            address = (byte)(address & 0x3F);
            return _scratchpad[address];
        }

        public void WriteRAM(byte address, byte value)
        {
            if (address >= _scratchpad.Length)
            {
                throw new ArgumentException("Писане извън scratchpad паметта.");
            }
            address = (byte)(address & 0x3F);
            _scratchpad[address] = value;

            this.OnPropertyChanged(this.PropertyName(() => this.Scratchpad));
        }

        public void Reset()
        {
            this.ZERO = false;
            this.CARRY = false;
            this.INTERRUPT_ENABLE = false;
            this.PC = 0;
            this.Stack.Clear();

            this.OnPropertyChanged(this.PropertyName(() => this.Stack));
        }

        public void Interrupt()
        {
            if (this.INTERRUPT_ENABLE)
            {
                this.Stack.Push(this.PC);

                this.PRESERVED_ZERO = this.ZERO;
                this.PRESERVED_CARRY = this.CARRY;
                this.INTERRUPT_ENABLE = false;
                this.PC = 0x3FF;
            }
        }

        public void Output(byte PORT_ID, byte OUT_PORT)
        {
            MessageBox.Show(
                string.Format("Изходни данни на порт 0x{0:x2}: 0x{1:x2}", PORT_ID, OUT_PORT),
                "OUTPUT",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public byte Input(byte PORT_ID)
        {
            var inputData = Microsoft.VisualBasic.Interaction.InputBox(
                 string.Format("Входни данни на порт 0x{0:x2}:", PORT_ID),
                 "INPUT",
                 "0",
                 0,
                 0);

            byte input = 0;
            byte.TryParse(inputData, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out input);
            return input;
        }

        #endregion

        public TheCpu(frmMain parent)
        {
            this._parent = parent;
            CommandManager.InitializeCommands();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private delegate void OnPropertyChangedDelegate(string text);
        public void OnPropertyChanged(string propertyName)
        {
            if (this._parent.InvokeRequired)
            {
                this._parent.Invoke(new OnPropertyChangedDelegate(this.OnPropertyChanged), propertyName);
            }
            else
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        #endregion
    }
}
