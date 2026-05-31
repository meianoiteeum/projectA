using System;
using UnityEngine;
using UnityEngine.UI;

public class EndLevel : MonoBehaviour
{
    [SerializeField] Button creditsButton;
    [SerializeField] Button backToMainMenuButton;
    [SerializeField] ScreenTransition screenTransition;
    [SerializeField] GameObject victoryPanel;

    public void PlayerWin()
    {
        victoryPanel.SetActive(true);
    }
    void BackToMainMenu()
    {
        screenTransition.LoadScene("MainMenu");
        victoryPanel.SetActive(false);
    }

    void GoToCredits()
    {
        screenTransition.LoadScene("Credits");
        victoryPanel.SetActive(false);
    }
    private void OnEnable()
    {
        creditsButton.onClick.AddListener(GoToCredits);
        backToMainMenuButton.onClick.AddListener(BackToMainMenu);
    }

    private void OnDisable()
    {
        creditsButton.onClick.RemoveListener(GoToCredits);
        backToMainMenuButton.onClick.RemoveListener(BackToMainMenu);
    }
}
