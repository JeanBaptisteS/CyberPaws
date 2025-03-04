using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace PxlSpace.Fox
{
	public enum LoopType { Once, PingPong, Repeat }

	public abstract class TransformAnimator : MonoBehaviour
	{
		public enum AnimationState { Idle, Waiting, Animating }
		public enum TriggerFunction { OnStart, Manual }

		[FoldoutGroup("Animation")]
		[SerializeField] protected Transform objectToMove;
		[FoldoutGroup("Animation")]
		[SerializeField] private TriggerFunction startTrigger = TriggerFunction.OnStart;
		[FoldoutGroup("Animation")]
		[SerializeField] private LoopType loopType = LoopType.PingPong;
		[FoldoutGroup("Animation")]
		[SerializeField] private float idleWaitTime = 0f;
		[FoldoutGroup("Animation")]
		[SerializeField] private float duration = 3f;
		[FoldoutGroup("Animation")]
		[SerializeField] protected AnimationCurve curve = new(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });

		[FoldoutGroup("Events")]
		public UnityEvent OnStart = new();
		[FoldoutGroup("Events")]
		public UnityEvent OnEnd = new();

		[FoldoutGroup("Bounds")]
		[Range(0, 1)]
		[OnValueChanged(nameof(Evaluate))]
		public float previewPosition = 0f;

		private AnimationState currentState;
		private float progress;
		private bool reverse = false;
		private int direction;
		private float target;

		#region Unity
		private void Start()
		{
			currentState = AnimationState.Idle;
			Evaluate(0f);
			if (startTrigger == TriggerFunction.OnStart)
				Play(true);
		}

		private void Update()
		{
			if (currentState != AnimationState.Animating) return;

			float speed = 1f / duration;
			progress += Time.deltaTime * direction * speed;
			progress = Mathf.Clamp01(progress);
			Evaluate(progress);
			if (progress == target)
			{
				OnEnd?.Invoke();
				StartCoroutine(WaitTime(false));
			}
		}
		#endregion

		#region API
		public void PlayPause(bool _state)
		{
			if (_state)
			{
				if (currentState != AnimationState.Animating)
					PlayForward();
				else
					Resume();
			}
			else
				Pause();
		}

		[FoldoutGroup("Buttons"), Button]
		public void PlayForward()
		{
			Play(true);
		}

		[FoldoutGroup("Buttons"), Button]
		public void PlayReverse()
		{
			Play(false);
		}

		public void Play(bool _forward)
		{
			Resume();
			SetValues(_forward);
			StopAllCoroutines();
			if (currentState != AnimationState.Animating)
			{
				ResetProgress(_forward);
				StartCoroutine(WaitTime(true));
			}
		}

		[FoldoutGroup("Buttons"), Button]
		public void Pause()
		{
			enabled = false;
		}

		[FoldoutGroup("Buttons"), Button]
		public void Resume()
		{
			enabled = true;
		}
		#endregion

		private void ResetProgress(bool _forward)
		{
			progress = _forward ? 0f : 1f;
			Evaluate(progress);
		}

		private void SetValues(bool _forward)
		{
			reverse = !_forward;
			direction = _forward ? 1 : -1;
			target = _forward ? 1f : 0f;
		}

		private IEnumerator WaitTime(bool _start)
		{
			currentState = AnimationState.Waiting;
			yield return new WaitForSeconds(idleWaitTime);
			if (_start)
				AnimationStart();
			else
				AnimationEnd();
		}

		private void AnimationStart()
		{
			OnStart?.Invoke();
			currentState = AnimationState.Animating;
		}

		private void AnimationEnd()
		{
			currentState = AnimationState.Idle;
			switch (loopType)
			{
				case LoopType.PingPong:
					SetValues(reverse);
					AnimationStart();
					break;
				case LoopType.Repeat:
					Play(!reverse);
					break;
				default:
					break;
			}
		}



		public abstract void Evaluate(float _t);
	}
}