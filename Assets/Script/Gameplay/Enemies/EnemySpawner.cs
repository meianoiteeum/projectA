using System;
using System.Collections.Generic;
using Script.Core;
using UnityEngine;

namespace Script.Gameplay.Enemies
{
    public class EnemySpawner
    {
        private readonly List<GameObject> _currentEnemies = new();

        public void Spawn(MapData mapData, MapBuilder mapBuilder, Action onHitPlayer)
        {
            foreach (var enemy in _currentEnemies)
                if (enemy != null) UnityEngine.Object.Destroy(enemy);
            _currentEnemies.Clear();

            foreach (var nodeData in mapData.Nodes)
            {
                if (string.IsNullOrEmpty(nodeData.enemie)) continue;

                var prefab = Resources.Load<GameObject>(nodeData.enemie);
                if (prefab == null)
                {
                    Debug.LogWarning($"[EnemySpawner] Prefab '{nodeData.enemie}' não encontrado em Resources.");
                    continue;
                }

                var nodePos = mapBuilder.Nodes[nodeData.id].transform.position;
                var enemyObject = UnityEngine.Object.Instantiate(prefab, nodePos, Quaternion.identity);
                enemyObject.name = $"Enemy_{nodeData.id}_{nodeData.enemie}";

                var collision = enemyObject.GetComponent<EnemyCollision>() ?? enemyObject.AddComponent<EnemyCollision>();
                collision.Init(onHitPlayer);

                var salamander = enemyObject.GetComponent<Salamander>();
                if (salamander != null)
                    salamander.Init(nodeData.id, mapBuilder, mapData);

                _currentEnemies.Add(enemyObject);
            }
        }
    }
}
