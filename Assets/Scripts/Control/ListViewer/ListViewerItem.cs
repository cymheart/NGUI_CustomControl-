using CommonNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ControlNS
{
    public class ListViewerItem : Control
    {
        public ListViewerGroup listViewerGroup;
        UIWidget itemGroupPanel;
        UISprite bg;
        UIWidget container;

        int removeStyle = 0;

        int state = -1;
        float startTime;
        float duration = 0.1f;
        float expandSize = 0;
        float curtItemExpandHeight = 0;
        float defaultItemHeight;


        public override int Width
        {
            get { return base.Width; }
            set
            {
                base.Width = value;
                bg.width = value;
                container.width = value;
            }
        }

        public override int Height
        {
            get { return base.Height; }
            set
            {
                base.Height = value;
                bg.height = value;
                container.height = value;
            }
        }


        bool isHideBg;
        public bool IsHideBg
        {
            get { return isHideBg; }
            set
            {
                isHideBg = value;
                bg.gameObject.SetActive(!isHideBg);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            bg = transform.Find("Bg").GetComponent<UISprite>();
            container = transform.Find("Container").GetComponent<UIWidget>();
            defaultItemHeight = height;
        }
       
        public virtual void Destroy()
        {
            listViewerGroup.updater.UnReg(GetHashCode());
            state = -1;
            Destroy(gameObject);
        }


        public void AddChild(Control ctrl)
        {
            if (ctrl == null)
                return;

            ctrl.Parent = this;
            ctrl.transform.SetParent(container.transform);
        }

        public void Expand(float expandSize)
        {
            this.expandSize = expandSize;
            curtItemExpandHeight = Height;
            state = 0;
            startTime = Time.time;
            listViewerGroup.updater.Reg(GetHashCode(), Update);
        }

        public void Narrow(float narrowSize)
        {
            this.expandSize = -narrowSize;
            curtItemExpandHeight = Height;

            state = 0;
            startTime = Time.time;
            listViewerGroup.updater.Reg(GetHashCode(), Update);
        }

        public void NarrowDefaultHeight()
        {
            Narrow(Height - defaultItemHeight);
        }

        public void Remove()
        {
            if (removeStyle == 0)
            {
                container.gameObject.SetActive(false);
                bg.gameObject.SetActive(false);
            }

            Narrow(Height);
            state = 1; 
        }

  
        protected override void Update()
        {
            base.Update();

            if (state < 0)
            {
                if (listViewerGroup != null)
                    listViewerGroup.updater.UnReg(GetHashCode());
                return;
            }

            float m = Time.time - startTime;
            float n = Math.Min(1, m / duration);
            float ch = (float)Math.Ceiling(expandSize * n);
            float h = curtItemExpandHeight + ch;

            if (listViewerGroup.itemList.First.Value == this)
            {
                Vector3[] oldCorners = WorldCorners;
                Height = (int)h;
                Vector3[] newCorners = WorldCorners;

                float y = newCorners[1].y - oldCorners[1].y;
                Vector3 pos = transform.position;
                pos.y -= y;
                transform.position = pos;
            }
            else
            {
                Height = (int)h;
            }

            if (n >= 1)
            {
                if (state == 1)
                {
                    listViewerGroup.RemoveItem(this);
                }

                state = -1;
            }
        }

        public Vector2 GetUnitSize()
        {
            float itemWidth = Width / scale;
            float itemHeight = Height / scale;
            return new Vector2(itemWidth, itemHeight);
        }   
    }
}
