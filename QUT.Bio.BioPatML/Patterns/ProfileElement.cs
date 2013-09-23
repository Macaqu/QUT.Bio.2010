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
	/// A profile element describes a pattern with a gap to a preceding pattern or
	/// profile element, if there is one. It is mainly a container to aggregate
	/// the gap information and a pattern.
	/// <para></para>
	/// The gap is described by a minimum and
	/// a maximum length and refers to a position (START, END, ...) of the preceding 
	/// profile element (this is called alignment). 
	/// Profiles are built of profile elements.
	/// </summary>
	public class ProfileElement : IXmlConvertable {
		/// <summary> Enumeration used to signifiy the alignment of this ProfileElement to referenced match.
		/// </summary>
		public enum AlignmentType {
			/// <summary> No gap </summary>
			NONE,

			/// <summary> Gap alignment to the start of the preceding pattern match </summary>
			START,

			/// <summary> Gap alignment to the end of the preceding pattern match </summary>
			END,

			/// <summary> Gap alignment to the center of the preceding pattern match </summary>
			CENTER
		}

		private string name;

		/// <summary> Get or set the name of this element (required for use in references I beilieve).
		/// </summary>

		public string Name {
			get {
				return name;
			}
			set {
				AutoName.SetName( ref name, value );
			}
		}

		/// <summary> Minimum gap length
		/// </summary>

		public int MinGap { get; private set; }

		/// <summary> Current gap length
		/// </summary>

		public int CurrGap { get; set; }

		/// <summary> Maximum gap length
		/// </summary>

		public int MaxGap { get; private set; }

		/// <summary> The pattern 
		/// </summary>

		public IPattern Pattern { get; private set; }

		/// <summary> Reference to the preceding profile element the gap refers to 
		/// </summary>

		public ProfileElement RefElement { get; private set; }

		/// <summary> Alignment e.g. END,START for the gap 
		/// </summary>

		public AlignmentType Alignment { get; private set; }

		/// <summary> Getter for the start position of the gap. This position depends on the
		/// match of the preceding pattern and the alignment.
		/// </summary>

		public int GapStart {
			get {
				Match match = RefElement.Pattern.LatestMatch;

				switch ( Alignment ) {
					case AlignmentType.START: return ( (int)match.Start );
					case AlignmentType.END: return ( (int)(match.End + 1) );
					case AlignmentType.CENTER: return ( (int)(( match.End + match.Start ) / 2 + 1) );
				}

				return 0;
			}
		}

		/// <summary> Resets the current gap length to the minimum length.
		/// </summary>

		public void ResetGap () {
			this.CurrGap = MinGap;
		}

		private Profile container;

		/// <summary> Constructor for internal use
		/// </summary>

		public ProfileElement ( Profile container ) {
			this.container = container;
		}

		/// <summary> Creates a profile element. A profile element is a pattern with a 
		/// preceding gap to another profile element.
		/// </summary>
		/// <param name="container">The profile that contains this profile element.</param>
		/// <param name="name">The local name of this profile element.</param>
		/// <param name="refElement">Reference to the preceding profile element. Null if there
		/// is none. If there is gap then there must be a preceding profile element 
		/// defined!
		/// </param>
		/// <param name="alignment">
		/// Alignment the gap is based on, e.g. END, START of the 
		/// the preceding profile element or NONE if there is no preceding profile element 
		/// or no gap.
		/// </param>
		/// <param name="minGap">Minimum gap length.</param>
		/// <param name="maxGap">Maximum gap length. Must be greater than or equal to the 
		///  minimum gap length.</param>
		/// <param name="pattern">A pattern.</param>

		public ProfileElement (
			Profile container,
			string name,
			ProfileElement refElement,
			AlignmentType alignment,
			int minGap,
			int maxGap,
			IPattern pattern
		)
			: this( container ) {
			Name = name;

			this.RefElement = refElement;
			this.Alignment = alignment;
			this.MinGap = minGap;
			this.CurrGap = minGap;
			this.MaxGap = maxGap;
			this.Pattern = pattern;

			Validate();
		}

		/// <summary> Ensure that the class invariant is maintained.
		/// </summary>

		private void Validate () {
			if ( MinGap > MaxGap ) {
				throw new ArgumentException( "Minimum gap length is greater than maximum gap length." );
			}

			if ( ( RefElement == null || Alignment == AlignmentType.NONE )
				&& ( MinGap != 0 || MaxGap != 0 )
			) {
				throw new ArgumentException( "Missing reference or alignment to proceding pattern in gap definition." );
			}
		}

		/* The schema does not contain a definition for Profile and ProfileElement... */

		/// <summary> Populates the present profile element with the contents of an XMl element.
		/// </summary>
		/// <param name="element">The XMl source element.</param>
		/// <param name="containingDefinition">A reference to the containing definition.</param>

		public void Parse (
			XElement element,
			Definition containingDefinition
		) {
			Name = element.String( "name" );
			MinGap = element.Int( "minGap" );
			MaxGap = element.Int( "maxGap" );

			Alignment = AlignmentType.NONE;

			try {
				Alignment = element.EnumValue<AlignmentType>( "alignment" );
			}
			catch {
				throw new ArgumentException( "Profile element alignment should be one of NONE, START, END, CENTER" );
			}

			XElement child = element.Elements().FirstOrDefault();

			Pattern = QUT.Bio.BioPatML.Patterns.Pattern.CreateFrom ( child );
	
			if ( Pattern != null ) {
				Pattern.Parse( child, containingDefinition ); 
			}

			string reference = element.String( "reference" );

			RefElement = reference == null
				? null
				: container.FirstOrDefault( pe => pe.Name == reference );

			if ( reference != null && RefElement == null ) {
				throw new ArgumentException( "Unknown reference : " + reference );
			}

		}

		/// <summary> Gets an XML representation of this profile element.
		/// </summary>
		/// <returns></returns>

		public XElement ToXml () {
			XElement result = new XElement( "Region",
				new XAttribute( "name", name ),
				new XAttribute( "minGap", name ),
				new XAttribute( "maxGap", name ),
				new XAttribute( "alignment", name )
			);

			if ( RefElement != null ) {
				result.Add( new XAttribute( "reference", RefElement.Name ) );
			}

			if ( Pattern != null ) {
				result.Add( Pattern.ToXml() );
			}

			return result;
		}
	}
}
