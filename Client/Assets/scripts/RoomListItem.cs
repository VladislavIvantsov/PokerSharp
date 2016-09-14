using UnityEngine;

public class RoomListItem : MonoBehaviour 
{
	public GuiListItem ListItem;
	public string RoomName;
	public int SmallBlind;
	public int GamersInRoom;
	public int MaxCountOfGamers;

	public RoomListItem(string _RoomName, int _SmallBlind, int _GamersInRoom, int _MaxCountOfGamers)
	{
		RoomName = _RoomName;
		SmallBlind = _SmallBlind;
		GamersInRoom = _GamersInRoom;
		MaxCountOfGamers = _MaxCountOfGamers;
		ListItem = new GuiListItem (RoomName);
	}
}