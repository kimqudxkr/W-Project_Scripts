using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class button_scene_manager : MonoBehaviour
{
    // Start is called before the first frame update

    public void gotoTitleButton()
    {
        //타이틀 씬으로 이도
        SceneManager.LoadScene("Title_Scene");
    }

    public void endGameButton()
    {
        //씬 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//유니티 에디터일 경우 종료

#else
            Application.Quit();//어플일 경우 종새
#endif
    }
}
