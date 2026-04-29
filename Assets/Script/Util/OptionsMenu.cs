using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class OptionsMenu : MonoBehaviour
{
    //Titulos para organização no Inspector
    [Header("UI Elements - Tabs")] 
    [SerializeField] private GameObject[] tabsContent; // Um Array (lista) com os painéis (GameObject) de cada aba (Ex: Painel de Áudio, Painel de Gráficos).
    [SerializeField] private TMP_Text[] tabsText; // Um Array com os Textos dos títulos das abas.
    
    // Cores para mostrar qual aba está selecionada. O padrão é magenta para a ativa e cinza para as outras.
    [SerializeField] private Color activeTabColor = Color.magenta; 
    [SerializeField] private Color inactiveTabColor = Color.gray; 
    
    // Guarda o número (índice) da aba que o jogador está vendo agora. Começa em 0 (a primeira aba).
    private int currentTab;

    [Header("UI Elements - Texts")] 
    [SerializeField] private TMP_Text screenRes; //O texto na tela que vai mostrar a resolução atual (Ex: "1920 x 1080 (60) Hz").
    
    [Header("UI Elements - Toggles")]
    [SerializeField] private Toggle toggleFullscreen; ///A caixinha de "check" (V) para ligar e desligar a tela cheia.
    
    [Header("UI Elements - Sliders")]
    // As barras deslizantes de volume (UI).
    [SerializeField] private Slider masterVolumeSlider; 
    [SerializeField] private Slider musicVolumeSlider;  
    [SerializeField] private Slider sfxVolumeSlider; 
    
    // Variáveis internas
    //Uma lista com todas as resoluções que o monitor do jogador suporta
    private Resolution[] availableScreenResolutions;
    //Qual o número da resolução que está selecionada agora
    private int currentResolutionIndex;
    
    [Header("Audio Settings")]
    //O painel de controle mestre de áudio da Unity.
    [SerializeField] AudioMixer audioMixer;

    private void Awake()
    {
        //Função que garante que assim que o jogo carregar, as abas sejam atualizadas para mostrar a aba 0.
        UpdateTabsVisual();
    }

    void Start()
    {
        //Função que descobre quais resoluções o PC do jogador aguenta.
        StartScreenResolution();
        
        //Verifica se o jogo já está em tela cheia e marca/desmarca a caixinha do Toggle para ficar certinho.
        toggleFullscreen.isOn = Screen.fullScreen;
        
        //Carrega e aplica os volumes que o jogador salvou da última vez que jogou.
        StartAudioSettings();
    }

    private void Update()
    {
        MudarAba();
    }

    // --- Tabs System ---
    
    //Função chamada para mudar a aba. O 'direction' será 1 (direita) ou -1 (esquerda).
    private void ChangeTab(int direction)
    {
        //Soma a direção à aba atual.
        currentTab += direction;
        
        // Loop: se o jogador apertar para a esquerda estando na primeira aba (0)
        if (currentTab < 0)
        {
            //ele vai para a última aba da lista.
            currentTab = tabsContent.Length - 1;
        }
        //E se ele apertar para a direita estando na última aba
        else if (currentTab >= tabsContent.Length)
        {
            //ele volta para a primeira aba (0).
            currentTab = 0;
        }
        //Depois de fazer a matemática, atualiza o visual.
        UpdateTabsVisual();
    }
    
    // Atualiza a UI das Abas
    private void UpdateTabsVisual()
    {
        // Loop: liga o conteúdo da aba atual e desliga os outros. E muda a cor do texto de titulo lá em cima.
        for (int tab = 0; tab < tabsContent.Length; tab++)
        {
            // Se o número da aba que o 'for' está checando for igual à aba que queremos ver (currentTab),
            // ele liga o painel (SetActive true). Se for diferente, ele desliga (SetActive false).
            tabsContent[tab].SetActive(tab == currentTab);
            // Faz a mesma coisa com a cor do texto: se for a aba atual, pinta de magenta, senão, pinta de cinza.
            tabsText[tab].color = (tab == currentTab) ? activeTabColor : inactiveTabColor;
        }   
    }
    
    // --- Screen Res Function ---
    private void StartScreenResolution()
    {
        //Pega do sistema operacional (Windows/Mac) todas as resoluções que o monitor atual aceita.
        availableScreenResolutions = Screen.resolutions;
        
        //Passa por todas essas resoluções uma por uma
        for (int res = 0; res < availableScreenResolutions.Length; res++)
        {
            //e checa: A largura, altura e taxa de quadros (Hz) dessa resolução é igual à que o jogo está rodando agora?
            if (availableScreenResolutions[res].width == Screen.currentResolution.width &&
                availableScreenResolutions[res].height == Screen.currentResolution.height && 
                availableScreenResolutions[res].refreshRateRatio.value == Screen.currentResolution.refreshRateRatio.value) 
            {
                //Se achou, salva o número (índice) dela e para de procurar ('break').
                currentResolutionIndex = res;
                break;
            }
        }
        //Atualiza o textinho na tela para o jogador ver a resolução atual.
        UpdateResolutionText();
    }

    //Função que o botão de trocar resolução chama setas na interface
    public void ChangeResolution(int direction)
    {
        //Igualzinho ao sistema de abas: vai para frente ou para trás na lista de resoluções.
        currentResolutionIndex += direction;
        if (currentResolutionIndex < 0)
        {
            currentResolutionIndex = availableScreenResolutions.Length - 1;
        }
        else if (currentResolutionIndex >= availableScreenResolutions.Length)
        {
            currentResolutionIndex = 0;
        }
        
        UpdateResolutionText();
        
        //Pega a nova resolução escolhida e manda a Unity aplicá-la de verdade no monitor.
        Resolution resolution = availableScreenResolutions[currentResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
    }
    
    //Função que escreve a resolução no formato "Largura x Altura (Hz) Hz".
    private void UpdateResolutionText()
    {
        Resolution res = availableScreenResolutions[currentResolutionIndex];
        
        //Transforma a taxa de atualização (ex: 59.94) em um número inteiro arredondado (60).
        int refreshRate = Mathf.RoundToInt((float)res.refreshRateRatio.value);
        
        //Junta todos os textos e números numa frase só.
        screenRes.text = res.width + " x " + res.height + " (" + refreshRate +") " + "Hz";
    }
    
    // --- Function Fullscreen ---
    
    //Função chamada quando o jogador clica na caixinha (Toggle) de Tela Cheia.
    public void SetFullscreen(bool isFullscreen)
    {
        //Aplica a tela cheia.    
        Debug.Log("Ativou Fullscreen");
        Screen.fullScreen = isFullscreen;
    }
    
    // --- AUDIO SYSTEM ---
    
    //Função de Controle do Audio
    private void StartAudioSettings()
    {
        //'PlayerPrefs' é a "memória do jogo". Ele procura se o jogador já salvou algum volume antes.
        //Se não achar nada (é a primeira vez jogando), ele usa o valor padrão '1f' (volume máximo).
        //É importante que os Sliders na Unity estejam configurados de 0.0001 a 1.
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        //Atualiza a posição da "bolinha" nos Sliders da interface para combinar com o volume salvo.
        masterVolumeSlider.value = masterVol;
        musicVolumeSlider.value = musicVol;
        sfxVolumeSlider.value = sfxVol;

        //Aplica o volume no Mixer.
        SetMasterVolume(masterVol);
        SetMusicVolume(musicVol);
        SetSfxVolume(sfxVol);
    }

    //Importante: Chamar essa função no evento OnValueChanged do Slider de Master
    public void SetMasterVolume(float volume)
    {
        // O AudioMixer usa decibéis (dB), que vai de -80 (mudo) a 0 (volume máximo) ou 20 (muito alto).
        // Usar Log10 converte o valor do Slider (de 0.0001 a 1) para a curva correta em dB.
        audioMixer.SetFloat("MasterParam", Mathf.Log10(volume) * 20);
        //Salva preferencia do jogador no computador dele.
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    // Chamar essa função no evento OnValueChanged do Slider de Music
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicParam", Mathf.Log10(volume) * 20);
        //Salva preferencia do jogador no computador dele.
        PlayerPrefs.SetFloat("MusicVolume", volume); 
    }

    // Chamar essa função no evento OnValueChanged do Slider de SFX
    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("SFXParam", Mathf.Log10(volume) * 20);
        //Salva preferencia do jogador no computador dele.
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
    
    // --- Input System ---
    
    // A função que é chamada quando o jogador aperta os botões mapeados.
     void MudarAba()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeTab(-1); // Muda para a aba anterior.
        }
        // Se o valor X for positivo, significa que apertou Right (E)   
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeTab(1); // Muda para a próxima aba.
        }
    }
}