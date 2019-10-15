using CommonNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ControlNS
{
    public class ListViewerGroup
    {
        public EventUpdater<ListViewerItem> updater;
        public LinkedList<ListViewerItem> itemList = new LinkedList<ListViewerItem>();
        float spacing = 0.01f;
        public ListViewer listViewer;

        public ListViewerGroup(ListViewer listViewer)
        {
            this.listViewer = listViewer;   
            updater = new EventUpdater<ListViewerItem>();
        }

        public void Destory()
        {
            LinkedListNode<ListViewerItem> node;
            ListViewerItem item;
            for (node = itemList.First; node != null; node = node.Next)
            {
                item = node.Value;
                item.Destroy();
            }

            itemList.Clear();
        }

        public void AddItem(ListViewerItem item)
        {
            if (itemList.Last != null)
            {
                ListViewerItem lastItem = itemList.Last.Value;
                Vector3[] itemCorner = lastItem.WorldCorners;
                float y = itemCorner[0].y - spacing - item.GetUnitSize().y / 2;
                float x = lastItem.transform.position.x;
                item.transform.position = new Vector3(x, y, 0);
            }
            else
            {
                Vector3[] itemCorner = listViewer.WorldCorners;
                float y = itemCorner[1].y - item.GetUnitSize().y / 2;
                float x = listViewer.transform.position.x;
                item.transform.position = new Vector3(x, y, 0);
            }

            item.transform.SetParent(listViewer.transform);
            itemList.AddLast(item);
            item.Parent = listViewer;
            item.listViewerGroup = this;
        }

        public void RemoveItem(ListViewerItem item)
        {
            itemList.Remove(item);
            item.Destroy();
        }

        public void Update()
        {

            if (itemList.Count == 0)
                return;

            updater.Update();

            ListViewerItem item;
            float offsety = 0;
            float m = 0;
            Vector3 pos;
            LinkedListNode<ListViewerItem> node = itemList.First;
            Vector3[] prevItemCorner = node.Value.WorldCorners;
            Vector3[] itemCorner;

            for (node = node.Next; node != null; node = node.Next)
            {
                item = node.Value;
                itemCorner = item.WorldCorners;
                m = Math.Abs(itemCorner[1].y - prevItemCorner[0].y + spacing);

                if (m > 0.001f)
                {
                    offsety = itemCorner[1].y - prevItemCorner[0].y + spacing;
                    pos = item.transform.position;
                    pos.y -= offsety;
                    item.transform.position = pos;
                    prevItemCorner = item.WorldCorners;
                }
                else
                {
                    prevItemCorner = itemCorner;
                }   
            }

            //
            Vector3[] groupWorldCorners = listViewer.WorldCorners;
            ListViewerItem firstItem = itemList.First.Value;
            Vector3[] firstItemCorner = firstItem.WorldCorners;
            float topOffsetY = firstItemCorner[1].y - groupWorldCorners[1].y;

            if(topOffsetY < -0.001f)
            {
                node = itemList.First;
                for (; node != null; node = node.Next)
                {
                    pos = node.Value.transform.position;
                    pos.y -= topOffsetY;
                    node.Value.transform.position = pos;
                }

                return;
            }

            item = itemList.Last.Value;
            itemCorner = item.WorldCorners;
            float bottomOffsetY = itemCorner[0].y - groupWorldCorners[0].y;
            if (bottomOffsetY > 0.001f)
            {
                offsety = Math.Min(topOffsetY, bottomOffsetY);

                node = itemList.First;
                for (; node != null; node = node.Next)
                {
                    pos = node.Value.transform.position;
                    pos.y -= offsety;
                    node.Value.transform.position = pos;
                }
            }

        }

        public bool CanDownPull()
        {
            if (itemList == null || itemList.First == null)
                return false;

            ListViewerItem item = itemList.First.Value;
            Vector3[] headCorners = item.WorldCorners;
            if (headCorners[1].y <= listViewer.WorldCorners[1].y)
                return false;
            return true;
        }


        public bool CanUpPull()
        {
            if (itemList == null || itemList.Last == null)
                return false;
     
            ListViewerItem item = itemList.Last.Value;
            Vector3[] tailCorners = item.WorldCorners;
            if (tailCorners[0].y >= listViewer.WorldCorners[0].y)
                return false;
            return true;
        }

        public void SetOffsetY(float offset, bool isLimitPos = true)
        {
            if (itemList == null || itemList.Count == 0)
                return;

            if (isLimitPos)
            {
                if (offset > 0)
                {
                    ListViewerItem item = itemList.Last.Value;
                    Vector3[] tailCorners = item.WorldCorners;
                    float value = tailCorners[0].y + offset;
                    if (value >= listViewer.WorldCorners[0].y)
                    {
                        offset = listViewer.WorldCorners[0].y - tailCorners[0].y;
                    }
                }
                else if (offset < 0)
                {
                    ListViewerItem item = itemList.First.Value;
                    Vector3[] headCorners = item.WorldCorners;
                    float value = headCorners[1].y + offset;
                    if (value <= listViewer.WorldCorners[1].y)
                    {
                        offset = listViewer.WorldCorners[1].y - headCorners[1].y;
                    }
                }
            }

            if (offset == 0)
                return;

            Vector3 pos;
            LinkedListNode<ListViewerItem> node = itemList.First;
            for (; node != null; node = node.Next)
            {
                pos = node.Value.transform.position;
                pos.y += offset;
                node.Value.transform.position = pos;
            }

        }
    }
}
