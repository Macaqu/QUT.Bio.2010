using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QUT.Bio.BioPatML.Sequences {

	/// <summary> Arithmetic operations for Strands.
	/// </summary>

	public sealed class Strand {
		private int value;

		/** <summary>Strand unknown.</summary> */
		public static readonly Strand Unknown = null;

		/** <summary>Forward strand.</summary> */
		public static readonly Strand Forward = new Strand { value = 1 };
		
		/** <summary>Reverse strand.</summary> */
		public static readonly Strand Reverse = new Strand { value = -1 };

		/// <summary> Make constructor private to prohibit instantiation of this type.
		/// </summary>

		private Strand () {
		}

		/// <summary> Allows shorthand use of multiplication operator to traverse along a strand.
		/// </summary>

		public static int operator * ( Strand strand, int distance ) {
			return strand.value * distance;
		}

		/// <summary> Allows shorthand use of multiplication operator to traverse along a strand.
		/// </summary>

		public static int operator * ( int distance, Strand strand ) {
			return strand.value * distance;
		}

		/// <summary> Convert a string value into a Strand.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>

		public static Strand Parse( string s ) {
			return s == "Reverse" ? Reverse :
				   s == "Forward" ? Forward :
				                    Unknown;                      
		}

		/// <summary> Gets a string representation of a strand.
		/// </summary>
		/// <returns></returns>

		public override string ToString () {
			return this == Unknown ? "Unknown" :
			       this == Forward ? "Forward" :
				                     "Reverse"; 
		}
	}
}
