using CommonNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ControlNS
{
    [ExecuteInEditMode]
    public class OptionSelecter : Control
    {
        UISprite leftArraw;
        UISprite rightArraw;
        UISprite optionSpr;
        UILabel label;

        string[] option;
        public string[] Option
        {
            get { return option; }
            set
            {
                option = value;
                SelectedIdx = 0;
            }
        }

        int selectedIdx = 0;
        public int SelectedIdx
        {
            get { return selectedIdx; }
            set
            {          
                if (Option == null || value >= Option.Length)
                    return;

                selectedIdx = value;
                label.text = Option[selectedIdx];
            }

        }

        [SerializeField, SetProperty("IsDisabled")]
        bool isDisabled = false;
        public bool IsDisabled
        {
            get
            {
                return isDisabled;
            }
            set
            {
                isDisabled = value;

                if (isDisabled)
                {
                    UIEventListener.Get(leftArraw.gameObject).onClick = null;
                    UIEventListener.Get(rightArraw.gameObject).onClick = null;

                    UIEventListener.Get(leftArraw.gameObject).onPress = null;
                    UIEventListener.Get(rightArraw.gameObject).onPress = null;
          
                }
                else
                {
                    UIEventListener.Get(leftArraw.gameObject).onClick = Click;
                    UIEventListener.Get(rightArraw.gameObject).onClick = Click;
                    UIEventListener.Get(leftArraw.gameObject).onPress = Press;
                    UIEventListener.Get(rightArraw.gameObject).onPress = Press;
                }

                if (BindProcess != null)
                    BindProcess(this);
            }
        }

        // <summary>
        /// 设置文字尺寸
        /// </summary>
        /// <param name="alignment"></param>
        [SerializeField, SetProperty("FontSize")]
        int fontSize;
        public int FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
                label.fontSize = fontSize;
            }
        }


        protected override void Awake()
        {
            base.Awake();

            leftArraw = gameObject.transform.Find("LeftArraw").GetComponent<UISprite>();
            rightArraw = gameObject.transform.Find("RightArraw").GetComponent<UISprite>();
            optionSpr = gameObject.transform.Find("Option").GetComponent<UISprite>();
            label = optionSpr.transform.Find("Label").GetComponent<UILabel>();

            UIEventListener.Get(leftArraw.gameObject).onClick = Click;
            UIEventListener.Get(rightArraw.gameObject).onClick = Click;

            UIEventListener.Get(leftArraw.gameObject).onPress = Press;
            UIEventListener.Get(rightArraw.gameObject).onPress = Press;
        }


        void Press(GameObject go, bool state)
        {
            UISprite arraw;
            if (go == leftArraw.gameObject)
            {
                arraw = leftArraw;
            }
            else
            {
                arraw = rightArraw;
            }


            if (state == true)
            {
                arraw.alpha = 0.5f;
            }
            else
            {
                arraw.alpha = 1f;
            }
        }

        void Click(GameObject go)
        {
            int idx;

            if(go == leftArraw.gameObject)
            {
                idx = Math.Max(0, SelectedIdx - 1);
            }
            else
            {
                idx = Math.Min(Option.Length - 1, SelectedIdx + 1);
            }

            SelectedIdx = idx;

            if (BindProcess != null)
                BindProcess(this);
        }

        protected override void Layout()
        {
            leftArraw.width = Height;
            leftArraw.height = Height;
            float x = -Width / 2 + leftArraw.width / 2;
            leftArraw.transform.localPosition = new Vector3(x, 0, 0);

            //
            rightArraw.width = Height;
            rightArraw.height = Height;
            x = Width / 2 - rightArraw.width / 2;
            rightArraw.transform.localPosition = new Vector3(x, 0, 0);

            //
            optionSpr.width = Width - Height * 2;
            optionSpr.height = Height;
            x = -Width / 2 + leftArraw.width + optionSpr.width / 2;
            optionSpr.transform.localPosition = new Vector3(x, 0, 0);

            float spacing = 20;
            label.width =(int)(optionSpr.width - spacing);
            label.height = optionSpr.height;
            label.transform.localPosition = Vector3.zero;
        }
    }
}