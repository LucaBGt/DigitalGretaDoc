using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProductCanvas : MonoBehaviour
{
    public Product productInf;
    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;
    public GameObject buttonPrefab;
    public Transform linkParent;

    public void Setup(Product _productInf)
    {
        productInf = _productInf;
        title.text = productInf.productName;
        desc.text = productInf.productDescription;
        icon.sprite = productInf.icons[0];

        foreach (Transform transform in linkParent)
        {
            Destroy(transform.gameObject);
        }


        foreach (Sprite l in productInf.icons)
        {

            GameObject g = Instantiate(buttonPrefab, linkParent);

            ProductShowButton psb = g.GetComponent<ProductShowButton>();
            psb.Setup(this, l);
        }
    }

    public void SetClose()
    {
        GameData.setCanMove.Invoke(false);
        GameData.setLookMode.Invoke(true);

        this.gameObject.SetActive(false);
    }

    public void ShowImage(Sprite s)
    {
        icon.sprite = s;
    }
}
