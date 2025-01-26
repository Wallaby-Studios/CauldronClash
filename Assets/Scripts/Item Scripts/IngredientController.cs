using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IngredientController : MonoBehaviour
{
    VoiceLineManager voiceLineManager;
    ItemSoundsManager itemSoundsManager;
    BadSoundsManager badSoundsManager;
    public int goodClipPercentage;
    bool isBad = false;
    // Start is called before the first frame update
    void Start()
    {
        badSoundsManager = FindObjectOfType<BadSoundsManager>();
        voiceLineManager = FindObjectOfType<VoiceLineManager>();
        itemSoundsManager = FindObjectOfType<ItemSoundsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            transform.position = new Vector2(Random.Range(-8,-3), 3);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0.25f, 1f) * 100);
        }
        if (Input.GetMouseButtonDown(1))
        {
            transform.position = new Vector2(Random.Range(-8,-3), 3);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            isBad = true;
        }
        */
    }

    public void setBad()
    {
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
