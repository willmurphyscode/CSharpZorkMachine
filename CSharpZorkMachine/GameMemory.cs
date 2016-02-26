using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace CSharpZorkMachine
{
    public class GameMemory
    {
        private int lengthOfDynamicState;
        private ImmutableArray<byte> staticMemory;
        private ImmutableByteWrapper dynamicState; 

        public GameMemory(byte[] staticMemory, ImmutableByteWrapper dynamicState)
        {
            this.lengthOfDynamicState = dynamicState.Length;
            this.staticMemory = staticMemory.ToImmutableArray<byte>();
            this.dynamicState = dynamicState;
        }

        private int ByteAddressToStaticIx(ByteAddress address)
        {
            return address.Value - lengthOfDynamicState;
        }

        private bool IsInStaticRange(ByteAddress address)
        {
            return lengthOfDynamicState <= address.Value && address.Value < lengthOfDynamicState + staticMemory.Length;
        }


        private void ValidateAddressToStatic(ByteAddress address)
        {
            if(!IsInStaticRange(address))
            {
                throw new IndexOutOfRangeException();
            }
        }
        private byte ReadAddressFromStaticMemory(ByteAddress address)
        {
            ValidateAddressToStatic(address);
            return this.staticMemory[ByteAddressToStaticIx(address)];
        }


        public byte ReadAddress(ByteAddress address)
        {
            if(this.dynamicState.IsInRange(address))
            {
                return this.dynamicState[address]; 
            }
            else
            {
                return ReadAddressFromStaticMemory(address);
            }

        }
        public Word ReadWord(WordAddress address)
        {
            ByteAddress highByteAddress = Bits.AddressOfHighByte(address);
            ByteAddress lowByteAddress = Bits.AddressOfLowByte(address);
            byte high = ReadAddress(highByteAddress);
            byte low = ReadAddress(lowByteAddress);

            return new Word((256 * high) + low); 
        }


        public byte this[ByteAddress address]
        {
            get
            {
                return this.ReadAddress(address);
            }
        }

        public void WriteByte(ByteAddress address, byte toWrite)
        {
            this.dynamicState = this.dynamicState.WriteToAddress(address, toWrite); 
        }
        public void WriteWord(WordAddress wordAddress, Word toWrite)
        {
            short bytes = toWrite.Value;
            byte high = (byte)((bytes << 8) & 0xFF);
            byte low = (byte)(bytes & 0xFF);
            this.WriteByte(Bits.AddressOfHighByte(wordAddress), high);
            this.WriteByte(Bits.AddressOfLowByte(wordAddress), low);
        }


    }
}
