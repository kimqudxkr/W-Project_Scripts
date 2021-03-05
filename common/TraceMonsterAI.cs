using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class TraceMonsterAI : MonoBehaviour
{

    public int MonsterHP = 100;
    NavMeshAgent m_enemy = null;
    public Transform PlayerPos;
    [SerializeField] Transform[] m_tfWayPoints = null;
    int m_count = 0;
    public bool trace = false;

    private float DistanceToPlayer; //플레이어와의 거리
    Transform m_target = null;


    public void RemoveTarget()
    {
        m_target = null;//타겟제겨
        InvokeRepeating("MoveToNextWayPoint", 0f, 2f);//다시 반복 시작
    }


    void MoveToNextWayPoint()
    {
        if(m_target == null)
        {
            if (m_enemy.velocity == Vector3.zero)
            {
                m_enemy.SetDestination(m_tfWayPoints[m_count++].position);//속도가 0이 되면 다음 경로로 이동

                if (m_count >= m_tfWayPoints.Length)
                    m_count = 0;
            }
        }
        
    }


    private void OntriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            
           
            trace = true;
        }


        else if (other.tag == "Bullet")
        {
            other.gameObject.SetActive(false);
            MonsterHP = MonsterHP - 25;
            if (MonsterHP == 0)
            {
                Destroy(gameObject);

            }



        }
    }






    // Start is called before the first frame update
    void Start()
    {
        m_enemy = GetComponent<NavMeshAgent>();
        InvokeRepeating("MoveToNextWayPoint", 0f, 2f);//시작 후 2초마다 반복
       

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
        }
        if(trace == false)
        {
            InvokeRepeating("MoveToNextWayPoint", 0f, 2f);//시작 후 2초마다 반복
        }

        
    }

   void move()
    {
        m_enemy.SetDestination(PlayerPos.position);//거리가 멀면 이동
    }
}
