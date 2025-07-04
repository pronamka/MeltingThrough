using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PictureSwitcher : MonoBehaviour
{
    public Sprite[] pictures;
    public Image displayImage;
    public string nextSceneName = "TestIceLevel";
    public TMP_Text uiText;
    private int currentIndex = 0;

    void Start()
    {
        if (pictures.Length > 0 && displayImage != null)
        {
            displayImage.sprite = pictures[0];
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentIndex == pictures.Length - 2)
            {
                if (uiText != null)
                {
                    uiText.text = "";
                }
                ShowNextPicture();
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                ShowNextPicture();
            }
        }
    }

    void ShowNextPicture()
    {
        if (pictures.Length == 0) return;
        currentIndex++;
        if (currentIndex < pictures.Length)
        {
            displayImage.sprite = pictures[currentIndex];
        }
    }
}