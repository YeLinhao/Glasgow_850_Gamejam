using UnityEngine;
using UnityEngine.UI;


public class SettingsManage : MonoBehaviour
{
    public Slider mainV;
    public Slider musicV;


    private void Start()
    {
        mainV.onValueChanged.AddListener(ChangeMainVolumn);
        musicV.onValueChanged.AddListener(ChangeMusicVolumn);
    }


    public void ChangeMainVolumn(float input) 
    {

        PlayerPrefs.SetFloat("MainVolume", input);
    }

    public void ChangeMusicVolumn(float input)
    {

        PlayerPrefs.SetFloat("MusicVolume", input);
    }


}
