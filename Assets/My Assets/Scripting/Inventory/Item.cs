using UnityEngine;

public enum CategoryItem { Trash, Scrap, Key}

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject {

    new public string name = "New Item";
    public CategoryItem category;
    public Sprite icon = null;
    //public bool isDefaultItem = false;
    public float value = 1f;
    public GameObject physicalItem;
    public float weight = 0f;
    public GameObject model = null;
    public Vector3 positionOffset; //Position offset for the model, only x / y are actually used
    public Vector3 rotationOffset = new Vector3(22.5f, 0f, 50f); //Euler angles for the rotation of the model
    public float itemScale = 2f;
    public bool consumable = false;
    public bool cursed = false;
    public bool negativeValue = false;

    [TextArea(3,10)]
    public string description = "Item Description";
    public string riven = "Riven's Comment";

    /*
    public virtual void Use() {
//Use the item
        Debug.Log("Using" + name);
    }
    //*/
}
