using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [Header("Butonlar")]
    public Button playButton;   
    public Button deckButton;   
    public Button quitButton;
    [Header("Paneller")]
    public GameObject deckUI;
    public GameObject mainMenuUI;

    

    void Start()
    {
        GameObject playObj = GameObject.Find("PlayButton");
        GameObject deckObj = GameObject.Find("DeckButton");
        GameObject quitObj = GameObject.Find("Quit Button");

        // 1. OYNA BUTONU
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(PlayGame);
        }
        else
        {
            Debug.LogError("HATA: 'Play Button' Inspector'da atanmamış veya kayıp!");
        }

        // 2. DESTE BUTONU
        if (deckButton != null)
        {
            deckButton.onClick.RemoveAllListeners();
            deckButton.onClick.AddListener(OpenDeck);
        }
        else
        {
            Debug.LogWarning("UYARI: 'Deck Button' atanmamış.");
        }

        // 3. ÇIKIŞ BUTONU
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }
    }
    public void PlayGame()
    {
        Time.timeScale = 1f;
        if (PauseManager.isGamePaused)
        {
            PauseManager.isGamePaused = false;
        }
        SceneManager.LoadScene("DemoLevel");
    }
    public void OpenDeck()
    {
        mainMenuUI.SetActive(false);
        deckUI.SetActive(true);
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}