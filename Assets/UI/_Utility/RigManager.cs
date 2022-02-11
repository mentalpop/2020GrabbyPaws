using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RigManager : MonoBehaviour
{
    public List<RigReference> rigs = new List<RigReference>();

    private RigReference activeRig;
    public HoldableHeld HoldableHeld { get; private set; }

    public HoldableHeld AssignRig(int holdableID) {
        return AssignRig(Inventory.instance.holdableMetaList.GetHoldable(holdableID));
    }

    public HoldableHeld AssignRig(HoldableData holdableData) {
        if (activeRig == null || (activeRig != null && holdableData.rigID != activeRig.rigID)) {
            ClearRig();
            activeRig = rigs.FirstOrDefault(p => p.rigID == holdableData.rigID);
            activeRig.rig.weight = 1;
        //Spawn the held object
            GameObject newGO = Instantiate(holdableData.heldPrefab, activeRig.rigTransform.transform);
            newGO.transform.localPosition = Vector3.zero;
            newGO.transform.localRotation = Quaternion.identity;
            HoldableHeld = newGO.GetComponent<HoldableHeld>();
            HoldableHeld.Unpack(holdableData);
        }
        return HoldableHeld;
    }

    public void ClearRig() {
        if (activeRig != null) {
            activeRig.rig.weight = 0f;
            activeRig = null;
        }
        if (HoldableHeld != null) {
            Destroy(HoldableHeld.gameObject);
        }
    }

    //public void DropHeldItem() {
    //    if (activeRig != null) {
    //        Instantiate(holdableHeld.holdableData.worldPrefab, activeRig.rigTransform.transform);
    //        ClearRig();
    //    }
    //}
}