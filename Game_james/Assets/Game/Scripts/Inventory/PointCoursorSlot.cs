using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointCoursorSlot : MonoBehaviour
{
    public GameObject PointCoursor;

    public bool IsUse;
    public Image UIImage;
    public int Count = 0;
    public Text CountText;
    public Text NameText;
    public Item ItemType;

    public GameObject SelectedSlotUI;
    public void DestroySlot()
    {
        UIImage.sprite = null;
        Count = 0;
        CountText.text = string.Empty;
        NameText.text = string.Empty;
        ItemType = null;
        IsUse = false;

        PointCoursor.SetActive(false);
    }

    public void AddItemToSlot(Item item, int PlusCount)
    {
        ItemType = item;
        Count = PlusCount;
        UIImage.sprite = ItemType.icon;
        CountText.text = Count + "";
        NameText.text = ItemType.itemName;
        IsUse = true;

        PointCoursor.SetActive(true);
    }

    public bool IsUseSlot()
    {
        return IsUse;
    }

    private void Update()
    {
        if (IsUseSlot())
        {
            transform.position = Input.mousePosition;
        }
    }
}
