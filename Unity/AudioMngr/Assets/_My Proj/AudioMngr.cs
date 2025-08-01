using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMngr : MonoBehaviour
{
    private int currentBGM = -1;

    public AudioSource sourceBGM;
    public AudioSource sourceSound;

    public List<AudioClip> bgms;
    public List<AudioClip> effects;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayBGM(int idx)
    {
        if (currentBGM != idx && idx >= 0 && bgms.Count > idx)
        {
            sourceBGM.Stop();
            currentBGM = idx;
            sourceBGM.clip = bgms[idx];
            sourceBGM.Play();
        }
    }

    public void PlaySound(int idx)
    {
        if (idx >= 0 && effects.Count > idx)
        {
            //sourceSound.clip = effects[idx];
            sourceSound.PlayOneShot(effects[idx]);
        }
    }

    public int NumSounds()
    {
        return effects.Count;
    }

    public int NumBGMs()
    {
        return bgms.Count;
    }

    public int GetCurrentBGM()
    {
        return currentBGM;
    }

    public void PlayNextBGM()
    {
        currentBGM++;
        if (bgms.Count <= currentBGM)
        {
            currentBGM = 0;
        }
        if (bgms.Count > currentBGM)
        {
            Debug.Log("Playing BGM #" + currentBGM);
            sourceBGM.clip = bgms[currentBGM];
            sourceBGM.loop = true;
            sourceBGM.Play();
        }

    }
}
