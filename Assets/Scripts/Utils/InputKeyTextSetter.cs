using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

namespace PxlSpace.Fox
{
	public class InputKeyTextSetter : MonoBehaviour
	{
		public TextMeshProUGUI keyText;
		public InputActionAsset inputActions;
		public int mapIdx = 0;
		public int actionIdx = 0;

		private void Start()
		{
			UpdateText();
		}

		public void UpdateText()
		{
			keyText.text = inputActions.actionMaps[mapIdx].actions[actionIdx].bindings[0].ToDisplayString();
		}


#if UNITY_EDITOR
		[CustomEditor(typeof(InputKeyTextSetter))]
		public class InputKeyTextSetterEditor : Editor
		{
			private InputKeyTextSetter textSetter;

			private SerializedProperty keyText, inputActions, mapIdx, actionIdx;

			private string[] mapNames;
			private string[] actionNames;

			private void OnEnable()
			{
				textSetter = (InputKeyTextSetter)target;

				keyText = serializedObject.FindProperty("keyText");
				inputActions = serializedObject.FindProperty("inputActions");
				mapIdx = serializedObject.FindProperty("mapIdx");
				actionIdx = serializedObject.FindProperty("actionIdx");
			}

			public override void OnInspectorGUI()
			{
				serializedObject.Update();

				EditorGUILayout.PropertyField(keyText);

				using (var inputChangeScope = new EditorGUI.ChangeCheckScope())
				{
					var oldInputActions = inputActions.objectReferenceValue;
					EditorGUILayout.PropertyField(inputActions);

					if (inputChangeScope.changed)
					{
						bool changed = oldInputActions != inputActions.objectReferenceValue;

						if (changed)
						{
							mapIdx.intValue = 0;
							actionIdx.intValue = 0;
						}

						serializedObject.ApplyModifiedProperties();
						GetMapNames();
					}
				}

				if (inputActions.objectReferenceValue == null) return;
				if (mapNames == null) GetMapNames();

				using (var mapChangeScope = new EditorGUI.ChangeCheckScope())
				{
					int oldMap = mapIdx.intValue;
					mapIdx.intValue = EditorGUILayout.Popup(new GUIContent("Action Map"), mapIdx.intValue, mapNames);

					if (mapChangeScope.changed)
					{
						bool changed = oldMap != mapIdx.intValue;

						if (changed)
						{
							actionIdx.intValue = 0;
						}

						serializedObject.ApplyModifiedProperties();
						GetActionNames();
					}
				}

				if (actionNames == null) GetActionNames();

				using (var actionChangedScope = new EditorGUI.ChangeCheckScope())
				{
					int oldAction = actionIdx.intValue;
					actionIdx.intValue = EditorGUILayout.Popup(new GUIContent("Action"), actionIdx.intValue, actionNames);
					serializedObject.ApplyModifiedProperties();

					if (actionChangedScope.changed)
					{
						bool changed = oldAction != actionIdx.intValue;

						if (changed)
							textSetter.UpdateText();
					}
				}
			}

			private void GetMapNames()
			{
				mapNames = new string[textSetter.inputActions.actionMaps.Count];
				for (int i = 0; i < textSetter.inputActions.actionMaps.Count; i++)
				{
					mapNames[i] = textSetter.inputActions.actionMaps[i].name;
				}
			}

			private void GetActionNames()
			{
				InputActionMap map = textSetter.inputActions.actionMaps[textSetter.mapIdx];
				actionNames = new string[map.actions.Count];
				for (int i = 0; i < map.actions.Count; i++)
				{
					actionNames[i] = map.actions[i].name;
				}
				if (textSetter.actionIdx >= actionNames.Length)
					textSetter.actionIdx = actionNames.Length - 1;
				else if (textSetter.actionIdx < 0)
					textSetter.actionIdx = 0;
			}
		}
#endif
	}
}