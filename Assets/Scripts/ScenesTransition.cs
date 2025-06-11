using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesTransition : MonoBehaviour
{
    private string gameSceneName = "PlayerScene";
    private string menuSceneName = "MenuScene";

    [SerializeField] private Button startGameButton;

    private void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
