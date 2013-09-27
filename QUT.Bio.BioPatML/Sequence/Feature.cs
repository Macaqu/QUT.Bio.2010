using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QUT.Bio.BioPatML.Sequences.Annotations;
using QUT.Bio.BioPatML.Common.XML;
using Bio;
using QUT.Bio.BioPatML.Util;
using System.Xml.Linq;


/*****************| Queensland  University Of Technology |*******************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
namespace QUT.Bio.BioPatML.Sequences
{
    /// <summary>
    /// This class describes a feature. A feature stores information about
    /// a region of a sequence and is described by a region, a list of
    /// properties and a reference to the sequence the feature is related to.
    /// <para></para>
    /// Note that a feature extends the sequence class and all methods which
    /// are available for sequences are therefore also applicable to 
    /// features.
    /// </summary>
    public class Feature : ISequence, ISequenceRange
    {
        private long start;
        private long end;
        private string id;
        private ISequence sequence;
        private Sequences.Strand strand;

        public Feature() { }

        /// <summary>
        ///  Creates a feature with the given name. A feature contains information
        ///  about a section of a sequence, e.g. locus of a certain gene.
        /// </summary>
        /// <param name="name"> Name of the feature, e.g. name of a gene. </param>
        /// <param name="start">
        /// Start position of the feature within the sequence. 
        /// The first symbol in a sequence has start one and the start
        /// refers to the forward strand.
        /// </param>
        /// <param name="end">
        /// End position of the feature within the sequence. First position
        /// is one. End position must be bigger than the start position.
        /// </param>
        /// <param name="strand">
        /// Strand the feature belongs to. +1 = forward strand, 
        /// -1 = backward strand, 0 = n.a. or unknown. 
        /// </param>

        public Feature(
            String name,
            int start,
            int end,
            Strand strand,
            ISequence baseSequence
        )
        {
            this.ID = name;
            Set(start, end, strand);
            this.BaseSequence = baseSequence;
        }

        internal void Set(int start, int end, Strand strand)
        {
            this.Start = start;
            this.End = end;
            this.Strand = strand;
        }

        /// <summary> Calculates the distance between the start positions of two features. If the 
        /// second feature has a smaller position than the current feature it is assumed 
        /// that the sequence is cyclic and the distance is calculated the other way
        /// around. Therefore the distance is always positive. 
        /// Note that the feature MUST be attached to a sequence! Otherwise a null
        /// pointer exception will occur.
        /// </summary>
        /// <param name="other"> Second feature. </param>
        /// <returns> 
        /// Returns the distance between the two features (feature.start -
        /// this.start if feature.start >= this.start).
        /// </returns>

        public int DistanceBetweenStartPositions(Feature other)
        {
            return (int)(other.Start < Start ? BaseSequence.Count - Start + other.Start
                                       : other.Start - Start);
        }

        /// <summary> Calculates the distance between the end positions of two features. If the 
        /// second feature has a smaller position than the current feature it is assumed
        /// that the sequence is cyclic and the distance is calculated the other way
        /// around. Therefore the distance is always positive. 
        /// Note that the feature MUST be attached to a sequence! Otherwise a null
        /// pointer exception will occur.
        /// </summary>
        /// <param name="other"> Second feature. </param>
        /// <returns>
        /// Returns the distance between the two features (feature.end -
        /// this.end if feature.end >= this.end).
        /// </returns>

        public int DistanceBetweenEndPositions(Feature other)
        {
            return (int)(other.End < End ? BaseSequence.Count - End + other.End
                                   : other.End - End);
        }

        /// <summary> Calculates the distance between the start position of the current feature
        ///  and the end position of the second feature. If the  second feature has a 
        ///  smaller end position than the current feature it is assumed 
        ///  that the sequence is cyclic and the distance is calculated the other way
        ///  around. Therefore the distance is always positive. 
        ///  Note that the feature MUST be attached to a sequence! Otherwise a null
        ///  pointer exception will occur.
        /// </summary>
        /// <param name="feature"> Second feature. </param>
        /// <returns>
        /// Returns the distance between the two features (feature.end -
        /// this.start if feature.end >= this.start).
        /// </returns>
        public int DistanceStartEnd(Feature feature)
        {
            if (feature.End < this.Start)
                return (int)(this.BaseSequence.Count - this.Start + feature.End);

            return (int)(feature.End - this.Start);
        }

        /// <summary>
        /// Calculates the distance between the end position of the current feature
        /// and the start position of the second feature. If the  second feature has a 
        /// smaller start position than the current feature it is assumed 
        /// that the sequence is cyclic and the distance is calculated the other way
        /// around. Therefore the distance is always positive. 
        /// Note that the feature MUST be attached to a sequence! Otherwise a null
        /// pointer exception will occur.
        /// </summary>
        /// <param name="feature"> Second feature. </param>
        /// <returns> Returns the distance between the two features (feature.start -
        /// this.end if feature.start >= this.end). 
        /// </returns>
        public int DistanceEndStart(Feature feature)
        {
            if (feature.Start < this.End)
                return (int)(this.BaseSequence.Count - this.End + feature.Start);

            return (int)(feature.Start - this.End);
        }

        /// <summary>
        /// Determines if the region of the current feature overlaps with the region
        /// of the given feature.
        /// </summary>
        /// <param name="feature"> A feature. </param>
        /// <returns>
        /// True, if the regions of the features are overlapping. False,
        /// otherwise.
        /// </returns>
        public bool IsOverlapping(Feature feature)
        {
            if (feature.End >= this.Start
                        && feature.End <= this.End)
                return true;

            if (feature.Start >= this.Start
                        && feature.Start <= this.End)
                return true;

            if (feature.Start <= this.Start
                        && feature.End >= this.End)
                return true;


            return false;
        }


        /// <summary>
        ///  Calculates the upstream distance to the given feature. Attention:  The
        ///  calculated distance can be negative if the features are overlapping. 
        ///  The method takes into account circular sequences and features 
        ///  on both strands.
        /// </summary>
        /// <param name="feature">
        ///  Feature the upstream distance is calculated to. If the
        ///  feature is not in the upstream region to the end of the sequnce but the
        ///  sequence is circular, the distance is calculated accross the sequence 
        ///  boundary. For linear sequences the distance to the corresponding sequence 
        ///  terminus is calculated. 
        /// </param>
        /// <returns></returns>

        public int DistanceUpstream(Feature feature)
        {
            int dist = (int)(Strand == Strand.Forward ? Start - feature.End : feature.Start - End) - 1;

            if ((int)(feature.Start - Start) * Strand > 0)
            {
                dist = (int)(Strand == Strand.Forward ? Start - 1 : BaseSequence.Count - End);
            }

            return dist;
        }

        /// <summary> Calculates the downstream distance to the given feature. Attention:  The
        /// calculated distance can be negative if the features are overlapping. 
        /// The method takes into account circular sequences and features 
        /// on both strands.
        /// </summary>
        /// <param name="feature">
        /// Feature the downstream distance is calculated to. If the
        /// feature is not in the downstream region to the end of the sequnce but the
        /// sequence is circular, the distance is calculated accross the sequence 
        /// boundary. For linear sequences the distance to the corresponding sequence 
        /// terminus is calculated. 
        /// </param>
        /// <returns>
        /// Returns the downstream distance to the given feature.
        /// </returns>

        public int DistanceDownstream(Feature feature)
        {
            int dist = (int)(this.Strand == Strand.Forward ? feature.Start - End : Start - feature.End) - 1;

            if ((int)(End - feature.End) * Strand > 0)
            {
               dist = (int)(Strand == Strand.Forward ? Count - End : Start - 1);
            }

            return dist;
        }

        /// <summary> Creates a string representation of a feature.
        /// </summary>
        /// <returns> Representation of a feature. </returns>

        public override string ToString()
        {
            /*
            StringBuilder sb = new StringBuilder(Name + ": " + BaseSequence + "'\n\t{ " + Start + ", " + End + ", " + Strand + " }\n");

            AnnotationList annotations = base.Annotations;

            foreach (var a in base.Annotations)
            {
                sb.Append("\t" + a.Name + "=" + a.Value + "\n");
            }

            return sb.ToString();
             
            */return String.Empty;
        }

        public IAlphabet Alphabet
        {
            get { 
                if(BaseSequence != null){
                    return this.BaseSequence.Alphabet;
                }
                return null;
            }
        }


        public long Count
        {
            get { 
                if(BaseSequence != null){
                    long maxCount =  this.BaseSequence.Count;
                    long count = End - Start + 1;
                    if (maxCount > count)
                    {
                        return count;
                    }
                    else 
                    {
                        return maxCount;
                    }
                }
                return 0;
            }
        }

        public ISequence GetComplementedSequence()
        {
            if(BaseSequence != null){
                return this.BaseSequence.GetComplementedSequence();
            }
            return null;
        }

        public ISequence GetReverseComplementedSequence()
        {
            if(BaseSequence != null){
                return this.BaseSequence.GetReverseComplementedSequence();
            }
            return null;
        }

        public ISequence GetReversedSequence()
        {
            if(BaseSequence != null){
                return this.BaseSequence.GetReversedSequence();
            }
            return null;
        }
            
        public ISequence GetSubSequence(long start, long length)
        {
            if(BaseSequence != null){
                return this.BaseSequence.GetSubSequence(start, length);
            }
            return null;
        }

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        public long IndexOfNonGap(long startPos)
        {
            if (BaseSequence == null)
            {
                throw new ArgumentNullException("sequence object is not implemented yet");
            }
            
            return this.BaseSequence.IndexOfNonGap(startPos);
           
        }

        public long IndexOfNonGap()
        {
            if (BaseSequence == null)
            {
                throw new ArgumentNullException("sequence object is not implemented yet");
            }
            
            return this.BaseSequence.IndexOfNonGap();
            
        }

        public long LastIndexOfNonGap(long endPos)
        {
            if (BaseSequence == null)
            {
                throw new ArgumentNullException("sequence object is not implemented yet");
            }
            
            return this.BaseSequence.LastIndexOfNonGap(endPos);
            
        }

        public long LastIndexOfNonGap()
        {
            if (BaseSequence == null)
            {
                throw new ArgumentNullException("sequence object is not implemented yet");
            }
            
                return this.BaseSequence.LastIndexOfNonGap();
        }



        public Dictionary<string, object> Metadata
        {
            get {
                if (BaseSequence == null)
                {
                    throw new ArgumentNullException("sequence object is not implemented yet");
                }
                
                return this.BaseSequence.Metadata; 
                
            }
        }

        public byte this[long index]
        {
            get { 
                if(BaseSequence == null){
                    throw new ArgumentNullException("sequence object is not implemented yet");
                }

                return this.BaseSequence[index];
            }
        }

        public IEnumerator<byte> GetEnumerator()
        {
            if (BaseSequence == null)
            {
                throw new ArgumentNullException("sequence object is not implemented yet");
            }

            for (int i = (int)Start; i <= (int)End; i++ ) {
                yield return BaseSequence[(long)i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (BaseSequence == null)
            {
                throw new ArgumentNullException("sequence object is not implemented yet");
            }
            return this.BaseSequence.GetEnumerator();
        }

        public long End
        {
            get {
                if (BaseSequence == null)
                {
                    throw new ArgumentNullException("sequence object is not implemented yet");
                }

                if (end <= BaseSequence.Count - 1)
                    return end;
                else
                    return BaseSequence.Count - 1;
            }
            set
            {
                end = value;
            }
        }

        public List<ISequenceRange> ParentSeqRanges
        {
            get;
            set;
        }

        public long Start
        {
            get { return start; }
            set
            {
                start = value;
            }
        }



        

        public ISequence BaseSequence
        {
            get {
                if (sequence != null)
                {
                    return sequence;
                }

                return null;
            }
            set {
                sequence = value;
            }
        }

        public Strand Strand
        {
            get {
                return strand;
            }
            set {
                strand = value;
            }
        }

        public virtual XElement ToXml()
        {
            return new XElement("Region",
                new XAttribute("Start", this.Start),
                new XAttribute("End", this.End),
                new XAttribute("Strand", this.Strand)
            ); 
        }



        /// <summary> Loads this object from an xml element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="baseSequence_notUsedInRegion"></param>

        virtual public void Parse(System.Xml.Linq.XElement element, Sequence baseSequence_notUsedInRegion)
        {
            this.Start = element.Int("Start");
            this.End = element.Int("End");
            Strand = Strand.Parse("Strand");
        }


        
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 0;

            SequenceRange sequenceRange = obj as SequenceRange;
            if (obj == null)
                return 0;

            return CompareTo(sequenceRange);
        }
        

        public int CompareTo(ISequenceRange other)
        {
            if (other == null)
            {
                return -1;
            }

            int compare = Start.CompareTo(other.Start);

            if (compare == 0)
                compare = End.CompareTo(other.End);

            if (compare == 0)
                compare = string.Compare(ID, other.ID, StringComparison.OrdinalIgnoreCase);

            if (compare == 0)
            {
                compare = ParentSeqRanges.Count.CompareTo(other.ParentSeqRanges.Count);

                if (compare == 0)
                {
                    for (int index = 0; index < ParentSeqRanges.Count; index++)
                    {
                        compare = ParentSeqRanges[index].CompareTo(other.ParentSeqRanges[index]);
                        if (compare != 0)
                            break;
                    }
                }
            }

            return compare;
        }
    }
}
