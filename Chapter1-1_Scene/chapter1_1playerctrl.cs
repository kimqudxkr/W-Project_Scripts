using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class chapter1_1playerctrl : MonoBehaviour
{


    public int ExtraBullet = 10; //탄창 수

    public Transform FirePos;   //총알이 발사될 위치
    public GameObject Gun;

    public GameObject Bullet;   //총알 오브젝트 가져옴

    public int HP = 100;      //플레이어의 체력

    public int itemCount;

    public GameObject portal;

    public bool FirePossible = false;

    public Text HPCountText;
    public Text BulletCountText;
    public GameObject BulletImage;
    public GameObject BulletCountTextObject;

    public float dirX = 0;
    public float dirZ = 0;
    public int speedForward = 12;
    public int speedSide = 6;

    public Transform Head;
    public Transform Body;

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void Update()
    {

        if ((Input.GetKeyDown(KeyCode.Mouse0) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) && (ExtraBullet > 0) && FirePossible == true)
        //GetKeyDown : 한번 입력, GetKey : 누른만큼 계속 입력
        {
            if (ExtraBullet > 0)
            {
                ExtraBullet = ExtraBullet - 1;
                Fire();
            }
            if (ExtraBullet == 0)
            {
                Invoke("ReLoad", 2);
            }

        }

        if (HP <= 0)
        {
            Debug.Log("you die");
            SceneManager.LoadScene("Death2Scene");
        }

        if (itemCount == 3)
        {
            FirePossible = true;
            //portal.SetActive(true);포탈 생성
            //BulletImage.SetActive(true);//탄창 수 이미/
            //BulletCountTextObject.SetActive(true);//탄창 수
            Gun.SetActive(true);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            HP = HP - 5;

        }
        else if (other.tag == "item")
        {
            itemCount++;
            other.gameObject.SetActive(false);
        }


        if (other.tag == "MachineGun")
        {
            SceneManager.LoadScene("EndScene");
        }
    }



    void Fire()
    {

        //총알 오브젝트가 FirePos의 위치와 회전방향을 가진상태로 생성
        Destroy(Instantiate(Bullet, FirePos.position, FirePos.rotation), 3.0f);



    }

    void ReLoad()
    {
        ExtraBullet = 10;
    }

    void MovePlayer()
    {
        dirX = 0;
        dirZ = 0;

        if (OVRInput.Get(OVRInput.Touch.PrimaryTouchpad))      //터치패드를 터치했을때
        {
            Vector2 coord = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
            //터치패드의 좌표를 받아옴

            var absX = Mathf.Abs(coord.x);
            var absY = Mathf.Abs(coord.y);
            //좌표값을 절대값으로 변환

            if (absX > absY)
            {
                if (coord.x > 0)
                {
                    dirX = -1;
                }
                else
                {
                    dirX = +1;
                }
            }
            else
            {
                if (coord.y > 0)
                    dirZ = -1;
                else
                    dirZ = +1;
            }
        }

        if (Input.GetKey(KeyCode.RightArrow))
            dirX = -1;
        else if (Input.GetKey(KeyCode.LeftArrow))
            dirX = +1;
        else if (Input.GetKey(KeyCode.UpArrow))
            dirZ = -1;
        else if (Input.GetKey(KeyCode.DownArrow))
            dirZ = +1;

        var rotY = Head.transform.eulerAngles;
        Quaternion Right = Quaternion.Euler(0, rotY.y, 0);
        Body.transform.rotation = Quaternion.Slerp(transform.rotation, Right, Time.smoothDeltaTime * 5.0f);

        Vector3 moveDir = new Vector3(dirX * speedSide, 0, dirZ * speedForward);
        transform.Translate(Right * moveDir * Time.smoothDeltaTime);
    }
}
