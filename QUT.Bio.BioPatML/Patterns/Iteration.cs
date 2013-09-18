using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using QUT.Bio.BioPatML.Common.XML;
using QUT.Bio.BioPatML.Sequences;
using Bio;

/*****************| Queensland University Of Technology |********************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/

/*
  <xsd:complexType name="Iteration">
    <xsd:group ref="Pattern" minOccurs="1" maxOccurs="10"/>
    <xsd:attribute name="name"       type="xsd:string"                />
    <xsd:attribute name="minimum"    type="length"      use="required"/>
    <xsd:attribute name="maximum"    type="length"      use="required"/>
    <xsd:attribute name="threshold"  type="threshold"   use="required"/>
    <xsd:attribute name="impact"     type="impact"      default="1.0" />
  </xsd:complexType>
*/

namespace QUT.Bio.BioPatML.Patterns {
	/// <summary>
	///  This class defines an iteration pattern. This pattern iterates over its
	///  sub-pattern.
	/// </summary>
	public sealed class Iteration : Pattern {
		/// <summary> The pattern that is iterated
		/// </summary>
		public IPattern Pattern { get; private set; }

		/// <summary> Minimum number of iterations required 
		/// </summary>
		public int Minimum { get; private set; }

		/// <summary> Maximum number of iterations required 
		/// </summary>
		public int Maximum { get; private set; }

		/// <summary> The usual no param constructor, 
		/// builds an Iteration pattern with an "Iteration" + unique generated id name.
		/// </summary>
		public Iteration () { }

		/// <summary> Constructs an iteration pattern.
		/// </summary>
		/// <param name="name">Name of the pattern.</param>
		/// <param name="pattern">The pattern to iterate over.</param>
		/// <param name="minimum">Minimum number of iterations.</param>
		/// <param name="maximum">Maximum number of iterations.</param>
		/// <param name="threshold">Similarity threshold</param>

		public Iteration (
			String name,
			IPattern pattern,
			int minimum,
			int maximum,
			double threshold
		)
			: base( name ) {
			Threshold = threshold;
			Set( pattern, minimum, maximum );
		}

		/// <summary> Setter for iteration parameters.
		/// </summary>
		/// <param name="pattern">The pattern to iterate over.</param>
		/// <param name="minimum">Minimum number of iterations. </param>
		/// <param name="maximum">Maximum number of iterations.</param>

		private void Set ( IPattern pattern, int minimum, int maximum ) {
			if ( maximum < minimum )
				throw new ArgumentException
					( "Maximum smaller than minimum in iteration pattern!" );

			if ( minimum < 1 )
				throw new ArgumentException
					( "Minimum smaller than one in iteration pattern!" );

			Pattern = pattern;
			Minimum = minimum;
			Maximum = maximum;
		}

		#region -- IMatcher Members --
		/// <summary>
		/// Implementation of the IMatcher interface. An any pattern matches any sequence.
		/// <see cref="QUT.Bio.BioPatML.Patterns.IMatcher">IMatcher interface</see>.
		/// </summary>
		/// <param name="sequence"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		public override Match Match ( ISequence sequence, int position ) {
			Match match = LatestMatch;
			int counter = 0;

			match.SubMatches.Clear();

			while ( counter < Maximum ) {
				Match bestMatch = null;

				do {  // find best match at current position/iteration
					Match nextMatch = Pattern.Match( sequence, position );

					if ( nextMatch != null && ( bestMatch == null ||
					   nextMatch.Similarity >= bestMatch.Similarity ) )
						bestMatch = nextMatch.Clone();
				} 
				while ( Pattern.Increment == 0 );

				if ( bestMatch == null ) break;

				match.SubMatches.Add( bestMatch );
				position += (int)bestMatch.Count;
				counter++;
			}

			match.CalcSimilarity();

			if ( match.Similarity < Threshold || counter < Minimum ) {
				match.Similarity = 0.0;
				match.SubMatches.Clear();
				return null;
			}

			// fill match object according to sub-matches
			match.CalcStartEnd();
			match.CalcLength();
			match.Strand = Strand.Forward;
			match.BaseSequence = sequence; //TO CHANGE
			return match;
		}

		#endregion

		/// <summary> Reads the parameters and populate the attributes for this pattern.
		/// </summary>
		/// <param name="element">Any Pattern node</param>
		/// <param name="definition">The container encapsulating this pattern</param>

		public override void Parse (
			XElement element,
			Definition definition
		) {
			Name = element.String( "name" );
			Impact = element.Double( "impact", 1.0 );
			Threshold = element.Double( "threshold", 1.0 );

			XElement child = element.Elements().FirstOrDefault();
			IPattern pattern = QUT.Bio.BioPatML.Patterns.Pattern.CreateFrom( child );
			pattern.Parse( child, definition );

			Set( pattern, element.Int( "minimum" ), element.Int( "maximum" ) );
		}

		/// <summary> Saves the contents of this object in an xml element.
		/// </summary>
		/// <returns>An xml element containign the content of this object.</returns>

		public override XElement ToXml () {
			return new XElement( "Iteration",
				new XAttribute( "name", Name ),
				new XAttribute( "impact", Impact ),
				new XAttribute( "threshold", Threshold ),
				new XAttribute( "minimum", Minimum ),
				new XAttribute( "maximum", Maximum ),
				Pattern.ToXml()
			);
		}
	}
}
