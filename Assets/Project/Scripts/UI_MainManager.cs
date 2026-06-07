using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainManager : MonoBehaviour {
    [Header("UI Panels")]
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI capsuleCounterText;

    [Header("Buttons")]
    [SerializeField] private Button victoryRestartButton;
    [SerializeField] private Button defeatRestartButton;

    private GameManager gameManager;

    private void Awake() {
        gameManager = FindFirstObjectByType<GameManager>();

        if (victoryRestartButton != null) victoryRestartButton.onClick.AddListener(OnRestartButtonClicked);
        if (defeatRestartButton != null) defeatRestartButton.onClick.AddListener(OnRestartButtonClicked);
    }

    private void OnEnable() {
        GameManager.OnGameStateChanged += HandleStateChanged;
        GameManager.OnCapsuleCountChanged += UpdateCapsuleCountUI;
        GameManager.OnCountdownChanged += UpdateCountdownUI;
        GameManager.OnGameplayTimerChanged += UpdateGameplayTimerUI;
    }

    private void OnDisable() {
        GameManager.OnGameStateChanged -= HandleStateChanged;
        GameManager.OnCapsuleCountChanged -= UpdateCapsuleCountUI;
        GameManager.OnCountdownChanged -= UpdateCountdownUI;
        GameManager.OnGameplayTimerChanged -= UpdateGameplayTimerUI;
    }

    private void HandleStateChanged(GameManager.GameState gameState) {
        countdownPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);

        switch (gameState) {
            case GameManager.GameState.Countdown:
                countdownPanel.SetActive(true);
                break;
            case GameManager.GameState.Playing:
                gameplayPanel.SetActive(true);
                break;
            case GameManager.GameState.Victory:
                victoryPanel.SetActive(true);
                break;
            case GameManager.GameState.Defeat:
                defeatPanel.SetActive(true);
                break;
        }
    }

    private void UpdateCountdownUI(float timeRemain) {
        if (countdownText != null) countdownText.text = Mathf.CeilToInt(timeRemain).ToString();
    }

    private void UpdateCapsuleCountUI(int current, int total) {
        if (capsuleCounterText != null) capsuleCounterText.text = $"{current}/{total}";
    }

    private void UpdateGameplayTimerUI(float timeRemain) {
        if (timerText == null) return;

        if (timeRemain < 0) timeRemain = 0;

        int minute = Mathf.FloorToInt(timeRemain / 60f);
        int seconds = Mathf.FloorToInt(timeRemain % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minute, seconds);
    }

    private void OnRestartButtonClicked() {
        if (gameManager == null) return;

        gameManager.RestartGame();

        // P_Movement playerMovement = FindFirstObjectByType<P_Movement>();
        // if (playerMovement != null) playerMovement.TeleportToStar();

        // CapsuleManager[] allCapsules = FindObjectsByType<CapsuleManager>(FindObjectsSortMode.None);
        // foreach (var capsule in allCapsules) {
        //     capsule.ResetCapsule();
        // }
    }
}
