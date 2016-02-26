﻿using System;
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
            TestMemoryBase();
            TestReadingAndWritingGameState();
            Console.ReadKey();
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
            IEnumerable<WordAddress> firstFivePointers = Enumerable.Range(0, 5)
                .Select(el => zeroPointer + el);

            IEnumerable<ByteAddress> firstTenByteAddress = Enumerable.Range(0, 10)
                .Select(el => new ByteAddress(el));

            byte[] firstTenBytes = firstTenByteAddress
                .Select(p => gameState.ReadAddress(p))
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
                .Select(p => gameState.ReadAddress(p))
                .ToArray();

            string mutated = new string(Encoding.UTF8.GetChars(firstTenBytesAgain)); 


            Console.WriteLine($"{whatIRead} became {mutated}");
            
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
            Console.ReadKey();
        }
    }
}
