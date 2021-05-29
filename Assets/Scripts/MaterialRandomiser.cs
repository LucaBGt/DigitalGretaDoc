using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MaterialRandomiser : MonoBehaviour
{
    [SerializeField] Material[] materials;
    private void OnEnable()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && materials != null)
        {
            meshRenderer.material = materials[Random.Range(0, materials.Length)];
        }
    }
}
