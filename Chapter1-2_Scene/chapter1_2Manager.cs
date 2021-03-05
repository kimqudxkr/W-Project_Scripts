using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class chapter1_2Manager : MonoBehaviour
{
    public GameObject rightMonster;
    public GameObject leftMonster;
    public GameObject centerMonster;
    public int Mcount = 14;

    public GameObject right;
    public GameObject left;
    public GameObject center;

    gameInformationManager infomanager;

    // Start is called before the first frame update
    void Start()
    {
        infomanager = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>();
        infomanager.isTime = true;

        Debug.Log("생성합니다Right");
        createRightMonster();
        Invoke("createRightMonster", 1.0f);
        Invoke("createRightMonster", 2.0f);

        Debug.Log("생성합니다Left");
        Invoke("createLeftMonster", 7.0f);
        Invoke("createLeftMonster", 8.0f);
        Invoke("createLeftMonster", 9.0f);

        Invoke("createCenterMonster", 14.0f);
        Invoke("createCenterMonster", 15.0f);

        Invoke("createRightMonster", 18.0f);
        Invoke("createRightMonster", 19.0f);

        Invoke("createLeftMonster", 18.0f);
        Invoke("createLeftMonster", 19.0f);

        Invoke("createCenterMonster", 18.0f);
        Invoke("createCenterMonster", 19.0f);
    }

    private void Update()
    {
        if (Mcount == 0)
        {
            Mcount = -1;
            infomanager.isTime = false;
            GameObject.Find("DialogueManager").GetComponent<chapter1_2DialogueManager>().ShowDialogue();
        }
    }

    public void createRightMonster()
    {
        Instantiate(rightMonster, right.transform, false);
    }

    public void createLeftMonster()
    {
        Instantiate(leftMonster, left.transform, false);
    }

    public void createCenterMonster()
    {
        Instantiate(centerMonster, center.transform, false);
    }
}
