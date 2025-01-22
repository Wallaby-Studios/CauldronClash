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

    /// <summary>
    /// Set the values of the the CPU Player UI
    /// </summary>
    /// <param name="name">The name of the CPU player</param>
    /// <param name="computerInput">The component of the CPU gameObject</param>
    public void SetupValues(string name, ComputerInput computerInput) {
        cpuNameText.text = name;
        inputObject = computerInput;
    }

    /// <summary>
    /// Decrement the CPU's difficulty and update text
    /// </summary>
    private void PreviousDifficultyText() {
        ComputerDifficulty newDifficulty = inputObject.PreviousDifficulty();
        difficultyText.text = newDifficulty.ToString();
    }

    /// <summary>
    /// Increment the CPU's difficulty and update text
    /// </summary>
    private void NextDifficultyText() {
        ComputerDifficulty newDifficulty = inputObject.NextDifficulty();
        difficultyText.text = newDifficulty.ToString();
    }
}
