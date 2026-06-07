using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Countdown,
        Playing,
        Victory,
        Defeat
    }

    public GameState currentGameState { get; private set; }

    public static event Action<GameState> OnGameStateChanged;
    public static event Action<int, int> OnCapsuleCountChanged;
    public static event Action<float> OnCountdownChanged;
    public static event Action<float> OnGameplayTimerChanged;


    [Header("Timers")]
    [SerializeField] private float countdownDuration = 5f;
    [SerializeField] private float gameplayTimerDuration = 300f;

    private int totalCapsules = 4;
    private int coloredCapsules = 0;

    private Coroutine countdownCoroutine;
    private Coroutine gameplayTimerCoroutine;

    private InputActions inputActions;

    private void Awake()
    {
        inputActions = new InputActions();
    }

    private void Start()
    {
        ChangeState(GameState.Countdown);
    }

    private void OnEnable()
    {
        inputActions.Player.Restart.Enable();
        inputActions.Player.Exit.Enable();

        inputActions.Player.Restart.performed += OnGameRestart;
        inputActions.Player.Exit.performed += OnGameExit;
        CapsuleManager.OnCapsuleMaterialChanged += OnCapsuleRegistered;
    }

    private void OnDisable()
    {
        CapsuleManager.OnCapsuleMaterialChanged -= OnCapsuleRegistered;
        inputActions.Player.Restart.performed -= OnGameRestart;
        inputActions.Player.Exit.performed -= OnGameRestart;

        inputActions.Player.Restart.Disable();
        inputActions.Player.Exit.Disable();
    }

    private void OnGameRestart(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        RestartGame();
    }
    private void OnGameExit(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnCapsuleRegistered()
    {
        if (currentGameState != GameState.Playing) return;

        coloredCapsules++;
        OnCapsuleCountChanged?.Invoke(coloredCapsules, totalCapsules);

        if (coloredCapsules >= totalCapsules) ChangeState(GameState.Victory);
    }


    public void ChangeState(GameState newState)
    {
        currentGameState = newState;
        OnGameStateChanged?.Invoke(newState);

        StopAllTimers();

        switch (newState)
        {
            case GameState.Countdown:
                coloredCapsules = 0;
                OnCapsuleCountChanged?.Invoke(coloredCapsules, totalCapsules);

                countdownCoroutine = StartCoroutine(CountdownRoutine());
                break;

            case GameState.Playing:
                gameplayTimerCoroutine = StartCoroutine(GameplayTimerRoutine());

                break;
        }
    }

    private IEnumerator CountdownRoutine()
    {
        float remainingTime = countdownDuration;

        while (remainingTime > 0)
        {
            OnCountdownChanged?.Invoke(remainingTime);

            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        ChangeState(GameState.Playing);
    }

    private IEnumerator GameplayTimerRoutine()
    {
        float remainingTime = gameplayTimerDuration;

        while (remainingTime > 0)
        {
            OnGameplayTimerChanged?.Invoke(remainingTime);

            yield return null;
            remainingTime -= Time.deltaTime;
        }

        ChangeState(GameState.Defeat);
    }

    private void StopAllTimers()
    {
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        if (gameplayTimerCoroutine != null) StopCoroutine(gameplayTimerCoroutine);
    }

    public void RestartGame()
    {
        if (currentGameState == GameState.Countdown) return;

        ChangeState(GameState.Countdown);

        P_Movement playerMovement = FindFirstObjectByType<P_Movement>();
        if (playerMovement != null) playerMovement.TeleportToStar();

        P_ColorManager playerColor = FindFirstObjectByType<P_ColorManager>();
        if (playerColor != null) playerColor.ResetPlayerMaterial();

        CapsuleManager[] allCapsules = FindObjectsByType<CapsuleManager>(FindObjectsSortMode.None);
        foreach (var capsule in allCapsules)
        {
            capsule.ResetCapsule();
        }

        CylinderManager[] allCylinders = FindObjectsByType<CylinderManager>(FindObjectsSortMode.None);
        string[] materials = { "Blue_Material", "Green_Material", "Yellow_Material", "Purple_Material", };

        for (int i = 0; i < allCylinders.Length; i++)
        {
            if (i < materials.Length)
            {
                allCylinders[i].ResetCylinderMaterial(materials[i]);
            }
        }
    }
}
