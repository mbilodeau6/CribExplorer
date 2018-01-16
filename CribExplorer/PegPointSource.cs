using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer
{
    public enum PegPointType
    {
        Fifeteen,
        ThirtyOne,
        LastCardInRound,
        TwoOfSame,
        ThreeOfSame,
        FourOfSame,
        RunOfThree,
        RunOfFour,
        RunOfFive
    }

    public class PegPointSource
    {
        public PegPointSource(PegPointType pointSource)
        {
            PointSource = pointSource;
        }

        public int GetPegPoints()
        {
            switch(PointSource)
            {
                case PegPointType.Fifeteen:
                    return 2;
                case PegPointType.ThirtyOne:
                    return 2;
                case PegPointType.LastCardInRound:
                    return 1;
                case PegPointType.TwoOfSame:
                    return 2;
                case PegPointType.ThreeOfSame:
                    return 6;
                case PegPointType.FourOfSame:
                    return 12;
                case PegPointType.RunOfThree:
                    return 3;
                case PegPointType.RunOfFour:
                    return 4;
                case PegPointType.RunOfFive:
                    return 5;
            }

            throw new NotImplementedException("PegPointType provided is not supported.");
        }

        public PegPointType PointSource
        {
            get;
            private set;
        }
    }
}
