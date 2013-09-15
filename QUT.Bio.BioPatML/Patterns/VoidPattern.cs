using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using QUT.Bio.BioPatML.Common.XML;
using System.Xml.Linq;
using Bio;

/*****************| Queensland University Of Technology |********************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
/* <xsd:complexType name="Void">
    <xsd:attribute name="name"      type="xsd:string"                />
    <xsd:attribute name="impact"    type="impact"       default="0.0"/>
  </xsd:complexType>
*/

namespace QUT.Bio.BioPatML.Patterns {
	/// <summary>
	/// This class defines a void pattern. This pattern always matches but
	/// the match is of length zero.
	/// </summary>
	public class VoidPattern : Pattern {
		#region -- Constructor --
		/// <summary>
		/// Constructor - used when your void element has no unique name
		/// </summary>
		public VoidPattern () {}

		/// <summary>
		/// Same as the above constructor, but with a given name
		/// </summary>
		/// <param name="name">Name of void element</param>
		public VoidPattern ( string name ) : base( name ) { }

		#endregion

		#region -- IMatcher Implementation --
		/// <summary>
		/// See interface <see cref="QUT.Bio.BioPatML.Patterns.IMatcher">IMatcher</see>
		/// </summary>
		/// <param name="sequence">the sequence used for matching</param>
		/// <param name="position">position of match</param>
		/// <returns></returns>
		public override Match Match ( ISequence sequence, int position ) {
			LatestMatch.Set( sequence, position, 0, 1, 1.0 );
			return LatestMatch;
		}

		#endregion

		/// <summary> Reads the parameters and populate the attributes for this pattern.
		/// </summary>
		/// <param name="containingDefinition">Definition wrapping this node element</param>
		/// <param name="element">The node with name Void</param>

		public override void Parse ( 
			XElement element, 
			Definition containingDefinition 
		) {
			base.Parse( element , containingDefinition );
		}

		/// <summary> Gets an XML representation of this Void element.
		/// </summary>
		/// <returns></returns>

		public override XElement ToXml () {
			return ToXml( "Void" );
		}

		#region -- ToString() --
		/// <summary>
		/// returns a string representation of void element
		/// </summary>
		/// <returns></returns>
		public override string ToString () {
			return "Void: " + Name;
		}

		#endregion
	}
}
