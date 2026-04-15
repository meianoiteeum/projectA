using UnityEngine;
using UnityEngine.UI;

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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
