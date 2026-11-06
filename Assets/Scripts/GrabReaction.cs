using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabReaction : MonoBehaviour
{
    public string cupName = "coffe_cup";
    public Color selectedColor = Color.red;
    public TMP_Text statusText;

    XRGrabInteractable interactable;
    Renderer rend;
    Color originalColor;
    bool isCup;

    void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        isCup = gameObject.name == cupName.ToLower();

        if (!isCup)
        {
            rend = GetComponent<Renderer>();
            originalColor = rend.material.color;
        }
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnGrab);
        interactable.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (isCup) statusText.text = "Cup Grabbed";
        else rend.material.color = selectedColor;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        if (isCup) statusText.text = "Cup Dropped";
        else rend.material.color = originalColor;
    }
}
