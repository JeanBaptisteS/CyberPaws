using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace PxlSpace.Fox
{
	public class MagneticArm : MonoBehaviour
	{
		#region Variables
		[FoldoutGroup("References"), SerializeField]
		private Transform objectHolder;
		[FoldoutGroup("References"), SerializeField]
		private Transform aimer;
		[FoldoutGroup("References"), SerializeField]
		private LineRenderer aimLine;
		[FoldoutGroup("References"), SerializeField]
		private bool requiresChargeToThrow = true;
		[FoldoutGroup("References"), ShowIf(nameof(requiresChargeToThrow)), SerializeField]
		private EnergyManager energyManager;
		[FoldoutGroup("References"), SerializeField]
		private UnityEngine.UI.Slider chargeSlider;
		[FoldoutGroup("References"), ShowIf(nameof(chargeSlider)), SerializeField]
		private MalbersAnimations.FadeInOutGraphic sliderFade;
		[FoldoutGroup("References"), SerializeField]
		private MagHand handAnim;
		[FoldoutGroup("References"), SerializeField]
		private FMODUnity.StudioEventEmitter grabSound;
		[FoldoutGroup("References"), SerializeField]
		private FMODUnity.StudioEventEmitter throwSound;

		[FoldoutGroup("Values"), SerializeField]
		private LayerMask grabbableLayers;
		[FoldoutGroup("Values"), SerializeField]
		private float maxGrabDistance = 10f;
		[FoldoutGroup("Values"), SerializeField]
		private float sphereCastRadius = 0.25f;
		[FoldoutGroup("Values"), SerializeField]
		private float attractionSpeed = 5f;
		[FoldoutGroup("Values"), SerializeField]
		private Vector3 throwForce;
		private float chargePercent = 0f;
		private Vector3 localChargedForce;
		private Vector3 worldChargedForce;
		[FoldoutGroup("Values"), SerializeField]
		private float chargeSpeed = 1f;
		[FoldoutGroup("Values"), SerializeField]
		private float maxChargeTime = 3f;
		[FoldoutGroup("Values"), SerializeField]
		private float maxChargeMultiplier = 2f;

		[FoldoutGroup("Visuals"), SerializeField]
		private Transform armPivot;
		[FoldoutGroup("Visuals"), SerializeField]
		private float armAimSpeed = 0.1f;
		[FoldoutGroup("Visuals"), SerializeField]
		private Vector3 eulerOffset;
		[FoldoutGroup("Visuals"), SerializeField]
		private Vector3 restingEulers;
		private Quaternion restingRotation;

		[FoldoutGroup("Aim Line"), SerializeField, Range(10, 100)]
		private int linePoints = 25;
		[FoldoutGroup("Aim Line"), SerializeField, Range(0.01f, 0.25f)]
		private float timeBetweenPoints = 0.1f;
		[FoldoutGroup("Aim Line"), SerializeField]
		private LayerMask objectCollisionMask;
		private bool isCharging = false;

		private Rigidbody grabbedRB;
		#endregion

		#region Input
		public void GrabInput()
		{
			DisplayCharge(false);
			if (grabbedRB == null)
			{
				Grab();
			}
			else
			{
				Drop();
			}
		}

		public void PressCharge()
		{
			if (grabbedRB == null) return;

			if (isCharging)
			{
				Throw();
			}
			else
			{
				SetupCharge();
			}
			isCharging = !isCharging;
		}

		public void ChargeUp()
		{
			if (!isCharging) return;
			chargePercent = Mathf.Clamp01(chargePercent + chargeSpeed * Time.deltaTime);
			CalculateCharge();
		}

		public void ChargeDown()
		{
			if (!isCharging) return;
			chargePercent = Mathf.Clamp01(chargePercent - chargeSpeed * Time.deltaTime);
			CalculateCharge();
		}

		public void TryDrop(GameObject obj)
		{
			if (grabbedRB == null) return;
			if (grabbedRB.gameObject == obj)
				Drop();
		}
		#endregion

		#region Unity
		private void OnEnable()
		{
			restingRotation = Quaternion.Euler(restingEulers);
			DisplayCharge(false);
		}

		private void FixedUpdate()
		{
			if (grabbedRB != null)
			{
				grabbedRB.MovePosition(Vector3.Lerp(grabbedRB.position, objectHolder.position, Time.fixedDeltaTime * attractionSpeed));
				armPivot.LookAt(grabbedRB.position);
				armPivot.Rotate(eulerOffset, Space.Self);
			}
			else
			{
				armPivot.localRotation = Quaternion.Lerp(armPivot.localRotation, restingRotation, armAimSpeed);
			}
			if (isCharging)
			{
				worldChargedForce = aimer.TransformDirection(localChargedForce);
				DrawProjection();
			}
		}
		#endregion

		#region PrivateFunctions
		private void Grab()
		{
			if (!enabled || !gameObject.activeSelf) return;
			RaycastHit hit;
			Ray ray = new Ray(aimer.position, aimer.forward);
			if (Physics.SphereCast(ray, sphereCastRadius, out hit, maxGrabDistance, grabbableLayers))
			{
				GrabObject(hit.collider.GetComponentInParent<Rigidbody>());
			}
		}

		private void GrabObject(Rigidbody obj)
		{
			grabbedRB = obj;
			if (grabbedRB == null) return;
			grabbedRB.useGravity = false;
			handAnim.Open();
			grabSound.SetParameter(Constants.IS_LOOPING_PARAMETER, 1);
			grabSound.Play();
		}

		private void Drop()
		{
			if (grabbedRB == null) return;
			grabbedRB.useGravity = true;
			grabbedRB = null;
			handAnim.Close();
			grabSound.SetParameter(Constants.IS_LOOPING_PARAMETER, 0);
		}

		private void SetupCharge()
		{
			if (chargeSlider != null) chargeSlider.value = 0f;
			DisplayCharge(true);
			chargePercent = 0f;
			localChargedForce = throwForce;
		}

		private void DisplayCharge(bool _state)
		{
			aimLine.gameObject.SetActive(_state);
			if (sliderFade != null)
			{
				if (_state)
					sliderFade.Fade_In();
				else
					sliderFade.Fade_Out();
			}
		}

		private void CalculateCharge()
		{
			chargeSlider.value = chargePercent;
			float chargeMultiplier = Mathf.Lerp(1, maxChargeMultiplier, chargePercent);
			localChargedForce = new(throwForce.x, throwForce.y, throwForce.z * chargeMultiplier);
		}

		private void DrawProjection()
		{
			if (grabbedRB == null)
			{
				DisplayCharge(false);
				isCharging = false;
				return;
			}
			aimLine.positionCount = Mathf.CeilToInt(linePoints / timeBetweenPoints) + 1;
			Vector3 startPos = grabbedRB.transform.position;
			Vector3 startVel = worldChargedForce;
			int i = 0;
			aimLine.SetPosition(i, startPos);
			for (float time = 0; time < linePoints; time += timeBetweenPoints)
			{
				i++;
				Vector3 point = startPos + time * startVel;
				point.y = startPos.y + startVel.y * time + (Physics.gravity.y / 2f * time * time);
				aimLine.SetPosition(i, point);

				Vector3 lastPosition = aimLine.GetPosition(i - 1);
				if (Physics.Raycast(lastPosition, (point - lastPosition).normalized, out RaycastHit hit, (point - lastPosition).magnitude, objectCollisionMask))
				{
					aimLine.SetPosition(i, hit.point);
					aimLine.positionCount = i + 1;
					break;
				}
			}
		}

		public void Throw()
		{
			DisplayCharge(false);
			if (grabbedRB == null) return;
			if (!enabled || !gameObject.activeSelf || (requiresChargeToThrow && !energyManager.HasEnergy))
			{
				Drop();
				return;
			}

			var obj = grabbedRB;
			Drop();
			obj.AddForce(worldChargedForce, ForceMode.VelocityChange);
			if (requiresChargeToThrow)
				energyManager.ConsumeEnergy();
			throwSound.Play();
		}
		#endregion
	}
}