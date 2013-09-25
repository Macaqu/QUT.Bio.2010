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
namespace QUT.Bio.BioPatML.Sequences.List
{
    /// <summary> This interfaces describes accessor methods for annotated objects.
    /// </summary>
    
	public interface IAnnotated
    {
        /// <summary> Checks whether the list has any annotations
        /// </summary>
        /// <returns> Returns true if the object has at least one annotation and
        /// false otherwise.</returns>
        
		bool HasAnnotations{ get; }

        /// <summary> Gets a list of available annotations. 
        /// </summary>
        /// <returns>  Returns the list of annotations attached to the object. </returns>
        
		AnnotationList Annotations { get; }
    }
}
