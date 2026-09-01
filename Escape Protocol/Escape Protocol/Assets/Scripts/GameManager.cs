using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Stores the one (and only) instance of this script
    public static GameManager Instance {get; private set;}
    [SerializeField] public static bool isGameOver = false; // A flag to determine if the game is over or not
    public static int CurrentRound { get; private set; } = 1;
    private bool changingRound;
    private bool paused;
    private const int reviveCost = 10;

    private void Update()
    {
        if (isGameOver)
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame && CoinSystem.Spend(reviveCost))
            {
                Time.timeScale = 1f;
                isGameOver = false;
                PlayerController.Instance.ReviveInPlace();
                UIManager.Instance.ToggleGameOverUI(false);
                UIManager.Instance.HideHitNotice();
                UIManager.Instance.HideRoundMessage();
                AudioManager.Instance?.StartGameplayMusic();
            }
            return;
        }
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;
        paused = !paused;
        Time.timeScale = paused ? 0f : 1f;
        if (paused) UIManager.Instance.ShowRoundMessage("PAUSED\nPRESS ESC TO CONTINUE");
        else UIManager.Instance.HideRoundMessage();
    }

    private void Awake()
    {
        // Check our singleton
        if (Instance == null)
        {
            // Assign this instance of the script as THE instance
            Instance = this; 
        }
        else // There is already a GameManager assigned
        {
            // Destroy this extra copy of this script
            Destroy(gameObject);
        }

        // Reset the game over flag
        isGameOver = false;
        Time.timeScale = 1f;
        if (GetComponent<BossWorldController>() == null)
        {
            gameObject.AddComponent<BossWorldController>();
        }
    } 

    public void BeginRun()
    {
        if (PlayerPrefs.GetInt("BossMode", 0) == 1)
        {
            CurrentRound = 5;
            StartCoroutine(StartUnescapableMode());
            return;
        }

        CurrentRound = 1;
        StartCoroutine(RunStartCountdown());
    }

    private IEnumerator StartUnescapableMode()
    {
        Time.timeScale = 0f;
        BossWorldController.Instance?.EnterBossWorld();
        UIManager.Instance.SetDangerTheme(true);

        EnemyFollow[] enemies = FindObjectsByType<EnemyFollow>(FindObjectsSortMode.None);
        for (int index = 0; index < enemies.Length; index++)
        {
            bool isBoss = index == 0;
            enemies[index].gameObject.SetActive(isBoss);
            if (isBoss)
            {
                enemies[index].transform.localScale = Vector3.one * 3f;
                enemies[index].gameObject.name = "Unescapable Boss";
                enemies[index].SetUnescapableMode(true);
            }
        }

        bool accepted = false;
        UIManager.Instance.ShowUnescapableControls(() => accepted = true);
        yield return new WaitUntil(() => accepted);
        UIManager.Instance.HideRoundMessage();
        Time.timeScale = 1f;
        AudioManager.Instance?.StartUnescapableMusic();
    }

    private IEnumerator RunStartCountdown()
    {
        Time.timeScale = 0f;
        UIManager.Instance.ShowRoundMessage("READY");
        yield return new WaitForSecondsRealtime(1f);
        UIManager.Instance.ShowRoundMessage("SET");
        yield return new WaitForSecondsRealtime(1f);
        UIManager.Instance.ShowRoundMessage("RUN!");
        yield return new WaitForSecondsRealtime(.6f);

        UIManager.Instance.HideRoundMessage();
        Time.timeScale = 1f;
        AudioManager.Instance?.StartGameplayMusic();
    }

    public void RoundCompleted()
    {
        if (isGameOver || changingRound) return;
        StartCoroutine(StartNextRound());
    }

    private IEnumerator StartNextRound()
    {
        changingRound = true;
        Time.timeScale = 0f;
        UIManager.Instance.ShowRoundMessage($"ROUND {CurrentRound} COMPLETE!\nROUND {CurrentRound + 1} BEGINS");
        yield return new WaitForSecondsRealtime(2f);

        CurrentRound++;
        UIManager.Instance.SetDangerTheme(CurrentRound >= 5);
        if (CurrentRound == 5)
        {
            yield return StartCoroutine(StartBossRound());
        }
        if (CurrentRound <= 6)
        {
            yield return StartCoroutine(ChoosePerk());
        }
        UIManager.Instance.HideRoundMessage();
        Time.timeScale = 1f;
        changingRound = false;
        GiveRoundReward();
        AudioManager.Instance?.StartGameplayMusic();
    }

    private IEnumerator ChoosePerk()
    {
        bool chosen = false;
        Time.timeScale = 0f;
        gameObject.AddComponent<PerkChoiceUI>().Show(CurrentRound, perk => { ApplyPerk(perk); chosen = true; });
        yield return new WaitUntil(() => chosen);
        Time.timeScale = 1f;
    }

    private void ApplyPerk(PerkType perk)
    {
        if (perk == PerkType.ExtraLife) PlayerController.Instance.AddLife();
        else PlayerController.Instance.StartSpeedBurst();
    }

    private IEnumerator StartBossRound()
    {
        Time.timeScale = 0f;
        BossWorldController.Instance?.EnterBossWorld();
        UIManager.Instance.ShowRoundMessage("BOSS ROUND!\n\nDODGE THE BIG ENEMY\nSHOOT IT WITH ROCKETS\n\nPRESS ENTER TO START");
        yield return new WaitUntil(() => Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame);

        EnemyFollow boss = FindFirstObjectByType<EnemyFollow>();
        if (boss != null)
        {
            boss.transform.localScale = Vector3.one * 3f;
            boss.gameObject.name = "Boss Enemy";
        }
        Time.timeScale = 1f;
    }

    private void GiveRoundReward()
    {
        int reward = UnityEngine.Random.Range(0, 3);
        if (reward == 0)
        {
            PlayerController.Instance.ActivateShield(5f);
            UIManager.Instance.ShowRoundMessage("ROUND REWARD!\nSHIELD ON");
        }
        else if (reward == 1)
        {
            CoinSystem.AddCoin();
            CoinSystem.AddCoin();
            CoinSystem.AddCoin();
            UIManager.Instance.UpdateCoins(CoinSystem.Coins);
            UIManager.Instance.ShowRoundMessage("ROUND REWARD!\n+3 COINS");
        }
        else
        {
            PlayerController.Instance.AddScoreBonus(25);
            UIManager.Instance.ShowRoundMessage("ROUND REWARD!\n+25 SCORE");
        }
        StartCoroutine(HideRewardMessage());
    }

    private IEnumerator HideRewardMessage()
    {
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.HideRoundMessage();
    }

    public void GameOver()
    {
        if (isGameOver) return; // Do nothing if the game is already over 

        PlayerController.Instance.GameOver();
        // Set the game to be over
        isGameOver = true;
        Time.timeScale = 1f;
        AudioManager.Instance?.StopSound("MainTheme");
        UIManager.Instance.ShowRunSummary(PlayerController.Instance.CurrentScore, PlayerController.Instance.CurrentDanger);
        string[] taunts = { "UH OH. I GOT YOU.", "TOO SLOW.", "RUN FASTER NEXT TIME.", "THERE IS NO ESCAPE." };
        string reviveText = CoinSystem.Coins >= reviveCost ? "PRESS R: REVIVE (10 COINS)" : "NEED 10 COINS TO REVIVE";
        UIManager.Instance.ShowHitNotice(taunts[UnityEngine.Random.Range(0, taunts.Length)] + "\n" + reviveText);
        // Trigger Game Over UI
        UIManager.Instance.ToggleGameOverUI(true);
    }

    public void LoadMainMenu()
    {
        AudioManager.Instance?.StopSound("MainTheme");
        // Play UI Audio
        // Load the Main Menu Scene
        SceneManager.LoadScene(0);
    }

    public void LoadCurrentScene()
    {
        AudioManager.Instance?.StopSound("MainTheme");
        Time.timeScale = 1f;
        // Play UI Audio
        // Restarts the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
