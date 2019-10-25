using System;
using UnityEngine;

namespace ControlNS
{
    [ExecuteInEditMode]
    public class Button : Control
    {
        UISprite bg;
        Label label;



        [SerializeField, SetProperty("Font")]
        float textOffsetY = 4;
        public float TextOffsetY
        {
            get
            {
                return textOffsetY;
            }
            set
            {
                textOffsetY = value;
                isReLayout = true;
            }
        }


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
        string text = "Button";
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                label.Text = text;
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
                label.IsHideText = value;
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
                label.Height = fontSize;
            }
        }

        [SerializeField, SetProperty("IsChangedDisabledColor")]
        bool isChangedDisabledColor = false;
        public bool IsChangedDisabledColor
        {
            get
            {
                return isChangedDisabledColor;
            }
            set
            {
                isChangedDisabledColor = value;

                if (isChangedDisabledColor && isDisabled)
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
                    if (IsChangedDisabledColor)
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
            }
        }

        // <summary>
        /// 设置Atlas
        /// </summary>
        [SerializeField, SetProperty("Atlas")]
        UIAtlas atlas;
        public UIAtlas Atlas
        {
            get
            {
                return atlas;
            }
            set
            {
                atlas = value;
                bg.atlas = value;
            }
        }


        // <summary>
        /// 设置背景图片
        /// </summary>
        /// <param name="alignment"></param>
        [SerializeField, SetProperty("BgImgName")]
        string bgImgName;
        public string BgImgName
        {
            get
            {
                return bgImgName;
            }
            set
            {
                bgImgName = value;
                bg.spriteName = bgImgName;
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
                isReLayout = true;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            IsReLayoutByChild = true;

            bg = gameObject.transform.Find("bg").GetComponent<UISprite>();
            label = gameObject.transform.Find("Label").GetComponent<Label>();
            label.Parent = this;
            label.BindProcess = null;
            label.DockType = DockType.None;
            label.MatchType = MatchType.None;
            label.CtrlSizeChangeMode = ctrlSizeChangeMode;
            atlas = bg.atlas;
            bgImgName = bg.spriteName;


            UIEventListener.Get(gameObject).onClick = Click;
            UIEventListener.Get(gameObject).onHover = Hover;
            UIEventListener.Get(gameObject).onPress = Press;
        }

        void Click(GameObject go)
        {
            if (isDisabled)
                return;

            if (BindProcess != null)
                BindProcess(this);
        }

        void Hover(GameObject go, bool state)
        {
            if (isDisabled)
                return;

            //if (state == true)
            //    bg.alpha = 0.5f;
            //else
            //    bg.alpha = 1f;
        }

        void Press(GameObject go, bool state)
        {
            if (state == true)
                bg.alpha = 0.5f;
            else
                bg.alpha = 1f;
        }

        protected override void Update()
        {
            base.Update();
   
        }

        protected override void Layout()
        {
            if (ctrlSizeChangeMode == ControlSizeChangeMode.FitContentSize)
            {
                Width = label.Width + (label.Width / 6) * 2;
                Height = label.Height + 2 * (label.Height / 4);
            }
            else
            {
                label.Width = Width -  2*(Width / 6);
                label.Height = Height - 2 * (Height / 6);
            }

            bg.width = Width;
            bg.height = Height;


            Vector3[] worldCorners = WorldCorners;
            float boxPosX = worldCorners[0].x;
            float x = (worldCorners[0].x + worldCorners[3].x) / 2;
            float y = (worldCorners[0].y + worldCorners[1].y) / 2;

            //
            Vector3 pos = new Vector3(x, y, 0);
            bg.transform.position = pos;

            pos.y += textOffsetY / scale;
            label.transform.position = pos;

            baseCollider.size = new Vector3(Width, Height, 0);

        }
    }
}