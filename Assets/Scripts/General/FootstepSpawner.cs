using FMODUnity;
using MalbersAnimations;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class FootstepSpawner : MonoBehaviour
	{
		[SerializeField] private LayerMask groundLayer;
		[SerializeField] private float groundCheckLength = 0.05f;
		[SerializeField] private ParticleSystem footstepParticlesPrefab;
		[SerializeField] private ParticleSystem dustParticles;
		[SerializeField] private int dustEmission;
		[SerializeField] private StudioEventEmitter soundEmitter;
		[SerializeField] private float offset;

		private void OnTriggerEnter(Collider other)
		{
			if (other.isTrigger) return;
			if (!groundLayer.ContainsLayer(other.gameObject.layer)) return;
			SpawnFootstep(other);
		}

		private void SpawnFootstep(Collider surface)
		{
			soundEmitter.Play();

			Ray ray = new(transform.position, Vector3.down);
			if (Physics.Raycast(ray, out RaycastHit hit, groundCheckLength))
			{
				Vector3 spawnPos = transform.position + Vector3.up * offset;
				Quaternion spawnRot = Quaternion.FromToRotation(-transform.forward, hit.normal) * transform.rotation;

				SpawnDust(spawnPos, spawnRot);

				SpawnFootstep(spawnPos, spawnRot, hit.transform);
			}
		}

#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position, Vector3.down * groundCheckLength);
		}
#endif

		private void SpawnDust(Vector3 pos, Quaternion rot)
		{
			dustParticles.transform.SetPositionAndRotation(pos, rot);
			dustParticles.transform.Rotate(-90, 0, 0);
			dustParticles.Emit(dustEmission);
		}

		private void SpawnFootstep(Vector3 pos, Quaternion rot, Transform surface)
		{
			ParticleSystem.EmitParams emitParams = new()
			{
				position = Vector3.zero,
				rotation3D = rot.eulerAngles
			};

			ParticleSystem footstep = Instantiate(footstepParticlesPrefab);
			Transform parent = footstep.transform.SetParentScaleFixer(surface, pos);

			footstep.Emit(emitParams, 1);
			this.Delay_Action(() => footstep.isPlaying, () =>
			{
				if (parent != null)
					Destroy(parent.gameObject);
				else
					Destroy(footstep.gameObject);
			});
		}
	}
}