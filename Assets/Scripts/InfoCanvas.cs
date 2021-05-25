using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoCanvas : MonoBehaviour
{

    public Vendor vendorInfo;

    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;
    public GameObject buttonPrefab;
    public Transform linkPrefabs;

    public void Setup(Vendor _vendorInfo)
    {
        vendorInfo = _vendorInfo;
        title.text = vendorInfo.companyName;
        desc.text = vendorInfo.description;
        icon.sprite = vendorInfo.companyIcon;

        foreach (Transform transform in linkPrefabs)
        {
            Destroy(transform.gameObject);
        }


        foreach (Link l in vendorInfo.links)
        {
            GameObject g = Instantiate(buttonPrefab, linkPrefabs);
            SocialMediaButton smb = g.GetComponent<SocialMediaButton>();
            smb.linklink = l.Address;

            Image i = g.GetComponent<Image>();
            i.sprite = l.icon;
        }
    }

    public void SetClose()
    {
        GameData.setCanMove.Invoke(false);
        GameData.setLookMode.Invoke(true);

        this.gameObject.SetActive(false);
    }
}
