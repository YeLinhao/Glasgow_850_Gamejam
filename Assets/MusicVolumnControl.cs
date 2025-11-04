using UnityEngine;

public class MusicVolumnControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
    }

}
