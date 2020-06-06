using UnityEngine;

public class Interact : MonoBehaviour {

	public float range = 10f;
	public Camera fpsCam;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Steal") || Input.GetMouseButtonDown(0)) {
			InteractWith ();
		}
		Debug.DrawRay (fpsCam.transform.position, fpsCam.transform.forward);
	}

	void InteractWith (){

		LayerMask lm = LayerMask.NameToLayer("IgnoreRayCast");
		RaycastHit hit;
		if (Physics.Raycast (fpsCam.transform.position, fpsCam.transform.forward, out hit, range,lm)) {
			Interactable target = hit.transform.GetComponent<Interactable> ();

			if (target != null) {
				target.Interact ();
				//Debug.Log (target.name);
			}
		}

	}
}
