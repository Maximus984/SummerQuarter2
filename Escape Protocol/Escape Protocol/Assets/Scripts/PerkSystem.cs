using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PerkType { ExtraLife, SpeedBurst, SecondChance, CoinMagnet, ShieldStart, ScoreBoost }

public static class PerkSystem
{
    public static bool HasSecondChance;
    public static bool CoinMagnet;
    public static void ResetRun() { HasSecondChance = false; CoinMagnet = false; }
}

public class PerkChoiceUI : MonoBehaviour
{
    private GameObject panel;
    private PerkType selected;
    private bool hasSelected;
    private Action<PerkType> onConfirm;

    public void Show(int round, Action<PerkType> confirm)
    {
        onConfirm = confirm;
        Canvas canvas = FindFirstObjectByType<Canvas>();
        TextMeshProUGUI template = UIManager.Instance.TextTemplate;
        panel = new GameObject("Perk Choice", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
        rect.sizeDelta = new Vector2(800, 430);
        panel.GetComponent<Image>().color = new Color(.03f, .06f, .12f, .97f);
        AddText(template, "CHOOSE A PERK - ROUND " + round, new Vector2(0, 150), 38, Color.yellow);
        AddButton(template, "TOUGH FRAME\n+1 LIFE", new Vector2(-200, 10), PerkType.ExtraLife);
        AddButton(template, "SPEED BURST\n10 SEC FAST", new Vector2(200, 10), PerkType.SpeedBurst);
        AddButton(template, "CONFIRM", new Vector2(0, -145), null);
    }

    private void AddText(TextMeshProUGUI template, string message, Vector2 position, float size, Color color)
    {
        TextMeshProUGUI text = Instantiate(template, panel.transform);
        text.rectTransform.anchoredPosition = position; text.rectTransform.sizeDelta = new Vector2(700, 90);
        text.text = message; text.fontSize = size; text.color = color; text.alignment = TextAlignmentOptions.Center; text.raycastTarget = false;
    }

    private void AddButton(TextMeshProUGUI template, string label, Vector2 position, PerkType? perk)
    {
        GameObject buttonObject = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(panel.transform, false);
        RectTransform rect = buttonObject.GetComponent<RectTransform>(); rect.anchoredPosition = position; rect.sizeDelta = new Vector2(300, 100);
        buttonObject.GetComponent<Image>().color = perk.HasValue ? new Color(.1f, .35f, .55f) : new Color(.15f, .6f, .25f);
        Button button = buttonObject.GetComponent<Button>();
        if (perk.HasValue) button.onClick.AddListener(() => { selected = perk.Value; hasSelected = true; });
        else button.onClick.AddListener(Confirm);
        AddText(template, label, position, 25, Color.white);
    }

    private void Confirm()
    {
        if (!hasSelected) return;
        onConfirm?.Invoke(selected); Destroy(panel); Destroy(this);
    }
}
