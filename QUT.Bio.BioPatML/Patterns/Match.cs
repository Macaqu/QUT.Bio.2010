using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QUT.Bio.BioPatML.Sequences;
//using QUT.Bio.BioPatML.Sequences.List;
using System.Xml.Linq;
using QUT.Bio.BioPatML.Common.XML;
using Bio;

/*****************| Queensland University Of Technology |********************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 ***************************************************************************/

// TODO: Review the sequence class hierarchy; replace Sequence with an implementation of MBF.ISequence; a Match should be a Feature but not a Sequence.

namespace QUT.Bio.BioPatML.Patterns
{
    /// <summary> The match class is an extension of a {Feature} and stores information 
    ///  about a match between a pattern and a sequence. A match object can contain 
    ///  sub matches (which is are organized as a {FeatureList}) when the pattern
    ///  is composed of sub patterns, e.g. {ProfileAll} patterns.
    /// </summary>

    public sealed class Match : Feature, ICloneable
    {

        private readonly List<Match> subMatches = new List<Match>();
        private double similarity;
        private double impact = 1.0;
        
        /** <summary> The similarity of the match within the interval [0,1] 1.0 means perfect/maximum match.  </summary> */

        public Double Similarity
        {
            get
            {
                return similarity;
            }
            set
            {
                // TODO: make Similarity be something derived from an actual match, and remove this mutator.

                if (value < 0 || value > 1.0) throw new ArgumentException("Similarity must be between 0.0 and 1.0 inclusive.");

                similarity = value;
            }
        }

        /// <summary> Reference to the pattern the match belongs to.
        /// </summary>

        public IPattern MatchPattern { get; set; }

        /// <summary> Creates a match object. </summary>
        /// <param name="pattern"> Pattern the match belongs to. </param>

        public Match(IPattern pattern) : base() { this.MatchPattern = pattern; }

        /// <summary> Creates a match object.
        /// </summary>
        /// <param name="pattern">The referenced matching pattern</param>
        /// <param name="sequence">Sequence the match was found on.</param>
        /// <param name="start">Start position of the match.</param>
        /// <param name="length">Length of the match.</param>
        /// <param name="strand">Strand the match belongs to. +1 = forward strand, 
        /// -1 = backward strand, 0 = n.a. or unknown.</param>
        /// <param name="similarity"></param>

        public Match(
            IPattern pattern,
            ISequence sequence,
            int start,
            int length,
            Strand strand,
            double similarity
        )
            : base()
        {
            this.MatchPattern = pattern;
            Set(sequence, start, length, strand, similarity);
        }

        public Match(ISequence sequence, int start, int length, Sequences.Strand strand, double similarity)
            : base()
        {
            // TODO: Complete member initialization
                
            this.BaseSequence = sequence;
            this.Start = start - 1;
            this.End = (long)(length + Start - 1);
            this.Strand = strand;
            this.similarity = similarity;
        }


        /// <summary> Setter for sequence, start, length, strand and similarity
        /// </summary>
        /// <param name="seq">Sequence the match belongs to.</param>
        /// <param name="start">Start position of the match.</param>
        /// <param name="length">Length of the match,</param>
        /// <param name="strand">Strand the match belongs to. +1 = forward strand, 
        /// -1 = backward strand, 0 = n.a. or unknown.</param>
        /// <param name="similarity">Similarity of the match. Should be in interval [0,1].</param>

        public void Set(
            ISequence seq,
            int start,
            int length,
            Strand strand,
            double similarity
        )
        {
            base.Set(start, start + length - 1, strand);
            Set(seq, strand, similarity);
        }

        /// <summary>
        /// Setter for sequence,  strand and similarity.
        /// </summary>
        /// <param name="seq">Sequence the match belongs to.</param>
        /// <param name="strand">
        /// Strand the match belongs to. +1 = forward strand, 
        /// -1 = backward strand, 0 = n.a. or unknown.
        /// </param>
        /// <param name="similarity">
        /// Similarity of the match. Should be in interval [0,1].
        /// </param>

        public void Set(
            ISequence seq,
            Strand strand,
            double similarity
        )
        {
            base.Strand = strand;
            this.BaseSequence = seq;
            this.similarity = similarity;
        }

        /// <summary> Replaces the contents of this Match object with that of another. </summary>
        /// <param name="match">Match object with initial values.</param>

        public void Set(Match match)
        {
            if (this == match) return;

            Set(
                match.BaseSequence,
                (int)match.Start,
                (int)match.BaseSequence.Count,
                match.Strand,
                match.Similarity
            );

            this.MatchPattern = match.MatchPattern;
            this.impact = match.impact;

            subMatches.Clear();
            subMatches.AddRange(match.subMatches.Select(m => m.Clone()));
        }

        /// <summary> Calculates the number of matches based on similarity and match length. </summary>

        public int Matches
        {
            get
            {
                return ((int)(Similarity * this.BaseSequence.Count));
            }

        }

        /// <summary> Gets/Sets the impact of the match criteria.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when weight value is less than 0 or more than 1.0
        /// </exception>

        public double Impact
        {
            get
            {
                return this.impact;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException("Invalid weight : " + value);
                }

                this.impact = value;
            }
        }

        /// <summary> Retrieve the matched listing </summary>

        public IList<Match> SubMatches
        {
            get
            {
                return subMatches;
            }
            //set {
            //    AnnotatedList<FeatureList> lists = FeatureLists;

            //    if ( value != null )

            //        if ( lists.Count == 0 )
            //            lists.Add( value );
            //        else
            //            lists.Set( 0, value );

            //    else
            //        if ( lists.Count > 0 )
            //            lists.RemoveAt( 0 );
            //}
        }

        /// <summary> Calculates the number of mismatches based on similarity and match length.
        /// </summary>
        /// <returns></returns>

        public int CalcMismatches()
        {
            return ((int)(this.BaseSequence.Count - Similarity * this.BaseSequence.Count));
        }

        /// <summary> Calculates the maximum start and end position over all submatches of
        /// the current match to determine the match length and sets the corresponding
        /// parameters (start, length) of the match. If the match does not
        /// contain any submatches the method returns without changing any
        /// parameters.
        /// </summary>

        public void CalcStartEnd()
        {
            int minStart = int.MaxValue;
            int maxEnd = int.MinValue;

            foreach (var match in subMatches)
            {
                if (match.Start < minStart) minStart = (int) match.Start;
                if (match.End > maxEnd) maxEnd = (int) match.End;
            }

            Start = minStart;
            End = maxEnd;
        }

        /// <summary> Calculates the mean similarity over all submatches and sets the
        /// similarity parameter. If the match does not contain any submatches 
        /// the method returns without changing any parameters.
        /// </summary>

        public void CalcSimilarity()
        {
            if (subMatches.Count > 0)
            {
                double sum = 0.0;
                double wsum = 0.0;

                foreach (var match in subMatches)
                {
                    match.CalcSimilarity();
                    sum += match.Similarity * match.impact;
                    wsum += match.impact;
                }

                similarity = wsum > 0 ? sum / wsum : 0.0;
            }
        }

        /// <summary>
        ///  Calculates the pure length of match which is the length without gaps and
        ///  without taking overlaps into account. This is the length of the match 
        ///  itself (if it has no sub matches) or the sum of the lengths of the sub
        ///  matches. 
        /// </summary>
        /// <returns></returns>

        public int CalcLength()
        {
            return subMatches.Count > 0
                ? subMatches.Sum(match => match.CalcLength())
                : (int)this.Count;
        }

        /// <summary> Creates a string representation.
        /// </summary>
        /// <returns>Returns a string representation of a match. </returns>

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{" + Start + ", " + this.Count + ", " + Strand + ", " +
                        (int)(Similarity * 100) / 100.0 + ", " + Letters());

            foreach (var subMatch in subMatches)
            {
                sb.Append(", " + subMatch.ToString());
            }

            sb.Append("}");

            return (sb.ToString());
        }

        public string Letters()
        {
            if (this.BaseSequence != null)
            {
                ISequence subSequence = this.BaseSequence.GetSubSequence(this.Start, this.End - this.Start + 1);
                if (Strand == QUT.Bio.BioPatML.Sequences.Strand.Reverse)
                {
                    return subSequence.GetReverseComplementedSequence().ToString().ToLower();
                }
                return subSequence.ToString();
            }
            
            return null;
        }

        /// <summary> Return a new copy of this Match
        /// </summary>
        /// <returns></returns>

        object System.ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary> Creates a deep copy of a match object.
        /// </summary>
        /// <returns>Returns a copy of the match object.</returns>

        public Match Clone()
        {
            Match newMatch = new Match(null);
            newMatch.Set(this);
            return newMatch;
        }

        /// <summary> Loads content into this match from an Xml element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="context"></param>

        public override void Parse(XElement element, Sequence context)
        {
            base.Parse(element, context);

            XElement subMatchElements = element.Element("SubMatches");

            if (subMatchElements != null)
            {
                foreach (XElement subMatchElement in subMatchElements.Elements("SubMatch"))
                {
                    Match subMatch = new Match(null) { BaseSequence = context };
                    subMatch.Parse(subMatchElement, context);
                }
            }

            impact = element.Double("Impact", 1.0);
            Similarity = element.Double("Similarity");
        }

        /// <summary> Gets an Xml representation of this Match.
        /// </summary>
        /// <returns></returns>

        public override XElement ToXml()
        {
            XElement result = base.ToXml();
            result.Name = "Match";

            if (subMatches.Count > 0)
            #region Add <SubMatches> child element.
            {
                XElement subMatchElement = new XElement("SubMatches");
                result.Add(subMatchElement);

                foreach (var subMatch in subMatches)
                {
                    subMatchElement.Add(subMatch.ToXml());
                }
            }
            #endregion

            if (impact != 1) result.Add(new XAttribute("Impact", impact));

            result.Add(new XAttribute("Similarity", similarity));

            return result;
        }

        /// <summary>
        ///  Searches for the pattern with the highest similarity within the
        ///  given sequence. Search finds only matches of the pattern which are
        ///  within the specified range. 
        ///  <para></para>
        ///  To find all matches in a sequence just call SearchBest(0,0, pattern) or
        ///  SearchBest(1,1, pattern) for example.
        ///  <para></para>
        ///  To find all matching patterns of a circular genome the pattern length has 
        ///  to be added to the sequence length:
        ///  <example>
        /// <code>
        ///  SearchBest(1, seq.length()+pattern.length()-1, pattern);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="startPos"> Start position for search (first position is one).
        /// The start position can be negative.</param>
        /// <param name="endPos">End position for search. If endPos less than or equals startPos the
        /// start position will be set to one and the end position will be set to the 
        /// length of the sequence. The end position can be greater than the length of 
        /// the sequence (to search across the genome boundary in circular genomes
        /// for instance).
        /// </param>
        /// <param name="pattern"> 
        /// An object which implements the <see cref="QUT.Bio.BioPatML.Patterns.IPattern">IPattern</see> interface,
        /// e.g. a sequence or a start weight matrix. 
        /// </param>
        /// <returns> 
        /// Returns a match object which describes the best match of
        /// the pattern.
        /// </returns>
        public Match SearchBest(int startPos, int endPos, IPattern pattern)
        {
            Match maxMatch = new Match(null, null, 0, 0, Strand.Forward, -1.0);

            if (endPos <= startPos)
            {
                startPos = 1;
                endPos = startPos + (int)Count - 1;
            }

            while (startPos <= endPos)
            {
                Match match = pattern.Match(this, startPos);
                if (match != null
                    && match.Similarity > maxMatch.Similarity
                    && match.End <= endPos)
                    maxMatch.Set(match);

                startPos += pattern.Increment;
            }

            if (maxMatch.Similarity < 0) //nothing found
                return (null);

            return (maxMatch);
        }


        /// <summary>
        /// Searches for the pattern within the given sequence and creates a 
        /// <see cref="QUT.Bio.BioPatML.Sequences.List.FeatureList"> FeatureList </see> containning all matches.
        /// <para></para>
        /// Search finds only matches of the pattern which are within the specified range. 
        /// To find all matches in a sequence just call Search(0,0, pattern) or
        /// Search(1,1, pattern) for example.
        /// <para></para>
        /// To find all matching patterns of a circular genome the pattern length has 
        /// to be added to the sequence length:
        /// 
        /// search(1, seq.length()+pattern.length()-1, pattern);
        /// 
        /// </summary>
        /// <param name="startPos"> Start position for search (first position is one).
        /// The start position can be negative. </param>
        /// <param name="endPos"> End position for search. If endPos less than or equals startPos the
        /// start position will be set to one and the end position will be set to the 
        /// length of the sequence. The end position can be greater than the length of 
        /// the sequence (to search across the genome boundary in circular genomes
        /// for instance).</param>
        /// <param name="pattern"> An object which implements the 
        /// <see cref="QUT.Bio.BioPatML.Patterns.IPattern">IPattern</see>
        /// interface. </param>
        /// <returns> Returns a <see cref="QUT.Bio.BioPatML.Sequences.List.FeatureList"> FeatureList </see>
        /// with all matches. 
        /// </returns>
        public List<Match> Search(int startPos, int endPos, IPattern pattern)
        {
            List<Match> featureList = new List<Match>();
            Match match;

            if (endPos <= startPos)
            {
                startPos = 1;
                endPos = startPos + (int)Count - 1;
            }

            while (startPos <= endPos)
            {
                // TODO: Clean up.

                match = pattern.Match(this, startPos);

                if (match != null && match.End <= endPos)
                {
                    featureList.Add(match.Clone());
                }

                startPos += pattern.Increment;
            }

            return (featureList);
        }
    }
}
