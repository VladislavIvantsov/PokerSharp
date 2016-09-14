class Bank
{
    public int[] Bet;
    public int[] MaxBetOfPlayer;
    public int GeneralBank = 0;
    public int MaxBet = 0;

    public Bank(int _CountOfGamers)
    {
        Bet = new int[_CountOfGamers];
        MaxBetOfPlayer = new int[_CountOfGamers];
    }

    public void Raise(int Index, int _Bet, ref int _LastRise)
    {
        Bet[Index] += _Bet;
        MaxBetOfPlayer[Index] += _Bet;
        GeneralBank += _Bet;
        if (Bet[Index] > MaxBet)
        {
            MaxBet = Bet[Index];
            _LastRise = Index;
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < 6; i++)
        {
            Bet[i] = 0;
            MaxBetOfPlayer[i] = 0;
        }
        GeneralBank = 0;
        MaxBet = 0;
    }

    public void RefreshBets()
    {
        for (int i = 0; i < 6; i++) 
        Bet[i] = 0;
    }


    public void RefreshBets(int index)
    {
        Bet[index] = 0;
    }
}
