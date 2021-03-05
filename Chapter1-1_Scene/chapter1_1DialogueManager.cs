using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]

public class StoryDialogue1_1
{
    //대화창을 위한 다이얼로그 클래스
    [TextArea]
    public string text;
    public Sprite character;
}


public class chapter1_1DialogueManager : MonoBehaviour
{

    [SerializeField] private SpriteRenderer dialogueChar;   //대화창 속 캐릭터
    [SerializeField] private GameObject dialogueBox;    //대화창 속 상자
    [SerializeField] private Text dialogueText;             //대화창 속 글
    [SerializeField] private StoryDialogue[] dialogue;


    public bool isDialogue = false;    //대화창 판정
    public int count = 0;              //대화 진행도

    public GameObject firstSpot;

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




    private void Start()
    {
        ShowDialogue();
    }

    private void Update()
    {
        if (isDialogue)
        {
            if (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                if (count < 4)      //3개까지 보여주고 게임 시작
                    NextDialogue();
                else if (count == 4 && isDialogue == true)
                {
                    HideDialogue();
                    isDialogue = false;
                }

                else if (count < 9)
                    NextDialogue();

                else if (count == 9 && isDialogue == true)//클리어
                {
                    firstSpot.SetActive(false);
                    HideDialogue();
                    isDialogue = false;
                }

                else if (count < 14)
                    NextDialogue();

                else if (count == 14 && isDialogue == true)//클리어
                {
                    HideDialogue();
                    isDialogue = false;
                    SceneManager.LoadScene("ShopExample");
                }
            }
        }
    }
}


