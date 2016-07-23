using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpZorkMachine
{
    public struct Word
    {
        private int value; 
        public Word(int value)
        {
            this.value = value; 
        }

        public short Value { get { return (short)this.value; } }

        public bool IsTerminal(ISet<char> breakers = null)
        {
            int lastBit = Bits.FetchBits(BitNumber.Bit15, BitSize.Size1, this);
            string thisPrinted = Zchar.PrintWordAsZchars(this);
            if(breakers != null && thisPrinted.ToCharArray().Any(ch => breakers.Contains(ch)))
            {
                return true; 
            }
            return lastBit != 0;
        }

    }
}
