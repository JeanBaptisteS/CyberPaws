using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class MaterialSwitcher : MonoBehaviour
	{
		public enum SwitchType { Material, MainTexture, EmissionTexture, EmissionOnOff }

		[SerializeField] private Renderer rend;
		[SerializeField] private SwitchType type;
		[SerializeField, ShowIf(nameof(type), SwitchType.Material)]
		private Material matA;
		[SerializeField, ShowIf(nameof(type), SwitchType.Material)]
		private Material matB;

		[SerializeField, ShowIf(nameof(IsTexture))]
		private Texture2D texA;
		[SerializeField, ShowIf(nameof(IsTexture))]
		private Texture2D texB;

		[SerializeField, ShowIf(nameof(type), SwitchType.EmissionOnOff)]
		private bool invert;

		[OnValueChanged(nameof(SetState))]
		[SerializeField] private bool isOptionA;

		[SerializeField] private bool includeAllMaterials = false;
		[SerializeField, HideIf(nameof(includeAllMaterials))]
		private int materialIndex = 0;

		private bool IsTexture()
		{
			return type == SwitchType.MainTexture || type == SwitchType.EmissionTexture;
		}

		private void Start()
		{
			SetState(isOptionA);
		}

		public void SetState(bool _state)
		{
			isOptionA = _state;
			Material[] mats = rend.materials;
			switch (type)
			{
				case SwitchType.Material:
					mats[materialIndex] = isOptionA ? matA : matB;
					break;
				case SwitchType.MainTexture:
					Texture2D mainTex = isOptionA ? texA : texB;
					mats[includeAllMaterials ? 0 : materialIndex].mainTexture = mainTex;
					if (includeAllMaterials)
					{
						for (int i = 1; i < mats.Length; i++)
						{
							mats[i].mainTexture = mainTex;
						}
					}
					break;
				case SwitchType.EmissionTexture:
					Texture2D emiTex = isOptionA ? texA : texB;
					mats[includeAllMaterials ? 0 : materialIndex].SetTexture(Constants.SHADER_EMISSION_MAP, emiTex);
					if (includeAllMaterials)
					{
						for (int i = 1; i < mats.Length; i++)
						{
							mats[i].SetTexture(Constants.SHADER_EMISSION_MAP, emiTex);
						}
					}
					break;
				case SwitchType.EmissionOnOff:
					if (isOptionA != invert)
						EnableEmission(mats);
					else
						DisableEmission(mats);
					break;
				default:
					break;
			}
			rend.materials = mats;
		}

		private void EnableEmission(Material[] mats)
		{
			mats[includeAllMaterials ? 0 : materialIndex].EnableKeyword(Constants.SHADER_EMISSION_TOGGLE);
			if (!includeAllMaterials) return;
			for (int i = 1; i < mats.Length; i++)
			{
				mats[i].EnableKeyword(Constants.SHADER_EMISSION_TOGGLE);
			}
		}

		private void DisableEmission(Material[] mats)
		{
			mats[includeAllMaterials ? 0 : materialIndex].DisableKeyword(Constants.SHADER_EMISSION_TOGGLE);
			if (!includeAllMaterials) return;
			for (int i = 1; i < mats.Length; i++)
			{
				mats[i].DisableKeyword(Constants.SHADER_EMISSION_TOGGLE);
			}
		}
	}
}