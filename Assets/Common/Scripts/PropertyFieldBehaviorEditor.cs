using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace APlusOrFail
{
    // http://wiki.unity3d.com/index.php?title=Expose_properties_in_inspector
    [CustomEditor(typeof(PropertyFieldBehavior), true)]
    public class PropertyFieldBehaviorEditor : Editor
    {
        private static readonly GUILayoutOption[] emptyLayoutOptions = Array.Empty<GUILayoutOption>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginVertical(emptyLayoutOptions);
                
            foreach (PropertyInfo info in target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Attribute.IsDefined(info, typeof(EditorPropertyFieldAttribute)) && info.CanRead && info.CanWrite)
                {
                    Type type = info.PropertyType;
                    string nickName = ObjectNames.NicifyVariableName(info.Name);

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginHorizontal(emptyLayoutOptions);

                    if (type == typeof(AnimationCurve))
                    {
                        info.SetValue(target, EditorGUILayout.CurveField(nickName, (AnimationCurve)info.GetValue(target), emptyLayoutOptions));
                    }
                    else if (type == typeof(bool))
                    {
                        info.SetValue(target, EditorGUILayout.Toggle(nickName, (bool)info.GetValue(target), emptyLayoutOptions));
                    }
                    else if (type == typeof(Bounds))
                    {
                        info.SetValue(target, EditorGUILayout.BoundsField(nickName, (Bounds)info.GetValue(target), emptyLayoutOptions));
                    }
                    else if (type == typeof(BoundsInt))
                    {
                        info.SetValue(target, EditorGUILayout.BoundsIntField(nickName, (BoundsInt)info.GetValue(target), emptyLayoutOptions));
                    }
                    else if (type == typeof(Color))
                    {
                        info.SetValue(target, EditorGUILayout.ColorField(nickName, (Color)info.GetValue(target), emptyLayoutOptions));
                    }
                    else if (type.IsEnum)
                    {
                        if (Attribute.IsDefined(type, typeof(FlagsAttribute)))
                        {
                            info.SetValue(target, EditorGUILayout.EnumFlagsField(nickName, (Enum)info.GetValue(target), emptyLayoutOptions));
                        }
                        else
                        {
                            info.SetValue(target, EditorGUILayout.EnumPopup(nickName, (Enum)info.GetValue(target), emptyLayoutOptions));
                        }
                    }
                    else if (type == typeof(float))
                    {
                        info.SetValue(target, EditorGUILayout.FloatField(nickName, (float)info.GetValue(target), emptyLayoutOptions));
                    }
                    else if (type == typeof(int))
                    {
                        info.SetValue(target, EditorGUILayout.IntField(nickName, (int)info.GetValue(target), emptyLayoutOptions));
                    }
                    else if (type == typeof(LayerMask))
                    {
                        // TODO: check
                        string[] layerNames = new string[32];
                        for (int i = 0; i < 32; ++i)
                        {
                            layerNames[i] = LayerMask.LayerToName(i);
                            if (layerNames[i].Length == 0) layerNames[i] = null;
                        }
                        info.SetValue(target, EditorGUILayout.MaskField(nickName, (LayerMask)info.GetValue(target), layerNames, emptyLayoutOptions));
                    }
                    else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                    {
                        info.SetValue(target, EditorGUILayout.ObjectField(nickName, (UnityEngine.Object)info.GetValue(target), type, true, emptyLayoutOptions));
                    }
                    else if (type == typeof(Rect))
                    {
                        info.SetValue(target, EditorGUILayout.RectField(nickName, (Rect)info.GetValue(target), emptyLayoutOptions));
                    }
                    else if (type == typeof(RectInt))
                    {
                        info.SetValue(target, EditorGUILayout.RectIntField(nickName, (RectInt)info.GetValue(target), emptyLayoutOptions));
                    }
                    else if (type == typeof(string))
                    {
                        info.SetValue(target, EditorGUILayout.TextField(nickName, (string)info.GetValue(target), emptyLayoutOptions));
                    }
                    else if (type == typeof(Vector2))
                    {
                        info.SetValue(target, EditorGUILayout.Vector2Field(nickName, (Vector2)info.GetValue(target), emptyLayoutOptions));
                    }
                    else if (type == typeof(Vector2Int))
                    {
                        info.SetValue(target, EditorGUILayout.Vector2IntField(nickName, (Vector2Int)info.GetValue(target), emptyLayoutOptions));
                    }
                    else if (type == typeof(Vector3))
                    {
                        info.SetValue(target, EditorGUILayout.Vector3Field(nickName, (Vector3)info.GetValue(target), emptyLayoutOptions));
                    }
                    else if (type == typeof(Vector3Int))
                    {
                        info.SetValue(target, EditorGUILayout.Vector3IntField(nickName, (Vector3Int)info.GetValue(target), emptyLayoutOptions));
                    }
                    else if (type == typeof(Vector4))
                    {
                        info.SetValue(target, EditorGUILayout.Vector4Field(nickName, (Vector4)info.GetValue(target), emptyLayoutOptions));
                    }

                    EditorGUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
                    {
                        Undo.RecordObject(target, $"Changed {nickName}");
                        EditorUtility.SetDirty(target);
                        EditorSceneManager.MarkSceneDirty(((Component)target).gameObject.scene);
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
}
