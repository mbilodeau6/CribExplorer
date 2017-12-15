using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CribExplorer.Model;

namespace CribExplorerTests
{
    public class TestHelpers
    {
        public static void DiscardCards(GameState state, int discardCount)
        {
            int i = 0;
            foreach (Player player in state.Players)
            {
                IList<Card> cardsInHand = new List<Card>(player.Hand.Cards);

                foreach (Card card in cardsInHand)
                {
                    if (i >= discardCount)
                        return;

                    player.Discard(card);
                    i++;
                }
            }

            if (i < discardCount)
                throw new ArgumentException(string.Format("Unable to discard {0} cards because only {1} cards exist.", discardCount, i));
        }
    }
}
