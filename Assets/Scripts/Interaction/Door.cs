using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Door : MonoBehaviour, ICancallableInteractable
{
    private static readonly int ANIM_OpenDoor = Animator.StringToHash("OpenDoor");

    [SerializeField] int doorID;

    [SerializeField] Transform goalPosition;

    [SerializeField] CinemachineVirtualCamera vcam;

    [SerializeField] Transform signTransform;


    [SerializeField] AudioClip openDoor, closeDoor;
    [SerializeField] Shader changeHueShader;
    [SerializeField] MeshRenderer logoMeshRenderer;
    [SerializeField] Texture[] doorTextures;

    Animator animator;

    RuntimeVendorData data;
    public RuntimeVendorData Data => data;

    public event Action Cancel;

    public string CompanyName => IsSetup() ? data.InternalData.Name : "NULL";
    public string CompanyDescription => IsSetup() ? data.InternalData.Description : "NULL";
    public TextureRequest Logo => IsSetup() ? data.Logo : null;

    public int ID => doorID;

    private void Start()
    {
        animator = GetComponent<Animator>();
        RandomiseDoor();
    }

    private void RandomiseDoor()
    {
        UnityEngine.Random.InitState(ID); 

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
                        matInstance.mainTexture = doorTextures[UnityEngine.Random.Range(0, doorTextures.Length)];
                        matInstance.SetFloat("_hueShift", UnityEngine.Random.Range(0, 360));
                    }

                    mat[i] = matInstance;
                }
            }

            meshRenderer.materials = mat;
        }

        float deviation = UnityEngine.Random.Range(-0.2f, 0.2f);
        Vector3 scale = new Vector3(1, 1 - deviation, 1 - (deviation * -1));
        transform.localScale = scale;
        signTransform.localScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
    }

    public void Setup(RuntimeVendorData runtimeVendorData)
    {
        data = runtimeVendorData;
        DisplayLogo(Logo);
    }

    private void DisplayLogo(TextureRequest logo)
    {
        if (logo.IsReady)
        {
            Material newMaterial = new Material(logoMeshRenderer.material);
            newMaterial.mainTexture = logo.Texture;
            logoMeshRenderer.material = newMaterial;

            float width = ((float)logo.Texture.width / (float)logo.Texture.height) * logoMeshRenderer.transform.localScale.y;
            float multiplier = (width > 0.135f) ? (0.135f / width) : 1f;
            logoMeshRenderer.transform.localScale = new Vector3(width, 1, logoMeshRenderer.transform.localScale.y) * multiplier;
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
        string url = IsSetup() ? data.InternalData.GetLink(SocialMediaType.Zoom) : null;
        GretaUtil.OpenURL(url);
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