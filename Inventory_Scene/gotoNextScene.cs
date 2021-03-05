using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gotoNextScene : MonoBehaviour
{
    public bool isQuickSlot = false;
    public GameObject cancelNext;

    public void nextScene()
    {
        Inventory inven = GameObject.Find("inventorySystem").GetComponent<Inventory>();//현재 인벤토리 정보 가져옴
        gameInformationManager gameInformationManager = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>();


        for (int i = 0; i < 3; i++)
        {
            if (inven.slots[i].isEmpty == false)//퀵슬롯이 하나라도 차있으면
            {
                isQuickSlot = true;//퀵슬롯에 아이템이 등록되어있다고 표ㅅ
                break;
            }

        }

        if (isQuickSlot == true)
        {
            // SceneManager.LoadScene("inventory");
            gameInformationManager.gotoNextScene();
        }

        else
        {
            Instantiate(cancelNext, GameObject.Find("CheckLocal").transform, false);//안내창 생cancelNext
        }
    }


    public void okCancelNext()
    {
        Destroy(this.transform.parent.gameObject);
    }

}
