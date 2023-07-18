// /*****************************
// 项目:Unity.Loader
// 文件:GridLayoutHandler.cs
// 创建时间:20:22
// 作者:cocoa
// 描述：
// *****************************/

using System;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

namespace ET.Client
{
    public class GridLayoutHandler : BaseLayoutHandler
    {
        #region 初始化时获得的Field

        private GridLayoutGroup gridLayoutGroup;
        private RectTransform content;
        private RectTransform viewRect;
        private LoopScrollView loopScrollView;

        public bool isInverse
        {
            get
            {
                bool verticalInverse = (gridLayoutGroup.startCorner == GridLayoutGroup.Corner.LowerLeft
                                        || gridLayoutGroup.startCorner == GridLayoutGroup.Corner.LowerRight) &&
                                       isVertical;
                bool horizontalInverse =
                    (gridLayoutGroup.startCorner == GridLayoutGroup.Corner.UpperRight
                     || gridLayoutGroup.startCorner == GridLayoutGroup.Corner.LowerRight) && !isVertical;

                return isVertical ? verticalInverse : horizontalInverse;
            }
        }


        private float slotPosX;
        private float slotPosY;

        #endregion

        #region Help Component Property

        private RectOffset padding => gridLayoutGroup?.padding;
        private Vector2 spacing => gridLayoutGroup?.spacing ?? Vector2.zero;

        private Vector2 ContentRect
        {
            get
            {
                //获取content的高度
                return new Vector2(content.rect.width, content.rect.height);
            }
            set { SetWidthAndHeight(content, value.x, value.y); }
        }

        private float totalWidth => viewRect.rect.width;
        private float slotWidth => gridLayoutGroup.cellSize.x;

        private float totalHeight => viewRect.rect.height;
        private float slotHeight => gridLayoutGroup.cellSize.y;

        private int columnCount
        {
            get
            {
                switch (gridLayoutGroup.constraint)
                {
                    case GridLayoutGroup.Constraint.Flexible:
                        return (int)((totalWidth +
                                      gridLayoutGroup.spacing.x) / (slotWidth + gridLayoutGroup.spacing.x));

                    case GridLayoutGroup.Constraint.FixedColumnCount:
                        return gridLayoutGroup.constraintCount;

                    case GridLayoutGroup.Constraint.FixedRowCount:
                        return Mathf.CeilToInt((float)dataCount / this.rowCount);
                }

                throw new Exception($"GridLayout Constraint Error");
            }
        }

        private int rowCount
        {
            get
            {
                switch (gridLayoutGroup.constraint)
                {
                    case GridLayoutGroup.Constraint.Flexible:
                        return (int)((totalHeight +
                                      gridLayoutGroup.spacing.y) / (slotHeight + gridLayoutGroup.spacing.y));

                    case GridLayoutGroup.Constraint.FixedColumnCount:
                        return Mathf.CeilToInt((float)dataCount / this.columnCount);


                    case GridLayoutGroup.Constraint.FixedRowCount:
                        return gridLayoutGroup.constraintCount;
                }

                throw new Exception($"GridLayout Constraint Error");
            }
        }

        private bool isVertical => loopScrollView?.vertical ?? true;

        #endregion

        public GridLayoutHandler(LoopScrollView scrollView, RectTransform viewRect, RectTransform content,
            RectTransform prefabItem, int dataCount)
        {
            this.loopScrollView = scrollView;
            this.content = content;
            this.viewRect = viewRect;
            this.dataCount = dataCount;
            this.gridLayoutGroup = content.GetComponent<GridLayoutGroup>();


            this.slotPosY = slotWidth * (1 - prefabItem.pivot.y);
            this.slotPosX = slotHeight * prefabItem.pivot.x;


            if (ItemCount > MaxShowCount)
                ReSize(0);
        }

        #region Override

        public override int MaxShowCount => columnCount * rowCount;


        public override int ItemCount
        {
            get
            {
                if (isVertical)
                {
                    if (dataCount <= MaxShowCount + columnCount)
                        return dataCount;

                    if (dataCount - startIndex < (rowCount + 2) * columnCount)
                        return dataCount - startIndex;


                    return (rowCount + 2) * columnCount;
                }
                else
                {
                    if (dataCount <= MaxShowCount + rowCount)
                        return dataCount;
                    return rowCount * (columnCount + 2);
                }
            }
        }

        public override Vector2 OnePageOffset
        {
            get => new Vector2((slotWidth + gridLayoutGroup.spacing.x), (slotHeight + gridLayoutGroup.spacing.y));
        }

        #endregion

        #region Override Method

        private int startIndex;

        public override int GetStartIndexAndReSize(bool forceResize)
        {
            Vector2 anchorPos = content.anchoredPosition;


            int endIndex = Mathf.Min(dataCount, startIndex + ItemCount);

            int startRow = isVertical ? startIndex / columnCount : startIndex % rowCount;
            int startCol = isVertical ? startIndex % columnCount : startIndex / rowCount;

            if (!isInverse)
            {
                float top =startRow * OnePageOffset.y;
                float bottom = (startRow + 1) * OnePageOffset.y;
                float left = startCol* OnePageOffset.x;
                float right = (startCol + 1) * OnePageOffset.x;

                //头尾不需要处理
                if (startIndex == 0 && ((isVertical && anchorPos.y < top)
                                        || (!isVertical && -anchorPos.x < left)))
                {
                    if (forceResize)
                        ReSize(startIndex);
                    return startIndex;
                }

                if (endIndex == dataCount &&
                    ((isVertical && anchorPos.y > bottom) || (!isVertical && -anchorPos.x > right)))
                {
                    if (forceResize)
                        ReSize(startIndex);
                    return startIndex;
                }

                if ((isVertical && anchorPos.y > bottom) || (!isVertical && -anchorPos.x > right))
                {
                    startIndex += isVertical ? columnCount : rowCount;

                    ReSize(startIndex);

                    //调整content的长度
                }
                else if ((isVertical && anchorPos.y < top) || (!isVertical && -anchorPos.x < left))
                {
                    startIndex -= isVertical ? columnCount : rowCount;
                    ReSize(startIndex);
                }
                else if (forceResize)
                    ReSize(startIndex);
            }
            else
            {
                float top = (startRow + 1) * OnePageOffset.y;
                float bottom = (startRow) * OnePageOffset.y;
                float left = (startCol + 1) * OnePageOffset.x;
                float right = (startCol) * OnePageOffset.x;

                //头尾不需要处理
                if (startIndex == 0 && ((isVertical && -anchorPos.y < bottom) || (!isVertical && anchorPos.x < right)))
                {
                    if (forceResize)
                        ReSize(startIndex);
                    return startIndex;
                }

                if (endIndex == dataCount &&
                    ((isVertical && -anchorPos.y > top) || (!isVertical && anchorPos.x > left)))
                {
                    if (forceResize)
                        ReSize(startIndex);
                    return startIndex;
                }

                if ((isVertical && -anchorPos.y > top) || (!isVertical && anchorPos.x > left))
                {
                    startIndex += isVertical ? columnCount : rowCount;

                    ReSize(startIndex);

                    //调整content的长度
                }
                else if ((isVertical && -anchorPos.y < bottom) || (!isVertical && anchorPos.x < right))
                {
                    startIndex -= isVertical ? columnCount : rowCount;
                    ReSize(startIndex);
                }
                else if (forceResize)
                    ReSize(startIndex);
            }

            return startIndex;
        }

        public override Vector2 GetItemPos(int index)
        {
            int start = isVertical ? startIndex / columnCount : startIndex / rowCount;

            int rowIndex = isVertical
                ? index / columnCount
                : index % rowCount;

            rowIndex += isVertical ? start : 0;

            int columnIndex = !isVertical
                ? index / rowCount
                : index % columnCount;

            columnIndex += !isVertical ? start : 0;

            float x = slotPosX + columnIndex * OnePageOffset.x;
            float y = slotPosY + rowIndex * OnePageOffset.y;

            //计算偏移
            if (isInverse)
            {
                if (isVertical)
                {
                    //     y = y - padding.top + padding.bottom;
                    y = -y;
                }
                else
                {
                    //    x -= padding.left + padding.right;
                    x = -x; // Inverted horizontally
                }
            }

            float xOffset = 0, yOffset = 0;
            //计算对齐
            if (isVertical)
            {
                switch (gridLayoutGroup.childAlignment)
                {
                    //不提供右对齐
                    case TextAnchor.UpperLeft:
                    case TextAnchor.MiddleLeft:
                    case TextAnchor.LowerLeft:
                    case TextAnchor.UpperRight:
                    case TextAnchor.MiddleRight:
                    case TextAnchor.LowerRight:
                        xOffset += padding.left;
                        break;

                    case TextAnchor.UpperCenter:
                    case TextAnchor.MiddleCenter:
                    case TextAnchor.LowerCenter:
                        float slotArea = columnCount * OnePageOffset.x - spacing.x;
                        xOffset =
                            (viewRect.rect.width - slotArea) / 2;

                        break;


                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (isInverse)
                    yOffset -= padding.bottom;
                else
                    yOffset += padding.top;
            }
            else
            {
                switch (gridLayoutGroup.childAlignment)
                {
                    //不提供下对齐
                    case TextAnchor.UpperLeft:
                    case TextAnchor.UpperCenter:
                    case TextAnchor.UpperRight:
                    case TextAnchor.LowerCenter:
                    case TextAnchor.LowerLeft:
                    case TextAnchor.LowerRight:
                        yOffset += padding.top;
                        break;

                    case TextAnchor.MiddleRight:
                    case TextAnchor.MiddleLeft:
                    case TextAnchor.MiddleCenter:

                        float slotArea = rowCount * OnePageOffset.y - spacing.y;
                        yOffset =
                            (viewRect.rect.height - slotArea) / 2;

                        break;


                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (isInverse)
                    xOffset -= padding.right;
                else
                    xOffset += padding.left;
            }

            return new Vector2(x + xOffset, -y - yOffset);
        }

        public override void SetAnchor(RectTransform transform)
        {
            if (!isInverse)
                base.SetAnchor(transform);
            else
            {
                if (isVertical)
                {
                    transform.anchorMin = Vector2.zero;
                    transform.anchorMax = Vector2.zero;
                }
                else
                {
                    transform.anchorMin = Vector2.one;
                    transform.anchorMax = Vector2.one;
                }
            }
        }

        #endregion

        private void ReSize(int startIndex)
        {
            this.startIndex = startIndex;
            int endIndex = Mathf.Min(dataCount, startIndex + ItemCount);

            if (endIndex < MaxShowCount)
                endIndex = MaxShowCount;

            if (isVertical)
            {
                // If the scroll is vertical, we adjust the content height
                int rows = endIndex / columnCount;
                int overflow = (endIndex % columnCount == 0) ? 0 : 1;

                ContentRect = new Vector2(ContentRect.x,
                    (rows + overflow) * slotHeight + Mathf.Max(0, rows + overflow - 1) * gridLayoutGroup.spacing.y +
                    gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom);
            }
            else
            {
                // If the scroll is horizontal, we adjust the content width
                int columns = endIndex / rowCount;
                int overflow = (endIndex % rowCount == 0) ? 0 : 1;
                ContentRect = new Vector2((columns + overflow) * slotWidth +
                                          Mathf.Max(0, columns + overflow - 1) * gridLayoutGroup.spacing.x +
                                          gridLayoutGroup.padding.left + gridLayoutGroup.padding.right, ContentRect.y);
            }
        }
    }
}