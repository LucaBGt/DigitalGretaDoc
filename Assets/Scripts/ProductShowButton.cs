using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductShowButton : MonoBehaviour
{
    Sprite myIcon;
    ProductCanvas pc;

    Image image;

    public void Setup(ProductCanvas _pc, Sprite _myIcon)
    {
        pc = _pc;
        myIcon = _myIcon;
        image = GetComponent<Image>();
        image.sprite = _myIcon;
    }

    public void ShowIcon()
    {
        pc.ShowImage(myIcon);
    }
}
