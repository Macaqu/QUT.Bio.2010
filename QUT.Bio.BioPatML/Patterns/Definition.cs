using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using QUT.Bio.BioPatML.Common.XML;
using System.IO;
using QUT.Bio.BioPatML.Sequences.Annotations;

/*****************| Queensland University Of Technology |********************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/

/*
  <xsd:complexType  name="Definition">
    <xsd:sequence>
      <xsd:element name="Parameters"  type="Parameters"  minOccurs="0" maxOccurs="1"/>
      <xsd:element name="Annotations" type="Annotations" minOccurs="0" maxOccurs="1"/>
      <xsd:element name="Definitions" type="Definitions" minOccurs="0" maxOccurs="1"/>
      <xsd:group ref="Pattern" minOccurs="1" maxOccurs="1"/>
    </xsd:sequence>
    <xsd:attribute name="name" type="xsd:string" />
  </xsd:complexType>
 */

namespace QUT.Bio.BioPatML.Patterns
{
    /// <summary>
    /// This class describes a pattern definition. A pattern definition is composed
    /// of a parameter list, a list of annotations, a list of other pattern 
    /// definition and the actual pattern description. 
    /// </summary>
    public class Definition
    {
        /// <summary> Name of the definition
        /// </summary>

        public string Name
        {
            get
            {
                return name;
            }
            private set
            {
                AutoName.SetName(ref name, value);
            }
        }

        private string name;

        /// <summary> The annoations associated withg this definition.
        /// </summary>

        public AnnotationList Annotations
        {
            get { return annotations; }
        }

        private AnnotationList annotations = new AnnotationList();

        /// <summary> List of patterns within the definition tag
        /// </summary>

        private IPattern pattern;

        /// <summary> List of sub-definitions. 
        /// </summary>

        public DefinitionList Definitions { get { return definitions; } }

        private DefinitionList definitions = new DefinitionList();

        /// <summary> Gets the parameter list for this definition.
        /// </summary>

        public ParameterList Parameters
        {
            get
            {
                return parameters;
            }
        }

        private ParameterList parameters = new ParameterList();

        /// <summary> Constructs an empty definition with an unique default name 
        /// </summary>

        public Definition()
        {
            Name = string.Empty;
        }

        /// <summary>
        /// Constructs an empty pattern definition with a given name. 
        /// </summary>
        /// <param name="name">Name of the definition pattern.</param>
        public Definition(string name)
        {
            Name = name;
        }

        /// <summary> Construct a definition pattern with a desired name and also 
        /// a main pattern.
        /// <para></para>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pattern">The main pattern.</param>

        public Definition(string name, IPattern pattern)
        {
            Name = name;
            this.pattern = pattern;
        }

        /// <summary> Gets the main pattern within this Definition (the first element in definition)
        /// Sets the main pattern within the definition.
        /// <para></para>
        /// Note that this
        /// methods replaces the pattern list of the definition by all patterns and
        /// sub-patterns of the given pattern.
        /// </summary>

        public IPattern Pattern
        {
            get
            {
                return pattern;
            }
            set
            {
                pattern = value;
            }
        }

        /// <summary> Gets the sub definition by a given name.
        /// </summary>
        /// <param name="name">The name of sub definition within this definition.</param>
        /// <returns>Sub definition</returns>

        public Definition SubDefinition(String name)
        {
            return Definitions.definition(name);
        }

        /// <summary> A static method for reading the definition of a pattern.
        /// </summary>
        /// <param name="element">The definition node with the starting tag of Definition</param>
        /// <returns></returns>

        public void Parse(
            XElement element
        )
        {
            if (element.Name != "Definition")
            {
                throw new ArgumentException(string.Format("Expecting Definition but encountered {0}", element.Name));
            }

            Name = element.String("name");

            foreach (XElement child in element.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "Annotations":
                        annotations.Parse(child.Elements());
                        break;

                    case "Parameters":
                        parameters.Parse(child.Elements());
                        break;

                    case "Definitions":
                        definitions.Parse(child);
                        break;

                    default:
                        if (pattern != null)
                        {
                            throw new ArgumentException("A definition may only contain one top-level pattern.");
                        }

                        pattern = QUT.Bio.BioPatML.Patterns.Pattern.CreateFrom(child);
                        pattern.Parse(child, this);
                        break;
                }
            }
        }

        /*
          <xsd:complexType  name="Definition">
            <xsd:sequence>
              <xsd:element name="Parameters"  type="Parameters"  minOccurs="0" maxOccurs="1"/>
              <xsd:element name="Annotations" type="Annotations" minOccurs="0" maxOccurs="1"/>
              <xsd:element name="Definitions" type="Definitions" minOccurs="0" maxOccurs="1"/>
              <xsd:group ref="Pattern" minOccurs="1" maxOccurs="1"/>
            </xsd:sequence>
            <xsd:attribute name="name" type="xsd:string" />
          </xsd:complexType>
         */

        /// <summary> Saves the contents of this object in an xml element.
        /// </summary>
        /// <returns>An xml element containign the content of this object.</returns>

        public virtual XElement ToXml()
        {
            XElement result = new XElement("Definition");

            if (!AutoName.IsAnonymous(name))
            {
                result.Add(new XAttribute("name", name));
            }

            if (annotations.Count > 0)
            {
                result.Add(annotations.ToXml());
            }

            if (parameters.Count > 0)
            {
                result.Add(parameters.ToXml());
            }

            if (definitions.Count > 0)
            {
                result.Add(definitions.ToXml());
            }

            if (pattern != null)
            {
                result.Add(pattern.ToXml());
            }

            return result;
        }

        /// <summary> Copies the nested patterns into a newly created array and returns
        /// the resulting collection.
        /// </summary>

        public IPattern[] Patterns
        {
            get
            {
                List<IPattern> patterns = new List<IPattern>();

                if (pattern != null) patterns.AddRange(pattern.SelfAndChildren);

                return patterns.ToArray();
            }
        }
    }
}
