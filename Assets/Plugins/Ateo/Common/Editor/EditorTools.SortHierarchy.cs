using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Ateo.Common.Editor
{
	public static partial class EditorTools
	{
		[MenuItem("Tools/Editor Tools/Sort Hierarchy #h")]
		public static void SortHierarchy()
		{
			List<Transform> objsToSort = new List<Transform>();
			int minIndex = int.MaxValue;

			// Ensure there are selected transforms
			if (Selection.transforms != null && Selection.transforms.Length > 1)
			{
				foreach (Transform t in Selection.transforms)
				{
					if (t == null) continue; // Check if the transform is not null

					minIndex = Mathf.Min(minIndex, t.GetSiblingIndex());
					objsToSort.Add(t);
				}
			}
			else if (Selection.activeTransform != null && Selection.activeTransform.childCount > 0)
			{
				// Sorting all children, so minIndex is 0.
				minIndex = 0;
				Transform rootParent = Selection.activeTransform;

				for (int i = 0; i < rootParent.childCount; ++i)
				{
					Transform child = rootParent.GetChild(i);
					if (child == null) continue; // Check if the child is not null

					objsToSort.Add(child);
				}
			}

			// Check if there are objects to sort
			if (objsToSort.Count == 0) return;

			objsToSort.Sort(new NaturalNameComparer());

			// Sort the objects and handle undo operations
			for (int i = 0; i < objsToSort.Count; ++i)
			{
				if (objsToSort[i] == null || objsToSort[i].transform == null) continue; // Check if the transform is not null

				Undo.SetTransformParent(objsToSort[i].transform, objsToSort[i].transform.parent, "Sort Hierarchy");
				Undo.RegisterFullObjectHierarchyUndo(objsToSort[i].transform, "Sort Hierarchy");
				objsToSort[i].SetSiblingIndex(minIndex + i);
			}
		}

		[MenuItem("Tools/Editor Tools/Sort Hierarchy #h", true)]
		public static bool SortHierarchyValidate()
		{
			return Selection.transforms != null &&
			       (Selection.transforms.Length > 1 || (Selection.transforms.Length == 1 && Selection.activeTransform != null &&
			                                            Selection.activeTransform.childCount > 1));
		}
	}
	
	public class NaturalNameComparer : IComparer<Transform>
	{
		private static readonly Regex Re = new Regex(@"(?<=\D)(?=\d)|(?<=\d)(?=\D)");

		public int Compare(Transform obj1, Transform obj2)
		{
			if (obj1 == null || obj2 == null) return 0;

			string x = obj1.name.ToLower();
			string y = obj2.name.ToLower();

			if (string.Compare(x, 0, y, 0, Mathf.Min(x.Length, y.Length)) == 0)
			{
				if (x.Length == y.Length)
					return 0;
				return x.Length < y.Length ? -1 : 1;
			}

			string[] a = Re.Split(x);
			string[] b = Re.Split(y);

			int i = 0;

			while (true)
			{
				int r = PartCompare(a[i], b[i]);
				if (r != 0)
					return r;
				++i;
			}
		}

		private static int PartCompare(string x, string y)
		{
			if (int.TryParse(x, out int a) && int.TryParse(y, out int b))
			{
				return a.CompareTo(b);
			}

			return string.Compare(x, y, StringComparison.Ordinal);
		}
	}
}