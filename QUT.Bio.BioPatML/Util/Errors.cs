using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QUT.Bio.BioPatML.Util
{
    enum ErrorType {
        SequenceRangeNonNegative,
        SequenceRangeStartError,
        SequenceRangeEndError
    }

    static class Errors
    {
        public static String DisplayError(ErrorType type) { 
            switch(type){
                case ErrorType.SequenceRangeNonNegative : return "index cannot be negative";
                case ErrorType.SequenceRangeStartError : return "start index cannot out of range";
                case ErrorType.SequenceRangeEndError: return "end index cannnot be negative";
                default: return String.Empty;
                    
            }
        }     
    }
}
