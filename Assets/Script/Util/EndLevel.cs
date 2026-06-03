using System;
using UnityEngine;
using UnityEngine.UI;

public class EndLevel : MonoBehaviour
{
    [SerializeField] Button creditsButton;
    [SerializeField] Button backToMainMenuButton;
    [SerializeField] Button nextLevelButton;
    [SerializeField] ScreenTransition screenTransition;
    [SerializeField] GameObject victoryPanel;
    
    [SerializeField] bool isFinalLevel;
    [SerializeField]float currentLevelIndex = 1f;
    
    public void PlayerWin()
    {
        victoryPanel.SetActive(true);
        if (isFinalLevel)
        {
            //É a fase final: Mostra os créditos, esconde o botão de próxima fase
            creditsButton.gameObject.SetActive(true);
            nextLevelButton.gameObject.SetActive(false);
            
            //Salva que o jogo foi zerado
            GameSave.SaveLevel("GameCompleted", 1);
        }
        else
        {
            //Não é a fase final: Esconde os créditos, mostra o botão de próxima fase
            creditsButton.gameObject.SetActive(false);
            nextLevelButton.gameObject.SetActive(true);
            
            //Salva a liberação da PRÓXIMA fase
            GameSave.SaveLevel("UnlockedLevels", currentLevelIndex + 1);
        }
    }
    void BackToMainMenu()
    {
        screenTransition.LoadScene("MainMenu");
        victoryPanel.SetActive(false);
    }

    void GoToCredits()
    {
        screenTransition.LoadScene("Créditos");
        victoryPanel.SetActive(false);
    }

    public void GoToNextLevel()
    {
        victoryPanel.SetActive(false);
        float nextLevelIndex = GameSave.LoadLevel("UnlockedLevels", currentLevelIndex + 1);
        string nextLevelName = "Level" + nextLevelIndex;
        screenTransition.LoadScene(nextLevelName);
        Debug.Log($"Carregando próximo Level: {nextLevelName}");
    }
    private void OnEnable()
    {
        creditsButton.onClick.AddListener(GoToCredits);
        backToMainMenuButton.onClick.AddListener(BackToMainMenu);
        nextLevelButton.onClick.AddListener(GoToNextLevel);
    }

    private void OnDisable()
    {
        creditsButton.onClick.RemoveListener(GoToCredits);
        backToMainMenuButton.onClick.RemoveListener(BackToMainMenu);
        nextLevelButton.onClick.RemoveListener(GoToNextLevel);
    }
}
