using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpZorkMachine
{
    public struct WordAddress
    {
        private const int WordSize = 2; 
        private int value;
        public WordAddress(int value)
        {
            this.value = value; 
        }
        public int Value { get { return this.value; } }

        public static WordAddress operator +(WordAddress address, int offset)
        {
            return new WordAddress(address.Value + (WordSize * offset)); 
        }
    }
}
