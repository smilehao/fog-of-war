//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIWrapContent), true)]
public class UIWrapContentEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(90f);
        
		string error = null;
		UIScrollView sv = null;

		if (!serializedObject.isEditingMultipleObjects)
		{
			UIWrapContent list = target as UIWrapContent;
			sv = NGUITools.FindInParents<UIScrollView>(list.gameObject);

			if (sv == null)
			{
				error = "UIWrappedList needs a Scroll View on its parent in order to work properly";
			}
			else if (sv.movement != UIScrollView.Movement.Horizontal && sv.movement != UIScrollView.Movement.Vertical)
			{
				error = "Scroll View needs to be using Horizontal or Vertical movement";
			}
		}

		serializedObject.Update();

        NGUIEditorTools.DrawProperty("Item Width", serializedObject, "itemWidth");
        NGUIEditorTools.DrawProperty("Item Height", serializedObject, "itemHeight");

        GUILayout.BeginHorizontal();
        SerializedProperty sp = NGUIEditorTools.DrawProperty("Column Limit", serializedObject, "maxPerLine", GUILayout.Width(130f));
        NGUIEditorTools.SetLabelWidth(90f);
        if (sp.intValue <= 0)
        {
            if (sv != null)
            {
                if (sv.movement == UIScrollView.Movement.Horizontal)
                {
                    GUILayout.Label("horizontal unlimited");
                }
                else
                {
                    GUILayout.Label("vertical unlimited");
                }
            }
            else
            {
                GUILayout.Label("unlimited");
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
		SerializedProperty sp1 = NGUIEditorTools.DrawProperty("Range Limit", serializedObject, "minIndex", GUILayout.Width(130f));
		NGUIEditorTools.SetLabelWidth(20f);
		SerializedProperty sp2 = NGUIEditorTools.DrawProperty("to", serializedObject, "maxIndex", GUILayout.Width(60f));
		NGUIEditorTools.SetLabelWidth(90f);
		if (sp1.intValue == sp2.intValue) GUILayout.Label("unlimited");
		GUILayout.EndHorizontal();


		NGUIEditorTools.DrawProperty("Cull Content", serializedObject, "cullContent");
        if (sv != null)
        {
            if (sv.movement == UIScrollView.Movement.Horizontal)
            {
                NGUIEditorTools.DrawProperty("Left Arrow", serializedObject, "arrow1");
                NGUIEditorTools.DrawProperty("Right Arrow", serializedObject, "arrow2");
            }
            else
            {
                NGUIEditorTools.DrawProperty("Top Arrow", serializedObject, "arrow1");
                NGUIEditorTools.DrawProperty("Bottom Arrow", serializedObject, "arrow2");
            }
        }

        NGUIEditorTools.DrawProperty("Debug", serializedObject, "isDebug");

        if (!string.IsNullOrEmpty(error))
		{
			EditorGUILayout.HelpBox(error, MessageType.Error);
			if (sv != null && GUILayout.Button("Select the Scroll View"))
				Selection.activeGameObject = sv.gameObject;
		}

		serializedObject.ApplyModifiedProperties();

		if (sp1.intValue != sp2.intValue)
		{
			if ((target as UIWrapContent).GetComponent<UICenterOnChild>() != null)
			{
				EditorGUILayout.HelpBox("Limiting indices doesn't play well with UICenterOnChild. You should either not limit the indices, or not use UICenterOnChild.", MessageType.Warning);
			}
		}
	}
}
