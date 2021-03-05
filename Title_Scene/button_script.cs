using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class button_script : MonoBehaviour
{

    public GameObject Title_Nickname;
    string nickname = "unknown";

    // Start is called before the first frame update
    void Start()
    {
        nickname = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>().nickName;
        Title_Nickname.GetComponent<Text>().text = nickname + "님 환영합니다";
    }

    // Update is called once per frame
    public void gameStartButton()
    {
        //게임 매니져 로직의 씬 함수 불러옴
        
        SceneManager.LoadScene("ScenarioScene");
    }

    public void checkRank()
    {
        //랭크 확인 함수
        
        SceneManager.LoadScene("End_Scene");
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//유니티 에디터일 경우 종료

#else
            Application.Quit();//어플일 경우 종새
#endif
    }
}
