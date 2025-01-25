using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	#region Singleton Code
	// A public reference to this script
	public static UIManager instance = null;

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

	[SerializeField]    // Menu UI Parents
	private GameObject mainMenuUIParent, playerJoinUIParent, gameUIParent, gameEndUIParent;
	[SerializeField]    // Buttons
	private Button mainMenuToPlayerJoinButton, playerJoinToGameButton, addCPUButton, gameEndToMainMenuButton;
	[SerializeField]    // Text
	private TMP_Text gameEndTitle;
	[SerializeField]	// GameObject Parents
	private GameObject playerListParent, playerSequenceParent;
	[SerializeField]    // Prefabs
	private GameObject playerListPrefab, cPUPlayerListPrefab;
	[SerializeField]	// Arrow Sprites
	private GameObject arrowUpPrefab, arrowDownPrefab, arrowLeftPrefab, arrowRightPrefab;

	private Dictionary<InputDirection, GameObject> inputDirectionArrowMap;
	private float sequenceArrowXOffset;

	// Start is called before the first frame update
	void Start() {
		SetupButtons();

		inputDirectionArrowMap = new Dictionary<InputDirection, GameObject>();
		inputDirectionArrowMap.Add(InputDirection.Up, arrowUpPrefab);
		inputDirectionArrowMap.Add(InputDirection.Down, arrowDownPrefab);
		inputDirectionArrowMap.Add(InputDirection.Left, arrowLeftPrefab);
		inputDirectionArrowMap.Add(InputDirection.Right, arrowRightPrefab);

		sequenceArrowXOffset = 90.0f;

		playerJoinUIParent.SetActive(false);
		gameUIParent.SetActive(false);
		gameEndUIParent.SetActive(false);
	}

	// Update is called once per frame
	void Update() {

	}

	#region Public Methods
	/// <summary>
	/// Update the UI based on a new game state
	/// </summary>
	/// <param name="newGameState">The new game state</param>
	public void UpdateUI(GameState newGameState) {
		switch(newGameState) {
			case GameState.MainMenu:
				gameEndUIParent.SetActive(false);
				mainMenuUIParent.SetActive(true);
				break;
			case GameState.PlayerJoin:
				mainMenuUIParent.SetActive(false);
				playerJoinUIParent.SetActive(true);
				UpdatePlayerJoinToGameButton();
				break;
			case GameState.Game:
				playerJoinUIParent.SetActive(false);
				gameUIParent.SetActive(true);
				for(int i = 0; i < playerSequenceParent.transform.childCount; i++) {
					ResetArrow(i);
				}
				break;
			case GameState.GameEnd:
				gameUIParent.SetActive(false);
				gameEndUIParent.SetActive(true);
				break;
		}
	}

	public void DisplaySequence(int playerIndex) {
		List<InputDirection> sequence = GameManager.instance.Sequence;

		for(int i = 0; i < sequence.Count; i++) {
			GameObject arrowObjectPrefab = inputDirectionArrowMap[sequence[i]];
			Vector2 position = new Vector2(sequenceArrowXOffset * i, 0.0f);
			Transform parentTransform = playerSequenceParent.transform.GetChild(playerIndex).GetChild(1);
			GameObject arrowObject = Instantiate(arrowObjectPrefab, parentTransform);
			arrowObject.transform.localPosition = position;
		}
	}

	/// <summary>
	/// Advance the position of the arrow
	/// </summary>
	/// <param name="playerIndex">The int index of the player</param>
	public void AdvanceSequenceIndicator(int playerIndex) {
		Vector2 pos = playerSequenceParent.transform.GetChild(playerIndex).GetChild(0).position;
		pos.x += sequenceArrowXOffset;
		playerSequenceParent.transform.GetChild(playerIndex).GetChild(0).position = pos;
	}

	/// <summary>
	/// Create a UI element when a player joins (player or CPU)
	/// </summary>
	/// <param name="index">The int index of the player joined</param>
	/// <param name="inputComponent">The component of the CPU's input. Possibly null if a player has joined</param>
	public void DisplayJoinedPlayer(int index, ComputerInput inputComponent) {
		float yDiff = -80.0f;
		Vector2 position = new Vector2(0.0f, index * yDiff);

		if(inputComponent != null) {
			// If inputComponent exists, a CPU has been added to the game
			GameObject cPUPlayerListItem = Instantiate(
				cPUPlayerListPrefab,
				Vector2.zero,
				Quaternion.identity,
				playerListParent.transform);
			cPUPlayerListItem.transform.localPosition = position;
			cPUPlayerListItem.GetComponent<CPUPlayerUI>().SetupValues(
				string.Format("CPU {0}", index + 1),
				inputComponent);
		} else {
			// If inputComponent is null, a non-CPU player has joined
			GameObject playerTextObject = Instantiate(
				playerListPrefab,
				Vector2.zero,
				Quaternion.identity,
				playerListParent.transform);
			playerTextObject.transform.localPosition = position;
			playerTextObject.GetComponent<TMP_Text>().text = string.Format("Player {0}", index + 1);
		}

		UpdatePlayerJoinToGameButton();
	}

	/// <summary>
	/// Updates the PlayerJoinToGame Button based on if there are enough players joined
	/// </summary>
	public void UpdatePlayerJoinToGameButton() {
		playerJoinToGameButton.interactable = GameManager.instance.GetPlayerCount() >= GameManager.instance.MinPlayerCount;
	}

	/// <summary>
	/// Update the Game End Title Text
	/// </summary>
	/// <param name="winningPlayerIndex">The index of the winning player</param>
	public void UpdateGameEndText(int winningPlayerIndex) {
		gameEndTitle.text = string.Format("Player {0} Wins!", winningPlayerIndex + 1);
	}

	public void ResetArrow(int playerIndex) {
		playerSequenceParent.transform.GetChild(playerIndex).GetChild(0).localPosition = new Vector2(0.0f, 0.0f);
	}
	#endregion Public Methods

	/// <summary>
	/// Set up button onClicks
	/// </summary>
	private void SetupButtons() {
		mainMenuToPlayerJoinButton.onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.PlayerJoin));
		playerJoinToGameButton.onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.Game));
		addCPUButton.onClick.AddListener(GameManager.instance.AddCPUInput);
		gameEndToMainMenuButton.onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.MainMenu));
	}
}
