using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/***************************************************************************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
namespace QUT.Bio.BioPatML.Sequences.List
{
    /// <summary>
    ///  This class implements a list of {@link Region}s and serves as a base class
    ///  for other lists as {@link SequenceList} and {<see> FeatureList </see>}.
    /// </summary>
    public class RegionList<T> : AnnotatedList<T> where T: Region
    {
        /// <summary>
        ///  Creates an empty region list.
        /// </summary>
        public RegionList() : base(){}

        /// <summary>
        ///  Creates a region list with the given name.
        /// </summary>
        /// <param name="name"></param>
        public RegionList(String name)
            : base (name)
        { /* No implementation required */ }

        /// <summary>
        ///  Calculates the minimum length of all regions in the list.
        /// </summary>
        /// <returns> Returns the minimum length. </returns>
        
		public int MinLength()
        {
            return this.Min( region => region.Length );
        }

        /// <summary>
        ///   Calculates the maximum length of all regions in the list.
        /// </summary>
        /// <returns> Returns the maximum length. </returns>
        
		public int MaxLength()
        {
            return this.Max( region => region.Length );
        }

        /// <summary>
        ///  Calculates the average length of all regions in the list.
        /// </summary>
        /// <returns> Returns the average length. </returns>
        
		public double AverageLength()
        {
			return this.Average( region => region.Length );
        }
    }
}
