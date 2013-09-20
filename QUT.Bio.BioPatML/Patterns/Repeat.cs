using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using QUT.Bio.BioPatML.Sequences;
using QUT.Bio.BioPatML.Symbols;
using QUT.Bio.BioPatML.Common.XML;
using Bio;

/*****************| Queensland University Of Technology |********************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
namespace QUT.Bio.BioPatML.Patterns {
	/// <summary>
	///  This class implements the repeat pattern. A repeat pattern consists of a
	///  reference pattern and a repeat element pattern which is the repeat of
	///  the reference pattern. More complex repeats can be built by using profiles
	///  and repeat element patterns directly. See {RepeatElement} for an example.
	/// </summary>
	public sealed partial class Repeat : Pattern {
		
		/// <summary> Defines the valid kinds of repeat. 
		/// </summary>

		public enum RepeatType {
			/** Direct repeat. */
			DIRECT,

			/** Inverted repeat */
			INVERTED
		};

		private RepeatType repeatMode;

		private IPattern referencedPattern;

		/// <summary> Matcher used to match a repeat 
		/// </summary>
		private IMatcher matcher;

		/// <summary> Weight matrix for symbol pairing
		/// </summary>
		private double[,] weights = new double[21, 21];

		/// <summary> The profile which contains the reference pattern and the repeat element 
		/// </summary>
		public IPattern ReferencedPattern {
			get {
				return referencedPattern;
			}
			private set {
				if ( value == null ) {
					throw new ArgumentException( "Reference pattern must be specified!" );
				}

				referencedPattern = value;
			}
		}

		/// <summary> Get or set the  
		/// </summary>
		public RepeatType RepeatMode {
			get {
				return repeatMode;
			}
			private set {
				repeatMode = value;
				matcher = repeatMode == RepeatType.DIRECT
					? (IMatcher) new MatcherDirect( this )
					: (IMatcher) new MatcherInverted( this );
			}
		}

		/// <summary> Parameterless constructor for deserialization.
		/// </summary>

		public Repeat () { }

		/// <summary>
		///  Constructs a repeat pattern.
		/// </summary>
		/// <param name="name">Name of the repeat.</param>
		/// <param name="pattern">The Pattern to repeat.</param>
		/// <param name="mode">DIRECT, INVERTED</param>
		/// <param name="threshold">Similarity threshold</param>
		public Repeat (
			string name,
			IPattern pattern,
			RepeatType mode,
			double threshold
		)
			: base( name ) {
			Threshold = threshold;
			RepeatMode = mode;
			ReferencedPattern = pattern;
		}


		/// <summary> Sets the pairing weight
		/// </summary>
		/// <param name="ch1">First character  (One letter code)</param>
		/// <param name="ch2">Second character  (One letter code)</param>
		/// <param name="weight">Returns the weight for the given pairing of symbols.</param>
		/// <exception cref="System.ArgumentException">Thrown when weight is less than 0 or more than 1.0</exception>

		public void Weight ( char ch1, char ch2, double weight ) {
			if ( weight < 0.0 || weight > 1.0 )
				throw new ArgumentException
					( "Invalid pairing weight " + weight );

			weights[Index( ch1 ), Index( ch2 )] = weight;
		}

		/// <summary> Gets a pairing weight
		/// </summary>
		/// <param name="ch1">First character (One letter code)</param>
		/// <param name="ch2">Second character (One letter code)</param>
		/// <returns>Weight for the given pairing of symbols. Has to be
		/// in interval [0,1]</returns>

		public double Weight ( char ch1, char ch2 ) {
			return weights[Index( ch1 ), Index( ch2 )];
		}

		/// <summary> Calculates the index of character within the weight matrix.
		/// </summary>
		/// <param name="ch">One letter code of a symbol.</param>
		/// <returns>Returns the array index.</returns>

		private int Index ( char ch ) {
			return char.ToUpper( ch ) - 'A';
		}

		/// <summary> Returns a string representation of repeat pattern
		/// </summary>
		/// <returns></returns>

		public override String ToString () {
			return ( "Repeat: " + Name + " pattern=" + ReferencedPattern.Name );
		}

		#region IMatcher implementation

		/// <summary>
		/// The implementation ensures that
		/// a match fails for a given position if there is no match. Otherwise the
		/// matcher might return a match at a different position.
		/// <see cref="QUT.Bio.BioPatML.Patterns.IPattern">IPattern Match(Sequence, int) method</see>
		/// </summary>
		/// <param name="sequence"> the sequence for matching</param>
		/// <param name="position"> position used for matching</param>
		/// <returns>The matched</returns>
		public override Match Match ( ISequence sequence, int position ) {
			return matcher.Match( sequence, position );
		}

		#endregion

		/*
		  <xsd:complexType name="Repeat">
			<xsd:sequence>
			  <xsd:element name="Pairing"  type="RepeatPairing" minOccurs="0" maxOccurs="400"/>
			</xsd:sequence>
			<xsd:attribute name="name"      type="xsd:string"                />
			<xsd:attribute name="pattern"   type="xsd:string"  use="required"/>
			<xsd:attribute name="mode"      type="repeatmode"  use="required"/>
			<xsd:attribute name="threshold" type="threshold"   use="required"/>
			<xsd:attribute name="impact"    type="impact"      default="1.0"/>
		  </xsd:complexType>

		  <xsd:complexType name="RepeatPairing">
			<xsd:attribute name="original"  type="letter"      use="required"/>
			<xsd:attribute name="repeat"    type="letter"      use="required"/>
			<xsd:attribute name="weight"    type="weight"      default="1.0"/>
		  </xsd:complexType>
		*/

		/// <summary> Loads this repeat object from an xml element.
		/// </summary>

		public override void Parse ( XElement element, Definition definition ) {
			base.Parse( element, definition );

			RepeatMode = element.EnumValue<RepeatType>( "mode" );
			ReferencedPattern = definition.Pattern.Child(element.String( "pattern" ));

			foreach ( XElement childElement in element.Elements() ) {
				if ( childElement.Name.ToString().Equals( "Pairing" ) ) {
					Weight(
						childElement.String( "original" )[0],
						childElement.String( "repeat" )[0],
						childElement.Double( "weight", 1.0 )
					);
				}
				else {
					throw new ArgumentException( string.Format(
						"Unexpected content '{0}' in Repeat structure",
						childElement.Name
					) );
				}
			}
		}

		/// <summary> Get an XML representation of this Repeat element.
		/// </summary>
		/// <returns></returns>

		public override XElement ToXml () {
			XElement result = ToXml( "Repeat",
				new XAttribute( "pattern", referencedPattern.Name ),
				new XAttribute( "mode", repeatMode )
			);

			for ( int i = 0; i < weights.GetLength( 0 ); i++ ) {
				for ( int j = 0; j < weights.GetLength( 1 ); j++ ) {
					if ( weights[i, j] > 0 ) {
						result.Add( new XElement( "Pairing",
							new XAttribute( "original", (char)('A' + i) ),
							new XAttribute( "repeat", (char)('A' + j) ),
							new XAttribute( "weight", weights[i, j] )
						) );
					}
				}
			}

			return result;
		}
	}
}




