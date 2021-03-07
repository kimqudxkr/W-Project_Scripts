using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    public ItemBuffer itemBuffer;
    public Transform SlotRoot;
    private List<Slots> slot;

    public Text text;
    public int Money;

    public GameObject checkbox;

    public System.Action<ItemProperty> onSlotClick;     //아이템 클릭시 넘겨주는 역할

    void Start()
    {
        slot = new List<Slots>();
        int slotCount = SlotRoot.childCount;        //현재 오브젝트의 자식의 갯수 저장

        for(int i = 0; i < slotCount; i++)
        {
            var slots = SlotRoot.GetChild(i).GetComponent<Slots>();   //자식 오브젝트중 컴포넌트를 불러옴

            if (i < itemBuffer.items.Count)         //버퍼의 갯수에 맞는 만큼
            {
                slots.SetItem(itemBuffer.items[i]);     //슬롯에 아이템이 보이도록 설정함
            }
            else
            {
                slots.GetComponent<UnityEngine.UI.Button>().interactable = false;       //유니티 버튼 기능 중 interatable을 꺼서 이벤트 비활성화 되도록
            }

            slot.Add(slots);
        }

        text = GameObject.Find("Money").GetComponent<Text>();
    }
    
    void Update()
    {
       // text.GetComponent<Text>().text = "보유금액:" + Money;
    }

    public void OnClickSlot(Slots slots)
    {
        if (Money >= slots.price)
        {
            checkbox.SetActive(true);       //확인 상자를 띄움
            
        }
        else
        {
            Debug.Log("돈없다");           //돈 없다는 상자 띄움
        }
    }

    public void OnClickYes(Slots slots)
    {
        Money -= slots.price;           //돈 깎이고 물품이 사져야함
                                        //이제 문제는 슬롯데이터를 못가져오고 
    }

    public void OnClickCancel()
    {
        checkbox.SetActive(false);      //창 닫김
    }

    public void onClickInven()
    {
        SceneManager.LoadScene("itemSelectScene");
    }

    public void onClickShop()
    {
        SceneManager.LoadScene("ShopExample");
    }
}
