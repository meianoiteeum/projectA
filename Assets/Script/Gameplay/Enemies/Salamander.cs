using System.Collections;
using System.Collections.Generic;
using Script.Core;
using UnityEngine;

namespace Script.Gameplay.Enemies
{
    public enum MovementDirection { HORIZONTAL, VERTICAL }

    public class Salamander : MonoBehaviour
    {
        [SerializeField] public MovementDirection direction;
        [SerializeField] public float second = 1f;
        [SerializeField] public float velocity = 3f;

        private int _currentNodeId;
        private MapBuilder _mapBuilder;
        private MapData _mapData;

        public void Init(int nodeId, MapBuilder mapBuilder, MapData mapData)
        {
            _currentNodeId = nodeId;
            _mapBuilder = mapBuilder;
            _mapData = mapData;
            StartCoroutine(MovementLoop());
        }

        private IEnumerator MovementLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(second);
                yield return StartCoroutine(DoMovement());
            }
        }

        private IEnumerator DoMovement()
        {
            var neighbors = _mapBuilder.GetNeighbors(_currentNodeId, _mapData.connections);
            var filtered = FilterByDirection(neighbors);

            if (filtered.Count == 0)
            {
                Debug.Log($"[Salamander] Sem caminho na direção {direction} a partir do nó {_currentNodeId}.");
                yield return new WaitForSeconds(second);
                yield break;
            }

            var target = filtered[Random.Range(0, filtered.Count)];
            int previousId = _currentNodeId;

            yield return StartCoroutine(MoveToNode(target));
            yield return new WaitForSeconds(second);
            yield return StartCoroutine(MoveToNode(_mapBuilder.Nodes[previousId]));
        }

        private List<MapNode> FilterByDirection(List<MapNode> neighbors)
        {
            var currentPos = _mapBuilder.Nodes[_currentNodeId].transform.position;
            var result = new List<MapNode>();

            foreach (var neighbor in neighbors)
            {
                if (neighbor.Data.type == NodeType.Voidd)
                {
                    Debug.Log($"[Salamander] Movimento ignorado: nó {neighbor.Data.id} é do tipo Voidd.");
                    continue;
                }

                var nPos = neighbor.transform.position;
                bool matches = direction == MovementDirection.HORIZONTAL
                    ? Mathf.Approximately(nPos.y, currentPos.y)
                    : Mathf.Approximately(nPos.x, currentPos.x);

                if (matches) result.Add(neighbor);
            }

            return result;
        }

        private IEnumerator MoveToNode(MapNode target)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = target.transform.position;
            float distance = Vector3.Distance(startPos, endPos);
            float duration = velocity > 0f ? distance / velocity : 0.3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                yield return null;
            }

            transform.position = endPos;
            _currentNodeId = target.Data.id;
        }
    }
}
