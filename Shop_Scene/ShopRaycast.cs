using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopRaycast : MonoBehaviour
{
    RaycastHit hit;     //레이캐스트에 맞은 물체
    float MaxDistance = 150f;        //레이캐스트 거리

    public bool isBuy = true;
    public GameObject buyCanvas;
    public GameObject SellCanvas;
    public GameObject storeItems;
    public GameObject inventorySystem;

    public string[] price;
    public GameObject check;
    public CreateAssetByNameShop shopScript;
    public GameObject notEnoughMoney;

    public GameObject confirm;

    public GameObject checkbox;
    public GameObject sellCancel;//하나 남겨야 할때 경고/

    public string sellname;
    public string[] tmpname;
    public string tmp1;
    public string realname;
    public string objname;
    
    public CreateAssetInShop inShop;

    public void Start()
    {
        shopScript = GameObject.Find("GameObject").GetComponent<CreateAssetByNameShop>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            Debug.DrawRay(transform.position, transform.forward * MaxDistance, Color.blue, 0.3f);

            if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance))
            {
                if (hit.collider.tag == "ShopItem")
                {
                    price = hit.transform.name.Split(new string[] { "/" }, System.StringSplitOptions.None);
                    if (shopScript.money >= int.Parse(price[1]))
                    {
                        Instantiate(check, GameObject.Find("CheckLocal").transform, false).name = price[0] + "/check";
                    }
                    else
                    {
                        Instantiate(notEnoughMoney, GameObject.Find("CheckLocal").transform, false);         //돈 없다는 상자 띄움
                    }
                }

                if(hit.collider.tag == "Change")
                {
                    if (isBuy == true)
                    {
                        isBuy = false;
                        buyCanvas.SetActive(false);
                        SellCanvas.SetActive(true);
                        storeItems.SetActive(false);
                        inventorySystem.SetActive(true);
                        GameObject.Find("inventorySystem").GetComponent<CreateAssetInShop>().LoadInventoryIteams();
                    }
                    else
                    {
                        Transform[] childList = GameObject.Find("inventorypanel").GetComponentsInChildren<Transform>(true);
                        if(childList != null)
                            for(int i = 1; i < childList.Length; i++)
                                if (childList[i] != transform)
                                    Destroy(childList[i].gameObject);
                        
                        isBuy = true;
                        buyCanvas.SetActive(true);
                        SellCanvas.SetActive(false);
                        storeItems.SetActive(true);
                        inventorySystem.SetActive(false);
;                    }
                }

                if (hit.collider.tag == "OKButton")
                {
                    Destroy(hit.transform.parent.gameObject);
                }

                if(hit.collider.tag == "confirmYes")
                {
                    price = hit.transform.parent.name.Split(new string[] { "/" }, System.StringSplitOptions.None);
                    shopScript.gunname = price[0];
                    Debug.Log("처음 클릭 무기 이름:" + price[0]);
                    shopScript.clicked = true;
                    Destroy(hit.transform.transform.parent.gameObject);
                    Destroy(GameObject.Find("Loading(Clone)").gameObject);
                }

                if (hit.collider.tag == "goToInventory")
                {
                    SceneManager.LoadScene("itemSelectScene");
                }

                if(hit.collider.tag == "SellingSlot")
                {
                    inShop = GameObject.Find("inventorySystem").GetComponent<CreateAssetInShop>();
                    
                    if (inShop.sellCount < 2)
                    {
                        Instantiate(sellCancel, GameObject.Find("CheckLocal").transform, false);
                    }
                    else
                    {
                        Instantiate(checkbox, GameObject.Find("CheckLocal").transform, false);
                        objname = hit.transform.name;
                        tmp1 = hit.transform.name.Substring(5);
                        tmpname = tmp1.Split(new string[] { "-" }, System.StringSplitOptions.None);
                        sellname = tmpname[0];

                        shopScript.sellgunname = sellname;
                        shopScript.destroyName = objname;
                    }

                }

                if (hit.collider.tag == "SellYes")
                {
                    shopScript.sellWeapon();
                    Destroy(hit.transform.parent.gameObject);
                }
            }
        }
    }
}
