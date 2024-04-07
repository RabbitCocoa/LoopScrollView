using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ET.Client;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    public class TestLoop1 : MonoBehaviour
    {
        public GameObject prefab;
        public int Count;

        public void Change()
        {
            if (Application.isPlaying)
                GetComponent<LoopScrollView>()?.ChangeCount(Count);
        }

        private void Awake()
        {
            GetComponent<LoopScrollView>()?.SetRefreshHandler(OnRefreshHandler);
            GetComponent<LoopScrollView>()?.InitLoopScroll(prefab, Count);
            GetComponent<LoopScrollView>()?.JumpTo(29);
        }

        void OnRefreshHandler(Transform t, int i, int moveState)
        {
            
            int showCount = GetComponent<LoopScrollView>().GetCurMaxShowItemCount();
            int index = GetComponent<LoopScrollView>().GetShowIndex(i);

            t.localScale = Vector3.one;
            if (index == showCount / 2)
            {
                Debug.Log($"Cut Middle{i} index;{index} :{showCount}");
                if (moveState == 0)
                {
                    t.DOKill();
                    t.localScale = Vector3.one * 2;
                }
                else
                {
                    t.DOKill();
                    t.localScale = Vector3.one;
                    t.DOScale(2f, 1);
                }
            }

            if (moveState > 0)
            {
                if (index == showCount / 2 + 1)
                {
                    t.DOKill();
                    t.localScale = Vector3.one * 2;
                    t.DOScale(1f, 1);
                }
            }
            else if (moveState < 0)
            {
                if (index == showCount / 2 - 1)
                {
                    t.DOKill();
                    t.localScale = Vector3.one * 2;
                    t.DOScale(1f, 1);
                }
            }


            t.GetComponent<Text>().text = i.ToString();
        }
    }
}