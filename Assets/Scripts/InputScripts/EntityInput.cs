using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInput : MonoBehaviour {
    protected int index;

    private int completedPotions;

    public int Index {
        get { return index; }
        set { if(value >= 0) index = value; }
    }
    public int CompletedPotionsCount { get { return completedPotions; } }

    // Start is called before the first frame update
    protected virtual void Start() {

    }

    protected virtual void FixedUpdate() {
        
    }

    public void ClearPotions() {
        completedPotions = 0;
    }

    public void CompletePotion() {
        completedPotions++;
    }
}
