using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class KeyboardKey : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;

    [SerializeField] AnimationCurve heightCurve, emmisionCurve;
    [SerializeField] Color baseColor, activeColor;
    [SerializeField] AudioSource sound;

    Vector3 basePos;
    float alpha = 0;

    private void OnEnable()
    {
        basePos = transform.position;
        meshRenderer.material = new Material(meshRenderer.material);
        Animate(0);
    }

    private IEnumerator AnimateTowards(bool up = true)
    {
        while (up ? alpha < 1 : alpha > 0)
        {

            alpha += Time.deltaTime * (up ? 1f : -1f) * 2;

            if (up ? (alpha >= 1) : (alpha <= 0))
                alpha = up ? 1 : 0;

            Animate(alpha);

            yield return null;
        }
    }

    private void Animate(float alpha)
    {
        transform.position = basePos + Vector3.up * heightCurve.Evaluate(alpha);
        meshRenderer.material.SetColor("baseColor", Color.Lerp(baseColor, activeColor, alpha));
        meshRenderer.material.SetColor("topColor", Color.Lerp(baseColor, activeColor, alpha));
        meshRenderer.material.SetFloat("emissionIntensity", emmisionCurve.Evaluate(alpha));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (sound != null)
            sound.Play();
        StopAllCoroutines();
        StartCoroutine(AnimateTowards(true));
    }

    private void OnTriggerExit(Collider other)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateTowards(false));
    }
}
