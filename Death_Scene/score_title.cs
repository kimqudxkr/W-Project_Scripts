using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class score_title : MonoBehaviour
{

    public GameObject score_text;
    public int score;

    void Start()
    {
        score = GameObject.Find("GameInformationManager").GetComponent<gameInformationManager>().total_score;
        //점수 가져오기

        score_text.GetComponent<Text>().text = "점수 : " + score.ToString();

    }


}
