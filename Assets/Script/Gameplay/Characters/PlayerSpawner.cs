using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.Core
{
    public class PlayerSpawner
    {
        public void Spawn(MapData mapData, IReadOnlyDictionary<int, MapNode> nodes,
                          GameObject playerPrefab, MapBuilder mapBuilder)
        {
            var startNode = mapData.Nodes.FirstOrDefault(n => n.type == NodeType.Start);
            if (startNode == null)
            {
                Debug.LogWarning("[PlayerSpawner] Nó de início não encontrado.");
                return;
            }

            Vector3 pos = nodes[startNode.id].transform.position;

            GameObject player;
            if (playerPrefab != null)
            {
                player = Object.Instantiate(playerPrefab, pos, Quaternion.identity);
            }
            else
            {
                player = new GameObject();
                player.AddComponent<SpriteRenderer>();

                var tex = new Texture2D(32, 32);
                var pixels = new Color[32 * 32];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
                tex.SetPixels(pixels);
                tex.Apply();

                var sr = player.GetComponent<SpriteRenderer>();
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
                sr.color = new Color(0.53f, 0.81f, 0.98f);
                sr.sortingOrder = 100;

                player.transform.position = pos;
            }

            if (player.GetComponent<PlayerController>() == null)
                player.AddComponent<PlayerController>();

            var pc = player.GetComponent<PlayerController>();
            pc.Init(startNode.id, mapBuilder, mapData.Connections);
            player.name = "Player";

            var cam = Camera.main;
            if (cam != null && cam.GetComponent<CameraFollow>() == null)
                cam.gameObject.AddComponent<CameraFollow>();

            MapEvents.OnPlayerSpawned?.Invoke(player.transform);
        }
    }
}
