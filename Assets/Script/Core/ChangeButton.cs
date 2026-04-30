using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class ChangeButton : MonoBehaviour
{
    [SerializeField] 
    private KeyCode button;

    [SerializeField] 
    private Image image;
    
    [SerializeField]
    private Sprite spritePress;
    
    [SerializeField]
    private Sprite spriteUp;
    void Update()
    {
        if (Input.GetKeyDown(button))
        {
            image.sprite = spritePress;
        }else if (Input.GetKeyUp(button))
        {
            image.sprite = spriteUp;
        }
    }
}
