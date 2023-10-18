namespace ZaiR37.InventoryBox
{
    using UnityEngine;

    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [SerializeField] private Transform selectedItem;
        [SerializeField] private Transform inventoryContainer;
        [SerializeField] private Transform playerContainer;
        [SerializeField] private Transform dragParent;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There's more than one InventoryManager! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public Transform GetDragParent()=> dragParent;
    }

}