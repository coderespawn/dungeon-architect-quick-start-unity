//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections;
using DungeonArchitect;


public class MiniMapCameraTracker : MonoBehaviour {
	public Transform trackingTransform;
	public Transform baseDungeonTransform;
	public Transform dotTransform;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		var delta = trackingTransform.position - baseDungeonTransform.position;
		gameObject.transform.localPosition = delta;
		dotTransform.rotation = trackingTransform.rotation;
	}
}
