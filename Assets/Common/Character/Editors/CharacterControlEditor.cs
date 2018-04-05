using System;
using UnityEngine;
using UnityEditor;

namespace APlusOrFail.Character.Editors
{ 
    //[CustomEditor(typeof(CharacterControl))]
    //public class CharacterControlEditor : Editor
    //{
    //    private static readonly GUILayoutOption[] emptyLayoutOptions = Array.Empty<GUILayoutOption>();

    //    private CharacterControl charControl;
    //    private CapsuleCollider2D capsuleCollider;

    //    private void OnEnable()
    //    {
    //        charControl = target as CharacterControl;
    //        capsuleCollider = charControl?.GetComponent<CapsuleCollider2D>();
    //    }

    //    public override void OnInspectorGUI()
    //    {
    //        base.OnInspectorGUI();
    //        if (charControl != null)
    //        {
    //            GUILayout.BeginVertical(emptyLayoutOptions);

    //            GUILayout.BeginHorizontal(emptyLayoutOptions);

    //            GUILayout.Label("Show", emptyLayoutOptions);
    //            if (GUILayout.Button("Idle", emptyLayoutOptions))
    //            {
    //                capsuleCollider.offset = charControl.capsuleColliderIdleOffset;
    //                capsuleCollider.size = charControl.capsuleColliderIdleSize;
    //                capsuleCollider.direction = charControl.capsuleColliderIdleDirection;
    //            }
    //            if (GUILayout.Button("Squat", emptyLayoutOptions))
    //            {
    //                capsuleCollider.offset = charControl.capsuleColliderSquatOffset;
    //                capsuleCollider.size = charControl.capsuleColliderSquatSize;
    //                capsuleCollider.direction = charControl.capsuleColliderSquatDirection;
    //            }

    //            GUILayout.EndHorizontal();

    //            GUILayout.BeginHorizontal(emptyLayoutOptions);

    //            GUILayout.Label("Save", emptyLayoutOptions);
    //            if (GUILayout.Button("Idle", emptyLayoutOptions))
    //            {
    //                charControl.capsuleColliderIdleOffset = capsuleCollider.offset;
    //                charControl.capsuleColliderIdleSize = capsuleCollider.size;
    //                charControl.capsuleColliderIdleDirection = capsuleCollider.direction;
    //            }
    //            if (GUILayout.Button("Squat", emptyLayoutOptions))
    //            {
    //                charControl.capsuleColliderSquatOffset = capsuleCollider.offset;
    //                charControl.capsuleColliderSquatSize = capsuleCollider.size;
    //                charControl.capsuleColliderSquatDirection = capsuleCollider.direction;
    //            }

    //            GUILayout.EndHorizontal();

    //            GUILayout.EndVertical();
    //        }
    //    }
    //}
}
