using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ComputerDifficulty {
    Easy,
    Medium,
    Hard
}

public class ComputerInput : EntityInput {
    [SerializeField]
    private ComputerDifficulty difficulty;

    private float inputRate;
    private float wrongInputChance;

    private float currentTimer;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        switch(difficulty) {
            case ComputerDifficulty.Easy:
                inputRate = 2.0f;
                wrongInputChance = 0.4f;
                break;
            case ComputerDifficulty.Medium:
                inputRate = 1.5f;
                wrongInputChance = 0.2f;
                break;
            case ComputerDifficulty.Hard:
                inputRate = 1.0f;
                wrongInputChance = 0.0f;
                break;
        }

        currentTimer = inputRate;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if(GameManager.instance.CurrentGameState == GameState.Game) {
            currentTimer -= Time.deltaTime;
            // Simulate input from the computer 
            if(currentTimer <= 0.0f) {
                // Randomize whether the input is correct (based on difficulty)
                float random = Random.Range(0.0f, 1.0f);
                bool isInputCorrect = random > wrongInputChance;
                if(!isInputCorrect) {
                    DisableInput();
                }
                SubmitInput(isInputCorrect);
                currentTimer = inputRate;
            }
        }
    }

    /// <summary>
    /// Provides "input" from the computer
    /// </summary>
    /// <param name="isInputCorrect">Whether the computer's input is correct</param>
    private void SubmitInput(bool isInputCorrect) {
        GameManager.instance.CheckInput(index, isInputCorrect);
    }
}
