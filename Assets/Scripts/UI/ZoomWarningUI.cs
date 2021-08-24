using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScalingUIElement))]
public class ZoomWarningUI : SingletonBehaviour<ZoomWarningUI>
{
    ScalingUIElement scalingUIElement;
    [SerializeField] Button continueButton, closeButton;

    protected override void Awake()
    {
        base.Awake();
        scalingUIElement = GetComponent<ScalingUIElement>();
        closeButton.onClick.AddListener(() => Close());
    }

    public void ShowWarning(string url)
    {
        scalingUIElement.SetActiveTransition(true);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => GretaUtil.OpenURL(url));
        continueButton.onClick.AddListener(() => Close());
    }
    public void Close()
    {
        scalingUIElement.SetActiveTransition(false);
    }
}
