using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
//using QUT.Bio.BioPatML.Sequences;
using QUT.Bio.BioPatML.Common.XML;

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
    /// This interface defines a pattern. Every object which implements the 
    /// pattern interface can be searched within a {Sequence}.
    /// </summary>
    public interface IPattern : IMatcher, IXmlConvertable
    {
        /// <summary>
        /// Gets/Sets the pattern name.
        /// </summary> 
        string Name { get; set;  }

        /// <summary>
        /// Sets/Gets the similarity threshold. Only matches will be returned with a 
        /// similarity equal or higher than the given similarity threshold.
        /// It is recommended that the constructor of a pattern requires the 
        /// threshold parameter as well to make sure that a proper threshold is
        /// set for new pattern.
        /// 
        /// </summary>
        Double Threshold { get; set; }

        /// <summary>
        /// Sets/ Gets the impact of a pattern. This a weight is taken into account
        /// when the overall similarity of a structured pattern, consisting of
        /// other patterns, is calculated.
        /// *Note given param value Impact weight. Must be between zero and one.
        /// </summary>
        Double Impact { get; set; }

		/// <summary> (For structured patterns) Gets a reference to the direct child pattern having the specified name. </summary>
		/// <param name="name">The name of a child pattern to return.</param>
		/// <returns>The referenced child pattern. </returns>
		/// <exception cref="QUT.Bio.BioPatML.Patterns.ChildNotFoundException">Throws a ChildNotFoundException if the named child is absent.</exception>
		/// <exception cref="QUT.Bio.BioPatML.Patterns.NonStructuredPatternException">Throws a NonStructuredPatternException if the pattern does not support child patterns.</exception>

		IPattern Child( string name );

		/// <summary> Gets an enumerator that ranges over the pattern and its children, with recursion into child patterns. </summary>

		IEnumerable<IPattern> SelfAndChildren { get; }

		/// <summary> Maintains the current match </summary>

		Match LatestMatch { get; }
		// TODO: I'd like to get rid of this, we should be able to rewrite match to be recursive and safe, which this is not.

    }
}
