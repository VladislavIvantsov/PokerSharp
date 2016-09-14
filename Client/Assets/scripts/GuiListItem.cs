using UnityEngine;
using System;

public class GuiListItem : MonoBehaviour 
{
	public bool Selected;
	public string Name;
	public GuiListItem(bool mSelected, string mName)
	{
		Selected = mSelected;
		Name = mName;
	}
	public GuiListItem(string mName)
	{
		Selected = false;
		Name = mName;
	}
	public void Enable()
	{
		Selected = true;
	}
	public void Disable()
	{
		Selected = false;
	}
}