using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapDoorIndicator : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI companyName;
    [SerializeField] Image logo;
    [SerializeField] Button button;

    public void Setup(Door d)
    {
        if (d.Logo == null)
            logo.color = Color.clear;
        else
            logo.sprite = d.Logo;

        companyName.text = d.CompanyName;

        button.onClick.AddListener(delegate { d.EnterInteraction(); });
    }
}
