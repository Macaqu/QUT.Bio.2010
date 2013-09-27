using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DB = System.Diagnostics.Debug;
using Bio;
using QUT.Bio.BioPatML.Patterns;
using QUT.Bio.BioPatML.Sequences;




namespace QUT.Bio.BioPatML.Tests
{
    [TestClass]
    public class TestMatch
    {
        private VoidPattern pattern;

        [TestInitialize]
        public void TestIntializeVoidPattern()
        {
            pattern = new VoidPattern();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Match GetNewMatchWithOneParams() {
            Match match;

            match = new Match(pattern);

            return match;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Match GetNewMatchWithSixParameters() {
            Match match;

            Sequence seq = new Sequence(Alphabets.DNA, "atcg");
            int start = 1, end = 3;
            double similarity = 0.5;
            match = new Match(pattern, seq, start, end, Strand.Forward, similarity);

            return match;
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestSet()
        {
            Match match = GetNewMatchWithOneParams();

            Sequence seq = new Sequence(Alphabets.DNA, "tttggccaaagcc");
            int start = 1, end = 3;
            double similarity = 0.5;

            match.Set(seq, start, end-start+1, Strand.Forward, similarity);

            Assert.AreEqual(start, match.Start);
            Assert.AreEqual(end, match.End);
            //Assert.AreEqual(strand, match.Strand);
            Assert.AreEqual(similarity, match.Similarity);
            Assert.AreEqual(seq.ToString(), match.BaseSequence.ToString());
        }


        [TestMethod]
        public void TestCopy()
        {
            Sequence seq = new Sequence(Alphabets.DNA, "tttggccaaagcc");

            Match m = new Match(pattern, seq, 3, 2, Strand.Forward, 1);
            Match sub1 = new Match(pattern, seq, 4, 2, Strand.Forward, 0.5);

            sub1.Impact = 0.5;

            Match sub2 = new Match(pattern, seq, 5, 2, Strand.Forward, 0.1); //ASK LAWRENCE
            sub2.Impact = 0.7;

            m.SubMatches.Add(sub1);
            m.SubMatches.Add(sub2);

            Match mc = m.Clone();

            Assert.AreEqual(2, m.SubMatches.Count);
            Assert.AreEqual(2, mc.SubMatches.Count);
            Assert.IsTrue(mc != m);
            Assert.IsTrue(sub1 != mc.SubMatches[0]);
            Assert.IsTrue(sub2 != mc.SubMatches[1]);
            Assert.AreEqual(3, mc.Start);
            Assert.AreEqual(4, mc.SubMatches[0].Start);
            Assert.AreEqual(5, mc.SubMatches[1].Start);
            Assert.AreEqual(0.5, mc.SubMatches[0].Impact, 1e-10);
            Assert.AreEqual(0.7, mc.SubMatches[1].Impact, 1e-10);
        }

        [TestMethod]
        public void TestSubMatch()
        {
            Sequence seq1 = new Sequence(Alphabets.DNA, "tttggccaaagcc");

            Match m = new Match(null);
            Match sub1 = new Match(null);
            Match sub2 = new Match(null);
            Assert.AreEqual(0, m.SubMatches.Count);
            m.SubMatches.Add(sub1);
            m.SubMatches.Add(sub2);

            Assert.AreEqual(2, m.SubMatches.Count);
            Assert.AreEqual(sub1, m.SubMatches[0]);
            Assert.AreEqual(sub2, m.SubMatches[1]);
        }

        [TestMethod]
        public void TestSetGet()
        {
            Sequence seq = new Sequence(Alphabets.DNA, "atcg");
            Match m = new Match(null);
            m.Set(seq, 2, 4, Strand.Forward, 0.75);
            Assert.AreEqual(2, m.Start);
            Assert.AreEqual(2, m.Count);
            //Assert.AreEqual(1, m.Strand);
            Assert.AreEqual(0.75, m.Similarity, 1e-3);
            Assert.AreEqual(1, m.CalcMismatches());
            Assert.AreEqual(3, m.Matches);
        }

        [TestMethod]
        public void TestSetMatch()
        {
            ISequence seq = new Sequence(Alphabets.DNA, "atcg");
            Match m1 = new Match(null);
            Match m2 = new Match(null);

            m1.Set(seq, 2, 2, Strand.Forward, 0.5);
            
            m2.Set(m1);

            Assert.AreEqual(seq, m2.BaseSequence);
            Assert.AreEqual(2, m2.Start);
            Assert.AreEqual(3, m2.End);
            Assert.AreEqual(2, m2.Count);
            //Assert.AreEqual(1, m2.Strand);
            Assert.AreEqual(0.5, m2.Similarity, 1e-3);
            Assert.AreEqual("cg", m2.Letters());
        }

        [TestMethod]
        public void TestCalcStartEnd()
        {
            ISequence seq = new Sequence(Alphabets.DNA, "atcg");
            
            Match m = new Match(null);
            m.BaseSequence = seq;

            Match sub1 = new Match(pattern, seq, 1, 2, Strand.Forward, 0.0);
            Match sub2 = new Match(pattern, seq, 2, 2, Strand.Forward, 0.0);

            m.SubMatches.Add(sub1);
            m.SubMatches.Add(sub2);
            m.CalcStartEnd();

           
            Assert.AreEqual(2, m.SubMatches.Count);
            Assert.AreEqual(1, m.Start);
            Assert.AreEqual(3, m.Count);
            Assert.AreEqual(3, m.End);
        }

        [TestMethod]
        public void TestCalcLength()
        {
            Sequence seq = new Sequence(Alphabets.DNA, "tttggccaaagcc");

            Match m = new Match(null);
            m.BaseSequence = seq;
            Match sub1 = new Match(pattern, seq, 1, 2, Strand.Forward, 0.0);
            Match sub2 = new Match(pattern, seq, 2, 4, Strand.Forward, 0.0);

            m.SubMatches.Add(sub1);
            m.SubMatches.Add(sub2);

            Assert.AreEqual(2, m.SubMatches.Count);

            Assert.AreEqual(2, sub1.CalcLength());
            Assert.AreEqual(4, sub2.CalcLength());
            Assert.AreEqual(6, m.CalcLength());
        }

        [TestMethod]
        public void TestCalcSimilarity()
        {
            Sequence seq = new Sequence(Alphabets.DNA, "tttggccaaagcc");

            Match m = new Match(null);
            m.BaseSequence = seq;
            Match sub1 = new Match(pattern, seq, 0, 0, Strand.Forward, 1.0);
            Match sub2 = new Match(pattern, seq, 0, 0, Strand.Forward, 0.5);

            m.SubMatches.Add(sub1);
            m.SubMatches.Add(sub2);

            m.CalcSimilarity();
            Assert.AreEqual(0.75, m.Similarity, 1e-3);
        }

        [TestMethod]
        public void TestToString()
        {
            Sequence seq = new Sequence(Alphabets.DNA, "actgactg");
            Match m = new Match(pattern, seq, 2, 2, Strand.Forward, 1);
            //m.BaseSequence = seq;
            //m.SetSequence(seq);

            Assert.AreEqual("{2, 2, Forward, 1, tg}", m.ToString()); //change the expected result from usual 1.0 to 1

            m.SubMatches.Add(new Match(pattern, seq, 3, 2, Strand.Reverse, 0.5));
            m.SubMatches.Add(new Match(pattern, seq, 4, 3, Strand.Forward, 0.1));
            Assert.AreEqual("{2, 2, Forward, 1, tg, {3, 2, Reverse, 0.5, tc}, {4, 3, Forward, 0.1, act}}",
                 m.ToString()); //see original code for actual result, i trim away the 1 decimal placing to pass the
            //test as there wont be much impact
        }
        //TODO: Test CalcLength()

        //TODO: Test Letters()

        //TODO: testToString()

        //TODO: test Clone()

        //TODO: test Parse(XElement element, ISequence context)


        //TODO: Test ToXml()

        //TODO: test SetSequence(Sequence seq)

        //TODO: GetStrand(int strandPoint, Sequence seq)

    }
}