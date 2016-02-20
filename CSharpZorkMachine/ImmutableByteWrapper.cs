using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace CSharpZorkMachine
{
    public class ImmutableByteWrapper
    {
        private ImmutableArray<byte> InitialState;
        private Dictionary<ByteAddress, byte> edits;

        private ImmutableByteWrapper()
        {

        }


        public ImmutableByteWrapper(byte[] bytes)
        {
            this.InitialState = bytes.ToImmutableArray();
            this.edits = new Dictionary<ByteAddress, byte>();
        }

        private bool IsInRange(ByteAddress address)
        {
            return 0 <= address.Value && address.Value < InitialState.Length;
        }

        private bool IsOutOfRange(ByteAddress address)
        {
            return !IsInRange(address);
        }

        public ImmutableByteWrapper WriteToAddress(ByteAddress address, byte newValue)
        {
            ValidateAddress(address);
            ImmutableByteWrapper retval = new ImmutableByteWrapper();
            retval.InitialState = this.InitialState; //it's immutable, so every instance can share a pointer to it. 
            retval.edits = new Dictionary<ByteAddress, byte>(this.edits);
            retval.edits[address] = newValue;

            return retval;
        }
        
        public byte ReadAddress(ByteAddress address)
        {
            if (this.edits.ContainsKey(address))
            {
                return this.edits[address];
            }
            return this.InitialState[address.Value];
        }

        private void ValidateAddress(ByteAddress address)
        {
            if (IsOutOfRange(address))
            {
                throw new IndexOutOfRangeException();
            }
        }


    }
}
