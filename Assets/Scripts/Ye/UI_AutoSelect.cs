using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_AutoSelect : MonoBehaviour
{

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
    public void SelectMe() 
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}
