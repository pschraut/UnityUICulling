// UI Culling for Unity. Copyright (c) 2022 Peter Schraut (www.console-dev.de). See LICENSE.md
// https://github.com/pschraut/UnityUICulling.git
#pragma warning disable IDE1006 // Naming Styles
using UnityEngine;

namespace Oddworm.Framework
{
    [DefaultExecutionOrder(1000)] // Run after default Components, because these might modify the position
    public class UICullingBehaviour : UnityEngine.EventSystems.UIBehaviour
    {
        [Tooltip("When the 'Rect' is inside the 'Viewport', the 'Rect' is considered visible.")]
        [SerializeField] RectTransform m_Rect;

        [Tooltip("When the 'Rect' is inside the 'Viewport', the 'Rect' is considered visible.")]
        [SerializeField] RectTransform m_Viewport;

        [Tooltip("Specifies how to communicate with Unity's OnBecameVisible and OnBecameInvisible magic-method implementations.\n\nSend:\nAll Components on this GameObject receive the message.\n\nBroadcast:\nAll Components on this GameObject and all its children receive the message. This option is more expensive than 'Send'.\n\nNone:\nNo message is being sent.")]
        [SerializeField] MessageMode m_MessageMode = MessageMode.Send;

        [Space]
        [Tooltip("The event is raised when the 'Rect' became visible or invisible. 'Message Mode' doesn't affect this event.")]
        [SerializeField] BoolEvent m_OnVisibleChanged;

        [Tooltip("The event is raised when the 'Rect' became visible. 'Message Mode' doesn't affect this event.")]
        [SerializeField] VoidEvent m_OnBecameVisible;

        [Tooltip("The event is raised when the 'Rect' became invisible. 'Message Mode' doesn't affect this event.")]
        [SerializeField] VoidEvent m_OnBecameInvisible;

        public RectTransform rect
        {
            get => m_Rect;
            set => m_Rect = value;
        }

        public RectTransform viewport
        {
            get => m_Viewport;
            set => m_Viewport = value;
        }

        public MessageMode messageMode
        {
            get => m_MessageMode;
            set => m_MessageMode = value;
        }

        /// <summary>
        /// Gets whether the rect is inside the viewport, thus visible.
        /// </summary>
        public bool isVisible
        {
            get => m_IsVisible > 0;
        }

        /// <summary>
        /// The event is raised when the rect became visible or invisible.
        /// The bool argument specifies whether the rect became visible or invisible.
        /// The <see cref="messageMode"/> doesn't affect this event.
        /// </summary>
        public BoolEvent onVisibleChanged
        {
            get
            {
                if (m_OnVisibleChanged == null)
                    m_OnVisibleChanged = new BoolEvent();
                return m_OnVisibleChanged;
            }
            set => m_OnVisibleChanged = value;
        }

        /// <summary>
        /// The event is raised when the rect became visible.
        /// The <see cref="messageMode"/> doesn't affect this event.
        /// </summary>
        public VoidEvent onBecameVisible
        {
            get
            {
                if (m_OnBecameVisible == null)
                    m_OnBecameVisible = new VoidEvent();
                return m_OnBecameVisible;
            }
            set => m_OnBecameVisible = value;
        }

        /// <summary>
        /// The event is raised when the rect became invisible.
        /// The <see cref="messageMode"/> doesn't affect this event.
        /// </summary>
        public VoidEvent onBecameInvisible
        {
            get
            {
                if (m_OnBecameInvisible == null)
                    m_OnBecameInvisible = new VoidEvent();
                return m_OnBecameInvisible;
            }
            set => m_OnBecameInvisible = value;
        }

        public enum MessageMode
        {
            None = 0,
            /// <summary>Use <see cref="GameObject.SendMessage"/>.</summary>
            Send = 1,
            /// <summary>Use <see cref="GameObject.BroadcastMessage"/>.</summary>
            Broadcast = 2
        }

        [System.Serializable]
        public class BoolEvent : UnityEngine.Events.UnityEvent<bool> { }

        [System.Serializable]
        public class VoidEvent : UnityEngine.Events.UnityEvent { }

        // static cached to avoid garbage alloctions
        static readonly Vector3[] s_CornerCache = new Vector3[4];

        // The MonoBehavior magic-method names from Unity
        // https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnBecameVisible.html
        static readonly string[] s_MagicMethodName = new[]
        {
            "OnBecameInvisible",
            "OnBecameVisible"
        };

        // We use an integer instead of boolean, so we can differentiate
        // between visible, invisible and uninitialized (when component is first enabled).
        int m_IsVisible = -1;

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateIsVisible();
        }

        protected virtual void LateUpdate()
        {
            UpdateIsVisible();
        }

        void UpdateIsVisible()
        {
            var visible = CalculateVisibility() ? 1 : 0;
            if (visible == m_IsVisible)
                return; // visible didn't change

            m_IsVisible = visible;
            RaiseEvents();
        }

        protected void RaiseEvents()
        {
            // Raise the Unity magic-message event
            var methodName = s_MagicMethodName[isVisible ? 1 : 0];
            switch (m_MessageMode)
            {
                case MessageMode.Send:
                    gameObject.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
                    break;

                case MessageMode.Broadcast:
                    gameObject.BroadcastMessage(methodName, SendMessageOptions.DontRequireReceiver);
                    break;

                default:
                    break;
            }

            // Raise Unity events
            if (m_OnVisibleChanged != null)
                m_OnVisibleChanged.Invoke(isVisible);

            if (m_OnBecameVisible != null && isVisible)
                m_OnBecameVisible.Invoke();

            if (m_OnBecameInvisible != null && !isVisible)
                m_OnBecameInvisible.Invoke();
        }

        /// <summary>
        /// Calculates whether the rect is inside or overlaps the viewport.
        /// </summary>
        /// <returns>true when visible, false otherwise.</returns>
        protected bool CalculateVisibility()
        {
            var rect = GetWorldRect(m_Rect);
            var viewport = GetWorldRect(m_Viewport);

            var visible = rect.Overlaps(viewport);
            return visible;
        }

        /// <summary>
        /// Gets the rectangle of the specified <paramref name="rectTransform"/> in world-space.
        /// </summary>
        protected Rect GetWorldRect(RectTransform rectTransform)
        {
            rectTransform.GetWorldCorners(s_CornerCache);

            // s_CornerCache[0] = bottom left
            // s_CornerCache[1] = top left
            // s_CornerCache[2] = top right
            // s_CornerCache[3] = bottom right
            return new Rect(
                s_CornerCache[0].x,
                s_CornerCache[0].y,
                s_CornerCache[2].x - s_CornerCache[0].x,
                s_CornerCache[2].y - s_CornerCache[0].y);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_Rect == null)
            {
                m_Rect = GetComponent<RectTransform>();

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }
    }
}
