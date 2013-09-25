using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;
using QUT.Bio.BioPatML.Common.XML;

/***************************************************************************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/

/*
 */

namespace QUT.Bio.BioPatML.Sequences.Annotations
{
    
    /// <summary>
    ///  This class implements an annotation to any sequences. An annotation should consist of
    ///  an item name and a value describing the item and these information are stored in 
    ///  <see cref="QUT.Bio.BioPatML.Sequences.Annotations.AnnotationList"> AnnotationList </see>.
    /// </summary>

    [Serializable]
    
	public class Annotation 
    {

        /// <summary> Name of the annotation
        /// </summary>
        
		public String Name { get; set; }

        /// <summary> Value of the annotation
        /// </summary>
        
		public Object Value { get; set; }

		/// <summary> Provided to enable deserialization.
		/// </summary>

		public Annotation () {}

        /// <summary> Creates an annotation with the given name and object value.
        /// </summary>
        /// <param name="name"> Annotation name </param>
        /// <param name="value"> Annotation value as object. </param>
        
		public Annotation(String name, Object value)
        {
            SetAnnotationAttr(name, value);
        }

        /// <summary>
        ///  Setup the name and value of this annotation
        ///  Common method for constructors
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void SetAnnotationAttr
            (String name, Object value)
        {
            Name = String.Intern(name);
            Value = value;
        }

        /// <summary>
        ///  Returns the value of an annotation as an object.
        /// </summary>
        /// <returns></returns>
        public Object ToObject()
        {
            return Value;
        }  

        /// <summary>
        /// Tests if the given object is equal to the value of the annotation.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.Value.Equals(obj);
        }

        /// <summary>
        ///  Creates a string representation of the annotation consisting of the
        ///  annotations name and its value. 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                (Name + "='" + Value + "'\n");
        }

        /// <summary> Returns the Hash code
        /// </summary>
        /// <returns>Hash code of annotation object</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

		/// <summary> Loads this Annotation from an XElement.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>

		public Annotation Parse ( XElement element ) {
			Name = element.String( "name" );
			Value = element.Value;
			return this;
		}
    }
}
