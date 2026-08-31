                  using UnityEngine;
using TMPro;
using System;

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
