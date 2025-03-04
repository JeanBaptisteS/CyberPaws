using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace PxlSpace.Fox
{
	public class Teleporter : MonoBehaviour
	{
		[SerializeField, OnValueChanged(nameof(ValidateState))] private bool onState = true;
		[SerializeField, OnValueChanged(nameof(ValidateState))] private Teleporter linkedTeleporter;
		[SerializeField] private Transform spawn;
		[SerializeField] private Renderer warpSphere;
		[SerializeField] private ReflectionProbe reflectionProbe;
		[SerializeField] private Animation ring;
		[SerializeField] private EnergyInteractor energyManager;

		private MagneticArm foxMagArm;
		public bool warpSphereVisible;
		private Camera cam;

		private void Start()
		{
			cam = Camera.main;
			foxMagArm = MalbersAnimations.Controller.MAnimal.MainAnimal.GetComponentInChildren<MagneticArm>();
			StartCoroutine(SetWarpTexture());
			SetState(onState, false);
		}

		private void ValidateState()
		{
			if (linkedTeleporter == null) return;
			if (linkedTeleporter.linkedTeleporter != this)
				linkedTeleporter.linkedTeleporter = this;
			if (linkedTeleporter.onState != onState)
				linkedTeleporter.onState = onState;
		}

		private IEnumerator SetWarpTexture()
		{
			reflectionProbe.RenderProbe();
			linkedTeleporter.warpSphere.transform.rotation.ToAngleAxis(out float angle, out Vector3 axis);
			warpSphere.material.SetVector(Constants.PORTAL_ROTATION_AXIS, axis);
			warpSphere.material.SetFloat(Constants.PORTAL_ANGLE, angle);
			yield return new WaitForSeconds(0.1f);
			warpSphere.material.SetTexture(Constants.CUBEMAP, linkedTeleporter.reflectionProbe.realtimeTexture);
		}

		private IEnumerator ReflectionProbeUpdate()
		{
			while (onState)
			{
				warpSphereVisible = cam.IsInViewFrustrum(warpSphere);
				if (linkedTeleporter.warpSphereVisible)
					reflectionProbe.RenderProbe();
				yield return new WaitForSecondsRealtime(0.1f);
			}
		}

		public void SetState(bool _state)
		{
			SetState(_state, true);
		}

		private void SetState(bool _state, bool _tellOther)
		{
			onState = _state;
			if (_state)
				ring.Play();
			else
				ring.Stop();
			warpSphere.gameObject.SetActive(onState);
			energyManager?.SetChargeState(_state);
			if (_tellOther)
				linkedTeleporter?.SetState(_state, false);
			if (onState)
				StartCoroutine(ReflectionProbeUpdate());
		}

		public void Teleport(GameObject _go)
		{
			if (!onState) return;
			if (linkedTeleporter == null) return;
			if (_go != MalbersAnimations.Controller.MAnimal.MainAnimal.gameObject)
				foxMagArm.TryDrop(_go);
			linkedTeleporter.Arrive(_go);
		}

		private void Arrive(GameObject _go)
		{
			var animal = _go.GetComponent<MalbersAnimations.Controller.MAnimal>();
			if (animal != null)
				animal.TeleportRot(spawn);
			else
				_go.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
		}
	}
}