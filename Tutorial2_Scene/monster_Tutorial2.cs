using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monster_tutorial2 : MonoBehaviour
{
    public GameObject GameManager;
    tutorial2_GameManagerLogic logic;
    public void Start()
    {
        logic = GameManager.GetComponent<tutorial2_GameManagerLogic>();
    }

    public void addKillCount()
    {
        logic.kill++;
    }
}
