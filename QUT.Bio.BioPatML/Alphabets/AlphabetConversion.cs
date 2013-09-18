using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bio;

namespace QUT.Bio.BioPatML.Alphabets
{
    public static class AlphabetConversion
    {
        public static IAlphabet Convert(AlphabetType type) { 
            switch(type){
                case AlphabetType.DNA: return DnaAlphabet.Instance;
                case AlphabetType.RNA: return RnaAlphabet.Instance;
                case AlphabetType.AA: return ProteinAlphabet.Instance;
                default: return null;
            }
        }

        public static bool IsValidSymbols(IAlphabet alphabet, char symbol) {
            
            if (alphabet.GetValidSymbols().Contains((byte)symbol)) {
                return true;
            }

            return false;
        }
    }
}
