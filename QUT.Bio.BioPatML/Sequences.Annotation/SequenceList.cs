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
	/// <summary> This class describes a list of sequences. </summary>
	public class SequenceList : RegionList<Sequence> {

		/// <summary> Creates an empty sequence list. </summary>

		public SequenceList ()
			: base() { /* No implementation */ }

		/// <summary>
		///  Creates an empty sequence list with the given name.
		/// </summary>
		/// <param name="name"></param>

		public SequenceList ( String name )
			: base( name ) { /* No implementation */ }

		/// <summary>
		/// Creates a feature list with all features of the sequences of the list
		/// which match the given feature name .
		/// </summary>
		/// <param name="featureListName">
		/// Name of the feature lists which contain the features to extract.
		/// </param>
		/// <param name="featureName">Feature name.</param>
		/// <returns>
		/// Returns a feature list with all features which name matches the
		/// given feature name over all sequences of the sequence list.
		/// 
		/// </returns>
		
		public FeatureList Features ( String featureListName, String featureName ) {
			FeatureList result = new FeatureList( featureListName );

			foreach ( var sequence in this ) {
				FeatureList list = sequence.FeatureLists.FirstOrDefault(
					featureList => featureList.Name == featureListName
				);

				if ( list != null ) {
					result.AddRange( list.Where( feature => feature.Name == featureName ) );
				}
			}

			return result;
		}
	}
}
