using System;
using UnityEngine;

namespace Script.Gameplay.Enemies
{
    public class EnemyCollision : MonoBehaviour
    {
        private Action _onHitPlayer;

        public void Init(Action onHitPlayer)
        {
            _onHitPlayer = onHitPlayer;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                _onHitPlayer?.Invoke();
        }
    }
}
