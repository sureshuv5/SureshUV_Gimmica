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

    private bool isOpen = false;
    private bool isClosed = false;

    void Start()
    {
        interactionButton.onClick.AddListener(HandleButtonClick);

        uiObject.SetActive(false);

        interactionButtonCanvasGroup.alpha = 0;
        interactionButtonCanvasGroup.interactable = false;
        interactionButtonCanvasGroup.blocksRaycasts = false;

        if (interactionButtonText != null)
        {
            interactionButtonText.text = "Tap to Open";
        }

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
        interactionButtonCanvasGroup.interactable = true; // Re-enable interaction
        interactionButtonCanvasGroup.blocksRaycasts = true; // Enable raycasting

        // Reset the interaction button in case any other issues are causing the problem
        ResetButtonInteractionState();
    }

    void HandleButtonClick()
    {
        Debug.Log("I am Clickable!");
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
        yield return new WaitForSeconds(delay);
        uiObject.SetActive(true);
    }

    IEnumerator CloseSequence()
    {
        uiObject.SetActive(false);
        giftBoxAnimator.SetBool("close", true);
        giftBoxAnimator.SetBool("open", false);
        isOpen = false;

        // Immediately hide the interaction button when the close animation starts
        interactionButtonCanvasGroup.alpha = 0;
        interactionButtonCanvasGroup.interactable = false;
        interactionButtonCanvasGroup.blocksRaycasts = false;

        yield return new WaitForSeconds(delay);

        // Wait for the loop animation to start again
        yield return new WaitUntil(
            () => giftBoxAnimator.GetCurrentAnimatorStateInfo(0).IsName("Loop")
        );

        // Fade in the interaction button and make it interactable
        StartCoroutine(FadeInInteractionButton());
    }

    void ResetButtonInteractionState()
    {
        // This function ensures the button state is refreshed and doesn't get stuck in a non-interactive state.
        interactionButtonCanvasGroup.interactable = true;
        interactionButtonCanvasGroup.blocksRaycasts = true;
    }
}
