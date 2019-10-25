using CommonNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ControlNS
{
    [ExecuteInEditMode]
    public class CheckBox : Control
    {
        UISprite bg;
        UISprite check;
        UISprite locked;
        Label label;

 
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
                label.Font = value;
                label.Text = text;
            }
        }

        [SerializeField, SetProperty("Text")]
        string text = "CheckBox";
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                label.Text = text;

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
                if (value == 0)
                {
                    fontSize = check.height;
                    label.Height = fontSize;
                }
                else
                {
                    fontSize = value;
                    label.Height = fontSize;
                }
            }
        }

        [SerializeField, SetProperty("IsNotChangedDisabledColor")]
        bool isNotChangedDisabledColor = true;
        public bool IsNotChangedDisabledColor
        {
            get
            {
                return isNotChangedDisabledColor;
            }
            set
            {
                isNotChangedDisabledColor = value;

                if(!isNotChangedDisabledColor && isDisabled)
                {
                    label.Alpha = 0.5f;
                    bg.alpha = 0.5f;
                }
                else
                {
                    label.Alpha = 1f;
                    bg.alpha = 1f;
                }
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
                    if(!IsNotChangedDisabledColor)
                    {
                        label.Alpha = 0.5f;
                        bg.alpha = 0.5f;
                    }

                    baseCollider.enabled = false;
                }
                else
                {
                    label.Alpha = 1f;
                    baseCollider.enabled = true;
                    bg.alpha = 1f;
                }

                if (BindProcess != null)
                    BindProcess(this);
            }
        }

        [SerializeField, SetProperty("IsDisabledAndNotChangeColor")]
        bool isDisabledAndNotChangeColor = false;
        public bool IsDisabledAndNotChangeColor
        {
            get
            {
                return isDisabledAndNotChangeColor;
            }
            set
            {
                isDisabledAndNotChangeColor = value;
                IsNotChangedDisabledColor = value;
                IsDisabled = value;
            }
        }

        [SerializeField, SetProperty("IsCheck")]
        bool isCheck = false;
        public bool IsCheck
        {
            get
            {
                return isCheck;
            }
            set
            {
                if (isLock || isDisabled)
                    return;

                isCheck = value;
                if (isCheck)
                    check.gameObject.SetActive(true);
                else
                    check.gameObject.SetActive(false);

                if (BindProcess != null)
                    BindProcess(this);
            }
        }

        [SerializeField, SetProperty("IsLock")]
        bool isLock = false;
        public bool IsLock
        {
            get
            {
                return isLock;
            }
            set
            {
                if (isDisabled)
                    return;

                isLock = value;
                if (isLock)
                    locked.gameObject.SetActive(true);
                else
                    locked.gameObject.SetActive(false);

                if (BindProcess != null)
                    BindProcess(this);
            }
        }

        override public ControlSizeChangeMode CtrlSizeChangeMode
        {
            get
            {
                return ctrlSizeChangeMode;
            }
            set
            {
                ctrlSizeChangeMode = value;
                label.CtrlSizeChangeMode = ctrlSizeChangeMode;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            IsReLayoutByChild = true;

            bg = gameObject.transform.Find("bg").GetComponent<UISprite>();
            check = gameObject.transform.Find("check").GetComponent<UISprite>();
            locked = gameObject.transform.Find("lock").GetComponent<UISprite>();
            label = gameObject.transform.Find("Label").GetComponent<Label>();
            label.Parent = this;
            label.BindProcess = null;
            label.DockType =  DockType.None;
            label.MatchType = MatchType.None;

            UIEventListener.Get(gameObject).onClick = Click;
            UIEventListener.Get(gameObject).onHover = Hover;
        }

        void Click(GameObject go)
        {
            if (isLock || isDisabled)
                return;

            IsCheck = !isCheck;
        }

        void Hover(GameObject go, bool state)
        {
            if (isDisabled)
                return;

            if (state == true)
                bg.alpha = 0.5f;
            else
                bg.alpha = 1f;
        }

        protected override void Layout()
        {
            if (ctrlSizeChangeMode == ControlSizeChangeMode.FitContentSize)
            {
                Width = label.Width + bg.width + bg.width / 3;
                Height = Math.Max(label.Height, check.height);
            }
            else
            {
                label.Width = Width - (bg.width + bg.width / 3);
            }


            Vector3[] worldCorners = WorldCorners;
            float boxPosX = worldCorners[0].x;
            float y = (worldCorners[0].y + worldCorners[1].y) / 2;

            //
            Vector3[] conrners = bg.worldCorners;
            float bgWidth = conrners[3].x - conrners[0].x;
            float x = boxPosX + bgWidth / 2;
            Vector3 pos = bg.transform.position;
            pos.y = y;
            pos.x = x;
            bg.transform.position = pos;

           
            //
            check.transform.position = bg.transform.position + new Vector3(0.01f, 0, 0);

            //
            locked.transform.position = bg.transform.position + new Vector3(0.0025f, 0.012f, 0);


            //
            float labelWidth = label.Width / scale;
            x = boxPosX + bgWidth + bgWidth/3 + labelWidth / 2;
            pos = label.transform.position;
            pos.y = y;
            pos.x = x;
            label.transform.position = pos;

            int colliderHeight = Math.Max(label.Height, check.height);
            baseCollider.size = new Vector3(Width, colliderHeight, 0);

        }
    }
}
