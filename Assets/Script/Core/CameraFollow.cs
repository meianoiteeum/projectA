using UnityEngine;

namespace Script.Core
{
    public class CameraFollow : MonoBehaviour
    {
        public float smoothSpeed = 5f;

        private Transform _target;

        private void OnEnable()  => MapEvents.OnPlayerSpawned += SetTarget;
        private void OnDisable() => MapEvents.OnPlayerSpawned -= SetTarget;

        private void SetTarget(Transform t) => _target = t;

        void LateUpdate()
        {
            if (_target == null) return;
            Vector3 desired = new Vector3(_target.position.x, _target.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
        }
    }
}
