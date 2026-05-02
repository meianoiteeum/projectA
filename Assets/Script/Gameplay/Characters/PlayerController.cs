using System.Collections;
using System.Collections.Generic;
using Script.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Gameplay.Characters
{
    public class PlayerController : MonoBehaviour
    {
        public float moveDuration = 0.3f;
        public float rotationDuration = 0.25f;

        private int _currentNodeId;
        private int _selectedNodeId;
        private int _originNodeId;
        private bool _hasMoved;
        private MapBuilder _mapBuilder;
        private MapData _mapData;
        private bool _isMoving;
        private bool _isRotating;
        private bool _controllingPlayer = true;

        private IReadOnlyList<ConnectionData> Connections => _mapData?.connections;

        public void Init(int startNodeId, MapBuilder mapBuilder, MapData mapData)
        {
            _currentNodeId = startNodeId;
            _mapBuilder = mapBuilder;
            _mapData = mapData;
        }

        void Update()
        {
            if (_mapBuilder == null) return;

            if (Keyboard.current.tabKey.wasPressedThisFrame)
                SwitchCharacter();

            if (_isMoving || _isRotating) return;

            if (!_controllingPlayer)
            {
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    TryRotate(true);
                    return;
                }
                if (Keyboard.current.qKey.wasPressedThisFrame)
                {
                    TryRotate(false);
                    return;
                }
            }

            Vector2 direction = Vector2.zero;
            if (Keyboard.current.wKey.wasPressedThisFrame) direction = Vector2.up;
            else if (Keyboard.current.sKey.wasPressedThisFrame) direction = Vector2.down;
            else if (Keyboard.current.aKey.wasPressedThisFrame) direction = Vector2.left;
            else if (Keyboard.current.dKey.wasPressedThisFrame) direction = Vector2.right;

            if (direction == Vector2.zero) return;

            if (_controllingPlayer)
                TryMove(direction);
            else
                TryMoveNode(direction);
        }

        private void SwitchCharacter()
        {
            if (_isRotating) return;

            _controllingPlayer = !_controllingPlayer;

            if (_controllingPlayer)
            {
                _mapBuilder.Nodes[_selectedNodeId].Unhighlight();
                MapEvents.OnCharacterSwitched?.Invoke(transform, true);
            }
            else
            {
                _selectedNodeId = _currentNodeId;
                _originNodeId = _currentNodeId;
                _hasMoved = false;
                _mapBuilder.Nodes[_selectedNodeId].Highlight();
                MapEvents.OnCharacterSwitched?.Invoke(_mapBuilder.Nodes[_selectedNodeId].transform, false);
            }
        }

        private void TryMove(Vector2 direction)
        {
            List<MapNode> neighbors = _mapBuilder.GetWalkableNeighbors(_currentNodeId, Connections);
            Vector2 currentPos = _mapBuilder.Nodes[_currentNodeId].transform.position;

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

        private void TryMoveNode(Vector2 direction)
        {
            if (_hasMoved)
            {
                Vector2 currentPos = _mapBuilder.Nodes[_selectedNodeId].transform.position;
                Vector2 originPos = _mapBuilder.Nodes[_originNodeId].transform.position;
                Vector2 dirToOrigin = (originPos - currentPos).normalized;
                float dot = Vector2.Dot(dirToOrigin, direction);
                if (dot <= 0f) return;

                _mapBuilder.Nodes[_selectedNodeId].Unhighlight();
                _selectedNodeId = _originNodeId;
                _hasMoved = false;
                _mapBuilder.Nodes[_selectedNodeId].Highlight();
                MapEvents.OnCharacterSwitched?.Invoke(_mapBuilder.Nodes[_selectedNodeId].transform, false);
                return;
            }

            List<MapNode> neighbors = _mapBuilder.GetNeighbors(_selectedNodeId, Connections);
            Vector2 pos = _mapBuilder.Nodes[_selectedNodeId].transform.position;

            MapNode bestNeighbor = null;
            float bestDot = 0f;

            foreach (var neighbor in neighbors)
            {
                Vector2 neighborPos = neighbor.transform.position;
                Vector2 dirToNeighbor = (neighborPos - pos).normalized;
                float d = Vector2.Dot(dirToNeighbor, direction);
                if (d > bestDot)
                {
                    bestDot = d;
                    bestNeighbor = neighbor;
                }
            }

            if (bestNeighbor == null) return;

            _mapBuilder.Nodes[_selectedNodeId].Unhighlight();
            _selectedNodeId = bestNeighbor.Data.id;
            _hasMoved = true;
            _mapBuilder.Nodes[_selectedNodeId].Highlight();
            MapEvents.OnCharacterSwitched?.Invoke(_mapBuilder.Nodes[_selectedNodeId].transform, false);
        }

        private void TryRotate(bool clockwise)
        {
            _isRotating = true;
            bool started = _mapBuilder.RotateNode(_selectedNodeId, clockwise, _mapData, rotationDuration,
                () => _isRotating = false);
            if (!started) _isRotating = false;
        }

        private IEnumerator MoveToNode(MapNode target)
        {
            _isMoving = true;
            _currentNodeId = target.Data.id;
            Vector3 startPos = transform.position;
            Vector3 endPos = new Vector3(target.transform.position.x, target.transform.position.y + 0.1f, target.transform.position.z);
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
