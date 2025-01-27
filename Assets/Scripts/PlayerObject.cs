using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;


public class PlayerObject : MonoBehaviour
{
    public TMP_Text playerName; 
    public TMP_Text playerJerseyNumber;

    public GameObject infoCard;
    public TMP_Text infoName;
    public TMP_Text infoFoot;
    public RawImage jerseyEIMG;
    public bool isMoved = false;
    public bool isInPitch = false;

    Camera cam;
    Collider2D collider2D;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        collider2D = GetComponent<Collider2D>();
        //playerName.transform.position = cam.ScreenToWorldPoint(transform.position) + Vector3.down * 10;
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift))
        {
            infoCard.SetActive(!infoCard.activeInHierarchy);
            Debug.Log("Click has been succesfully detected");
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        playerName.transform.position = cam.WorldToScreenPoint(transform.position) - Vector3.up * 60;
        playerJerseyNumber.transform.position = cam.WorldToScreenPoint(transform.position);
        jerseyEIMG.transform.position = cam.WorldToScreenPoint(transform.position) + Vector3.up * 20;
        infoCard.transform.position = cam.WorldToScreenPoint(transform.position) + Vector3.up * 60;
        */
    }
}
