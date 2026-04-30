using Script.Core;
using UnityEngine;

namespace Script.Util
{
    public class ArrowController : MonoBehaviour
    {
        private Transform _target;
        private Vector3 _offset;
        private SpriteRenderer _sr;

        public void Init(Transform playerTransform)
        {
            _target = playerTransform;
            _offset = transform.position - playerTransform.position;
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null) _sr.color = Color.blue;
        }

        private void OnEnable() => MapEvents.OnCharacterSwitched += OnCharacterSwitched;
        private void OnDisable() => MapEvents.OnCharacterSwitched -= OnCharacterSwitched;

        private void OnCharacterSwitched(Transform newTarget, bool isPlayer)
        {
            _target = newTarget;
            if (_sr != null) _sr.color = isPlayer ? Color.blue : Color.yellow;
        }

        void Update()
        {
            if (_target == null) return;
            transform.position = _target.position + _offset;
        }
    }
}
