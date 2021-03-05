using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]

public class StoryDialogue
{
    //대화창을 위한 다이얼로그 클래스
    [TextArea]
    public string text;
    public Sprite character;
}



public class tutorial2_GameManagerLogic : MonoBehaviour
{

    public int TotalItemCount;


    public GameObject boss;
    public bool bosscreate = false;
    public Transform gunSpawn;//normal_riple
    public GameObject monsterSpawn;
    public bool isOver = false;
    public int kill = 0;
    public GameObject field_normal_riple;
    public GameObject Player;

    public int min_dam;
    public int max_dam;

    Inventory inven;

    [SerializeField] private SpriteRenderer dialogueChar;   //대화창 속 캐릭터
    [SerializeField] private GameObject dialogueBox;    //대화창 속 상자
    [SerializeField] private Text dialogueText;             //대화창 속 글
    [SerializeField] private StoryDialogue[] dialogue;
    //public GameObject gun;

    gameInformationManager gameInformationManager;

    public bool isDialogue = false;    //대화창 판정
    public int count = 0;              //대화 진행도

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

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            ShowDialogue();
            monsterSpawn.SetActive(true);
            this.GetComponent<BoxCollider>().enabled = false;
        }
    }



        public void Start()
    {
        field_normal_riple.SetActive(false);


        monsterSpawn.SetActive(false);

        inven = Player.GetComponent<Inventory>();


 
        inven.slots[0].isEmpty = false;//비어있는 상태 해제

         Instantiate(Resources.Load("prefabs/" + "image_normal_handgun"), inven.slots[0].slotObj.transform, false);//슬롯에 슬롯 이미지 생성
        
        ShowDialogue();



        gameInformationManager = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>();

    }



    private void Update()
    {
        if (isDialogue)
        {
            if (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                if (count < 3)
                    NextDialogue();
                else if(count == 3 && isDialogue == true)
                {
                    HideDialogue();
                    isDialogue = false;
                }

                else if( count <7)
                    NextDialogue();

                else if (count == 7 && isDialogue == true)
                {
                   
                    HideDialogue();
                    
                    isDialogue = false;
                }
                else if (count < 10)
                    NextDialogue();

                else if (count == 10 && isDialogue == true)
                {

                    HideDialogue();
                    
                    isDialogue = false;
                }

                else if (count < 13)
                    NextDialogue();

                else if (count == 13 && isDialogue == true)
                {

                    HideDialogue();
                    this.GetComponent<tutorial2_soundManager>().PlayNextBGM();
                    bosscreate = true;
                    isDialogue = false;
                }

                else if (count < 17)
                    NextDialogue();

                else if (count == 17 && isDialogue == true)
                {

                    HideDialogue();

                    isDialogue = false;
                    gameInformationManager.isTime = false;
                    SceneManager.LoadScene("ShopExample");
                }
            }
        }

        if(bosscreate == true)
        {
            //보스 맵에 생성
            boss.SetActive(true);
        }

        if (kill == 10)
        {
            kill = 0;
            ShowDialogue();
            field_normal_riple.SetActive(true);



}


    }


    public void bossDie()
    {
        //보스가 죽었을 때 실행
        //대사 몇번 치고 다음 씬으로 이동
        ShowDialogue();

    }
    
}
