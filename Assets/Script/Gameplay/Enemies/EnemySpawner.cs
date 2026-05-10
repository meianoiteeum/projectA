using System;
using System.Collections.Generic;
using System.Linq;
using Script.Core;
using UnityEngine;

namespace Script.Gameplay.Enemies
{
    public class EnemySpawner
    {
        private readonly List<GameObject> _currentEnemies = new();

        public void Spawn(MapData mapData, IReadOnlyDictionary<int, MapNode> nodes, GameObject enemyPrefab, Action onHitPlayer)
        {
            if (enemyPrefab == null)
            {
                Debug.LogWarning("[EnemySpawner] Enemy prefab não atribuído.");
                return;
            }

            foreach (var enemy in _currentEnemies)
                if (enemy != null) UnityEngine.Object.Destroy(enemy);
            _currentEnemies.Clear();

            var combatNodes = mapData.Nodes.Where(n => n.type == NodeType.Combat);
            foreach (var nodeData in combatNodes)
            {
                var nodePos = nodes[nodeData.id].transform.position;
                Vector3 pos = new Vector3(nodePos.x, nodePos.y, nodePos.z);
                var enemyObject = UnityEngine.Object.Instantiate(enemyPrefab, pos, Quaternion.identity);
                enemyObject.name = $"Enemy_{nodeData.id}";

                var collision = enemyObject.GetComponent<EnemyCollision>() ?? enemyObject.AddComponent<EnemyCollision>();
                collision.Init(onHitPlayer);

                _currentEnemies.Add(enemyObject);
            }
        }
    }
}
