using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private SpriteRenderer dialogueChar;   //대화창 속 캐릭터
    [SerializeField] private SpriteRenderer dialogueBox;    //대화창 속 상자
    [SerializeField] private Text dialogueText;             //대화창 속 글

    private bool isDialogue = false;    //대화창 설정
    private int count = 0;              //대화 진행도
    
    void Update()
    {
        
    }
}
