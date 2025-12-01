using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    [Header("Configuração de UI de Transição")]
    public Animator transitionAnimator; 
    public float transitionTime = 1f; 

    private const string TRANSITION_IN_TRIGGER = "StartTransition"; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(GameScene scene)
    {
        StartCoroutine(LoadSceneWithTransition(scene));
    }

    private IEnumerator LoadSceneWithTransition(GameScene scene)
    {
        // 1. Fade Out
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger(TRANSITION_IN_TRIGGER); 
            yield return new WaitForSeconds(transitionTime); 
        }

        // 2. Carregar Cena
        string sceneName = scene.ToString();
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            yield return null;
        }

        // 3. Fade In (Pequeno delay para estabilidade)
        yield return new WaitForSeconds(0.5f);
    }

    // --- CONTROLE DE FLUXO (ATUALIZADO PARA 3 FASES) ---
    
    public void StartNewGame()
    {
        LoadScene(GameScene.IntroPrologo);
    }
    
    public void AdvanceToNextPhase(GameScene currentPhase)
    {
        GameScene nextScene = GameScene.MenuInicial;

        switch (currentPhase)
        {
            case GameScene.MenuInicial:
                nextScene = GameScene.IntroPrologo;
                break;
            
            // --- DE FASE JOGÁVEL PARA INTRO ---
            
            // Do Prólogo para o Dia 1
            case GameScene.FasePrologo:
                nextScene = GameScene.IntroDia1;
                break;

            // Do Dia 1 para o Dia 2
            case GameScene.FaseDia1_Porto:
                nextScene = GameScene.IntroDia2;
                break;

            // Do Dia 2 para o FIM DE JOGO (Volta ao Menu)
            case GameScene.FaseDia2_Serra:
                Debug.Log("Jogo Finalizado! Voltando ao Menu.");
                nextScene = GameScene.MenuInicial; 
                break;
            
            // --- DE INTRO PARA FASE JOGÁVEL ---
            
            case GameScene.IntroPrologo:
                nextScene = GameScene.FasePrologo;
                break;
            case GameScene.IntroDia1:
                nextScene = GameScene.FaseDia1_Porto;
                break;
            case GameScene.IntroDia2:
                nextScene = GameScene.FaseDia2_Serra;
                break;
            
            default:
                Debug.LogError("Cena desconhecida ou não mapeada: " + currentPhase);
                return;
        }
        
        LoadScene(nextScene);
    }
}