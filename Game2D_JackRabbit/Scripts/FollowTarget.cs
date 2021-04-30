//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace JackRabbit
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        public float sensitivity = 1;

        void Awake()
        {
            SetTarget(target.position);
        }

        // Update is called once per frame
        void Update()
        {
            var current = gameObject.transform.position;
            var desired = target.position;
            var dt = Mathf.Min(Time.deltaTime, 0.1f);
            var t = sensitivity * dt;
            SetTarget(Vector3.Lerp(current, desired, t));
        }

        void SetTarget(Vector3 position3D)
        {
            var position = gameObject.transform.position;
            position.x = position3D.x;
            position.y = position3D.y;
            gameObject.transform.position = position;
        }
    }
}