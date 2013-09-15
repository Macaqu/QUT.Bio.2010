using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace QUT.Bio.BioPatML.Common.XML {

	/// <summary> An interface promising the ability to convert items to Xml.
	/// </summary>
	/// <returns></returns>

	public interface IToXml {

		/// <summary> Save the contents of this object in an xml element.
		/// </summary>
		/// <returns></returns>
		
		XElement ToXml ();
	}
}
