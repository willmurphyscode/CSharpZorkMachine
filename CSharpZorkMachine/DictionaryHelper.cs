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



        /*
         Structure of Dictionary Header:
         [0] => n := byte containing number of word separator characters
         [1:n] => word separator characters, I believe in ASCII 
         [n+1] => size := size of each entry
         [n+2:n+3] => as word, numOfEntries := number of entries
         [n+4:n+(numOfEntires * 2 * size)] => the actual entries

            NOTE that we care about both "entry number" (index into this structure) 
            AND about the entry address (offset into file of a given entry)
             
        */

        public WordAddress GetOffSetOfNthEntry(int n, GameMemory zorkFile)
        {
            ByteAddress startOfDictionary = ptrToDictionary(zorkFile);
            int dictionaryBaseOffset = startOfDictionary.Value;
            int sizeOfSepCountEntry = 1; 
            int numberOfSeps = CountOfSeparatorCharacters(zorkFile);
            int sizeOfSizeEntry = 1;
            int sizeOfEntryCountEntry = 2;
            int offsetFirstEntry = dictionaryBaseOffset + 
                                    sizeOfSepCountEntry + 
                                    numberOfSeps + 
                                    sizeOfSizeEntry + 
                                    sizeOfEntryCountEntry;

            ByteAddress ptrSizeEntry = new ByteAddress(startOfDictionary.Value + numberOfSeps + 1);
            int sizeOfEntry = zorkFile.ReadByte(ptrSizeEntry);

            return new WordAddress(offsetFirstEntry + (n * sizeOfEntry));
        }

        public int GetEntrySizeInBytes(GameMemory zorkFile )
        {
            ByteAddress startOfDictionary = ptrToDictionary(zorkFile);
            int numberOfSeps = CountOfSeparatorCharacters(zorkFile);

            ByteAddress ptrSizeEntry = new ByteAddress(startOfDictionary.Value + numberOfSeps + 1);
            int sizeOfEntry = zorkFile.ReadByte(ptrSizeEntry);
            return sizeOfEntry;
        }

        public Word[] GetContentsOfNthEntry(int n, GameMemory zorkFile)
        {
            int entrySize = GetEntrySizeInBytes(zorkFile);
            int entrySizeInWords = entrySize / 2;

            Word[] retval = new Word[entrySizeInWords];
            WordAddress ptrStartOfEntry = GetOffSetOfNthEntry(n, zorkFile);

            for(int i = 0; i < entrySizeInWords; i++)
            {
                WordAddress here = new WordAddress(ptrStartOfEntry.Value + (i * 2));
                retval[i] = zorkFile.ReadWord(here);
            }


            return retval; 
        }

        public DictionaryEntry ReadNthEntry(int n, GameMemory zorkFile)
        {
            WordAddress offset = GetOffSetOfNthEntry(n, zorkFile);
            Word[] entryContents = GetContentsOfNthEntry(n, zorkFile);

            List <Zchar> entryZchars = new List<Zchar>();
            foreach(Word contents in entryContents)
            {
                var zchars = Zchar.ReadWordAsZchars(contents);
                entryZchars.AddRange(zchars);
            }
            var chars = Zchar.DecodeFromZString(entryZchars, zorkFile);
            
            return new DictionaryEntry(
                print: new string(chars.ToArray()),
                ix: n,
                offset: offset);
        }


        public ByteAddress ptrToDictionary(GameMemory zorkFile)
        {
            return new ByteAddress(zorkFile.ReadWord(ptrDictionaryPtr).Value);
        }

        public int CountOfSeparatorCharacters(GameMemory zorkFile)
        {
            return zorkFile.ReadByte(ptrToDictionary(zorkFile));
        }

        public int CountOfEntries(GameMemory zorkFile)
        {
            ByteAddress startOfDictionary = ptrToDictionary(zorkFile);
            int countSepCharacters = CountOfSeparatorCharacters(zorkFile);
            WordAddress ptrToEntryCount = new WordAddress(startOfDictionary.Value + 1 + countSepCharacters + 2);
            return zorkFile.ReadWord(ptrToEntryCount).Value;
        }




        public ISet<char> Separators(GameMemory zorkFile)
        {
            //TODO read separators from dictionary. 
            return new HashSet<char>(new char[] { ' ' });
        }

        public struct DictionaryEntry
        {
            public DictionaryEntry(string print, WordAddress offset, int ix)
            {
                this.Print = print;
                this.OffsetIntoFile = offset;
                this.IndexIntoDictionary = ix; 
            }
            public string Print { get; }
            public WordAddress OffsetIntoFile { get; }
            public int IndexIntoDictionary { get; }
        }

    }
}
