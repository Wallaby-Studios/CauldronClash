using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IngredientController : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    VoiceLineManager voiceLineManager;
    ItemSoundsManager itemSoundsManager;
    BadSoundsManager badSoundsManager;
    public int goodClipPercentage;
    public IngredientSpawner spawner;
    bool isBad = false;
    // Start is called before the first frame update

    public IngredientController(IngredientSpawner s)
    {
        spawner = s;
    }    
    void Start()
    {
        if (isBad == true)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[4];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Count - 1)];
        }
        if (spawner != null)
        {
            badSoundsManager = spawner.badSoundsManager;
            voiceLineManager = spawner.voiceLineManager;
            itemSoundsManager = spawner.itemSoundsManager;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setBad()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[4];
        isBad = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cauldron"))
        {
            itemSoundsManager.playSplash();
            if (isBad)
            {
                
                voiceLineManager.playBadClip();
                badSoundsManager.PlayBadSound();
                isBad = false;
            }
            else
            {
                int randomizer = Random.Range(0, 100);
                if (goodClipPercentage <= randomizer)
                {
                    voiceLineManager.playGoodClip();
                }

            }
            Destroy(gameObject);
        }
     
    }
}
