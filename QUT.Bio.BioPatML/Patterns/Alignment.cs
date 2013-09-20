using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using QUT.Bio.BioPatML.Common.XML;
using QUT.Bio.BioPatML.Sequences;
using Bio;

/*****************| Queensland  University Of Technology |*******************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
namespace QUT.Bio.BioPatML.Patterns {

	/// <summary>
	/// This class implements an alignment pattern. An alignment pattern aligns 
	/// the match of another pattern with the start of the following pattern.
	/// The pattern does not consume symbols and it has no length.
	/// </summary>
	
	public sealed class Alignment : Pattern {

		/// <summary> Increment for position 
		/// </summary>

		public new int Increment { internal get; set; }

		/// <summary> Symbolic position: START, END, CENTER 
		/// </summary>

		public AlignmentPosition Position { get; private set; }

		/// <summary> Offset for alignment 
		/// </summary>

		public int Offset { get; private set; }

		/// <summary>
		/// Reference pattern for alignment
		/// </summary>
		public IPattern Pattern { get; private set; }

		/// <summary> Parameterless constructor mainly for serialization.
		/// </summary>

		public Alignment () : base() { }

		/// <summary>
		/// Constructs an alignment.
		/// </summary>
		/// <param name="name"> Name of the alignment. </param>
		/// <param name="pattern"> Pattern the cursor position is relative to. </param>
		/// <param name="position"> Symboloc position e.g. START, END, CENTER. </param>
		/// <param name="offset"> Offset to the specified alignment. Can be positive or negative.</param>

		public Alignment (
			String name,
			IPattern pattern,
			AlignmentPosition position,
			int offset
		)
			: base( name ) {
			Set( pattern, position, offset );
		}

		/// <summary>
		/// Sets the alignment parameters
		/// </summary>
		/// <param name="pattern"> Pattern the cursor position is relative to.  </param>
		/// <param name="position"> Symboloc position e.g. START, END, CENTER. </param>
		/// <param name="offset"> Offset to the specified alignment. Can be positive or negative</param>

		private void Set (
			IPattern pattern,
			AlignmentPosition position,
			int offset
		) {
			if ( pattern == null )
				throw new ArgumentNullException( "No reference pattern specified!" );

			Pattern = pattern;
			Position = position;
			Offset = offset;
		}

		/// <summary> Implementation of the <see cref="QUT.Bio.BioPatML.Patterns.IPattern">pattern interface</see>.
		/// </summary>
		/// <param name="sequence">Sequence to compare with.</param>
		/// <param name="position">Matching position.</param>
		/// <returns>The result.</returns>

		public override Match Match (
			ISequence sequence,
			int position
		) {
			int absAlignPos = AlignPosition();
			int absPos = position - 1;

			Increment = absAlignPos - absPos;

			LatestMatch.Set ( 
				sequence, 
				position + Increment, 
				0, 
				Strand.Forward, 
				1.0 
			);

			return LatestMatch;
		}

		/// <summary> Returns a string representation.
		/// </summary>
		/// <returns></returns>

		public override String ToString () {
			return ( "Alignment: " + Name +
				" Pattern=" + Pattern.Name +
				" Position=" + Position +
				" Offset=" + Offset );
		}

		/// <summary> Calculates the absolute position of the alignment.
		/// </summary>
		/// <returns>The alignment position value</returns>

		public int AlignPosition () {
			Match match = Pattern.LatestMatch;

			int result = Offset /*+ match.Position()*/;

			if ( Position == AlignmentPosition.END ) {
				result += (int)match.Count;
			}
			else if ( Position == AlignmentPosition.CENTER ) {
				result += (int)match.Count / 2;
			}

			return result;
		}

		/// <summary> Reads the parameters for a pattern at the given node.
		/// </summary>
		/// <param name="node">The Alignment pattern node</param>
		/// <param name="definition">Definition encapsulating the pattern</param>

		public override void Parse (
			XElement node,
			Definition definition
		) {
			Name = node.String( "name" );
			Impact = node.Double( "impact", 1.0 );

			AlignmentPosition position;

			try {
				position = node.EnumValue<AlignmentPosition>( "position" );
			}
			catch {
				throw new ArgumentException( "Alignment position should be START, END or CENTER" );
			}

			Set( definition.Pattern.Child( node.String( "pattern" ) ),
				   position,
				   node.Int( "offset" )
			);
		}

		/// <summary> Saves the contents of this object in an xml element.
		/// </summary>
		/// <returns>An xml element containign the content of this object.</returns>

		public override XElement ToXml () {
			return new XElement( "Alignment",
				new XAttribute( "name", Name ),
				new XAttribute( "impact", Impact ),
				new XAttribute( "position", Position ),
				new XAttribute( "offset", Offset ),
				new XAttribute( "pattern", Pattern.Name )
			);
		}
	}
}
