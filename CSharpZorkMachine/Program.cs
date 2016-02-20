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
            byte[] bytes = new byte[12];
            Random rand = new Random();
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)rand.Next(256);
            }
            ImmutableByteWrapper initialState = new ImmutableByteWrapper(bytes);
            ByteAddress address1 = new ByteAddress(1);
            ImmutableByteWrapper edited = initialState.WriteToAddress(address1, 0x00);
            Console.WriteLine(edited.ReadAddress(address1));
            Console.WriteLine(initialState.ReadAddress(address1));
            Console.ReadKey();
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
