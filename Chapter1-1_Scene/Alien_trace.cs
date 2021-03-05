using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Alien_trace : MonoBehaviour
{
    public float MonsterHP = 100;
    public bool trace = false;
    public Transform PlayerPos;
    public GameObject sight;
    public GameObject Player;

    //데미지 오브젝트 생성
    public GameObject HPParticle;

    public bool death = false;

    [SerializeField] Transform[] m_tfWayPoints = null;
    int m_count = 0;
    NavMeshAgent agent;
    private Animator anim;
    Transform m_target = null;
    public float DistanceToPlayer; //플레이어와의 거리

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        InvokeRepeating("MoveToNextWayPoint", 0f, 2f);//시작 후 2초마다 반복
    }

    // Update is called once per frame
    void Update()
    {

        DistanceToPlayer = Vector3.Distance(transform.position, PlayerPos.position);
        if (death == true)
        {
            monterDie();
            death = false;
        }

        if (trace == true)//플레이어 인식중
        {
            CancelInvoke();//순찰 반복 중지
            move();
            if (DistanceToPlayer > 15.0f)//거리가 너무 멀어지면
            {
                trace = false;//플레이어를 쫓아가는 것을 포기한ㄷ
            }

            if (DistanceToPlayer > 2.0f)//공격 사정범위 안에서 멀어지면
            {
                agent.enabled = true;
                move();
                anim.SetBool("walk", true);//플레이어를 쫓아가기 위해 공격을 멈추고 걷는다
            }
            else//플레이어가 공격 사정범위 안에 있다면
            {
                anim.SetBool("walk", false);
                agent.enabled = false;
                anim.SetTrigger("attack1");//공격
            }
        }
        if (trace == false)
        {
            InvokeRepeating("MoveToNextWayPoint", 0f, 2f);//시작 후 2초마다 반복
        }
    }

    void MoveToNextWayPoint()
    {
        anim.SetBool("walk", true);

        if (m_target == null)
        {

            if (agent.velocity == Vector3.zero)
            {
                anim.SetBool("walk", false);

                agent.SetDestination(m_tfWayPoints[m_count++].position);//속도가 0이 되면 다음 경로를 목적지로 지/
                sight.gameObject.SetActive(true);

                if (m_count >= m_tfWayPoints.Length)
                    m_count = 0;
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))//플레이어 발견 시
        {
            trace = true;
            sight.gameObject.SetActive(false);
        }
    }

    void move()
    {
        agent.SetDestination(PlayerPos.position);//거리가 멀면 이동
    }


    //Change the HP and Instantiates an HP Particle with default force and color//노말
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

    //죽는 모션동안 몬스터 파괴하지 않고 대기하는 코루틴
    IEnumerator DeathFunc()
    {
        yield return new WaitForSeconds(1f);    //1초 대기
        Destroy(gameObject);
    }

    public void monterDie()     //몬스터 죽음
    {
        anim.SetBool("walk", false);
        anim.SetTrigger("death");   //죽는 애니메이션
        StartCoroutine("DeathFunc");
    }
}
