using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RayScript : MonoBehaviour
{
    RaycastHit hit;     //레이캐스트에 맞은 물체
    float MaxDistance = 150f;        //레이캐스트 거리
    
    public GameObject Ray;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)|| OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            //게임 개발 화면 속에 레이캐스트를 표시
            Debug.DrawRay(transform.position, transform.forward * MaxDistance, Color.blue, 0.3f);

            //레이캐스트가 해당 오브젝트의 위치에서 바라보는 방향으로 쐈을때 맞은 물체를 hit에 저장
            if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance))
            { 
                //부딪힌 오브젝트의 태그가 ReturnButton 일시
                if (hit.collider.tag == "StartButton" || hit.collider.tag == "RetryButton1")
                {
                    SceneManager.LoadScene("SampleScene");      
                }
                if(hit.collider.tag == "RetryButton2")
                {
                    SceneManager.LoadScene("SampleScene");
                }
                if(hit.collider.tag == "MainButton")
                {
                    SceneManager.LoadScene("mainScene");
                }
                //부딪힌 오브젝트의 태그가 ExitButton 일시
                if (hit.collider.tag == "QuitButton")
                {
                    Application.Quit();     //앱 종료
                }
            }
        }
    }
}
