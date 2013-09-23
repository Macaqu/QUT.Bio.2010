using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bio;
using QUT.Bio.BioPatML.Sequences;

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
    /// This class implements a pattern series which returns the best match
    /// of all patterns in the series for a given position. 
    /// </summary>
    public class SeriesBest : Series
    {
        #region -- Constructors --

        /// <summary> Parameterless constructor for deserialisation. 
        /// </summary>
        public SeriesBest()  {}

        /// <summary>
        /// Constructs an empty series.
        /// </summary>
        /// <param name="name">Name of pattern</param>
        /// <param name="threshold">Similarity threshold. </param>
        public SeriesBest( 
			string name, 
			double threshold
		)
            : base (name)
        {
            Threshold = threshold;
        }

        #endregion

        #region -- Public Methods --

        /// <summary>
        /// The implementation ensures that
        /// a match fails for a given position if there is no match. Otherwise the
        /// matcher might return a match at a different position.
        /// <see cref="QUT.Bio.BioPatML.Patterns.IPattern">IPattern Match(Sequence, int) method</see>
        /// </summary>
        /// <param name="sequence"> The sequence for comparing</param>
        /// <param name="position"> Matching position</param>
        /// <returns></returns>
        public override Match Match(ISequence sequence, int position)
        {
			Match match = LatestMatch;
            Match bestMatch = match.Clone();
            Match nextMatch = null;
            int patternNumber = Patterns.Count;
            int index = 0;

            bestMatch.Similarity = -1.0;

            while (index >= 0)
            {
                IPattern pattern = this[index];

                position = index > 0 ? (int)match.SubMatches[index - 1].End + 1 : position;
                nextMatch = pattern.Match(sequence, position);

                if (nextMatch == null)
                {
                    while (--index >= 0 && this[index].Increment > 0) ;
                    continue;
                }

                match.SubMatches[index].Set(nextMatch);

                index++;

                if (index == patternNumber)
                {
                    while (--index >= 0 && this[index].Increment > 0) ;
					match.CalcSimilarity();

					if ( match.Similarity >= Threshold 
						&& match.Similarity > bestMatch.Similarity 
					) {
						bestMatch.Set( match );
					}
                }
            }

            if (bestMatch.Similarity < 0)
                return (null);

            LatestMatch.Set(bestMatch);
			LatestMatch.CalcStartEnd();
			LatestMatch.Strand = Strand.Forward;
			LatestMatch.BaseSequence = sequence;

            return bestMatch;
        }

        #endregion
    }
}
