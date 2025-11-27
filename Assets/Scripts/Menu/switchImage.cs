using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class switchImage : MonoBehaviour
{
    public Camera FirstCamera;
    public Camera SecondCamera;

    void start()
    {
        SecondCamera.enabled = false;
        FirstCamera.enabled = true;
    }
    public void First()
    {
        FirstCamera.enabled = true;
        SecondCamera.enabled = false;
    }
    public void Second()
    {
        SecondCamera.enabled = true;
        FirstCamera.enabled = false;

    }
}
    
