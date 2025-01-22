using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CPUPlayerUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text cpuNameText, difficultyText;
    [SerializeField]
    private Button previousDifficultyButton, nextDifficultyButton;
    [SerializeField]
    private ComputerInput inputObject;
    private ComputerDifficulty difficulty;

    // Start is called before the first frame update
    void Start()
    {
        difficulty = ComputerDifficulty.Easy;
        difficultyText.text = difficulty.ToString();

        previousDifficultyButton.onClick.AddListener(PreviousDifficultyText);
        nextDifficultyButton.onClick.AddListener(NextDifficultyText);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupValues(string name, ComputerInput computerInput) {
        cpuNameText.text = name;
        inputObject = computerInput;
    }

    private void PreviousDifficultyText() {
        ComputerDifficulty newDifficulty = inputObject.PreviousDifficulty();
        difficultyText.text = newDifficulty.ToString();
    }

    private void NextDifficultyText() {
        ComputerDifficulty newDifficulty = inputObject.NextDifficulty();
        difficultyText.text = newDifficulty.ToString();
    }
}
