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
	/// This abstract class provides basic functionality for profiles. A profile is a 
	/// sequence of patterns described by weight matrices, consensus patterns, 
	/// regular expression, motifs, ... which are usually separated by gaps.<para></para>
	/// This class handles only the administrative aspects but does not implement
	/// any matching strategies between a profile and a sequence.
	/// Patterns are stored as <see cref="QUT.Bio.BioPatML.Patterns.ProfileElement">
	/// ProfileElement </see> which are an aggregation of
	/// a pattern and its preceding gap.
	/// </summary>
	public abstract class Profile : PatternComplex, IEnumerable<ProfileElement> {
		/// <summary> List of patterns and gaps
		/// </summary>

		protected List<ProfileElement> profileElements = new List<ProfileElement>();

		/// <summary> Parameterless constructor for deserialization.
		/// </summary>

		public Profile () {

		}
		
		/// <summary> The default constructor for profile
		/// </summary>
		/// <param name="name"></param>

		public Profile ( String name ) : base( name ) { }

		/// <summary> <see cref="QUT.Bio.BioPatML.Patterns.IPattern">IPattern #Increment()</see>
		/// </summary>
		/// <returns></returns>

		public override int Increment {
			get {
				int maxInc = -int.MaxValue;
				int gapSum = 0;

				for ( int i = 0; i < Count; i++ ) {
					ProfileElement element = this[i];
					gapSum += element.MaxGap;
					int inc = element.Pattern.Increment - gapSum;

					if ( inc > maxInc )
						maxInc = inc;
				}

				return ( Math.Max( 1, maxInc ) );
			}
		}

		/// <summary> Gets the number of patterns/elements of the profile.
		/// </summary>

		public new int Count {
			get { return profileElements.Count; }
		}

		/// <summary> Gets a profile element based on the given index.
		/// <para></para>
		/// Note that all patterns in a profile are stored 
		/// as <see cref="QUT.Bio.BioPatML.Patterns.ProfileElement">ProfileElement</see> 
		/// which is a gap description followed by the genuine
		/// pattern.
		/// </summary>
		/// <param name="index">Index within pattern list.</param>
		/// <returns>Returns a profile element or null if the index is invalid.</returns>

		public new ProfileElement this[int index] {
			get {
				if ( index < 0 || index >= Count )
					return ( null );

				return ( profileElements[index] );
			}
		}

		/// <summary> Adds a pattern description, e.g. a sequence, a weight matrix to the 
		/// profile. This method assumes that the pattern follows the preceding
		/// pattern in the profile (if there is one) and that there is no gap
		/// between the patterns.
		/// 
		/// </summary>
		/// <param name="pattern">Any object which implements the pattern interface.</param>
		/// <returns>Returns a reference to the added {ProfileElement}.</returns>

		public ProfileElement Add ( IPattern pattern ) {
			return ( Add( this[Count - 1], ProfileElement.AlignmentType.END, 0, 0, pattern ) );
		}

		/// <summary> Adds a pattern preceeded by a gap of variable length to the profile. This
		/// pattern can't be the first pattern of the profile because the gap is
		/// defined between the given pattern and a preceding pattern!
		/// </summary>
		/// <param name="alignmentType">Alignment the gap is based on, e.g. END, START of the
		/// the preceding profile element or NONE if there is no preceding profile element 
		/// or no gap.
		/// </param>
		/// <param name="minGap">Minimum gap length.</param>
		/// <param name="maxGap">Maximum gap length. Must be greater than or equal to the 
		/// minimum gap length.</param>
		/// <param name="pattern">Any object which implements the pattern interface.</param>
		/// <returns>Returns a reference to the added { ProfileElement}.</returns>

		public ProfileElement Add (
			ProfileElement.AlignmentType alignmentType,
			int minGap,
			int maxGap,
			IPattern pattern
		) {
			if ( Count == 0 )
				throw new ArgumentException
					( "First pattern of a profile must not have a gap!" );

			return
				( Add( this[Count - 1], alignmentType, minGap, maxGap, pattern ) );
		}

		/// <summary> Adds a pattern preceeded by a gap of variable length to the profile. This
		/// pattern can't be the first pattern of the profile because the gap is
		/// defined between the given pattern and a preceding pattern! The gap
		/// is assumed to start at the end of the preceding pattern.
		/// Use #add(IPattern) to add the first ungapped pattern to the profile.
		/// </summary>
		/// <param name="minGap">Minimum gap length.</param>
		/// <param name="maxGap">Maximum gap length. Must be greater than or equal to the 
		/// inimum gap length.</param>
		/// <param name="pattern">Any object which implements the pattern interface.</param>
		/// <returns>a reference to the added {@link ProfileElement}.</returns>

		public ProfileElement Add (
			int minGap,
			int maxGap,
			IPattern pattern
		) {
			return ( Add( this[Count - 1], ProfileElement.AlignmentType.END, minGap, maxGap, pattern ) );
		}


		/// <summary> Adds a pattern preceeded by a gap of variable length to the profile.
		/// </summary>
		/// <param name="refElement">
		/// refElement Reference to the preceding profile element. Null if there
		/// is none. If there is gap then there must be a preceding profile element 
		/// defined!
		/// </param>
		/// <param name="alignmentType">Alignment the gap is based on, e.g. END, START of the 
		/// the preceding profile element or NONE if there is no preceding profile 
		/// element or no gap.</param>
		/// <param name="minGap">Minimum gap length.</param>
		/// <param name="maxGap">Maximum gap length. Must be greater than or equal to the 
		/// minimum gap length.</param>
		/// <param name="pattern">Any object which implements the pattern interface.</param>
		/// <returns></returns>

		public ProfileElement Add (
			ProfileElement refElement,
			ProfileElement.AlignmentType alignmentType,
			int minGap,
			int maxGap,
			IPattern pattern
		) {
			ProfileElement element = new ProfileElement(
				this,
				string.Empty,
				refElement, 
				alignmentType, 
				minGap, 
				maxGap, 
				pattern 
			);
			Add( element );
			return ( element );
		}

		/// <summary> Adds a pattern preceeded by a gap of variable length to the profile.
		/// </summary>
		/// <param name="elementIndex">
		/// Index to a preceding profile element. If the index
		/// smaller than zero it is assumed that there is no preceding profile element.
		/// In this case minGap and maxGap must be zero!
		/// Please note that the same pattern can not be added twice except it it
		/// a copy (with its own internal match object).
		/// </param>
		/// <param name="alignmentType">
		/// Alignment the gap is based on, e.g. END, START of the 
		/// the preceding profile element or NONE if there is no preceding profile element 
		/// or no gap.
		/// </param>
		/// <param name="minGap"> The min length</param>
		/// <param name="maxGap">
		/// Maximum gap length. Must be greater than or equal to the 
		/// minimum gap length.
		/// </param>
		/// <param name="pattern">Any object which implements the pattern interface.</param>
		/// <returns>Returns a reference to the added {ProfileElement}.</returns>

		public ProfileElement Add (
			int elementIndex,
			ProfileElement.AlignmentType alignmentType,
			int minGap,
			int maxGap,
			IPattern pattern
		) {
			if ( IndexOf( pattern ) >= 0 ) {
				throw new ArgumentException ( "The same pattern can not be added twice to a profile!" );
			}

			ProfileElement element = new ProfileElement (
				this,
				string.Empty,
				this[elementIndex], 
				alignmentType, 
				minGap, 
				maxGap, 
				pattern 
			);

			Add( element );
			return ( element );
		}

		/// <summary> Adds a region to this profile.
		/// </summary>
		/// <param name="profileElement"></param>

		internal void Add (
			ProfileElement profileElement
		) {
			profileElements.Add( profileElement );
			LatestMatch.SubMatches.Add( profileElement.Pattern.LatestMatch );
		}

		/// <summary> Getter for a pattern. 
		/// </summary>
		/// <param name="index">Your requested index</param>
		/// <returns>Returns the number of patterns/elements.</returns>

		public IPattern Pattern ( int index ) {
			return ( this[index].Pattern );
		}

		/// <summary> Getter for the index of the given profile element.
		/// </summary>
		/// <param name="element">Profile element.</param>
		/// <returns>
		/// Index of the given profile element within the profile or -1 if
		/// the profile element is not part of the profile.
		/// </returns>

		public int IndexOf ( ProfileElement element ) {
			return ( profileElements.IndexOf( element ) );
		}

		/// <summary> Getter for the index of the given pattern.
		/// </summary>
		/// <param name="pattern">Pattern reference.</param>
		/// <returns>
		/// Index of the given pattern within the profile or -1 if the
		/// pattern is not part of the profile.
		/// </returns>

		public int IndexOf ( IPattern pattern ) {
			for ( int i = 0; i < Count; i++ )
				if ( this[i].Pattern == pattern )
					return ( i );

			return -1; //not found
		}

		/// <summary> Get an XML representation of this object.
		/// </summary>
		/// <returns></returns>

		public override XElement ToXml () {
			XElement result = new XElement( "Profile",
				new XAttribute( "mode", this is ProfileAll ? "ALL" : "BEST" ),
				new XAttribute( "name", Name ),
				new XAttribute( "threshold", Threshold ),
				new XAttribute( "impact", Impact )
			);

			foreach ( var profileElement in this.profileElements ) {
				result.Add( profileElement.ToXml() );
			}

			return result;
		}

		/// <summary> Reads the parameters and populate the attributes for this pattern.
		/// 
		/// Hides the ReadNode method for PatternComplex
		/// </summary>
		/// <param name="element">Profile Pattern node</param>
		/// <param name="containingDefinition">The container encapsulating this pattern</param>

		public override void Parse (
			XElement element,
			Definition containingDefinition
		) {
			Name = element.String( "name" );
			Threshold = element.Double( "threshold", 1.0 );
			Impact = element.Double( "impact", 1.0 );

			foreach ( var child in element.Elements() ) {
				if ( child.Name.Equals( "Region" ) ) {
					ProfileElement profileElement = new ProfileElement( this );
					profileElement.Parse( child, containingDefinition );
					Add( profileElement );
				}
				else {
					throw new ArgumentException( "Profile patterns may only contain nested Regions" );
				}
			}
		}

		#region IEnumerable<ProfileElement> Members

		/// <summary> Gets and enumerator for the ProfileElements contained in this collection.
		/// </summary>
		/// <returns></returns>
		
		public IEnumerator<ProfileElement> GetEnumerator () {
			return profileElements.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
			return profileElements.GetEnumerator();
		}

		#endregion
	}
}
