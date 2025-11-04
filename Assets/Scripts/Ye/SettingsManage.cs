using UnityEngine;

public class SettingsManage : MonoBehaviour
{
    public void ChangeMainVolumn(float input) 
    {

        PlayerPrefs.SetFloat("MainVolume", input);
    }

    public void ChangeMusicVolumn(float input)
    {

        PlayerPrefs.SetFloat("MusicVolume", input);
    }


}
