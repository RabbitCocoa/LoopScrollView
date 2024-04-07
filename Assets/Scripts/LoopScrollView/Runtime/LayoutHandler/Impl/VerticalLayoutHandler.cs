// /*****************************
// 项目:Unity.Loader
// 文件:VerticalLayouerHandler.cs
// 创建时间:18:39
// 作者:cocoa
// 描述：
// *****************************/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    public class VerticalLayoutHandler : BaseLayoutHandler
    {
        #region 初始化时获得的Field

        private LoopScrollView loopScrollView;

        private VerticalLayoutGroup verticalLayoutGroup;
        private RectTransform content;
        private RectTransform viewRect;
        private float scale;
        private float slotHeight;
        private float slotPosX;
        private float slotPosY;
        private bool isInverse => verticalLayoutGroup.reverseArrangement;

        #endregion

        #region Help Component Property

        private float SlotHeight => this.slotHeight * this.scale;
        private float totalHeight => viewRect?.rect.height ?? 0;

        private RectOffset padding => verticalLayoutGroup?.padding;
        private float spacing => verticalLayoutGroup?.spacing ?? 0;

        private float ContentHeight
        {
            get
            {
                //获取content的高度
                return content.rect.height;
            }
            set { SetWidthAndHeight(content, content.rect.width, value); }
        }

        #endregion

        public VerticalLayoutHandler(LoopScrollView loopScrollView,
            RectTransform prefabItem, int dataCount, float scale)
        {
            this.content = loopScrollView.content;
            this.viewRect = loopScrollView.viewport;
            this.dataCount = dataCount;
            this.verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>();
            if (isInverse)
                this.slotPosY = prefabItem.rect.height * (prefabItem.pivot.y);
            else
                this.slotPosY = prefabItem.rect.height * (1 - prefabItem.pivot.y);
            this.slotPosX = prefabItem.rect.width * prefabItem.pivot.x;

            this.scale = scale;

            this.slotHeight = prefabItem.rect.height;

            if (ItemCount > MaxShowCount)
                ReSize(0);
        }

        #region Override

        public override int MaxShowCount
        {
            get
            {
                int count = (int)((totalHeight + spacing) / (SlotHeight + spacing));
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
            get => new Vector2(0, SlotHeight + spacing);
        }

        #endregion


        #region Override Method

        private int startIndex;

        public override int GetStartIndexAndReSize(bool forceResize = false)
        {
            //顶部是1 底部是0
            //float offset =  (slotHeight + spacing) / content.rect.height;
            Vector2 anchorPos = content.anchoredPosition;

            int endIndex = Mathf.Min(dataCount, startIndex + ItemCount);


            if (!isInverse)
            {
                float top = (startIndex) * OnePageOffset.y;
                ;
                float bottom = (startIndex + 1) * OnePageOffset.y;

                //头尾不需要处理
                if (startIndex == 0 && anchorPos.y < top)
                {
                    if (forceResize)
                        ReSize(startIndex);
                    return startIndex;
                }


                if (endIndex == dataCount && anchorPos.y > bottom)
                {
                    if (forceResize)
                        ReSize(startIndex);
                    return startIndex;
                }


                if (anchorPos.y > bottom)
                {
                    ReSize(startIndex + 1);
                    //调整content的长度
                }
                else if (anchorPos.y < top)
                {
                    ReSize(startIndex - 1);
                }
                else if (forceResize)
                {
                    ReSize(startIndex);
                }
            }
            else
            {
                float top = (startIndex + 1) * OnePageOffset.y;
                ;
                float bottom = (startIndex) * OnePageOffset.y;

                //头尾不需要处理
                if (startIndex == 0 && -anchorPos.y < bottom)
                {
                    if (forceResize)
                        ReSize(startIndex);
                    return startIndex;
                }


                if (endIndex == dataCount && -anchorPos.y > top)
                {
                    if (forceResize)
                        ReSize(startIndex);
                    return startIndex;
                }


                if (-anchorPos.y > top)
                {
                    ReSize(startIndex + 1);
                    //调整content的长度
                }
                else if (-anchorPos.y < bottom)
                {
                    ReSize(startIndex - 1);
                }
                else if (forceResize)
                {
                    ReSize(startIndex);
                }
            }

            return startIndex;
        }

        public override Vector2 GetItemPos(int index)
        {
            float pad = isInverse ? padding.bottom : padding.top;
            float p = pad + slotPosY + (startIndex + index) * OnePageOffset.y;
            int inv = isInverse ? -1 : 1;

            float xOffset = 0;
            switch (verticalLayoutGroup.childAlignment)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.LowerLeft:
                    xOffset += padding.left;
                    break;
                case TextAnchor.UpperCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.LowerCenter:
                    xOffset = viewRect.rect.width / 2 - slotPosX;
                    break;
                case TextAnchor.UpperRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.LowerRight:
                    xOffset = viewRect.rect.width - 2 * slotPosX;
                    xOffset -= padding.right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            return new Vector2(slotPosX + xOffset, inv * -p);
        }

        public override void SetAnchor(RectTransform transform)
        {
            if (!isInverse)
                base.SetAnchor(transform);
            else
            {
                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.zero;
            }
        }

        public override int JumpTo(int index)
        {
            int startIndex = Mathf.Max(0, index - this.MaxShowCount / 2);


            //调整ContentSize

            //    startIndex = startIndex - MaxShowCount / 2;
            Vector2 anchorPos = Vector2.zero;

            if (!isInverse)
            {
                float top = (startIndex) * OnePageOffset.y;
                ;
                float bottom = (startIndex + 1) * OnePageOffset.y;
                anchorPos = new Vector2(0, top);
            }
            else
            {
                float top = (startIndex + 1) * OnePageOffset.y;

                float bottom = (startIndex) * OnePageOffset.y;
                anchorPos = new Vector2(0, -bottom);
            }

            content.anchoredPosition = anchorPos;

            ReSize(startIndex);
            return this.startIndex;
        }

        #endregion


        private void ReSize(int startIndex)
        {
            this.startIndex = startIndex;
            int endIndex = Mathf.Min(dataCount, startIndex + ItemCount);
            if (endIndex < MaxShowCount)
                endIndex = MaxShowCount;

            this.ContentHeight = endIndex * SlotHeight
                                 + Mathf.Max(0, endIndex - 1) * spacing +
                                 padding.bottom + padding.top;
        }
    }
}