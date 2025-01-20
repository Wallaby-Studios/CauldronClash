using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	[SerializeField]
	private int playerCount;
	[SerializeField]
	private int length;
	[SerializeField]
	private List<InputDirection> sequence;
	[SerializeField]
	private List<int> currentPlayerIndecies;

	[SerializeField]
	private InputDirection nextDirection;

	// Start is called before the first frame update
	void Start()
    {
		for(int i = 0; i < length; i++) {
			int randomIndex = UnityEngine.Random.Range(0, Enum.GetNames(typeof(InputDirection)).Length);
			InputDirection randomInputKey = (InputDirection)randomIndex;
			sequence.Add(randomInputKey);
        }

		currentPlayerIndecies = new List<int>();
		for(int i = 0; i < playerCount; i++) {
			currentPlayerIndecies.Add(0);
		}
	}

    // Update is called once per frame
    void Update()
    {
		nextDirection = sequence[currentPlayerIndecies[0]];

	}

	public InputDirection GetNextKeyForPlayer(int playerNum) {
		int index = currentPlayerIndecies[playerNum];
		return sequence[index];
    }

	public void CheckInput(int playerNum, InputDirection inputDirection) {
		InputDirection nextDirection = GetNextKeyForPlayer(playerNum);
		bool isInputCorrect = nextDirection == inputDirection;
		CheckInput(playerNum, isInputCorrect);
    }

	public void CheckInput(int playerNum, bool isInputCorrect) {
		if(isInputCorrect) {
			currentPlayerIndecies[playerNum]++;
			Debug.Log(string.Format("Player {0} is on input {1} out of {2}", playerNum, currentPlayerIndecies[playerNum], sequence.Count));
		}
	}
}
