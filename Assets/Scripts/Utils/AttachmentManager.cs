using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class AttachmentManager : MonoBehaviour
	{
		public enum AttachmentType { MagArm = 0, }

		[FoldoutGroup("Visuals"), SerializeField]
		private GameObject attachmentVest;
		[FoldoutGroup("Visuals"), SerializeField]
		private AttachmentDictionary attachments = new();
		private int numAttachments = 0;

		[Button]
		public void HideVest()
		{
			attachments.ForEach(kvp => kvp.Value.SetActive(false));
			attachmentVest.SetActive(false);
			numAttachments = 0;
		}

		public void Attach(int _idx)
		{
			Attach((AttachmentType)_idx);
		}

		[Button]
		public void Attach(AttachmentType _type)
		{
			if (attachments[_type].IsActive) return;
			attachmentVest.SetActive(true);
			attachments[_type].SetActive(true);
			numAttachments++;
		}

		public void Detach(int _idx)
		{
			Detach((AttachmentType)_idx);
		}

		[Button]
		public void Detach(AttachmentType _type)
		{
			if (!attachments[_type].IsActive) return;
			attachments[_type].SetActive(false);
			numAttachments--;
			if (numAttachments <= 0)
				HideVest();
		}

		[System.Serializable]
		public class AttachmentDictionary : UnitySerializedDictionary<AttachmentType, Attachment> { }

		[System.Serializable]
		public class Attachment
		{
			public GameObject visuals;
			public GameObject system;
			public bool IsActive => system.activeSelf;

			public void SetActive(bool _state)
			{
				visuals.SetActive(_state);
				system.SetActive(_state);
			}
		}
	}
}