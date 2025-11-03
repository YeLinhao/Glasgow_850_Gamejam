using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float moveSpeed = 1f;
    public float fadeSpeed = 1f;
    private Color textColor;

    public void Setup(int score)
    {
        textMesh.text = "+" + score.ToString();
        textColor = textMesh.color;
    }

    void Update()
    {
        // Move upward
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Fade out
        textColor.a -= fadeSpeed * Time.deltaTime;
        textMesh.color = textColor;

        // Destroy when fully transparent
        if (textColor.a <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void LateUpdate()
    {
        // Always face the camera
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);
        }
    }
}
