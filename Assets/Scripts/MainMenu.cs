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
    public DeckMenuManager deckMenuManager;
    public GameObject PerkUI;
    public GameObject settingsUI;
    public GameObject ClickBlock;

    void Start()
    {
        GameObject playObj = GameObject.Find("PlayButton");
        GameObject deckObj = GameObject.Find("DeckButton");
        GameObject quitObj = GameObject.Find("Quit Button");

        
        if (AudioManager.Instance != null)
        {
        AudioManager.Instance.PlayMenuMusic();
        }
        
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(PlayGame);
        }
        else
        {
            Debug.LogError("HATA: 'Play Button' Inspector'da atanmamış veya kayıp!");
        }

        
        if (deckButton != null)
        {
            deckButton.onClick.RemoveAllListeners();
            deckButton.onClick.AddListener(OpenDeck);
        }
        else
        {
            Debug.LogWarning("UYARI: 'Deck Button' atanmamış.");
        }

       
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
        deckMenuManager.RefreshUI();
    }
    public void OpenSettings(bool isActive)
    {
        settingsUI.SetActive(isActive);
        ClickBlock.SetActive(isActive);
        
    }
    public void TogglePerk(bool isActive)
    {
        PerkUI.SetActive(isActive);
        mainMenuUI.SetActive(!isActive);
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