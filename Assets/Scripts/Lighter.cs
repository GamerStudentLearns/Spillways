using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighter : MonoBehaviour
{

    public GameObject lighter;
    public GameObject Flames;

    public AudioSource lighterSound;

    public bool isOn;


    // Start is called before the first frame update
    void Start()
    {
        isOn = false;
        Flames.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("Fire1") && lighter.activeInHierarchy)
        {
            Flames.SetActive(true);
            lighterSound.Play();
            isOn = true;
        }

        else if (Input.GetKeyDown("Fire1") && isOn) 
        {
            return;
        }
        if (Input.GetKeyDown("Fire2") && lighter.activeInHierarchy && isOn)
        {
            Flames.SetActive(false);
            isOn = false;
        }
    }
}
