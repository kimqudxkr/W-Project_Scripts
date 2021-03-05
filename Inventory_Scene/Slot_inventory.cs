using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Slot_inventory : MonoBehaviour
{
    public int num;
    public bool isEquiped = false;
    string itemname;
    public GameObject seletedSlotEffect;

    public bool checkslotFool;
    public int maxCount;

    public Inventory inven;
    public gameInformationManager gameInformationManager;

    private void Start()
    {
        inven = GameObject.Find("inventorySystem").GetComponent<Inventory>();//현재 인벤토리 정보 가져옴
        gameInformationManager = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>();
    }

    private void Update()
    {
        checkslotFool = false;//슬롯이 가득차있는지 알려주는 변수
        for (int i = 0; i < 3; i++)
        {
            if (inven.slots[i].isEmpty == true)
            {
                checkslotFool = true;
                maxCount = i;
                break;
            }
        }
    }

    public void selectItem()
    {
        Debug.Log("setletItem 들어옴");
        //퀵슬롯이 꽉찼는지 확인, 현재 클릭된 장비가 장착 안됐는지
        if (checkslotFool == true && isEquiped == false)//3번째 인벤토리가 꽉찼는지, 현재 장비가 등록 되었는ㅅ
        {
            for (int i = 0; i < inven.slots.Count; i++)//슬롯 개수만큼 반복문
            {
                if (inven.slots[i].isEmpty)//비어있고 현재 장착중이지 않으며
                {
                    //Debug.Log(this.name.Substring(5));
                    isEquiped = true;//장착된 상태로 변경
                    inven.slots[i].isEmpty = false;//비어있는 상태 해제
                    inven.slots[i].equipItemName = this.name.Substring(5);//퀵슬롯 객체에 장비 이름 저장
                    itemname = this.name.Split(new string[] { "-" }, System.StringSplitOptions.None)[0].Substring(5);
                    Instantiate(Resources.Load("prefabs/" + "image_" + itemname),
                        inven.slots[i].slotObj.transform, false).name = "QuickSlot_" + this.name.Substring(5);//슬롯에 슬롯 이미지 생성
                    Instantiate(seletedSlotEffect, this.transform, false);

                    break;
                }
            }
        }

        else if (isEquiped == true)//이미 장착중인 장비라면
        {
            isEquiped = false;
            for (int i = 0; i < inven.slots.Count; i++)//슬롯 개수만큼 반복문
            {
                if (inven.slots[i].equipItemName == this.name.Substring(5))//슬롯에 이미 이름이 같은 퀵슬롯 객체가 있다ㅑ
                {
                    inven.slots[i].isEmpty = true;
                    Destroy(GameObject.Find("QuickSlot_" + this.name.Substring(5)));

                    Destroy(transform.Find("selectedSlot" + "(Clone)").gameObject);
                    
                    gameInformationManager.removeQuickSlot(itemname);//문자열 삭제

                    break;
                }
            }
        }
    }
}
