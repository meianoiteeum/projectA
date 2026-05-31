using UnityEngine;

public class HUD_AnimationController : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI _textMode;
    [SerializeField] TMPro.TextMeshProUGUI _textTab;
    [SerializeField] Animation hudAnimation;
    [SerializeField] private GameObject rotateButtons;
    [SerializeField]  private GameObject moveButtons;
    
    // Variável para controlar qual texto deve aparecer
    private bool _isRotationMode;

    void Start()
    {
        moveButtons.SetActive(true);
        rotateButtons.SetActive(false);
        _textMode.text = "MOVE MODE";
        _textTab.text = "TAB";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Inverte o estado atual
            _isRotationMode = !_isRotationMode;

            // Garante que a animação comece do início, mesmo se já estiver tocando
            hudAnimation.Rewind(); 
            hudAnimation.Play();
        }
    }

    // Este método continua sendo chamado pelo Animation Event no frame da rotação
    public void ChangeTextEvent()
    {
        if (_isRotationMode)
        {
            _textMode.text = "ROTATE MODE";
            _textTab.text = "TAB";
            rotateButtons.SetActive(true);
            moveButtons.SetActive(false);
        }
        else
        {
            _textMode.text = "MOVE MODE";
            _textTab.text = "TAB";
            rotateButtons.SetActive(false);
            moveButtons.SetActive(true);
        }
    }
}