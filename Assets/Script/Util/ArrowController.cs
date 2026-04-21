using System;
using UnityEngine;

namespace Script.Util
{
    public class ArrowController : MonoBehaviour
    {
        private Transform _playerTransform;
        public void Init(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        void Update()
        {
            if (Math.Abs(transform.position.x - _playerTransform.position.x) < 1e-6f)
                return;
            
            Vector3 pos = new Vector3(_playerTransform.position.x, _playerTransform.position.y, _playerTransform.position.z).normalized;


            transform.position += pos * 5f * Time.deltaTime;
        }
    }
}
