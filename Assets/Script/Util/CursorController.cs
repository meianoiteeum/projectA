using System.Numerics;
using UnityEngine;

public class CursorController : MonoBehaviour
{
void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        UnityEngine.Vector2 cursorPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        transform.position = cursorPos;
    }

}
