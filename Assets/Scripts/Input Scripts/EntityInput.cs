using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInput : MonoBehaviour {
    protected int index;
    private float inputDisabledTimerMax, inputDisabledTimer;
    private bool isInputEnabled;

    public int Index {
        get { return index; }
        set { if(value >= 0) index = value; }
    }
    public bool IsInputEnabled { get { return isInputEnabled; } }

    // Start is called before the first frame update
    protected virtual void Start() {
        inputDisabledTimerMax = 1.0f;
        inputDisabledTimer = inputDisabledTimerMax;
        isInputEnabled = true;
    }

    protected virtual void FixedUpdate() {
        if(GameManager.instance.CurrentGameState == GameState.Game) {
            // If penalized increment the timer and check if the penalized time is over
            if(!isInputEnabled) {
                inputDisabledTimer -= Time.deltaTime;
                if(inputDisabledTimer <= 0.0f) {
                    EnableInput();
                }
            }
        }
    }

    public virtual void DisableInput() {
        inputDisabledTimer = inputDisabledTimerMax;
        isInputEnabled = false;
        UIManager.instance.UpdateArrow(index, false);
    }

    public virtual void EnableInput() {
        isInputEnabled = true;
        UIManager.instance.UpdateArrow(index, true);
    }
}
