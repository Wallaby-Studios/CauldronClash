using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSoundsManager : MonoBehaviour
{
    AudioSource audioSource;
    public List<AudioClip> clips = new List<AudioClip>();
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clips[Random.Range(0, clips.Count - 1)];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playSplash()
    {
        audioSource.clip = clips[Random.Range(0, clips.Count - 1)];
        audioSource.pitch = Random.Range(1, 1.5f);
        audioSource.Play();
    }
}
