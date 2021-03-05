using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getQuickSlot : MonoBehaviour
{
    gameInformationManager gameInformationManager;

    // Start is called before the first frame update
    public void Awake()
    {

        gameInformationManager = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>();

    }


    public void quickSlotUpdate()
    {
        
        Inventory inven = GameObject.Find("Player").GetComponent<Inventory>();//현재 인벤토리 정보 가져옴//최적화



        foreach (string item in gameInformationManager.quickSlotItem)
        {

            Debug.Log(inven.slots.Count);
            for (int i = 0; i < inven.slots.Count; i++)//슬롯 개수만큼 반복문
            {

                if (inven.slots[i].isEmpty)//비어있다면
                {

                    inven.slots[i].isEmpty = false;//비어있는 상태 해제

                    Instantiate(Resources.Load("prefabs/" + "image_" + item), inven.slots[i].slotObj.transform, false);//슬롯에 슬롯 이미지 생성


                    break;


                }
            }
        }
    }
}
