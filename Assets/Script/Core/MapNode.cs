using System;
using UnityEngine;

namespace Script.Core
{
    public class MapNode : MonoBehaviour
    {
        [Header("Sprites por Tipo (pixel art)")]
        public Sprite spriteStart;
        public Sprite spriteCombat;
        public Sprite spriteShop;
        public Sprite spriteRest;
        public Sprite spriteTreasure;
        public Sprite spriteEnd;
        public Sprite spriteNormal;

        [Header("Colors (fallback sem sprite)")]
        public Color colorStart    = new Color(0.2f, 0.9f, 0.2f);
        public Color colorCombat   = new Color(0.9f, 0.2f, 0.2f);
        public Color colorShop     = new Color(0.9f, 0.8f, 0.1f);
        public Color colorRest     = new Color(0.3f, 0.6f, 1.0f);
        public Color colorTreasure = new Color(1.0f, 0.6f, 0.0f);
        public Color colorEnd      = new Color(0.8f, 0.2f, 0.9f);
        public Color colorNormal = new Color(1f,1f,1f);
        
        public NodeData Data {get; private set;}

        private SpriteRenderer _sr;
        private Color _originalColor;

        public void Init(NodeData data)
        {
            Debug.Log($"Nó {data.label} — tipo recebido: {data.type}");
            Data = data;

            _sr = GetComponent<SpriteRenderer>();
            if (_sr == null)
            {
                Debug.LogError($"SpriteRenderer não encontrado no prefab! Nó: {data.label}");
                return;
            }

            (_sr.sprite, _sr.color) = data.type switch
            {
                NodeType.Start    => (spriteStart,    colorStart),
                NodeType.Normal   => (spriteNormal,   colorNormal),
                NodeType.Combat   => (spriteCombat,   colorCombat),
                NodeType.Shop     => (spriteShop,     colorShop),
                NodeType.Rest     => (spriteRest,     colorRest),
                NodeType.Treasure => (spriteTreasure, colorTreasure),
                NodeType.End      => (spriteEnd,      colorEnd),
                _ => (_sr.sprite, Color.blue),
            };

            _originalColor = _sr.color;

            var tmp = GetComponentInChildren<TMPro.TMP_Text>();
            if (tmp != null) tmp.text = data.label;
        }

        public void Highlight()
        {
            if (_sr != null) _sr.color = new Color(1f, 0.92f, 0.016f);
        }

        public void Unhighlight()
        {
            if (_sr != null) _sr.color = _originalColor;
        }
    }
}