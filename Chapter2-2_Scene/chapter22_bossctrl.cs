using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chapter22_bossctrl : MonoBehaviour
{

    public Animator anim;
    public float MonsterHP = 100;
    public float DistanceToPlayer; //플레이어와의 거리
    public Transform PlayerPos;
    UnityEngine.AI.NavMeshAgent agent;


    public bool death = false;
    public GameObject gamemanager;
    public GameObject field_dropItem;
    public GameObject item_loc;

    //데미지 오브젝트 생성
    public GameObject HPParticle;

    public GameObject player;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();

        field_dropItem.SetActive(false);
    }

    void Update()
    {
        transform.LookAt(PlayerPos);//플레이어 쳐다봄
        DistanceToPlayer = Vector3.Distance(transform.position, PlayerPos.position);
        if (DistanceToPlayer > 6.0f)
        {
            anim.SetBool("walk", true);
            move();
        }
        else
        {
            anim.SetTrigger("attack1");
            anim.SetBool("walk", false);
        }
        if (death == true)
        {
            monterDie();
            death = false;
        }

        field_dropItem.transform.position = item_loc.transform.position;
    }

    void OnTriggerEnter(Collider other)
    {


        if (other.tag == "Bullet")
        {
            other.gameObject.SetActive(false);
/*            anim.SetBool("walk", false);
            anim.SetTrigger("hit");
            anim.SetBool("walk", true);
*/





        }
    }

    void move()
    {
        agent.SetDestination(PlayerPos.position);//거리가 멀면 이동
    }


    public void ChangeHP(float Delta, Vector3 Position)//총알 데미지, 위치
    {
        MonsterHP = MonsterHP + Delta;//체력 감소

        Destroy(Instantiate(HPParticle, Position, gameObject.transform.rotation), 3.0f);


        if (MonsterHP <= 0)
        {
            //this.GetComponent<monster_tutorial2>().addKillCount();

            this.GetComponent<BoxCollider>().enabled = false;
            agent.enabled = false;

            death = true;


        }

        
    }

    public void monterDie()//몬스터 죽음
    {
        anim.SetBool("walk", false);
        anim.SetTrigger("death");//죽는 애니메이션
        field_dropItem.SetActive(true);
        gamemanager.GetComponent<chapter22_gamemanager>().ShowDialogue();
        StartCoroutine("DeathFunc");
    }


    IEnumerator DeathFunc()
    {
        

        yield return new WaitForSeconds(2.0f);//3초 대기
        Destroy(gameObject);


    }
}
