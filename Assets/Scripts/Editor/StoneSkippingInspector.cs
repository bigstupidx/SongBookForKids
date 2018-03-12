using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Syng {
    [CustomEditor(typeof(StoneSkipping))]
    public class StoneSkippingInspector : Editor {

        private SerializedProperty maxThrowDistance;
        private SerializedProperty maxHorizontalAngle;
        private SerializedProperty numSkipsPerThrow;
        private SerializedProperty firstSkipOffset;
        private SerializedProperty skipDistribution;
        private Transform targetT;

        void OnEnable() {
            targetT = (target as StoneSkipping).transform;
            maxThrowDistance = serializedObject.FindProperty("maxThrowDistance");
            maxHorizontalAngle = serializedObject.FindProperty("maxHorizontalAngle");
            numSkipsPerThrow = serializedObject.FindProperty("numSkipsPerThrow");
            firstSkipOffset = serializedObject.FindProperty("firstSkipOffset");
            skipDistribution = serializedObject.FindProperty("skipDistribution");
        }

        void OnDisable() {
            Tools.current = Tool.Move;
        }

        // public override void OnInspectorGUI() {
        //     DrawDefaultInspector();

        //     if (GUI.changed) {
        //         serializedObject.ApplyModifiedProperties();
        //     }
        // }

        public void OnSceneGUI() {
            Vector3 maxThrowPoint = targetT.position + Vector3.up * maxThrowDistance.floatValue;
            Vector3 newSkipDistPoint = Vector3.zero;

            // Drag handle for setting skip distance
            Handles.color = Color.blue;
            Vector3 offset = Vector3.up * HandleUtility.GetHandleSize(maxThrowPoint) * 0.25f;
            if (Drag(maxThrowPoint + offset, ref newSkipDistPoint)) {
                maxThrowDistance.floatValue = (newSkipDistPoint - offset - targetT.position).magnitude;
            }

            // Dotted line from transform to skip distance handle
            Handles.color = Color.white;
            Handles.DrawDottedLine(targetT.position, maxThrowPoint, 1f);

            // Calculate the opposite Vector
            float angle = maxHorizontalAngle.floatValue;
            float oppositeLen = Mathf.Tan(angle * Mathf.Deg2Rad) * maxThrowDistance.floatValue;
            Vector3 oppositeR = new Vector3(targetT.position.x + oppositeLen,
                                            targetT.position.y + maxThrowDistance.floatValue,
                                            targetT.position.z);
            Vector3 oppositeL = oppositeR - (Vector3.right * oppositeLen * 2f);

            // Angle handle
            Vector3 newOppositeR = Vector3.zero;
            if (Drag(oppositeR, ref newOppositeR)) {
                Vector3 hypotenuse = newOppositeR - targetT.position;
                maxHorizontalAngle.floatValue = Mathf.Atan(hypotenuse.x / hypotenuse.y) * Mathf.Rad2Deg;
            }

            // Visualize throw space
            Handles.DrawLine(targetT.position, oppositeR);
            Handles.DrawLine(targetT.position, oppositeL);

            // Handle for first skip offset
            Vector3 newFirstOffPos = Vector3.zero;
            Vector3 firstOffPos = GetOpposite(maxHorizontalAngle.floatValue,
                                              firstSkipOffset.floatValue) + targetT.position;
            if (Drag(firstOffPos, ref newFirstOffPos)) {
                firstSkipOffset.floatValue = newFirstOffPos.y - targetT.position.y;
            }

            // Draw skip lines
            float throwLen = maxThrowDistance.floatValue - firstSkipOffset.floatValue;
            float skipDist = throwLen / numSkipsPerThrow.intValue;
            int maxSkips = numSkipsPerThrow.intValue + 1;
            for (int i=0; i<maxSkips; i++) {
                float skipPos = skipDistribution.animationCurveValue.Evaluate((skipDist * i) / throwLen);
                float adjacent = skipPos * throwLen + firstSkipOffset.floatValue;
                oppositeLen = Mathf.Tan(angle * Mathf.Deg2Rad) * adjacent;
                oppositeL = new Vector3(oppositeLen * -1, adjacent, 0f);
                oppositeL += targetT.position;
                oppositeR = oppositeL + Vector3.right * oppositeLen * 2;
                Handles.color = Color.Lerp(new Color(1f, 0.6f, 0f),
                                           Color.red,
                                           (float)i / (float)maxSkips);
                Handles.DrawLine(oppositeL, oppositeR);
            }

            // Visualize skip coordinates for all animations
            List<StoneSkipAnim> anims = (target as StoneSkipping).animations;
            if (anims != null) {
                Handles.color = Color.white;
                for (int i=0; i<anims.Count; i++) {
                    StoneSkipAnim anim = anims[i];
                    Vector3[] coords = anim.skipCoords;

                    for (int j=0; j<anim.numSkips; j++) {
                        Handles.DrawWireDisc(targetT.position + coords[j],
                                             Vector3.forward,
                                             10f);
                    }
                    Handles.DrawLine(targetT.position + coords[0],
                                     targetT.position + coords[anim.numSkips - 1]);
                }
            }

            // For some odd reason, the first child object gets the
            // default position handle. Therefore, we need to manually
            // render one at this transform's position.
            Tools.current = Tool.None;
            Vector3 pos = targetT.position;
            pos = Handles.PositionHandle(pos, Quaternion.identity);
            targetT.position = pos;

            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private Vector3 GetOpposite(float angle, float dist) {
            float oppositeLen = Mathf.Tan(angle * Mathf.Deg2Rad) * dist;
            return new Vector3(oppositeLen, dist, 0f);
        }

        private bool Drag(Vector3 handlePos, ref Vector3 pos) {
            float size = HandleUtility.GetHandleSize(handlePos) * 0.25f;
            HandlesHelper.DragHandleResult res;
            pos = HandlesHelper.DragHandle(handlePos,
                                           size,
                                           Handles.CubeCap,
                                           Color.white,
                                           out res);

            switch (res) {
                case HandlesHelper.DragHandleResult.LMBDrag:
                    return true;
            }
            return false;
        }
    }
}
