﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using QUT.Bio.BioPatML.Sequences;
//using QUT.Bio.BioPatML.Symbols;
using QUT.Bio.BioPatML.Common.XML;
using Bio;

/*****************| Queensland University Of Technology |********************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
namespace QUT.Bio.BioPatML.Patterns
{
    /// <summary>
    /// Part of Repeat class
    /// </summary>
    public partial class Repeat : Pattern
    {
        /// <summary>
        /// The direct matcher for our repeat element
        /// </summary>
        private class MatcherDirect : MatcherRepeat
        {
            #region -- Constructor --
            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="repeat"></param>
            public MatcherDirect(Repeat repeat)
                : base(repeat) { }

            #endregion

            #region -- IMatcher Implementation --

            /// <summary>
            /// The direct Match algorithm for matching
            /// </summary>
            /// <param name="sequence">sequence to compare</param>
            /// <param name="position">matching position</param>
            /// <returns></returns>
            public override Match Match(ISequence sequence, int position)
            {
                Init();

                for (int i = 0; i < matchLen; i++)
                {
                    remSim -= (1.0 - Compare((char)matchSeq[1 + i], (char)sequence[position + i]));

                    if (remSim / matchLen < repeat.Threshold)
                        return null;
                }

                repeat.LatestMatch.Set(sequence, position, matchLen, Strand.Forward, remSim / matchLen);

                return repeat.LatestMatch;
            }

            #endregion
        }
    }
}
