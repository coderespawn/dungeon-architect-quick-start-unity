//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;


namespace DungeonArchitect.Samples.ShooterGame
{
    public class MiniMapCameraTracker : MonoBehaviour
    {
        public Transform trackingTransform;
        public Transform baseDungeonTransform;
        public Transform dotTransform;


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var delta = trackingTransform.position - baseDungeonTransform.position;
            gameObject.transform.localPosition = delta;
            dotTransform.rotation = trackingTransform.rotation;
        }
    }
}
