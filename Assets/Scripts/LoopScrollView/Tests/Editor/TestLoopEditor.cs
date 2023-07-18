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
    [CustomEditor(typeof(TestLoop))]
    public class TestLoopEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            TestLoop testLoop = (TestLoop)target;

            testLoop.Count = EditorGUILayout.IntField("Content", testLoop.Count);
            testLoop.prefab = EditorGUILayout.ObjectField("prefab",  testLoop.prefab,typeof(GameObject)) as GameObject;
          
            if (GUI.changed)
            {
                EditorUtility.SetDirty(testLoop);
                if (Application.isPlaying)
                {
                    testLoop.Change();
                }
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
            loopScrollView.transform.SetParent(select.transform);
            loopScrollView.transform.localScale = Vector3.one;
            loopScrollView.transform.localPosition = Vector3.zero;
            ;

            loopScrollView.layer = LayerMask.NameToLayer("UI");
            var loop = loopScrollView.AddComponent<LoopScrollView>();
            loop.vertical = true;
            loopScrollView.AddComponent<Image>().color = new Color(1, 1, 1, 0.5f);

            GameObject viewrect = AddChild("ViewRect", loopScrollView.transform,Vector2.one * 0.5f);
            viewrect.AddComponent<RectMask2D>();
            loop.viewport = viewrect.transform as RectTransform;
            

            GameObject content = AddChild("Content", viewrect.transform,new Vector2(0.5f,1f));
            loop.content = content.transform as RectTransform;
            content.AddComponent<VerticalLayoutGroup>().childForceExpandHeight = false;
        }
        private static GameObject AddChild(string Name, Transform parent,Vector2 pivot)
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
            rectTransform.pivot =pivot;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            return content;
        }

        #endregion

    }
}
#endif