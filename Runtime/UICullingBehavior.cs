using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Oddworm.Framework
{
	[DefaultExecutionOrder(1000)] // Run after default Components, because these might modify the position
	public class UICullingBehavior : MonoBehaviour
	{
		[SerializeField] RectTransform m_Rect;
		[SerializeField] RectTransform m_Viewport;
		[SerializeField] MessageMode m_MessageMode = MessageMode.SendMessage;

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

		public bool isVisible
        {
			get => m_IsVisible > 0;
		}

		public System.Action<UICullingBehavior> onVisibleChanged;

		public enum MessageMode
		{
			None = 0,
			SendMessage = 1,
			BroadcastMessage = 2
		}

		// static cached to avoid garbage alloctions
		static Vector3[] s_CornerCache = new Vector3[4];

		// The MonoBehavior magic-method names from Unity
		// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnBecameVisible.html
		static string[] s_MagicMethodName = new[]
		{
			"OnBecameInvisible",
			"OnBecameVisible"
		};

		// We use an int instead of bool, so we can differentiate
		// between visible, invisible and uninitialized which is the case
		// when the component gets enabled the first time.
		int m_IsVisible = -1;

		protected virtual void OnEnable()
		{
			UpdateIsVisible();
		}

		protected virtual void OnDisable()
		{
		}

		protected virtual void LateUpdate()
        {
			UpdateIsVisible();
		}

		protected virtual void OnValidate()
		{
			if (m_Rect == null)
			{
				m_Rect = GetComponent<RectTransform>();

#if UNITY_EDITOR
				if (!Application.isPlaying)
					UnityEditor.EditorUtility.SetDirty(this);
#endif
			}
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
				case MessageMode.SendMessage:
					gameObject.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
					break;

				case MessageMode.BroadcastMessage:
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
	}
}
