using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public int num;
    public bool isEquiped = false;
    Inventory inventory;

    private void Start()
    {
        inventory = GameObject.Find("Player").GetComponent<Inventory>();
        num = int.Parse(gameObject.name.Substring(gameObject.name.IndexOf("_") + 1));
        
    }
    /*private void Update()//자식 컴포넌트가 없다면 슬롯의 상태를 비었다고 표시
    {
        if (transform.childCount <= 0)
        {
            inventory.slots[num].isEmpty = true;
        }
    }*/
}
