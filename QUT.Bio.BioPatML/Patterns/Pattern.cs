using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
//using QUT.Bio.BioPatML.Sequences;
using Bio;
using QUT.Bio.BioPatML.Common.XML;
using System.Reflection;

/*****************| Queensland University Of Technology |********************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
/*
  <xsd:group name="Pattern">
    <xsd:choice>
      <xsd:element name="Void"          type="Void"/>
      <xsd:element name="Alignment"     type="Alignment"/>
      <xsd:element name="Constraint"    type="Constraint"/>
      <xsd:element name="Use"           type="Use"/>
      <xsd:element name="Any"           type="Any"/>
      <xsd:element name="Gap"           type="Gap"/>
      <xsd:element name="Motif"         type="Motif"/>
      <xsd:element name="Prosite"       type="Prosite"/>
      <xsd:element name="Regex"         type="Regex"/>
      <xsd:element name="Block"         type="Block"/>
      <xsd:element name="PWM"           type="PWM"/>
      <xsd:element name="Composition"   type="Composition"/>
      <xsd:element name="Repeat"        type="Repeat"/>
      <xsd:element name="Iteration"     type="Iteration"/>
      <xsd:element name="Logic"         type="Logic"/>
      <xsd:element name="Set"           type="Set"/>
      <xsd:element name="Series"        type="Series"/>
    </xsd:choice>
  </xsd:group>
 */

namespace QUT.Bio.BioPatML.Patterns {

	/// <summary> Abstract base class for patterns. </summary>

	public abstract class Pattern : IPattern {

		private double threshold = 1.0;

		/// <summary> The minimum required similarity threshold for a match.
		/// <para>
		///		This is required to lie within the interval [0,1].
		/// </para>
		/// </summary>

		public  double Threshold {
			get {
				return threshold;
			}
			set {
				if ( value < 0 || value > 1 ) throw new ArgumentException( "Threshold out of range" );

				threshold = value;
			}
		}

		/// <summary> Name of pattern </summary>

		private string name;

		/// <summary> The match object of a pattern </summary>

		private Match match;

		/// <summary> Initialises this pattern, setting its name to an anonymous (automatic) value. </summary>

		public Pattern () {
			match = new QUT.Bio.BioPatML.Patterns.Match( this );
			Name = string.Empty;
		}

		/// <summary> Construct a pattern with a given name usually passed from child class </summary>
		/// <param name="name"> name of element</param>

		public Pattern ( string name ) {
			match = new QUT.Bio.BioPatML.Patterns.Match( this );
			Threshold = 0.0;
			Name = name;
			Impact = 1.0; //start it with a default value
		}

		/// <summary> Gets or set the name of this pattern. If the supplied value is null or empty, an automatic serial id will be created. </summary>

		public String Name {
			get { return name; }

			set {
                this.name = value;
				//AutoName.SetName( ref name, value );
			}
		}

		/// <summary> Sets/ Gets the impact of a pattern. This a weight is taken into account
		/// when the overall similarity of a structured pattern, consisting of
		/// other patterns, is calculated.
		/// <para></para>
		/// Note the given param value for Impact weight must be between zero and one.
		/// </summary>
		public virtual double Impact {
			get { return match.Impact; }
			set { match.Impact = value; }
		}

		
		/// <summary> Get the position increment after matching a pattern. Some pattern
		/// can match several times with different length at the same position. In
		/// this case the increment is zero until all matches are performed. For some
		/// patterns an increment greater than one can be performed, e.g. string
		/// searching with the Boyer-Moore algorithm. 
		/// </summary>
		/// <returns>
		/// Returns the increment of the search position.
		/// </returns>

		public virtual int Increment {
			get {
				return ( 1 );
			}
		}
        
		private static Tuple<string, Type> [] lookup = {
			new Tuple<string, Type>( "Any", typeof(Any) ),
			//new Tuple<string, Type>( "Alignment", typeof(Alignment) ),
			new Tuple<string, Type>( "Composition", typeof(Composition) ),
			//new Tuple<string, Type>( "Constraint", typeof(Constraint) ),
			//new Tuple<string, Type>( "Iteration", typeof(Iteration) ),
			//new Tuple<string, Type>( "Logic", typeof(Logic) ),
			new Tuple<string, Type>( "Motif", typeof(Motif) ),
			//new Tuple<string, Type>( "PWM", typeof(PWM) ),
			//new Tuple<string, Type>( "Regex", typeof(RegularExp) ),
			//new Tuple<string, Type>( "Prosite", typeof(Prosite) ),
			//new Tuple<string, Type>( "Block", typeof(Block) ),
			//new Tuple<string, Type>( "Gap", typeof(Gap) ),
			//new Tuple<string, Type>( "Repeat", typeof(Repeat) ),
			new Tuple<string, Type>( "Void", typeof(VoidPattern) )//,
			//new Tuple<string, Type>( "Use", typeof(Use) ),
		};
        
		private static Tuple<string, Type, Type> [] allOrBestLookup = {
		//	new Tuple<string, Type, Type>( "Set", typeof(SetAll), typeof(SetBest) ),
		//	new Tuple<string, Type, Type>( "Series", typeof(SeriesAll), typeof(SeriesBest) ),
		//	new Tuple<string, Type, Type>( "Profile", typeof(ProfileAll), typeof(ProfileBest) )
		};
        
		/// <summary> Reads a pattern from a starting specified node. This method
		/// recursivly calls the reading methods of the different patterns.
		/// </summary>
		/// <param name="element">Node of the pattern the reading starts with.</param>
		/// <returns>The read pattern or null if there is no pattern to read.</returns>
		/// <exception cref="System.SystemException">Thrown when unknown pattern was found</exception>
        
		public static IPattern CreateFrom (
			XElement element
		) {
			if ( element == null ) return null;

			IPattern result = null;

			foreach ( var t in lookup ) {
				if ( t.Item1 == element.Name ) {
					Type type =  t.Item2;
					ConstructorInfo constructor = type.GetConstructor( Type.EmptyTypes );
					result = (IPattern) constructor.Invoke( null );
				}
			}

			if ( result == null ) {
				string mode = element.String( "mode" );

				foreach ( var t in allOrBestLookup ) {
					if ( t.Item1 == element.Name ) {
						Type type = mode == "ALL" ? t.Item2 : t.Item3;
						ConstructorInfo constructor = type.GetConstructor( Type.EmptyTypes );
						result = (IPattern) constructor.Invoke( null );
					}
				}
			}

			if ( result == null ) {
				throw new ArgumentException( string.Format( "Unknown pattern type {0}", element.Name ) );
			}

			return result;
		}
        
		/// <summary> Tries to match the supplied sequence, return the results in a Match object.
		/// </summary>
		/// <param name="sequence"></param>
		/// <param name="position"></param>
		/// <returns></returns>

		public abstract Match Match (
			ISequence sequence,
			int position
		);

		/// <summary> Creates an Xml element representing this Pattern.
		/// </summary>
		/// <returns>An Xml element representing this Pattern.</returns>

		public abstract XElement ToXml ();

		/// <summary> Loads this pattern definition from the supplied Xml element.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="containingDefinition"></param>

		public virtual void Parse (
			XElement element,
			Definition containingDefinition
		) {
			Name = element.String( "name" );
			Threshold = element.Double( "threshold", 0.0 );
			Impact = element.Double( "impact", 1.0 );
		}

		/// <summary> 
		/// </summary>
		/// <param name="elementName"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>

		internal XElement ToXml (
			string elementName,
			params XAttribute[] attributes
		) {

			XElement result = new XElement( elementName );

			//if ( !AutoName.IsAnonymous( Name ) ) result.Add( new XAttribute( "name", Name ) );
			if ( Threshold > 0 ) result.Add( new XAttribute( "threshold", Threshold ) );
			if ( Impact < 1 ) result.Add( new XAttribute( "impact", Impact ) );

			result.Add( attributes );

			return result;
		}

		internal XElement ToXml (
			string elementName,
			IEnumerable<XElement> childElements,
			params XAttribute[] attributes
		) {

			XElement result = ToXml( elementName, attributes );

			foreach ( var childElement in childElements ) {
				result.Add( childElement );
			}

			return result;
		}

		/// <summary> By default, patterns other than structured patterns do not have nested child patterns. </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="NonStructuredPatternException">If this patern does not support nested patterns.</exception>

		public virtual IPattern Child ( string name ) {
			throw new NonStructuredPatternException();
		}

		private class SimplePatternEnumerator : IEnumerable<IPattern> {
			private IPattern pattern;

			public SimplePatternEnumerator ( IPattern pattern ) {
				this.pattern = pattern;
			}

			public IEnumerator<IPattern> GetEnumerator () {
				yield return pattern;
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
				yield return pattern;
			}
		}

		/// <summary> Gets a list containing this element and its children. </summary>

		public virtual IEnumerable<IPattern> SelfAndChildren {
			get {
				return new SimplePatternEnumerator( this );
			}
		}

		/// <summary> Retain a record of the cumulative match status of an ongoing match operation. </summary>

		public Match LatestMatch {
			// TODO: This should be rewritten and removed.
			get { return match; }
		}
	}
}
