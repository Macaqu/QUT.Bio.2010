using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

/***************************************************************************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
namespace QUT.Bio.BioPatML.Sequences.Annotations {
	/// <summary>
	/// 
	///  This class implements a list of <see cref="QUT.Bio.BioPatML.Sequences.Annotations.Annotation">Annotation</see>. 
	///  Annotation lists are
	///  usually attached to a <see cref="QUT.Bio.BioPatML.Sequences.Sequence">Sequence </see> to describe properties of a
	///  sequence such as name, or id.
	/// <para></para> 
	/// An annotation list can contain multiple
	///  annotation with the same name. 
	///  Annotation list are implemented as simple array lists. It is quite efficient
	///  as long as the number of annotations is small and the annotations are
	///  retrieved by constant/internally pooled string names (see String.intern(),
	///  String.equals() and Annotation(String, Object)).
	/// </summary>

	[Serializable]

	public class AnnotationList : IEnumerable<Annotation> {
		/// <summary> List containning annotations </summary>

		private readonly List<Annotation> annotations = new List<Annotation>();

		/// <summary>
		///  Adds an annotation to the list.
		/// </summary>
		/// <param name="annotation"> Annotation to add. </param>
		/// <see> Annotation </see>
		public void Add ( Annotation annotation ) {
			annotations.Add( annotation );
		}

		/// <summary> Appends a list of annotations to this list. </summary>
		/// <param name="annotations"></param>

		public void AddRange ( IEnumerable<Annotation> annotations ) {
			this.annotations.AddRange( annotations );
		}

		/// <summary> Gets the number of items in the list </summary>

		public int Count {
			get {
				return annotations.Count;
			}
		}

		/// <summary> Getter for an annotation within the list.
		/// </summary>
		/// <param name="index"> Index of the annotation to retrieve. </param>
		/// <returns> Returns the annotation for the given index. </returns>
		public Annotation this[int index] {
			get {
				return annotations[index];
			}
		}

		/// <summary>
		/// Getter for an annotation within the list. If there are more than one
		/// annotations with the same name only the first occurence will be returned
		/// </summary>
		/// <param name="name">
		///  Name of the annotation to retrieve.
		/// </param>
		/// <returns> Returns the annotation for the given name or null if no 
		/// annotation with this name exists.
		/// </returns>
		
		public Annotation this[string name] {
			get {
				return annotations.FirstOrDefault( a => a.Name == name );
			}
		}

		/// <summary> Gets the value of the child annotation having the specified name, 
		///		or null if no annotation having the specified name is present.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>

		public string Value( string name ) {
			Annotation annotation = this[name];
			return annotation == null ? null : annotation.Value.ToString();
		}

		/// <summary>
		///  Get all annotations with a given name.
		/// </summary>
		/// <param name="name"> Name of the annotations to retrieve. </param>
		/// <returns> 
		/// Returns a of list annotations that matches the
		/// specified name. 
		/// </returns>

		public AnnotationList GetAll ( String name ) {
			AnnotationList result = new AnnotationList();
			result.AddRange( annotations.Where( a => a.Name == name ) );
			return result;
		}

		/// <summary>
		/// Gets an annotation which name matches the given regular expression.
		/// </summary>
		/// <param name="regExp"> 
		/// Regular expression, <see>Pattern package</see>
		/// </param>
		/// <returns>
		/// Returns the first (not all) annotation that name matches the
		/// regular expression or null if no such annotation exists.
		/// </returns>
		
		public Annotation this[Regex regExp] {
			get {
				return annotations.FirstOrDefault( a => regExp.IsMatch( a.Name ) );
			}

		}

		/// <summary>
		/// Gets all annotations which name matches the given regular expression.
		/// </summary>
		/// <param name="regExp">
		/// Regular expression, <see> Pattern package </see>
		/// </param>
		/// <returns> 
		/// Returns an annotation list with all annotations matching the given name.
		/// </returns>
		public AnnotationList GetAll ( Regex regExp ) {
			AnnotationList aList = new AnnotationList();

			foreach ( Annotation a in annotations )

				if ( regExp.IsMatch( a.Name ) )
					aList.Add( a );


			return ( aList );
		}

		/// <summary>
		/// Gets the number of annotation that has the param name.
		/// </summary>
		/// <param name="name"> Name of the annotation. </param>
		/// <returns> 
		/// Returns the number of annotations matching the
		/// given name.
		/// </returns>
		public int CountBy ( String name ) {
			int number = 0;

			foreach ( Annotation a in annotations )

				if ( a.Name.Equals( name ) )
					number++;

			return number;
		}

		/// <summary>
		/// Gets the number of annotation that has the matching param name and value.
		/// </summary>
		/// <param name="name"> Name of the annotation. </param>
		/// <param name="value"> Value of the annotation. </param>
		/// <returns> Returns the number of annotations with the
		/// given name and value. </returns>
		public int CountBy ( String name, Object value ) {
			int number = 0;

			foreach ( Annotation a in annotations )

				if ( a.Name.Equals( name ) &&
					a.Value.Equals( value ) )

					number++;

			return ( number );
		}

		/// <summary>
		/// Test if there is an annotation with the given name and value contained
		/// in the annotation list.
		/// </summary>
		/// <param name="name"> Name of the annotation. </param>
		/// <param name="value"> Value of the annotation. </param>
		/// <returns>
		/// true: if the annotation with the given name and value is 
		/// contained, false: otherwise.
		///</returns>
		
		public bool Contains ( String name, Object value ) {
			AnnotationList aList = GetAll( name );

			for ( int i = 0; i < aList.Count; i++ )
				if ( aList[i].Equals( value ) )
					return true;


			return false;
		}

		/// <summary> Creates a string representation of an annotation list. </summary>
		/// <returns></returns>
		
		public override string ToString () {
			return ToXml().ToString();
		}

		/// <summary> Loads a list of annotations from a list of xml elements. </summary>
		/// <param name="elements"></param>

		public void Parse ( IEnumerable<XElement> elements ) {
			foreach ( var element in elements ) {
				Add( ( new Annotation() ).Parse( element ) );
			}
		}

		/// <summary> Get an enumerator. </summary>
		/// <returns></returns>
		
		public IEnumerator<Annotation> GetEnumerator () {
			return annotations.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
			return annotations.GetEnumerator();
		}

		/// <summary> Gets an xml element that represents this list of annotations. </summary>
		/// <returns></returns>

		public XElement ToXml () {
			XElement result = new XElement( "Annotations" );

			foreach ( var annotation in annotations ) {
				result.Add( new XElement( "Annotaion", 
					new XAttribute("name", annotation.Name ), 
					new XAttribute("value", annotation.Value ) 
				) );
			}

			return result;
		}
	}
}
