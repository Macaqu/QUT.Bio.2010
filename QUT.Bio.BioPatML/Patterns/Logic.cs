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
  <xsd:complexType name="Logic">
    <xsd:group ref="Pattern" minOccurs="1" maxOccurs="unbounded"/>
    <xsd:attribute name="name"      type="xsd:string"               />
    <xsd:attribute name="operation" type="operation"  use="required"/>
    <xsd:attribute name="threshold" type="threshold"  use="required"/>
    <xsd:attribute name="impact"    type="impact"     default="1.0" />
  </xsd:complexType>
*/

namespace QUT.Bio.BioPatML.Patterns {
	/// <summary>
	/// This class describes a logic pattern. The logic pattern performs
	/// boolean operations such as AND, OR or XOR over a set of patterns.
	/// </summary>

	public class Logic : PatternComplex {
		
		/// <summary> The list of available logic operations. </summary>

		public enum OperationType {
			/** <summary>Logical AND.</summary> */
			AND, 
			
			/** <summary>Logical OR.</summary>*/
			OR, 
			
			/** <summary>Logical XOR.</summary> */
			XOR
		};

		private OperationType operation;

		/// <summary>
		/// Logical operation: <para></para>
		/// The Key values are "AND, OR, XOR"
		/// </summary>
		public OperationType Operation {
			get {
				return operation;
			}
			set {
				operation = value;
			}
		}

		/// <summary> Builds an Logic pattern with an "Logic" + unique generated id name.
		/// <para></para>
		/// 
		/// </summary>
		
		public Logic () { }

		/// <summary> A standard logic constructor, this constructor is recommended because
		/// it fills the neccessary attributes up for processing.
		/// </summary>
		/// <param name="name">Name of the pattern.</param>
		/// <param name="operation">Logical operation to perform.</param>
		/// <param name="threshold">Similarity threshold.</param>

		public Logic (
			string name,
			OperationType operation,
			double threshold
		)
			: base( name ) {
			Threshold = threshold;
			this.operation = operation;
		}

		/// <summary> Attach a searching pattern to our logic list
		/// </summary>
		/// <param name="pattern">Your desired pattern</param>
		
		public void Add ( IPattern pattern ) {
			Patterns.Add( pattern );
		}

		/// <summary> Returns a string representation of our logic's name 
		/// plus operation and a list of pattern names attached to logic.
		/// </summary>
		/// <returns></returns>
		
		public override string ToString () {
			StringBuilder sb = new StringBuilder();
			sb.Append( "Logic: " + Name + " operation=" + Operation + "  " );

			for ( int i = 0; i < base.Count; i++ ) {
				sb.Append( "{" + base.Patterns[i].Name + "}" );
			}

			return ( sb.ToString() );
		}

		/// <summary> Implementation of the IMatcher interface. An any pattern matches any sequence.
		/// <see cref="QUT.Bio.BioPatML.Patterns.IMatcher">IMatcher interface</see>.
		/// </summary>
		/// <param name="sequence">Sequence to compare with.</param>
		/// <param name="position">Matching position.</param>
		/// <returns>A match object containning the search result</returns>
		
		public override Match Match ( ISequence sequence, int position ) {
			Match match = LatestMatch;
			match.SubMatches.Clear();

			foreach ( IPattern pattern in Patterns ) {
				Match nextMatch = pattern.Match( sequence, position );

				if ( nextMatch != null ) match.SubMatches.Add( nextMatch );
			}

			int numPattern = Patterns.Count;
			int numMatches = match.SubMatches.Count;

			match.CalcSimilarity();              // mean sim. over all sub-matches

			if ( match.Similarity < Threshold ||
				( operation == OperationType.AND && numMatches != numPattern ) ||
				( operation == OperationType.OR && numMatches == 0 ) ||
				( operation == OperationType.XOR && numMatches > 1 )
			) {
				match.Similarity = 0.0;
				match.SubMatches.Clear();
				return null;
			}

			// fill match object according to sub-matches
			match.CalcStartEnd();
			match.CalcLength();
			match.Strand = Strand.Forward;
			match.BaseSequence = sequence;
			return match;
		}

		/// <summary>
		/// Reads the parameters and populate the attributes for our Logic pattern.
		/// </summary>
		/// <param name="node">Logic Pattern node</param>
		/// <param name="containingDefinition">The container encapsulating this pattern</param>

		public override void Parse (
			XElement node,
			Definition containingDefinition
		) {
			base.Parse( node, containingDefinition );

			try {
				operation = node.EnumValue<OperationType>( "operation" );
			}
			catch {
				throw new ArgumentException( "Logic operation must be 'AND', 'OR' or 'XOR'" );
			}

			Patterns.Parse( node.Elements(), containingDefinition );
		}

		/// <summary> Serialize this object to an xml element.
		/// </summary>
		/// <returns></returns>

		public override XElement ToXml () {
			return ToXml( "Logic",
				Patterns.ToXml(),
				new XAttribute( "operation", operation )
			);
		}
	}
}
