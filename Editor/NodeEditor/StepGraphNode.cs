using System;
using System.Reflection;
using Framework.Pipeline.PipelineGraph;
using UnityEditor;
using UnityEngine;

namespace Editor.NodeEditor
{
    [Serializable]
    public class StepGraphNode : GraphNode
    {
        private MonoScript srScript;
        private object[] values;
        internal Node dataStorage;

        public StepGraphNode(Vector2 position,
            float width,
            float height,
            GUIStyle nodeStyle,
            GUIStyle inPointStyle,
            GUIStyle outPointStyle,
            Action<ConnectionPoint> OnClickInPoint,
            Action<ConnectionPoint> OnClickOutPoint,
            GUIStyle selectedStyle,
            Action<GraphNode> OnClickRemoveNode) :
            base(position, width, height, nodeStyle, inPointStyle, outPointStyle,
                OnClickInPoint, OnClickOutPoint, selectedStyle, OnClickRemoveNode)
        {
            dataStorage = new Node();
            title = "New Node";
        }

        protected override void WindowFunction(int windowId)
        {
            srScript = EditorGUILayout.ObjectField(srScript, typeof(MonoScript), false) as MonoScript;

            if (DrawFields()) return;

            if (srScript != null)
            {
                title = srScript.name;
                dataStorage.Name = title;
                var scriptClass = srScript.GetClass();
                var fields = scriptClass.GetFields();
                values = new object[fields.Length];
                dataStorage.Instance = Activator.CreateInstance(scriptClass);
            }
        }
        
        private bool DrawFields()
        {
            if (srScript != null && values != null)
            {
                var scriptClass = srScript.GetClass();
                var fields = scriptClass.GetFields();

                for (var i = 0; i < fields.Length; i++)
                {
                    DrawField(i, fields[i]);
                }

                return true;
            }

            return false;
        }

        private void DrawField(int id, FieldInfo fieldInfo)
        {
            var t = fieldInfo.FieldType;
            if (t == typeof(Single))
            {
                values[id] = EditorGUILayout.FloatField(new GUIContent(fieldInfo.Name),
                    values[id] is float ? (float) values[id] : 0);
            }

            if (t == typeof(Int32))
            {
                values[id] = EditorGUILayout.IntField(new GUIContent(fieldInfo.Name),
                    values[id] is int ? (int) values[id] : 0);
            }

            if (t == typeof(Vector2))
            {
                values[id] = EditorGUILayout.Vector2Field(new GUIContent(fieldInfo.Name),
                    values[id] is Vector2 ? (Vector2) values[id] : Vector2.zero);
            }

            if (t == typeof(Vector3))
            {
                values[id] = EditorGUILayout.Vector3Field(new GUIContent(fieldInfo.Name),
                    values[id] is Vector3 ? (Vector3) values[id] : Vector3.zero);
            }
            
            fieldInfo.SetValue(dataStorage.Instance, values[id]);
        }
    }
}