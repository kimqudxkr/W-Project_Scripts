using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeCanvas : MonoBehaviour
{

    public GameObject buyCanvas;
    public GameObject SellCanvas;
    public bool isbuy = true;
    
    public GameObject storeItems;
    public GameObject inventorySystem;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changecanvas()//패널 전환
    {
        if(isbuy == true)
        {
            buyCanvas.SetActive(false);
            SellCanvas.SetActive(true);
            storeItems.SetActive(false);
            inventorySystem.SetActive(true);
            isbuy = false;
        }
        else
        {
            buyCanvas.SetActive(true);
            SellCanvas.SetActive(false);
            storeItems.SetActive(true);
            inventorySystem.SetActive(false);
            isbuy = true;
        }
    }
}
