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

    [SerializeField]
    private GameObject mainMenuUIParent, gameUIParent, gameEndUIParent;
    [SerializeField]
    private Button mainMenuToGameButton, gameEndToMainMenuButton;
    [SerializeField]
    private TMP_Text sequenceText;
    [SerializeField]
    private GameObject arrowPrefab, playerArrowsParent;

    // Start is called before the first frame update
    void Start() {
        SetupButtons();

        gameUIParent.SetActive(false);
        gameEndUIParent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI(GameState newGameState) {
        switch(newGameState) {
            case GameState.MainMenu:
                gameEndUIParent.SetActive(false);
                mainMenuUIParent.SetActive(true);
                break;
            case GameState.Game:
                mainMenuUIParent.SetActive(false);
                gameUIParent.SetActive(true);
                CreateArrows();
                break;
            case GameState.GameEnd:
                gameUIParent.SetActive(false);
                gameEndUIParent.SetActive(true);
                break;
        }
    }

    private void SetupButtons() {
        mainMenuToGameButton.onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.Game));
        gameEndToMainMenuButton.onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.MainMenu));
    }

    public void DisplaySequence() {
        List<InputDirection> sequence = GameManager.instance.Sequence;
        string sequenceString = "";
        foreach(InputDirection direction in sequence) {
            sequenceString += direction.ToString() + "\n";
        }
        sequenceText.text = sequenceString;
    }

    public void CreateArrows() {
        // Clean up all child arrows before creating new ones
        foreach(Transform child in playerArrowsParent.transform) {
            Destroy(child.gameObject);
        }

        float xOffset = 60.0f;
        float xOffsetPerPlayer = 20.0f;
        // Create an arrow for each player
        for(int i = 0; i < GameManager.instance.Players.Length; i++) {
            Vector2 position = new Vector2(
                playerArrowsParent.transform.position.x - xOffset - (i * xOffsetPerPlayer),
                playerArrowsParent.transform.position.y
                );
            GameObject arrow = Instantiate(arrowPrefab, playerArrowsParent.transform);
            arrow.transform.position = position;
            arrow.name = string.Format("Player{0}Arrow", i + 1);
        }
    }

    public void UpdateArrow(int arrowIndex) {
        Vector2 pos = playerArrowsParent.transform.GetChild(arrowIndex).position;
        pos.y -= 40.0f;
        playerArrowsParent.transform.GetChild(arrowIndex).position = pos;
    }
}
