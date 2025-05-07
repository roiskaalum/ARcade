using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        MainMenu,
        EnterName,
        ChooseScoreOption,
        Options,
        Scoreboard,
        ARScanning,
        ARPlacement,
        ARTrackingLost,
        WaitingForInput,
        Playing,
        CheckingResult,
        Animation,
        Pause,
        SavingScore,
        GameOver
    }

    public GameState CurrentState { get; private set; }

    [SerializeField] private GameObject winAnimationObject;
    [SerializeField] private GameObject loseAnimationObject;
    [SerializeField] private GameObject menuObjects;
    [SerializeField] private GameObject gameObjects;

    private string playerName;
    private int playerScore;
    private bool continueOldScore;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        menuObjects.SetActive(true);
        gameObjects.SetActive(false);
        SetState(GameState.MainMenu);
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        Debug.Log($"Game state changed to: {newState}");

        switch (newState)
        {
            case GameState.MainMenu:
                HandleMainMenu();
                break;

            case GameState.EnterName:
                HandleEnterName();
                break;

            case GameState.ChooseScoreOption:
                HandleChooseScoreOption();
                break;

            case GameState.Options:
                HandleOptions();
                break;

            case GameState.Scoreboard:
                HandleScoreboard();
                break;

            case GameState.ARScanning:
                HandleARScanning();
                break;

            case GameState.ARPlacement:
                HandleARPlacement();
                break;

            case GameState.ARTrackingLost:
                HandleARTrackingLost();
                break;

            case GameState.WaitingForInput:
                HandleWaitingForInput();
                break;

            case GameState.Playing:
                HandlePlaying();
                break;

            case GameState.CheckingResult:
                HandleCheckingResult();
                break;

            case GameState.Animation:
                StartCoroutine(PlayEndAnimation());
                break;

            case GameState.Pause:
                HandlePause();
                break;

            case GameState.SavingScore:
                SaveScore();
                break;

            case GameState.GameOver:
                HandleGameOver();
                break;
        }
    }

    //handlers flytter vi til UIManager eller whatever
    #region State Handlers

    private void HandleMainMenu()
    {
        menuObjects.SetActive(true);
        gameObjects.SetActive(false);

        UIManager.Instance.OnButtonPressed(0); //MainMenu panel
    }

    private void HandleEnterName()
    {
        UIManager.Instance.OnButtonPressed(1); //Enter Name panelet
    }

    private void HandleChooseScoreOption()
    {
        UIManager.Instance.OnButtonPressed(2); //ChooseScoreOption panel
    }

    private void HandleOptions()
    {
        UIManager.Instance.OnButtonPressed(3); //Options panel
    }

    private void HandleScoreboard()
    {
        UIManager.Instance.OnButtonPressed(4); //Scoreboard panel
    }

    private void HandleARScanning()
    {
        Debug.Log("ARScanning: Søger efter plane");
    }

    private void HandleARPlacement()
    {
        Debug.Log("ARPlacement: Vent på brugertryk");
    }

    private void HandleARTrackingLost()
    {
        Debug.LogWarning("Tracking mistet");
    }

    private void HandleWaitingForInput()
    {
        Debug.Log("Vent på kast");
    }

    private void HandlePlaying()
    {
        Debug.Log("Spilleren kaster bolden");
    }

    private void HandleCheckingResult()
    {
        int cansHit = PhysicsManager.Instance.CheckCansHit();
        playerScore += cansHit;

        if (PhysicsManager.Instance.AllCansKnocked() || BallManager.Instance.OutOfBalls())
            SetState(GameState.Animation);
        else
            SetState(GameState.WaitingForInput);
    }

    private IEnumerator PlayEndAnimation()
    {
        if (PhysicsManager.Instance.AllCansKnocked())
        {
            winAnimationObject.SetActive(true);
        }
        else
        {
            loseAnimationObject.SetActive(true);
        }
        yield return new WaitForSeconds(2f);

        SetState(GameState.SavingScore);
    }

    private void HandlePause()
    {
        UIManager.Instance.OnButtonPressed(5);
    }

    private void HandleGameOver()
    {
        UIManager.Instance.OnButtonPressed(6);
    }

    #endregion

    //flowet: overgangene 
    #region Public Gameplay Flow

    public void StartGameplay(string name)
    {
        playerName = name;
        playerScore = 0;
        menuObjects.SetActive(false);
        gameObjects.SetActive(true);

        SetState(GameState.WaitingForInput);
    }

    public void StartGuestGame()
    {
        string guestName = "Guest" + Random.Range(1000, 9999);

        StartGameplay(guestName);
    }

    public void OnScoreOptionSelected(bool continuePrevious)
    {
        continueOldScore = continuePrevious;
        playerScore = continuePrevious ? SaveManager.Instance.LoadScore(playerName) : 0;

        menuObjects.SetActive(false);
        gameObjects.SetActive(true);

        SetState(GameState.WaitingForInput);
    }

    public void OnBallThrown()
    {
        SetState(GameState.CheckingResult);
    }

    public void SaveScore()
    {
        SaveManager.Instance.SaveScore(playerName, playerScore);

        SetState(GameState.GameOver);
    }

    public void RestartGame()
    {
        playerScore = 0;

        SetState(GameState.WaitingForInput);
    }

    public void ExitGame()
    {
        Debug.Log("Game exiting...");

        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        menuObjects.SetActive(true);
        gameObjects.SetActive(false);

        SetState(GameState.MainMenu);
    }

    #endregion

    private void Update()
    {
        if (CurrentState == GameState.WaitingForInput &&
            UnityEngine.InputSystem.Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true)
        {
            SetState(GameState.Playing);
        }
    }
}
