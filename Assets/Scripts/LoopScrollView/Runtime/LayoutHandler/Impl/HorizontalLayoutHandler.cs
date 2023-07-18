// /*****************************
// 项目:Unity.Loader
// 文件:HorizontalLayoutHandler.cs
// 创建时间:19:47
// 作者:cocoa
// 描述：
// *****************************/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    public class HorizontalLayoutHandler : BaseLayoutHandler
    {
        #region 初始化时获得的Field

        private float slotWidth;
        private float slotPosX;
        private float slotPosY;
        private LoopScrollView loopScrollView;
        private HorizontalLayoutGroup horizontalLayout;
        private RectTransform content;
        private RectTransform viewRect;

        #endregion

        private bool isInverse => horizontalLayout.reverseArrangement;


        #region Help Component Property

        private float totalWidth => viewRect?.rect.width ?? 0;

        private RectOffset padding => horizontalLayout?.padding;
        private float spacing => horizontalLayout?.spacing ?? 0;

        private float ContentWidth
        {
            get
            {
                //获取content的高度
                return content.rect.width;
            }
            set { SetWidthAndHeight(content, value, content.rect.height); }
        }

        #endregion

        public HorizontalLayoutHandler(LoopScrollView loopScrollView,
            RectTransform prefabItem, int dataCount)
        {
            this.loopScrollView = loopScrollView;
            this.content = loopScrollView.content;
            this.viewRect = loopScrollView.viewport;


            this.dataCount = dataCount;

            this.slotWidth = prefabItem.rect.width;
            this.slotPosY = prefabItem.rect.height * (1 - prefabItem.pivot.y);
            this.slotPosX = prefabItem.rect.width * prefabItem.pivot.x;


            this.horizontalLayout = content.GetComponent<HorizontalLayoutGroup>();


            if (ItemCount > MaxShowCount)
                ReSize(0);
        }


        #region Override

        public override int MaxShowCount
        {
            get
            {
                int count = (int)((totalWidth + spacing) / (slotWidth + spacing));
                return count;
            }
        }

        public override int ItemCount
        {
            get
            {
                if (dataCount <= MaxShowCount + 1)
                    return dataCount;

                return MaxShowCount + 2;
            }
        }

        public override Vector2 OnePageOffset
        {
            get => new Vector2(slotWidth + spacing, 0);
        }

        #endregion


        #region Override Method

        private int startIndex;

        public override int GetStartIndexAndReSize(bool forceResize)
        {
            //顶部是1 底部是0
            //float offset =  (slotHeight + spacing) / content.rect.height;
            Vector2 anchorPos = content.anchoredPosition;

            int endIndex = Mathf.Min(dataCount, startIndex + ItemCount);

            if (!isInverse)
            {
                float left = (startIndex) * OnePageOffset.x;
                ;
                float right = (startIndex + 1) * OnePageOffset.x;


                //头尾不需要处理
                if (startIndex == 0 && -anchorPos.x < left)
                {
                    if (forceResize)
                        ReSize(startIndex);
                    return startIndex;
                }


                if (endIndex == dataCount && -anchorPos.x > right)
                {
                    if (forceResize)
                        ReSize(startIndex);
                    return startIndex;
                }


                if (-anchorPos.x > right)
                {
                    ReSize(startIndex + 1);
                    //调整content的长度
                }
                else if (-anchorPos.x < left)
                {
                    ReSize(startIndex - 1);
                }
                else if (forceResize)
                    ReSize(startIndex);
            }
            else
            {
                float left = (startIndex+1) * OnePageOffset.x;
                ;
                float right = (startIndex ) * OnePageOffset.x;

                //头尾不需要处理
                if (startIndex == 0 && anchorPos.x < right)
                {
                    if (forceResize)
                        ReSize(startIndex);
                    return startIndex;
                }


                if (endIndex == dataCount && anchorPos.x > left)
                {
                    if (forceResize)
                        ReSize(startIndex);
                    return startIndex;
                }
                
                if (anchorPos.x > left)
                {
                    ReSize(startIndex + 1);
                    //调整content的长度
                }
                else if (anchorPos.x < right)
                {
                    ReSize(startIndex - 1);
                }
                else if (forceResize)
                    ReSize(startIndex);
            }

            return startIndex;
        }

        public override Vector2 GetItemPos(int index)
        {
            float pad = isInverse ? padding.right : padding.left;
            float p = pad + slotPosX + (startIndex + index) * OnePageOffset.x;
            int inv = isInverse ? -1 : 1;

            float yOffset = 0;
            switch (horizontalLayout.childAlignment)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.UpperCenter:
                case TextAnchor.UpperRight:
                    yOffset += padding.top;
                    break;
                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    yOffset = viewRect.rect.height/2 - slotPosY ;
                    break;
                case TextAnchor.LowerLeft:
                case TextAnchor.LowerCenter:
                case TextAnchor.LowerRight:
                    yOffset = viewRect.rect.height - 2*slotPosY;
                    yOffset -= padding.bottom;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new Vector2(inv * p, -slotPosY-yOffset);
        }

        public override void SetAnchor(RectTransform transform)
        {
            if (!isInverse)
                base.SetAnchor(transform);
            else
            {
                transform.anchorMin = Vector2.one;
                transform.anchorMax = Vector2.one;
            }
        }

        #endregion

        private void ReSize(int startIndex)
        {
            this.startIndex = startIndex;
            int endIndex =
                Mathf.Min(dataCount, startIndex + ItemCount);

            if (endIndex < MaxShowCount)
                endIndex = MaxShowCount;

            this.ContentWidth =
                endIndex * slotWidth + Mathf.Max(0, endIndex - 1) * spacing +
                padding.left + padding.right;
        }
    }
}