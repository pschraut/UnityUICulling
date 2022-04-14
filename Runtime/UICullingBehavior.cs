// UI Culling for Unity. Copyright (c) 2022 Peter Schraut (www.console-dev.de). See LICENSE.md
// https://github.com/pschraut/UnityUICulling.git
#pragma warning disable IDE1006 // Naming Styles
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Oddworm.Framework
{
	[DefaultExecutionOrder(1000)] // Run after default Components, because these might modify the position
	public class UICullingBehavior : UnityEngine.EventSystems.UIBehaviour
    {
		[Tooltip("When the 'Rect' is inside the 'Viewport', the 'Rect' is considered visible.")]
		[SerializeField] RectTransform m_Rect;

		[Tooltip("When the 'Rect' is inside the 'Viewport', the 'Rect' is considered visible.")]
		[SerializeField] RectTransform m_Viewport;

		[Tooltip("Controls how to communicate with Unity's OnBecameVisible and OnBecameInvisible magic-methods.\n\nSend:\nAll Components on this GameObject receive the message.\n\nBroadcast:\nAll Components on this GameObject and all its children receive the message. This option is more expensive than 'Send'.\n\nNone:\nNo message is being sent to OnBecameVisible and OnBecameInvisible.")]
		[SerializeField] MessageMode m_MessageMode = MessageMode.Broadcast;

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
		/// The event is triggered when the visibility changed.
		/// The event is triggered even when <see cref="messageMode"/> is set to <see cref="MessageMode.None"/>.
		/// </summary>
		public System.Action<UICullingBehavior> onVisibleChanged;

		public enum MessageMode
		{
			None = 0,
			/// <summary>Use <see cref="GameObject.SendMessage"/>.</summary>
			Send = 1,
			/// <summary>Use <see cref="GameObject.BroadcastMessage"/>.</summary>
			Broadcast = 2
		}

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
				return; // visbility didn't change

			m_IsVisible = visible;
			RaiseEvent();
		}

		void RaiseEvent()
        {
			// Raise the C# event
			onVisibleChanged?.Invoke(this);

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
