using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ControlNS
{
    public class ListViewer : Control
    {
        public ListViewerGroup listViewerGroup;

        Vector3 downMousePos;
        Camera cam;
        int state = -1;
        float stateStartTime;
        float stateLiveTime;

        float slideStartTime;
        float slideDuration = 5f;
        float slideCheckStartTime;
        Vector3 slideCheckStartPos;
        float slideDir;
        float averageSpeed = 0;
        float slideAccel = 0.01f;
        float minAccel = 0.001f;
        float maxAccel = 0.05f;

        bool isStartAutoSlide = false;
        bool isStartCalAvgSpeed = false;
        protected override void Awake()
        {
            base.Awake();
            IsReLayoutByChild = true;
            cam = ControlGobal.uiRootCam;
            listViewerGroup = new ListViewerGroup(this);
            UIEventListener.Get(gameObject).onPress = Press;
        }

        public ListViewerItem AddChild(Control control, int height, int depth = 10000, string name = null)
        {

            ListViewerItem viewerItem = Create<ListViewerItem>();
            viewerItem.name = name;
            viewerItem.DockType = DockType.HoriCenter;
            viewerItem.MatchType =MatchType.MatchParentWidth;
            viewerItem.CtrlSizeChangeMode = ControlSizeChangeMode.FixedControlSize;
            viewerItem.Height = height;
            viewerItem.Depth = depth;
            AddItem(viewerItem);
            viewerItem.AddChild(control);

            return viewerItem;
        }

        public void Destroy()
        {
            isReLayout = true;
            listViewerGroup.Destory();   
        }

        public void Press(GameObject go, bool btnState)
        {
            if (btnState == true)
            {
                SetState(0, Time.time, 0);
                isStartAutoSlide = false;
                isStartCalAvgSpeed = true;
                downMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                slideCheckStartPos = downMousePos;
                slideCheckStartTime = Time.time;
                averageSpeed = 0;
            }
            else
            {
                isStartCalAvgSpeed = false;
                SetState(-1, Time.time, 0);
                Vector3 curtMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                float offset = curtMousePos.y - downMousePos.y;

                if (Math.Abs(offset) < 0.0001 || averageSpeed < 1.5f)
                    return;

                SetState(0, Time.time, 0);
                slideAccel = GetAutoSlideAccel(averageSpeed, 15f);       
                slideDir = offset / Math.Abs(offset);
                isStartAutoSlide = true;
                slideStartTime = Time.time;
            }
        }


        void AddItem(ListViewerItem item)
        {
            listViewerGroup.AddItem(item);
        }

        void SetState(int state, float startTime, float liveTime)
        {
            this.state = state;
            stateStartTime = startTime;
            stateLiveTime = liveTime;
        }

        protected override void Layout()
        {
            baseCollider.size = new Vector3(Width, Height, 0);
        }

        protected override void Update()
        {
            base.Update();

            listViewerGroup.Update();

            if (state < 0)
                return;

            if (Time.time - stateStartTime < stateLiveTime)
            {
                return;
            }

            //
            UpdateAverageSpeed();

            //
            float offset = 0;

            if (isStartAutoSlide)
            {
                offset = GetAutoSlideOffset();
                if (offset == 0)
                {
                    isStartAutoSlide = false;
                    state = -1;
                    return;
                }
            }

            switch (state)
            {
                case 0:
                    {
                        if (!isStartAutoSlide)
                        {
                            Vector3 curtMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                            offset = curtMousePos.y - downMousePos.y;
                           
                            if ((offset > 0 && listViewerGroup.CanUpPull() == false) ||
                               (offset < 0 && listViewerGroup.CanDownPull() == false))
                            {
                                downMousePos = curtMousePos;
                                break;
                            }

                            listViewerGroup.SetOffsetY(offset);
                            downMousePos = curtMousePos;
                        }
                        else
                        {
                            if ((offset > 0 && listViewerGroup.CanUpPull() == false) ||
                                (offset < 0 && listViewerGroup.CanDownPull() == false))
                            {
                                break;
                            }

                            listViewerGroup.SetOffsetY(offset);
                        }
                    }
                    break;
            }

        }

        void UpdateAverageSpeed()
        {
            if (isStartCalAvgSpeed == false)
                return;

            float tm = Time.time - slideCheckStartTime;
            if (tm < 0.0001)
            {
                averageSpeed = 0;
                return;
            }

            Vector3 curtMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            float dist = curtMousePos.y - slideCheckStartPos.y;
            averageSpeed = Math.Abs(dist / tm);

            if (averageSpeed < 1.5f)
            {
                slideCheckStartTime = Time.time;
                slideCheckStartPos = curtMousePos;
            }
        }

        float GetAutoSlideOffset()
        {
            float tm = Time.time - slideStartTime;
            if (tm > slideDuration)
                tm = slideDuration;

            float x = tm / slideDuration;
            float offset = -slideAccel * x + slideAccel;
            return offset * slideDir;
        }

        float GetAutoSlideAccel(float speed, float maxSpeed)
        {
            if (speed > maxSpeed)
                speed = maxSpeed;

            float x = speed / maxSpeed;
            float accel = (maxAccel - minAccel) * x + minAccel;
            return accel;
        }

    }
}
