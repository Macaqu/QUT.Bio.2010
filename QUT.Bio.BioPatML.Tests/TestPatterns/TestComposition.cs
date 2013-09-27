using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using DB = System.Diagnostics.Debug;using Microsoft.VisualStudio.TestTools.UnitTesting;
//using QUT.Bio.BioPatML.Alphabets;
using QUT.Bio.BioPatML.Common.XML;
using QUT.Bio.BioPatML.Patterns;
using QUT.Bio.BioPatML.Sequences;
//using QUT.Bio.BioPatML.Sequences.List;
using BioPatML.Test;
using Bio;

/*****************| Queensland  University Of Technology |*******************
 *  Original Author          : Dr Stefan Maetschke 
 *  Translated By            : Samuel Toh (Email: yu.toh@connect.qut.edu.au) 
 *  Project supervisors      : Dr James Hogan
 *                             Mr Lawrence BuckingHam
 * 
 ***************************************************************************/
namespace TestBioPatML.TestPatterns
{
    [TestClass]
    public class TestComposition
    {
        [TestMethod]
        /** Test the constructor */
        public void TestConstructor1()
        {
            Composition composition = new Composition("Composition", Alphabets.DNA, 1, 3, 3.14, Composition.MatchMode.ALL, 0.7);
            Assert.AreEqual("Composition", composition.Name);
            Assert.AreEqual(Composition.MatchMode.ALL, composition.Mode);
            Assert.AreEqual(1, composition.MinLength);
            Assert.AreEqual(3, composition.MaxLength);
            Assert.AreEqual(3.14, composition.IncLength);
            Assert.AreEqual(0.7, composition.Threshold);
        }

        
        [TestMethod]
        /** Tests the adding of symbols to the composition */
        public void TestAdd()
        {
			Composition composition = new Composition( "Composition", Alphabets.DNA, 1, 3, 1, Composition.MatchMode.ALL, 0.0 );
            composition.Add('a', 1.0);
            composition.Add('c', 3.0);
            composition.Add('t', 4.0);
            composition.Add('g', 2.0);
            composition.DefaultWeight = (3.5);
            Assert.AreEqual(4.0, composition.MaxWeight);
            Assert.AreEqual(1.0, composition.MinWeight);
            Assert.AreEqual(1.0, composition.Weight('a'));
            Assert.AreEqual(3.0, composition.Weight('c'));
            Assert.AreEqual(4.0, composition.Weight('t'));
            Assert.AreEqual(2.0, composition.Weight('g'));
            Assert.AreEqual(3.5, composition.Weight('u'));
        }

        [TestMethod]
        /** Test the setter for the default weight */
        public void TestDefaultWeight()
        {
			Composition composition = new Composition( "Composition", Alphabets.DNA, 1, 3, 1, Composition.MatchMode.ALL, 0.0 );
            composition.DefaultWeight = (7.0);
            Assert.AreEqual(7.0, composition.MaxWeight);
            Assert.AreEqual(7.0, composition.MinWeight);

            composition.Add('c', 3.0);
            Assert.AreEqual(7.0, composition.MaxWeight);
            Assert.AreEqual(3.0, composition.MinWeight);
        }

         /** Test for match with length increment */
        [TestMethod]
        public void TestMatchIncrement()
        {
            Sequence seq = new Sequence(Alphabets.DNA, "actgactg");
			Composition composition = new Composition( "Composition", Alphabets.DNA, 1, 7, 1.7, Composition.MatchMode.ALL, 0.0 );
            Match match;
            match = composition.Match(seq, 1);
            Assert.AreEqual(1, (int)match.Count);
            Assert.AreEqual(0, composition.Increment);
            Assert.AreEqual("a", match.BaseSequence.ToString());
            match = composition.Match(seq, 1);
            Assert.AreEqual(3, match.Count);
            Assert.AreEqual(0, composition.Increment);
            Assert.AreEqual("act", match.BaseSequence.ToString());
            match = composition.Match(seq, 1);
            Assert.AreEqual(4, match.Count);
            Assert.AreEqual(0, composition.Increment);
            Assert.AreEqual("actg", match.BaseSequence.ToString());
            match = composition.Match(seq, 1);
            Assert.AreEqual(6, match.Count);
            Assert.AreEqual(0, composition.Increment);
            Assert.AreEqual("actgac", match.BaseSequence.ToString());
            match = composition.Match(seq, 1);
            Assert.AreEqual(7, match.Count);
            Assert.AreEqual(1, composition.Increment);
            Assert.AreEqual("actgact", match.BaseSequence.ToString());
        }


        /** Test for match method from pattern interface */
        public void TestMatchAll()
        {
            Sequence seq = new Sequence(Alphabets.DNA, "act");
			Composition composition = new Composition( "Composition", Alphabets.DNA, 2, 3, 1, Composition.MatchMode.ALL, 0.0 );
            composition.Add('a', 1.0);
            composition.Add('c', 2.0);
            composition.DefaultWeight = (3.0);
            Match match = composition.Match(seq, 1);
            Assert.AreEqual(2, match.Count);
            Assert.AreEqual(1, match.Start);
            Assert.AreEqual(2, match.End);
            Assert.AreEqual("ac", match.BaseSequence.ToString());
            Assert.AreEqual(0.5, match.Similarity, 1e-2);

            match = match.SearchBest(0, 0, composition);
            Assert.AreEqual(2, match.Start);
            Assert.AreEqual("ct", match.BaseSequence.ToString());
            Assert.AreEqual(0.83, match.Similarity, 1e-2);

            var matches = match.Search(0, 0, composition);
            Assert.AreEqual(3, matches.Count);
            Assert.AreEqual("ac", match.BaseSequence.ToString());
            Assert.AreEqual("act", match.BaseSequence.ToString());
            Assert.AreEqual("ct", match.BaseSequence.ToString());
        }


        /** Test for match method from pattern interface */
        [TestMethod]
        public void TestMatchBest()
        {
            Sequence seq = new Sequence(Alphabets.DNA, "actcctctg");
            Composition composition = new Composition("Composition", Alphabets.DNA, 3, 8, 1.7, Composition.MatchMode.BEST, 0.0);
            composition.Add('a', 1.0);
            composition.Add('c', 2.0);
            composition.Add('t', 3.0);
            composition.DefaultWeight = (1.0);

            Match match = composition.Match(seq, 1);
            Assert.AreEqual(1, match.Start);
            Assert.AreEqual(8, match.End);
            Assert.AreEqual("actcctct", match.Letters());
            Assert.AreEqual(0.75, match.Similarity, 1e-2);

            Match newMatch = match.SearchBest(0, 0, composition);
            Assert.AreEqual(6, newMatch.Start);
            Assert.AreEqual("tct", newMatch);
            Assert.AreEqual(0.88, newMatch.Similarity, 1e-2);
        }

        [TestMethod]
        public void TestRead()
        {
			Definition definition = DefinitionIO.Read( Global.GetResourceReader(  "BioPatMLXML/Composition.xml" ) );
            Composition composition = (Composition)definition.Pattern;

            Assert.AreEqual("Composition", definition.Name);
            Assert.AreEqual("hydrophobic region", composition.Name);
            Assert.AreEqual(15, composition.MinLength);
            Assert.AreEqual(25, composition.MaxLength);
            Assert.AreEqual(0.9, composition.Impact);
            Assert.AreEqual(Composition.MatchMode.BEST, composition.Mode);
            Assert.AreEqual(0.7, composition.Threshold);
            Assert.AreEqual(1.0, composition.Weight('V'));
            Assert.AreEqual(2.2, composition.Weight('I'));
            Assert.AreEqual(3.3, composition.Weight('L'));
 
            Assert.AreEqual(1.4, composition.IncLength);
        }

        [TestMethod]
        public void TestToXml()
        {
			Definition definition = DefinitionIO.Read( Global.GetResourceReader(  "BioPatMLXML/Composition.xml" ) );
			Assert.IsTrue( definition.ToXml().ToString().IndexOf( "name=\"auto-" ) < 0 );
			Definition def2 = DefinitionIO.Read( DefinitionIO.Write( definition ) );
			Composition composition = (Composition) def2.Pattern;

            Assert.AreEqual("Composition", definition.Name);
            Assert.AreEqual("hydrophobic region", composition.Name);
            Assert.AreEqual(15, composition.MinLength);
            Assert.AreEqual(25, composition.MaxLength);
            Assert.AreEqual(0.9, composition.Impact);
            Assert.AreEqual(Composition.MatchMode.BEST, composition.Mode);
            Assert.AreEqual(0.7, composition.Threshold);
            Assert.AreEqual(1.0, composition.Weight('V'));
            Assert.AreEqual(2.2, composition.Weight('I'));
            Assert.AreEqual(3.3, composition.Weight('L'));
 
            Assert.AreEqual(1.4, composition.IncLength);
			
		}

    }
}
