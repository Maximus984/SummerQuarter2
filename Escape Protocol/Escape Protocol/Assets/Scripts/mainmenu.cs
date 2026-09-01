using UnityEngine;
using UnityEngine.SceneManagement; // Enable this script to change scenes and more
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    private GameObject howToPlayButton;
    private GameObject howToPlayPanel;
    private GameObject settingsButton;
    private GameObject settingsPanel;
    private GameObject unescapableButton;

    private void Start()
    {
        CreateHowToPlayMenu();
        CreateSettingsMenu();
        CreateUnescapableButton();
    }

    public void StartGame()
    {
        // Load the game scene by its exact name, so menu changes cannot open an empty scene.
        PlayerPrefs.SetInt("BossMode", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }

    private void CreateUnescapableButton()
    {
        GameObject startButton = GameObject.Find("Start button");
        if (startButton == null) return;

        unescapableButton = Instantiate(startButton, startButton.transform.parent);
        unescapableButton.name = "Unescapable Button";
        unescapableButton.transform.SetSiblingIndex(1);
        SetButtonText(unescapableButton, "UNESCAPABLE");

        Button button = unescapableButton.GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(StartUnescapable);
    }

    public void StartUnescapable()
    {
        PlayerPrefs.SetInt("BossMode", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitGame()
    {
        // Close the game application
        Application.Quit();
    }

    private void CreateHowToPlayMenu()
    {
        GameObject startButton = GameObject.Find("Start button");
        if (startButton == null) return;

        howToPlayButton = Instantiate(startButton, startButton.transform.parent);
        howToPlayButton.name = "How To Play Button";
        howToPlayButton.transform.SetSiblingIndex(1);
        SetButtonText(howToPlayButton, "HOW TO PLAY");

        Button button = howToPlayButton.GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(ShowHowToPlay);

        howToPlayPanel = CreatePage("How To Play Page",
            "HOW TO PLAY\n\n" +
            "Move: A and D, or arrow keys\n" +
            "Do not hit walls\n" +
            "Stay away from enemies\n" +
            "You have 3 lives\n" +
            "Get a high score\n\n" +
            "PRESS HERE TO GO BACK");
        howToPlayPanel.GetComponent<Button>().onClick.AddListener(HideHowToPlay);
        howToPlayPanel.SetActive(false);
    }

    public void ShowHowToPlay()
    {
        howToPlayButton.SetActive(false);
        howToPlayPanel.SetActive(true);
    }

    public void HideHowToPlay()
    {
        howToPlayPanel.SetActive(false);
        howToPlayButton.SetActive(true);
    }

    private void SetButtonText(GameObject buttonObject, string message)
    {
        TextMeshProUGUI label = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
        label.text = message;
        label.fontSize = 22f;
        label.alignment = TextAlignmentOptions.Center;
    }

    private void CreateSettingsMenu()
    {
        GameObject startButton = GameObject.Find("Start button");
        if (startButton == null) return;

        // Create the Settings button on the main menu
        settingsButton = Instantiate(startButton, startButton.transform.parent);
        settingsButton.name = "Settings Button";
        settingsButton.transform.SetSiblingIndex(2);
        SetButtonText(settingsButton, "SETTINGS");

        Button settings = settingsButton.GetComponent<Button>();
        settings.onClick = new Button.ButtonClickedEvent();
        settings.onClick.AddListener(ShowSettings);

        // Create the Settings panel
        settingsPanel = CreatePage("Settings Page", "SETTINGS");

        // Clicking the main settings panel changes the movement keys
        settingsPanel.GetComponent<Button>().onClick.AddListener(ChangeMoveKeys);

        // Create a separate close button inside the Settings panel
        GameObject closeButton = Instantiate(startButton, settingsPanel.transform);
        closeButton.name = "Close Settings Button";
        SetButtonText(closeButton, "CLOSE");

        RectTransform closeRect = closeButton.GetComponent<RectTransform>();

        // Position the close button at the bottom center
        closeRect.anchorMin = new Vector2(0.5f, 0f);
        closeRect.anchorMax = new Vector2(0.5f, 0f);
        closeRect.pivot = new Vector2(0.5f, 0f);
        closeRect.anchoredPosition = new Vector2(0f, 20f);
        closeRect.sizeDelta = new Vector2(220f, 60f);
        closeRect.localScale = Vector3.one;

        Button closeButtonComponent = closeButton.GetComponent<Button>();
        closeButtonComponent.onClick = new Button.ButtonClickedEvent();
        closeButtonComponent.onClick.AddListener(ExitSettingsMenu);

        settingsPanel.SetActive(false);
    }

    private void ExitSettingsMenu()
    {
        settingsPanel.SetActive(false);
        settingsButton.SetActive(true);
    }

    private void ShowSettings()
    {
        settingsButton.SetActive(false);
        settingsPanel.SetActive(true);
        UpdateSettingsText();
    }

    private void ChangeMoveKeys()
    {
        int useWasd = PlayerPrefs.GetInt("UseWasdControls", 0) == 1 ? 0 : 1;
        PlayerPrefs.SetInt("UseWasdControls", useWasd);
        PlayerPrefs.Save();
        UpdateSettingsText();
    }


    private void UpdateSettingsText()
    {
        string selected = PlayerPrefs.GetInt("UseWasdControls", 0) == 1 ? "WASD" : "ARROW KEYS";
        SetButtonText(settingsPanel,
            "SETTINGS\n\nMOVE KEYS: " + selected + "\nCLICK TO SWITCH\n\nLEFT CLICK: SHOOT\nLEFT SHIFT: SPRINT\nV: CAMERA VIEW\n\nSTARTS IN THIRD-PERSON");
    }

    private GameObject CreatePage(string pageName, string message)
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        GameObject page = new GameObject(pageName, typeof(RectTransform), typeof(Image), typeof(Button));
        page.transform.SetParent(canvas.transform, false);

        RectTransform pageRect = page.GetComponent<RectTransform>();
        pageRect.anchorMin = new Vector2(.5f, .5f);
        pageRect.anchorMax = new Vector2(.5f, .5f);
        pageRect.pivot = new Vector2(.5f, .5f);
        pageRect.sizeDelta = new Vector2(760f, 520f);
        page.GetComponent<Image>().color = new Color(.03f, .07f, .12f, .96f);

        GameObject startButton = GameObject.Find("Start button");
        TextMeshProUGUI template = startButton.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI pageText = Instantiate(template, page.transform);
        pageText.rectTransform.anchorMin = Vector2.zero;
        pageText.rectTransform.anchorMax = Vector2.one;
        pageText.rectTransform.offsetMin = new Vector2(40f, 40f);
        pageText.rectTransform.offsetMax = new Vector2(-40f, -40f);
        pageText.text = message;
        pageText.fontSize = 30f;
        pageText.alignment = TextAlignmentOptions.Center;
        pageText.color = Color.white;
        pageText.raycastTarget = false;
        return page;
    }
}
