using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpZorkMachine
{
    /// <summary>
    /// This struct holds an index into the address table.
    /// </summary>
    public struct AbbreviationNumber
    {
        const int abbreviationTableLength = 96; 
        private int value;
        public AbbreviationNumber(int value)
        {
            if(0 > value || value >= abbreviationTableLength)
            {
                throw new ArgumentException("value is out of range.");
            }
            this.value = value; 
        }
        public int Value { get { return this.value; }  }
    }
    /// <summary>
    /// I believe this struct is just to hold a pointer
    /// to the beginning of the address table. 
    /// </summary>
    public struct AbbreviationTableBase
    {
        private int value;
        public AbbreviationTableBase(int value)
        {
            this.value = value;
        }
        public AbbreviationTableBase(GameMemory memory)
        {
            //the address of the first word in the table is 
            //stored at word 24 in the game memory
            WordAddress addrOfTableBasePtr = new WordAddress(24);
            Word addressOfTableBase = memory.ReadWord(addrOfTableBasePtr);
            this.value = addressOfTableBase.Value;
        }
        public int Value { get { return this.value; } }

        public static WordAddress addressOfAbbreviationTable(GameMemory memory)
        {
            return new WordAddress(new AbbreviationTableBase(memory).Value);
        }
    }

    /// <summary>
    /// This is a 17-bit pointer that has been divided by
    /// 2 in order to fit it into a word. 
    /// </summary>
    public struct WordZstringAddress
    {
        private int value; 
        public WordZstringAddress(int value)
        {
            this.value = value; 
        }
        public int Value { get { return this.value;  } }
    }
    /// <summary>
    /// This is an uncompressed 17-bit pointer produced 
    /// by 2 * WordZstringAddress.Value
    /// I believe there is no valid way to instantiate this
    /// pointer besides decompressing the compressed struct above,
    /// hence the constraint with the constructor
    /// </summary>
    public struct ZstringAddress
    {
        private int value;
        public ZstringAddress(WordZstringAddress compressed)
        {
            this.value = compressed.Value * 2;
        }
        public int Value { get { return this.value; } }
    }
}
