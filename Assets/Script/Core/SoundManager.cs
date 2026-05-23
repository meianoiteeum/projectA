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

// Player esta se movendo no MODO MOVIMENTO
       public void SfxMoving()
    {
        sfxManager.PlayOneShot(movingSound);
    }
    
// Player apertou TAB e trocou de modo
        public void SfxSwitchMode()
    {
        sfxManager.PlayOneShot(switchModeSound);
    }

// Player apertou "Q" ou "E" e rotacionou a peca
        public void SfxRotate()
    {
        sfxManager.PlayOneShot(rotateSound);
    }

// Player esta movendo a seta no MODO ROTACAO
       public void SfxSwitchNode()
    {
        sfxManager.PlayOneShot(switchNodeSound);
    }

// Player morreu
           public void SfxDead()
    {
        sfxManager.PlayOneShot(deadSound);
    }

// Player fez algum comando que nao foi permitido (exemplo: tentou rotacionar a peca que nao eh permitida rotacionar)
               public void SfxError()
    {
        sfxManager.PlayOneShot(errorSound);
    }
}
