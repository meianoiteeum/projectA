using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
   [SerializeField] RectTransform creditsPanel;
   [SerializeField] CanvasScaler canvasScaler;
   [Range(0.1f, 2)][SerializeField] float screenPerSecond = 0.1f;
   [SerializeField] private int margin = 40;
   float screenHeight;
   Vector2 creditsPosition;
   bool isPaused = false;
   
    IEnumerator Start()
    {
        screenHeight = canvasScaler.referenceResolution.y;
        WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
        creditsPosition = new Vector2(0, -screenHeight * 0.5f);
        creditsPanel.localPosition = creditsPosition;
        while (true)
        {
            yield return endOfFrame;
            if (isPaused)
                continue;
            creditsPosition.y += screenHeight * screenPerSecond * Time.deltaTime;
            creditsPanel.localPosition = creditsPosition;
            if (creditsPanel.anchoredPosition.y + margin > creditsPanel.sizeDelta.y)
            {
                EndCredits();
                break;
            }
        }
    }
    void Update()
    {
        //To do New Input System Version & stop game time for animations
        if (Input.anyKeyDown)
        {
            isPaused = !isPaused;
            
            //Make this being a hold key down for 3 seconds count logic
            if (Input.GetKey(KeyCode.Escape))
            {
                EndCredits();
            }
        }
    }

    void EndCredits()
    {
        Debug.Log("Ending credits");
        //TO DO Fade Out to Menu
        SceneManager.LoadScene("MainMenu");

    }
}
