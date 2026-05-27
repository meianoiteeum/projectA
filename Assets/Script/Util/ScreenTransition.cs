using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenTransition : MonoBehaviour
{
    [SerializeField] private Animator anim;
    string sceneNameToLoad;

    public void LoadScene(string sceneName)
    {
        anim.Play("FadeOut");
        sceneNameToLoad = sceneName;
        Invoke(nameof(LoadSceneAfter), anim.GetCurrentAnimatorStateInfo(0).length);
    }
    void LoadSceneAfter()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }
}
