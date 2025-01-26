using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ComputerDifficulty
{
    Easy,
    Medium,
    Hard,
    Cheating
}

public class ComputerInput : EntityInput
{
    [SerializeField]
    private ComputerDifficulty difficulty;

    private float inputRate, wrongInputChance, currentTimer;

    public ComputerDifficulty Difficulty { get { return difficulty; } }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        UpdateDifficulty(difficulty);
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if(GameManager.instance.CurrentGameState == GameState.Game) {
            currentTimer -= Time.deltaTime;
            // Simulate input from the computer 
            if(currentTimer <= 0.0f) {
                // Randomize whether the input is correct (based on difficulty)
                float random = UnityEngine.Random.Range(0.0f, 1.0f);
                bool isInputCorrect = random > wrongInputChance;
                SubmitInput(isInputCorrect);
                currentTimer = inputRate;
            }
        }
    }

    public void UpdateDifficulty(ComputerDifficulty newDifficulty) {
        switch(difficulty) {
            case ComputerDifficulty.Easy:
                inputRate = 1.5f;
                wrongInputChance = 0.2f;
                break;
            case ComputerDifficulty.Medium:
                inputRate = 1.0f;
                wrongInputChance = 0.1f;
                break;
            case ComputerDifficulty.Hard:
                inputRate = 0.5f;
                wrongInputChance = 0.0f;
                break;
            case ComputerDifficulty.Cheating:
                inputRate = 0.25f;
                wrongInputChance = 0.0f;
                break;
        }

        currentTimer = inputRate;
    }

    /// <summary>
    /// Provides "input" from the computer
    /// </summary>
    /// <param name="isInputCorrect">Whether the computer's input is correct</param>
    private void SubmitInput(bool isInputCorrect) {
        GameManager.instance.CheckInput(index, isInputCorrect);
    }

    /// <summary>
    /// Move the CPU's difficulty to the previous difficulty
    /// </summary>
    /// <returns>The previous difficulty (including looping)</returns>
    public ComputerDifficulty PreviousDifficulty() {
        // Get the index of the current difficulty and decrement it
        int currentEnumIndex = (int)difficulty;
        int previousEnumIndex = currentEnumIndex - 1;
        // Check if the index needs to be looped to the end
        if(previousEnumIndex < 0) {
            previousEnumIndex = Enum.GetNames(typeof(ComputerDifficulty)).Length - 1;
        }
        // Set the difficulty and the difficulty text
        difficulty = (ComputerDifficulty)previousEnumIndex;
        UpdateDifficulty(difficulty);
        return difficulty;
    }

    /// <summary>
    /// Move the CPU's difficulty to the next difficulty
    /// </summary>
    /// <returns>The next difficulty (including looping)</returns>
    public ComputerDifficulty NextDifficulty() {
        // Get the index of the current difficulty and increment it
        int currentEnumIndex = (int)difficulty;
        int nextEnumIndex = currentEnumIndex + 1;
        // Check if the index needs to be looped to the start
        if(nextEnumIndex >= Enum.GetNames(typeof(ComputerDifficulty)).Length) {
            nextEnumIndex = 0;
        }
        // Set the difficulty and the difficulty text
        difficulty = (ComputerDifficulty)nextEnumIndex;
        UpdateDifficulty(difficulty);
        return difficulty;
    }
}
