#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Ateo.StateManagement;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Ateo.CodeGeneration
{
	[Serializable]
	public class AddState
	{
		[HorizontalGroup(LabelWidth = 50)]
		public string Name = string.Empty;

		[HorizontalGroup, ValueDropdown("GetStates")]
		public IState Parent;

#if STATEMACHINE
		private IEnumerable<IState> GetStates() => StateHelper.GetStates();
#else
        private IEnumerable<IState> GetStates() => new List<IState>();
#endif

	}

	[ExecuteInEditMode]
	public class CodeGeneratorStatesBehaviour : SerializedMonoBehaviour
	{
#if STATEMACHINE
		[ReadOnly, ListDrawerSettings(ShowIndexLabels = false, DraggableItems = false, HideAddButton = true, HideRemoveButton = true),
		 ShowIf("HasStates"), LabelText("All States")]
		public List<string> States = new List<string>();

		[ValidateInput("ValidateForCharactersAndDuplicates", "State already exists OR string contains special characters"), LabelText("Add"),
		 NonSerialized, OdinSerialize, HideReferenceObjectPicker, ListDrawerSettings(CustomAddFunction = "AddState")]
		public List<AddState> AddStates = new List<AddState>();

		[LabelText("Remove")]
		public List<StatesEnum> RemoveStates = new List<StatesEnum>();

		private void AddState()
		{
			AddStates.Add(new AddState());
		}

		private void OnEnable()
		{
			States = StateHelper.GetStateNames();
		}

		[Button(ButtonSizes.Large), ButtonGroup]
		public void RunCodeGeneration()
		{
			if (!ValidateAll(AddStates))
			{
				if (EditorUtility.DisplayDialog("Add States have errors",
					"State already exists OR string contains special characters. Fix entries with errors?", "Yes", "No"))
				{
					RemoveNewStatesWithErrors();
				}

				return;
			}

			List<string> removeStates = new List<string>();
			foreach (StatesEnum state in RemoveStates)
			{
				if (state != StatesEnum.None)
				{
					removeStates.Add(state.ToString());
				}
			}

			CodeGeneratorStates.GenerateStates(AddStates, removeStates);
			AddStates.Clear();
			RemoveStates.Clear();
		}

		[ButtonGroup]
		public void Refresh() => StateHelper.UpdateDictionaries();

		private bool HasStates()
		{
			return States != null && States.Count > 0;
		}

		private bool ValidateForCharactersAndDuplicates(List<AddState> list)
		{
			Refresh();

			foreach (AddState addState in list)
			{
				foreach (char ch in addState.Name)
				{
					if (!char.IsLetterOrDigit(ch)) return false;
				}

				if (StateHelper.GetStatesEnum(addState.Name) != StatesEnum.None)
					return false;
			}

			return true;
		}

		private bool ValidateAll(List<AddState> list)
		{
			if (ValidateForCharactersAndDuplicates(list))
			{
				foreach (AddState addState in list)
				{
					if (string.IsNullOrEmpty(addState.Name))
						return false;

					if (string.IsNullOrWhiteSpace(addState.Name))
						return false;
				}

				return true;
			}

			return false;
		}

		private void RemoveNewStatesWithErrors()
		{
			foreach (AddState addState in AddStates)
			{
				addState.Name = Regex.Replace(addState.Name, "[^0-9a-zA-Z]+", "", RegexOptions.Compiled);
			}

			List<string> states = StateHelper.GetStateNames();

			for (int i = AddStates.Count - 1; i >= 0; i--)
			{
				AddState addState = AddStates[i];

				if (string.IsNullOrEmpty(addState.Name) || string.IsNullOrWhiteSpace(addState.Name))
				{
					AddStates.RemoveAt(i);
				}
				else
				{
					if (addState.Name.Length == 1)
					{
						addState.Name = AddStates[i].Name = char.ToUpper(addState.Name[0]).ToString();
					}
					else
					{
						addState.Name = AddStates[i].Name = char.ToUpper(addState.Name[0]) + addState.Name.Substring(1);
					}
				}

				if (states.Contains(addState.Name))
				{
					AddStates.RemoveAt(i);
				}
			}
		}
#endif
	}
}
#endif