using System;
using System.Globalization;
using System.Reflection;
using Project.Source;
using UnityEditor;
using UnityEngine;

namespace Project.Editor
{
    public class PlayerProgressStorageWindow : EditorWindow
    {
        [MenuItem("Cheats/Player Progress Storage")]
        public static void ShowWindow()
        {
            GetWindow<PlayerProgressStorageWindow>("Player Progress Storage");
        }

        private void OnGUI()
        {
            GUILayout.Label("Data", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("box");

            GUILayout.Label("Coins", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Value", PlayerProgressStorage.Data.Coins.Value.ToString(),
                EditorStyles.boldLabel);

            EditorGUILayout.Space();

            GUILayout.Label("Magnet", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Magnet Unlocked", PlayerProgressStorage.Data.Magnet.Unlocked.ToString());
            EditorGUILayout.LabelField("Magnet Level", PlayerProgressStorage.Data.Magnet.Level.ToString());
            EditorGUILayout.LabelField("Magnet Radius",
                PlayerProgressStorage.Data.Magnet.Radius.ToString(CultureInfo.CurrentCulture));

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            GUILayout.Label("Commands", EditorStyles.boldLabel);

            if (GUILayout.Button("Hack Coins", GUILayout.Height(30)))
                HackCoins();

            if (GUILayout.Button("Reset Coins", GUILayout.Height(30)))
                ResetCoins();

            if (GUILayout.Button("Reset Magnet", GUILayout.Height(30)))
                ResetMagnet();

            if (Application.isPlaying)
                Repaint();
        }

        private void HackCoins()
        {
            object coinsInstance = PlayerProgressStorage.Data.Coins;
            if (coinsInstance == null) return;

            // 1. Modify the private backing variable
            SetPrivateField(coinsInstance, "_value", int.MaxValue);

            // 2. Trigger the "changed" delegate on the Coins instance
            InvokeEvent(coinsInstance, "changed", int.MaxValue);

            GUI.changed = true;
        }

        private void ResetCoins()
        {
            object coinsInstance = PlayerProgressStorage.Data.Coins;
            if (coinsInstance == null) return;

            // 1. Modify the private backing variable
            SetPrivateField(coinsInstance, "_value", 0);

            // 2. Trigger the "changed" delegate on the Coins instance
            InvokeEvent(coinsInstance, "changed", 0);

            GUI.changed = true;
        }

        private void ResetMagnet()
        {
            object magnetInstance = PlayerProgressStorage.Data.Magnet;
            if (magnetInstance == null) return;

            // 1. Modify the internal private parameters
            SetPrivateField(magnetInstance, "_unlocked", false);
            SetPrivateField(magnetInstance, "_level", 0);

            // 2. Trigger the "changed" event passing the modified magnet object reference itself
            InvokeEvent(magnetInstance, "changed", magnetInstance);

            GUI.changed = true;
        }

        private void SetPrivateField(object targetInstance, string fieldName, object value)
        {
            var type = targetInstance.GetType();
            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (field != null)
            {
                field.SetValue(targetInstance, value);
            }
            else
            {
                Debug.LogWarning($"Reflection Error: Field '{fieldName}' not found on type {type.Name}");
            }
        }

        private void InvokeEvent(object targetInstance, string eventFieldName, params object[] args)
        {
            Type type = targetInstance.GetType();
            FieldInfo eventField = null;

            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            eventField = type.GetField(eventFieldName, bindingFlags);
            if (eventField == null && type.BaseType != null)
            {
                foreach (var field in type.GetFields(bindingFlags))
                {
                    if (field.Name.EndsWith("." + eventFieldName))
                    {
                        eventField = field;
                        break;
                    }
                }
            }

            if (eventField == null) eventField = type.GetField("_" + eventFieldName, bindingFlags);
            if (eventField == null) eventField = type.GetField(eventFieldName + "Event", bindingFlags);

            if (eventField != null)
            {
                MulticastDelegate del = eventField.GetValue(targetInstance) as MulticastDelegate;
                if (del != null)
                {
                    del.DynamicInvoke(args);
                }
                else
                {
                    Debug.LogWarning($"Event '{eventFieldName}' has no active subscribers.");
                }
            }
            else
            {
                Debug.LogWarning(
                    $"Reflection Error: Could not resolve backing field for public event '{eventFieldName}' on type {type.Name}");
            }
        }
    }
}