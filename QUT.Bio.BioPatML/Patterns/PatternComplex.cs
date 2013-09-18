using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
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
	/// This abstract class provides methods for complex patterns, which are composed
	/// of multiple sub-patterns.
	/// </summary>
	public abstract class PatternComplex : Pattern {
		private PatternList patterns = new PatternList();

		/// <summary> List of the sub-pattern the complex pattern is composed of 
		/// </summary>

		public PatternList Patterns {
			get {
				return patterns;
			}
		}

		/// <summary> Parameterless constructor for deserialisation only
		/// </summary>

		public PatternComplex () { }

		/// <summary> 
		/// </summary>
		/// <param name="name">Name of the pattern.</param>

		public PatternComplex ( String name )
			: base( name ) { }

		/// <summary> Gets a pattern by index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>

		public IPattern this[int index] {
			get { return Patterns[index]; }
		}

		/// <summary>
		/// Gets a specified pattern by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>

		public IPattern this[string name] {
			get { return Patterns[name]; }
		}

		/// <summary>
		/// Returns the total number of patterns within ComplexPattern
		/// </summary>

		public int Count {
			get { return Patterns.Count; }
		}

		/// <summary> Parses a sequence of patterns and adds them to the encapsulated list.
		/// </summary>
		/// <param name="elements"></param>
		/// <param name="containingDefinition"></param>

		public void Parse (
			IEnumerable<XElement> elements,
			Definition containingDefinition
		) {
			Patterns.Parse( elements, containingDefinition );
		}

		/// <summary> Gets a reference to the first child pattern with the specified name. </summary>
		/// <param name="name">The pattern name.</param>
		/// <returns></returns>
		/// <exception cref="ChildNotFoundException">Throws a ChildNotFoundException if no child having the supplied name is found. </exception>

		public override IPattern Child ( string name ) {
			try {
				return patterns.First( childPattern => childPattern.Name == name );
			}
			catch {
				throw new ChildNotFoundException( name );
			}
		}

		private class ComplexPatternEnumerator : IEnumerable<IPattern> {
			private PatternComplex pattern;

			public ComplexPatternEnumerator ( PatternComplex pattern ) {
				this.pattern = pattern;
			}

			public IEnumerator<IPattern> GetEnumerator () {
				yield return pattern;

				foreach ( var p in pattern.Patterns ) {
					foreach ( var q in p.SelfAndChildren ) {
						yield return q;
					}
				}
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
				yield return pattern;

				foreach ( var p in pattern.Patterns ) {
					foreach ( var q in p.SelfAndChildren ) {
						yield return q;
					}
				}
			}
		}

		/// <summary> Gets a list of patterns consisting of this and its children. </summary>

		public override IEnumerable<IPattern> SelfAndChildren {
			get {
				return new ComplexPatternEnumerator( this );
			}
		}

	}
}
