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
    }
}
