using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
//using QUT.Bio.BioPatML.Symbols;
using QUT.Bio.BioPatML.Sequences.List;
using QUT.Bio.BioPatML.Sequences;
using QUT.Bio.BioPatML.Statistic;
using QUT.Bio.BioPatML.Common.XML;
//using QUT.Bio.BioPatML.Alphabets;
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
	/// This pattern describes a block of aligned sequences. Simplification
	/// of a <see cref="QUT.Bio.BioPatML.Patterns.PWM"> PWM (position weight matrix) </see>
	/// and it is directly derived from the PWM class.
	/// </summary>
	public class Block : PWM {
        private readonly SequenceList<ISequence> sequenceList = new SequenceList<ISequence>();


		/// <summary> Parameterless constructor for deserialization.
		/// </summary>

		public Block () { }

		/// <summary> Constructs a Block of aligned sequences
		/// (<see cref="QUT.Bio.BioPatML.Patterns.PWM"> PWM </see>).
		/// </summary>
		/// <param name="name">Name for element block</param>
		/// <param name="sequenceList"> List of aligned sequences. </param>
		/// <param name="background"> Histogram with base counts of the background
		/// sequences.</param>
		/// <param name="threshold"> Similarity threshold. </param>
		public Block (
			String name,
			SequenceList<ISequence> sequenceList,
			HistogramSymbol background,
			double threshold
		)
			: base( name, sequenceList.ElementAt(0).Alphabet, threshold ) {
			
            this.sequenceList.AddRange(sequenceList);
			Estimate( background );
		}

		/// <summary>
		/// Estimates the weights of the PWM that's behind a Block pattern.
		/// </summary>
		/// <exception cref="System.ArgumentException">
		/// Thrown when sequences length are not equal</exception>
		/// <param name="background"> Histogram with base counts of the background
		/// sequences. Can be null. In that case all frequencies are set equally.</param>

		private void Estimate ( 
			HistogramSymbol background 
		) {
			int length = sequenceList.MinLength();

			if ( sequenceList.MaxLength() != length )
				throw new ArgumentException
					( "Sequences must be of equal length!" );

			if ( background == null ) {
				background = new HistogramSymbol();

				foreach ( char sym in Alphabet )
					background.Add( sym, Alphabet );
			}

			base.Init( length );
			base.Estimate( sequenceList, 1, background );
		}

		/// <summary> Reads the parameters and populate the attributes for this pattern.
		/// </summary>
		/// <exception cref="System.ArgumentNullException">
		///		Thrown when sequences in blocks are missing.
		/// </exception>
		/// <param name="element"></param>
		/// <param name="containingDefinition">The Definition element where the node sits in</param>

		public override void Parse (
			XElement element,
			Definition containingDefinition
		) {
			base.Parse( element, containingDefinition );

			foreach ( var childElement in element.Elements() ) {
				if ( childElement.Name == "Sequence" ) {
					String letters = element.Value.Trim();

					if ( letters == null ) {
						throw new ArgumentNullException( "Sequences in Block are missing!" );
					}

					sequenceList.Add( new Sequence( Alphabet, letters, false ) );
				}
			}

			Estimate( null );
		}

		/// <summary> Writes the contents of this Block to a XElement. </summary>
		/// <returns></returns>

		public override XElement ToXml () {
			return base.ToXml( "Block",
				sequenceList.Select( seq => new XElement( "Sequence", seq) ),
				new XAttribute( "alphabet", Alphabet.Name )
			);
		}
	}
}
