using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public enum PlayerState
{
    Walking, Looking, UI
}

public enum PerspectiveMode
{
    FirstPerson,
    ThirdPerson,
}

[DefaultExecutionOrder(-100)]
public class LocalPlayerBehaviour : MonoBehaviour
{
    private static LocalPlayerBehaviour instance;

    [SerializeField] NavMeshAgent agent;

    private PerspectiveMode perspective;
    [SerializeField] GameObject firstPersonCam, thirdPersonCam;

    [Header("Inputs")]
    float turnInput;
    [SerializeField] float sensetivity;

    private PlayerState state;
    //Local References
    Camera camera;

    public event System.Action<PlayerState> ChangePlayerState;

    public static LocalPlayerBehaviour Instance => instance;
    public PlayerState State
    {
        get => state;
        set
        {
            state = value;
            ChangePlayerState?.Invoke(state);
            Debug.Log($"New State: {value}");
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Spawned Second Instance of LocalPlayer");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        //If the mouse button is clicked/The Screen is tapped...
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }

        if (ReachedDestination())
        {
            StopMoving();
        }

    }

    private bool ReachedDestination()
    {
        if (!agent.pathPending)
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
                if (State == PlayerState.Looking)
                {
                    ClickOnLooking(hit);
                }
                else if (State == PlayerState.Walking)
                {
                    ClickOnWalking(hit);
                }
            }
        }
    }

    private void ClickOnWalking(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent(out IInteractable interactable))
        {
            GoTo(interactable.GetInteractPosition());
        }
        else
        {
            GoTo(hit.point);
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
        if (State != PlayerState.Walking) return;

        State = PlayerState.Looking;
        agent.isStopped = true;
    }

    private void ClickOnLooking(RaycastHit hit)
    {
        ClickOnWalking(hit);
    }

    public void TogglePerspective()
    {
        perspective = perspective == PerspectiveMode.FirstPerson ? PerspectiveMode.ThirdPerson : PerspectiveMode.FirstPerson;

        firstPersonCam.SetActive(perspective == PerspectiveMode.FirstPerson);
        thirdPersonCam.SetActive(perspective == PerspectiveMode.ThirdPerson);
    }

    private void FixedUpdate()
    {
        if (State != PlayerState.UI)
            transform.Rotate(0, turnInput * sensetivity, 0);
    }

    private IEnumerator turnToDoor()
    {
        State = PlayerState.UI;
        //LeanTween.rotate(this.gameObject, currentDoor.goalPosition.rotation.eulerAngles, 1);
        yield return new WaitForSeconds(1f);
        State = PlayerState.Walking;
    }

    public void TurnLeft()
    {
        StopMoving();
        turnInput = -1;
    }

    public void TurnRight()
    {
        StopMoving();
        turnInput = 1;
    }

    public void StopTurning()
    {
        turnInput = 0;
    }


    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

}