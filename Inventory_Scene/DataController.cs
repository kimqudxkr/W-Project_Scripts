using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class DataController : MonoBehaviour
{
    
    public playerData playerData;


    //아이템 추가함수
    public void additem(string pickupitem)//아이템 추가하는 함수(pickup에서 실행)
    {
        LoadPlayerDataFromJson();//json 업데이ㄴ

        string[] itemslist = playerData.items;
        foreach(string item in itemslist)//아이템 배열 속에 저장된 아이템 이름들을 하나씩 불러ㅇ
        {
            if (item == pickupitem)//만약 아이템 배열에 이미 같은 이름의 아이템이 있다면
                return;//리턴
        }

        Array.Resize(ref playerData.items, playerData.items.Length + 1);//배열 크기 1 증가
        // reading 배열의 길이를 3만큼 늘림, Array.Resize(배열의 주소값, 새 배열의 크기)
        int length = playerData.items.Length;//현재 배열 크기 
        playerData.items[length] = pickupitem;//배열에 이름 저장

        SavePlayerDataToJson();//json에 저장
    }

    //json 데이터 부분

    [ContextMenu("To Json Data")]//json파일에 데이터 저장
    void SavePlayerDataToJson()
    {
        string jsonData = JsonUtility.ToJson(playerData,true);
        string path = Path.Combine(Application.dataPath, "playerData.json");//현재 유니티 프로젝트 경로에 저장
        File.WriteAllText(path, jsonData);
    }

    [ContextMenu("From Json Data")]//json에서 데이터로 불러오ㅇ
    void LoadPlayerDataFromJson()
    {
        string path = Path.Combine(Application.dataPath, "playerData.json");//현재 유니티 프로젝트 경로에 저장
        string jsonData = File.ReadAllText(path);//경로로 부터 데이터 가져옴
        playerData = JsonUtility.FromJson<playerData>(jsonData);

    }


   

}


[System.Serializable]
public class playerData
{
    // Start is called before the first frame update
    public string name;
    public string[] items;
    public bool isDead;


}


