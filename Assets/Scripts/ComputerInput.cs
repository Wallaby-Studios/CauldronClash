using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ComputerDifficulty {
    Easy,
    Medium,
    Hard
}

public class ComputerInput : MonoBehaviour
{
    [SerializeField]
    private ComputerDifficulty difficulty;

    private float inputRate;
    private float wrongInputChance;

    private float currentTimer;

    // Start is called before the first frame update
    void Start()
    {
        switch(difficulty) {
            case ComputerDifficulty.Easy:
                inputRate = 2.0f;
                wrongInputChance = 0.4f;
                break;
            case ComputerDifficulty.Medium:
                inputRate = 1.5f;
                wrongInputChance = 0.3f;
                break;
            case ComputerDifficulty.Hard:
                inputRate = 1.0f;
                wrongInputChance = 0.2f;
                break;
        }

        currentTimer = inputRate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() {
        currentTimer -= Time.deltaTime;
        if(currentTimer <= 0.0f) {
            float random = Random.Range(0.0f, 1.0f);
            bool isInputCorrect = random > wrongInputChance;
            SubmitInput(isInputCorrect);
            currentTimer = inputRate;
        }
    }

    private void SubmitInput(bool isInputCorrect) {
        GameManager.instance.CheckInput(1, isInputCorrect);
    }
}
