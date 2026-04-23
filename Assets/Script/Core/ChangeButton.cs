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
    
    // InputSystem.onEvent
    //     .ForDevice<Keyboard>()
    //     .Where(e => e.HasButtonPress())
    //     .CallOnce(act =>
    //     {
    //         foreach (var control in act.EnumerateChangedControls())
    //         {
    //             if (control.name.Equals(button.ToString().ToLower()))
    //             {
    //                 image.sprite = spritePress;
    //             }
    //         }
    //     });
    //
    // InputSystem.onEvent
    //     .ForDevice<Keyboard>()
    //     .Where(e => !e.HasButtonPress())
    //     .CallOnce(act =>
    //     {
    //         foreach (var control in act.EnumerateChangedControls())
    //         {
    //             if (control.name.Equals(button.ToString().ToLower()))
    //             {
    //                 image.sprite = spriteUp;
    //             }
    //         }
    //     });
    }
}
