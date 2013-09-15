using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QUT.Bio.BioPatML.Sequences.Annotations;

/***************************************************************************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
namespace QUT.Bio.BioPatML.Sequences.List {
	/// <summary>
	/// This class is a base class for lists of regions, sequences, features and
	/// other objects if required.
	/// 
	/// It appears to be a list of objects, which is itself associated with a list of annotations.
	/// </summary>
	public class AnnotatedList<T> : List<T>, IAnnotated where T : IAnnotated {
		/// <summary>
		/// List of annotations 
		/// </summary>
		private readonly AnnotationList annotations = new AnnotationList();

		/// <summary>
		///  Creates an empty object list.
		/// </summary>
		public AnnotatedList () { }

		/// <summary>
		///  Creates an object list with the given name. The name will be stored
		///  as a String under the tag "Name" in the annotation list of the object list. 
		///  This is just a conveniency method.
		/// </summary>
		/// <param name="name">Name of the object list. 
		/// </param>

		public AnnotatedList ( String name ) {
			if ( name != null )
				Annotations.Add( new Annotation( "Name", name ) );
		}

		/// <summary>
		/// Gets the name of annotationList
		/// </summary>
		/// <returns> string representation of the object list name list </returns>

		public String Name {
			get {
				Annotation annotation = Annotations["Name"];
				return annotation == null ? null : annotation.Value.ToString();
			}
		}

		/// <summary>
		///  Setter method for our list of elements. This setter is cyclic.
		/// </summary>
		/// <param name="index"> Index of the element to set. Indicies out of the interval [0, size-1] 
		/// are wrapped to valid indices making the list cyclic.</param>
		/// <param name="annotated"> An annotated object.</param>
		public void Set ( int index, T annotated ) {
			index %= Count;

			if ( index < 0 )
				index = Count + index;

			base[index] = annotated; //Replace the index element with the specified annotated Interface
		}


		/// <summary>
		///  Gets an IAnnotated element by the given name.
		/// </summary>
		/// <param name="name"> Name of element.</param>
		/// <returns>Returns the first list element with the given name or null 
		/// if no such element exists.</returns>

		public T this[string name] {
			get {
				return GetFirst( "Name", name );
			}
		}

		/// <summary>
		///  Gets an list element which has an annotation variable of the given 
		///  name and value.
		/// </summary>
		/// <param name="annotationName">  Name of the annotation variable, e.g. AccessionNumber </param>
		/// <param name="annotationValue"> Value of the annotation, e.g. the accession number </param>
		/// <returns> Returns the first list element with the specified annotation or 
		///  null if no such element exists.
		///  </returns>
		public T GetFirst (
			String annotationName,
			String annotationValue
		) {
			return this.FirstOrDefault( element => {
				Annotation annotation = element.Annotations[annotationName];
				return annotation != null && annotation.Value != null &&
					annotation.Value.Equals( annotationValue );
			} );
		}

		/// <summary>
		///  Gets a list of elements which have an annotation variable of the given 
		///  name and value.
		/// </summary>
		/// <param name="annotationName"> Name of the annotation variable, e.g. AccessionNumber </param>
		/// <param name="annotationValue"> Value of the annotation.</param>
		/// <returns> Returns a list with elements that have the required annotation. </returns>
		public AnnotatedList<T> Get (
			String annotationName,
			String annotationValue
		) {
			AnnotatedList<T> list = new AnnotatedList<T>( null );

			list.AddRange( this.Where( element => {
				Annotation annotation = element.Annotations[annotationName];
				return annotation != null && annotation.Value != null &&
					annotation.Value.Equals( annotationValue );
			} ) );

			return list;
		}

		/// <summary>
		/// Gets a of list filled with elements which have an annotation variable of the given 
		/// name and value contained in the provide list of annotation values.
		/// </summary>
		/// <param name="annotationName"> Name of the annotation variable, e.g. subcellular localization </param>
		/// <param name="annotationValues"> Array of annotation values </param>
		/// <returns> Returns a list with elements that have annotation variables
		/// with values contained in the annotation values list. 
		/// </returns>
		public AnnotatedList<T> Get ( String annotationName, IEnumerable<string> annotationValues ) {
			AnnotatedList<T> list = new AnnotatedList<T>( null );

			list.AddRange( this.Where( element => {
				Annotation annotation = element.Annotations[annotationName];
				return annotation != null && annotation.Value != null &&
					annotationValues.FirstOrDefault( a => a.Equals( annotation.Value ) ) != null;
			} ) );

			return ( list );
		}

		/// <summary>
		///  Getter for a list element. This getter is cyclic. 
		/// </summary>
		/// <param name="index">
		///  Index of the element to get. Indicies out of the interval [0, size-1]
		///  are wrapped to valid indices making the list cyclic.</param>
		/// <returns> Return the specified list element. </returns>

		public new T this[int index] {
			get {
				index %= Count;

				if ( index < 0 )
					index = Count + index;

				return base[index];
			}
			set {
				index %= Count;

				if ( index < 0 )
					index = Count + index;

				base[index] = value;
			}
		}

		/// <summary> Query method if the list has annotations or not. 
		/// </summary>
		/// <returns>Returns true if the list has at least one annotation and
		/// false otherwise.</returns>

		public bool HasAnnotations {
			get {
				return annotations.Count > 0;
			}
		}

		/// <summary> Gets the list of annotations attached to the list. As soon as
		///  this method is called an empty annotation list will be attached to the
		///  list if none is existing before.
		/// </summary>
		/// <returns> Returns the list of annotations attached to the list. </returns>

		public AnnotationList Annotations {
			get { return annotations; }
		}

		/// <summary> Adds a list of annotations to the already existing annotations of the
		///  object list. 
		/// </summary>
		/// <param name="annotationList"> List of annotations to add. </param>

		public void AddAnnotations ( AnnotationList annotationList ) {
			annotations.AddRange( annotationList );
		}

		/// <summary> Appends a list to the list. All elements of the given list are appended to
		/// the list.
		/// </summary>
		/// <param name="list"> List to add. </param>
		
		public void Append ( AnnotatedList<T> list ) {
			AddRange( list );
		}

		/// <summary> Creates a list of annotated lists which are a split of this list according
		///  to the values of the specified annotation. This means each list of the
		///  split contains only elements which annotations where the specified
		///  annotation name has the same value. 
		///  * Note that the source list is not changed.
		/// </summary>
		/// <param name="annotationName"> Name of the annotation which is used for the splitting. </param>
		/// <returns> Returns an annotated list of annotated lists. The name of the lists is the
		/// annotation value for this list.</returns>

		public AnnotatedList<AnnotatedList<T>> Split ( String annotationName ) {
			AnnotatedList<AnnotatedList<T>> lists = new AnnotatedList<AnnotatedList<T>>();

			foreach ( T element in this ) {
				if ( element.HasAnnotations ) {
					Annotation annotation = element.Annotations[annotationName];
					String name = annotation == null ? null : annotation.Value.ToString();

					if ( name != null ) {
						AnnotatedList<T> list = lists[name];

						if ( list == null ) {
							list = new AnnotatedList<T>( name );
							lists.Add( list );
						}

						list.Add( element );
					}
				}
			}

			return ( lists );
		}
	}
}
