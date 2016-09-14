public class GamePlace
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
        Online = true;
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
}
