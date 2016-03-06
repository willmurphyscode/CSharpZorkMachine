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

        public AbbreviationTableBase(GameMemory memory)
        {
            WordAddress addrOfTableBasePtr = new WordAddress(24);
            WordAddress addressOfTableBase = new WordAddress(memory.ReadWord(addrOfTableBasePtr).Value);
            this.value = addressOfTableBase.Value;
        }
        public int Value { get { return this.value; } }

        public static WordAddress AddressOfAbbreviationByNumber(AbbreviationNumber number, GameMemory memory)
        {
            AbbreviationTableBase basePtr = new AbbreviationTableBase(memory);
            WordAddress addressOfPtrToAbbrTable = new WordAddress(24);
            WordAddress ptrToAbbrevTable = new WordAddress(memory.ReadWord(addressOfPtrToAbbrTable).Value);
            WordAddress ptrToChosenAbbrv = ptrToAbbrevTable + ( number.Value * 2);
            WordAddress decompressedAbbrvPtr = new WordAddress(memory.ReadWord(ptrToChosenAbbrv).Value * 2);
            return decompressedAbbrvPtr;
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

    public struct AbbreviationZstring
    {
        private int value;
        public AbbreviationZstring(int value)
        {
            this.value = value;
        }
        public int Value { get { return this.value; } }
    }

    public struct WordZstring
    {
        private int value;
        public WordZstring(int value)
        {
            this.value = value;
        }
        public int Value { get { return this.value; } }
    }
    public struct Zchar
    {
        private int value;
        private static char[] Table = { ' ', '?', '?', '?', '?', '?', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' }; 
        public Zchar(int value)
        {
            this.value = value;
        }
        public int Value { get { return this.value; } }
        public static string PrintFromZchar(IEnumerable<Zchar> chars)
        {
            char[] values = chars.Select(ch => Table[ch.Value]).ToArray();
            return new string(values);
        }
        public static string PrintWordAsZchars(Word word)
        {
            Zchar low = new Zchar(Bits.FetchBits(BitNumber.Bit14, BitSize.Size5, word));
            Zchar mid = new Zchar(Bits.FetchBits(BitNumber.Bit9, BitSize.Size5, word));
            Zchar high = new Zchar(Bits.FetchBits(BitNumber.Bit4, BitSize.Size5, word));

            return Zchar.PrintFromZchar(new List<Zchar> { low, mid, high }); 
        }

        public static string DiagnosticPringFromZchar(IEnumerable<Zchar> chars)
        {
            StringBuilder retval = new StringBuilder();
            foreach(Zchar ch in chars)
            {
                retval.Append($"{ch.Value.ToString("X")} {Table[ch.Value]} |");
            }
            return retval.ToString();
        }
    }


}
