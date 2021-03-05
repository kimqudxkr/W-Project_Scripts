using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class death_button_manager : MonoBehaviour
{


    public void reGame()//다시하기
    {
        GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>().gotoNowScene();
    }

    public void forGive()//포기하기
    {
        SceneManager.LoadScene("Title_Scene");

    }

    public void endGame()//게임 종료
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//유니티 에디터일 경우 종료

        #else
            Application.Quit();//어플일 경우 종새
        #endif
    }


}
