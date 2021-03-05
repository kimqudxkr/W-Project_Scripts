using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class PlayerCtrl_Data : MonoBehaviour
{


    public int ExtraBullet = 10; //탄창 수

    public Transform Head;      //플레이어의 머리위치

    public Transform FirePos;   //총알이 발사될 위치
    public GameObject Gun;

    public GameObject Bullet;   //총알 오브젝트 가져옴

    public int HP = 100;      //플레이어의 체력

    public int Score;   //플레이어의 점수

    public int itemCount;

    public GameObject portal;
  
    public bool FirePossible=false;

    public Text HPCountText;
    public Text BulletCountText;
    public GameObject BulletImage;
    public GameObject BulletCountTextObject;



    void Start()
    {
      
        Score = 0;
        

    }

    void Update()
    {
        
        HPCountText.text = HP.ToString();
        BulletCountText.text = ExtraBullet.ToString();

        //FirePos.rotation = Head.rotation;
        //총알이 발사될 위치가 플레이어 머리위치와 동일하게 회전하도록 설정

        if (Input.GetKeyDown(KeyCode.Mouse0)&& (ExtraBullet > 0)&& FirePossible==true) 
            //GetKeyDown : 한번 입력, GetKey : 누른만큼 계속 입력
        {
            ExtraBullet = ExtraBullet - 1;
            
            Fire();
            
            
        }

        else if(Input.GetKeyDown(KeyCode.R))
        {
            ExtraBullet = 10;
            
        }

        if(itemCount==3)
        {
            FirePossible = true;
            portal.SetActive(true);
            BulletImage.SetActive(true);
            BulletCountTextObject.SetActive(true);
            Gun.SetActive(true);
        }


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "sword")
        {
            HP = HP - 2;
            
        }
        else if(other.tag == "item")
        {
            itemCount++;

        }
    }

    void Fire()
    {
        
        //총알 오브젝트가 FirePos의 위치와 회전방향을 가진상태로 생성
        Destroy(Instantiate(Bullet, FirePos.position, FirePos.rotation), 3.0f);

   
        
    }

}


