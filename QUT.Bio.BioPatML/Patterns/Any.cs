using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using QUT.Bio.BioPatML.Sequences;
using Bio;
using QUT.Bio.BioPatML.Common.XML;

/*****************| Queensland  University Of Technology |*******************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
namespace QUT.Bio.BioPatML.Patterns {
	/// <summary>
	/// This class defines an any pattern. This pattern matches any sequence
	/// within a specified length interval. 
	/// </summary>
	public class Any : PatternFlexible  {

		/// <summary>
		/// Common constructor for creating a plain Any pattern object 
		/// </summary>
		
		public Any () { }

		/// <summary>
		/// Constructs an any pattern of variable length.
		/// </summary>
		/// <param name="name">Name of the pattern.</param>
		/// <param name="minLength">Minimum length of the sequence to match. </param>
		/// <param name="maxLength">Maximum length of the sequence to match.</param>
		/// <param name="incLength">Length increment for the pattern.</param>
		
		public Any (
			string name,
			int minLength,
			int maxLength,
			double incLength
		)
			: base( name ) 
		{
			Set( minLength, maxLength, incLength );
		}

		/// <summary>
		/// Implementation of the IMatcher interface. An any pattern matches any sequence.
		/// <see cref="QUT.Bio.BioPatML.Patterns.IMatcher">IMatcher interface</see>.
		/// </summary>
		/// <param name="sequence">Sequence to compare with.</param>
		/// <param name="position">Matching position.</param>
		/// <returns>A match object containning the search result</returns>
		
		public override Match Match (
			ISequence sequence, 
			int position 
		) {
			return new Match( sequence, position, NextLength(), Strand.Forward, 1.0 );
		}

		/// <summary>
		/// Reads the parameters and populate the attributes for this pattern.
		/// </summary>
		/// <param name="node">Any Pattern node</param>
		/// <param name="definition">The container encapsulating this pattern</param>

		public override void Parse (
			XElement node,
			Definition definition
		) {
			base.Parse( node, definition );
			Impact = node.Double( "impact", 1.0 );
		}

		/// <summary> Saves the contents of this object in an xml element.
		/// </summary>
		/// <returns>An xml element containign the content of this object.</returns>

		public override XElement ToXml () {
			XElement result = new XElement( "Any" );
			SetXmlAttributes( result );
			result.Add( new XAttribute( "impact", Impact ) );
			return result;
		}
	}
}
