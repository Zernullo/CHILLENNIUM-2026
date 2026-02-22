using UnityEngine;
using TMPro;

public class InvertWarning : MonoBehaviour
{
    [Header("References")]
    public Transform panel;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI bossText;
    public InvertKey invertKey;

    [Header("Settings")]
    public float timeTillInvert = 5f;
    public float invertDuration = 10f;
    public float timeTillRevert = 5f;

    [Header("Key Text")]
    public string originalKeyLabel = "Q W E R";
    public string invertedKeyLabel = "R E W Q";

    private float timer = 0f;
    private enum Phase { Idle, CountdownToInvert, Inverted }
    private Phase phase = Phase.Idle;

    private void Awake()
    {
        if (panel != null) panel.gameObject.SetActive(false);
        if (bossText != null) bossText.gameObject.SetActive(false);
        SetKeyText(false);
    }

    public void StartCountdown()
    {
        if (phase != Phase.Idle) return;
        timer = 0f;
        phase = Phase.CountdownToInvert;
        if (panel != null) panel.gameObject.SetActive(true);
        if (bossText != null)
        {
            bossText.gameObject.SetActive(true);
            bossText.text = "Boss: \"Nah, I win\" Your keys are inverted to:";
        }
        SetKeyText(true); 
    }

    public void StopAll()
    {
        timer = 0f;
        phase = Phase.Idle;
        SetKeyText(false);
        HidePanel();
    }

    private void HidePanel()
    {
        if (panel != null) panel.gameObject.SetActive(false);
        if (countdownText != null) countdownText.text = "";
        if (bossText != null) bossText.gameObject.SetActive(false);
    }

    private void SetKeyText(bool inverted)
    {
        if (keyText == null) return;
        keyText.text = inverted ? invertedKeyLabel : originalKeyLabel;
        keyText.color = inverted ? Color.red : Color.white;
    }

    private void Update()
    {
        switch (phase)
        {
            case Phase.Idle:
                break;

            case Phase.CountdownToInvert:
            {
                timer += Time.deltaTime;
                float remaining = Mathf.Max(0f, timeTillInvert - timer);
                SetText($"In: {remaining:F1}s", remaining);

                if (remaining <= 0f)
                {
                    HidePanel();
                    invertKey?.TriggerInvert();
                    SetKeyText(true);  // keys just became inverted
                    timer = 0f;
                    phase = Phase.Inverted;
                }
                break;
            }

            case Phase.Inverted:
            {
                timer += Time.deltaTime;
                float remaining = Mathf.Max(0f, invertDuration - timer);

                if (remaining <= timeTillRevert)
                {
                    if (panel != null) panel.gameObject.SetActive(true);
                    if (bossText != null)
                    {
                        bossText.gameObject.SetActive(true);
                        bossText.text = "Boss: \"Back to normal... for now\" Your keys will revert to:";
                    }
                    SetKeyText(false);
                    SetText($"In: {remaining:F1}s", remaining);
                }

                if (remaining <= 0f)
                {
                    HidePanel();
                    invertKey?.TriggerInvert();
                    SetKeyText(false);  // keys just went back to original
                    timer = 0f;
                    phase = Phase.Idle;
                }
                break;
            }
        }
    }

    private void SetText(string message, float remaining)
    {
        if (countdownText == null) return;
        countdownText.text = message;
        countdownText.color = remaining < 2f
            ? Color.Lerp(Color.red, Color.white, remaining / 2f)
            : Color.white;
    }
}