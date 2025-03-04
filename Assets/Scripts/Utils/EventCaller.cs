using Sirenix.OdinInspector;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace PxlSpace.Fox
{
	[AddComponentMenu("Pxl Space/Event Caller")]
	public class EventCaller : MonoBehaviour
	{
		public enum EventTrigger { OnEnable, OnDisable, OnAwake, Manual }
		public enum EventType { Void, Bool, Float, GameObject }

		[SerializeField] private EventTrigger eventTrigger = EventTrigger.Manual;
		[SerializeField] private EventType eventType = EventType.Void;
		[SerializeField] private float delay = 0f;
		[ShowIf(nameof(eventType), EventType.Void)] public UnityEvent OnVoidEventCalled;
		[ShowIf(nameof(eventType), EventType.Bool), SerializeField] private bool invert;
		[ShowIf(nameof(eventType), EventType.Bool), SerializeField] private bool defaultBool;
		[ShowIf(nameof(eventType), EventType.Bool)] public UnityEvent<bool> OnBoolEventCalled;
		[ShowIf(nameof(eventType), EventType.Float), SerializeField] private float defaultFloat;
		[ShowIf(nameof(eventType), EventType.Float)] public UnityEvent<float> OnFloatEventCalled;
		[ShowIf(nameof(eventType), EventType.GameObject), SerializeField] private GameObject defaultGameObject;
		[ShowIf(nameof(eventType), EventType.GameObject)] public UnityEvent<GameObject> OnGameObjectEventCalled;

		private void OnEnable()
		{
			if (eventTrigger != EventTrigger.OnEnable) return;
			CallDefault();
		}

		private void OnDisable()
		{
			if (eventTrigger != EventTrigger.OnDisable) return;
			CallDefault();
		}

		private void Awake()
		{
			if (eventTrigger != EventTrigger.OnAwake) return;
			CallDefault();
		}

		private void CallDefault()
		{
			switch (eventType)
			{
				case EventType.Void:
					CallEvent();
					break;
				case EventType.Bool:
					CallEventBool(defaultBool);
					break;
				case EventType.Float:
					CallEventFloat(defaultFloat);
					break;
				case EventType.GameObject:
					CallEventGameObject(defaultGameObject);
					break;
				default:
					break;
			}
		}

		public void SetDelay(float _delay)
		{
			delay = _delay;
		}

		public void CancelAll()
		{
			StopAllCoroutines();
		}

		[FoldoutGroup("Buttons"), Button]
		public void CallEvent()
		{
			if (delay > 0)
				StartCoroutine(CallEventVoid());
			else
				OnVoidEventCalled?.Invoke();
		}
		private IEnumerator CallEventVoid()
		{
			yield return new WaitForSeconds(delay);
			OnVoidEventCalled?.Invoke();
		}

		[FoldoutGroup("Buttons"), Button]
		public void CallEvent(bool _value)
		{
			if (delay > 0)
				StartCoroutine(CallEventBool(_value));
			else
				OnBoolEventCalled?.Invoke(invert ? !_value : _value);
		}
		private IEnumerator CallEventBool(bool _value)
		{
			yield return new WaitForSeconds(delay);
			OnBoolEventCalled?.Invoke(invert ? !_value : _value);
		}

		[FoldoutGroup("Buttons"), Button]
		public void CallEvent(float _value)
		{
			if (delay > 0)
				StartCoroutine(CallEventFloat(_value));
			else
				OnFloatEventCalled?.Invoke(_value);
		}
		private IEnumerator CallEventFloat(float _value)
		{
			yield return new WaitForSeconds(delay);
			OnFloatEventCalled?.Invoke(_value);
		}

		[FoldoutGroup("Buttons"), Button]
		public void CallEvent(GameObject _value)
		{
			if (delay > 0)
				StartCoroutine(CallEventGameObject(_value));
			else
				OnGameObjectEventCalled?.Invoke(_value);
		}
		private IEnumerator CallEventGameObject(GameObject _value)
		{
			yield return new WaitForSeconds(delay);
			OnGameObjectEventCalled?.Invoke(_value);
		}
	}
}