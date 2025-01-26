using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSoundsManager : MonoBehaviour
{
    AudioSource audio;
    public List<AudioClip> clips = new List<AudioClip>();
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = clips[Random.Range(0, clips.Count - 1)];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playSplash()
    {
        audio.clip = clips[Random.Range(0, clips.Count - 1)];
        audio.pitch = Random.Range(1, 1.5f);
        audio.Play();
    }
}
