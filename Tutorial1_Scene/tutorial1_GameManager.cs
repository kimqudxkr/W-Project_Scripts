using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class StoryDialogue1
{
    //대화창을 위한 다이얼로그 클래스
    [TextArea]
    public string text;
    public Sprite character;
}

public class tutorial1_GameManager : MonoBehaviour
{

    public int gun_item=0;

    public GameObject portal;

    [SerializeField] private SpriteRenderer dialogueChar;   //대화창 속 캐릭터
    [SerializeField] private GameObject dialogueBox;    //대화창 속 상자
    [SerializeField] private Text dialogueText;             //대화창 속 글
    [SerializeField] private StoryDialogue[] dialogue;
    //public GameObject gun;

    public bool isDialogue = false;    //대화창 판정
    public int count = 0;              //대화 진행도
    gameInformationManager infomanager;
    public GameObject Player;

    private void Start()
    {

        portal.SetActive(false);
        ShowDialogue();
        
        infomanager = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>();
    }



    public void ShowDialogue()
    {
        //처음 씬 시작시 대화상자를 보이도록 하는 함수
        dialogueBox.gameObject.SetActive(true);
        dialogueChar.gameObject.SetActive(true);
        dialogueText.gameObject.SetActive(true);
        isDialogue = true;
        NextDialogue();
        Time.timeScale = 0;
    }

    private void NextDialogue()
    {
        //다음 대화창으로 넘어가기 위한 함수
        dialogueText.text = dialogue[count].text;
        dialogueChar.sprite = dialogue[count].character;
        count++;
    }

    private void HideDialogue()
    {
        //대화창이 다 끝날시
        dialogueBox.gameObject.SetActive(false);
        dialogueChar.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        //gun.gameObject.SetActive(true);
        isDialogue = false;
        Time.timeScale = 1;
    }


    private void Update()
    {
        if(gun_item == 3)//총 재료를 다 모았으면
        {
            //인벤토리 창에 무기 활성화
            Inventory inven = Player.GetComponent<Inventory>();//현재 인벤토리 정보 가져옴
            inven.slots[0].isEmpty = false;//비어있는 상태 해제

            Instantiate(Resources.Load("prefabs/" + "image_normal_handgun"), inven.slots[0].slotObj.transform, false);//슬롯에 슬롯 이미지 생성
            ShowDialogue();
            portal.SetActive(true);
            
            gun_item = 0;
        }


        if (isDialogue)
        {
            if (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                if (count < 9)
                    NextDialogue();
                else if (count == 9 && isDialogue == true)
                {
                    HideDialogue();
                    isDialogue = false;
                }

                else if (count < 13)
                    NextDialogue();

                else if (count == 13 && isDialogue == true)
                {
                    HideDialogue();
                    isDialogue = false;
                }

                else if (count < 18)
                    NextDialogue();

                else if (count == 18 && isDialogue == true)
                {
                    HideDialogue();
                    isDialogue = false;
                }
            }
        }

    }




}
