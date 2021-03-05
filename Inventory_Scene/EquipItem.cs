using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItem : MonoBehaviour
{
    string[] ObjName;
    public GameObject Item_Obj;
    public GameObject checkUsingItem;

    public int weaponIndex = 0;//아무것도 장착 안했을 때
    public int maxcount = 0;
    int tempcount;
    public bool isGearVrController = true;

    private Inventory inven;
    public GameObject Player_arm;
    public GameObject useitempanel;

    private void Start()
    {
        inven = GameObject.Find("Player").GetComponent<Inventory>();
        Player_arm = GameObject.Find("Player_arm");
        useitempanel = GameObject.Find("useitempanel");

        for (int j = 0; j < 3; j++)
        {
            if (useitempanel.transform.GetChild(j).childCount == 2)
            {
                weaponIndex = j+1;
            }
        }
    }

    private void Update()
    {

        tempcount = 0;//초기화
        for (int i = 0; i < 3; i++)
        {
            if (inven.slots[i].isEmpty == false)
                tempcount++;//장착 아이템 개수 카운트(최대 3개) 무기는 1 2 3 까지 이므로 인덱스가 4가되면 1로 되돌림
        }
        maxcount = tempcount;

        if ((OVRInput.Get(OVRInput.Button.PrimaryTouchpad)))//버튼을 누르면
        {
            Vector2 coord = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
            var absX = Mathf.Abs(coord.x);
            var absY = Mathf.Abs(coord.y);

            if (absX > absY)
            {
                if (coord.x > 0)
                {
/*                    maxcount = 0;//초기화
                    for (int i = 0; i < 3; i++)
                    {
                        if (inven.slots[i].isEmpty == false)
                            maxcount++;//장착 아이템 개수 카운트(최대 3개) 무기는 1 2 3 까지 이므로 인덱스가 4가되면 1로 되돌림
                    }*/

                    //weaponIndex++;
                    if (weaponIndex + 1 <= maxcount)
                    {
                        weaponIndex++;//인덱스 증가
                    }
                    else
                    {
                        weaponIndex = 1;//첫 무기 슬롯
                    }

                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
/*            maxcount = 0;//초기화
            for (int i = 0; i < 3; i++)
            {
                if (inven.slots[i].isEmpty == false)
                    maxcount++;//장착 아이템 개수 카운트(최대 3개) 무기는 1 2 3 까지 이므로 인덱스가 4가되면 1로 되돌림
            }*/

            if(weaponIndex+1 <= maxcount)
            {
                weaponIndex++;//인덱스 증가
            }
            else
            {
                weaponIndex = 1;//첫 무기 슬롯
            }
        }

/*        if (weaponIndex > maxcount)
            weaponIndex = 1;//첫 무기 슬롯*/

        //아이템 번호와 누른 번호가 같으면(현재 이미지 오브젝트가 부착되어있는 슬롯의 번호)
        if (weaponIndex == (transform.parent.GetComponent<Slot>().num + 1))
        {
            if (transform.parent.GetComponent<Slot>().isEquiped == false)//현재 지정한 오브젝트의 부모인 슬롯의 장비 상태가 장착이 false라면
            {
                //Destroy(GameObject.Find("equip"));//이미 장비중인 아이템을 파괴
                foreach (Transform child in Player_arm.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                //Destroy(GameObject.Find("selectedSlot(Clone)"));//@@@@@@수정필요

                //아이템 사용
                //Instantiate로 손에 무기 생성
                Debug.Log((transform.parent.GetComponent<Slot>().num + 1).ToString() + "번 무기 변경");

                transform.parent.GetComponent<Slot>().isEquiped = true;


                Instantiate(Item_Obj, GameObject.Find("Player_arm").transform, false);
                //GameObject.Find("Player").GetComponent<PlayerCtrl>().gunAnim = Item_Obj.GetComponent<Animator>();
                //플레이어의 손에 장착된 애니메이터를 넘겨줌

                Instantiate(checkUsingItem, this.transform.parent, false);
                Debug.Log("장비 상태" + transform.parent.GetComponent<Slot>().isEquiped);
                
                GameObject.Find("GearVrController").SetActive(false);
            }
        }
        else
        {
            transform.parent.GetComponent<Slot>().isEquiped = false;
            Destroy(transform.parent.transform.GetChild(1).gameObject);
        }
        //ObjName = this.gameObject.name.Substring(6).Split(new string[] { "(" },System.StringSplitOptions.None);
        //image_ 다음부분부터 그걸 ( 기준으로 반반 나눠서 첫번째
    }

}
