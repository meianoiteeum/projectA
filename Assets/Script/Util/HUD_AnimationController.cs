using UnityEngine;

public class HUD_AnimationController : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI _text;
    [SerializeField] Animation hudAnimation;
    [SerializeField] private GameObject buttons;
    
    // Variável para controlar qual texto deve aparecer
    private bool _isPreparationMode = false;

    void Start()
    {
        buttons.SetActive(false);
        _text.text = "GAME MODE";
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
            _text.text = "PREPARATION MODE";
            buttons.SetActive(true);
        }
        else
        {
            _text.text = "GAME MODE";
            buttons.SetActive(false);
        }
    }
}