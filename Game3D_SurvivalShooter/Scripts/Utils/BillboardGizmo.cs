//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

public class BillboardGizmo : MonoBehaviour {
	public string iconName = "flag_icon.png";
	void OnDrawGizmos() {
		Gizmos.DrawIcon(gameObject.transform.position, iconName, true);
		transform.localScale = new Vector3(.2f, .2f, .2f);
	}
}
