using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GiftBoxAnimationController : MonoBehaviour
{
    public Animator giftBoxAnimator;
    public GameObject uiObject;
    public Button interactionButton;
    public CanvasGroup interactionButtonCanvasGroup;
    public TextMeshProUGUI interactionButtonText; // Use TextMeshProUGUI instead of Text
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

        // Set the initial text of the interaction button using TextMeshPro
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
        yield return new WaitForSeconds(delay);
        uiObject.SetActive(true);
    }

    IEnumerator CloseSequence()
    {
        yield return new WaitForSeconds(delay);
        giftBoxAnimator.SetBool("open", false);
        giftBoxAnimator.SetBool("close", true);
        yield return new WaitForSeconds(delay);
        interactionButton.gameObject.SetActive(false);
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
