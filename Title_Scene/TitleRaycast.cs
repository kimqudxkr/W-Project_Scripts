using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Security.Cryptography;
using Loom.Client;

public class TitleRaycast : MonoBehaviour
{
    RaycastHit hit;     //레이캐스트에 맞은 물체
    float MaxDistance = 150f;

    public GameObject Title_Nickname;
    string nickname = "unknown";

    gameInformationManager manager;

     void Start()
    {

        manager = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>();
        nickname = manager.nickName;

        Title_Nickname.GetComponent<Text>().text = nickname + "님 환영합니다";
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            Debug.DrawRay(transform.position, transform.forward * MaxDistance, Color.blue, 0.3f);

            if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance))
            {
                if (hit.collider.tag == "GameStart")
                {
                    SceneManager.LoadScene("ScenarioScene");
                }
                if (hit.collider.tag == "RankCheck")
                {
                    SceneManager.LoadScene("End_Scene");
                }
                if(hit.collider.tag == "Quit")
                {
                    //UnityEditor.EditorApplication.isPlaying = false;
                    Application.Quit();
                }
            }
        }
    }
}
