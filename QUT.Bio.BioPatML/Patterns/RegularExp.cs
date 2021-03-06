﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using QUT.Bio.BioPatML.Common.Structures;
using QUT.Bio.BioPatML.Common.XML;
using QUT.Bio.BioPatML.Sequences;
using Bio;

/*****************| Queensland University Of Technology |********************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/

/*
	<xsd:complexType name="Regex">
		<xsd:attribute name="name"      type="xsd:string"                      />
		<xsd:attribute name="case"      type="case"       default="INSENSITIVE"/>
		<xsd:attribute name="regex"     type="xsd:string" use="required"/>
		<xsd:attribute name="impact"    type="impact"     default="1.0"/>
	</xsd:complexType>
 */

namespace QUT.Bio.BioPatML.Patterns {
	/// <summary>
	/// This class implements regular expressions. The implementation is not very<para></para>
	/// efficient but should be good enough for most purposes. Let's wait until<para></para>
	/// someone yells.<para></para>
	/// See http://java.sun.com/j2se/1.5.0/docs/api/ for a description<para></para>
	/// of regular expressions. In the following an excerpt of the Java API <para></para>
	/// description for regular expressions.<para></para>
	/// <para></para>
	/// Character classes  <para></para>
	/// [abc] 					a, b, or c (simple class) <para></para>
	/// [^abc] 				Any character except a, b, or c (negation) <para></para>
	/// [a-zA-Z]				a through z or A through Z, inclusive (range) <para></para>
	/// [a-d[m-p]] 		a through d, or m through p: [a-dm-p] (union) <para></para>
	/// [a-z&amp;&amp;[def]] 	d, e, or f (intersection) <para></para>
	/// [a-z&amp;&amp;[^bc]] 	a through z, except for b and c: [ad-z] (subtraction) <para></para>
	/// [a-z&amp;&amp;[^m-p]] 	a through z, and not m through p: [a-lq-z](subtraction) <para></para>
	///<para></para>
	/// Special characters<para></para>
	/// ^ 		The beginning of a sequence <para></para>
	/// $ 		The end of a sequence<para></para>
	/// . 		Any character  <para></para>
	///<para></para>
	/// Greedy quantifiers <para></para>
	/// X? 			X, once or not at all <para></para>
	/// X* 			X, zero or more times <para></para>
	/// X+ 			X, one or more times <para></para>
	/// X{n} 		X, exactly n times <para></para>
	/// X{n,} 		X, at least n times <para></para>
	/// X{n,m} 	X, at least n but not more than m times <para></para>
	/// 
	/// </summary>
	public class RegularExp : Pattern {

		/** Enumerated type which defines the valid case sensitivity values.*/

		public enum CaseSensitivity {
			/** Not case sensitive. */
			INSENSITIVE,

			/** Case sensitive. */
			SENSITIVE
		}

		private CaseSensitivity caseSensitivity;

		/// <summary> The regular expression string pattern value for searching within this pattern
		/// </summary>

		public String RegularEx { get; protected set; }

		/// <summary>
		/// Stating whether should the search be case sensitive
		/// </summary>
		public bool IsCaseSensitive {
			get {
				return caseSensitivity == CaseSensitivity.SENSITIVE;
			}
			set {
				caseSensitivity = value ? CaseSensitivity.SENSITIVE : CaseSensitivity.INSENSITIVE;
			}
		}

		/// <summary>
		/// This value is used during matching. For determining the next startPos of match.
		/// </summary>
		public new int Increment { get; protected set; }

		#region -- Constructors --

		/// <summary> Internal constructor for deserialization.
		/// </summary>

		public RegularExp () { }

		/// <summary>
		/// Creates an regular expression. Please note, that this pattern will match 
		/// only once for a sequence position, even if more matches are possible. 
		/// Typically, regular expression (except otherwise constructed) and will
		/// match with the longest possible part of the sequence.
		/// </summary>
		/// <param name="name">Name for regular expression element</param>
		/// <param name="regex">Regular expression.</param>
		/// <param name="isCaseSensitive">
		/// True: matching is case sensitive, false: matching
		/// is case insensitive.
		/// </param>

		public RegularExp ( String name, String regex, bool isCaseSensitive )
			: base( name ) {
			IsCaseSensitive = isCaseSensitive;
			Init( regex );
		}

		/// <summary>
		/// Same as default constructor (String, bool) however bool in this case is set to false
		/// </summary>
		/// <param name="name">Name of regular expression pattern element</param>
		/// <param name="regex"></param>
		public RegularExp ( String name, String regex )
			: base( name ) {
			this.Init( regex );
		}

		/// <summary>
		/// This is the core of the constructor which is also used by the
		/// ReadNode() method. See the class desc for a detailed description 
		/// of the parameters. 
		/// </summary>
		/// <param name="regex"></param>
		
		protected void Init ( String regex ) {
			RegularEx = regex;
			Threshold = 1.0;
			Increment = 1;
			//this.match = Regex.Match(regex, 
		}


		/// <summary> The implementation ensures that
		/// a match fails for a given position if there is no match. Otherwise the
		/// matcher might return a match at a different position.
		/// <see cref="QUT.Bio.BioPatML.Patterns.IPattern">IPattern Match(Sequence, int) method</see>
		/// </summary>
		/// <param name="sequence"> the sequence for matching</param>
		/// <param name="position"> position used for matching</param>
		/// <returns></returns>

		public override Match Match (
			ISequence sequence,
			int position
		) {
			String charSequence = sequence.GetSubSequence((long) position, (long)(sequence.Count - position) ).ToString();
			int startIndex = position - 1;
			//String charSequence = sequence.Letters();

			System.Text.RegularExpressions.Match myMatch;

			if ( caseSensitivity != CaseSensitivity.SENSITIVE ) {
				myMatch = Regex.Match( charSequence, RegularEx, RegexOptions.IgnoreCase );
			}
			else {
				myMatch = Regex.Match( charSequence, RegularEx );
			}


			if ( myMatch.Success ) {

				//if ((myMatch.Index + (position - 1)) //Minus away the infront seq's position -1 for the start index 0
				//                      != position - 1)
				int foundIndex = myMatch.Index + startIndex;

				if ( ( foundIndex ) != position - 1 )
				//if((myMatch.Index) != position - 1)
                {

					//Increment = myMatch.Index + 1 - position;
					Increment = foundIndex + 1 - position;
					return ( null );
				}

				Increment = 1;
				LatestMatch.Set( sequence, foundIndex + 1,
						  myMatch.Length, Strand.Forward, 1.0 );

				//GetMatch().Set(sequence, myMatch.Index + 1,
				//          myMatch.Length, sequence.Strand, 1.0);

				return ( LatestMatch );

			}

			Increment = Math.Max( 1, (int)sequence.Count - position );
			return null;
		}

		/// <summary>
		/// Returns the regular expression string. 
		/// </summary>
		/// <returns></returns>
		public override string ToString () {
			return ( "Regex: '" + Name + "'=" + RegularEx );
		}

		#endregion

		/// <summary> Loads the content of this RegularExp from an Xml element.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="definition"></param>

		public override void Parse (
			XElement element,
			Definition definition
		) {
			IsCaseSensitive = false;
			Name = element.String( "name" );
			Impact = element.Double( "impact", 1.0 );
			Enum.TryParse<CaseSensitivity>( element.String( "case" ), out caseSensitivity );
			Init( element.String( "regex" ) );
		}

		/// <summary> Create an xml element that represents this regular expression.
		/// </summary>
		/// <returns></returns>

		public override XElement ToXml () {
			return new XElement( "Regex",
				new XAttribute( "name", Name ),
				new XAttribute( "case", caseSensitivity ),
				new XAttribute( "regex", RegularEx ),
				new XAttribute( "impact", Impact )
			);
		}
	}
}
