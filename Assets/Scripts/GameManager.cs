using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
	MainMenu,
	Game,
	GameEnd
}

public class GameManager : MonoBehaviour
{
	#region Singleton Code
	// A public reference to this script
	public static GameManager instance = null;

	// Awake is called even before start
	private void Awake()
	{
		// If the reference for this script is null, assign it this script
		if(instance == null)
			instance = this;
		// If the reference is to something else (it already exists)
		// than this is not needed, thus destroy it
		else if(instance != this)
			Destroy(gameObject);
	}
	#endregion

	private GameObject[] players;
	private GameState currentGameState;

	[SerializeField]
	private GameObject computerPrefab;
	[SerializeField]
	private int minPlayerCount, sequenceLength;
	[SerializeField]
	private List<InputDirection> sequence;
	[SerializeField]
	private List<int> currentPlayerIndecies;

	public GameObject[] Players { get { return players; } }
	public GameState CurrentGameState { get { return currentGameState; } }
	public List<InputDirection> Sequence { get { return sequence; } }

	// Start is called before the first frame update
	void Start()
    {
		ChangeGameState(GameState.MainMenu);
	}

    // Update is called once per frame
    void Update()
    {

	}

	/// <summary>
	/// Changes the game state of the game, similar to menus
	/// </summary>
	/// <param name="newGameState">The new state of the game</param>
	public void ChangeGameState(GameState newGameState) {
		switch(newGameState) {
			case GameState.MainMenu:
				break;
			case GameState.Game:
				SetupGame();
				break;
			case GameState.GameEnd:
				break;
		}

		currentGameState = newGameState;
		UIManager.instance.UpdateUI(newGameState);
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
				Debug.Log(string.Format("Player {0} Wins!", playerIndex));
				ChangeGameState(GameState.GameEnd);
            } else {
				// Otherwise, update its arrow
				UIManager.instance.UpdateArrow(playerIndex);
            }
		} else {
			// Penalize the player for an incorrect input
			PlayerInputControls playerInputControls = players[playerIndex].GetComponent<PlayerInputControls>();
			if(playerInputControls != null) {
				playerInputControls.Penalize();
			}
        }
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
	/// Set up initial values needed for each round
	/// </summary>
	private void SetupGame() {
		// Generate input sequence
		sequence = new List<InputDirection>();
		for(int i = 0; i < sequenceLength; i++) {
			int randomIndex = UnityEngine.Random.Range(0, Enum.GetNames(typeof(InputDirection)).Length);
			InputDirection randomInputKey = (InputDirection)randomIndex;
			sequence.Add(randomInputKey);
		}
		UIManager.instance.DisplaySequence();

		// Get all "Players" in the scene - includes Computers
		players = GameObject.FindGameObjectsWithTag("Player");

		// Check if enough players are created, if not, fill in with computers
        int numberOfPlayers = Mathf.Max(minPlayerCount, players.Length);
        currentPlayerIndecies = new List<int>();
		for(int i = 0; i < numberOfPlayers; i++) {
			// Create a Computer until the minimum players are hit
            if(i >= players.Length) {
                GameObject computer = Instantiate(computerPrefab);
				computer.name = "Computer" + i;
				computer.GetComponent<ComputerInput>().Index = i;
            }
            currentPlayerIndecies.Add(0);
		}

		// Re - Get all "Players" in the scene - includes Computers
		players = GameObject.FindGameObjectsWithTag("Player");
	}
}
