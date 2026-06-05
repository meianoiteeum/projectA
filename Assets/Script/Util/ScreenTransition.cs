using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenTransition : MonoBehaviour
{
    [SerializeField] private Animator anim;
    string sceneNameToLoad;
    
    // Propriedade para checar se a transição está acontecendo
    public bool IsTransitioning { get; private set; }

    private void Start()
    {
        IsTransitioning = true;
        anim.Update(0); 
        float fadeInDuration = anim.GetCurrentAnimatorStateInfo(0).length;
        Invoke(nameof(EndTransition), fadeInDuration);
    }

    public void LoadScene(string sceneName)
    {
        if (IsTransitioning) return; 

        IsTransitioning = true; 
        
        anim.Play("FadeOut");
        sceneNameToLoad = sceneName;
        Invoke(nameof(LoadSceneAfter), anim.GetCurrentAnimatorStateInfo(0).length);
    }
    
    void LoadSceneAfter()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }
    
    void EndTransition()
    {
        IsTransitioning = false;
    }
}