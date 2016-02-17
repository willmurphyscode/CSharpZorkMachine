using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpZorkMachine
{
    public struct BitNumber
    {
        private int value;
        public BitNumber(int value)
        {
            this.value = value;
        }
        public int Value { get { return this.value; } }

        public static readonly BitNumber Bit1 =  new BitNumber(1);
        public static readonly BitNumber Bit2 =  new BitNumber(2);
        public static readonly BitNumber Bit3 =  new BitNumber(3);
        public static readonly BitNumber Bit4 =  new BitNumber(4);
        public static readonly BitNumber Bit5 =  new BitNumber(5);
        public static readonly BitNumber Bit6 =  new BitNumber(6);
        public static readonly BitNumber Bit7 =  new BitNumber(7);
        public static readonly BitNumber Bit8 =  new BitNumber(8);
        public static readonly BitNumber Bit9 =  new BitNumber(9);
        public static readonly BitNumber Bit10 = new BitNumber(10);
        public static readonly BitNumber Bit11 = new BitNumber(11);
        public static readonly BitNumber Bit12 = new BitNumber(12);
        public static readonly BitNumber Bit13 = new BitNumber(13);
        public static readonly BitNumber Bit14 = new BitNumber(14);
        public static readonly BitNumber Bit15 = new BitNumber(15);
        public static readonly BitNumber Bit16 = new BitNumber(16);
    }
}
