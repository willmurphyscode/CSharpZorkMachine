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
            if (0 > value || value >= abbreviationTableLength)
            {
                throw new ArgumentException("value is out of range.");
            }
            this.value = value;
        }
        public int Value { get { return this.value; } }
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
            WordAddress ptrToChosenAbbrv = ptrToAbbrevTable + (number.Value);
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
        public int Value { get { return this.value; } }
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
        private static char[] Symbol = { ' ', '?', '?', '?', '?', '?', '?', '\n', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', ',', '!', '?', '_', '#', '\'', '"', '/', '\\', '-', ':', '(', ')' };
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

        private static AbbreviationNumber GetFromZcharAndState(state st, Zchar value)
        {
            // 32(z-1)+x 
            int z = 0;
            switch (st)
            {
                case state.abbr:
                    z = 1;
                    break;
                case state.abbr32:
                    z = 2;
                    break;
                case state.abbr64:
                    z = 3;
                    break;
                default:
                    throw new ArgumentException("Apparently invalid state");
            }
            int x = value.Value;
            return new AbbreviationNumber(32 * (z - 1) + x);
        }

        public static IEnumerable<char> DecodeFromZString(IEnumerable<Zchar> chars, GameMemory memory, bool permitRecurse = true)
        {
            //THIS IS the least functional-style code in here. it's so stateful. (well, it's a state machine)
            //another way would be to write a function that 
            //takes a state and a zchar and decides what to do. 
            const int ZCHAR_PER_ABBREV = 6;
            const int lowEntry = 6;
            Zchar? previous = null;
            state current = state.lower;
            int ixCounter = 0; //TEMP DELME
            int fivesToSkip = 0;
            foreach (Zchar ch in chars)
            {
                ixCounter++;

                bool isAbbrev = current == state.abbr || current == state.abbr32 || current == state.abbr64;
                if(isAbbrev && ch.Value <= 5)
                {
                    int a = 3; 

                }

                if ((ch.Value >= lowEntry || ch.Value == 0)|| (/*ch.Value == 5 &&*/ isAbbrev))
                {
                    switch (current)
                    {
                        case state.abbr:
                        case state.abbr32:
                        case state.abbr64:
                            AbbreviationNumber num = GetFromZcharAndState(current, ch);
                            IEnumerable<Zchar> abbrevs = ReadAbbrevTillBreak(num, memory).ToList();
                            List<char> inner = DecodeFromZString(abbrevs, memory, false).ToList();
                            fivesToSkip = ZCHAR_PER_ABBREV - inner.Count - 1;
                            foreach (char innerChar in inner)
                            {
                                yield return innerChar;
                            }
                            if(fivesToSkip > 0)
                            {
                                current = state.lower;
                                fivesToSkip--;
                                continue; 
                            }

                            current = state.lower;
                            break;
                        case state.lower:
                            Console.Write(Lower[ch.Value]);
                            yield return Lower[ch.Value];
                            break;
                        case state.upper:
                            current = state.lower;
                            Console.Write(Upper[ch.Value]);
                            yield return Upper[ch.Value];
                            break;
                        case state.symbol:
                            if (ch.Value == 6)
                            {
                                current = state.ascii;
                                continue;
                            }
                            else
                            {
                                current = state.lower;
                                Console.Write(Symbol[ch.Value]);
                                yield return Symbol[ch.Value];
                            }
                            break;
                        case state.ascii:
                            previous = ch;
                            current = state.ascii2;
                            continue;
                        case state.ascii2:
                            throw new System.NotImplementedException();
                            //TODO the previous char and the current char
                            // are combined to make an ascii char
                            // not in the tables. 
                            break;
                    }
                }
                else
                {
                    switch (ch.Value)
                    {
                        case 1:
                            current = state.abbr;
                            if (!permitRecurse)
                            {
                                yield break;
                            }
                            break;
                        case 2:
                            current = state.abbr32;
                            if (!permitRecurse)
                            {
                                yield break;
                            }
                            break;
                        case 3:
                            current = state.abbr64;
                            if (!permitRecurse)
                            {
                                yield break;
                            }
                            break;
                        case 4:
                            current = state.upper;
                            break;
                        case 5:
                            //TODO if we're in the abbreviation, 5s do nothing.
                            //if(fivesToSkip > 0)
                            //{
                            //    fivesToSkip--;
                            //    current = state.lower;
                            //    break; 
                            //}

                            current = state.symbol;

                            break;
                        case 0:
                            //yield return ' ';
                            break;
                        default:
                            throw new ArgumentException("Invalid state.");
                    }
                    continue;
                }
            }
        }
        public static IEnumerable<char> YieldAbbreviationFromAbbreviationNumber(AbbreviationNumber number, GameMemory memory)
        {
            WordAddress addressOfPtrToAbbrTable = new WordAddress(24);
            WordAddress ptrToAbbrevTable = new WordAddress(memory.ReadWord(addressOfPtrToAbbrTable).Value);
            WordAddress address = AbbreviationTableBase.AddressOfAbbreviationByNumber(number, memory);
            Word word = memory.ReadWord(address);
            while (true)
            {
                char[] firstThree = Zchar.PrintWordAsZchars(word).ToCharArray();
                foreach (char ch in firstThree)
                {
                    yield return ch;
                }
                if (word.IsTerminal())
                {
                    yield break;
                }
                address = address + 1;
                word = memory.ReadWord(address);
            }
        }

        public static string PrintWordAsZchars(Word word)
        {
            return Zchar.PrintFromZchar(ReadWordAsZchars(word));
        }

        public static IEnumerable<Zchar> ReadWordAsZchars(Word word)
        {
            Zchar low = new Zchar(Bits.FetchBits(BitNumber.Bit14, BitSize.Size5, word));
            Zchar mid = new Zchar(Bits.FetchBits(BitNumber.Bit9, BitSize.Size5, word));
            Zchar high = new Zchar(Bits.FetchBits(BitNumber.Bit4, BitSize.Size5, word));
            return new Zchar[] { low, mid, high };
        }

        public static IEnumerable<Zchar> ReadAbbrevTillBreak(AbbreviationNumber num, GameMemory memory)
        {
            WordAddress addressOfPtrToAbbrTable = new WordAddress(24);
            WordAddress ptrToAbbrevTable = new WordAddress(memory.ReadWord(addressOfPtrToAbbrTable).Value);
            WordAddress ptrChosenAbbrev = AbbreviationTableBase.AddressOfAbbreviationByNumber(num, memory);
            return ReadWordsTillBreak(ptrChosenAbbrev, memory);
        }

        public static IEnumerable<Zchar> ReadWordsTillBreak(WordAddress address, GameMemory memory, ISet<char> breakers = null)
        {
            while (true)
            {
                Word word = memory.ReadWord(address);
                Zchar low = new Zchar(Bits.FetchBits(BitNumber.Bit14, BitSize.Size5, word));
                Zchar mid = new Zchar(Bits.FetchBits(BitNumber.Bit9, BitSize.Size5, word));
                Zchar high = new Zchar(Bits.FetchBits(BitNumber.Bit4, BitSize.Size5, word));
                yield return low;
                yield return mid;
                yield return high;
                if (word.IsTerminal(breakers))
                {
                    yield break;
                }
                address = address + 1;
            }


        }

        public static string DiagnosticPrintFromZchar(IEnumerable<Zchar> chars)
        {
            StringBuilder retval = new StringBuilder();
            foreach (Zchar ch in chars)
            {
                retval.Append($"{ch.Value.ToString("X")} {Lower[ch.Value]} |");
            }
            return retval.ToString();
        }
    }


}
