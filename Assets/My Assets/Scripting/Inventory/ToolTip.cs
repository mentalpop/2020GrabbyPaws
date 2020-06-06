using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour {




    #region Singleton
    public static ToolTip instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one inventory in scene");
            return;
        }
        instance = this;

    }
    #endregion

    Item myItem;
    InventorySlot mySlot;





    public void ToggleMe(Item i, Transform t, InventorySlot s)
    {
        transform.localPosition = new Vector3(t.localPosition.x + 288.6f/2, t.localPosition.y - 176.5f/2, -50.0f);
        
        itemName.text = i.name;
        itemDescription.text = i.description;
        myItem = i;
        mySlot = s;
       


    }

    public Text itemName;
    public Text itemDescription;

	// Use this for initialization
	void Start () {
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void CloseToolTip()
    {
        itemName.text = "";
        itemDescription.text = "";
        gameObject.SetActive(false);

    }

    /*
    public void UseItem()
    {
        myItem.Use();
        if (myItem.consumable)
        {
            Inventory.instance.Remove(myItem);
        }
        CloseToolTip();

    }
    //*/
}
