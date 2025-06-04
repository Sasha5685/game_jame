using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour
{
    public float interactionDistance = 3f;
    public GameObject interactionText;
    public string interactKey = "E";
    public string interactMessage = "Press (E) to open";

    private Camera mainCamera;
    private bool isLookingAt = false;

    private void Start()
    {
        mainCamera = Camera.main;
        if (interactionText != null)
        {
            interactionText.SetActive(false);
        }
    }

    private void Update()
    {
        if (mainCamera == null) return;

        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
        isLookingAt = IsPlayerLookingAtObject();

        if (distance <= interactionDistance && isLookingAt)
        {
            if (interactionText != null)
            {
                interactionText.SetActive(true);
                interactionText.GetComponent<TextMeshPro>().text = interactMessage;
                interactionText.transform.position = transform.position + Vector3.up * 1.5f;
                interactionText.transform.LookAt(mainCamera.transform);
                interactionText.transform.Rotate(0, 180, 0);
            }
        }
        else
        {
            if (interactionText != null)
            {
                interactionText.SetActive(false);
            }
        }
    }

    private bool IsPlayerLookingAtObject()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            return hit.collider.gameObject == gameObject;
        }
        return false;
    }
}