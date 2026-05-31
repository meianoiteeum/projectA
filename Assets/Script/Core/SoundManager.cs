using Script.Core;
using Script.Gameplay.Characters;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource sfxManager;
    [SerializeField] AudioClip movingSound;
    [SerializeField] AudioClip switchModeSound;
    [SerializeField] AudioClip rotateSound;
    [SerializeField] AudioClip switchNodeSound;
    [SerializeField] AudioClip deadSound;
    [SerializeField] AudioClip errorSound;

    private void OnEnable()  => MapEvents.OnGameStateChanged += HandleGameState;
    private void OnDisable() => MapEvents.OnGameStateChanged -= HandleGameState;

    private void HandleGameState(GameState state)
    {
        switch (state)
        {
            case GameState.MOVIMENTACAO:      SfxMoving();     break;
            case GameState.SWITCH:            SfxSwitchMode(); break;
            case GameState.ROTACAO:           SfxRotate();     break;
            case GameState.MOVIMENTACAO_NODE: SfxSwitchNode(); break;
            case GameState.MORTE:             SfxDead();       break;
            case GameState.ERRO:              SfxError();      break;
        }
    }

// Player esta se movendo no MODO MOVIMENTO
    public void SfxMoving(){
        Debug.Log("SfxMoving()");
        sfxManager.PlayOneShot(movingSound);
    }

// Player apertou TAB e trocou de modo
    public void SfxSwitchMode(){
        Debug.Log("SfxSwitchMode()");
        sfxManager.PlayOneShot(switchModeSound);
    }

// Player apertou "Q" ou "E" e rotacionou o node
    public void SfxRotate(){
        Debug.Log("SfxRotate()");
        sfxManager.PlayOneShot(rotateSound);
    }

// Player esta movendo o node
    public void SfxSwitchNode(){
        Debug.Log("SfxSwitchNode()");
        sfxManager.PlayOneShot(switchNodeSound);
    }

// Player morreu
    public void SfxDead(){
        Debug.Log("SfxDead()");
        sfxManager.PlayOneShot(deadSound);
    }

// Player fez algum comando que nao foi permitido (exemplo: tentou rotacionar a peca que nao eh permitida rotacionar)
    public void SfxError(){
        Debug.Log("SfxError()");
        sfxManager.PlayOneShot(errorSound);
    }
}
