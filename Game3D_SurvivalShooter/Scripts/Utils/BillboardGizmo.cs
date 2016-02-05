using UnityEngine;
using System.Collections;

public class BillboardGizmo : MonoBehaviour {
	public string iconName = "flag_icon.png";
	void OnDrawGizmos() {
		Gizmos.DrawIcon(gameObject.transform.position, iconName, true);
		transform.localScale = new Vector3(.2f, .2f, .2f);
	}
}
