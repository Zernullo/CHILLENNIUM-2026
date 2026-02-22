using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public List<ItemData> itemPool;
    public Transform[] slots;
    public GameObject itemPrefab;

    [Header("Choice UI Setup")]
    public GameObject choicePanel;
    public Transform choiceContainer;
    public GameObject choiceCardPrefab;

    void Update()
    {
        if (Keyboard.current[Key.P].wasPressedThisFrame)
            OpenChoiceMenu();
    }

    public void OpenChoiceMenu()
    {
        choicePanel.SetActive(true);
        Time.timeScale = 0;

        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        // Use a copy of the pool so we can pick unique items
        List<ItemData> pool = new List<ItemData>(itemPool);

        for (int i = 0; i < 3 && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            ItemData localData = pool[idx];
            pool.RemoveAt(idx);

            GameObject cardObj = Instantiate(choiceCardPrefab, choiceContainer);
            ChoiceCard cardScript = cardObj.GetComponent<ChoiceCard>();
            if (cardScript == null) continue;

            cardScript.Setup(localData, () =>
            {
                AddItemToInventory(localData);
                CloseChoiceMenu();
            });
        }
    }

    void AddItemToInventory(ItemData data)
    {
        Transform targetSlot = null;

        // Find first empty slot
        foreach (Transform slot in slots)
        {
            if (slot.childCount == 0)
            {
                targetSlot = slot;
                break;
            }
        }

        // No empty slot — shift items left and use last slot
        if (targetSlot == null)
        {
            if (slots[0].childCount > 0)
                Destroy(slots[0].GetChild(0).gameObject);

            for (int i = 1; i < slots.Length; i++)
            {
                if (slots[i].childCount > 0)
                {
                    Transform item = slots[i].GetChild(0);
                    item.SetParent(slots[i - 1]);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }
            }

            targetSlot = slots[slots.Length - 1];
        }

        GameObject newItem = Instantiate(itemPrefab, targetSlot);
        newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        newItem.GetComponent<Image>().sprite = data.itemIcon;
    }

    public void CloseChoiceMenu()
    {
        choicePanel.SetActive(false);
        Time.timeScale = 1;
    }
}