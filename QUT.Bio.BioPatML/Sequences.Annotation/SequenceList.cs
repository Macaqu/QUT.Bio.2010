using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QUT.Bio.BioPatML.Sequences.Annotations;
using Bio;

/***************************************************************************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrance BuckingHam
 * 
 ***************************************************************************/
namespace QUT.Bio.BioPatML.Sequences.List {
	/// <summary> This class describes a list of sequences. </summary>
	public class SequenceList : RegionList<ISequence> {

		/// <summary> Creates an empty sequence list. </summary>

		public SequenceList ()
			: base() { /* No implementation */ }

		/// <summary>
		///  Creates an empty sequence list with the given name.
		/// </summary>
		/// <param name="name"></param>

		public SequenceList ( String name )
			: base( name ) { /* No implementation */ }

		
	}
}
