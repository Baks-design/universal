using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Universal.Runtime.Systems.InventoryManagement
{
    public abstract class StorageView : MonoBehaviour
    {
        [SerializeField] protected UIDocument document;
        [SerializeField] protected StyleSheet styleSheet;
        public Slot[] Slots;
        protected VisualElement root;
        protected VisualElement container;
        protected static VisualElement ghostIcon;
        static bool isDragging;
        static Slot originalSlot;

        public event Action<Slot, Slot> OnDrop = delegate { };

        void Start()
        {
            ghostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            ghostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);

            for (var i = 0; i < Slots.Length; i++)
                Slots[i].OnStartDrag += OnPointerDown;
        }

        public abstract IEnumerator InitializeView(ViewModel viewModel);

        static void OnPointerDown(Vector2 position, Slot slot)
        {
            isDragging = true;
            originalSlot = slot;

            SetGhostIconPosition(position);

            ghostIcon.style.backgroundImage = originalSlot.BaseSprite.texture;
            originalSlot.Icon.image = null;
            originalSlot.StackLabel.visible = false;

            ghostIcon.style.visibility = Visibility.Visible;
            // TODO: UI - Inventory - show stack size on ghost icon
        }

        void OnPointerMove(PointerMoveEvent evt)
        {
            if (!isDragging) return;
            SetGhostIconPosition(evt.position);
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            if (!isDragging) return;

            Slot closestSlot = null;
            var closestDistance = float.MaxValue;

            // Manually find the closest overlapping slot
            for (var i = 0; i < Slots.Length; i++)
            {
                var slot = Slots[i];
                if (slot.worldBound.Overlaps(ghostIcon.worldBound))
                {
                    var distance = Vector2.Distance(slot.worldBound.position, ghostIcon.worldBound.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestSlot = slot;
                    }
                }
            }

            if (closestSlot != null)
                OnDrop.Invoke(originalSlot, closestSlot);
            else
                originalSlot.Icon.image = originalSlot.BaseSprite.texture;

            isDragging = false;
            originalSlot = null;
            ghostIcon.style.visibility = Visibility.Hidden;
        }

        static void SetGhostIconPosition(Vector2 position)
        {
            ghostIcon.style.top = position.y - ghostIcon.layout.height / 2f;
            ghostIcon.style.left = position.x - ghostIcon.layout.width / 2f;
        }

        void OnDestroy()
        {
            for (var i = 0; i < Slots.Length; i++)
                Slots[i].OnStartDrag -= OnPointerDown;
        }
    }
}