using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public GameObject ingredientPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject i = Instantiate(ingredientPrefab);
            i.transform.position = new Vector2(Random.Range(-8, -5), 3);
            i.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.5f, 1) * 100);

        }
        if (Input.GetMouseButtonDown(1))
        {
            GameObject i = Instantiate(ingredientPrefab);
            i.transform.position = new Vector2(Random.Range(-8, -5), 3);
            i.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -1) * 100);
            i.GetComponent<IngredientController>().setBad();
        }
    }
}
