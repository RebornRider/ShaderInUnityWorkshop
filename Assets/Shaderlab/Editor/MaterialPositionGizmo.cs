using System;
using UnityEditor;
using UnityEngine;

public class MaterialPositionGizmo : ExtendedMaterialPropertyGizmo
{
    private readonly Space space = Space.Disabled;

    private enum Space
    {
        Disabled,
        Local,
        World,
    }
    public MaterialPositionGizmo(string space)
    {
        object parsedComparisionFunction = Enum.Parse(typeof(Space), space, true);
        if (Enum.IsDefined(typeof(Space), parsedComparisionFunction))
        {
            this.space = (Space)parsedComparisionFunction;
        }
        else
        {
            Debug.LogError("MaterialPositionGizmo space is not valid on porperty:  " + Prop.name);
        }
    }

    public override void OnSceneGUI()
    {
        if (space == Space.Disabled || ExtendedDrawer is MaterialVector3Drawer == false)
        {
            return;
        }

        var matrix = Handles.matrix;

        if (space == Space.Local)
        {
            Handles.matrix = Selection.activeTransform.localToWorldMatrix;
            Handles.DrawLine(Prop.vectorValue, Vector3.zero);
        }
        else
        {
            Handles.DrawLine(Prop.vectorValue, Selection.activeTransform.position);
        }




        if (space == Space.World)
        {
            Handles.matrix = matrix;
        }

        var style = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 12,
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0),
            border = new RectOffset(1, 1, 1, 1),
            normal =
                    {
                        textColor = Color.black
                    }
        };
        Handles.Label(Prop.vectorValue, Prop.displayName, style);
        Prop.vectorValue = Handles.PositionHandle(Prop.vectorValue, Quaternion.identity);
        Handles.matrix = matrix;
    }

    public override void ExtendedApply(ExtendedMaterialEditor.MaterialPropertyInfo materialPropertyInfo)
    {
        base.ExtendedApply(materialPropertyInfo);

        if (ExtendedDrawer is MaterialVector3Drawer == false)
        {
            Debug.LogWarning("MaterialPositionGizmo used on non Vector3 property: " + Prop.name);
        }
    }
}