using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using QUT.Bio.BioPatML.Symbols;
using QUT.Bio.BioPatML.Sequences;
//using QUT.Bio.BioPatML.Alphabets;
using QUT.Bio.BioPatML.Common.XML;
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
	/// This class defines a composition pattern. A composition pattern describes
	/// the symbol composition of a sequence section of variable length.
	/// </summary>
	public partial class Composition : PatternFlexible {

		/// <summary> List of available match modes. </summary>

		public enum MatchMode {
			/// <summary> Match all. </summary>
			
			ALL,

			/** <summary> Match best. </summary>*/
			
			BEST
		};

		#region -- Fields --

        private IAlphabet alphabet;

		/// <summary>
		/// Alphabet the composition is based on
		/// </summary>
        public IAlphabet Alphabet
        {
            get { 
                
                return alphabet;} 
            
            private set{
                alphabet = value;
            }
        
        }

		#endregion

		#region -- Private Fields --

		/// <summary>
		/// Dictionary that maps a symbol to a weight 
		/// </summary>
		private Dictionary<byte, Double> SymbolWeights = new Dictionary<byte, double>();

		/// <summary>
		/// Default weight
		/// </summary>
		private double defaultWeight = 0;

		/// <summary>
		/// Maximum weight 
		/// </summary>
		private double maxWeight = -Double.MaxValue;

		/// <summary>
		/// Minimum weight 
		/// </summary>
		private double minWeight = Double.MaxValue;

		/// <summary> Match mode: ALL or BEST 
		/// </summary>
		private MatchMode mode;

		/** Matcher used to match a composition description against a sequence */
		private IMatcher matcher;

		#endregion -- Private Fields --

		/// <summary> Default constructor for creating a plain Composition pattern object 
		/// </summary>
		public Composition () {
		}

		/// <summary> Constructs a composition pattern of variable length.
		/// </summary>
		/// <exception cref="System.ArgumentException">When given mode is invalid</exception>
		/// <param name="name">Name of the pattern.</param>
		/// <param name="alphabetType">Name of the alphabet the pattern operates on.</param>
		/// <param name="minLength">Minimum length of the sequence to match. </param>
		/// <param name="maxLength">Maximum length of the sequence to match.</param>
		/// <param name="incLength">Length increment for the pattern.</param>
		/// <param name="mode">Match mode: BEST, ALL</param>
		/// <param name="threshold">Threshold for the composition.</param>
		public Composition (
			String name,
			IAlphabet alphabet,
			int minLength,
			int maxLength,
			double incLength,
			MatchMode mode,
			double threshold
		)
			: base( name ) {
			base.Set( minLength, maxLength, incLength );
			this.mode = mode;
			Threshold = threshold;
			Alphabet = alphabet;
			SetMatcher();
		}

		/// <summary>
		/// Gets the mode of this composition pattern
		/// <para></para>
		/// Note: Only internal library has the permission to change the mode of composition,
		/// even so, in most cases we are not suppose to tweak the mode of composition in the middle
		/// of computation.
		/// </summary>
		public MatchMode Mode {
			get {
				return this.mode;
			}

			internal set {
				mode = value;
				SetMatcher();
			}
		}

        
		private void SetMatcher () {
			matcher = mode == MatchMode.ALL ? (IMatcher) new MatcherAll( this ) : new MatcherBest( this );
		}

		/// <summary>
		/// Adds a symbol and its weight to the composition.
		/// </summary>
		/// <exception cref="System.ArgumentException">Thrown when
		/// duplicate symbols were detected.</exception>
		/// <param name="symbol">Symbol to add.</param>
		/// <param name="weight">Weight of the symbol. Can be any value.</param>
		public void Add ( char symbol/*Symbol symbol*/, double weight ) {
			if ( SymbolWeights.ContainsKey((byte)symbol ) )
				throw new ArgumentException
					( "Duplicate symbol " + symbol + " in composition!" );

			SymbolWeights.Add((byte)symbol, weight );

			if ( weight > maxWeight )
				maxWeight = weight;

			if ( weight < minWeight )
				minWeight = weight;
		}

		/// <summary>
		/// Adds a symbol and its weight to the composition.
		/// </summary>
		/// <param name="letter">Letter to add.</param>
		/// <param name="weight">Weight of the symbol. Can be any value.</param>
        /*
		public void Add ( char letter, double weight ) {
			Add( Alphabet[letter], weight );
		}

         */
         
		/// <summary> Gets the symbol weight
		/// </summary>
		/// <param name="symbol">Symbol</param>
		/// <returns>Returns the weight for the Symbol or the default weight if no weight
		/// for the symbol is defined.</returns>

		public double Weight ( /*Symbol*/ char symbol ) {
			//Fix on 17 March
			if ( SymbolWeights.ContainsKey((byte) symbol ) )
				return SymbolWeights[(byte)symbol];

			return defaultWeight;
		}

		/// <summary>
		/// Gets the weight of a symbol
		/// </summary>
		/// <param name="letter">One letter code of the symbol.</param>
		/// <returns>Returns the weight for the Symbol or the default weight if no weight
		/// for the symbol is defined.</returns>
        /*
		public double Weight ( char letter ) {
			return Weight( Alphabet[letter] );
		}
        */
		/// <summary>
		/// Gets the default weight value / sets the defaultweight value
		/// (only internal library are allowed to change the value of defaultWeight.
		/// </summary>

		public double DefaultWeight {
			get { return defaultWeight; }
			set {
				defaultWeight = value;
				if ( value > maxWeight )
					maxWeight = value;
				if ( value < minWeight )
					minWeight = value;
			}
		}

		/// <summary> Gets the Minimum weight
		/// </summary>

		public double MinWeight {
			get { return minWeight; }
		}

		/// <summary> Gets the maximum weight
		/// </summary>

		public double MaxWeight {
			get { return maxWeight; }
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
            //return new Match(this, sequence, position, (int)sequence.Count, Strand.Forward, 0.0);
            return matcher.Match( sequence, position );
        }

		/// <summary> The increment value used by match
		/// </summary>

		public override int Increment {
			get {
				return matcher.Increment;
			}
		}

        private bool IsValidAlphabet(char symbol, int offset, int length) {
            byte[] symbolsArray = new byte[] {(byte)symbol};

            if( this.alphabet.Equals(DnaAlphabet.Instance) ||
                this.alphabet.Equals(RnaAlphabet.Instance) ||
                this.alphabet.Equals(ProteinAlphabet.Instance) 
                ){
                return alphabet.ValidateSequence(symbolsArray, (long)offset, (long)length);
            }
            
            return false;
        }

		/// <summary>
		/// Reads the parameters and populate the attributes for this pattern.
		/// </summary>
		/// <param name="element">Composition Pattern node</param>
		/// <param name="definition">The container encapsulating this pattern</param>

		public override void Parse (
			XElement element,
			Definition definition
		) {
			Name = element.String( "name" );
			Threshold = element.Double( "threshold", 1.0 );
			Impact = element.Double( "impact", 1.0 );
			//Alphabet = /*AlphabetFactory.Instance( element.EnumValue<AlphabetType>( "alphabet" ) );*/ 

			if ( !Enum.TryParse<MatchMode>( element.String( "mode" ), true, out mode ) ) {
				throw new ArgumentException( "Invalid match mode in Composition: expecting 'all' or 'best'" );
			}

			SetMatcher();

			Set(
				element.Int( "minimum" ),
				element.Int( "maximum" ),
				element.Double( "increment", 1.0 )
			);

			foreach ( XElement childElement in element.Elements() ) {
				switch ( childElement.Name.ToString() ) {
					case "Symbol":
						char symbol = childElement.String( "letter" )[0];
						double weight = childElement.Double( "weight", 1.0 );

                        if (!IsValidAlphabet(symbol, 1, 1))
                        {
                            throw new ArgumentException("Invalid alphabet letter: '" + symbol + "'!");
						}

                        Add(symbol, weight);
						break;

					case "Default":
						DefaultWeight = ( childElement.Double( "weight", 1.0 ) );
						break;

					default:
						throw new ArgumentException( string.Format( "Unknown child element in Composition : {0}", childElement.Name ) );
				}
			}
		}


        private string GetAlphabetName() { 
            
            if (alphabet.Equals(DnaAlphabet.Instance))
            {
                return "DNA";
            }
            else if (alphabet.Equals(RnaAlphabet.Instance))
            {
                return "RNA";
            }
            else if (alphabet.Equals(ProteinAlphabet.Instance))
            {
                return "AA";
            }
            else {
                return string.Empty;
            }
        
        }

		/// <summary> Saves the contents of this object in an xml element.
		/// </summary>
		/// <returns>An xml element containign the content of this object.</returns>

		public override XElement ToXml () {
			XElement result = new XElement( "Composition",
				new XAttribute( "name", Name ),
				new XAttribute( "threshold", Threshold ),
				new XAttribute( "impact", Impact ),
				new XAttribute( "alphabet", GetAlphabetName() ),
				new XAttribute( "mode", mode ),
				new XAttribute( "minimum", MinLength ),
				new XAttribute( "maximum", MaxLength ),
				new XAttribute( "increment", IncLength )
			);

			foreach ( var p in SymbolWeights ) {
				result.Add( new XElement( "Symbol",
					new XAttribute( "letter", p.Key ),
					new XAttribute( "weight", p.Value )
				) );
			}

			result.Add( new XElement( "Default", new XAttribute( "weight", DefaultWeight ) ) );

			return result;
		}


	}
}
