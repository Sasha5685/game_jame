using UnityEngine;
using UnityEngine.UI;

public class BarrelController : MonoBehaviour
{
    public Inventory playerInventory;
    public GameObject barrelUI;
    public float interactionDistance = 3f;

    [Header("UI Elements")]
    public Image haySlotIcon;
    public Text haySlotText;
    public Image[] beerSlots = new Image[3];

    [Header("Settings")]
    public Item hayItem;
    public Item beerItem;
    public int hayRequiredForBeer = 2;

    private int currentHay = 0;
    private int[] beerCounts = new int[3];
    private bool isUIOpen = false;

    private void Update()
    {
        if (Vector3.Distance(transform.position, playerInventory.transform.position) <= interactionDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleBarrelUI();
            }
        }
        else if (isUIOpen)
        {
            CloseBarrelUI();
        }
    }

    private void ToggleBarrelUI()
    {
        isUIOpen = !isUIOpen;
        barrelUI.SetActive(isUIOpen);
        playerInventory.isInContainerUI = isUIOpen;

        Cursor.lockState = isUIOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isUIOpen;
    }

    private void CloseBarrelUI()
    {
        isUIOpen = false;
        barrelUI.SetActive(false);
        playerInventory.isInContainerUI = false;
        playerInventory.EndItemTransfer();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnHaySlotClick()
    {
        // ���� ����� ��������� ����
        if (playerInventory.itemToTransfer == hayItem)
        {
            playerInventory.TryTransferItem(null, true);
            currentHay++;
            Debug.Log($"Added hay to barrel. Total: {currentHay}");
            UpdateUI();

            if (currentHay >= hayRequiredForBeer)
            {
                CraftBeer();
            }
        }
    }

    public void OnBeerSlotClick(int slotIndex)
    {
        // ����� ������ ����� ����
        if (beerCounts[slotIndex] > 0)
        {
            if (playerInventory.AddItem(beerItem))
            {
                beerCounts[slotIndex]--;
                UpdateUI();
            }
        }
    }

    private void CraftBeer()
    {
        currentHay -= hayRequiredForBeer;

        // ������� ��������� ���� ��� ����
        for (int i = 0; i < beerCounts.Length; i++)
        {
            if (beerCounts[i] == 0)
            {
                beerCounts[i] = 1;
                Debug.Log("Crafted beer from hay!");
                UpdateUI();
                return;
            }
        }
    }

    private void UpdateUI()
    {
        // ��������� UI ����
        haySlotIcon.sprite = currentHay > 0 ? hayItem.icon : null;
        haySlotIcon.enabled = currentHay > 0;
        haySlotText.text = currentHay > 0 ? currentHay.ToString() : "";

        // ��������� UI ����
        for (int i = 0; i < beerSlots.Length; i++)
        {
            beerSlots[i].sprite = beerCounts[i] > 0 ? beerItem.icon : null;
            beerSlots[i].enabled = beerCounts[i] > 0;
        }
    }
}