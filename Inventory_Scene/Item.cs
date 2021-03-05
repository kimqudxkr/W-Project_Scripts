using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
//생성 메뉴에 클래스 추가

public class item : ScriptableObject//게임 오브젝트에 안붙이고도 가능
{
    public string itemName;//아이템 이름
    public ItemType itemType;//아이템 타입
    public Sprite itemImage; //월드상에서 아이템 이미지 띄우는 변수
    public GameObject itemPrefab; //아이템이 맵에 나타날때 프리팹으로 나타남

    public string weaponType;//무기 타입 변수

    public enum ItemType//추가예정
    {
        Equipment,//장비
        ETC//기타
    }




}
