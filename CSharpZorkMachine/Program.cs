using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpZorkMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathToMiniZork = @"..\..\..\minizork.z3";
            GameMemory minizork = GameMemory.OpenFile(pathToMiniZork);

            //ReadDictionaryTillBreak(minizork);

            DictionaryHelper dictionary = new DictionaryHelper();

            Console.WriteLine($"Dictionary is expected to have {dictionary.CountOfEntries(minizork)} entries");
            string expectedFirst10EntriesString = "$ve . , #comm #rand #reco #unre \" a about";
            string[] expectedFirst10Entries = expectedFirst10EntriesString.Split(' ');

            List<string> nEntries = new List<string>();
            int n = 10; 
            for(int i = 0; i < n; i++ )
            {
                string entry = dictionary.ReadNthEntry(i, minizork).Print;
                Console.WriteLine($"Expected: {expectedFirst10Entries[i]} but Found: {entry}");
            }



            WordAddress testText = new WordAddress(0xb106);
            IEnumerable<Zchar> story = Zchar.ReadWordsTillBreak(testText, minizork);
            char[] toPrint = Zchar.DecodeFromZString(story, minizork).ToArray();
            string printMe = new string(toPrint);
            Console.Write(printMe);

            TestMemoryBase();
            TestReadingAndWritingGameState();
            TestReadVersionNumberFromFile(pathToMiniZork);


            //TestBitTwiddling();

            //19 t 0d h 0a e 00 _ 05 ? 05 ?
            //1e y 14 o 1a u 17 r 00 _ 05 ?
            ReadFromAbbrTable(pathToMiniZork);

            Console.WriteLine("Now reading every abbreviation in order");

            for(int i = 0; i < 96; i++)
            {
                AbbreviationNumber num = new AbbreviationNumber(i);
                var zchars = Zchar.ReadAbbrevTillBreak(num, minizork);
                var normalChars = Zchar.DecodeFromZString(zchars, minizork, false)
                    .ToArray();
                Console.WriteLine($"Abbrev number {i} : {new string(normalChars)}");
            }

            Console.ReadKey();
        }

        private static void ReadDictionaryTillBreak(GameMemory zorkFile)
        {
            DictionaryHelper dictionary = new DictionaryHelper();

            WordAddress startOfEntries = dictionary.GetOffSetOfNthEntry(0, zorkFile);
            WordAddress ptrFirstEntry = new WordAddress(startOfEntries.Value);

            var test = Zchar.ReadWordsTillBreak(ptrFirstEntry, zorkFile);
            char[] result = Zchar.DecodeFromZString(test, zorkFile, permitRecurse: false)
                .ToArray();

            Console.Write(new string(result)); 

        }





        private static void ReadFromAbbrTable(string pathToMiniZork)
        {
            GameMemory minizork = GameMemory.OpenFile(pathToMiniZork);
            WordAddress addressOfPtrToAbbrTable = new WordAddress(24);
            WordAddress ptrToAbbrevTable = new WordAddress(minizork.ReadWord(addressOfPtrToAbbrTable).Value);
            WordAddress decompressedAddressOfFirstAbbreviation = new WordAddress(minizork.ReadWord(ptrToAbbrevTable).Value * 2);

            Word word = minizork.ReadWord(decompressedAddressOfFirstAbbreviation);
            int abbreviationLenght = 0;
            while (!word.IsTerminal())
            {
                abbreviationLenght++;
                Console.Write(Zchar.PrintWordAsZchars(word));
                word = minizork.ReadWord(decompressedAddressOfFirstAbbreviation + abbreviationLenght);
            }
            WordAddress secondAbbrev = AbbreviationTableBase.AddressOfAbbreviationByNumber(new AbbreviationNumber(1), minizork);
            List<Zchar> wholeSecondAbbrev = Zchar.ReadAbbrevTillBreak(new AbbreviationNumber(2), minizork).ToList();
            Console.WriteLine();
            word = minizork.ReadWord(secondAbbrev);
            int len = 0;
            string accumulator = "";
            accumulator += Zchar.PrintWordAsZchars(word);
            while (true)
            {
                len++;
                word = minizork.ReadWord(secondAbbrev + len);
                accumulator += Zchar.PrintWordAsZchars(word);
                if (word.IsTerminal()) break; 
            }
            Console.WriteLine(accumulator);
        }

        private static void TestReadVersionNumberFromFile(string pathToMiniZork)
        {
            GameMemory miniZork = GameMemory.OpenFile(pathToMiniZork);
            ByteAddress versionAddress = new ByteAddress(0);
            byte versionNumber = miniZork.ReadByte(versionAddress);
            Console.WriteLine($"Mini-Zork version {versionNumber}");
        }

        private static void TestReadingAndWritingGameState()
        {
            string storyState = "Listen to a story 'bout a guy named Al";
            string interpreterState = "Who lived in the sewer with his hamster pal";
            int expectedImmutableSize = Encoding.UTF8.GetByteCount(storyState);
            byte[] immutablePart = Encoding.UTF8.GetBytes(storyState);
            int expectedDynamicSize = Encoding.UTF8.GetByteCount(interpreterState);
            byte[] dynamicPart = Encoding.UTF8.GetBytes(interpreterState);
            ImmutableByteWrapper changingPart = new ImmutableByteWrapper(dynamicPart);
            GameMemory gameState = new GameMemory(immutablePart, changingPart);

            WordAddress zeroPointer = new WordAddress(0);
            List<WordAddress> firstFivePointers = Enumerable.Range(0, 5)
                .Select(el => zeroPointer + el)
                .ToList();

            IEnumerable<ByteAddress> firstTenByteAddress = Enumerable.Range(0, 10)
                .Select(el => new ByteAddress(el));

            byte[] firstTenBytes = firstTenByteAddress
                .Select(p => gameState.ReadByte(p))
                .ToArray();

            byte[] firstFiveWords = firstFivePointers
                 .Select(p => gameState.ReadWord(p))
                 .SelectMany(word => new byte[] { (byte)(word.Value & 0xFF), (byte)((word.Value >> 8) & 0xFF) })
                 .ToArray();

            string firstFive = new string(Encoding.UTF8.GetChars(firstFiveWords));

            byte[] firstFiveTransposed = firstFivePointers
                .Select(p => gameState.ReadWord(p))
                .SelectMany(word => new byte[] { (byte)((word.Value >> 8) & 0xFF), (byte)(word.Value & 0xFF) })
                .ToArray();

            string firstFiveTransposedRead = new string(Encoding.UTF8.GetChars(firstFiveTransposed));

            string whatIRead = new String(Encoding.UTF8.GetChars(firstTenBytes));

            //test writing to game state:
            //replace "Who lived " with
            //        "zzCheese2 " 
            byte[] toWrite = Encoding.UTF8.GetBytes("zzCheese2 ");
            System.Diagnostics.Debug.Assert(toWrite.Length == firstTenBytes.Length);

            Enumerable.Range(0, 10)
                .Select(el => new { address = new ByteAddress(el), val = toWrite[el] })
                .ToList()
                .ForEach(item => gameState.WriteByte(item.address, item.val));

            byte[] firstTenBytesAgain = firstTenByteAddress
                .Select(p => gameState.ReadByte(p))
                .ToArray();

            string mutated = new string(Encoding.UTF8.GetChars(firstTenBytesAgain));

            //test writing Words instead of bytes to game state:
            byte[] toWriteWordWise = Encoding.UTF8.GetBytes("aaZZeess33");
            WordAddress zero = new WordAddress(0); 
            for(int i = 0; i < toWriteWordWise.Length - 1; i += 2)
            {
                int val = (toWriteWordWise[i] * 256 + toWriteWordWise[i + 1]);
                WordAddress address = zero + i / 2; 
                gameState.WriteWord(address, new Word(val));
            }
             
            byte[] firstTenBytesAgain2 = firstTenByteAddress
                .Select(p => gameState.ReadByte(p))
                .ToArray();

            string mutated2 = new string(Encoding.UTF8.GetChars(firstTenBytesAgain2)); 

            Console.WriteLine($"{whatIRead} became {mutated} and then {mutated2}");
            
        }

        private static void TestMemoryBase()
        {
            byte[] bytes = Encoding.UTF8.GetBytes("Hello World");
            ImmutableByteWrapper initialState = new ImmutableByteWrapper(bytes);
            ByteAddress address1 = new ByteAddress(1);
            ImmutableByteWrapper edited = initialState.WriteToAddress(address1, 0x00);
            //owerwrite "world"
            byte[] newBytes = Encoding.UTF8.GetBytes("woid!");
            IEnumerable<int> sixThruTen = Enumerable.Range(6, 5);
            var multiEdits = sixThruTen.Select(val =>
            {
                var address = new ByteAddress(val);
                var toWrite = newBytes[val - 6];
                return new Tuple<ByteAddress, byte>(address, toWrite);
            });
            ImmutableByteWrapper bulkEdits = initialState.WriteMultipleAddress(multiEdits);
            var toPrint = ReadAsUtf8String(bulkEdits);
            var oldToPrint = ReadAsUtf8String(initialState);
            Console.WriteLine($"Initial state has {oldToPrint} and bulk edited has {toPrint}");

            int length = initialState.Length;
            Console.WriteLine(edited.ReadAddress(address1));
            Console.WriteLine(initialState.ReadAddress(address1));
        }

        private static string ReadAsUtf8String(ImmutableByteWrapper bulkEdits)
        {
            byte[] asRead = new byte[bulkEdits.Length];
            for (int i = 0; i < bulkEdits.Length; i++)
            {
                asRead[i] = bulkEdits.ReadAddress(new ByteAddress(i));
            }
            return Encoding.UTF8.GetString(asRead);
        }

        private static void TestBitTwiddling()
        {

            int myWord = 0xBEEF;
            int otherWord = 0xF000;

            Word negativeOne = new Word(0xFFFF);
            Word everyOther = new Word(0 + 2 + 8 + 32 + 128 + 512);

            Console.WriteLine(Print3ZcharsFromWord(negativeOne));
            Console.WriteLine(Print3ZcharsFromWord(everyOther));




            Word word = new Word(myWord);
            Word theOtherWord = new Word(otherWord);

            string firstBinary = Convert.ToString(myWord, 2);
            string otherWords = Convert.ToString(otherWord, 2);

            var toPrint = Bits.FetchBits(BitNumber.Bit15, BitSize.Size4, word);
            var otherToPrint = Bits.FetchBits(BitNumber.Bit15, BitSize.Size4, theOtherWord);


            string binary = Convert.ToString(toPrint, 2);
            string secondBinary = Convert.ToString(otherToPrint, 2);

            Console.WriteLine($"{binary} obtained from {firstBinary}");
            Console.WriteLine($"{secondBinary} obtained from {otherWords}");
          
        }

        private static string Print3ZcharsFromWord(Word word)
        {

            int test1 = Bits.FetchBits(BitNumber.Bit14, BitSize.Size5, word);
            int test2 = Bits.FetchBits(BitNumber.Bit9, BitSize.Size5, word);
            int test3 = Bits.FetchBits(BitNumber.Bit4, BitSize.Size5, word);

            string str1 = Convert.ToString(test1, 2);
            string str2 = Convert.ToString(test2, 2);
            string str3 = Convert.ToString(test3, 2);

            List<Zchar> chars = new List<Zchar>
            {
                new Zchar(test1),
                new Zchar(test2),
                new Zchar(test3)
            };

            return Zchar.PrintFromZchar(chars);
        }
    }
}
