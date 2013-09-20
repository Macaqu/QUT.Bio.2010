using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QUT.Bio.BioPatML.Common.Structures;
using System.Xml;
using System.Xml.Linq;
using QUT.Bio.BioPatML.Common.XML;
using QUT.Bio.BioPatML.Sequences;
using QUT.Bio.BioPatML.Util;
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
	/// This class defines a gap pattern. This pattern matches any sequence
	/// within a specified length interval and can have a similiarity score
	/// based on a length distribution. 
	/// </summary>
	public class Gap : PatternFlexible {
		#region -- Automatic Properties --

		/// <summary>
		/// Gap similarities by its length distribution.
		/// External party can not modify this variable only internal member
		/// has permission to do so.
		/// </summary>
		private double[] GapSimArr { get; set; }

		#endregion

		/// <summary> Default constructor used to build an empty Gap with a unique name
		/// </summary>
		public Gap () { }

		/// <summary> Constructs a gap pattern of variable length.
		/// </summary>
		/// <param name="name">Name of Gap Pattern</param>
		/// <param name="minLength">Minimum length of the gap (Can be negative).</param>
		/// <param name="maxLength">Maximum length of the gap. </param>
		/// <param name="incLength">Length increment for the gap.</param>
		/// <param name="weights">
		///		Weights for different lengths of the gap. The first weight
		///		is the weight for a gap of minLength. Additional weights are for extended
		///		gap lengths. The weights are automatically scaled to [0..1].
		/// </param>
		/// <param name="threshold">Threshold for the gap.</param>
		/// <exception cref="System.ArgumentException">Thrown only when 
		/// the maximum length is not bigger than the minimum length.</exception>

		public Gap (
			string name,
			int minLength,
			int maxLength,
			double incLength = 1,
			double[] weights = null,
			double threshold = 1.0
		)
			: base( name ) {
			base.Set( minLength, maxLength, incLength );
			Weights = weights;
			Threshold = threshold;
		}

		/// <summary> Constructs an anonymous gap pattern of variable length.
		/// </summary>
		/// <param name="minLength">Minimum length of the gap (Can be negative).</param>
		/// <param name="maxLength">Maximum length of the gap. </param>
		/// <param name="incLength">Length increment for the gap.</param>
		/// <param name="weights">
		///		Weights for different lengths of the gap. The first weight
		///		is the weight for a gap of minLength. Additional weights are for extended
		///		gap lengths. The weights are automatically scaled to [0..1].
		/// </param>
		/// <param name="threshold">Threshold for the gap.</param>
		/// <exception cref="System.ArgumentException">Thrown only when 
		/// the maximum length is not bigger than the minimum length.</exception>

		public Gap (
			int minLength,
			int maxLength,
			double incLength = 1,
			double[] weights = null,
			double threshold = 1.0
		) : this ( null, minLength, maxLength, incLength, weights, threshold ) {}

		#region -- Public Methods --

		/// <summary>
		/// Sets the gap weights. All weights must be greater or equal to zero.
		/// Weights internally automatically scaled to the interval [0..1]. 
		/// <para></para>
		/// A weight vector with constant values is transformed to a
		/// weight vector with all elements set to one.
		/// </summary>
		public double[] Weights 
		{
			internal get { 
				return GapSimArr; 
			}
			set {
				if ( value != null ) {
					double min = value[SArray.MinIndex( value )];
					double max = value[SArray.MaxIndex( value )];
					int len = value.Length;

					if ( len == 0 )
						throw new ArgumentException
							( "Invalid numer of weights!" );

					GapSimArr = new double[len];

					for ( int i = 0; i < len; i++ )
						GapSimArr[i] = max == min ? 1.0 : ( value[i] - min ) / ( max - min );


				}
				else
					GapSimArr = null;
			}
		}

		/// <summary>
		/// Returns the gap similarity score according to the given length.
		/// </summary>
		/// <param name="length">Current gap length.</param>
		/// <returns>Returns the similarity score for this gap length.</returns>
		public double TabulateGapSim ( int length ) {
			if ( GapSimArr == null )
				return 1.0;

			else
				if ( length <= 0 )
					return GapSimArr[0];

				else
					if ( length >= GapSimArr.Length )
						return GapSimArr[GapSimArr.Length - 1];

			return GapSimArr[length];
		}

		#endregion

		#region -- IMatcher Members --
		/// <summary>
		/// Implementation of the pattern interface. A gap pattern matches any sequence.
		/// <see cref="QUT.Bio.BioPatML.Patterns.IMatcher">IMatcher interface</see>.
		/// </summary>
		/// <param name="sequence">Sequence to compare with.</param>
		/// <param name="position">Matching position.</param>
		/// <returns></returns>
		public override Match Match
			( ISequence sequence, int position ) {
			//New matching code 
			int length = NextLength();
			LatestMatch.Set( sequence, position, length, Strand.Forward, TabulateGapSim( length - MinLength ) );
			return LatestMatch;
		}

		#endregion

		/// <summary> Implementation of the pattern interface.
		/// Reads in the Gap node and populate the attributes accordingly.
		/// <see cref="QUT.Bio.BioPatML.Patterns.IPattern">IPattern</see>
		/// </summary>
		/// <param name="element"></param>
		/// <param name="containingDefinition"></param>

		public override void Parse (
			XElement element,
			Definition containingDefinition
		) {
			Name = element.String( "name" );
			Threshold = element.Double( "threshold", 0.0 );
			Impact = element.Double( "impact", 1.0 );
			Set(
				element.Int( "minimum" ),
				element.Int( "maximum" ),
				element.Double( "increment", 1.0 )
			);

			String weightstr = XMLHelper.ChildContent( element, "Weights" );

			Weights = ( weightstr == null ? null : PrimitiveParse.StringToDoubleArray( weightstr ) );
		}

		/// <summary> Saves the contents of this object in an xml element.
		/// </summary>
		/// <returns>An xml element containign the content of this object.</returns>

		public override XElement ToXml () {
			XElement result = base.ToXml( "Gap",
				new XAttribute( "minimum", MinLength ),
				new XAttribute( "maximum", MaxLength ),
				new XAttribute( "increment", IncLength )
			);

			if ( Weights != null ) result.Add( new XElement( "Weights", UtilHelper.Join( ";" ) ) );

			return result;
		}
	}
}
