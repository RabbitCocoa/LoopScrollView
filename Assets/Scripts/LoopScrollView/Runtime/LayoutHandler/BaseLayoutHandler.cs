// /*****************************
// 项目:Unity.Loader
// 文件:BaseLayoutHandler.cs
// 创建时间:17:44
// 作者:cocoa
// 描述：
// *****************************/

using UnityEngine;

namespace ET.Client
{
    public class BaseLayoutHandler : ILayoutHandler
    {
        public int dataCount;


        public virtual Vector2 OnePageOffset { get; }

        public virtual int MaxShowCount { get; }
        public virtual int ItemCount { get; }

        public virtual int GetStartIndexAndReSize(bool forceResize = false)
        {
            throw new System.NotImplementedException();
        }
        public virtual int JumpTo(int startIndex)
        {
            throw new System.NotImplementedException();
        }

        public virtual Vector2 GetItemPos(int index)
        {
            throw new System.NotImplementedException();
        }

        public virtual void SetAnchor(RectTransform transform)
        {
            //左上为锚点
            transform.anchorMin = Vector2.up;
            transform.anchorMax = Vector2.up;
        }

        #region Help Method

        protected void SetWidthAndHeight(RectTransform rectTransform, float width, float height)
        {
            Vector2 oldSize = rectTransform.rect.size;
            Vector2 deltaSize = new Vector2(width, height) - oldSize;

            rectTransform.offsetMin = rectTransform.offsetMin -
                                      new Vector2(deltaSize.x * rectTransform.pivot.x,
                                          deltaSize.y * rectTransform.pivot.y);
            rectTransform.offsetMax = rectTransform.offsetMax + new Vector2(deltaSize.x * (1f - rectTransform.pivot.x),
                deltaSize.y * (1f - rectTransform.pivot.y));
        }

       

        #endregion
    }
}