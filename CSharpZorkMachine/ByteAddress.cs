using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpZorkMachine
{
    public struct ByteAddress
    {
        private int value;

        public ByteAddress(int value)
        {
            this.value = value; 
        }
        public int Value { get { return this.value; } }

        public static ByteAddress operator +(ByteAddress address, int offset)
        {
            return new ByteAddress(address.Value + offset);
        }
        public static ByteAddress operator -(ByteAddress address, int offset)
        {
            return address + (0 - offset);
        }
    }
}
