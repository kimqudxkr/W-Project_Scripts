using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TraceAI : MonoBehaviour
{
    NavMeshAgent m_enemy = null;
    
    [SerializeField] Transform[] m_tfWayPoints = null;
    int m_count = 0;

    public int MonsterHP = 100;

    Rigidbody myRigid;
    [SerializeField] private float moveSpeed;
    public Transform PlayerPos;
    NavMeshAgent agent;
    private Animator anim;

    public float DistanceToPlayer; //플레이어와의 거리

    void MoveToNextWayPoint()
    {
        if(m_enemy.velocity == Vector3.zero)
        {
            m_enemy.SetDestination(m_tfWayPoints[m_count].position);//속도가 0이 되면 다음 경로로 이동

            if (m_count >= m_tfWayPoints.Length)
                m_count = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_enemy = GetComponent<NavMeshAgent>();
        InvokeRepeating("MoveToNextWayPoint", 0f, 2f);//시작 후 2초마다 반복
        myRigid = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
/*        transform.LookAt(PlayerPos);//플레이어 쳐다봄
        DistanceToPlayer = Vector3.Distance(transform.position, PlayerPos.position);
        if (DistanceToPlayer > 5.0f)
        {
            anim.SetBool("isNear", false);
            move();
        }
        else
        {

            anim.SetBool("isNear", true);//애니메이터

        }*/


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            other.gameObject.SetActive(false);
            MonsterHP = MonsterHP - 25;
            if (MonsterHP == 0)
            {
                Destroy(gameObject);

            }



        }
    }


    void move()
    {

        agent.SetDestination(PlayerPos.position);//거리가 멀면 이동
    }
}
