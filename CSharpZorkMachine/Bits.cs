using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpZorkMachine
{
    public static class Bits
    {
        public static short FetchBits(BitNumber high, BitSize length, Word word)
        {
            short mask = (short) ~(-1 << length.Size);
            short retval = (short)(word.Value >> (high.Value - length.Size + 1) & mask);
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
                Zchar char1 = new Zchar(Bits.FetchBits(BitNumber.Bit14, BitSize.Size5, word));
                Zchar char2 = new Zchar(Bits.FetchBits(BitNumber.Bit9, BitSize.Size5, word));
                Zchar char3 = new Zchar(Bits.FetchBits(BitNumber.Bit4, BitSize.Size5, word));
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
