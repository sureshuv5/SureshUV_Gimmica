using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GiftBoxAnimationController : MonoBehaviour
{
    public Animator giftBoxAnimator;
    public GameObject uiObject;
    public Button interactionButton;
    public Button restartButton;
    public CanvasGroup interactionButtonCanvasGroup;

    private bool isOpen = false;
    private bool isClosed = false;

    void Start()
    {
        interactionButton.onClick.AddListener(HandleButtonClick);
        restartButton.onClick.AddListener(RestartScene);

        uiObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        interactionButtonCanvasGroup.alpha = 0;
        interactionButtonCanvasGroup.interactable = false;
        interactionButtonCanvasGroup.blocksRaycasts = false;

        StartCoroutine(WaitForIntroAnimation());
    }

    IEnumerator WaitForIntroAnimation()
    {
        yield return new WaitUntil(
            () => giftBoxAnimator.GetCurrentAnimatorStateInfo(0).IsName("Loop")
        );
        StartCoroutine(FadeInInteractionButton());
    }

    IEnumerator FadeInInteractionButton()
    {
        float duration = 1.0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            interactionButtonCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            yield return null;
        }

        interactionButtonCanvasGroup.alpha = 1;
        interactionButtonCanvasGroup.interactable = true;
        interactionButtonCanvasGroup.blocksRaycasts = true;
    }

    void HandleButtonClick()
    {
        if (!isOpen)
        {
            isOpen = true;
            giftBoxAnimator.SetBool("open", true);
            StartCoroutine(OpenSequence());
        }
        else if (!isClosed)
        {
            isClosed = true;
            uiObject.SetActive(false);
            StartCoroutine(CloseSequence());
        }
    }

    IEnumerator OpenSequence()
    {
        yield return new WaitForSeconds(1.5f);
        uiObject.SetActive(true);
    }

    IEnumerator CloseSequence()
    {
        yield return new WaitForSeconds(1.5f);
        giftBoxAnimator.SetBool("open", false);
        giftBoxAnimator.SetBool("close", true);
        yield return new WaitForSeconds(1.5f);
        restartButton.gameObject.SetActive(true);
        interactionButton.gameObject.SetActive(false);
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
