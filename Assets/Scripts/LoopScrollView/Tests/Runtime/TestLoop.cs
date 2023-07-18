using System;
using System.Collections;
using System.Collections.Generic;
using ET.Client;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    public class TestLoop : MonoBehaviour
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
            GetComponent<LoopScrollView>()?.SetRefreshHandler(((transform1, i) =>
                    transform1.GetComponent<Text>().text = i.ToString()
                ));
            GetComponent<LoopScrollView>()?.InitLoopScroll(prefab, Count);
        }
    }
}