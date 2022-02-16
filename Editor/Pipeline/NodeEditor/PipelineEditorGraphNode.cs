using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Editor.NodeEditor;
using Framework.Pipeline;
using Framework.Pipeline.PipelineGraph;
using UnityEditor;
using UnityEngine;

namespace Editor.Pipeline.NodeEditor
{
    [Serializable]
    public class PipelineEditorGraphNode : EditorGraphNode
    {
        private object[] values;
        internal GraphNode dataStorage;

        private List<Type> pipelineSteps;
        private string[] pipelineStepsNames;
        private Type selectedPipelineStep;
        private int selectedPipelineStepIndex;

        public PipelineEditorGraphNode(Vector2 position,
            float width,
            float height,
            GUIStyle nodeStyle,
            GUIStyle inPointStyle,
            GUIStyle outPointStyle,
            Action<ConnectionPoint> onClickInPoint,
            Action<ConnectionPoint> onClickOutPoint,
            GUIStyle selectedStyle,
            Action<EditorGraphNode> onClickRemoveNode) :
            base(position, width, height, nodeStyle, inPointStyle, outPointStyle,
                onClickInPoint, onClickOutPoint, selectedStyle, onClickRemoveNode)
        {
            dataStorage = new GraphNode();
            title = "New Node";
            pipelineSteps = FindPipelineScripts();
            pipelineStepsNames = pipelineSteps.Select(step => step.Name).ToArray();
            selectedPipelineStepIndex = 0;
        }

        private List<Type> FindPipelineScripts()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(ass => ass.GetTypes())
                .Where(mytype => mytype.GetInterfaces().Contains(typeof(IPipelineStep))).ToList();
        }

        protected override void WindowFunction(int windowId)
        {
            //script dropdown
            var selectedStepIndex =
                EditorGUILayout.Popup("PipelineSteps", selectedPipelineStepIndex, pipelineStepsNames);

            DrawFields();
            
            //changed script
            if (selectedStepIndex != selectedPipelineStepIndex)
            {
                //update index
                selectedPipelineStepIndex = selectedStepIndex;
                //set new values and new instance variable
                selectedPipelineStep = pipelineSteps[selectedPipelineStepIndex];
                title = selectedPipelineStep.Name;
                var fields = selectedPipelineStep.GetFields();
                values = new object[fields.Length];
                var assembly = selectedPipelineStep.Assembly;
                var instance = assembly.CreateInstance(selectedPipelineStep.FullName);
                dataStorage.Instance = instance;
            }
        }

        private void DrawFields()
        {
            if (selectedPipelineStep != null && values != null)
            {
                var scriptClass = selectedPipelineStep;
                var fields = scriptClass.GetFields();

                for (var i = 0; i < fields.Length; i++)
                {
                    DrawField(i, fields[i]);
                }
            }
        }

        private void DrawField(int id, FieldInfo fieldInfo)
        {
            var t = fieldInfo.FieldType;
            if (t == typeof(Single))
            {
                values[id] = EditorGUILayout.FloatField(new GUIContent(fieldInfo.Name),
                    values[id] is float ? (float)values[id] : 0);
            }

            if (t == typeof(Int32))
            {
                values[id] = EditorGUILayout.IntField(new GUIContent(fieldInfo.Name),
                    values[id] is int ? (int)values[id] : 0);
            }

            if (t == typeof(Vector2))
            {
                values[id] = EditorGUILayout.Vector2Field(new GUIContent(fieldInfo.Name),
                    values[id] is Vector2 ? (Vector2)values[id] : Vector2.zero);
            }

            if (t == typeof(Vector3))
            {
                values[id] = EditorGUILayout.Vector3Field(new GUIContent(fieldInfo.Name),
                    values[id] is Vector3 ? (Vector3)values[id] : Vector3.zero);
            }

            fieldInfo.SetValue(dataStorage.Instance, values[id]);
        }
    }
}