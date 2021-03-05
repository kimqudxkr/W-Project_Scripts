using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{

    public int MonsterHP = 100;

    Rigidbody myRigid;
    [SerializeField] private float moveSpeed;
    public Transform PlayerPos;
    NavMeshAgent agent;
    private Animator anim;

    public float DistanceToPlayer; //플레이어와의 거리

    // Start is called before the first frame update
    void Start()
    {
        myRigid = GetComponent < Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(PlayerPos);//플레이어 쳐다봄
        DistanceToPlayer = Vector3.Distance(transform.position, PlayerPos.position);
        if(DistanceToPlayer > 5.0f)
        {
            anim.SetBool("isNear", false);
            move();
        }
        else
        {

            anim.SetBool("isNear", true);//애니메이터

        }


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            other.gameObject.SetActive(false);
            MonsterHP = MonsterHP - 25;
            if(MonsterHP == 0)
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
