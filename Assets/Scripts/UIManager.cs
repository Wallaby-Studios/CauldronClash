using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
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

    [SerializeField]
    private GameObject mainMenuUIParent, playerJoinUIParent, gameUIParent, gameEndUIParent;
    [SerializeField]
    private Button mainMenuToPlayerJoinButton, playerJoinToGameButton, fillWithCPUsButton, gameEndToMainMenuButton;
    [SerializeField]
    private TMP_Text playerListText, sequenceText;
    [SerializeField]
    private GameObject arrowPrefab, playerArrowsParent;

    // Start is called before the first frame update
    void Start() {
        SetupButtons();

        playerJoinUIParent.SetActive(false);
        gameUIParent.SetActive(false);
        gameEndUIParent.SetActive(false);
    }

    // Update is called once per frame
    void Update() {

    }

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
                UpdatePlayersList();
                UpdatePlayerJoinToGameButton();
                break;
            case GameState.Game:
                playerJoinUIParent.SetActive(false);
                gameUIParent.SetActive(true);
                CreateArrows();
                break;
            case GameState.GameEnd:
                gameUIParent.SetActive(false);
                gameEndUIParent.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Set up button onClicks
    /// </summary>
    private void SetupButtons() {
        mainMenuToPlayerJoinButton.onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.PlayerJoin));
        playerJoinToGameButton.onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.Game));
        fillWithCPUsButton.onClick.AddListener(GameManager.instance.FillPlayersWithCPUs);
        gameEndToMainMenuButton.onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.MainMenu));
    }

    /// <summary>
    /// Show the sequence as a column of strings
    /// </summary>
    public void DisplaySequence() {
        List<InputDirection> sequence = GameManager.instance.Sequence;
        string sequenceString = "";
        foreach(InputDirection direction in sequence) {
            sequenceString += direction.ToString() + "\n";
        }
        sequenceText.text = sequenceString;
    }

    /// <summary>
    /// Create an arrow for each player to indicate what input they are on
    /// </summary>
    public void CreateArrows() {
        // Clean up all child arrows before creating new ones
        foreach(Transform child in playerArrowsParent.transform) {
            Destroy(child.gameObject);
        }

        float xOffset = 60.0f;
        float xOffsetPerPlayer = 20.0f;
        // Create an arrow for each player
        for(int i = 0; i < GameManager.instance.GetPlayerCount(); i++) {
            Vector2 position = new Vector2(
                playerArrowsParent.transform.position.x - xOffset - (i * xOffsetPerPlayer),
                playerArrowsParent.transform.position.y
                );
            GameObject arrow = Instantiate(arrowPrefab, playerArrowsParent.transform);
            arrow.transform.position = position;
            arrow.name = string.Format("Player{0}Arrow", i + 1);
        }
    }

    /// <summary>
    /// Advance the position of the arrow
    /// </summary>
    /// <param name="arrowIndex">The int index of the arrow that correspondes to the player</param>
    public void AdvanceArrow(int arrowIndex) {
        Vector2 pos = playerArrowsParent.transform.GetChild(arrowIndex).position;
        pos.y -= 40.0f;
        playerArrowsParent.transform.GetChild(arrowIndex).position = pos;
    }

    /// <summary>
    /// Updates the color of the arrow
    /// </summary>
    /// <param name="arrowIndex">The int index of the arrow that correspondes to the player</param>
    /// <param name="enabled">Whether the player is currently penalized</param>
    public void UpdateArrow(int arrowIndex, bool enabled) {
        // Red if penalized, white if not
        playerArrowsParent.transform.GetChild(arrowIndex).GetComponent<Image>().color = enabled ? Color.white : Color.red;
    }

    /// <summary>
    /// Update the player list Text UI
    /// </summary>
    public void UpdatePlayersList() {
        int playerCount = GameManager.instance.GetPlayerCount();
        // If there is at least one player, display it first
        playerListText.text = playerCount > 0 ? DisplayPlayerName(GameManager.instance.GetPlayerByIndex(0).name, 0) : "";

        // Display all other players with 2 line breaks
        for(int i = 1; i < playerCount; i++) {
            string playerName = DisplayPlayerName(GameManager.instance.GetPlayerByIndex(i).name, i);
            playerListText.text += string.Format("\n\n{0}", playerName);
        }
    }

    /// <summary>
    /// Properly display a player or CPU's name in the list of players
    /// </summary>
    /// <param name="objectName">The string name of the player object</param>
    /// <param name="index">The int index of the object in the player list</param>
    /// <returns></returns>
    private string DisplayPlayerName(string objectName, int index) {
        if(objectName.Contains("Player")) {
            return string.Format("Player {0}", index + 1);
        } else if(objectName.Contains("CPU")) {
            return string.Format("CPU {0}", index + 1);
        }

        return string.Format("Object {0}", index + 1);
    }

    /// <summary>
    /// Updates the PlayerJoinToGame Button based on if there are enough players joined
    /// </summary>
    public void UpdatePlayerJoinToGameButton() {
        playerJoinToGameButton.interactable = GameManager.instance.GetPlayerCount() >= GameManager.instance.MinPlayerCount;
    }
}
