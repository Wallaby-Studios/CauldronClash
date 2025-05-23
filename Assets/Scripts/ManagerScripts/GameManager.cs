using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum GameState
{
    MainMenu,
    PlayerJoin,
    Game,
    GameEnd
}

public class GameManager : MonoBehaviour
{
    #region Singleton Code
    // A public reference to this script
    public static GameManager instance = null;

    // Awake is called even before start
    private void Awake() {
        // If the reference for this script is null, assign it this script
        if(instance == null)
            instance = this;
        // If the reference is to something else (it already exists)
        // than this is not needed, thus destroy it
        else if(instance != this)
            Destroy(gameObject);
    }
    #endregion

    #region Fields
    private GameState currentGameState;

    [SerializeField]
    private int sequenceLength;
    [SerializeField]
    private List<List<InputDirection>> sequences;
    [SerializeField]
    private List<int> currentPlayerIndecies, playerTotals;
    [SerializeField]
    private float gameTimerMax;
    [SerializeField]
    private List<IngredientSpawner> ingredientSpawners;
    private float gameTimerCurrent;
    #endregion Fields

    #region Properties
    public GameState CurrentGameState { get { return currentGameState; } }
    public List<List<InputDirection>> Sequences { get { return sequences; } }
    public List<int> PlayerTotals { get { return playerTotals; } }
    public float GameTimerCurrent { get { return gameTimerCurrent; } }
    #endregion Properties

    // Start is called before the first frame update
    void Start() {
        ChangeGameState(GameState.MainMenu);
    }

    void FixedUpdate() {
        if(currentGameState == GameState.Game) {
            gameTimerCurrent -= Time.deltaTime;
            UIManager.instance.UpdateGameTimerBar(gameTimerCurrent / gameTimerMax);
            if(gameTimerCurrent <= 0.0f) {
                GameOver();
            }
        }   
    }

    #region Public Methods
    /// <summary>
    /// Changes the game state of the game, similar to menus
    /// </summary>
    /// <param name="newGameState">The new state of the game</param>
    public void ChangeGameState(GameState newGameState) {
        switch(newGameState) {
            case GameState.MainMenu:
                sequences = new List<List<InputDirection>>();
                break;
            case GameState.PlayerJoin:
                break;
            case GameState.Game:
                SetupGame();
                break;
            case GameState.GameEnd:
                break;
        }

        currentGameState = newGameState;
        PlayerManager.instance.ChangeJoiningState(currentGameState);
        UIManager.instance.ChangeUI(newGameState);
    }

    /// <summary>
    /// Checks provided input for the specified player
    /// </summary>
    /// <param name="playerIndex">The int index corresponding to the player</param>
    /// <param name="inputDirection">The input pressed by the player</param>
    public void CheckInput(int playerIndex, InputDirection inputDirection) {
        if(currentGameState == GameState.Game) {
            InputDirection nextDirection = GetNextKeyForPlayer(playerIndex);
            bool isInputCorrect = nextDirection == inputDirection;
            CheckInput(playerIndex, isInputCorrect);
        }
    }

    /// <summary>
    /// Checks provided input for the specified player
    /// </summary>
    /// <param name="playerIndex">The int index corresponding to the player</param>
    /// <param name="isInputCorrect">A Boolean whether the input sent is the next required input direction</param>
    public void CheckInput(int playerIndex, bool isInputCorrect) {
        if(isInputCorrect) {
            // Increment the player to the next input in the sequence 
            currentPlayerIndecies[playerIndex]++;

            // If a player has input the last of the sequence
            if(currentPlayerIndecies[playerIndex] >= sequences[playerTotals[playerIndex]].Count) {
                // Add to the player's total and reset the sequence
                playerTotals[playerIndex]++;
                if(playerTotals[playerIndex] >= sequences.Count) {
                    sequences.Add(GenerateSequence());
                }
                // ===========
                // TODO: Play sound for completing a potion
                // ===========
                ResetProgress(playerIndex, false);
                UIManager.instance.DisplaySequence(playerIndex);
            } else {
                // Otherwise, spawn a good item and update its indicator
                ingredientSpawners[playerIndex].SpawnGoodItem();
                UIManager.instance.AdvanceSequenceIndicator(playerIndex, currentPlayerIndecies[playerIndex]);
            }
        } else {
            // If the input is incorrect, reset the player's progress
            ingredientSpawners[playerIndex].SpawnBadItem();
            ingredientSpawners[playerIndex].BubbleOver();
            ResetProgress(playerIndex, true);
        }
    }

    public void ResetProgress(int playerIndex, bool receivedWrongInput) {
        currentPlayerIndecies[playerIndex] = 0;
        UIManager.instance.ResetIndicator(playerIndex, receivedWrongInput);
    }
    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// Set up initial values needed for each round
    /// </summary>
    private void SetupGame() {
        // Set the game timer
        ResetGameTimer();

        // Create a "progress" index and total counter for each player
        currentPlayerIndecies = new List<int>();
        playerTotals = new List<int>();
        // Generate an initial input sequence and display it to both players
        sequences.Add(GenerateSequence());

        for(int i = 0; i < PlayerManager.instance.GetPlayerCount(); i++) {
            currentPlayerIndecies.Add(0);
            playerTotals.Add(0);
            UIManager.instance.DisplaySequence(i);
        }
    }

    private List<InputDirection> GenerateSequence() {
        List<InputDirection> newSequence = new List<InputDirection>();
        for(int i = 0; i < sequenceLength; i++) {
            int randomIndex = UnityEngine.Random.Range(0, Enum.GetNames(typeof(InputDirection)).Length);
            InputDirection randomInputKey = (InputDirection)randomIndex;
            newSequence.Add(randomInputKey);
        }
        return newSequence;
    }

    /// <summary>
    /// Gets the next required input for the specified player
    /// </summary>
    /// <param name="playerIndex">The int index corresponding to the player</param>
    /// <returns>The next input needed by the player</returns>
    private InputDirection GetNextKeyForPlayer(int playerNum) {
        int index = currentPlayerIndecies[playerNum];
        return sequences[playerTotals[playerNum]][index];
    }

    private void ResetGameTimer() {
        gameTimerCurrent = gameTimerMax;
        UIManager.instance.UpdateGameTimerBar(1.0f);
    }

    /// <summary>
    /// Move the game to the game end state
    /// </summary>
    private void GameOver() {
        string gameEndTitleText = "Tie Game";
        if(playerTotals[0] > playerTotals[1]) {
            gameEndTitleText = string.Format("{0} Wins", PlayerManager.instance.PlayerNames[0]);
        } else if(playerTotals[0] < playerTotals[1]) {
            gameEndTitleText = string.Format("{0} Wins", PlayerManager.instance.PlayerNames[1]);
        }

        UIManager.instance.UpdateGameEndText(gameEndTitleText);
        ChangeGameState(GameState.GameEnd);
    }
    #endregion Private Methods
}
