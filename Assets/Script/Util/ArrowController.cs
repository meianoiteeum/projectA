using UnityEngine;

namespace Script.Util
{
    public class ArrowController : MonoBehaviour
    {
        private Transform _playerTransform;
        private Vector3 _offset;

        public void Init(Transform playerTransform)
        {
            _playerTransform = playerTransform;
            _offset = transform.position - playerTransform.position;
        }

        void Update()
        {
            if (_playerTransform == null) return;
            transform.position = _playerTransform.position + _offset;
        }
    }
}
