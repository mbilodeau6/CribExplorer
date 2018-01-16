using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer
{
    public class PegPoints
    {
        private IList<PegPointSource> pegPointSources;

        public PegPoints(int playerId)
        {
            pegPointSources = new List<PegPointSource>();
            PlayerId = playerId;
        }

        public int PlayerId
        {
            get;
            private set;
        }

        public void Add(PegPointType pointSource)
        {
            pegPointSources.Add(new PegPointSource(pointSource));
        }

        public IEnumerable<PegPointSource> Sources()
        {
            foreach (PegPointSource source in pegPointSources)
                yield return source;
        }

        public int GetTotalPoints()
        {
            int points = 0;

            foreach (PegPointSource source in pegPointSources)
                points += source.GetPegPoints();

            return points;
        }
    }
}
