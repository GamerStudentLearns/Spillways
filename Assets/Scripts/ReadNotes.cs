using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadNotes : MonoBehaviour
{
    public GameObject player;
    public GameObject noteUI;
    public GameObject hud;
    public GameObject inv;

    public GameObject pickUpText;

    public AudioSource pickUpSound;

    public bool inReach;


    // Start is called before the first frame update
    void Start()
    {
        noteUI.SetActive(false);
        pickUpText.SetActive(false);

        inReach = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            pickUpText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            pickUpText.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
      if(Input.GetKeyDown("E") && inReach)
        {
            noteUI.SetActive(true);
            pickUpSound.Play(); 
            player.GetComponent<PlayerMovement>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }  
    }

    public void ExitButton()
    {
        noteUI.SetActive(false);
        player.GetComponent<PlayerMovement>().enabled = true;
     
    }
}
