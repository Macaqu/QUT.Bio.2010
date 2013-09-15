using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using QUT.Bio.BioPatML.Common.XML;

/*****************| Queensland University Of Technology |********************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/

/*
  <xsd:complexType name="Definitions">
    <xsd:choice minOccurs="1" maxOccurs="unbounded">
      <xsd:element name="Definition" type="Definition" />
      <xsd:element name="Import"     type="Import" />
    </xsd:choice>
  </xsd:complexType>
 */
namespace QUT.Bio.BioPatML.Patterns {
	/// <summary>
	/// Implementation of a container (list) encapsulating other sub-defininitions
	/// </summary>
	public class DefinitionList : IEnumerable<Definition> {
		private readonly Dictionary<string, Definition> definitions = new Dictionary<string, Definition>();

		/// <summary>
		/// Counts and return the number of elements within the patter: name dictionary.
		/// </summary>
		public int Count {
			get { return definitions.Count; }
		}

		/// <summary> Retrieves the desired definition element by the unique definition name. </summary>
		/// <param name="name">Name of definition</param>
		/// <returns>The definition object</returns>

		public Definition this[String name] {
			get {
				try {
					string [] names = name.Split( '.' );
					return this[names, 0];
				}
				catch {
					// throw new ArgumentException( string.Format( "Definition '{0}' not found in symbol table", name ) );
					return null;
				}
			}
		}

		private Definition this[string[] names, int offset] {
			get {
				Definition topLevelDefinition = definitions[names[offset]];

				if ( offset == names.Length - 1 ) {
					return topLevelDefinition;
				}
				else {
					return topLevelDefinition.Definitions[names, offset + 1];
				}
			}
		}

		/// <summary> Adds a definition to the list. The name of the definition must be unique.
		/// No two definitions with the same name can be stored in the list.
		/// </summary>
		/// <exception cref="System.ArgumentException">thrown when name already exist</exception>
		/// <param name="definition">Definition to add.</param>

		public void Add ( Definition definition ) {
			String name = definition.Name;

			if ( definitions.ContainsKey( name ) ) {
				throw new ArgumentException( "Duplicate definition name: " + name );
			}

			definitions.Add( name, definition );
		}

		/// <summary> Gets a definition by name.
		/// </summary>
		/// <param name="name"> Name of the definition. Note that the name can be the dot
		/// separated path to any sub-definition with in the tree of definitions,
		/// e.g. def0.defsub3.defsubsub1</param>
		/// <returns></returns>

		public Definition definition ( String name ) {
			String[] names = Regex.Split( name, "\\." );

			Definition definition = this[names[0]];

			for ( int i = 1; i < names.Length && definition != null; i++ )
				definition = definition.Definitions[names[i]];


			return definition;
		}

		/// <summary> Returns a list of browsable definitions. </summary>
		/// <returns></returns>
		
		public IEnumerable<Definition> Definitions () {
			return definitions.Values;
		}

		/// <summary> Loads a list of definitions from an xml element. 
		/// This method recursivly calls the reading methods of the different definitions.
		/// </summary>
		/// <param name="element">Node of the XML the reading starts with.</param>

		public void Parse ( 
			XElement element 
		) {
			if ( element.Name != "Definitions" ) {
				throw new ArgumentException( string.Format( "Expecting Definitions but encountered {0}", element.Name ) );
			}

			foreach ( XElement child in element.Elements() ) {

				switch ( child.Name.ToString() ) {
					case "Definition":
						Definition subDefinition = new Definition();
						subDefinition.Parse( child );
						Add( subDefinition );
						break;

					case "Import":
						Add( Import.Parse( child ) );
						break;

					default:
						throw new ArgumentException( string.Format( "Invalid definition: {0}", child.Name ) );
				}
			}
		}

		/// <summary> Saves the contents of this definition list as an xml element. </summary>
		/// <returns></returns>

		public XElement ToXml () {
			XElement result = new XElement( "Definitions" );

			List<Definition> t = new List<Definition>( definitions.Values );
			t.Sort( ( Definition x, Definition y ) => { return x.Name.CompareTo( y.Name ); } );

			foreach ( var definition in t ) {
				result.Add( definition.ToXml() );
			}

			return result;
		}

		/// <summary> Gets an enumerator that traverses the encapsulated definitions. </summary>
		/// <returns></returns>

		public IEnumerator<Definition> GetEnumerator () {
			return definitions.Values.GetEnumerator();
		}

		/// <summary> Gets an enumerator that traverses the encapsulated definitions. </summary>
		/// <returns></returns>

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
			return definitions.Values.GetEnumerator();
		}
	}
}
