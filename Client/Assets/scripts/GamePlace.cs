using UnityEngine;

public class GamePlace : MonoBehaviour
{
	public int Money;
	public string Name;
	public int Bet;
	public bool Dealer;
	public bool Hold;
	public bool Play; // участие в игре
	public string[] Card = new string[2];

	public GamePlace()
	{
		Money = 0;
		Name = string.Empty;
		Bet = 0;
		Dealer = false;
		Play = false;
		Hold = false;
		Card [0] = string.Empty;
		Card [1] = string.Empty;
	}
}