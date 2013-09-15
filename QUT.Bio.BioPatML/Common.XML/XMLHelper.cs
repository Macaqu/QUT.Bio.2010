using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.IO;
using System.Collections;

/***************************************************************************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
namespace QUT.Bio.BioPatML.Common.XML {
	/// <summary>
	/// This class implements some helper functions for XML.
	/// </summary>
	public static class XMLHelper {

		/// <summary>
		/// Getter for a specific text content of the given node.
		/// </summary>
		/// <param name="element">The desired node</param>
		/// <param name="name">name of the node to look out for</param>
		/// <returns></returns>
		
		static public String ChildContent ( 
			this XElement element, 
			String name 
		) {
			XElement child = element.Element( name );
			return child == null ? null : child.Value;
		}

		/// <summary>
		/// Getter for the attribute value of an attribute of the given name
		/// for the specified node. The attribute value is returned as a string.
		/// </summary>
		/// <param name="element"> Node </param>
		/// <param name="name" > Name of the attribute. </param>
		/// <returns>
		/// Returns the attribute value as string or null if the attribute
		/// does not exist.
		/// </returns>
		
		static public String String ( 
			this XElement element, 
			String name 
		) {
			XAttribute attr = element.Attribute( name );
			return attr == null ? null : attr.Value;
		}

		/// <summary> Gets an enumerated value of the specified type from an attribute 
		/// of the supplied xml element.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="element"></param>
		/// <param name="name"></param>
		/// <returns></returns>

		static public T EnumValue<T> (
			this XElement element,
			string name
		)
		where T : struct {
			return (T) Enum.Parse( typeof(T), element.String( name ), false );
		}

		/// <summary>
		///    Getter for the attribute value of an attribute of the given name
		///    for the specified node. The attribute value is returned as a double.
		/// </summary>
		/// <param name="element"> Node </param>
		/// <param name="name"> Name of the attribute. </param>
		/// <param name="defaultValue"> The value to use if no attribute having the specified name is present. </param>
		/// <returns>
		///    Returns the attribute value as a double value or 0.0 if
		///    the attribute does not exist. However in some special cases
		///    we return it as 1.0.
		/// </returns>
		
		static public Double Double (
			this XElement element,
			String name,
			double defaultValue = 0
		) {
			String value = element.String( name );

			if ( value == null ) {
				return defaultValue;
			}

			return Convert.ToDouble( value );
		}

		/// <summary>
		/// Getter for the attribute value of an attribute of the given name
		/// for the specified node. The attribute value is returned as a integer.
		/// </summary>
		/// <param name="node"> Node</param>
		/// <param name="name"> Name of the attribute. </param>
		/// <returns>
		/// Returns the attribute value as an integer or 0 if the
		/// attribute does not exist.
		/// </returns>
		static public int Int ( this XElement node, String name ) {
			String value = String( node, name );
			return Convert.ToInt32( value );
		}
	}
}
