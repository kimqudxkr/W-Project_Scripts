using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour//슬롯창 생성하는 부분
{

    public List<SlotData> slots = new List<SlotData>();
    private int maxSlot = 3;
    public GameObject slotPrefab;


    private void Awake()
    {
        GameObject slotPanel = GameObject.Find("useitempanel");

        for (int i = 0; i < maxSlot; i++)
        {
            GameObject go = Instantiate(slotPrefab, slotPanel.transform, false);
            //slotPrefab을 slotPanel위치에 생성

            go.name = "Slot_" + i;//생성된 슬롯의 이름
            SlotData slot = new SlotData();//슬롯 객체 생성
            //slot.ObjName = "";
            slot.isEmpty = true;//슬롯 객체 상태
            slot.slotObj = go;//슬롯 객체에 오브젝트 할당
            slots.Add(slot);//슬롯 추c
        }
    }


    private void Start()
    {
        GameObject.Find("EventSystem").GetComponent<getQuickSlot>().quickSlotUpdate();
    }
}
