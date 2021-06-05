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
    ThirdPerson,
}

[DefaultExecutionOrder(-100)]
public class LocalPlayerBehaviour : SingletonBehaviour<LocalPlayerBehaviour>
{
    [SerializeField] NavMeshAgent agent;

    [SerializeField] GameObject firstPersonCam, thirdPersonCam;

    [Header("Inputs")]
    [SerializeField] float sensetivity;

    PlayerState state;
    PerspectiveMode perspective;
    IInteractable currentInteractable = null;

    Camera camera;
    float turnInput;

    public event System.Action<PlayerState> ChangePlayerState;

    public PlayerState State
    {
        get => state;
        set
        {
            state = value;
            ChangePlayerState?.Invoke(state);
        }
    }

    private void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }

        if (ReachedDestination())
        {
            StopMoving();
            Debug.Log("Reached Destination");

            if (currentInteractable != null)
                state = PlayerState.RotatingToTarget;
            else
                state = PlayerState.Looking;
        }

        if (state == PlayerState.RotatingToTarget)
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

        dir = Mathf.Clamp(dir, -1, 1);
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

            state = PlayerState.InInteraction;
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

            if (Physics.Raycast(ray, out hit))
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
    }

    private void StopMoving()
    {
        agent.isStopped = true;
    }

    public void TogglePerspective()
    {
        perspective = (PerspectiveMode)((((int)perspective) + 1) % 2);

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
    }

    public void TurnRight()
    {
        QuitInteraction();
        StopMoving();
        turnInput = 1;
    }

    public void StopTurning()
    {
        turnInput = 0;
    }

    private void QuitInteraction()
    {
        if (state == PlayerState.InInteraction && currentInteractable != null)
        {
            currentInteractable.ExitInteraction();
            if (currentInteractable is ICancallableInteractable cancallableInteractable)
            {
                cancallableInteractable.Cancel -= OnInteractableCancelInteraction;
            }
            state = PlayerState.Looking;
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

}