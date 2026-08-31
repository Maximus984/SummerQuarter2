using UnityEngine;

public static class CoinSystem
{
    public static int Coins { get; private set; }
    public static void AddCoin() => Coins++;
    public static bool Spend(int amount)
    {
        if (Coins < amount) return false;
        Coins -= amount;
        return true;
    }
}

public static class XpSystem
{
    public static int Xp { get; private set; }
    public static int Level => Xp / 100 + 1;
    public static void AddXp(int amount) => Xp += amount;
}

public static class RankSystem
{
    public static string CurrentRank
    {
        get
        {
            int level = XpSystem.Level;
            if (level >= 20) return "DIAMOND";
            if (level >= 12) return "PLATINUM";
            if (level >= 7) return "GOLD";
            if (level >= 3) return "SILVER";
            return "BRONZE";
        }
    }
}

public class CoinPickupSpawner : MonoBehaviour
{
    private float nextCoinScore = 30f;
    private PlayerController player;
    private void Start() => player = GetComponent<PlayerController>();
    private void Update()
    {
        if (player.CurrentScore < nextCoinScore) return;
        nextCoinScore += 35f;
        GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        coin.name = "Coin";
        coin.transform.position = transform.position + Vector3.forward * 18f + Vector3.right * Random.Range(-3f, 3f) + Vector3.up * 2f;
        coin.transform.localScale = new Vector3(1f, .2f, 1f);
        coin.GetComponent<Renderer>().material.color = Color.yellow;
        coin.GetComponent<Collider>().isTrigger = true;
        coin.AddComponent<CoinPickup>();
        Destroy(coin, 15f);
    }
}

public class CoinPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null) return;
        CoinSystem.AddCoin();
        UIManager.Instance.UpdateCoins(CoinSystem.Coins);
        Destroy(gameObject);
    }
}
