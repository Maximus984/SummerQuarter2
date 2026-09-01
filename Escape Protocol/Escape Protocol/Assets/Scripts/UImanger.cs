                  using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {get; private set;}

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI highScoreText;
    public TextMeshProUGUI TextTemplate => scoreText;
    private TextMeshProUGUI livesText;
    private TextMeshProUGUI hitNoticeText;
    private TextMeshProUGUI roundMessageText;
    private TextMeshProUGUI shieldText;
    private TextMeshProUGUI coinText;
    private TextMeshProUGUI xpText;
    private TextMeshProUGUI rankText;
    private Image staminaFill;
    private TextMeshProUGUI staminaLabel;
    private float hitNoticeEndTime;

    // Awake() Called when this gameobject is enabled in the scene
    private void Awake()
    {
        // Check Singleton
        // If there is no other instance of this script in the scene...
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Destroy any duplicates of this script
            Destroy(gameObject);
        }

        //toggle off the game over panel
        ToggleGameOverUI(false);
        scoreText.color = Color.white;
        scoreText.fontSize = 30;
        scoreText.enableAutoSizing = false;
        scoreText.alignment = TextAlignmentOptions.TopLeft;
        scoreText.raycastTarget = false;
        scoreText.rectTransform.anchoredPosition = new Vector2(25f, -25f);
        scoreText.rectTransform.sizeDelta = new Vector2(420f, 100f);
        ConfigureHudText(scoreText);
        CreateHealthDisplay();
        CreateShieldDisplay();
        CreateCoinDisplay();
        CreateXpDisplay();
        CreateRankDisplay();
        CreateStaminaDisplay();
        shieldText.gameObject.SetActive(false);
        coinText.gameObject.SetActive(false);
        xpText.gameObject.SetActive(false);
        rankText.gameObject.SetActive(false);
    } 

    public void UpdateScore(int score, int health, int maxHealth, int dangerLevel)
    {
        // Update the score text object with the given score
        scoreText.text = $"SCORE: {score}\nDANGER LEVEL: {dangerLevel}/10";

        if (livesText != null)
        {
            livesText.text = $"LIVES: {health}";
        }

        if (hitNoticeText != null && Time.time > hitNoticeEndTime)
        {
            hitNoticeText.gameObject.SetActive(false);
        }
    }

    public void UpdateHighScore(float highScore)
    {
        // Keep the high score simple and readable: 1, 2, 3, 4...
        highScoreText.text = $"High Score: {Mathf.FloorToInt(highScore)}";
    }

    public void ToggleGameOverUI(bool show)
    {
        gameOverUI.SetActive(show);
    }

    public void ShowHitNotice(int health)
    {
        if (hitNoticeText == null) return;

        hitNoticeText.text = health > 0 ? $"HIT!  {health} LIVES REMAINING" : "LAST HIT!";
        hitNoticeText.gameObject.SetActive(true);
        hitNoticeEndTime = Time.time + 1.5f;
    }

    public void ShowHitNotice(string message)
    {
        if (hitNoticeText == null) return;
        hitNoticeText.text = message;
        hitNoticeText.gameObject.SetActive(true);
        hitNoticeEndTime = Time.time + 10f;
    }

    public void HideHitNotice()
    {
        if (hitNoticeText != null) hitNoticeText.gameObject.SetActive(false);
    }

    public void UpdateCoins(int coins)
    {
        if (coinText != null) coinText.text = $"● COINS: {coins}";
    }

    public void UpdateXp(int xp, int level)
    {
        if (xpText != null) xpText.text = $"XP: {xp}/100  LEVEL: {level}";
        if (rankText != null) rankText.text = $"RANK: {RankSystem.CurrentRank}";
    }

    public void UpdateStamina(float amount, bool sprinting)
    {
        if (staminaFill == null) return;
        staminaFill.fillAmount = Mathf.Clamp01(amount);
        staminaFill.color = sprinting ? new Color(1f, .55f, .1f) : Color.cyan;
        staminaLabel.text = "SPRINT";
    }

    public void ShowUnescapableControls(Action onOkay)
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        GameObject panel = new GameObject("Unescapable Controls", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        panel.GetComponent<Image>().color = new Color(.08f, .01f, .01f, .96f);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = panelRect.anchorMax = new Vector2(.5f, .5f);
        panelRect.pivot = new Vector2(.5f, .5f);
        panelRect.sizeDelta = new Vector2(760f, 500f);

        TextMeshProUGUI message = Instantiate(scoreText, panel.transform);
        ConfigureHudText(message);
        message.text = "UNESCAPABLE\n\nA FAST BOSS IS HUNTING YOU\n\nMOVE: A + D OR ARROW KEYS\nSPRINT: HOLD LEFT CLICK\nSHOOT: RIGHT CLICK\n\nORANGE TOKENS GIVE A SPEED BOOST";
        message.fontSize = 29;
        message.alignment = TextAlignmentOptions.Center;
        message.color = Color.white;
        message.rectTransform.anchorMin = new Vector2(0f, .2f);
        message.rectTransform.anchorMax = new Vector2(1f, 1f);
        message.rectTransform.offsetMin = new Vector2(30f, 15f);
        message.rectTransform.offsetMax = new Vector2(-30f, -25f);

        GameObject okay = new GameObject("Unescapable OK Button", typeof(RectTransform), typeof(Image), typeof(Button));
        okay.transform.SetParent(panel.transform, false);
        Image okayImage = okay.GetComponent<Image>();
        okayImage.color = new Color(.8f, .13f, .08f, 1f);
        RectTransform okayRect = okay.GetComponent<RectTransform>();
        okayRect.anchorMin = okayRect.anchorMax = new Vector2(.5f, 0f);
        okayRect.pivot = new Vector2(.5f, 0f);
        okayRect.anchoredPosition = new Vector2(0f, 35f);
        okayRect.sizeDelta = new Vector2(270f, 75f);

        TextMeshProUGUI okayText = Instantiate(scoreText, okay.transform);
        ConfigureHudText(okayText);
        okayText.text = "OK - START RUN";
        okayText.fontSize = 26;
        okayText.alignment = TextAlignmentOptions.Center;
        okayText.color = Color.white;
        okayText.rectTransform.anchorMin = Vector2.zero;
        okayText.rectTransform.anchorMax = Vector2.one;
        okayText.rectTransform.offsetMin = Vector2.zero;
        okayText.rectTransform.offsetMax = Vector2.zero;

        okay.GetComponent<Button>().onClick.AddListener(() =>
        {
            Destroy(panel);
            onOkay?.Invoke();
        });
    }

    public void ShowRoundMessage(string message)
    {
        if (roundMessageText == null) CreateRoundMessage();
        roundMessageText.text = message;
        roundMessageText.gameObject.SetActive(true);
    }

    public void HideRoundMessage()
    {
        if (roundMessageText != null) roundMessageText.gameObject.SetActive(false);
    }

    public void ShowRunSummary(float score, int danger)
    {
        if (roundMessageText == null) CreateRoundMessage();
        roundMessageText.text = $"SCORE: {Mathf.FloorToInt(score)}\nBEST DANGER: {danger}";
        roundMessageText.gameObject.SetActive(true);

        // Keep the result summary left of the restart and quit buttons.
        roundMessageText.alignment = TextAlignmentOptions.MidlineLeft;
        roundMessageText.fontSize = 34;
        roundMessageText.rectTransform.anchoredPosition = new Vector2(-360f, -100f);
        roundMessageText.rectTransform.sizeDelta = new Vector2(320f, 110f);
        roundMessageText.raycastTarget = false;
    }

    public void SetShieldStatus(bool active)
    {
        if (shieldText == null) return;
        shieldText.text = active ? "◆  SHIELD ACTIVE" : "◇  SHIELD READY";
        shieldText.color = active ? Color.cyan : new Color(.4f, .8f, 1f);
    }

    public void SetDangerTheme(bool dangerMode)
    {
        Color color = dangerMode ? new Color(1f, .2f, .2f) : Color.white;
        scoreText.color = color;
        livesText.color = dangerMode ? color : Color.red;
        shieldText.color = dangerMode ? color : shieldText.color;
        coinText.color = dangerMode ? color : Color.yellow;
        xpText.color = dangerMode ? color : Color.magenta;
        rankText.color = dangerMode ? color : new Color(1f, .65f, .1f);
    }

    private void CreateHealthDisplay()
    {
        livesText = Instantiate(scoreText, scoreText.transform.parent);
        livesText.name = "Lives Display";
        livesText.alignment = TextAlignmentOptions.Center;
        livesText.fontSize = 38;
        livesText.color = Color.red;
        livesText.text = "LIVES: 3";
        ConfigureHudText(livesText);

        RectTransform livesRect = livesText.rectTransform;
        livesRect.anchorMin = new Vector2(1f, 1f);
        livesRect.anchorMax = new Vector2(1f, 1f);
        livesRect.pivot = new Vector2(1f, 1f);
        livesRect.anchoredPosition = new Vector2(-25f, -25f);
        livesRect.sizeDelta = new Vector2(300f, 50f);

        hitNoticeText = Instantiate(livesText, scoreText.transform.parent);
        hitNoticeText.name = "Hit Notice";
        ConfigureHudText(hitNoticeText);
        hitNoticeText.fontSize = 46;
        hitNoticeText.color = new Color(1f, .2f, .2f);
        hitNoticeText.gameObject.SetActive(false);

        RectTransform noticeRect = hitNoticeText.rectTransform;
        noticeRect.anchorMin = new Vector2(.5f, .5f);
        noticeRect.anchorMax = new Vector2(.5f, .5f);
        noticeRect.pivot = new Vector2(.5f, .5f);
        noticeRect.anchoredPosition = new Vector2(0f, -255f);
        noticeRect.sizeDelta = new Vector2(700f, 55f);
        hitNoticeText.fontSize = 28;
    }

    private void CreateRoundMessage()
    {
        roundMessageText = Instantiate(scoreText, scoreText.transform.parent);
        roundMessageText.name = "Round Message";
        roundMessageText.alignment = TextAlignmentOptions.Center;
        roundMessageText.fontSize = 58;
        roundMessageText.color = Color.yellow;
        roundMessageText.raycastTarget = false;

        RectTransform messageRect = roundMessageText.rectTransform;
        messageRect.anchorMin = new Vector2(.5f, .5f);
        messageRect.anchorMax = new Vector2(.5f, .5f);
        messageRect.pivot = new Vector2(.5f, .5f);
        messageRect.anchoredPosition = Vector2.zero;
        messageRect.sizeDelta = new Vector2(900f, 180f);
    }

    private void CreateShieldDisplay()
    {
        shieldText = Instantiate(livesText, scoreText.transform.parent);
        shieldText.name = "Shield Icon";
        ConfigureHudText(shieldText);
        shieldText.fontSize = 28;
        shieldText.alignment = TextAlignmentOptions.Center;
        shieldText.rectTransform.anchoredPosition = new Vector2(-25f, -75f);
        shieldText.rectTransform.sizeDelta = new Vector2(300f, 45f);
        SetShieldStatus(false);
    }

    private void CreateCoinDisplay()
    {
        coinText = Instantiate(livesText, scoreText.transform.parent);
        coinText.name = "Coin Display";
        ConfigureHudText(coinText);
        coinText.fontSize = 28;
        coinText.color = Color.yellow;
        coinText.rectTransform.anchoredPosition = new Vector2(-25f, -120f);
        coinText.rectTransform.sizeDelta = new Vector2(300f, 45f);
        UpdateCoins(CoinSystem.Coins);
    }

    private void CreateXpDisplay()
    {
        xpText = Instantiate(livesText, scoreText.transform.parent);
        xpText.name = "XP Display";
        ConfigureHudText(xpText);
        xpText.fontSize = 24;
        xpText.color = Color.magenta;
        xpText.rectTransform.anchoredPosition = new Vector2(-25f, -165f);
        xpText.rectTransform.sizeDelta = new Vector2(300f, 45f);
        UpdateXp(XpSystem.Xp, XpSystem.Level);
    }

    private void CreateRankDisplay()
    {
        rankText = Instantiate(livesText, scoreText.transform.parent);
        rankText.name = "Rank Display";
        ConfigureHudText(rankText);
        rankText.fontSize = 26;
        rankText.color = new Color(1f, .65f, .1f);
        rankText.rectTransform.anchoredPosition = new Vector2(-25f, -210f);
        rankText.rectTransform.sizeDelta = new Vector2(300f, 45f);
        rankText.text = $"RANK: {RankSystem.CurrentRank}";
    }

    private void CreateStaminaDisplay()
    {
        GameObject background = new GameObject("Stamina Bar", typeof(RectTransform), typeof(Image));
        background.transform.SetParent(scoreText.transform.parent, false);
        background.GetComponent<Image>().color = new Color(0f, 0f, 0f, .65f);
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = backgroundRect.anchorMax = new Vector2(.5f, 1f);
        backgroundRect.pivot = new Vector2(.5f, 1f);
        backgroundRect.anchoredPosition = new Vector2(0f, -25f);
        backgroundRect.sizeDelta = new Vector2(220f, 18f);

        GameObject fill = new GameObject("Stamina Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(background.transform, false);
        staminaFill = fill.GetComponent<Image>();
        staminaFill.color = Color.cyan;
        staminaFill.type = Image.Type.Filled;
        staminaFill.fillMethod = Image.FillMethod.Horizontal;
        staminaFill.fillOrigin = 0;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(3f, 3f);
        fillRect.offsetMax = new Vector2(-3f, -3f);

        staminaLabel = Instantiate(scoreText, background.transform.parent);
        staminaLabel.name = "Sprint Label";
        staminaLabel.text = "SPRINT";
        staminaLabel.fontSize = 18;
        staminaLabel.alignment = TextAlignmentOptions.Center;
        staminaLabel.color = Color.white;
        ConfigureHudText(staminaLabel);
        RectTransform labelRect = staminaLabel.rectTransform;
        labelRect.anchorMin = labelRect.anchorMax = new Vector2(.5f, 1f);
        labelRect.pivot = new Vector2(.5f, 1f);
        labelRect.anchoredPosition = new Vector2(0f, -47f);
        labelRect.sizeDelta = new Vector2(220f, 22f);
    }

    private void ConfigureHudText(TextMeshProUGUI text)
    {
        text.rectTransform.localScale = Vector3.one;
        text.enableWordWrapping = false;
        text.enableAutoSizing = false;
        text.overflowMode = TextOverflowModes.Overflow;
        text.margin = Vector4.zero;
        text.raycastTarget = false;
    }
}
