using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Core
{
    public class PlayerController : MonoBehaviour
    {
        public float moveDuration = 0.3f;

        private int _currentNodeId;
        private MapBuilder _mapBuilder;
        private IReadOnlyList<ConnectionData> _connections;
        private bool _isMoving;
        private bool _controllingPlayer = true;

        public void Init(int startNodeId, MapBuilder mapBuilder, IReadOnlyList<ConnectionData> connections)
        {
            _currentNodeId = startNodeId;
            _mapBuilder = mapBuilder;
            _connections = connections;
        }

        void Update()
        {
            if (_mapBuilder == null) return;

            if (Keyboard.current.tabKey.wasPressedThisFrame)
                SwitchCharacter();

            if (_isMoving || !_controllingPlayer) return;

            Vector2 direction = Vector2.zero;
            if (Keyboard.current.wKey.wasPressedThisFrame) direction = Vector2.up;
            else if (Keyboard.current.sKey.wasPressedThisFrame) direction = Vector2.down;
            else if (Keyboard.current.aKey.wasPressedThisFrame) direction = Vector2.left;
            else if (Keyboard.current.dKey.wasPressedThisFrame) direction = Vector2.right;

            if (direction != Vector2.zero)
                TryMove(direction);
        }

        private void SwitchCharacter()
        {
            _controllingPlayer = !_controllingPlayer;
            Transform target = _controllingPlayer
                ? transform
                : _mapBuilder.Nodes[_currentNodeId].transform;
            MapEvents.OnCharacterSwitched?.Invoke(target, _controllingPlayer);
        }

        private void TryMove(Vector2 direction)
        {
            List<MapNode> neighbors = _mapBuilder.GetNeighbors(_currentNodeId, _connections);
            Vector2 currentPos = transform.position;

            MapNode bestNeighbor = null;
            float bestDot = 0f;

            foreach (var neighbor in neighbors)
            {
                Vector2 neighborPos = neighbor.transform.position;
                Vector2 dirToNeighbor = (neighborPos - currentPos).normalized;
                float dot = Vector2.Dot(dirToNeighbor, direction);
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestNeighbor = neighbor;
                }
            }

            if (bestNeighbor != null)
                StartCoroutine(MoveToNode(bestNeighbor));
        }

        private IEnumerator MoveToNode(MapNode target)
        {
            _isMoving = true;
            _currentNodeId = target.Data.id;
            Vector3 startPos = transform.position;
            Vector3 endPos = target.transform.position;
            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
                yield return null;
            }

            transform.position = endPos;
            _isMoving = false;
        }
    }
}
