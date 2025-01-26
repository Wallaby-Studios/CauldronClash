using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneObjectsManager : MonoBehaviour
{
    public List<GameObject> ch = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.CurrentGameState == GameState.Game)
        {
            foreach (Transform child in transform)
            {
                ch.Add(child.gameObject);
            }
        }
    }
}
