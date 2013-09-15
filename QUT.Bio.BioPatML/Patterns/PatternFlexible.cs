using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	///  Abstract class describing patterns of flexible length. 
	/// </summary>
	public abstract class PatternFlexible : Pattern {

		/// <summary> Minimum length </summary>
		public int minLength;

		/// <summary> Maximum length </summary>
		public int maxLength;

		/// <summary> Length increment </summary>
		public double incLength = 1;

		/// <summary> Current length </summary>
		private double length ;

		/// <summary> increment for search position </summary>
		private int increment;

		/// <summary> Parameterless constructor for deserialsation. </summary>
		
		public PatternFlexible () {
		}

		/// <summary> Initialise pattern, setting the name. </summary>

		public PatternFlexible ( string name )
			: base( name ) {
		}

		/// <summary> Sets the minimum and maximum length and length increment of the pattern.
		/// </summary>
		/// <param name="minLength">Minimum length of the pattern.</param>
		/// <param name="maxLength">Maximum length of the pattern.</param>
		/// <param name="incLength">Length increment for the pattern.</param>
		
		public void Set ( 
			int minLength, 
			int maxLength, 
			double incLength 
		) {
			if ( minLength < 0 )
				throw new ArgumentOutOfRangeException( "Minimum length cannot be negative!" );

			if ( maxLength < minLength )
				throw new ArgumentOutOfRangeException( "Maximum length smaller than minimum length!" );

			if ( incLength <= 0 )
				throw new ArgumentOutOfRangeException( "Length increment must be greater than zero!" );

			this.minLength = minLength;
			this.maxLength = maxLength;
			this.incLength = incLength;
			this.length = minLength;
		}

		/// <summary> Gets the flexible length of the pattern. This is difference between
		/// the maximum and the minimum length of the pattern. </summary>
		
		public int Flexiblity {
			get { return maxLength - minLength; }
		}

		/// <summary> Gets the length increment. </summary>
		
		public override int Increment {
			get {
				return increment;
			}
		}

		/// <summary> Increments the length.
		/// </summary>
		/// <returns>the old length rounded to an integer.</returns>
		
		protected int NextLength () {
			// TODO: the doc comment is innaccurate and does not describe the inscrutable side effects of this method.

			int oldLength = (int) ( Length + 0.5 );
			length += IncLength;

			if ( oldLength >= MaxLength ) {
				increment = 1;
				length = MinLength;
			}
			else
				increment = 0;

			return Math.Min( oldLength, MaxLength );
		}

		/// <summary> Reads the parameters and populate the attributes for this pattern. </summary>
		/// <param name="node">Any Pattern node that extends pattern flexible </param>
		/// <param name="definition">The container encapsulating this pattern</param>
		
		public override void Parse (
			XElement node,
			Definition definition
		) {
			Name = node.String( "name" );

			Set( node.Int( "minimum" ),
					node.Int( "maximum" ),
					node.Double( "increment", 1.0 ) );
		}

		/// <summary> Inserts the attributes found in every PatternFlexible into the 
		/// supplied node. </summary>
		/// <param name="element"></param>
		
		protected void SetXmlAttributes( XElement element ) {
			element.Add( 
				new XAttribute( "name", Name ),
				new XAttribute( "minimum", MinLength ),
				new XAttribute( "maximum", MaxLength ),
				new XAttribute( "increment", IncLength )
			);
		}

		/// <summary> gets the MinLength of this object. </summary>
		public int MinLength { get { return minLength; } }

		/// <summary> gets the MaxLength of this object. </summary>
		public int MaxLength { get { return maxLength; } }

		/// <summary> gets the IncLength of this object. </summary>
		public double IncLength { get { return incLength; } }

		/// <summary> gets the Length of this object. </summary>
		public double Length { get { return length; } }
	}

}
