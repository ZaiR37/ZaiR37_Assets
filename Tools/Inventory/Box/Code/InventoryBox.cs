namespace ZaiR37.InventoryBox
{
    using UnityEngine;

    public class InventoryBox : MonoBehaviour
    {
        [SerializeField] private InventoryItem currentItem;
        [SerializeField] private Transform slotImage;
        [SerializeField] private InventoryType boxType = InventoryType.Etc;

        private void Start()
        {
            if (currentItem == null) return;
            SetItem(currentItem);
        }

        public void SetItem(InventoryItem newItem)
        {
            if (newItem == null)
            {
                Debug.LogError("Item is null!");
                return;
            }

            currentItem = newItem;
            currentItem.transform.SetParent(transform);
        }

        public InventoryType GetItemType() => GetItemType();

        public InventoryType GetBoxType() => boxType;
        public void RemoveItem() => currentItem = null;
        public bool IsEmpty() => currentItem == null;
    }

}