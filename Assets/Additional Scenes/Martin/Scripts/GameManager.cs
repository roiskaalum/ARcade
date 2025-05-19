using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        None,
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

    public GameState CurrentState { get; private set; } = GameState.None;

    [SerializeField] private GameObject winAnimationObject;
    [SerializeField] private GameObject loseAnimationObject;
    [SerializeField] private GameObject menuObjects;
    [SerializeField] private GameObject gameObjects;

    private string playerName;
    private int playerScore;
    private bool continueOldScore;
    //[SerializeField] private ResetBallPosTemporary ballResetter;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => UIManager.Instance != null);

        menuObjects.SetActive(true);
        gameObjects.SetActive(false);
        SetState(GameState.MainMenu);
    }

    public void SetState(GameState newState)
    {
        Debug.Log($"Fors�ger at s�tte GameState til: {newState}, current er: {CurrentState}");

        CurrentState = newState;

        Debug.Log($"Game state changed to: {newState}");

        switch (newState)
        {
            case GameState.MainMenu:
                UIManager.Instance.HandleMainMenu(); // index 0
                break;
            case GameState.EnterName:
                UIManager.Instance.HandleEnterName(); //index 1
                break;
            case GameState.ChooseScoreOption:
                //UIManager.Instance.HandleChooseScoreOption(); //index er ikke defineret endnu 
                break;
            case GameState.Options:
                UIManager.Instance.HandleOptions(); //index 2
                break;
            case GameState.Scoreboard:
                UIManager.Instance.HandleScoreboard(); //index 3
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
                UIManager.Instance.HandlePause(); //index 5
                break;
            case GameState.SavingScore:
                SaveScore();
                break;
            case GameState.GameOver:
                UIManager.Instance.HandleGameOver(); //index 4
                break;
        }
    }

    #region Game Logic

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
        Debug.Log("VENTER: GameManager i state 'WaitingForInput'");
    }

    private void HandlePlaying()
    {
        Debug.Log("Spilleren kaster bolden");
    }

    private void HandleCheckingResult()
    {
        if (PhysicsManager.Instance == null)
        {
            Debug.LogError("PhysicsManager.Instance er NULL!");
            return;
        }

        if (BallManager.Instance == null)
        {
            Debug.LogError("BallManager.Instance er NULL!");
            return;
        }


        int cansHit = PhysicsManager.Instance.CheckCansHit();
        playerScore += cansHit;

        if (PhysicsManager.Instance.AllCansKnocked() || BallManager.Instance.OutOfBalls())
        {
            SetState(GameState.Animation);
        }
        else
        {
            //ballResetter.ResetBall(); 
            SetState(GameState.WaitingForInput);
        }
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

    #endregion

    #region Public Gameplay Flow

    public void StartGameplay(string name)
    {
        Debug.Log($"GameManager: StartGameplay() kaldt med navn: {name}");

        menuObjects?.SetActive(false);
        gameObjects?.SetActive(true);

        playerName = name;
        playerScore = 0;

        SetState(GameState.WaitingForInput);
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

        //i Raycasting under ReleaseBall() 
        //kald GameManager.Instance.OnBallThrown();
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
        Debug.Log("Game afsluttes...");
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
            (Input.GetKeyDown(KeyCode.Space) ||
             UnityEngine.InputSystem.Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true))
        {
            Debug.Log("Input registreret - går videre til Playing!");
            SetState(GameState.Playing);
        }
    }

    public void OnUIReady()
    {
        menuObjects.SetActive(true);
        gameObjects.SetActive(false);
        SetState(GameState.MainMenu);
    }
}
