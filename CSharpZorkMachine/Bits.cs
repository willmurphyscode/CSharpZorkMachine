using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpZorkMachine
{
    public static class Bits
    {
        public static int FetchBits(BitNumber high, BitSize length, Word word)
        {
            int mask = ~(-1 << length.Size);
            int retval = (word.Value >> (high.Value - length.Size + 1)) & mask;
            return retval;
        }

        public static ByteAddress AddressOfHighByte(WordAddress address)
        {
            return new ByteAddress(address.Value);
        }

        public static ByteAddress AddressOfLowByte(WordAddress address)
        {
            return new ByteAddress(address.Value + 1);
        }
        public static List<Zchar> ReadStringFromAddress(WordAddress address, GameMemory memory)
        {
            List<Zchar> retval = new List<Zchar>();
            Word word = memory.ReadWord(address);
            while(!word.IsTerminal())
            {          
                Zchar char1 = new Zchar(new AbbreviationNumber(Bits.FetchBits(BitNumber.Bit14, BitSize.Size5, word)));
                Zchar char2 = new Zchar(new AbbreviationNumber(Bits.FetchBits(BitNumber.Bit9, BitSize.Size5, word)));
                Zchar char3 = new Zchar(new AbbreviationNumber(Bits.FetchBits(BitNumber.Bit4, BitSize.Size5, word)));
                retval.Add(char1);
                retval.Add(char2);
                retval.Add(char3);
                address = address + 1; 
                word = memory.ReadWord(address);
            }
            return retval;
        }
    }
}
