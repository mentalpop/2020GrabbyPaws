using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RigManager : MonoBehaviour
{
    public List<RigReference> rigs = new List<RigReference>();

    private RigReference activeRig;
    private HoldableHeld holdableHeld;

    public HoldableHeld AssignRig(HoldableData holdableData/*RigIDs rigID*/) {
        if (activeRig == null || (activeRig != null && holdableData.rigID != activeRig.rigID)) {
            ClearRig();
            activeRig = rigs.FirstOrDefault(p => p.rigID == holdableData.rigID);
            activeRig.rig.weight = 1;
        //Spawn the held object
            GameObject newGO = Instantiate(holdableData.heldPrefab, activeRig.rigTransform.transform);
            newGO.transform.localPosition = Vector3.zero;
            newGO.transform.localRotation = Quaternion.identity;
            holdableHeld = newGO.GetComponent<HoldableHeld>();
            holdableHeld.Unpack(holdableData);
        }
        return holdableHeld;
    }

    public void ClearRig() {
        if (activeRig != null) {
            activeRig.rig.weight = 0f;
            activeRig = null;
        }
        if (holdableHeld != null) {
            Destroy(holdableHeld.gameObject);
        }
    }

    //public void DropHeldItem() {
    //    if (activeRig != null) {
    //        Instantiate(holdableHeld.holdableData.worldPrefab, activeRig.rigTransform.transform);
    //        ClearRig();
    //    }
    //}
}