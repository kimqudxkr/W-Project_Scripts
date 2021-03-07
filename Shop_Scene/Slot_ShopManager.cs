using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot_ShopManager : MonoBehaviour
{
    public string[] price;
    public GameObject check;
    public CreateAssetByNameShop shopScript;
    public GameObject notEnoughMoney;

    public void Start()
    {
        shopScript = GameObject.Find("GameObject").GetComponent<CreateAssetByNameShop>();
    }

    public void Update()
    {

    }

    public void OnClickSlot()
    {
        price = this.name.Split(new string[] { "/" }, System.StringSplitOptions.None);
        Debug.Log(price[0]);
        GameObject.Find("GameObject").GetComponent<CreateAssetByNameShop>().gunname = price[0];
        GameObject.Find("GameObject").GetComponent<CreateAssetByNameShop>().clicked = true;
        

        if (shopScript.money >= int.Parse(price[1]))
        {
           Instantiate(check, GameObject.Find("CheckLocal").transform, false).name=price[0]+"/check";       //확인 상자를 띄움

        }
         else
       {
            Instantiate(notEnoughMoney, GameObject.Find("CheckLocal").transform, false);         //돈 없다는 상자 띄움
        }
    }
}
