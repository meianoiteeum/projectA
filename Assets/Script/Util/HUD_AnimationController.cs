using UnityEngine;

public class HUD_AnimationController : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI _textMode;
    [SerializeField] TMPro.TextMeshProUGUI _textTab;
    [SerializeField] Animation hudAnimation;
    [SerializeField] private GameObject buttons;
    
    // Variável para controlar qual texto deve aparecer
    private bool _isPreparationMode = false;

    void Start()
    {
        buttons.SetActive(false);
        _textMode.text = "GAME MODE";
        _textTab.text = "TAB";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Inverte o estado atual
            _isPreparationMode = !_isPreparationMode;

            // Garante que a animação comece do início, mesmo se já estiver tocando
            hudAnimation.Rewind(); 
            hudAnimation.Play();
        }
    }

    // Este método continua sendo chamado pelo Animation Event no frame da rotação
    public void ChangeTextEvent()
    {
        if (_isPreparationMode)
        {
            _textMode.text = "PREPARATION MODE";
            _textTab.text = "TAB";
            buttons.SetActive(true);
        }
        else
        {
            _textMode.text = "GAME MODE";
            _textTab.text = "TAB";
            buttons.SetActive(false);
        }
    }
}