using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CribExplorer;

namespace CribExplorerTests
{
    [TestClass]
    public class PegPointTests
    {
        [TestMethod]
        public void PegPoints_PlayerId()
        {
            PegPoints pegPoints = new PegPoints(3);

            Assert.AreEqual(3, pegPoints.PlayerId);
        }

        [TestMethod]
        public void PegPoints_GetTotalPoints()
        {
            PegPoints pegPoints = new PegPoints(0);

            pegPoints.Add(PegPointType.Fifeteen);
            pegPoints.Add(PegPointType.FourOfSame);

            Assert.AreEqual(14, pegPoints.GetTotalPoints());
        }

        [TestMethod]
        public void PegPoints_PointSources()
        {
            PegPoints pegPoints = new PegPoints(0);

            IList<PegPointType> testSources = new List<PegPointType>()
            {
                PegPointType.Fifeteen,
                PegPointType.FourOfSame,
                PegPointType.LastCardInRound,
            };

            for(int i=0; i < testSources.Count; i++)
                pegPoints.Add(testSources[i]);

            int j = 0;

            foreach(PegPointSource source in pegPoints.Sources())
            {
                Assert.AreEqual(testSources[j], source.PointSource, string.Format("Unexpected entry at position {0}", j));
                j++;
            }

            Assert.AreEqual(j, testSources.Count, "Item missing from PointSources");
        }
    }
}
