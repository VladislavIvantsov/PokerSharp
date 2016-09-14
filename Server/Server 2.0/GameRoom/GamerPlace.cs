public class GamerPlace
{
    public Gamer Gameroncahair = null;
    public bool Hold; // занято место или нет
    public bool State; // галочка готовности
    public bool Play; // участие в игре
    public bool Online = true; // вышел ли из игры
    public bool Dealer;
    public bool Allin = false; // пошёл ли в all-in
    public string[] Card = new string[2];
    public int Prize;
    public int StrongOfCombination;

    public void Clean()
    {
        Hold = false;
        Gameroncahair = null;
        Play = false;
        State = false;
        for (int i = 0; i < 2; ++i)
        {
            Card[i] = string.Empty;
        }
    }

    public void Fold()
    {
        Play = false;
        for (int i = 0; i < 2; ++i)
        {
            Card[i] = string.Empty;
        }
    }

    public void NotReady()
    {
        State = false;
    }

    static public int GamerPlaceCompareBySOC(GamerPlace tmp1, GamerPlace tmp2)
    {
        return (-1) * tmp1.StrongOfCombination.CompareTo(tmp2.StrongOfCombination);
    }

    static public int GamerPlaceCompareByPrize(GamerPlace tmp1, GamerPlace tmp2)
    {
        return (-1) * tmp1.Prize.CompareTo(tmp2.Prize);
    }
}
