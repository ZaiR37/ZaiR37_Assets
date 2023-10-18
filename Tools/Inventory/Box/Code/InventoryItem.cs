namespace ZaiR37.InventoryBox
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private string itemName;
        [SerializeField] private InventoryType type;
        [SerializeField] private bool canStack;
        [SerializeField] private int quantity = 1;
        [SerializeField] private int maxAmount = 99;
        [SerializeField] private TextMeshProUGUI quantityText;
        private Transform formerParent;
        private Image itemImage;

        private void Start()
        {
            itemImage = transform.GetComponent<Image>();
            gameObject.name = itemName;

            if (type == InventoryType.Consumable) canStack = true;
            else quantity = 1;

            CheckQuantityText();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            formerParent = transform.parent;
            transform.SetParent(InventoryManager.Instance.GetDragParent());

            itemImage.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            itemImage.raycastTarget = true;
            InventoryBox targetBox = eventData.pointerEnter?.GetComponent<InventoryBox>();
            InventoryItem targetItem = eventData.pointerEnter?.GetComponent<InventoryItem>();

            if (IsValidTypeEmptyBox(targetBox))
            {
                formerParent.GetComponent<InventoryBox>().RemoveItem();
                targetBox.SetItem(this);
                return;
            }

            if (CanMerge(targetItem))
            {
                targetItem.AddQuantity(quantity);
                Destroy(gameObject);
            }

            ResetPosition();
        }

        private bool CanMerge(InventoryItem targetItem)
        {
            return targetItem != null
                && targetItem.GetName() == itemName
                && targetItem.GetItemType() == type
                && canStack
                && targetItem.GetQuantity() < maxAmount
                && quantity < maxAmount;
        }

        private bool IsValidTypeEmptyBox(InventoryBox targetBox)
        {
            return targetBox != null && targetBox.IsEmpty() && IsValidBoxType(targetBox);
        }

        private bool IsValidBoxType(InventoryBox targetBox)
        {
            if (targetBox.GetBoxType() == InventoryType.Etc)
            {
                return true;
            }

            return targetBox.GetBoxType() == type;
        }


        private void CheckQuantityText()
        {
            if (quantity == 1) quantityText.text = "";
            else quantityText.text = quantity.ToString();
        }

        private void ResetPosition() => transform.SetParent(formerParent);
        public int GetQuantity() => quantity;
        public string GetName() => itemName;
        public InventoryType GetItemType() => type;

        public void AddQuantity(int number)
        {
            quantity += number;
            CheckQuantityText();
        }
    }

}