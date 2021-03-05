using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBaseAI : MonoBehaviour
{

    public int MonsterHP = 100;
    public bool trace = false;
    Rigidbody myRigid;
    public Transform PlayerPos;
    public GameObject sight;
    public GameObject sword;


    [SerializeField] Transform[] m_tfWayPoints = null;
    int m_count = 0;
    NavMeshAgent agent;
    private Animator anim;
    Transform m_target = null;
    public float DistanceToPlayer; //플레이어와의 거리

    // Start is called before the first frame update
    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
        InvokeRepeating("MoveToNextWayPoint", 0f, 2f);//시작 후 2초마다 반복
        sword.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
 
        DistanceToPlayer = Vector3.Distance(transform.position, PlayerPos.position);


        if (trace == true)
        {
            CancelInvoke();//반복 중지
            move();
            if (DistanceToPlayer > 15.0f)
            {
                trace = false;
            }

            //if (DistanceToPlayer > 5.0f)//칼 애니메이션
            //{
            //    anim.SetBool("isNear", false);
            //    move();
            //}
            //else
            //{

            //    anim.SetBool("isNear", true);//애니메이터

            //}
        }
        if (trace == false)
        {
            InvokeRepeating("MoveToNextWayPoint", 0f, 2f);//시작 후 2초마다 반복
        }


    }

    void MoveToNextWayPoint()
    {
        if (m_target == null)
        {
            if (agent.velocity == Vector3.zero)
            {
                agent.SetDestination(m_tfWayPoints[m_count++].position);//속도가 0이 되면 다음 경로로 이동
                sight.gameObject.SetActive(true);
                sword.gameObject.SetActive(false);
                if (m_count >= m_tfWayPoints.Length)
                    m_count = 0;
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trace = true;
            sight.gameObject.SetActive(false);
            //sword.gameObject.SetActive(true);
        }


        //else if (other.tag == "Bullet")
        //{
        //    other.gameObject.SetActive(false);
        //    MonsterHP = MonsterHP - 25;
        //    if (MonsterHP == 0)
        //    {
        //        Destroy(gameObject);

        //    }
        //}
    }


    void move()
    {

        agent.SetDestination(PlayerPos.position);//거리가 멀면 이동
    }
}
