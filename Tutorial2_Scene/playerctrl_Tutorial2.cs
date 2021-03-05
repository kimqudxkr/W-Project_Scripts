using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playerctrl_tutorial2 : MonoBehaviour
{


    public int ExtraBullet = 0; //탄창 수@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    public bool first_equip = true;

    public Transform FirePos;   //총알이 발사될 위치

    public GameObject Bullet;   //총알 오브젝트 가져옴

    public int HP;      //플레이어의 체력

    public int itemCount;


    public bool FirePossible = false;



    public Text HPCountText;
    public Text BulletCountText;


    public float dirX = 0;
    public float dirZ = 0;
    public int speedForward = 12;
    public int speedSide = 6;

    public Transform Head;
    public Transform Body;

    public Rigidbody rb;

    gameInformationManager gameInformationManager;


    [Header("사운드 등록")]
    [SerializeField] Sound[] bgmSounds;
    //0 : 총 발사
    //1 : 아이템 획득

    [Header("브금 플레이어")]
    [SerializeField] AudioSource bgmPlayer;




    private void Start()
    {
        gameInformationManager = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>();
        gameInformationManager.isTime = true;
        rb = GetComponent<Rigidbody>();
    }


    public void Update()
    {
        MovePlayer();

        BulletCountText.text = "BULLET : " + ExtraBullet.ToString();


        if (GameObject.Find("Player_arm").transform.childCount >= 1)
        {
            FirePossible = true;
            if (first_equip == true)
            {
                ExtraBullet = 10;
                first_equip = false;
            }
        }
        else
        {
            FirePossible = false;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            if ((ExtraBullet > 0) && FirePossible == true)
            {
                Debug.Log("1번째");
                ExtraBullet = ExtraBullet - 1;
                Fire();
            }

            else if (ExtraBullet == 0 && FirePossible == true)
            {
                Debug.Log("2번째");
                GameObject.Find("Player_arm").transform.GetChild(0).gameObject.GetComponent<Animator>().SetTrigger("ReLoad");
                
                StartCoroutine("ReloadFunc");
            }
        }

        
        HP = gameInformationManager.player_HP;//게임정보 매니져의 플레이어 체력과 연동
        HPCountText.text = "HP : " + HP.ToString();

  



    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "equip")//수정 예정
        {
            bgmPlayer.clip = bgmSounds[1].Clip;//클립 불러옴###############
            bgmPlayer.Play();
            other.gameObject.SetActive(false);
            GameObject.Find("GameManager").GetComponent<tutorial2_GameManagerLogic>().ShowDialogue();
            //GameObject.Find("GameManager").GetComponent<GameManagerLogic>().bosscreate = true;//게임매니져로 이동
        }


        else if (other.tag == "heal_item")
        {

            bgmPlayer.clip = bgmSounds[1].Clip;//클립 불러옴###############
            bgmPlayer.Play();
            gameInformationManager.player_HP += 20;
            other.gameObject.SetActive(false);
        }


        
        else if (other.tag == "claw")
        {
            Debug.Log("맞았음");
            gameInformationManager.player_HP -= 5;

        }

    }



    void Fire()
    {
        bgmPlayer.clip = bgmSounds[0].Clip;//클립 불러옴###############
        bgmPlayer.Play();
        //총알 오브젝트가 FirePos의 위치와 회전방향을 가진상태로 생성
        Destroy(Instantiate(Bullet, FirePos.position, FirePos.rotation), 3.0f);
        



    }


    IEnumerator ReloadFunc()//장전 모션동안 대기하는 코루틴
    {

        //gunAnim.SetBool("isReLoad", true);//장전 모션 실행


        yield return new WaitForSeconds(3.0f);//3초 대기
        ExtraBullet = 10;//탄창 장전
        //gunAnim.SetBool("isReLoad", false);//장전 모션 중지

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
                    dirX = +1;
                }
                else
                {
                    dirX = -1;
                }
            }
            else
            {
                if (coord.y > 0)
                    dirZ = +1;
                else
                    dirZ = -1;
            }
        }

        if (Input.GetKey(KeyCode.RightArrow))
            dirX = +1;
        else if (Input.GetKey(KeyCode.LeftArrow))
            dirX = -1;
        else if (Input.GetKey(KeyCode.UpArrow))
            dirZ = +1;
        else if (Input.GetKey(KeyCode.DownArrow))
            dirZ = -1;

        var rotY = Head.transform.eulerAngles;
        Quaternion Right = Quaternion.Euler(0, rotY.y, 0);
        Body.transform.rotation = Quaternion.Slerp(transform.rotation, Right, Time.smoothDeltaTime * 5.0f);

        Vector3 moveDir = new Vector3(dirX * speedSide, 0, dirZ * speedForward);
        //transform.Translate(Right * moveDir * Time.smoothDeltaTime);
        rb.velocity = Right * moveDir;
    }
}
