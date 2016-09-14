using System.Collections.Generic;
using System;

public class Deck
{
    private string Values = "23456789TJQKA";
    private string Sout = "SCDH";
    public List<string> _Deck = new List<string>();
    const int CountOfSout = 4;
    const int CountOfValue = 13;

    public Deck() { }

    public Deck(string EraseCard)
    {
        ShuffleDeck();
        while (EraseCard.Length != 0)
        {
            _Deck.Remove(string.Concat(EraseCard[EraseCard.Length - 2], EraseCard[EraseCard.Length - 1]));
            EraseCard = EraseCard.Remove(EraseCard.Length - 2, 2);
        }
    }

    public void ShuffleDeck()
    {
        _Deck.Clear();
        for (int i = 0; i < CountOfSout; i++)
        {
            for (int j = 0; j < CountOfValue; j++)
            {
                _Deck.Add(string.Concat(Values[j], Sout[i]));
            }
        }
        Random RND = new Random();
        for (int i = 0; i < _Deck.Count; i++)
        {
            string tmp = _Deck[0];
            _Deck.RemoveAt(0);
            _Deck.Insert(RND.Next(_Deck.Count), tmp);
        }
    }

    public string GiveCard()
    {
        string card = _Deck[0];
        _Deck.Remove(card);
        return card;
    }
}