using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinAttribute))]
public class MinAttributeDrawer : PropertyDrawer {
    public override void OnGUI(Rect                 position,
                               SerializedProperty   property,
                               GUIContent           label) {

        MinAttribute min = attribute as MinAttribute;

        if (property.propertyType == SerializedPropertyType.Float) {
            float vf = EditorGUI.FloatField(position,
                                            label.text,
                                            property.floatValue);
            property.floatValue = Mathf.Clamp(vf,
                                              min.minValue,
                                              float.MaxValue);
            return;
        }

        if (property.propertyType == SerializedPropertyType.Integer) {
            int vi = EditorGUI.IntField(position,
                                        label.text,
                                        property.intValue);
            property.intValue = Mathf.Clamp(vi,
                                            (int)min.minValue,
                                            int.MaxValue);
            return;
        }

        EditorGUI.LabelField(position,
                             label.text,
                             "Use range with float or int.");
    }
}
