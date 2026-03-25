using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Util
{
    public class MainMenuManager : MonoBehaviour
{
    [Header("Configurações de Cena")]
    [Tooltip("Nome exato da cena do jogo para carregar.")]
    [SerializeField] private string gameSceneName = "SampleScene";

    [Header("Gerenciamento de Telas")]
    [Tooltip("Arraste o painel principal do menu aqui para abri-lo ao iniciar.")]
    [SerializeField] private GameObject initialMenu;
    
    [Tooltip("Coloque TODOS os painéis de menu aqui (Principal, Opções, Créditos, etc).")]
    [SerializeField] private GameObject[] allMenus;

    [Header("Links Externos")]
    [SerializeField] private string itchIoUrl = "https://paginaDojogo.itch.io/";
    [SerializeField] private string linkTreeUrl = "https://linktr.ee/perfilDoStudio";

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
        Debug.Log("Carregando a cena de teste: " + "SampleScene");
        SceneManager.LoadScene("SampleScene");
    }
    //Método para sair do jogo
    public void QuitGame()
    {
        Debug.Log("Fechando o jogo...");
        Application.Quit();
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
}
}