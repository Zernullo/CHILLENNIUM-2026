using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour {
    public List<ItemData> itemPool; 
    public Transform[] slots;    
    public GameObject itemPrefab;      

    [Header("Choice UI Setup")]
    public GameObject choicePanel;      
    public Transform choiceContainer;   
    public GameObject choiceCardPrefab;

    void Update() {
        if(Input.GetKeyDown(KeyCode.P)) {
            OpenChoiceMenu();
        }
    }

    public void OpenChoiceMenu() {
        choicePanel.SetActive(true);
        Time.timeScale = 0;

        foreach (Transform child in choiceContainer) Destroy(child.gameObject);

        for (int i = 0; i < 3; i++) {
            ItemData data = itemPool[Random.Range(0, itemPool.Count)];
            
            GameObject cardObj = Instantiate(choiceCardPrefab, choiceContainer);
            ChoiceCard cardScript = cardObj.GetComponent<ChoiceCard>();
            
            cardScript.Setup(data, () => {
                AddItemToInventory(data);
                CloseChoiceMenu();
            });
        }
    }

    void AddItemToInventory(ItemData data) {
        Transform targetSlot = null;
        foreach (Transform slot in slots) {
            if (slot.childCount == 0) {
                targetSlot = slot;
                break;
            }
        }
        if (targetSlot == null) {
            if (slots[0].childCount > 0) {
                Destroy(slots[0].GetChild(0).gameObject);
            }
            for (int i = 1; i < slots.Length; i++) {
                if (slots[i].childCount > 0) {
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
    
    public void CloseChoiceMenu() {
        choicePanel.SetActive(false);
        Time.timeScale = 1;
    }
}
