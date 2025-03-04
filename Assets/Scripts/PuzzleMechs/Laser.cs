using MalbersAnimations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using FMODUnity;

namespace PxlSpace.Fox
{
	public class Laser : MonoBehaviour
	{
#if UNITY_EDITOR
		[OnValueChanged(nameof(SetWorking))]
#endif
		[SerializeField] private bool working = true;
		[SerializeField] private Transform beamStart;
		[SerializeField] private LineRenderer beamLine;
		[SerializeField] private Transform beamEnd;
		[SerializeField] private LayerMask hitMask;
		[SerializeField] private StatID damageStat;
		[SerializeField] private StudioEventEmitter sound;

		private InteractableBase interactableTarget;

		private void Start()
		{
			SetWorking(working);
		}

		public void SetWorking(bool _state)
		{
			working = _state;
			beamStart.gameObject.SetActive(working);
			beamLine.gameObject.SetActive(working);
			beamEnd.gameObject.SetActive(working);
			if (working)
				sound.Play();
			else
				sound.Stop();
		}

		private void FixedUpdate()
		{
			if (!working) return;
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity, hitMask))
			{
				SetLaserPosition(hit.distance);

				TriggerRoot trigger = hit.collider.GetComponentInParent<TriggerRoot>();
				if (trigger)
				{
					HitTrigger(trigger.gameObject);
				}
				InteractableBase interactable = hit.collider.GetComponentInParent<InteractableBase>();
				HitInteractable(interactable);
			}
			else if (interactableTarget != null)
			{
				interactableTarget.InteractExit();
				interactableTarget = null;
			}
		}

		private void SetLaserPosition(float _distance)
		{
			beamStart.position = transform.position;
			Vector3 endLocalPos = Vector3.forward * _distance;
			beamLine.SetPosition(1, endLocalPos);
			beamEnd.localPosition = endLocalPos;
		}

		private void HitTrigger(GameObject _trigger)
		{
			var damageable = _trigger.GetComponent<MDamageable>();
			if (damageable)
			{
				damageable.ReceiveDamage(damageStat, 9999f, transform.forward, false, gameObject);
			}
		}

		private void HitInteractable(InteractableBase _interactable)
		{
			if (interactableTarget != _interactable)
			{
				if (interactableTarget != null)
					interactableTarget.InteractExit();
				if (_interactable != null)
					_interactable.InteractEnter();
			}
			interactableTarget = _interactable;
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (!working) return;
			if (Application.isPlaying) return;
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity, hitMask))
			{
				SetLaserPosition(hit.distance);
			}
		}
#endif
	}
}