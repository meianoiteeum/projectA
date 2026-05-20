using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script.Util
{
    public class MainMenuManager : MonoBehaviour
{
    [Header("Configurações de Cena")]
    [Tooltip("Nome exato da cena do jogo para carregar.")]
    [SerializeField] private string gameSceneName = "Animation";
    [SerializeField] private string creditsSceneName = "Credits";

    [Header("Gerenciamento de Telas")]
    [Tooltip("Arraste o painel principal do menu aqui para abri-lo ao iniciar.")]
    [SerializeField] private GameObject initialMenu;
    
    [Tooltip("Coloque TODOS os painéis de menu aqui (Principal, Opções, etc).")]
    [SerializeField] private GameObject[] allMenus;

    [Header("Links Externos")]
    [SerializeField] private string itchIoUrl = "https://paginaDojogo.itch.io/";
    [SerializeField] private string linkTreeUrl = "https://linktr.ee/perfilDoStudio";
    
    [Header("Buttons")]
    [Tooltip("Arraste os botões")]
    [SerializeField] Button playGameButton;
    [SerializeField]Button quitGameButton;
    [SerializeField]Button creditsButton;
    
    private void Start()
    {
        // Inicia o jogo sempre no menu principal
        if (initialMenu != null)
        {
            OpenMenu(initialMenu);
        }
    }
    //Método para abrir sub - menus (opções e créditos)
    public void OpenMenu(GameObject menuToOpen)
    {
        foreach (GameObject menu in allMenus)
        {
            if (menu != null)
            {
                menu.SetActive(menu == menuToOpen);
            }
        }
    }
    //Método para carregar a cena de gameplay
    public void PlayGame()
    {
        Debug.Log("Carregando a cena de teste: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    } 
    //Método para sair do jogo
    public void QuitGame()
    {
        Debug.Log("Fechando o jogo...");
        Application.Quit();
    }

    public void OpenCredits()
    {
        Debug.Log("Indo para os creditos...");
        SceneManager.LoadScene(creditsSceneName);
    }
    //Lista de links externos
    public enum SocialPlatform { ItchIo, LinkTree }

    public void OpenSocialLink(SocialPlatform platform)
    {
        string urlToOpen = "";

        switch (platform)
        {
            case SocialPlatform.ItchIo:
                urlToOpen = itchIoUrl;
                break;
            case SocialPlatform.LinkTree:
                urlToOpen = linkTreeUrl;
                break;
        }
        //Verifica se o link está preenchido
        if (!string.IsNullOrEmpty(urlToOpen))
        {
            Application.OpenURL(urlToOpen);
        }
        else
        {
            Debug.LogWarning("O link para " + platform.ToString() + " está vazio no Inspector!");
        }
    }

    private void OnEnable()
    {
        creditsButton.onClick.AddListener(OpenCredits);
        playGameButton.onClick.AddListener(PlayGame);
        quitGameButton.onClick.AddListener(QuitGame);
    }

    private void OnDisable()
    {
        creditsButton.onClick.RemoveListener(OpenCredits);
        playGameButton.onClick.RemoveListener(PlayGame);
        quitGameButton.onClick.RemoveListener(QuitGame);
    }
}
}