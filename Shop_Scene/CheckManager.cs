using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Loom.Client;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;


public class CheckManager : MonoBehaviour
{
    public GameObject confirm;
    public string[] price;

    public void OnClickYes()
    {
        price = this.name.Split(new string[] { "/" }, System.StringSplitOptions.None);
        //GameObject.Find("GameObject").GetComponent<CreateAssetByNameShop>().gunname = price[0];
        //GameObject.Find("GameObject").GetComponent<CreateAssetByNameShop>().clicked = true;
        Destroy(this.transform.parent.gameObject);
        Instantiate(confirm, GameObject.Find("CheckLocal").transform, false);
    }

    public void OnClickCancel()
    {
        Destroy(this.transform.parent.gameObject);
    }
}
