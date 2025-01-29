using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceLineManager : MonoBehaviour
{
    AudioSource audioSource;
    public List<AudioClip> goodclipList;
    public List<AudioClip> badClipList;
    public float cooldown = 7;
    public float timeSinceLastLine = 7;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastLine += Time.deltaTime;
    }

    public void playGoodClip()
    {
        if (timeSinceLastLine > cooldown)
        {
            AudioClip current = goodclipList[Random.Range(0, goodclipList.Count)];
            audioSource.clip = current;
            //audio.pitch = Random.Range(1, 1.5f);
            audioSource.Play();
            timeSinceLastLine = 0;
        }
       
    }

    public void playBadClip()
    {
        if (timeSinceLastLine > cooldown/2)
        {
            AudioClip current = badClipList[Random.Range(0, badClipList.Count - 1)];
            audioSource.clip = current;
            //audio.pitch = Random.Range(1, 1.5f);
            audioSource.Play();
            timeSinceLastLine = 0;
        }
    }
}
