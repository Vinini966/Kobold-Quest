using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeScript : MonoBehaviour
{

    public AudioMixer audioMixer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateMasterAudio(float value)
    {
        float temp = Mathf.Lerp(-80, 5, Mathf.InverseLerp(0, 1, value));
        audioMixer.SetFloat("Master", temp);
    }

    public void updateMusicAudio(float value)
    {
        float temp = Mathf.Lerp(-80, 0, Mathf.InverseLerp(0, 1, value));
        audioMixer.SetFloat("Music", temp);
    }
    public void updateSFXAudio(float value)
    {
        float temp = Mathf.Lerp(-80, 0, Mathf.InverseLerp(0, 1, value));
        audioMixer.SetFloat("SFX", temp);
    }
    public void updateVoiceAudio(float value)
    {
        float temp = Mathf.Lerp(-80, 0, Mathf.InverseLerp(0, 1, value));
        audioMixer.SetFloat("Voice", temp);
    }

}
