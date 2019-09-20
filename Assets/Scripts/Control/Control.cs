using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ControlNS
{
    public class Control : MonoBehaviour
    {
        public delegate void BindControlDelegate(Control control);
        public BindControlDelegate BindProcess;


        private UIWidget baseWidget;
        private UIPanel basePanel;
        private int oldWidth = 0;
        private int oldHeight = 0;
        protected BoxCollider baseCollider;
        protected bool isReLayout = false;

        [HideInInspector]
        public bool IsNotReLayout = false;

        [HideInInspector]
        public bool IsChildReLayout = false;

        [HideInInspector]
        public bool IsReLayoutByChild = false;

        public string name = "control";
        
        protected float scale;

        protected List<Control> childrens = new List<Control>(); 

        [SerializeField, SetProperty("CtrlSizeChangeMode")]
        protected ControlSizeChangeMode ctrlSizeChangeMode = ControlSizeChangeMode.FixedControlSize;
        virtual public ControlSizeChangeMode CtrlSizeChangeMode
        {
            get
            {
                return ctrlSizeChangeMode;
            }
            set
            {
                ctrlSizeChangeMode = value;
                isReLayout = true;
            }
        }


        protected DockType dockType = DockType.Center;
        virtual public DockType DockType
        {
            get
            {
                return dockType;
            }
            set
            {
                dockType = value;
                isReLayout = true;
            }
        }

        protected MatchType matchType = MatchType.MatchParentSize;
        virtual public MatchType MatchType
        {
            get
            {
                return matchType;
            }
            set
            {
                matchType = value;
                isReLayout = true;
            }
        }


        protected int width;
        public virtual int Width
        {
            get
            {
                if (baseWidget != null)
                    return baseWidget.width;
                return (int)basePanel.width;
            }
            set
            {
                width = value;

                if (baseWidget != null)
                {
                    baseWidget.width = value;
                }
                else
                {
                    Vector4 region = basePanel.baseClipRegion;
                    region.z = value;
                    basePanel.baseClipRegion = region;
                }
            }
        }

        protected int height;
        public virtual int Height
        {
            get
            {
                if (baseWidget != null)
                    return baseWidget.height;
                return (int)basePanel.height;
            }
            set
            {
                height = value;

                if (baseWidget != null)
                {
                    baseWidget.height = value;
                }
                else
                {
                    Vector4 region = basePanel.baseClipRegion;
                    region.w = value;
                    basePanel.baseClipRegion = region;
                }
            }
        }


        protected Control parent;
        public Control Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
                parent.childrens.Add(this);
            }
        }


        public int Depth
        {
            get
            {
                if (baseWidget != null)
                {
                    return baseWidget.depth;
                }
                else
                {
                    return basePanel.depth;
                }
            }
            set
            {

                if (baseWidget != null)
                {
                    baseWidget.depth = value;
                }
                else
                {
                    basePanel.depth = value;
                }
            }
        }


        public Vector3[] WorldCorners
        {
            get
            {
                Vector3[] mCorners = new Vector3[4];
                Vector3[] worldCorners;
                if (baseWidget != null)
                    worldCorners = baseWidget.worldCorners;
                else
                    worldCorners = basePanel.worldCorners;

                mCorners[0] = new Vector3(worldCorners[0].x, worldCorners[0].y);
                mCorners[1] = new Vector3(worldCorners[1].x, worldCorners[1].y);
                mCorners[2] = new Vector3(worldCorners[2].x, worldCorners[2].y);
                mCorners[3] = new Vector3(worldCorners[3].x, worldCorners[3].y);

                return mCorners;
            }
        }

        protected virtual void Awake()
        {
            Vector3[] worldCorners;
            baseWidget = gameObject.GetComponent<UIWidget>();

            if (baseWidget != null)
            {
                worldCorners = baseWidget.worldCorners;
                baseCollider = baseWidget.GetComponent<BoxCollider>();
                float unitWidth = worldCorners[3].x - worldCorners[0].x;
                scale = baseWidget.width / unitWidth;
            }
            else
            {
                basePanel = gameObject.GetComponent<UIPanel>();
                worldCorners = basePanel.worldCorners;
                baseCollider = basePanel.GetComponent<BoxCollider>();
                float unitWidth = worldCorners[3].x - worldCorners[0].x;
                scale = basePanel.width / unitWidth;
            }
        }

        static public T Create<T>()
        {
            Type type = typeof(T);
            return ControlGobal.CreateCtrl(type.Name).GetComponent<T>();
        }
        private void ReLayout()
        {
            if (IsNotReLayout == true)
                return;

            if (IsChildReLayout == false &&
              isReLayout == false &&
              Width == oldWidth &&
              Height == oldHeight)
                return;

            Layout();

            oldWidth = Width;
            oldHeight = Height;

            isReLayout = false;
            IsChildReLayout = false;
            
            //
            if (parent != null && parent.IsReLayoutByChild == true)
            {
                parent.IsChildReLayout = true;
                parent.ReLayout();
            }

            ReLayoutChildrens();    
        }

        void ReLayoutChildrens()
        {    
            for(int i=0; i<childrens.Count; i++)
            {
                if (childrens[i] == null)
                    continue;

                childrens[i].SetComputedSizeAndPosition();
                childrens[i].ReLayout();
            }

        }

        protected virtual void Layout()
        {

        }

      
        protected virtual void Update()
        {
            ReLayout();
        }

        public virtual void SetSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        protected virtual Vector3 FindChildDockPosition(Control child)
        {
            switch(child.DockType)
            {
                case DockType.Center:
                    return transform.position;

                case DockType.HoriCenter:
                    return new Vector3(transform.position.x, child.transform.position.y, 0);

                case DockType.VertCenter:
                    return new Vector3(child.transform.position.x, transform.position.y, 0);

                default:
                    return child.transform.position;
            }
        }

        protected virtual Vector2 FindChildMatchSize(Control child)
        {
            if (child.CtrlSizeChangeMode == ControlSizeChangeMode.FitContentSize)
                return new Vector2(child.Width, child.Height);

            switch (child.matchType)
            {
                case MatchType.MatchParentSize:
                    return new Vector2(Width, Height);

                case MatchType.MatchParentWidth:
                    return new Vector2(Width, child.Height);

                case MatchType.MatchParentHeight:
                    return new Vector2(child.Width, Height);

                default:
                    return new Vector2(child.Width, child.Height);
            }
        }

        protected virtual void SetComputedSizeAndPosition()
        {
            Vector2 size = parent.FindChildMatchSize(this);
            SetSize((int)size.x, (int)size.y);
            Vector3 pos = parent.FindChildDockPosition(this);
            transform.position = pos;
        }

        static public int GetTextRenderWidth(string txt, Font font, int fontSize, FontStyle fontStyle)
        {
            if (txt == null)
                return 0;

            int width = 0;
            font.RequestCharactersInTexture(txt, fontSize, fontStyle);

            CharacterInfo cinfo;
            for (int i = 0; i < txt.Length; i++)
            {
                font.GetCharacterInfo(txt[i], out cinfo, fontSize, fontStyle);
                width += cinfo.advance;
            }

            return width;
        }
    }
}
