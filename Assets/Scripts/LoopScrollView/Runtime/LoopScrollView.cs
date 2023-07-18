// /*****************************
// 项目:Unity.Loader
// 文件:LoopScrollView.cs
// 创建时间:14:49
// 作者:cocoa
// 描述：
// *****************************/

using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace ET.Client
{
    public class LoopScrollView : ScrollRect
    {
        #region Data Field

        private GameObject itemPrefab;
        private int Count; //数据的总量
        private List<GameObject> itemSlots = new();

        #endregion

        #region Handler

        public BaseLayoutHandler LayoutHandler;
        private Action<Transform, int> OnLoopRefershHandler;

        #endregion

        #region Inspertor

        public new bool horizontal
        {
            get => base.horizontal;
            set
            {
                base.horizontal = value;
                base.vertical = !value;
            }
        }

        public new bool vertical
        {
            get => base.vertical;
            set
            {
                base.vertical = value;
                base.horizontal = !value;
            }
        }

        #endregion


        #region Public

        // ReSharper disable Unity.PerformanceAnalysis
        public void InitLoopScroll(GameObject itemPrefab, int Count)
        {
            if (content == null)
            {
                Debug.LogError($"Content不能为空");
                return;
            }

            this.itemPrefab = itemPrefab;
            this.Count = Count;


            onValueChanged.RemoveListener(DragUpdate);
            onValueChanged.AddListener(DragUpdate);


            var layoutGroup = content.GetComponent<LayoutGroup>();

            bool isInverse = false;

            if (layoutGroup.GetType() == typeof(VerticalLayoutGroup))
            {
                isInverse = (layoutGroup as VerticalLayoutGroup).reverseArrangement;

                this.LayoutHandler = new VerticalLayoutHandler(this,
                    this.itemPrefab.transform as RectTransform,
                    Count
                );
            }
            else if (layoutGroup.GetType() == typeof(HorizontalLayoutGroup))
            {
                isInverse = (layoutGroup as HorizontalLayoutGroup).reverseArrangement;

                this.LayoutHandler = new HorizontalLayoutHandler(this,
                    this.itemPrefab.transform as RectTransform,
                    Count);
            }
            else if (layoutGroup.GetType() == typeof(GridLayoutGroup))
            {
                //不支持右下
                
                    

                this.LayoutHandler = new GridLayoutHandler(this,
                    viewport, content, itemPrefab.transform as RectTransform, Count);
                isInverse = ((GridLayoutHandler)LayoutHandler).isInverse;
            }

            if (horizontal)
            {
                content.pivot = isInverse
                    ? new Vector2(1, 0.5f)
                    : new Vector2(0, 0.5f);
            }
            else
            {
                content.pivot = isInverse
                    ? new Vector2(0.5f, 0)
                    : new Vector2(0.5f, 1); 
            }
            
            ReGenerateItems();

            layoutGroup.enabled = false;
        }


        public void ChangeCount(int Count)
        {
            this.Count = Count;
            LayoutHandler.dataCount = Count;
            ReGenerateItems();
        }

        public void SetRefreshHandler(Action<Transform, int> OnLoopRefershHandler)
        {
            this.OnLoopRefershHandler = OnLoopRefershHandler;
        }

        #endregion

        //当数据改变时 重新生成Slot
        private int lastIndex;

        //调整位置
        private void DragUpdate(Vector2 offset)
        {
            int startIndex = LayoutHandler.GetStartIndexAndReSize();

            if (startIndex != lastIndex)
            {
                lastIndex = startIndex;
                ResetDataAndPos(startIndex);
            }
        }

        private void ResetDataAndPos(int startIndex)
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (itemSlots[i] == null)
                    return;
                if (i +startIndex >= Count)
                {
                    itemSlots[i].SetActive(false);
                    continue;
                }
                else
                {
                    itemSlots[i].SetActive(true);
                }
                (itemSlots[i].transform as RectTransform).anchoredPosition
                    = LayoutHandler.GetItemPos(i);
                OnLoopRefershHandler(itemSlots[i].transform, startIndex + i);
            }
        }

        private void ReGenerateItems()
        {
            itemSlots.Clear();
            int i = Count;

            while (i < content.childCount)
            {
                GameObject.Destroy(content.GetChild(i++).gameObject);
            }

            for (i = 0; i < content.childCount; i++)
            {
                itemSlots.Add(content.GetChild(i).gameObject);
            }

            //根据数量判断是否需要生成新的预制体
            for (; i < LayoutHandler.ItemCount; i++)
            {
                itemSlots.Add(Instantiate(itemPrefab, content));
            }

            //设置锚点
            foreach (var itemSlot in itemSlots)
            {
                LayoutHandler.SetAnchor(itemSlot.transform as RectTransform);
            }

            //重新计算大小

            int index = LayoutHandler.GetStartIndexAndReSize(true);
            ResetDataAndPos(index);
        }
    }
}