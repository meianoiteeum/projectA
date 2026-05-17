using System.Numerics;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    static CursorController instance;
    [SerializeField] RectTransform tr;
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        tr.anchoredPosition = Input.mousePosition;
    }

}
