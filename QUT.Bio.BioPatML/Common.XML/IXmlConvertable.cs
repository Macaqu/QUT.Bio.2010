using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using QUT.Bio.BioPatML.Patterns;

namespace QUT.Bio.BioPatML.Common.XML {

	/// <summary> Interface for objects that can be saved and restored via xml elements.
	/// </summary>
    
	public interface IXmlConvertable: IXmlConvertable<Definition> {
	}
    
	/// <summary> Interface for objects that can be saved and restored via xml elements.
	/// </summary>
	/// <typeparam name="ContextDataType">
	///		Additional data needed by an object to successfully reconstitute itself or its children.
	///	</typeparam>

	public interface IXmlConvertable<ContextDataType> {
		/// <summary> Load the object from an xml element in the context of the containing definition.
		/// </summary>
		/// <param name="element">A non-null xml element presumed to contain the content of the object.</param>
		/// <param name="context">Additional data needed by an object to successfully reconstitute itself or its children.</param>

		void Parse ( XElement element, ContextDataType context );

		/// <summary> Save the contents of this object in an xml element.
		/// </summary>
		/// <returns></returns>

		XElement ToXml ();
	}
}
