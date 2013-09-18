using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using QUT.Bio.BioPatML.Common.XML;
using QUT.Bio.BioPatML.Sequences;
using Bio;

/***************************************************************************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan   
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
namespace QUT.Bio.BioPatML.Patterns
{
	/// <summary>
	/// This class implements a Constraint pattern. A Constraint pattern 
	/// matches only at specific sequence positions.
	/// </summary>
	public class Constraint : Pattern
	{
		#region -- Automatic Properties --
		/// <summary>
		/// Symbolic position the contraint, e.g. START, END, CENTER 
		/// </summary>
		public string Position { get; private set; }

		/// <summary>
		/// Offset for alignment/constraint 
		/// </summary>
		public int Offset { get; private set; }

		/// <summary>
		/// Position Increment
		/// </summary>
		public new int Increment { get; private set; }

		#endregion

		#region -- Constructors --

		/// <summary>
		/// Default constructor that takes in no param.
		/// A unique name is automatic generated for this class.
		/// The unique name usually starts with "Constraint" + Id
		/// </summary>
		public Constraint () { }

		/// <summary>
		/// Constructs a constraint.
		/// </summary>
		/// <param name="name">Name of the constraint.</param>
		/// <param name="position">Symbolic position e.g. START, END, CENTER. </param>
		/// <param name="offset">Offset to the specified alignment. Can be positive or negative</param>
		public Constraint
			( string name, string position, int offset )
			: base( name )
		{
			Position = position;
			Offset = offset;
		}

		#endregion -- Constructors --

		#region -- Public Methods --
		/// <summary>
		/// Calculates the absolute position of the constrain.
		/// </summary>
		/// <param name="sequence">Sequence the constraint is applied to.</param>
		/// <returns>Returns the absolute constrain position.</returns>
		public int CalcConstraintPos ( ISequence sequence )
		{
			ISequence seq = sequence;

            //TODO:: need to worry about position's sequence
            /*
			if ( Position.Equals( "START" ) )
				return Offset + seq.Position();

			if ( Position.Equals( "END" ) )
				return Offset + seq.Position() + (int)seq.Count;

			if ( Position.Equals( "CENTER" ) )
				return Offset + seq.Position() + (int)seq.Count / 2;
            */
			throw new ArgumentException
				( "Invalid alignment type: " + Position );
		}

		/// <summary>
		/// A string representation of the constraint
		/// </summary>
		/// <returns></returns>
		public override string ToString ()
		{
			return ( "Constraint: " + Name +
					   " Position=" + Position +
					   " Offset=" + Offset );
		}

		#endregion

		#region -- IMatcher Members --
		/// <summary>
		/// Implementation of the IMatcher interface. An any pattern matches any sequence.
		/// <see cref="QUT.Bio.BioPatML.Patterns.IMatcher">IMatcher interface</see>.
		/// </summary>
		/// <param name="sequence">Sequence to compare with.</param>
		/// <param name="position">Matching position.</param>
		/// <returns>A match object containning the search result</returns>
		public override Match Match ( ISequence sequence, int position )
		{
			int dist = CalcConstraintPos( sequence ) - position;

			Increment = dist < 1 ? 1 : dist;

			if ( dist == 0 )
			{
                return new Match(sequence, position, 0, Strand.Forward, 1.0);
                //LatestMatch.Set( sequence, position, 0, Strand.Forward, 1.0 );
				//return ( LatestMatch );
			}

			return ( null );
		}

		#endregion

		/// <summary> Reads the parameters and populate the attributes for this pattern.
		/// </summary>
		/// <param name="element">Constraint Pattern node</param>
		/// <param name="containingDefinition">The container encapsulating this pattern</param>
		
		public override void Parse ( 
			XElement element,
			Definition containingDefinition 
		) {
			Name = element.String( "name" );
			Impact = element.Double( "impact", 1.0 );
			Position = element.String( "position" );
			Offset = element.Int( "offset" );
		}

		/// <summary> Saves the contents of this object in an xml element.
		/// </summary>
		/// <returns>An xml element containign the content of this object.</returns>

		public override XElement ToXml() {
			return new XElement( "Constraint",
				new XAttribute( "name", Name ),
				new XAttribute( "impact", Impact ),
				new XAttribute( "position", Position ),
				new XAttribute( "offset", Offset )
			);
		}
	}
}
