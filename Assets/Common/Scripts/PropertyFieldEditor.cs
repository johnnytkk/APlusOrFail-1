using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace APlusOrFail
{
    // http://wiki.unity3d.com/index.php?title=Expose_properties_in_inspector
    public abstract class PropertyFieldEditor<T> : Editor where T : MonoBehaviour
    {
        private static readonly GUILayoutOption[] emptyLayoutOptions = Array.Empty<GUILayoutOption>();

        protected T instance;

        protected void OnEnable()
        {
            instance = target as T;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (instance != null)
            {
                EditorGUILayout.BeginVertical(emptyLayoutOptions);
                
                foreach (PropertyInfo info in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (Attribute.IsDefined(info, typeof(EditorPropertyFieldAttribute)) && info.CanRead && info.CanWrite)
                    {
                        Type type = info.PropertyType;
                        string nickName = ObjectNames.NicifyVariableName(info.Name);
                        
                        EditorGUILayout.BeginHorizontal(emptyLayoutOptions);

                        if (type == typeof(AnimationCurve))
                        {
                            info.SetValue(instance, EditorGUILayout.CurveField(nickName, (AnimationCurve)info.GetValue(instance), emptyLayoutOptions));
                        }
                        else if (type == typeof(bool))
                        {
                            info.SetValue(instance, EditorGUILayout.Toggle(nickName, (bool)info.GetValue(instance), emptyLayoutOptions));
                        }
                        else if (type == typeof(Bounds))
                        {
                            info.SetValue(instance, EditorGUILayout.BoundsField(nickName, (Bounds)info.GetValue(instance), emptyLayoutOptions));
                        }
                        else if (type == typeof(BoundsInt))
                        {
                            info.SetValue(instance, EditorGUILayout.BoundsIntField(nickName, (BoundsInt)info.GetValue(instance), emptyLayoutOptions));
                        }
                        else if (type == typeof(Color))
                        {
                            info.SetValue(instance, EditorGUILayout.ColorField(nickName, (Color)info.GetValue(instance), emptyLayoutOptions));
                        }
                        else if (type.IsEnum)
                        {
                            if (Attribute.IsDefined(type, typeof(FlagsAttribute)))
                            {
                                info.SetValue(instance, EditorGUILayout.EnumFlagsField(nickName, (Enum)info.GetValue(instance), emptyLayoutOptions));
                            }
                            else
                            {
                                info.SetValue(instance, EditorGUILayout.EnumPopup(nickName, (Enum)info.GetValue(instance), emptyLayoutOptions));
                            }
                        }
                        else if (type == typeof(float))
                        {
                            info.SetValue(instance, EditorGUILayout.FloatField(nickName, (float)info.GetValue(instance), emptyLayoutOptions));
                        }
                        else if (type == typeof(int))
                        {
                            info.SetValue(instance, EditorGUILayout.IntField(nickName, (int)info.GetValue(instance), emptyLayoutOptions));
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
                            info.SetValue(instance, EditorGUILayout.MaskField(nickName, (LayerMask)info.GetValue(instance), layerNames, emptyLayoutOptions));
                        }
                        else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                        {
                            info.SetValue(instance, EditorGUILayout.ObjectField(nickName, (UnityEngine.Object)info.GetValue(instance), type, true, emptyLayoutOptions));
                        }
                        else if (type == typeof(Rect))
                        {
                            info.SetValue(instance, EditorGUILayout.RectField(nickName, (Rect)info.GetValue(instance), emptyLayoutOptions));
                        }
                        else if (type == typeof(RectInt))
                        {
                            info.SetValue(instance, EditorGUILayout.RectIntField(nickName, (RectInt)info.GetValue(instance), emptyLayoutOptions));
                        }
                        else if (type == typeof(string))
                        {
                            info.SetValue(instance, EditorGUILayout.TextField(nickName, (string)info.GetValue(instance), emptyLayoutOptions));
                        }
                        else if (type == typeof(Vector2))
                        {
                            info.SetValue(instance, EditorGUILayout.Vector2Field(nickName, (Vector2)info.GetValue(instance), emptyLayoutOptions));
                        }
                        else if (type == typeof(Vector2Int))
                        {
                            info.SetValue(instance, EditorGUILayout.Vector2IntField(nickName, (Vector2Int)info.GetValue(instance), emptyLayoutOptions));
                        }
                        else if (type == typeof(Vector3))
                        {
                            info.SetValue(instance, EditorGUILayout.Vector3Field(nickName, (Vector3)info.GetValue(instance), emptyLayoutOptions));
                        }
                        else if (type == typeof(Vector3Int))
                        {
                            info.SetValue(instance, EditorGUILayout.Vector3IntField(nickName, (Vector3Int)info.GetValue(instance), emptyLayoutOptions));
                        }
                        else if (type == typeof(Vector4))
                        {
                            info.SetValue(instance, EditorGUILayout.Vector4Field(nickName, (Vector4)info.GetValue(instance), emptyLayoutOptions));
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }
    }
}
