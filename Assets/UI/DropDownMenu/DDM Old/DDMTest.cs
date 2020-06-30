using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDMTest : MonoBehaviour
{
	public DropDownMenu myDDM;
	public Image myImage;

    private void OnEnable() {
        myDDM.OnChoiceMade += UpdateColor;
    }

    private void OnDisable() {
        myDDM.OnChoiceMade -= UpdateColor;
    }

	void UpdateColor(int newColor) {
		switch(newColor) {
            default: myImage.color = Color.red; break;
            case 1: myImage.color = Color.green; break;
            case 2: myImage.color = Color.blue; break;
            case 3: myImage.color = Color.yellow; break;
        }
	}
}