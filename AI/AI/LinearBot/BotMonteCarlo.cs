using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

public class MonteCarloLogic
{
    const double CountOfIterations = 15000.0;
    PokerLogic Logic;
    Deck sDeck;

    public MonteCarloLogic() { }

    public double GetChance(string CardsOnDesk, string MyCards, int PlayersInGame)
    {
        double Chance = 0.0;
        for (int iter = 0; iter < CountOfIterations; ++iter)
        {
            Logic = new PokerLogic();
            string AllCardsString = CardsOnDesk;
            sDeck = new Deck(string.Concat(CardsOnDesk, MyCards));
            for (int i = CardsOnDesk.Length; i < 10; i += 2)
            {
                AllCardsString += sDeck.GiveCard();
            }
            AllCardsString += MyCards;
            for (int i = 0; i < PlayersInGame - 1; ++i)
            {
                AllCardsString += sDeck.GiveCard();
                AllCardsString += sDeck.GiveCard();
            }
            Chance += Logic.PlayRound(AllCardsString, PlayersInGame);
        }
        return Chance / CountOfIterations;
    }
}
