using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapDoorIndicator : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI companyName;
    [SerializeField] Image logo;


    public void Setup(Door d)
    {
        logo.sprite = d.Logo;
        companyName.text = d.CompanyName;
    }
}
