using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public enum PlayerState
{
    Walking,
    Looking,
    RotatingToTarget,
    InInteraction
}

public enum PerspectiveMode
{
    FirstPerson,
    ThirdPerson
}

[DefaultExecutionOrder(-100)]
public class LocalPlayerBehaviour : SingletonBehaviour<LocalPlayerBehaviour>, IPlayerBehaviour
{
    [SerializeField] NavMeshAgent agent;

    [SerializeField] GameObject firstPersonCam, thirdPersonCam;

    [SerializeField] GameObject targetPreviewPrefab;
    [SerializeField] PlayerAnimationHandler animationHandler;


    [SerializeField] LayerMask raycastLayer;
    [SerializeField] float raycastMaxDistance;

    [Header("Inputs")]
    [SerializeField] float sensetivity;

    [Header("Player Name")]
    [SerializeField] TextMeshPro playerNameText;

    PlayerState internal_State;
    PerspectiveMode perspective = PerspectiveMode.ThirdPerson;
    IInteractable currentInteractable = null;
    GameObject targetPreview;

    new Camera camera;
    float turnInput;
    GameObject currentVisuals;

    public event Action<PlayerState> PlayerStateChanged;

    public PlayerState State
    {
        get => internal_State;
        set
        {
            internal_State = value;
            PlayerStateChanged?.Invoke(internal_State);
        }
    }

    private void Start()
    {
        camera = Camera.main;
        targetPreview = Instantiate(targetPreviewPrefab);
        targetPreview.SetActive(false);
        UpdateCameras();
        UIHandler.Instance.ReturnedToGame += OnReturnedToGame;
    }

    private void OnReturnedToGame()
    {
        playerNameText.text = Settings.Instance.Username;
        SetupLocalPlayerVisuals(ref currentVisuals, transform, Settings.Instance.UserSkinID, animationHandler);
    }

    private void OnDestroy()
    {
        if (UIHandler.Instance)
            UIHandler.Instance.ReturnedToGame -= OnReturnedToGame;
    }

    void Update()
    {
        bool locked = UIHandler.Instance ? UIHandler.Instance.InLockedUIMode : false;

        if (!locked && Input.GetMouseButtonDown(0))
        {
            OnClick();
        }

        if (ReachedDestination())
        {
            StopMoving();

            if (currentInteractable != null)
                State = PlayerState.RotatingToTarget;
            else
                State = PlayerState.Looking;
        }

        if (State == PlayerState.RotatingToTarget)
            UpdateRotatingToTarget();


    }

    private void UpdateRotatingToTarget()
    {
        if (currentInteractable == null)
        {
            Debug.Log("Interactable null but state requires interactable.");
            return;
        }

        float target = NormalizedAngleRange(currentInteractable.GetInteractYRotation());
        float current = NormalizedAngleRange(transform.eulerAngles.y);

        float dir = OffsetBetweenAngles(current, target);

        //*0.1 to slow down when below 10 degrees, allow low fps devices to work correctly 
        dir = Mathf.Clamp(dir * 0.1f, -1, 1);
        float movement = dir * Time.deltaTime * sensetivity;
        transform.Rotate(0, movement, 0);

        if (Mathf.Abs(dir) < 0.01f)
        {
            //target reached
            currentInteractable.EnterInteraction();
            if (currentInteractable is ICancallableInteractable cancallableInteractable)
            {
                cancallableInteractable.Cancel += OnInteractableCancelInteraction;
            }

            State = PlayerState.InInteraction;
        }
    }

    private void OnInteractableCancelInteraction()
    {
        QuitInteraction();
    }

    private bool ReachedDestination()
    {
        if (!agent.pathPending && !agent.isStopped)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnClick()
    {
        //not over UI
        if (!IsPointerOverUIObject())
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, raycastMaxDistance, raycastLayer.value, QueryTriggerInteraction.Ignore))
            {
                Transform objectHit = hit.transform;
                ApplyClickOn(hit);
            }
        }
    }

    private void ApplyClickOn(RaycastHit hit)
    {
        QuitInteraction();

        if (hit.transform.TryGetComponent(out ICancallableInteractable interactable))
        {
            GoTo(interactable.GetInteractPosition());
            currentInteractable = interactable;
        }
        else
        {
            GoTo(hit.point);
            currentInteractable = null;
        }
    }

    private void GoTo(Vector3 point)
    {
        agent.isStopped = false;
        agent.destination = point;
        State = PlayerState.Walking;

        if (targetPreview)
        {
            targetPreview.SetActive(true);
            targetPreview.transform.position = point;
        }
    }

    private void StopMoving()
    {
        agent.isStopped = true;
        targetPreview?.SetActive(false);
    }

    public void TogglePerspective()
    {
        perspective = (PerspectiveMode)((((int)perspective) + 1) % 2);

        UpdateCameras();
    }

    private void UpdateCameras()
    {
        firstPersonCam.SetActive(perspective == PerspectiveMode.FirstPerson);
        thirdPersonCam.SetActive(perspective == PerspectiveMode.ThirdPerson);
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, turnInput * sensetivity * Time.fixedDeltaTime, 0);
    }

    public void TurnLeft()
    {
        QuitInteraction();
        StopMoving();
        turnInput = -1;
        State = PlayerState.Looking;
    }

    public void TurnRight()
    {
        QuitInteraction();
        StopMoving();
        turnInput = 1;
        State = PlayerState.Looking;
    }

    public void StopTurning()
    {
        turnInput = 0;
    }

    public void Stop()
    {
        StopMoving();
        State = PlayerState.Looking;
    }

    private void QuitInteraction()
    {
        if (State == PlayerState.InInteraction && currentInteractable != null)
        {
            currentInteractable.ExitInteraction();
            if (currentInteractable is ICancallableInteractable cancallableInteractable)
            {
                cancallableInteractable.Cancel -= OnInteractableCancelInteraction;
            }
            State = PlayerState.Looking;
        }
        currentInteractable = null;
    }


    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }


    /// <summary>
    /// Returns range -180 to +180
    /// </summary>
    private static float NormalizedAngleRange(float f)
    {
        f = f % 360;
        if (f > 180)
            return f - 360;
        else if (f < -180)
            return f + 360;

        return f;
    }

    private static float OffsetBetweenAngles(float start, float target)
    {
        float offset = target - start;
        return NormalizedAngleRange(offset);
    }

    public static void SetupLocalPlayerVisuals( ref GameObject currentVisuals, Transform parent, int _playerPrefabId, PlayerAnimationHandler animationHandler)
    {
        if (currentVisuals != null)
            Destroy(currentVisuals);
        GameObject prefab = null;

        if (_playerPrefabId < Settings.Instance.SkinsCount && _playerPrefabId >= 0)
        {
            prefab = Settings.Instance.VisualPrefabs[_playerPrefabId];
        }
        else
        {
            Debug.LogWarning($"Passed skinID {_playerPrefabId} not present in this version. Selecting default");
            prefab = Settings.Instance.VisualPrefabs[0];
        }

        currentVisuals = Instantiate(prefab , parent);
        currentVisuals.transform.localPosition = Vector3.zero;
        currentVisuals.transform.localRotation = Quaternion.identity;

        animationHandler.SetAnimator(currentVisuals.GetComponent<Animator>());

        Debug.Log("setup visuals: " + currentVisuals.name);

    }

}