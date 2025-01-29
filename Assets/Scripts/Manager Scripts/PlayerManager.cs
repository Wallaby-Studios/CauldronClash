using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    #region Singleton Code
    // A public reference to this script
    public static PlayerManager instance = null;

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

    private PlayerInputManager playerInputManager;

    [SerializeField]
    private GameObject playersParent, computerPrefab;
    [SerializeField]
    private int minPlayerCount;

    private List<string> playerNames;

    public int MinPlayerCount { get { return minPlayerCount; } }
    public List<string> PlayerNames { get { return playerNames; } }

    // Start is called before the first frame update
    void Start() {
        playerNames = new List<string>();
    }

    public void ChangeJoiningState(GameState currentGameState) {
        switch(currentGameState) {
            case GameState.PlayerJoin:
                playerInputManager.EnableJoining();
                break;
            default:
                playerInputManager.DisableJoining();
                break;
        }
    }

    public void UpdatePlayerJoining(bool canPlayersJoin) {
        if(canPlayersJoin) {
            playerInputManager.EnableJoining();
        } else {
            playerInputManager.DisableJoining();
        }
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
    /// Add a CPU to the game
    /// </summary>
    public void AddCPUInput() {
        int childCount = playersParent.transform.childCount;
        if(childCount < GetComponent<PlayerInputManager>().maxPlayerCount) {
            // Create a CPU gameObject as a child of the players gameObject, name it "CPU#", and set its player index
            GameObject computer = Instantiate(computerPrefab, playersParent.transform);
            computer.name = string.Format("{0} (CPU)", GeneratePlayerName());
            computer.GetComponent<ComputerInput>().Index = childCount;
            playerNames.Add(computer.name);
            // Restrict other players joining if max player count is reached
            if(GetPlayerCount() >= playerInputManager.maxPlayerCount) {
                playerInputManager.DisableJoining();
            }
            // Update UI
            UIManager.instance.DisplayJoinedPlayer(childCount, computer.GetComponent<ComputerInput>());
        }
    }

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
        // Restrict other players joining if max player count is reached
        if(GetPlayerCount() >= playerInputManager.maxPlayerCount) {
            playerInputManager.DisableJoining();
        }
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
}
