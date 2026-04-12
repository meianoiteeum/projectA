using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    //Lista de botões equivalentes a cada level
    [SerializeField]private Button[]levelButtons;
    void Start()
    {
        //Acessa os dados salvos do player no dispositivo
        //Tenta buscar o level atual, caso não encontre nada salvo, assume o valor padrão de 1.
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        //Percorre a lista de botões configuradas anteriormente na array
        for (int level = 0; level < levelButtons.Length; level++)
        {
            //O indice "level" começa em 0, mas os fases(levels do jogo) começam a partir de 1.
            //Se (level + 1) for maior que o nível atual salvo, o botão deve ser bloqueado.
            if (level + 1 > currentLevel)
            {
                // Deixa o botão cinza e impossível de clicar.
                levelButtons[level].interactable = false;
            }
            else
            {
                // Garante que o botão esteja ativo caso o jogador já tenha liberado a fase.
                levelButtons[level].interactable = true;
            }
        }
    }
    
    // Este método deve ser chamado no evento "OnClick()" de cada botão no Inspetor da Unity.
    public void LoadLevel(int numberOfLevel)
    {
        // Carrega a cena pelo nome.
        // Ex: se passar '2', carregará a cena "Level2".
        
        // IMPORTANTE: Atualmente para funcionar os nomes das cenas devem ser "Level" + o número equivalente.
        // Então quando for nomear a cena dos leveis utilizar essa nomenclatura "Level1", "Level2", etc.
        
        // Caso houver alteração nisso favor deixar comentado!
        SceneManager.LoadScene("Level" + numberOfLevel);
        //Para utilizar quando o jogador terminar a fase 1, lá no finalzinho da fase só adicionar a linha de código abaixo
        //para avisar ao jogo que ele agora tem acesso à fase 2

        // Salva que o jogador agora alcançou a Fase 2
        //PlayerPrefs.SetInt("NivelAlcancado", 2);
    }
}
