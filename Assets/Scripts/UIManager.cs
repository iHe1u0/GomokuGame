using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text infoText;
    public Toggle aiToggle;
    public Button restartBtn;

    public void Start()
    {
        if (aiToggle != null)
        {
            aiToggle.isOn = GameManager.Instance.vsAI;
            aiToggle.onValueChanged.AddListener(val => GameManager.Instance.vsAI = val);
        }
        if (restartBtn != null) restartBtn.onClick.AddListener(() => GameManager.Instance.RestartGame());
    }

    public void Update()
    {
        if (infoText != null)
        {
            if (GameManager.Instance == null) { return; }
            infoText.text = GameManager.Instance.currentTurn == Stone.Black ? "黑方回合" : "白方回合";
            if (GameManager.Instance != null && GameManager.Instance.playerIsBlack == false)
            { infoText.text += " (你执白)"; }
        }
    }
}
