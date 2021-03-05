using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class gameInformationManager : MonoBehaviour
{

    public int player_HP = 100;//플레이어 체=

    public List<string> quickSlotItem = new List<string>();//퀵슬롯에 선택된 아이템 리스트 ====>이걸 다음씬에 넘겨주어 사용자의 퀵슬롯 데이터로 사용

    public List<string> newAddedItem = new List<string>();//새로 획득한 아이템을 담고 있는 리스트

    public int sceneNum=0;

    public string[] sceneName;

    public float time;//시간 카운트 할 함수
    public bool isTime = false;//씬 종료 시 false로 바꿔서 시간 재는거 멈춤
    //씬 시작 시 true로 바뀌면서 시간을 재기 시작함
    public bool isGameEnd = false;

    public int score;//점수를 저장할 변수
    public int total_score=0;//총 점수
    public string nickName;//유저 닉네임 저장
    public int rank;//순=
    public string passwd;
    public Wallet playerWallet;

    public string[] newGuns = new string[5];
    public int gunCount = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }



    private void Start()
    {
        //씬 시작과 동시에 시간 카운트
        time = 0;
        score = 0;
        isTime = false;
    }

    private void Update()
    { 
        if (isTime == true)//게임 진행중
        {
            time += Time.deltaTime;
            isGameEnd = true;


        }
        else if (isTime == false && isGameEnd==true)
        {
            score = (player_HP * 4) + (600 - ((int)time / 2));
            total_score += score;
            isGameEnd = false;
            player_HP = 100;
        }

        if (player_HP <= 0)
        {
            Debug.Log("you die");
            SceneManager.LoadScene("Death_Scene");
            isTime = false;//시간 멈춤
        }
        else if(player_HP > 100)
        {
            player_HP = 100;
        }
    }

    public void addQuickSlot(string Item)//리스트에 문자열 추가
    {
        quickSlotItem.Add(Item);//아이템 이름 리스트에 저장하는 부분
    }


    public void removeQuickSlot(string Item)//리스트데서 문쟈열 삭제
    {
        int index = 0;

        foreach (string item in quickSlotItem)
        {
            if (item == Item)
                break;
            index++;
        }
        quickSlotItem.RemoveAt(index);

    }

    //씬 정보
    //현재 진행정보를 배열에 저장해두고
    //버튼을 누르면 find로 이 오브젝트 찾아서 배열에 저장된 씬으로 넘어가는 내용 실행
    public void gotoNextScene()
    {
        sceneNum++;
        SceneManager.LoadScene(sceneName[sceneNum]);//버튼을 누르면 진행중이던 씬에서 다음씬으로 넘어간다
    }

    public void gotoNowScene()//죽었을때 다시 하려면 실
    {
        SceneManager.LoadScene(sceneName[sceneNum]);
    }


    public void addNewItem(string Item)//리스트에 문자열 추가(새로 획득한 아이템)
    {
        newAddedItem.Add(Item);//아이템 이름 리스트에 저장하는 부분
        JsonGunState.Gun gun;
        gun.name = Item;
    }
}
