using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, Busy }
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemDescription;
    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;
    [SerializeField] private PartyScreen partyScreen;

    private Action _onItemUsed;
    
    private const int ItemsInViewport = 8;

    private InventoryUIState _state;
    private List<ItemSlotUI> _slotUIList;
    private Inventory _inventory;
    private int _selecteItem = 0;
    private RectTransform _itemListRect;
    private float _itemSlotUIHeight;

    private void Awake()
    {
        _inventory = Inventory.GetInventory();
        _itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();
        _itemSlotUIHeight = itemSlotUI.GetComponent<RectTransform>().rect.height;

        _inventory.OnUpdated += UpdateItemList;
    }

    private void UpdateItemList()
    {
        // Clear the existing items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        _slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in _inventory.Slots)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);
            
            _slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    public void HandeUpdate(Action onBack, Action onItemUsed = null)
    {
        _onItemUsed = onItemUsed;
        
        if (_state == InventoryUIState.ItemSelection)
        {
            int prevSelection = _selecteItem;
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _selecteItem++;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _selecteItem--;
            }
    
            _selecteItem = Mathf.Clamp(_selecteItem, 0, _inventory.Slots.Count - 1);
    
            if (_selecteItem != prevSelection)
            {
                UpdateItemSelection();
            }
            
            if (Input.GetKeyDown(KeyCode.X))
            {
                OpenPartyScreen();
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                onBack?.Invoke();
            }
        }
        else if (_state == InventoryUIState.PartySelection)
        {
            Action onSelected = () =>
            {
                // Use item on the selected pokemon
                StartCoroutine(UseItem());

            };
            
            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };
            
            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
    }

    IEnumerator UseItem()
    {
        _state = InventoryUIState.Busy;
        
        var usedItem = _inventory.UseItem(_selecteItem, partyScreen.SelectedMember);
        if (usedItem != null)
        {
            yield return DialogManager.Instance.ShowDialogText($"The player used {usedItem.Name}");
            _onItemUsed?.Invoke();
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"It won't have any effect");
        }
        
        ClosePartyScreen();
    }
    
    void UpdateItemSelection()
    {
        for (int i = 0; i < _slotUIList.Count; i++)
        {
            if (i == _selecteItem)
            {
                _slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            }
            else
            {
                _slotUIList[i].NameText.color = Color.black;
            }
        }

        var item = _inventory.Slots[_selecteItem].Item;
        itemIcon.sprite = item.Icon;
        itemDescription.text = item.Description;

        HandleScrolling();
    }
    
    private void HandleScrolling()
    {
        if (_slotUIList.Count <= ItemsInViewport)
        {
            return;
        }
        
        float scrollPos = Mathf.Clamp(_selecteItem - ItemsInViewport / 2, 0, _selecteItem) * _itemSlotUIHeight;
        _itemListRect.localPosition = new Vector2(_itemListRect.localPosition.x, scrollPos);

        bool showUpArrow = _selecteItem > ItemsInViewport / 2;
        upArrow.gameObject.SetActive(showUpArrow);
        
        bool showDownArrow = _selecteItem + ItemsInViewport / 2 < _slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }

    void OpenPartyScreen()
    {
        _state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }
    
    void ClosePartyScreen()
    {
        _state = InventoryUIState.ItemSelection;
        partyScreen.gameObject.SetActive(false);
    }
}
