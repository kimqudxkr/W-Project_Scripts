using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class SellCheck : MonoBehaviour
{
    public GameObject checkbox;
    public GameObject sellCancel;//하나 남겨야 할때 경고/


    public string sellname;
    public string[] tmpname;
    public string tmp1;
    public string realname;
    public string objname;

    public bool isClick=false;

    public CreateAssetByNameShop sellscript;

    public void Update()
    {
        if(isClick==true)
        {
            isClick = false;
            realname = sellname;
        }
    }

    public void onClickSellSlot()//판매입력을 처리할 스크립트 버튼 부
    {
        if(GameObject.Find("inventorySystem").GetComponent<CreateAssetInShop>().sellCount < 2)
        {
            Instantiate(sellCancel, GameObject.Find("CheckLocal").transform, false);
        }
        else
        {
            Instantiate(checkbox, GameObject.Find("CheckLocal").transform, false);
            objname = this.name;
            tmp1 = this.name.Substring(5);
            this.tmpname = tmp1.Split(new string[] { "-" }, System.StringSplitOptions.None);
            Debug.Log(tmpname[0]);
            this.sellname = this.tmpname[0];
            sellscript = GameObject.Find("GameObject").GetComponent<CreateAssetByNameShop>();
            sellscript.sellgunname = sellname;
            sellscript.destroyName = objname;
        }
        

    }

    async public void onClickYes()
    {
        GameObject.Find("inventorySystem").GetComponent<CreateAssetInShop>().sellCount--;
        sellscript = GameObject.Find("GameObject").GetComponent<CreateAssetByNameShop>();
        await sellscript.sellWeapon();        
        Destroy(this.transform.parent.gameObject);
        
    }

    public void onClickCancel()
    {
        Destroy(this.transform.parent.gameObject);
    }
    public void onClickOk()
    {
        //카운트 부분 추가

        Destroy(this.transform.parent.gameObject);//판매 시
    }


}
