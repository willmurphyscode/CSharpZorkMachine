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
            //TestBitTwiddling();
            //char[] helloWorld = "hello world!".ToCharArray();
            TestMemoryBase();
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
            Console.ReadKey();
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
