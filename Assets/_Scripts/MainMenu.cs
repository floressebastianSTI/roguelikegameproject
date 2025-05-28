using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button singleplayerButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Settings Menu")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button backButton;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("Button Colors")]
    [SerializeField] private Color normalColor = new Color(1, 1, 1, 0); // Transparent
    [SerializeField] private Color hoverColor = new Color(1, 1, 1, 0.2f); // Semi-transparent white
    [SerializeField] private Color pressedColor = new Color(1, 1, 1, 0.4f); // More visible white

    private void Start()
    {
        // Initialize the menu
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);

        // Setup button listeners
        SetupButton(singleplayerButton, OnSingleplayerClick);
        SetupButton(multiplayerButton, OnMultiplayerClick);
        SetupButton(settingsButton, OnSettingsClick);
        SetupButton(quitButton, OnQuitClick);
        backButton.onClick.AddListener(OnBackClick);

        // Load saved settings
        LoadSettings();
    }

    private void SetupButton(Button button, UnityEngine.Events.UnityAction onClick)
    {
        if (button != null)
        {
            // Set up the button's image colors
            var colors = button.colors;
            colors.normalColor = normalColor;
            colors.highlightedColor = hoverColor;
            colors.pressedColor = pressedColor;
            colors.selectedColor = normalColor;
            button.colors = colors;

            // Set the image component to start transparent
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                Color imageColor = buttonImage.color;
                imageColor.a = 0f;
                buttonImage.color = imageColor;
            }

            // Add click listener
            button.onClick.AddListener(onClick);
        }
    }

    private void OnSingleplayerClick()
    {
        Debug.Log("Attempting to load SingleplayerMode...");
        try
        {
            if (Application.CanStreamedLevelBeLoaded("SingleplayerMode"))
            {
                SceneManager.LoadScene("SingleplayerMode");
            }
            else
            {
                Debug.LogError("SingleplayerMode is not in Build Settings!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load SingleplayerMode: {e.Message}");
        }
    }

    private void OnMultiplayerClick()
    {
        Debug.Log("Attempting to load MultiplayerMode...");
        try
        {
            if (Application.CanStreamedLevelBeLoaded("MultiplayerMode"))
            {
                SceneManager.LoadScene("MultiplayerMode");
            }
            else
            {
                Debug.LogError("MultiplayerMode is not in Build Settings!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load MultiplayerMode: {e.Message}");
        }
    }

    private void OnSettingsClick()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void OnBackClick()
    {
        SaveSettings();
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    private void OnQuitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private void LoadSettings()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", 0);

        // Apply loaded settings
        AudioListener.volume = volumeSlider.value;
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Resolution", resolutionDropdown.value);
        PlayerPrefs.Save();

        // Apply settings
        AudioListener.volume = volumeSlider.value;
        Screen.fullScreen = fullscreenToggle.isOn;
    }
}