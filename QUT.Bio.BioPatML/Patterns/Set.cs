using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using QUT.Bio.BioPatML.Common.XML;

/*****************| Queensland University Of Technology |********************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
#if false
  <xsd:complexType name="Set">
    <xsd:group ref="Pattern" minOccurs="1" maxOccurs="unbounded"/>
    <xsd:attribute name="name"      type="xsd:string"               />
    <xsd:attribute name="mode"      type="mode"       use="required"/>
    <xsd:attribute name="threshold" type="threshold"  use="required"/>
    <xsd:attribute name="impact"    type="impact"     default="1.0" />
  </xsd:complexType>
#endif

namespace QUT.Bio.BioPatML.Patterns {

	/// <summary> This class describes a set of patterns which can be matched 
	/// against a sequence.
	/// </summary>

	public abstract class Set : PatternComplex {

		/// <summary> Position increment after a match.
		/// </summary>

		protected int increment;

		/// <summary> Parameterless constructor for deserialisation purposes.
		/// </summary>

		public Set () { }

		/// <summary> Constructor that takes in the name of Set element
		/// </summary>
		/// <param name="name">Name of element</param>

		public Set ( string name ) : base( name ) { }

		/// <summary> Return the minimum increment over all patterns within the set
		/// 
		/// </summary>
		/// <returns></returns>

		public override int Increment {
			get {
				return this.increment;
			}
		}

		/// <summary> Adds a pattern to the pattern set.
		/// </summary>
		/// <param name="pattern">Pattern to add.</param>

		public void Add ( IPattern pattern ) {
			base.Patterns.Add( pattern );
		}

		/// <summary> Returns a string representation of the pattern set.
		/// </summary>
		/// <returns></returns>

		public override string ToString () {
			StringBuilder sb = new StringBuilder();
			sb.Append( "Set: '" + Name + "'=" );

			for ( int i = 0; i < base.Count; i++ ) {
				sb.Append( "{" + this[i].ToString() + "}" );
			}

			return ( sb.ToString() );
		}

		/// <summary> Gets an XMl represetnation of the present Set object.
		/// </summary>
		/// <returns></returns>

		public override XElement ToXml () {
			return ToXml( "Set",
				Patterns.Select( p => p.ToXml() ),
				new XAttribute( "mode", this is SetAll ? "ALL" : "BEST" )
			);
		}


		/// <summary> Populates the presentSet object from an XML element.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="containingDefinition"></param>
		
		public override void Parse (
			XElement element,
			Definition containingDefinition
		) {
			Name = element.String( "name" );
			Threshold = element.Double( "threshold", 1.0 );
			Impact = element.Double( "impact", 1.0 );
			base.Parse( element.Elements(), containingDefinition );
		}

	}
}
