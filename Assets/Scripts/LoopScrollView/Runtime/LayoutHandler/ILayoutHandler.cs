// /*****************************
// 项目:Unity.Loader
// 文件:ILayoutHandler.cs
// 创建时间:18:38
// 作者:cocoa
// 描述：
// *****************************/


using UnityEngine;

namespace ET
{
 

    public interface ILayoutHandler
    {
        public int MaxShowCount { get; } //同一个窗口最大显示的数量
        public int ItemCount { get; } //应该生成的item总数

        
        
        public int GetStartIndexAndReSize(bool forceResize = false);
        public Vector2 GetItemPos(int index);



    }
}