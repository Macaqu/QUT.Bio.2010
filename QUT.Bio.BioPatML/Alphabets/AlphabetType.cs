﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QUT.Bio.BioPatML.Alphabet
{
    /// <summary> The alphabet types supported by BioPatML.
    /// </summary>

    public enum AlphabetType
    {
        /// <summary> Unknown alphabet: try to recognise. </summary>
        UNKNOWN,

        /// <summary> DNA Alphabet. </summary>
        DNA,

        /// <summary> RNA Alphabet. </summary>
        RNA,

        /// <summary> Protein Alphabet. </summary>
        AA,

        
        PROTEIN = AA
    }


}
