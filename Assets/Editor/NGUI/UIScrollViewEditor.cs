//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIScrollView))]
public class UIScrollViewEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		NGUIEditorTools.SetLabelWidth(130f);

		GUILayout.Space(3f);
		serializedObject.Update();

		SerializedProperty sppv = serializedObject.FindProperty("contentPivot");
		UIWidget.Pivot before = (UIWidget.Pivot)sppv.intValue;

		NGUIEditorTools.DrawProperty("Content Origin", sppv, false);

		SerializedProperty movement = NGUIEditorTools.DrawProperty("Movement", serializedObject, "movement");
        SerializedProperty customMovementX = null;
        SerializedProperty customMovementY = null;

		if (((UIScrollView.Movement)movement.intValue) == UIScrollView.Movement.Custom)
		{
			NGUIEditorTools.SetLabelWidth(20f);

			GUILayout.BeginHorizontal();
			GUILayout.Space(114f);
            customMovementX = NGUIEditorTools.DrawProperty("X", serializedObject, "customMovement.x", GUILayout.MinWidth(20f));
            customMovementY = NGUIEditorTools.DrawProperty("Y", serializedObject, "customMovement.y", GUILayout.MinWidth(20f));
			GUILayout.EndHorizontal();
		}

		NGUIEditorTools.SetLabelWidth(130f);

		NGUIEditorTools.DrawProperty("Drag Effect", serializedObject, "dragEffect");
		NGUIEditorTools.DrawProperty("Scroll Wheel Factor", serializedObject, "scrollWheelFactor");
		NGUIEditorTools.DrawProperty("Momentum Amount", serializedObject, "momentumAmount");

        SerializedProperty restrict = NGUIEditorTools.DrawProperty("Restrict Within Panel", serializedObject, "restrictWithinPanel");
        if (restrict.boolValue == true)
        {
            if (CanMoveHorizontally((UIScrollView.Movement)movement.intValue, customMovementX != null ? customMovementX.floatValue : 0f))
            {
                NGUIEditorTools.DrawProperty("Constrain To Left", serializedObject, "constrainToLeft");
            }
            if (CanMoveVertically((UIScrollView.Movement)movement.intValue, customMovementY != null ? customMovementY.floatValue : 0f))
            {
                NGUIEditorTools.DrawProperty("Constrain To Top", serializedObject, "constrainToTop");
            }
        }
		NGUIEditorTools.DrawProperty("Cancel Drag If Fits", serializedObject, "disableDragIfFits");
		NGUIEditorTools.DrawProperty("Smooth Drag Start", serializedObject, "smoothDragStart");
		NGUIEditorTools.DrawProperty("IOS Drag Emulation", serializedObject, "iOSDragEmulation");

		NGUIEditorTools.SetLabelWidth(100f);

		if (NGUIEditorTools.DrawHeader("Scroll Bars"))
		{
			NGUIEditorTools.BeginContents();
			NGUIEditorTools.DrawProperty("Horizontal", serializedObject, "horizontalScrollBar");
			NGUIEditorTools.DrawProperty("Vertical", serializedObject, "verticalScrollBar");
			NGUIEditorTools.DrawProperty("Show Condition", serializedObject, "showScrollBars");
			NGUIEditorTools.EndContents();
		}
		serializedObject.ApplyModifiedProperties();

		if (before != (UIWidget.Pivot)sppv.intValue)
		{
			(target as UIScrollView).ResetPosition();
		}
	}
    /// <summary>
    /// Whether the scroll view can move horizontally.
    /// </summary>

    public bool CanMoveHorizontally(UIScrollView.Movement movement, float customMovementX)
    {
        return movement == UIScrollView.Movement.Horizontal ||
            movement == UIScrollView.Movement.Unrestricted ||
            (movement == UIScrollView.Movement.Custom && customMovementX != 0f);
    }

    /// <summary>
    /// Whether the scroll view can move vertically.
    /// </summary>

    public bool CanMoveVertically(UIScrollView.Movement movement, float customMovementY)
    {
        return movement == UIScrollView.Movement.Vertical ||
            movement == UIScrollView.Movement.Unrestricted ||
            (movement == UIScrollView.Movement.Custom && customMovementY != 0f);
    }
}
