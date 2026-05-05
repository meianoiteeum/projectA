using System.Collections.Generic;
using System.Linq;
using Script.Core;
using Script.Util;
using UnityEngine;

namespace Script.Gameplay.Characters
{
    public class PlayerSpawner
    {
        public void Spawn(MapData mapData, IReadOnlyDictionary<int, MapNode> nodes,
                          Player player, MapBuilder mapBuilder, System.Action onReachEnd = null)
        {
            var startNode = mapData.Nodes.FirstOrDefault(n => n.type == NodeType.Start);
            if (startNode == null)
            {
                Debug.LogWarning("[PlayerSpawner] Nó de início não encontrado.");
                return;
            }

            var nodePos = nodes[startNode.id].transform.position;
            Vector3 pos = new Vector3(nodePos.x, nodePos.y + 0.1f, nodePos.z);

            GameObject playerObject = Object.Instantiate(player._playerPrefab, pos, Quaternion.identity);

            if (playerObject.GetComponent<PlayerController>() == null)
                playerObject.AddComponent<PlayerController>();

            var pc = playerObject.GetComponent<PlayerController>();
            pc.Init(startNode.id, mapBuilder, mapData, onReachEnd);
            playerObject.name = "Player";

            var cam = Camera.main;
            if (cam != null && cam.GetComponent<CameraFollow>() == null)
                cam.gameObject.AddComponent<CameraFollow>();

            var playerTransform = playerObject.transform;
            
            MapEvents.OnPlayerSpawned?.Invoke(playerTransform);
            
            InvokeArrow(playerTransform, player._arrowPrefab);
        }

        private void InvokeArrow(Transform playerTransform, GameObject arrowPrefab)
        {
            Vector3 pos = new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z);
            
            GameObject arrowObject = Object.Instantiate(arrowPrefab, pos, Quaternion.identity);
            
            if(arrowObject.GetComponent<ArrowController>() == null)
                arrowObject.AddComponent<ArrowController>();
            
            var ac = arrowObject.GetComponent<ArrowController>();
            ac.Init(playerTransform);

        }
    }
}
