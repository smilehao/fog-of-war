//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------
#define POLYGON_CLIP
#define SPRITE_EXTENTION
#define SPRITE_UVOFFSET

using UnityEditor;
using UnityEngine;

/// <summary>
/// Inspector class used to edit UITextures.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UIBasicSprite), true)]
public class UIBasicSpriteEditor : UIWidgetInspector
{
#if SPRITE_EXTENTION
    UIBasicSprite mBasicSprite;
    protected override void OnEnable()
    {
        base.OnEnable();
        mBasicSprite = target as UIBasicSprite;
    }
#endif
	/// <summary>
	/// Draw all the custom properties such as sprite type, flip setting, fill direction, etc.
	/// </summary>

	protected override void DrawCustomProperties ()
	{
		GUILayout.Space(6f);

		SerializedProperty sp = NGUIEditorTools.DrawProperty("Type", serializedObject, "mType", GUILayout.MinWidth(20f));

		UISprite.Type type = (UISprite.Type)sp.intValue;

		if (type == UISprite.Type.Simple)
		{
			NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");
		}
		else if (type == UISprite.Type.Tiled)
		{
			NGUIEditorTools.DrawBorderProperty("Trim", serializedObject, "mBorder");
			NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");
		}
		else if (type == UISprite.Type.Sliced)
		{
			NGUIEditorTools.DrawBorderProperty("Border", serializedObject, "mBorder");
			NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");

			EditorGUI.BeginDisabledGroup(sp.hasMultipleDifferentValues);
			{
				sp = serializedObject.FindProperty("centerType");
				bool val = (sp.intValue != (int)UISprite.AdvancedType.Invisible);

				if (val != EditorGUILayout.Toggle("Fill Center", val))
				{
					sp.intValue = val ? (int)UISprite.AdvancedType.Invisible : (int)UISprite.AdvancedType.Sliced;
				}
			}
			EditorGUI.EndDisabledGroup();
		}
		else if (type == UISprite.Type.Filled)
		{
			NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");
			NGUIEditorTools.DrawProperty("Fill Dir", serializedObject, "mFillDirection", GUILayout.MinWidth(20f));
			GUILayout.BeginHorizontal();
			GUILayout.Space(4f);
			NGUIEditorTools.DrawProperty("Fill Amount", serializedObject, "mFillAmount", GUILayout.MinWidth(20f));
			GUILayout.Space(4f);
			GUILayout.EndHorizontal();
			NGUIEditorTools.DrawProperty("Invert Fill", serializedObject, "mInvert", GUILayout.MinWidth(20f));
		}
		else if (type == UISprite.Type.Advanced)
		{
			NGUIEditorTools.DrawBorderProperty("Border", serializedObject, "mBorder");
			NGUIEditorTools.DrawProperty("  Left", serializedObject, "leftType");
			NGUIEditorTools.DrawProperty("  Right", serializedObject, "rightType");
			NGUIEditorTools.DrawProperty("  Top", serializedObject, "topType");
			NGUIEditorTools.DrawProperty("  Bottom", serializedObject, "bottomType");
			NGUIEditorTools.DrawProperty("  Center", serializedObject, "centerType");
			NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");
		}
		#if POLYGON_CLIP
        else if (type == UIBasicSprite.Type.PolygonClip)
        {
            SerializedProperty tps = serializedObject.FindProperty("mClipNodes");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(tps, new GUIContent("ClipNodes"), true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            //NGUIEditorTools.DrawProperty("ClipNodes", serializedObject, "mClipNodes",true);           
        }
#endif
#if SPRITE_EXTENTION
        else if (type == UISprite.Type.SlicedAlphaed)
        {
            NGUIEditorTools.DrawBorderProperty("Border", serializedObject, "mBorder");
            NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");

            EditorGUI.BeginDisabledGroup(sp.hasMultipleDifferentValues);
            {
                sp = serializedObject.FindProperty("centerType");
                bool val = (sp.intValue != (int)UISprite.AdvancedType.Invisible);

                if (val != EditorGUILayout.Toggle("Fill Center", val))
                {
                    sp.intValue = val ? (int)UISprite.AdvancedType.Invisible : (int)UISprite.AdvancedType.Sliced;
                }
            }
            EditorGUI.EndDisabledGroup();

            NGUIEditorTools.DrawProperty("SlicedAlphaSide", serializedObject, "mSlicedAlphaSide");
            if (mBasicSprite != null && mBasicSprite.slicedAlphaSide == false)
            {
                NGUIEditorTools.DrawProperty("Left Bottm", serializedObject, "mLeftBottomAlpha", GUILayout.MinWidth(20f));
                NGUIEditorTools.DrawProperty("Left Top", serializedObject, "mLeftTopAlpha", GUILayout.MinWidth(20f));
                NGUIEditorTools.DrawProperty("Right Top", serializedObject, "mRightTopAlpha", GUILayout.MinWidth(20f));
                NGUIEditorTools.DrawProperty("Right Bottom", serializedObject, "mRightBottomAlpha", GUILayout.MinWidth(20f));
            }
            else
            {
                NGUIEditorTools.DrawProperty("Left", serializedObject, "mSlicedLeftAlpha", GUILayout.MinWidth(20f));
                NGUIEditorTools.DrawProperty("Center", serializedObject, "mSlicedCenterAlpha", GUILayout.MinWidth(20f));
                NGUIEditorTools.DrawProperty("Right", serializedObject, "mSlicedRightAlpha", GUILayout.MinWidth(20f));
            }
        }
        else if (type == UISprite.Type.Cuted)
        {
            NGUIEditorTools.DrawProperty("Left", serializedObject, "mLeftBottomAlpha", GUILayout.MinWidth(20f));
            NGUIEditorTools.DrawProperty("Top", serializedObject, "mLeftTopAlpha", GUILayout.MinWidth(20f));
            NGUIEditorTools.DrawProperty("Right", serializedObject, "mRightTopAlpha", GUILayout.MinWidth(20f));
            NGUIEditorTools.DrawProperty("Bottom", serializedObject, "mRightBottomAlpha", GUILayout.MinWidth(20f));
        }
        else if (type == UISprite.Type.Alphaed)
        {
            NGUIEditorTools.DrawProperty("Left Bottm", serializedObject, "mLeftBottomAlpha", GUILayout.MinWidth(20f));
            NGUIEditorTools.DrawProperty("Left Top", serializedObject, "mLeftTopAlpha", GUILayout.MinWidth(20f));
            NGUIEditorTools.DrawProperty("Right Top", serializedObject, "mRightTopAlpha", GUILayout.MinWidth(20f));
            NGUIEditorTools.DrawProperty("Right Bottom", serializedObject, "mRightBottomAlpha", GUILayout.MinWidth(20f));
        }
        else if (type == UISprite.Type.Mirrored)
        {
            NGUIEditorTools.DrawProperty("Offset X", serializedObject, "mTiledOffset.x", GUILayout.MinWidth(20f));
            NGUIEditorTools.DrawProperty("Offset Y", serializedObject, "mTiledOffset.y", GUILayout.MinWidth(20f));
            NGUIEditorTools.DrawProperty("Flip", serializedObject, "mFlip");
        }
#endif
#if SPRITE_UVOFFSET
        SerializedProperty offset = NGUIEditorTools.DrawProperty("UVOffset", serializedObject, "mEnablePixelOffset", GUILayout.MinWidth(20f));
		bool enablePixelOffset = (bool)offset.boolValue;
        if (enablePixelOffset)
        {
            NGUIEditorTools.DrawProperty("Horizontal", serializedObject, "mHorizontalPixelOffset", GUILayout.MinWidth(20f));
            NGUIEditorTools.DrawProperty("Vertical", serializedObject, "mVerticalPixelOffset", GUILayout.MinWidth(20f));
        }
#endif
		//GUI.changed = false;
		//Vector4 draw = EditorGUILayout.Vector4Field("Draw Region", mWidget.drawRegion);

		//if (GUI.changed)
		//{
		//    NGUIEditorTools.RegisterUndo("Draw Region", mWidget);
		//    mWidget.drawRegion = draw;
        //}

		if (type == UIBasicSprite.Type.Simple || type == UIBasicSprite.Type.Sliced) // Gradients get too complicated for tiled and filled.
		{
			GUILayout.BeginHorizontal();
			SerializedProperty gr = NGUIEditorTools.DrawProperty("Gradient", serializedObject, "mApplyGradient", GUILayout.Width(95f));

			EditorGUI.BeginDisabledGroup(!gr.hasMultipleDifferentValues && !gr.boolValue);
			{
				NGUIEditorTools.SetLabelWidth(30f);
				serializedObject.DrawProperty("mGradientTop", "Top", GUILayout.MinWidth(40f));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				NGUIEditorTools.SetLabelWidth(50f);
				GUILayout.Space(79f);

				serializedObject.DrawProperty("mGradientBottom", "Bottom", GUILayout.MinWidth(40f));
				NGUIEditorTools.SetLabelWidth(80f);
			}
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
		}
		base.DrawCustomProperties();
	}
}
