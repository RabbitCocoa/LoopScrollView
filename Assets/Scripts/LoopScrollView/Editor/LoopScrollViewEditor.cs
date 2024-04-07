// /*****************************
// 项目:Unity.Loader
// 文件:LoopScrollViewEditor.cs
// 创建时间:18:03
// 作者:cocoa
// 描述：
// *****************************/

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
namespace ET.Client
{
    [CustomEditor(typeof(LoopScrollView))]
    public class LoopScrollViewEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LoopScrollView loopScrollView = (LoopScrollView)target;
            loopScrollView.scale = EditorGUILayout.FloatField("Scale", loopScrollView.scale);

            loopScrollView.content =
                EditorGUILayout.ObjectField("Content", loopScrollView.content, typeof(RectTransform), true) as
                    RectTransform;
            loopScrollView.viewport =
                EditorGUILayout.ObjectField("ViewPort", loopScrollView.viewport, typeof(RectTransform), true) as
                    RectTransform;


            loopScrollView.horizontal = EditorGUILayout.Toggle("Horizontal", loopScrollView.horizontal);
            loopScrollView.vertical = EditorGUILayout.Toggle("Vertical", loopScrollView.vertical);
            loopScrollView.movementType =
                (ScrollRect.MovementType)EditorGUILayout.EnumPopup("Movement Type", loopScrollView.movementType);
            loopScrollView.elasticity = EditorGUILayout.FloatField("Elasticity", loopScrollView.elasticity);
            loopScrollView.inertia = EditorGUILayout.Toggle("Inertia", loopScrollView.inertia);
            loopScrollView.decelerationRate =
                EditorGUILayout.FloatField("Deceleration Rate", loopScrollView.decelerationRate);
            loopScrollView.scrollSensitivity =
                EditorGUILayout.FloatField("Scroll Sensitivity", loopScrollView.scrollSensitivity);

            loopScrollView.horizontalScrollbar = (Scrollbar)EditorGUILayout.ObjectField("Horizontal Scrollbar",
                loopScrollView.horizontalScrollbar, typeof(Scrollbar), true);
            loopScrollView.horizontalScrollbarVisibility =
                (ScrollRect.ScrollbarVisibility)EditorGUILayout.EnumPopup("Horizontal Scrollbar Visibility",
                    loopScrollView.horizontalScrollbarVisibility);
            loopScrollView.horizontalScrollbarSpacing = EditorGUILayout.FloatField("Horizontal Scrollbar Spacing",
                loopScrollView.horizontalScrollbarSpacing);

            loopScrollView.verticalScrollbar = (Scrollbar)EditorGUILayout.ObjectField("Vertical Scrollbar",
                loopScrollView.verticalScrollbar, typeof(Scrollbar), true);
            loopScrollView.verticalScrollbarVisibility =
                (ScrollRect.ScrollbarVisibility)EditorGUILayout.EnumPopup("Vertical Scrollbar Visibility",
                    loopScrollView.verticalScrollbarVisibility);
            loopScrollView.verticalScrollbarSpacing = EditorGUILayout.FloatField("Vertical Scrollbar Spacing",
                loopScrollView.verticalScrollbarSpacing);


            if (GUI.changed)
            {
                EditorUtility.SetDirty(loopScrollView);
            }
        }

        #region MenuItem

        [MenuItem("GameObject/UI/Loop ScrollView")]
        public static void AddLoopScrollView()
        {
            //获得Select
            GameObject select = Selection.activeGameObject;
            //设置Recttransform 为0 


            GameObject loopScrollView = new GameObject("LoopScroll");
            if (select.transform != null)
                loopScrollView.transform.SetParent(select.transform);
            loopScrollView.transform.localScale = Vector3.one;
            loopScrollView.transform.localPosition = Vector3.zero;
            ;

            loopScrollView.layer = LayerMask.NameToLayer("UI");
            var loop = loopScrollView.AddComponent<LoopScrollView>();
            loop.vertical = true;
            loopScrollView.AddComponent<Image>().color = new Color(1, 1, 1, 0.5f);

            GameObject viewrect = AddChild("ViewRect", loopScrollView.transform, Vector2.one * 0.5f);
            viewrect.AddComponent<RectMask2D>();
            loop.viewport = viewrect.transform as RectTransform;


            GameObject content = AddChild("Content", viewrect.transform, new Vector2(0.5f, 1f));
            loop.content = content.transform as RectTransform;
            content.AddComponent<VerticalLayoutGroup>().childForceExpandHeight = false;
        }

        private static GameObject AddChild(string Name, Transform parent, Vector2 pivot)
        {
            GameObject content = new GameObject(Name);

            content.layer = LayerMask.NameToLayer("UI");
            content.transform.SetParent(parent);
            content.transform.localScale = Vector3.one;
            content.transform.localPosition = Vector3.zero;

            //设置RectTransform为四边模式
            RectTransform rectTransform = content.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = pivot;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            return content;
        }

        #endregion
    }
}
#endif