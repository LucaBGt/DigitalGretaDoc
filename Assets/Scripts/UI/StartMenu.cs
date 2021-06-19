using Cinemachine;
using System.Collections;
using UnityEngine;


public class StartMenu : MonoBehaviour
{
    [SerializeField] StartMenuStage[] startMenuStages;
    StartMenuStage current;

    private void OnEnable()
    {
        foreach (StartMenuStage stage in startMenuStages)
        {
            if (stage.canvasGroup != null)
            {
                stage.canvasGroup.alpha = 0;
                stage.canvasGroup.interactable = false;
            }
        }
        JumpToMenuStage(0);
    }

    public void JumpToMenuStage(int stage)
    {
        if (current != null)
        {
            current.Exit();

            if (current.ShouldFadeOut())
            {
                if (current.FadeCoroutine != null)
                    StopCoroutine(current.FadeCoroutine);

                current.FadeCoroutine = StartCoroutine(current.FadeRoutine(AnimationDirection.Down));
            }
        }

        current = startMenuStages[stage];

        if (current != null)
        {
            current.Enter();

            if (current.FadeCoroutine != null)
                StopCoroutine(current.FadeCoroutine);

            if (current.canvasGroup != null)
                current.FadeCoroutine = StartCoroutine(current.FadeRoutine(AnimationDirection.Up));
        }
    }
}

[System.Serializable]
public class StartMenuStage
{
    public CinemachineVirtualCamera virtualCamera;
    public CanvasGroup canvasGroup;
    public Coroutine FadeCoroutine;
    public bool DoFadeOut;

    public void Enter()
    {
        virtualCamera.Priority = 20;
    }

    public void Exit()
    {
        virtualCamera.Priority = 0;
    }

    public bool ShouldFadeOut()
    {
        return canvasGroup != null && DoFadeOut;
    }

    public IEnumerator FadeRoutine(AnimationDirection animationDirection)
    {
        canvasGroup.enabled = true;
        float current = animationDirection == AnimationDirection.Up ? 0 : 1;
        while (animationDirection == AnimationDirection.Up ? (current < 1) : (current > 0))
        {
            current += Time.deltaTime * animationDirection.ToFloat();
            canvasGroup.alpha = current;
            yield return null;
        }

        canvasGroup.interactable = animationDirection != AnimationDirection.Down;
    }
}
