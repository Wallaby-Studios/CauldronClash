using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState {
    MainMenu,
    PlayerJoin,
    Game,
    GameEnd
}

public class GameManager : MonoBehaviour {
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
    private GameObject playersParent;
    [SerializeField]
    private GameObject computerPrefab;
    [SerializeField]
    private int minPlayerCount, sequenceLength;
    [SerializeField]
    private List<InputDirection> sequence;
    [SerializeField]
    private List<int> currentPlayerIndecies;
    [SerializeField]
    private float gameTimerMax;

    private float gameTimerCurrent;
    #endregion Fields

    #region Properties
    public GameState CurrentGameState { get { return currentGameState; } }
    public int MinPlayerCount { get { return minPlayerCount; } }
    public List<InputDirection> Sequence { get { return sequence; } }
    public float GameTimerCurrent { get { return gameTimerCurrent; } }
    #endregion Properties

    // Start is called before the first frame update
    void Start() {
        ChangeGameState(GameState.MainMenu);
    }

    // Update is called once per frame
    void Update() {

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
                break;
            case GameState.PlayerJoin:
                // Only allow joining during this screen
                playerInputManager.EnableJoining();
                break;
            case GameState.Game:
                playerInputManager.DisableJoining();
                gameTimerCurrent = gameTimerMax;
                SetupGame();
                break;
            case GameState.GameEnd:
                break;
        }

        currentGameState = newGameState;
        UIManager.instance.UpdateUI(newGameState);
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

            if(currentPlayerIndecies[playerIndex] >= sequence.Count) {
                // End the round if a player has input the last of the sequence
                GameWon(playerIndex);
            } else {
                // Otherwise, update its arrow
                UIManager.instance.AdvanceSequenceIndicator(playerIndex);
            }
            // TODO: Throw in good ingredient... YUM
        } else {
            // If the input is incorrect, disable the player from inputting further
            PlayerInputControls playerInputControls = playersParent.transform.GetChild(playerIndex).GetComponent<PlayerInputControls>();
            if(playerInputControls != null) {
                ResetProgress(playerIndex);
            }
            // TODO: Throw in onion... STINKY
        }
    }

    /// <summary>
    /// Add a CPU to the game
    /// </summary>
    public void AddCPUInput() {
        int childCount = playersParent.transform.childCount;

        if(childCount < GetComponent<PlayerInputManager>().maxPlayerCount) {
			// Create a CPU gameObject as a child of the players gameObject, name it "CPU#", and set its player index
			GameObject computer = Instantiate(computerPrefab, playersParent.transform);
			computer.name = "CPU" + childCount;
			computer.GetComponent<ComputerInput>().Index = childCount;
			// Update UI
			UIManager.instance.DisplayJoinedPlayer(childCount, computer.GetComponent<ComputerInput>());
		}
	}

	public void ResetProgress(int playerIndex) {
		currentPlayerIndecies[playerIndex] = 0;
        UIManager.instance.ResetArrow(playerIndex);
	}
	#endregion Public Methods

	#region Private Methods
	/// <summary>
	/// A method called when a player joins via the PlayerInputManager
	/// </summary>
	/// <param name="playerInput">The PlayerInput script of the new player</param>
	private void PlayerInput_onPlayerJoined(PlayerInput playerInput) {
        // Set the player's name and index
        playerInput.gameObject.name = string.Format("Player{0}", playersParent.transform.childCount);
        playerInput.GetComponent<PlayerInputControls>().Index = GetPlayerCount();
        // Move the player GameObject as a child of the players parent GameObject
        playerInput.gameObject.transform.SetParent(playersParent.transform);
        // Update UI
        UIManager.instance.DisplayJoinedPlayer(playersParent.transform.childCount - 1, null);
    }

    /// <summary>
    /// Set up initial values needed for each round
    /// </summary>
    private void SetupGame() {
        // Generate an initial input sequence and display it to both players
        sequence = GenerateSequence();
        for(int i = 0; i < playersParent.transform.childCount; i++) {
			UIManager.instance.DisplaySequence(i);
		}

        // Create a "progress" index for each player
        currentPlayerIndecies = new List<int>();
        for(int i = 0; i < playersParent.transform.childCount; i++) {
            currentPlayerIndecies.Add(0);
        }
    }

    private List<InputDirection> GenerateSequence() {
        List <InputDirection> newSequence = new List<InputDirection>();
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
        return sequence[index];
    }

    /// <summary>
    /// Move the game to the game end state
    /// </summary>
    /// <param name="winningPlayerIndex">The index of the winning player</param>
    private void GameWon(int winningPlayerIndex) {
        //Debug.Log(string.Format("Player {0} Wins!", winningPlayerIndex + 1));
        UIManager.instance.UpdateGameEndText(winningPlayerIndex);
        ChangeGameState(GameState.GameEnd);
    }
    #endregion Private Methods
}
