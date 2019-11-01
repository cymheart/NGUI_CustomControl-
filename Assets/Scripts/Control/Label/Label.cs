using CommonNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ControlNS
{
    [ExecuteInEditMode]
    public class Label : Control
    {
        UILabel label;

        [SerializeField, SetProperty("Font")]
        Font font;
        public Font Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
                label.trueTypeFont = value;
                isReLayout = true;
            }
        }

        [SerializeField, SetProperty("Text")]
        string text = "Label";
        public string Text
        {
            get { return text; }
            set
            {
                text = value;

                if(isHideText == false)
                    label.text = text;

                isReLayout = true;

                if (BindProcess != null)
                    BindProcess(this);
            }
        }

        [SerializeField, SetProperty("IsHideText")]
        bool isHideText = false;
        public bool IsHideText
        {
            get { return isHideText; }
            set
            {
                isHideText = value;
                isReLayout = true;

                if (isHideText)
                    label.text = "";
                else
                    label.text = text;
            }
        }

        // <summary>
        /// 设置文字对齐方式
        /// </summary>
        /// <param name="alignment"></param>
        [SerializeField, SetProperty("TextAlignment")]
        NGUIText.Alignment textAlignment;
        public NGUIText.Alignment TextAlignment
        {
            get
            {
                return label.alignment;
            }
            set
            {
                label.alignment = value;
                textAlignment = label.alignment;
                isReLayout = true;
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
            }
        }

        public float Alpha
        {
            get
            {
                return label.alpha;
            }
            set
            {
                label.alpha = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            label = gameObject.transform.Find("label").GetComponent<UILabel>();
        }
        protected override void Layout()
        {
            if(ctrlSizeChangeMode == ControlSizeChangeMode.FitContentSize)
            {
                Width = GetTextRenderWidth(text, label.trueTypeFont, Height, label.fontStyle);      
            }

            Vector3[] worldCorners = WorldCorners;
            float boxPosX = worldCorners[0].x;
            float y = (worldCorners[0].y + worldCorners[1].y) / 2;

            //
            float x = (worldCorners[3].x + worldCorners[0].x) / 2;
            Vector3 pos = new Vector3(x, y, label.transform.position.z);
            label.width = Width;
            label.height = Height;
            label.transform.position = pos;
        }
    }
}