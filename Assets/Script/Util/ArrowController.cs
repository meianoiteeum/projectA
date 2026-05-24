using System.Collections;
using Script.Core;
using UnityEngine;

namespace Script.Util
{
    public class ArrowController : MonoBehaviour
    {
        private Transform _target;
        private Vector3 _offset;
        private SpriteRenderer _sr;
        private ArrowState _currentState;
        private Coroutine _redTimer;

        public void Init(Transform playerTransform)
        {
            _target = playerTransform;
            _offset = transform.position - playerTransform.position;
            _sr = GetComponent<SpriteRenderer>();
            _currentState = ArrowState.Player;
            if (_sr != null) _sr.color = Color.blue;
        }

        private void OnEnable() => MapEvents.OnArrowStateChanged += OnArrowStateChanged;
        private void OnDisable() => MapEvents.OnArrowStateChanged -= OnArrowStateChanged;

        private void OnArrowStateChanged(Transform newTarget, ArrowState state)
        {
            if (newTarget != null)
                _target = newTarget;

            //valida se não há a rotação e ativa para a cor vermelha, tem uma rotina de esperar 3s para retornar a cor anterior "amarela"
            if (state == ArrowState.NoRotation)
            {
                if (_redTimer != null) StopCoroutine(_redTimer);
                _redTimer = StartCoroutine(ShowRedThenRevert());
                return;
            }

            _currentState = state;
            ApplyColor(state);
        }
        
        //método para aplicar a cor vermelha e após 3s retornar a cor anterior
        private IEnumerator ShowRedThenRevert()
        {
            if (_sr != null) _sr.color = Color.red;
            yield return new WaitForSeconds(3f);
            ApplyColor(_currentState);
            _redTimer = null;
        }

        private void ApplyColor(ArrowState state)
        {
            if (_sr == null) return;
            _sr.color = state == ArrowState.Player ? Color.blue : Color.yellow;
        }

        void Update()
        {
            if (_target == null) return;
            transform.position = _target.position + _offset;
        }
    }
}
