using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    public GameObject canvasA;
    public GameObject canvasB;

    public void ShowCanvasA()
    {
        canvasA.SetActive(true);
        canvasB.SetActive(false);
    }

    public void ShowCanvasB()
    {
        canvasA.SetActive(false);
        canvasB.SetActive(true);
    }
}

