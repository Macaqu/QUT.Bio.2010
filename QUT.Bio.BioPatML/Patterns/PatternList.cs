using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml.Linq;

/*****************| Queensland University Of Technology |********************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
namespace QUT.Bio.BioPatML.Patterns {
	/// <summary>
	/// Implements a list of patterns that can be addressed by name as well.
	/// Pattern lists are used by complex patterns.
	/// </summary>
	public partial class PatternList : ICollection<IPattern>, IEnumerable<IPattern> {
		/// <summary> List of patterns
		/// </summary>
		private List<IPattern> list = new List<IPattern>();

		/// <summary> Maps pattern names to patterns
		/// </summary>
		private Dictionary<string, IPattern> dictionary = new Dictionary<string, IPattern>();

		/// <summary> Adds a pattern to the list. The name of the pattern must be unique.
		/// No two patterns with the same name can be stored in the list.
		/// </summary>
		/// <param name="item">Pattern to add.</param>
		/// <exception cref="System.ArgumentException">Thrown when duplicate name pattern was found</exception>

		public void Add ( IPattern item ) {
			Add( list.Count, item.Name, item );
		}

		#region ICollection<IPattern> members

		/// <summary>
		/// Adds a pattern to the list. The name of the pattern must be unique.
		/// No two patterns with the same name can be stored in the list.
		/// </summary>
		/// <param name="index">Index position where the pattern is added to the list.</param>
		/// <param name="pattern">Pattern to add.</param>
		/// <exception cref="System.ArgumentException">Thrown when duplicate name pattern was found</exception>

		public void Add ( int index, IPattern pattern ) {
			Add( index, pattern.Name, pattern );
		}

		/// <summary>
		/// Adds a pattern to the list. The name of the pattern must be unique.
		/// No two patterns with the same name can be stored in the list.
		/// 
		/// </summary>
		/// <param name="index">Index position where the pattern is added to the list.</param>
		/// <param name="name">Name of the pattern.</param>
		/// <param name="pattern">Pattern to add.</param>
		/// <exception cref="System.ArgumentException">Thrown when duplicate name pattern was found</exception>

		private void Add ( int index, string name, IPattern pattern ) {
			if ( dictionary.ContainsKey( name ) )
				throw new ArgumentException( "Duplicate pattern name: " + name );

			dictionary.Add( name, pattern );
			list.Insert( index, pattern );
		}

		/// <summary>
		/// Retrieves the pattern using a key that is mapped to the pattern
		/// </summary>
		/// <param name="key">Key mapped to the pattern</param>
		/// <returns>The desire pattern object</returns>
		public IPattern this[string key] {
			get { return dictionary[key]; }
		}

		/// <summary>
		/// Gets the pattern by the specified index
		/// </summary>
		/// <param name="index">location of pattern</param>
		/// <returns>The desired pattern object</returns>
		public IPattern this[int index] {
			get { return list[index]; }
		}

		/// <summary>
		/// Clear our dictionary and list.
		/// </summary>
		public void Clear () {
			list = new List<IPattern>();
			dictionary = new Dictionary<string, IPattern>();
		}

		/// <summary>
		/// Returns the number of patterns in this list
		/// </summary>
		public int Count {
			get { return list.Count; }
		}

		#endregion

		#region -- Not implemented methods --
		/// <summary>
		/// No implementation
		/// </summary>
		/// <exception cref="System.NotImplementedException">When called</exception>
		public bool IsReadOnly {
			get { throw new NotImplementedException(); }
		}
		/// <summary>
		/// No implementation
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException">When called</exception>
		public bool Remove ( IPattern item ) {
			throw new NotImplementedException();
		}
		/// <summary>
		/// No implementation
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException">When called</exception>
		public bool Contains ( IPattern item ) {
			throw new NotImplementedException();
		}
		/// <summary>
		/// No implementation
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		/// <exception cref="System.NotImplementedException">When called</exception>
		public void CopyTo ( IPattern[] array, int arrayIndex ) {
			throw new NotImplementedException();
		}

		#endregion


		/// <summary> Parses a sequence of patterns and adds them to this list.
		/// </summary>
		/// <param name="elements">A list of xml elements which should contain nested pattern definitions.</param>
		/// <param name="containingDefinition">The definition that most immediately contains the present patter list.</param>

		public void Parse ( 
			IEnumerable<XElement> elements, 
			Definition containingDefinition 
		) {
			foreach ( XElement element in elements ) {
				IPattern p = Pattern.CreateFrom( element );
				
				if ( p != null ) {
					p.Parse( element, containingDefinition );
					Add( p );
				}
			}
		}

		/// <summary> Gets an XML Representation of the present pattern list.
		/// </summary>
		/// <returns></returns>

		public IEnumerable<XElement> ToXml() {
			return list.Select( p => p.ToXml() );
		}

        public IEnumerator<IPattern> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<IPattern> IEnumerable<IPattern>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
