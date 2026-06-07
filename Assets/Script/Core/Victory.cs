using System;
using UnityEngine;

public class Victory : MonoBehaviour
{
    [SerializeField] EndLevel endLevelScript;
    [SerializeField] Transform triggerPos;
    [SerializeField] float triggerSize;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Time.timeScale = 0;
            endLevelScript.enabled = true;
            endLevelScript.PlayerWin();
        }
    }

        void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(triggerPos.position,triggerSize);
    }
        
}
