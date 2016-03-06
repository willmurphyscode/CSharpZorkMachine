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
        enum state { abbr, abbr32, abbr64, upper, symbol, lower, ascii, ascii2 };
        private int value;
        private static char[] Lower = { ' ', '?', '?', '?', '?', '?', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private static char[] Upper = { ' ', '?', '?', '?', '?', '?', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private static char[] Symbol = { ' ', '?', '?', '?', '?', '?', '?', '\n', '0', '1', '2', '3', '4', '5', '6', '7','8', '9', '.', ',', '!', '?', '_', '#', '\'', '"', '/', '\\', '-', ':', '(', ')' }; 
        public Zchar(int value)
        {
            this.value = value;
        }
        public int Value { get { return this.value; } }
        public static string PrintFromZchar(IEnumerable<Zchar> chars)
        {
            char[] values = chars.Select(ch => Lower[ch.Value]).ToArray();
            return new string(values);
        }
        
        public static IEnumerable<char> DecodeFromZString(IEnumerable<Zchar> chars, GameMemory memory)
        {
            //THIS IS the least functional-style code in here. it's so stateful. (well, it's a state machine)
            //another way would be to write a function that 
            //takes a state and a zchar and decides what to do. 
            const int lowEntry = 6;
            Zchar? previous = null;
            state current = state.lower;
            foreach(Zchar ch in chars)
            {
                if(ch.Value >= lowEntry)
                {
                    switch(current)
                    {
                        case state.abbr:
                        case state.abbr32:
                        case state.abbr64:
                            throw new NotImplementedException(); 
                            //TODO insert an entire string dereferenced from the abbreviation table here,
                            // probably with some horrible, nested state-machine-for-loop
                            break; 
                        
                        case state.lower:
                            yield return Lower[ch.Value];
                            break;
                        case state.upper:
                            current = state.lower;
                            yield return Upper[ch.Value];
                            break;
                        case state.symbol:
                            if(ch.Value == 6)
                            {
                                current = state.ascii;
                                continue;
                            }
                            else
                            {
                                current = state.lower;
                                yield return Symbol[ch.Value]; 
                            }
                            break;
                        case state.ascii:
                            previous = ch;
                            current = state.ascii2;
                            continue;
                        case state.ascii2:
                            throw new System.NotImplementedException();
                            //TODO the previous char and the currnt char
                            // are combined to make an ascii char
                            // not in the tables. 
                            break; 
                    }
                }
                else
                {
                    switch(ch.Value)
                    {
                        case 4:
                            current = state.upper;
                            break;
                        case 5:
                            current = state.abbr;
                            break;
                    }
                    continue;
                }
            }
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
                retval.Append($"{ch.Value.ToString("X")} {Lower[ch.Value]} |");
            }
            return retval.ToString();
        }
    }


}
