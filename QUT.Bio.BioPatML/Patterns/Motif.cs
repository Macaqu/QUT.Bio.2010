using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using QUT.Bio.BioPatML.Symbols;
using QUT.Bio.BioPatML.Common.XML;
using QUT.Bio.BioPatML.Sequences;
using Bio;
using QUT.Bio.BioPatML.Alphabet;

/*****************| Queensland University Of Technology |********************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
namespace QUT.Bio.BioPatML.Patterns {
	/// <summary>
	/// This class implements a pattern described by a motif sequence. Alternative
	/// symbols at a position are allowed and implemented as MetaSymbols.
	/// </summary>
	public class Motif : Pattern {

		/// <summary> The alphabet used for this motif.
		/// </summary>
		private IAlphabet alphabet;

		/// <summary> List of symbols in this motif element
		/// </summary>
        private List<char> motifSymbols = new List<char>();

		/// <summary> Internal constructor for deserialization.
		/// </summary>

		public Motif () { }

		/// <summary> Creates a new motif pattern. The motif is provided as a letter string
		/// </summary>
		/// <param name="name">Name of the pattern.</param>
		/// <param name="alphabetType">Name of the alphabet of the motif.</param>
		/// <param name="motif">Motif description.</param>
		/// <param name="threshold">The minimum similarity threshold required for a match.</param>

		public Motif (
			String name,
			IAlphabet alphabetType,
			String motif,
			double threshold
		)
			: base( name ) {
			Threshold = threshold;
			alphabet = alphabetType/*AlphabetFactory.Instance( alphabetType )*/;
			ParseSymbols( motif );
		}

		/// <summary> Creates a new anonymous motif with default threshold 1.0.
		/// </summary>
		/// <param name="alphabetType">The Alphabet from which the symbols in the motif are drawn.</param>
		/// <param name="motif">The motif.</param>
		/// <param name="threshold">The minimum similarity threshold required for a match.</param>

		public Motif (
			IAlphabet alphabetType,
			char motif,
			double threshold = 1.0
		) 
			: this ( null, alphabetType, motif.ToString(), threshold )
		{
		}

		/// <summary> Gets the letters of the motif pattern.
		/// </summary>
		
		public String Letters {
			get {
				StringBuilder sb = new StringBuilder();

				foreach ( char sym in motifSymbols ) {
                    if (alphabet.GetValidSymbols().Contains((byte)sym))
                    { 
						sb.Append( "[" + sym + "]" );
					}
					else {
						sb.Append( sym);
					}
				}
               return sb.ToString();
			}
		}

        /*
        private bool IsValidSymbol(String symbol) {
            byte[] symbolByte = GetSymbolsByte(symbol);
            for (int i = 0; i < symbolByte.Length; i++)
            {
                if (!alphabet.GetValidSymbols().Contains(symbolByte[i])) {
                    return false;
                }
                
            }
            return true;
        }
*/
        private byte[] GetSymbolsByte(String symbols) {
            return Encoding.Unicode.GetBytes(symbols); ;
        }


		/// <summary> Implementation of the IMatcher interface. An any pattern matches any sequence.
		/// <see cref="QUT.Bio.BioPatML.Patterns.IMatcher">IMatcher interface</see>.
		/// </summary>
		/// <param name="sequence">Sequence to compare with.</param>
		/// <param name="position">Matching position.</param>
		/// <returns>A match object containning the search result</returns>

		public override Match Match (
			ISequence sequence,
			int position
		) {
			int length = motifSymbols.Count;
			int mismatches = 0;
			int maxMismatches = (int) ( length * ( 1 - Threshold ) );

			for ( int i = 0; i < length; i++ ) {
				if ( !motifSymbols[i].Equals( /*sequence.GetSymbol( position + i )*/ sequence[(long)(position + i)] ) ) {
					if ( ++mismatches > maxMismatches ) {
						return ( null );
					}
				}
			}
            return new Match(this, sequence, 1, (int)sequence.Count, Strand.Forward, 1);
		}

		/// <summary> Parses the motif description and generates a symbol array that describes
		/// the motif. Alternatives are described by MetaSymbols.
		/// </summary>
		/// <param name="motif">Motif description.</param>
		/// <returns>Returns a symbol array.</returns>

		public void ParseSymbols (
			String motif
		) {
			Parse( motif, motifSymbols, alphabet );
		}

		/// <summary> Parse a ssequence of characters into a list of Symbol.
		/// </summary>
		/// <param name="sequence"></param>
		/// <param name="symbols"></param>
		/// <param name="alphabet"></param>

		public static void Parse(
			string sequence,
			List<char> symbols,
			 IAlphabet alphabet
		) {
			
            
            // TODO: this method should be moved into a Symbol-related class.


            //TODO : will back to this parse method later
            
			symbols.Clear();
			//SymbolMeta alternative = null;

            List<char> alternative = null;
            
            for ( int i = 0; i < sequence.Length; i++ ) {
				char letter = sequence[i];
                String name = alphabet.GetFriendlyName((byte)letter);
				if ( letter == '[' ) {
					if ( alternative != null ) {
						throw new ArgumentException( "'[' within alternative is not permitted" );
					}
                    //TODO: LB, How to define the bracket as a symbols in bio .net?
                    //alternative = new SymbolMeta('#', "ALT", "Alternative");//letter, code, name
				}

				else {
					if ( letter == ']' ) {
						if ( alternative == null ) {
							throw new ArgumentException( "Opening bracket for ']' is missing!" );
						}
                        symbols.AddRange(alternative);
                        alternative = null;
					}

					else if ( alternative != null ) {
                        alternative.Add(letter);
					}
					else {
                        symbols.Add(letter);
					}
				}

			}

			if ( alternative != null ) {
				throw new ArgumentException( "']' is missing" );
			}
            
		}

		/// <summary> Reads the parameters and populate the attributes for this pattern.
		/// </summary>
		/// <param name="element">Motif Pattern node</param>
		/// <param name="definition">The container encapsulating this pattern</param>

		public override void Parse (
			XElement element,
			Definition definition
		) {
			base.Parse( element, definition );
            alphabet = AlphabetConversion.Convert(element.EnumValue<AlphabetType>("alphabet"));
			string motif = element.String( "motif" );

			if ( motif == null ) {
				throw new ArgumentException( "Required attribute 'motif' is missing in Motif." );
			}

			ParseSymbols( motif );
		}

		/// <summary> Saves the contents of this object in an xml element.
		/// </summary>
		/// <returns>An xml element containign the content of this object.</returns>

		public override XElement ToXml () {
			return base.ToXml( "Motif",
				new XAttribute( "alphabet", alphabet.Name ),
				new XAttribute( "motif", Letters )
			);
		}

    }
}
