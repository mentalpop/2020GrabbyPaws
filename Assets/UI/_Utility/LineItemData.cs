using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "LineItem", menuName = "Generic Line Item", order = 1)]
public class LineItemData : ScriptableObject {
	public Sprite icon;
	public string text;
}