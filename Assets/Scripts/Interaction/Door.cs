using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Door : MonoBehaviour, ICancallableInteractable
{
    private static readonly int ANIM_OpenDoor = Animator.StringToHash("OpenDoor");

    [SerializeField] Transform goalPosition;

    [SerializeField] CinemachineVirtualCamera vcam;


    [SerializeField] AudioClip openDoor, closeDoor;
    [SerializeField] Shader changeHueShader;
    [SerializeField] MeshRenderer logoMeshRenderer;

    Animator animator;

    RuntimeVendorData data;
    public RuntimeVendorData Data => data;

    public event Action Cancel;

    public string CompanyName => IsSetup() ? data.InternalData.Name : "NULL";
    public string CompanyDescription => IsSetup() ? data.InternalData.Description : "NULL";
    public TextureRequest Logo => IsSetup() ? data.Logo : null;

    private void Start()
    {
        animator = GetComponent<Animator>();
        RandomiseDoorColor();
    }

    private void RandomiseDoorColor()
    {
        Material matInstance = null;
        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            Material[] mat = meshRenderer.materials;

            for (int i = 0; i < mat.Length; i++)
            {
                if (mat[i].shader == changeHueShader)
                {
                    if (matInstance == null)
                    {
                        matInstance = new Material(mat[i]);
                        matInstance.SetFloat("_hueShift", UnityEngine.Random.Range(0, 360));
                    }

                    mat[i] = matInstance;
                }
            }

            meshRenderer.materials = mat;
        }
    }

    public void Setup(RuntimeVendorData runtimeVendorData)
    {
        data = runtimeVendorData;
        DisplayLogo(Logo);
    }

    private void DisplayLogo(TextureRequest logo)
    {
        if(logo.IsReady)
        {
        Material newMaterial = new Material(logoMeshRenderer.material);
        newMaterial.mainTexture = logo.Texture;
        logoMeshRenderer.material = newMaterial;
        }
        else
        {
            //Logo has not finished downloading yet, should display some kind of loading indicator
            logo.FinishedDownload += (logo) => DisplayLogo(logo);
        }
    }

    private bool IsSetup()
    {
        return data != null;
    }

    public void EnterInteractionFromMap()
    {
        UIHandler.Instance.OpenDoor(this);
    }

    public void EnterInteraction()
    {
        UIHandler.Instance.OpenDoor(this);
        vcam.Priority = 20;
    }

    public void ExitInteraction()
    {
        UIHandler.Instance.CloseDoor(this);
        vcam.Priority = 5;
    }

    public Vector3 GetInteractPosition()
    {
        return goalPosition.position;
    }

    public void OpenURL()
    {
        string url = IsSetup() ? data.InternalData.LinkWebsite : null;
        if (!string.IsNullOrEmpty(url))
            Application.OpenURL(url);
    }

    public void CancelInteraction()
    {
        Cancel?.Invoke();
    }

    public float GetInteractYRotation()
    {
        Vector3 dir = transform.position - goalPosition.position;
        Vector3 rot = Quaternion.FromToRotation(Vector3.forward, dir).eulerAngles;
        return rot.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        animator.SetBool(ANIM_OpenDoor, true);
        SoundPlayer.Instance.Play(openDoor, source3D: transform, volume: 0.25f, randomPitchRange: 0.1f);
    }

    private void OnTriggerExit(Collider other)
    {
        animator.SetBool(ANIM_OpenDoor, false);
        SoundPlayer.Instance.Play(closeDoor, source3D: transform, volume: 0.25f, randomPitchRange: 0.1f);
    }
}