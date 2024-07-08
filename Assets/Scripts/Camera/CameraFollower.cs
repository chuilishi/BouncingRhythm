using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace DefaultNamespace.Camera
{
    public class CameraFollower : MonoBehaviour
    {
        public Transform target;
        public float smoothTime = 0.02f;
        private Vector2 velocity;

        private void Awake()
        {
            Assert.IsNotNull(target);
        }
        private void Update()
        {
            // var smoothDamp = Vector2.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
            transform.position = target.position;
            // new Vector3(smoothDamp.x,smoothDamp.y,-10);
        }
    }
}