using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

/***************************************************************************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
namespace QUT.Bio.BioPatML.Symbols {
	/// <summary>
	/// The symbol class represents each character of a strand (a sequence of characters of an Alphabet).
	/// A symbol is identified by its name.
	/// <para></para>
	/// We can safely assume that two symbols are always equal if they have identical names. 
	/// In other words, symbols in different Alphabets but with equal names are both similiar symbols.
	/// <para></para>
	/// (e.g an Adenine in RNA and DNA are always the same)
	/// </summary>
#if SERIALIZATION_DEFINED
	[Serializable()]
#endif
	public class Symbol {

		/// <summary> The opposite letter of symbol is also known as complement symbol. </summary>

		public Symbol Complement { get; set; }

		/// <summary> The symbol letter
		/// </summary>

		public char Letter { get; internal set; }

		/// <summary>  Name of symbol
		/// </summary>

		public String Name { get; internal set; }

		/// <summary> The three letter code word for the symbol
		/// </summary>
		protected String Code { get; set; }

		/// <summary> Simplest form of constructing a symbol.
		/// The complement of this symbol is set to the symbol itself.
		/// A symbol is identified by its name
		/// and two symbols are equal if they have the same name.
		/// </summary>
		/// <param name="letter"> Letter of the symbol. </param>
		/// <param name="code"> The three letter code for symbol. </param>
		/// <param name="name"> Full name for this symbol. </param>

		public Symbol (
			char letter,
			String code,
			String name
		) {
			Letter = letter;
			Complement = this;
			Name = String.Intern( name );
			Code = code;
		}

        public Symbol( char letter) {
            Letter = letter;
        }

		/// <summary> Two symbols are equal if the have same name or at least on of the is 
		/// a meta symbol which matches a set of symbols. This methods expects
		/// a non-null symbol as argument.
		/// <para></para>
		/// First the name of symbol is compared and 2nd comparison is done based on its meta type
		/// </summary>
		/// <param name="symbol">symbol Symbol to compare with.</param>
		/// <returns>true: symbols are equal, false: otherwise.</returns>

		public virtual bool Equals ( Symbol symbol ) {
			return symbol.Name == this.Name ? true : symbol is SymbolMeta ? symbol.Equals( this ) : false;
		}

		/// <summary> Compares this symbol to another for equality. </summary>
		/// <param name="obj"></param>
		/// <returns></returns>

		public override bool Equals ( object obj ) {
			return Equals( (Symbol) obj );
		}

		/// <summary> Returns a string representation of this symbol
		/// </summary>
		/// <returns> Name of Symbol </returns>
		
		public override string ToString () {
			return this.Name;
		}

		/// <summary> Gets the hashcode of this symbol (from name) </summary>
		/// <returns></returns>

		public override int GetHashCode () {
			return Name.GetHashCode();
		}
	}
}
