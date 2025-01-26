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

        playerInputManager = GetComponent<PlayerInputManager>();
        // Set up Player Input Manager player joined event trigger
        playerInputManager.onPlayerJoined += PlayerInput_onPlayerJoined;
    }
    #endregion

    #region Fields
    private GameState currentGameState;
    private PlayerInputManager playerInputManager;

    [SerializeField]
    private GameObject playersParent, computerPrefab;
    [SerializeField]
    private int minPlayerCount, sequenceLength;
    [SerializeField]
    private List<List<InputDirection>> sequences;
    [SerializeField]
    private List<int> currentPlayerIndecies, playerTotals;
    [SerializeField]
    private float gameTimerMax;
    public IngredientSpawner p0Spawner;
    public IngredientSpawner p1Spawner;
    private List<string> playerNames;
    private float gameTimerCurrent;
    #endregion Fields

    #region Properties
    public GameState CurrentGameState { get { return currentGameState; } }
    public int MinPlayerCount { get { return minPlayerCount; } }
    public List<List<InputDirection>> Sequences { get { return sequences; } }
    public List<int> PlayerTotals { get { return playerTotals; } }
    public List<string> PlayerNames { get { return playerNames; } }
    public float GameTimerCurrent { get { return gameTimerCurrent; } }
    #endregion Properties

    // Start is called before the first frame update
    void Start() {
        ChangeGameState(GameState.MainMenu);
        playerNames = new List<string>();
        //p0Spawner = FindObjectsOfType<IngredientSpawner>()[0];
        //p1Spawner = FindObjectsOfType<IngredientSpawner>()[1];

    }

    void FixedUpdate() {
        if(currentGameState == GameState.Game) {
            gameTimerCurrent -= Time.deltaTime;
            UIManager.instance.UpdateGameTimerBar(gameTimerCurrent / gameTimerMax);
            if(gameTimerCurrent <= 0.0f) {
                GameOver();
            }
        }

        if (currentGameState == GameState.PlayerJoin)
        {
            if (playersParent.transform.childCount >= 2)
            {
                playerInputManager.DisableJoining();
            }
            else
            {
                playerInputManager.EnableJoining();
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
                // Joining is disabled initally on the PIM so this is redundant but just in case 
                playerInputManager.DisableJoining();
                sequences = new List<List<InputDirection>>();
                break;
            case GameState.PlayerJoin:
                // Only allow joining during this screen
                playerInputManager.EnableJoining();
                break;
            case GameState.Game:
                playerInputManager.DisableJoining();
                SetupGame();
                break;
            case GameState.GameEnd:
                break;
        }

        currentGameState = newGameState;
        UIManager.instance.ChangeUI(newGameState);
    }

    /// <summary>
    /// Get a player by its index from the players parents
    /// </summary>
    /// <param name="index">The int index of the player</param>
    /// <returns>A GameObject of the player</returns>
    public GameObject GetPlayerByIndex(int index) {
        return playersParent.transform.GetChild(index).gameObject;
    }

    /// <summary>
    /// Get a count of all players, including CPUs
    /// </summary>
    /// <returns>An int of the number of players</returns>
    public int GetPlayerCount() {
        return playersParent.transform.childCount;
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
                // Play sound for completing a potion
                // ===========
                ResetProgress(playerIndex);
                UIManager.instance.DisplaySequence(playerIndex);
            } else {
                // ===========
                // Call method to spawn a good ingrient 
                // ===========
                if (playerIndex == 0)
                {
                    p0Spawner.SpawnGoodItem();
                }
                else
                {
                    p1Spawner.SpawnGoodItem();
                }
                // Otherwise, update its indicator
                UIManager.instance.AdvanceSequenceIndicator(playerIndex, currentPlayerIndecies[playerIndex]);
            }
        } else {
            // If the input is incorrect, disable the player from inputting further
            PlayerInputControls playerInputControls = playersParent.transform.GetChild(playerIndex).GetComponent<PlayerInputControls>();
            if(playerInputControls != null) {
                ResetProgress(playerIndex);
                // ===========
                // Call method to spawn a bad ingrient 
                // ===========
                if (playerIndex == 0)
                {
                    p0Spawner.SpawnBadItem();
                    p0Spawner.BubbleOver();
                }
                else
                {
                    p1Spawner.SpawnBadItem();
                    p1Spawner.BubbleOver();
                }
            }
        }
    }

    /// <summary>
    /// Add a CPU to the game
    /// </summary>
    public void AddCPUInput() {
        int childCount = playersParent.transform.childCount;
        Debug.Log(childCount);
        Debug.Log(GetComponent<PlayerInputManager>().maxPlayerCount);
        if(childCount < GetComponent<PlayerInputManager>().maxPlayerCount) {
            // Create a CPU gameObject as a child of the players gameObject, name it "CPU#", and set its player index
            GameObject computer = Instantiate(computerPrefab, playersParent.transform);
            computer.name = string.Format("{0} (CPU)", GeneratePlayerName());
            computer.GetComponent<ComputerInput>().Index = childCount;
            playerNames.Add(computer.name);
            // Update UI
            UIManager.instance.DisplayJoinedPlayer(childCount, computer.GetComponent<ComputerInput>());
        }
    }

    public void ResetProgress(int playerIndex) {
        currentPlayerIndecies[playerIndex] = 0;
        UIManager.instance.ResetIndicator(playerIndex);
    }
    #endregion Public Methods

    #region Private Methods
    /// <summary>
    /// A method called when a player joins via the PlayerInputManager
    /// </summary>
    /// <param name="playerInput">The PlayerInput script of the new player</param>
    private void PlayerInput_onPlayerJoined(PlayerInput playerInput) {
        // Set the player's name and index
        playerInput.gameObject.name = GeneratePlayerName();
        playerInput.GetComponent<PlayerInputControls>().Index = GetPlayerCount();
        // Move the player GameObject as a child of the players parent GameObject
        playerInput.gameObject.transform.SetParent(playersParent.transform);
        playerNames.Add(playerInput.gameObject.name);
        // Update UI
        UIManager.instance.DisplayJoinedPlayer(playersParent.transform.childCount - 1, null);
    }

    private string GeneratePlayerName() {
        List<string> firstName = new List<string>() {
            "Stinky",
            "Moldy",
            "Sweaty",
            "Smart",
            "Stupid",
            "Dumb",
            "Focused",
            "Distracted",
            "Rowdy",
            "Musty",
            "Wise",
            "Funny",
            "Nice",
            "Kind",
            "Jovial",
            "Intelligent",
            "Grumpy",
            "Sleepy"
        };
        List<string> secondName = new List<string>() {
            "Newt",
            "Salamander",
            "Iguana",
            "Gecko",
            "Dragon",
            "Beaver",
            "Otter",
            "Shrew",
            "Raccoon",
            "Possum",
            "Mouse",
            "Rat",
            "Squirrel",
            "Chipmunk",
            "Hyrax"
        };

        return string.Format(
            "{0} {1}",
            firstName[UnityEngine.Random.Range(0, firstName.Count)],
            secondName[UnityEngine.Random.Range(0, secondName.Count)]);
    }

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

        for(int i = 0; i < playersParent.transform.childCount; i++) {
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
            gameEndTitleText = string.Format("{0} Wins", playerNames[0]);
        } else if(playerTotals[0] < playerTotals[1]) {
            gameEndTitleText = string.Format("{0} Wins", playerNames[1]);
        }

        UIManager.instance.UpdateGameEndText(gameEndTitleText);
        ChangeGameState(GameState.GameEnd);
    }
    #endregion Private Methods
}
