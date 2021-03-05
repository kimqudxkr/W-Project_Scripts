using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RightAlienScript : MonoBehaviour
{
    public float monSpeed = 4.0f;

    public bool trace = false;
    private Transform RightTarget;
    private int pointCount=0;
    
    public float MonsterHP = 100;//@@@@
    public GameObject HPParticle;
    public bool death = false;


    private Animator anim;
    public GameObject cell;


    chapter1_2Manager manager;

    NavMeshAgent agent;

    void Start() 
    {
        RightTarget = WayPointScript.RightPoints[0];
        anim = GetComponent<Animator>();
        anim.SetBool("walk", true);
        agent = GetComponent<NavMeshAgent>();
        manager = GameObject.Find("GameManager").GetComponent<chapter1_2Manager>();
    }

    void Update() 
    {
        Vector3 dir = RightTarget.position - transform.position;
        transform.Translate(dir.normalized*monSpeed*Time.deltaTime,Space.World);

        if(Vector3.Distance(transform.position, RightTarget.position) <= 0.2f)
        {
            getNextWayPoint();
        }

        if (Vector3.Distance(transform.position, cell.transform.position) <= 4.0f)
        {
            transform.LookAt(cell.transform);
            agent.enabled = false;
            anim.SetBool("walk", false);
            anim.SetTrigger("attack1");
        }

        if (death == true)
        {
            monterDie();
            death = false;
        }
    }

    void getNextWayPoint()
    {
        if (pointCount < WayPointScript.RightPoints.Length)
        {
            transform.LookAt(WayPointScript.RightPoints[pointCount]);
            RightTarget = WayPointScript.RightPoints[pointCount];
            pointCount++;
        }   
    }




    //Change the HP and Instantiates an HP Particle with default force and color//노말@@@@@@@@@@@
    public void ChangeHP(float Delta, Vector3 Position)//총알 데미지, 위치
    {
        MonsterHP = MonsterHP + Delta;//체력 감소

        Destroy(Instantiate(HPParticle, Position, gameObject.transform.rotation), 3.0f);



        if (MonsterHP <= 0)
        {
            this.GetComponent<BoxCollider>().enabled = false;
            agent.enabled = false;

            death = true;


        }

    }

    public void monterDie()//몬스터 죽음
    {
        manager.Mcount--;
        anim.SetBool("walk", false);
        anim.SetTrigger("death");//죽는 애니메이션
        StartCoroutine("DeathFunc");
    }

    IEnumerator DeathFunc()//죽는 모션동안 몬스터 파괴하지 않고 대기하는 코루틴
    {


        yield return new WaitForSeconds(1f);//1초 대기
        Destroy(gameObject);


    }


}