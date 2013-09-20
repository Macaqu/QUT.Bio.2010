using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using QUT.Bio.BioPatML.Common.XML;
using QUT.Bio.BioPatML.Sequences;
using Bio;

/*****************| Queensland University Of Technology |********************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
/*
	<xsd:complexType name="Use">
		<xsd:attribute name="name"        type="xsd:string"               />
		<xsd:attribute name="definition"  type="xsd:string" use="required"/>
	</xsd:complexType>
*/
namespace QUT.Bio.BioPatML.Patterns {
	/// <summary>
	/// This class defines a Use pattern. The Use pattern uses a pattern definition to
	/// match a patter. The Use pattern is convenient to use a pattern multiple
	/// times.
	/// </summary>
	public class Use : Pattern {

		private Definition containingDefinition;
		private string referencePath;
		private Definition referencedDefinition;

		/// <summary> Gets a reference to the definition used by this Use. </summary>

		public Definition ReferencedDefinition {
			get {
				return referencedDefinition;
			}
		}

		/// <summary> Get a reference to the containing definition. </summary>
		
		public Definition ContainingDefinition {
			get {
				return containingDefinition;
			}
		}

		/// <summary> Gets the path to the referenced definition. </summary>

		public string ReferencePath {
			get {
				return referencePath;
			}
		}

		/// <summary> Parameterless constructor required for deserialisation. </summary>
		
		public Use () {
		}

		/// <summary> Sets the definition (indirectly) referenced by this Use. </summary>
		/// <param name="containingDefinition"></param>
		/// <param name="referencePath"></param>

		public void ReferTo (
			Definition containingDefinition,
			string referencePath
		) {
			this.containingDefinition = containingDefinition;
			this.referencePath = referencePath;
			referencedDefinition = containingDefinition.Definitions[referencePath];
		}

		/// <summary>
		/// Gets the position increment after matching a pattern.
		/// </summary>
		public override int Increment {
			get {
				return this.referencedDefinition.Pattern.Increment;
			}
		}

		/// <summary>
		/// The implementation ensures that
		/// a match fails for a given position if there is no match. Otherwise the
		/// matcher might return a match at a different position.
		/// <see cref="QUT.Bio.BioPatML.Patterns.IPattern">IPattern Match(Sequence, int) method</see>
		/// </summary>
		/// <param name="sequence"> The sequence for comparing</param>
		/// <param name="position"> Matching position</param>
		/// <returns></returns>
		
		public override Match Match ( 
			ISequence sequence, 
			int position 
		) {
			Match match = referencedDefinition.Pattern.Match( sequence, position );
			match.MatchPattern = this;
			return match;
		}

		/// <summary> Reads the parameters for a pattern at the given node.
		/// </summary>
		/// <param name="element">Use element</param>
		/// <param name="containingDefinition">Definition clause enclosing the Use element</param>

		public override void Parse (
			XElement element,
			Definition containingDefinition
		) {
			base.Parse( element, containingDefinition );
			ReferTo( containingDefinition, element.String( "definition" ) );
		}

		/// <summary> Gets a string that represents the specified definition. </summary>
		/// <returns></returns>

		public override XElement ToXml () {
			return ToXml( "Use",
				new XAttribute( "definition", referencePath )
			);
		}
	}
}
