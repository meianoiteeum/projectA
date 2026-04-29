using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements - Tabs")] 
    bool isPaused;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject confirmationTab;
    
    [Header("UI Elements - Buttons")] 
    [SerializeField] Button continueButton;
    [SerializeField] Button menuButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button backButton;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    private void Start()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    private void OnEnable()
    {
        continueButton.onClick.AddListener(OnContinueClick);
        menuButton.onClick.AddListener(OnMenuClick);
        optionsButton.onClick.AddListener(OnOptionsClick);
        quitButton.onClick.AddListener(OnQuitClick);
        backButton.onClick.AddListener(OnBackClick);
        yesButton.onClick.AddListener(OnYesClick);
        noButton.onClick.AddListener(OnNoClick);
    }

    private void OnDisable()
    {
        continueButton.onClick.RemoveListener(OnContinueClick);
        menuButton.onClick.RemoveListener(OnMenuClick);
        optionsButton.onClick.RemoveListener(OnOptionsClick);
        quitButton.onClick.RemoveListener(OnQuitClick);
        backButton.onClick.RemoveListener(OnBackClick);
        yesButton.onClick.RemoveListener(OnYesClick);
        noButton.onClick.RemoveListener(OnNoClick);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    //Função de pausar o jogo
    void PauseGame()
    {
        isPaused = true;
        Debug.Log("Jogo pausado");
        pauseMenu.SetActive(true);
        Time.timeScale = 1;
    }
    
    //Função de despausar o jogo
    void ResumeGame()
    {
        isPaused = false;
        Debug.Log("Jogo despausado");
        pauseMenu.SetActive(false);
        Time.timeScale = 0;

    }

    void OnContinueClick()
    {
        ResumeGame();
    }

    void OnMenuClick()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
        isPaused = false;
        pauseMenu.SetActive(false);
    }

    void OnOptionsClick()
    {
        optionsMenu.SetActive(true);
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
    void OnQuitClick()
    {
        confirmationTab.SetActive(true);
    }

    void OnYesClick()
    {
        Debug.Log("Jogo fechado");
        Application.Quit();
    }

    void OnNoClick()
    {
        confirmationTab.SetActive(false);
    }

    void OnBackClick()
    {
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
}
