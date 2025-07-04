using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScenesTransition : MonoBehaviour
{
    [Header("Названия сцен")]
    [SerializeField] private string gameSceneName = "TestIceLevel";
    [SerializeField] private string storySceneName = "Story";
    [SerializeField] private string loadingSceneName = "Loading";
    [SerializeField] private string menuSceneName = "MenuScene";

    [Header("Кнопки")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button returnToMenuButton;
    [SerializeField] private Button restartLevelButton;
    [SerializeField] private Button quitGameButton;

    [Header("Настройки переходов")]
    [SerializeField] private float transitionDelay = 0.1f;
    [SerializeField] private bool showLoadingScreen = false;
    [SerializeField] private GameObject loadingPanel;

    [Header("Настройки перезагрузки")]
    [SerializeField] private bool forceFullReload = true;
    [SerializeField] private bool clearDontDestroyOnLoadObjects = true;

    [Header("Звуковые эффекты (опционально)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;

    private void Start()
    {
        SetupButtons();

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    private void SetupButtons()
    {
        if (startGameButton != null)
            startGameButton.onClick.AddListener(() => LoadSceneWithDelay(storySceneName));

        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(() => LoadSceneWithDelay(menuSceneName));

        if (restartLevelButton != null)
            restartLevelButton.onClick.AddListener(RestartCurrentLevel);

        if (quitGameButton != null)
            quitGameButton.onClick.AddListener(QuitGame);
    }

    private void LoadSceneWithDelay(string sceneName)
    {
        PlayButtonSound();
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        if (showLoadingScreen && loadingPanel != null)
            loadingPanel.SetActive(true);

        yield return new WaitForSeconds(transitionDelay);

        
        if (forceFullReload)
        {
            yield return StartCoroutine(ForceFullReload(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator ForceFullReload(string sceneName)
    {
        
        if (clearDontDestroyOnLoadObjects)
        {
            ClearDontDestroyOnLoadObjects();
        }

        
        System.GC.Collect();
        yield return null;

        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        
        yield return new WaitForEndOfFrame();
        System.GC.Collect();
    }

    private void ClearDontDestroyOnLoadObjects()
    {
        
        GameObject[] dontDestroyObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in dontDestroyObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                
                if (!obj.name.Contains("EventSystem") &&
                    !obj.name.Contains("AudioListener") &&
                    !obj.name.Contains("Main Camera") &&
                    obj != this.gameObject)
                {
                    Debug.Log($"Destroying DontDestroyOnLoad object: {obj.name}");
                    Destroy(obj);
                }
            }
        }
    }

    private void StartGame()
    {
        LoadSceneWithDelay(storySceneName);
    }

    public void ReturnToMenu()
    {
        Debug.Log("return to menu");
        LoadSceneWithDelay(menuSceneName);
    }

    public void RestartCurrentLevel()
    {
        Debug.Log("restart");
        PlayButtonSound();

        StartCoroutine(LoadSceneCoroutine("Loading"));
        StartCoroutine(RestartCurrentScene());
    }

    private IEnumerator RestartCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (showLoadingScreen && loadingPanel != null)
            loadingPanel.SetActive(true);

        yield return new WaitForSeconds(transitionDelay);

        
        if (forceFullReload)
        {
            yield return StartCoroutine(ForceFullReload(currentSceneName));
        }
        else
        {
            SceneManager.LoadScene(currentSceneName);
        }
    }

    public void QuitGame()
    {
        PlayButtonSound();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadScene(string sceneName)
    {
        LoadSceneWithDelay(sceneName);
    }

    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    private bool DoesSceneExist(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameFromPath == sceneName)
                return true;
        }
        return false;
    }

 
    public void OnStartGameButtonClick() => StartGame();
    public void OnReturnToMenuButtonClick() => ReturnToMenu();
    public void OnRestartLevelButtonClick() => RestartCurrentLevel();
    public void OnQuitGameButtonClick() => QuitGame();

    private void OnDestroy()
    {
        if (startGameButton != null)
            startGameButton.onClick.RemoveAllListeners();

        if (returnToMenuButton != null)
            returnToMenuButton.onClick.RemoveAllListeners();

        if (restartLevelButton != null)
            restartLevelButton.onClick.RemoveAllListeners();

        if (quitGameButton != null)
            quitGameButton.onClick.RemoveAllListeners();
    }

#if UNITY_EDITOR
    [ContextMenu("Test Start Game")]
    private void TestStartGame()
    {
        Debug.Log($"Attempting to load scene: {gameSceneName}");
        if (DoesSceneExist(gameSceneName))
        {
            StartGame();
        }
        else
        {
            Debug.LogError($"Scene '{gameSceneName}' not found in Build Settings!");
        }
    }

    [ContextMenu("Clear DontDestroyOnLoad Objects")]
    private void TestClearDontDestroyOnLoad()
    {
        ClearDontDestroyOnLoadObjects();
    }
#endif
}