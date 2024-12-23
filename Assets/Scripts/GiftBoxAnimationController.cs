using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftBoxAnimationController : MonoBehaviour
{
    public Animator giftBoxAnimator;
    public GameObject uiObject;
    public Button interactionButton;
    public CanvasGroup interactionButtonCanvasGroup;
    public TextMeshProUGUI interactionButtonText;
    public float delay = 1.5f;
    public float overlayDelay = 1.5f;

    private bool isOpen = false;
    private bool isClosed = false;
    public float fadeinDuration = 1.0f;

    void Start()
    {
        interactionButton.onClick.AddListener(HandleButtonClick);

        uiObject.SetActive(false);

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
        if (interactionButtonText != null)
        {
            interactionButtonText.text = "Tap to Open";
        }

        float elapsedTime = 0f;
        interactionButton.gameObject.SetActive(true);
        while (elapsedTime < fadeinDuration)
        {
            elapsedTime += Time.deltaTime;
            interactionButtonCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeinDuration);
            yield return null;
        }

        interactionButtonCanvasGroup.alpha = 1;
        interactionButtonCanvasGroup.interactable = true;
        interactionButtonCanvasGroup.blocksRaycasts = true;

        ResetButtonInteractionState();
    }

    void HandleButtonClick()
    {
        if (!isOpen)
        {
            isClosed = false;
            isOpen = true;
            giftBoxAnimator.SetBool("close", false);
            giftBoxAnimator.SetBool("open", true);

            StartCoroutine(OpenSequence());
        }
        else if (!isClosed)
        {
            isClosed = true;
            StartCoroutine(CloseSequence());
        }
    }

    IEnumerator OpenSequence()
    {
        yield return new WaitForSeconds(overlayDelay);
        uiObject.SetActive(true);
        interactionButton.gameObject.SetActive(false);
    }

    IEnumerator CloseSequence()
    {
        uiObject.SetActive(false);
        giftBoxAnimator.SetBool("close", true);
        giftBoxAnimator.SetBool("open", false);
        isOpen = false;

        interactionButtonCanvasGroup.alpha = 0;
        interactionButtonCanvasGroup.interactable = false;
        interactionButtonCanvasGroup.blocksRaycasts = false;

        yield return new WaitForSeconds(delay);

        yield return new WaitUntil(
            () => giftBoxAnimator.GetCurrentAnimatorStateInfo(0).IsName("Loop")
        );

        StartCoroutine(FadeInInteractionButton());
    }

    void ResetButtonInteractionState()
    {
        interactionButtonCanvasGroup.interactable = true;
        interactionButtonCanvasGroup.blocksRaycasts = true;
    }
}
