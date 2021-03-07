using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots : MonoBehaviour
{
    public ItemProperty item;
    public UnityEngine.UI.Image image;
    public int price;

    public void SetItem(ItemProperty item)
    {
        this.item = item;

        if(item==null)
        {
            image.enabled = false;      //Image라는 컴포넌트를 비화성화
            gameObject.name = "Empty";
        }
        else
        {
            image.enabled = true;
            gameObject.name = item.name;
            image.sprite = item.sprite;
            price = item.price;
        }
    }
}
