using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class pickUp : MonoBehaviour
{
    DataController dataController;

    string[] pickupitem;
    public GameObject slotItem;
    public gameInformationManager manager;

    private void OnTriggerEnter(Collider other)//부딫히면
    {

        //유저 소유 아이템 블록체인에 업로드 코드 필요
        if (other.tag.Equals("Player"))//플레이어 태그라면
        {
            Inventory inven = other.GetComponent<Inventory>();//현재 인벤토리 정보 가져옴

            for (int i = 0; i < inven.slots.Count; i++)//슬롯 개수만큼 반복문
            {
                if (inven.slots[i].isEmpty)//비어있다면
                {
                    inven.slots[i].isEmpty = false;//비어있는 상태 해제
                    
                    Instantiate(slotItem, inven.slots[i].slotObj.transform, false);//슬롯에 슬롯 이미지 생성

                    break;
                }
            }

            Destroy(this.gameObject);//획득한 아이템 맵에서 제거

            gameInformationManager gameInformationManager = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>();
            gameInformationManager.newGuns[gameInformationManager.gunCount] = this.name.Substring(6);
            gameInformationManager.gunCount++;
        }
    }
}
