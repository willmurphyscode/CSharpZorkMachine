using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpZorkMachine
{
    public class DictionaryHelper
    {
        private const int DICTIONARY_LOCATION_OFFSET = 8; 
        public WordAddress ptrDictionaryPtr { get { return new WordAddress(DICTIONARY_LOCATION_OFFSET); } }

        public ByteAddress ptrToDictionary(GameMemory zorkFile)
        {
            return new ByteAddress(zorkFile.ReadWord(ptrDictionaryPtr).Value); 
        }

        public int CountOfSeparatorCharacters(GameMemory zorkFile)
        {
            return zorkFile.ReadByte(ptrToDictionary(zorkFile)); 
        }

        public ByteAddress StartOfEntries(GameMemory zorkFile)
        {
            ByteAddress startOfDictionary = ptrToDictionary(zorkFile);
            int countSepCharacters = CountOfSeparatorCharacters(zorkFile);

            return new ByteAddress(startOfDictionary.Value + countSepCharacters); 
        }

        public string ReadNthEntry(int n, GameMemory zorkFile)
        {

            int hardCodedOffset = 0x285a;
            ByteAddress startOfDictionary = StartOfEntries(zorkFile);
            int countSepCharacters = CountOfSeparatorCharacters(zorkFile);
            ByteAddress hardCodedStart = new ByteAddress(hardCodedOffset + 3);
            bool test = hardCodedStart.Value == startOfDictionary.Value; 
            WordAddress ptrFirst = new WordAddress(startOfDictionary.Value + (n * 4));
            Word first = zorkFile.ReadWord(ptrFirst);
            IEnumerable<Zchar> FirstHalf = Zchar.ReadWordAsZchars(first);
            WordAddress ptrSecond = ptrFirst + 1;
            Word second = zorkFile.ReadWord(ptrSecond);
            IEnumerable<Zchar> SecondHalf = Zchar.ReadWordAsZchars(second);
            char[] retval = Zchar.DecodeFromZString(FirstHalf.Concat(SecondHalf), zorkFile).ToArray();
            return new string(retval);
        }

        public ISet<char> Separators(GameMemory zorkFile)
        {
            //TODO read separators from dictionary. 
            return new HashSet<char>(new char[] { ' ' });
        }

    }
}
