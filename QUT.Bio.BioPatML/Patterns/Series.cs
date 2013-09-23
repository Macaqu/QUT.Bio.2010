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
namespace QUT.Bio.BioPatML.Patterns {
	/// <summary>
	/// This class is the base class for a pattern series. A pattern series is a 
	/// sequence of patterns (gaps are also patterns) which matches if all patterns
	/// of the series match in the given order. There are two derived classes
	/// which return ALL or the BEST match of the series.
	/// </summary>
	public abstract class Series : PatternComplex {
		
		/// <summary> Parameterless constructor from deserialisation.
		/// </summary>
		
		public Series () {}

		/// <summary> Initialise this pattern, setting its name.
		/// </summary>
		/// <param name="name">Name of the pattern.</param>
		
		public Series ( string name )
			: base( name ) { }

		/// <summary> Adds a pattern to the pattern series.
		/// </summary>
		/// <param name="patterns">A list of patterns to add.</param>
		/// <returns>Returns the added pattern.</returns>
		
		public void Add ( params IPattern [] patterns ) {
			foreach ( var pattern in patterns ) {
				Patterns.Add( pattern );
				LatestMatch.SubMatches.Add( pattern.LatestMatch );
			}
		}

		/// <summary>
		/// Returns a string representation of the series pattern.
		/// </summary>
		/// <returns></returns>
		
		public override string ToString () {
			StringBuilder sb = new StringBuilder( "Series: { " );

			for ( int i = 0; i < Patterns.Count; i++ )
				sb.Append( Patterns[i].ToString() ).Append( ' ' );
			
			sb.Append( ")" );
			return sb.ToString();
		}

		/// <summary> Reads the parameters and populate the attributes for this pattern.
		/// </summary>
		/// <param name="element">Series Pattern node</param>
		/// <param name="containingDefinition">The container encapsulating this pattern</param>

		public override void Parse ( 
			XElement element, 
			Definition containingDefinition 
		) {
			base.Parse( element, containingDefinition );

			foreach ( XElement child in element.Elements() ) {
				IPattern p = Pattern.CreateFrom( child );

				if ( p != null ) {
					p.Parse( child, containingDefinition );
					Add( p );
				}
			}
		}

		/// <summary> Copies the contents of this series to an xml element. </summary>
		/// <returns></returns>

		public override XElement ToXml () {
			return ToXml( "Series",
				Patterns.Select( p => p.ToXml() ),
				new XAttribute( "mode", this is SeriesAll ? "ALL" : "BEST" )
			);
		}
	}
}
