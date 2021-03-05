using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_EquipItem_PC : MonoBehaviour
{
    string[] ObjName;
    public GameObject Item_Obj;
    public GameObject checkUsingItem;



    private void Update()
    {


        if (transform.parent.GetComponent<Slot>().isEquiped == false)//현재 지정한 오브젝트의 부모인 슬롯의 장비 상태가 장착이 false라면
        {
            //아이템 번호와 누른 번호가 같으면(현재 이미지 오브젝트가 부착되어있는 슬롯의 번호)
           if (Input.inputString == (transform.parent.GetComponent<Slot>().num + 1).ToString())
            {
                Destroy(GameObject.FindGameObjectWithTag("equip"));//이미 장비중인 아이템을 파괴
                Destroy(GameObject.Find("selectedSlot(Clone)"));

                //아이템 사용
                //Instantiate로 손에 무기 생성
                Debug.Log((transform.parent.GetComponent<Slot>().num + 1).ToString() + "번 무기 변경");

                //transform.parent.GetComponent<Slot>().isEquiped = true;
                //if(isGearVrController == true)
                //{
                //    GameObject.Find("GearVrController").SetActive(false);
                //    isGearVrController = false;
                //}

                Instantiate(Item_Obj, GameObject.Find("Player_arm").transform, false);
                //GameObject.Find("Player").GetComponent<PlayerCtrl>().gunAnim = Item_Obj.GetComponent<Animator>();
                //플레이어의 손에 장착된 애니메이터를 넘겨줌

                Instantiate(checkUsingItem, this.transform.parent, false);
                Debug.Log("장비 상태" + transform.parent.GetComponent<Slot>().isEquiped);
                //GameObject.Find("GearVrController").SetActive(false);
            }

        }


        //ObjName = this.gameObject.name.Substring(6).Split(new string[] { "(" },System.StringSplitOptions.None);
        //image_ 다음부분부터 그걸 ( 기준으로 반반 나눠서 첫번째


    }

}
