using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsUse;
    public Image UIImage;
    public int Count = 0;
    public Text CountText;
    public Text NameText;
    public Item ItemType;

    public GameObject SelectedSlotUI;

    private Color normalColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    private Color hoveredColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public void DestroySlot()
    {
        UIImage.sprite = null;
        Count = 0;
        CountText.text = string.Empty;
        NameText.text = string.Empty;
        ItemType = null;
        IsUse = false;
    }

    public void AddItemToSlot(Item item, int PlusCount)
    {
        ItemType = item;
        Count = PlusCount;
        UIImage.sprite = ItemType.icon;
        CountText.text = Count + "";
        NameText.text = ItemType.itemName;
        IsUse = true;
    }
    public void SelectedSlot(bool select)
    {
        SelectedSlotUI.SetActive(select);
    }
    public bool IsUseSlot()
    {
        return IsUse;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        // Убрали вызов OnSlotClicked отсюда, так как это событие Inventory
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Inventory.Instance.HandleRightClick(this);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            Inventory.Instance.HandleLeftClick(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIImage.color = hoveredColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIImage.color = normalColor;
    }


}
