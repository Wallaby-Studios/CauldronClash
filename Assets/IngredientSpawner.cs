using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public GameObject ingredientPrefab;
    public GameObject bubblesPrefab;
    public Transform bubblesTransform;
    public VoiceLineManager voiceLineManager;
    public ItemSoundsManager itemSoundsManager;
    public BadSoundsManager badSoundsManager;
    // Start is called before the first frame update
    void Start()
    {
        badSoundsManager = GetComponentInChildren<BadSoundsManager>();
        voiceLineManager = GetComponentInChildren<VoiceLineManager>();
        itemSoundsManager = GetComponentInChildren<ItemSoundsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            GameObject i = Instantiate(ingredientPrefab);
            i.GetComponent<IngredientController>().spawner = this;
            i.transform.position = transform.position;
            i.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.5f, 1) * 100);

        }
        if (Input.GetMouseButtonDown(1))
        {
            GameObject i = Instantiate(ingredientPrefab);
            i.GetComponent<IngredientController>().spawner = this;
            i.GetComponent<IngredientController>().setBad();
            i.transform.position = transform.position;
            i.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -1) * 100);
            
        }
        */
    }

    public void SpawnGoodItem()
    {
        GameObject i = Instantiate(ingredientPrefab);
        i.GetComponent<IngredientController>().spawner = this;
        i.transform.position = transform.position;
        i.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.5f, 1) * 100);
    }

    public void SpawnBadItem()
    {
        GameObject i = Instantiate(ingredientPrefab);
        i.GetComponent<IngredientController>().spawner = this;
        i.GetComponent<IngredientController>().setBad();
        i.transform.position = transform.position;
        i.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -1) * 100);

    }

    public void BubbleOver()
    {
        GameObject b = Instantiate(bubblesPrefab);
        b.transform.position = bubblesTransform.position;
        Destroy(b, 5);
        bubblesTransform.GetComponent<AudioSource>().Play();
    }
}
