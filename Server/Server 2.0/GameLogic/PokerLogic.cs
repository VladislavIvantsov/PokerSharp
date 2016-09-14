using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

public class PokerLogic
{
    public class Card : IComparable
    {
        public int Suit;
        public int Value;

        public Card() { }

        public Card(string StringRep)
        {
            Value = Values.IndexOf(StringRep[0]);
            Suit = Suits.IndexOf(StringRep[1]);
        }

        public Card(int _Suit, int _Value)
        {
            Suit = _Suit;
            Value = _Value;
        }

        public int CompareTo(object _B)
        {
            if (_B == null) return -1;
            Card B = _B as Card;
            return CompareTo(B);
        }

        public int CompareTo(Card B)
        {
            if (Value < B.Value) return -1;
            if (Value > B.Value) return 1;
            if (Suit < B.Suit) return -1;
            if (Suit > B.Suit) return 1;
            return 0;
        }
    }

    public class Combination : IComparable
    {
        public int HandStrength;
        public List<Card> Cards;

        public Combination()
        {
            HandStrength = 0;
            Cards = new List<Card>();
        }

        public void AddCard(Card NewCard)
        {
            Cards.Add(NewCard);
        }

        public void GetHandStrength()
        {
            Cards.Sort();
            bool Straight = true;
            bool Flush = true;
            for (int i = 1; i < 5; ++i)
            {
                if (Cards[i].Suit != Cards[i - 1].Suit)
                    Flush = false;
                if (Cards[i].Value != Cards[i - 1].Value + 1)
                {
                    if (i != 4 || Cards[i - 1].Value != 3)
                        Straight = false;
                }
            }
            int PairsCount = 0;
            int SetCount = 0;
            int FourCount = 0;
            for (int i = 1; i < 5; ++i)
            {
                if (Cards[i].Value == Cards[i - 1].Value)
                    ++PairsCount;
                if (i < 2) continue;
                if (Cards[i].Value == Cards[i - 1].Value && Cards[i].Value == Cards[i - 2].Value)
                {
                    ++SetCount;
                    PairsCount -= 2;
                }
                if (i < 3) continue;
                if (Cards[i].Value == Cards[i - 1].Value && Cards[i].Value == Cards[i - 2].Value && Cards[i].Value == Cards[i - 3].Value)
                {
                    ++FourCount;
                    SetCount -= 2;
                    ++PairsCount;
                }
            }
            if (PairsCount == 1)
                HandStrength = 1;
            if (PairsCount == 2)
                HandStrength = 2;
            if (SetCount == 1)
                HandStrength = 3;
            if (Straight)
                HandStrength = 4;
            if (Flush)
                HandStrength = 5;
            if (PairsCount == 1 && SetCount == 1)
                HandStrength = 6;
            if (FourCount == 1)
                HandStrength = 7;
            if (Straight && Flush)
                HandStrength = 8;
            if (HandStrength == 8 && Cards[4].Value == 12 && Cards[0].Value == 8)
                HandStrength = 9;
            Cards.Reverse();
            List<Card> NewCards = new List<Card>();
            if (FourCount == 1)
            {
                for (int i = 0; i < 2; ++i)
                {
                    if (Cards[i].Value == Cards[i + 1].Value && Cards[i].Value == Cards[i + 2].Value && Cards[i].Value == Cards[i + 3].Value)
                    {
                        for (int j = i; j < i + 4; ++j)
                            NewCards.Add(Cards[j]);
                    }
                }
            }
            if (SetCount == 1)
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (Cards[i].Value == Cards[i + 1].Value && Cards[i].Value == Cards[i + 2].Value)
                    {
                        for (int j = i; j < i + 3; ++j)
                            NewCards.Add(Cards[j]);
                    }
                }
            }
            if (PairsCount > 0)
            {
                for (int i = 0; i < 4; ++i)
                {
                    if (Cards[i].Value == Cards[i + 1].Value)
                    {
                        if (i > 0 && Cards[i].Value == Cards[i - 1].Value) continue;
                        if (i < 3 && Cards[i].Value == Cards[i + 2].Value) continue;
                        NewCards.Add(Cards[i]);
                        NewCards.Add(Cards[i + 1]);
                    }
                }
            }
            for (int i = 0; i < 5; ++i)
            {
                if (i > 0 && Cards[i].Value == Cards[i - 1].Value) continue;
                if (i < 4 && Cards[i].Value == Cards[i + 1].Value) continue;
                NewCards.Add(Cards[i]);
            }
            if (HandStrength == 4 || HandStrength == 8)
            {
                if (NewCards[0].Value == 12 && NewCards[4].Value == 0)
                {
                    Card FirstCard = NewCards[0];
                    for (int i = 0; i < 4; ++i)
                        NewCards[i] = NewCards[i + 1];
                    NewCards[4] = FirstCard;
                }
            }
            Cards = NewCards;
        }

        public int CompareTo(object _B)
        {
            if (_B == null) return -1;
            Combination B = _B as Combination;
            return CompareTo(B);
        }

        public int CompareTo(Combination B)
        {
            if (HandStrength < B.HandStrength) return -1;
            if (HandStrength > B.HandStrength) return 1;
            for (int i = 0; i < 5; ++i)
            {
                if (Cards[i].Value < B.Cards[i].Value) return -1;
                if (Cards[i].Value > B.Cards[i].Value) return 1;
            }
            return 0;
        }
    }

    public static string Values = "23456789TJQKA";
    public static string Suits = "SCDH";
    List<Combination> BestCombinations;

    public Combination GetTheStrongestCombination(string StringRep)
    {
        List<Card> AllCards = new List<Card>();
        for (int i = 0; i < 7; ++i)
            AllCards.Add(new Card(StringRep.Substring(i * 2, 2)));
        Combination BestCombination = null;
        for (int i = 0; i < 3; ++i)
            for (int j = i + 1; j < 4; ++j)
                for (int t = j + 1; t < 5; ++t)
                    for (int k = t + 1; k < 6; ++k)
                        for (int p = k + 1; p < 7; ++p)
                        {
                            Combination CurrentCombination = new Combination();
                            CurrentCombination.AddCard(AllCards[i]);
                            CurrentCombination.AddCard(AllCards[j]);
                            CurrentCombination.AddCard(AllCards[t]);
                            CurrentCombination.AddCard(AllCards[k]);
                            CurrentCombination.AddCard(AllCards[p]);
                            CurrentCombination.GetHandStrength();
                            if (BestCombination == null)
                                BestCombination = CurrentCombination;
                            if (CurrentCombination.CompareTo(BestCombination) == 1)
                                BestCombination = CurrentCombination;
                        }
        return BestCombination;
    }

    public string GetBestCombination()
    {
        string Combination = string.Empty;
        BestCombinations.Sort();
        foreach (Card _Card in BestCombinations[5].Cards)
        {
            Combination += string.Concat(Values[_Card.Value], Suits[_Card.Suit]);
        }
        return Combination;
    }

    public string PlayRound(string AllCardsString, string AllBetsString)
    {
        BestCombinations = new List<Combination>();
        for (int PlayerID = 0; PlayerID < 6; ++PlayerID)
        {
            if (AllCardsString[10 + PlayerID * 4] == '-')
                BestCombinations.Add(null);
            else
                BestCombinations.Add(GetTheStrongestCombination(AllCardsString.Substring(0, 10) + AllCardsString.Substring(10 + PlayerID * 4, 4)));
        }
        List<int> MaxMoney = new List<int>();
        List<int> Money = new List<int>();
        List<bool> Played = new List<bool>();
        List<int> Profit = new List<int>();
        string[] ParseSplitValues = AllBetsString.Split(',');
        for (int PlayerID = 0; PlayerID < 6; ++PlayerID)
        {
            MaxMoney.Add(Convert.ToInt32(ParseSplitValues[PlayerID]));
            Money.Add(Convert.ToInt32(ParseSplitValues[PlayerID]));
            if (BestCombinations[PlayerID] == null)
                Played.Add(true);
            else
                Played.Add(false);
            Profit.Add(0);
        }

        for (int RoundID = 0; ; ++RoundID)
        {
            int WinnersCount = 0;
            Combination BestCombination = null;
            for (int PlayerID = 0; PlayerID < 6; ++PlayerID)
            {
                if (Played[PlayerID]) continue;
                if (BestCombination == null)
                {
                    WinnersCount = 1;
                    BestCombination = BestCombinations[PlayerID];
                    continue;
                }
                if (BestCombination.CompareTo(BestCombinations[PlayerID]) == -1)
                {
                    WinnersCount = 1;
                    BestCombination = BestCombinations[PlayerID];
                    continue;
                }
                if (BestCombination.CompareTo(BestCombinations[PlayerID]) == 0)
                {
                    ++WinnersCount;
                    BestCombination = BestCombinations[PlayerID];
                    continue;
                }
            }

            if (WinnersCount == 0) break;

            while (true)
            {
                int MinPot = -1;
                WinnersCount = 0;
                for (int PlayerID = 0; PlayerID < 6; ++PlayerID)
                {
                    if (Played[PlayerID]) continue;
                    if (Money[PlayerID] == 0) continue;
                    if (BestCombinations[PlayerID].CompareTo(BestCombination) == 0)
                    {
                        ++WinnersCount;
                        if (MinPot == -1)
                            MinPot = Money[PlayerID];
                        else
                            MinPot = Math.Min(MinPot, Money[PlayerID]);
                    }
                }

                if (MinPot == -1) break;

                int CurrentPot = 0;

                for (int PlayerID = 0; PlayerID < 6; ++PlayerID)
                {
                    if (Money[PlayerID] == 0) continue;
                    int Dif = Math.Min(MinPot, Money[PlayerID]);
                    Money[PlayerID] -= Dif;
                    CurrentPot += Dif;
                }
                int Remainder = CurrentPot % WinnersCount;
                CurrentPot /= WinnersCount;
                for (int PlayerID = 0; PlayerID < 6; ++PlayerID)
                {
                    if (Played[PlayerID]) continue;
                    if (BestCombinations[PlayerID].CompareTo(BestCombination) == 0)
                    {
                        Profit[PlayerID] += CurrentPot;
                        Profit[PlayerID] += Remainder;
                        Remainder = 0;
                    }
                }
                for (int PlayerID = 0; PlayerID < 6; ++PlayerID)
                    if (Money[PlayerID] == 0)
                        Played[PlayerID] = true;
            }
        }

        String Result = "";

        for (int PlayerID = 0; PlayerID < 6; ++PlayerID)
        {
            Result += Convert.ToString(Profit[PlayerID]);
            Result += ",";
        }

        for (int PlayerID = 0; PlayerID < 6; ++PlayerID)
        {
            if (PlayerID != 0)
                Result += ",";
            if (BestCombinations[PlayerID] == null)
            {
                Result += "0";
                continue;
            }
            Result += Convert.ToString(BestCombinations[PlayerID].HandStrength);
        }

        return Result;
    }
}
