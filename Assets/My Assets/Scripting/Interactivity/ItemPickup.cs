using System.Collections;
using UnityEngine;

public class ItemPickup : SaveLoadInteractable
{
    public Item item;
    public GameObject pickupSphere;
    public GameObject itemToPickup;

    /*
    public float velocityThreshold = 0.01f;
    public float timeUntilDisable = 1f;
    public Rigidbody myRigidBody;
    public Collider myCollider;
    private bool coroutineActive = false;
    private bool reactToDrop = false;

    private void OnEnable() {
        Inventory.instance.OnPickUp += inventory_physicsCheck;
        Inventory.instance.OnDrop += inventory_physicsCheck;
    }

    private void OnDisable() {
        Inventory.instance.OnPickUp -= inventory_physicsCheck;
        Inventory.instance.OnDrop -= inventory_physicsCheck;
    }

    public void inventory_physicsCheck() {
        if (!reactToDrop && !myCollider.enabled) { //If the object isn't already reacting to a drop and it's collider is disabled
            myCollider.enabled = true;
            reactToDrop = true;
            //Debug.Log("inventory_physicsCheck");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (reactToDrop && myRigidBody.isKinematic) {
            //Debug.Log("other: "+other.name);
            PickUpSphere _pickUpSphere = other.GetComponent<PickUpSphere>();
            //Debug.Log("pickUpSphere: "+_pickUpSphere);
            if (_pickUpSphere != null) {
                myRigidBody.isKinematic = false;
                reactToDrop = false;
            }
        }
    }
    //*/

    public override void Interact() {
        if (UI.LockControls) {
            return;
        }
        bool wasPickedUp = Inventory.instance.Add(item);
        if (wasPickedUp) {
            Instantiate(pickupSphere, transform.position, Quaternion.identity); //Drop a sphere at point of pick-up
            Inventory.instance.OnPickUp?.Invoke(item);
            if (itemToPickup == null) {
                Destroy(gameObject);
            } else {
                Destroy(itemToPickup);
            }
            hasBeenCollected = true;
        }
        base.Interact();
        //PickUp();
    }

    /*
    void PickUp() {
        bool wasPickedUp = Inventory.instance.Add(item);
        if (wasPickedUp) {
            Instantiate(pickupSphere, transform.position, Quaternion.identity); //Drop a sphere at point of pick-up
            Inventory.instance.OnPickUp?.Invoke(); //Currently; no instance subscribes to this method
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (!coroutineActive && myRigidBody.velocity.magnitude < velocityThreshold) {
            StartCoroutine(DisablePhyics());
        }
    }

    IEnumerator DisablePhyics() {
        coroutineActive = true;
        yield return new WaitForSeconds(timeUntilDisable);
        coroutineActive = false;
        if (myCollider.enabled && myRigidBody.velocity.magnitude < velocityThreshold) {
            //Debug.Log("DisablePhyics: "+myRigidBody);
            myRigidBody.isKinematic = true;
            myCollider.enabled = false;
        }
    }
    //*/
}
