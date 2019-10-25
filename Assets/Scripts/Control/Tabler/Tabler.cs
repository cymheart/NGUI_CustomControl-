using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ControlNS
{
    class Tabler : Control
    {
        Table table;
        public Dictionary<int, Tabler> ctableDict = new Dictionary<int, Tabler>();

        public override int Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
                table.Width = value;
            }
        }

        public override int Height
        {
            get { return base.Height; }
            set
            {
                base.Height = value;
                table.Height = value;
            }
        }

        override public DockType DockType
        {
            get
            {
                return dockType;
            }
            set
            {
                dockType = value;
                table.dockType = value;
                isReLayout = true;
            }
        }

        override public MatchType MatchType
        {
            get
            {
                return matchType;
            }
            set
            {
                matchType = value;
                table.matchType = value;
                isReLayout = true;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            table = new Table();
            IsReLayoutByChild = true;
        }

        public void SetTableName(string name)
        {
            table.name = name;
        }

        public void SetDefaultCellMargin( Margin margin)
        {
            table.SetDefaultCellMargin(margin);
        }

        public void SetCellMargin(int rowIdx, int colIdx, Margin margin)
        {
            table.SetCellMargin(rowIdx, colIdx, margin);
        }

        protected override Vector2 FindChildMatchSize(Control child)
        {
            if (child.CtrlSizeChangeMode == ControlSizeChangeMode.FitContentSize)
                return new Vector2(child.Width, child.Height);

            int[] rowcol = GetControlRowCol(child);
            if (rowcol == null)
                return new Vector2(-1, -1);

            RectangleF rect = table.GetCellContentRect(rowcol[0], rowcol[1]);
            rect = table.TransToGlobalRect(rect);

            switch (child.MatchType)
            {
                case MatchType.MatchParentSize:
                    return new Vector2(rect.Width, rect.Height);

                case MatchType.MatchParentWidth:
                    return new Vector2(rect.Width, -1);

                case MatchType.MatchParentHeight:
                    return new Vector2(-1, rect.Height);
                default:
                    return new Vector2(-1, -1);
            }
        }

        protected override Vector3 FindChildDockPosition(Control child)
        {
            int[] rowcol = GetControlRowCol(child);
            if (rowcol == null)
                return child.transform.position;

            RectangleF rect = table.GetCellContentRect(rowcol[0], rowcol[1]);
            RectangleF tableRect = table.GetTableRect();
            float xstart = -tableRect.Width / 2;
            float ystart = -tableRect.Height / 2;
            rect.X += xstart;
            rect.Y += ystart;
            float x, y;

            switch (child.DockType)
            {
                case DockType.Center:
                    x = (rect.Left + rect.Right) / 2;
                    y = (rect.Top + rect.Bottom) / 2;
                    return  new Vector3(x, y, 0);

                case DockType.HoriCenter:
                    x = (rect.Left + rect.Right) / 2;
                    return new Vector3(x, child.transform.localPosition.y, 0);

                case DockType.VertCenter:
                    y = (rect.Top + rect.Bottom) / 2;
                    return new Vector3(child.transform.localPosition.x, y, 0);

                case DockType.Left:
                    x = rect.Left + child.Width / 2;
                    y = (rect.Top + rect.Bottom) / 2;
                    return new Vector3(x, y, 0);

                default:
                    return child.transform.localPosition;
            }

        }

        int[] GetControlRowCol(Control ctrl)
        {
            TableCellData cellData;
            foreach (var item in table.cellValueInfoDict)
            {
                cellData = item.Value;

                if (cellData.type != TableCellDataType.Table)
                {
                    Control curtCtrl = (Control)item.Value.data;

                    if (curtCtrl == ctrl)
                    {
                        int[] rowcol = new int[2];
                        table.GetKeyRowCol(item.Key, rowcol);
                        return rowcol;
                    }
                }
                else if(ctrl.GetType() == typeof(Tabler))
                {
                    Tabler tabler = (Tabler)ctrl;
                    Table curtTable = (Table)item.Value.data;

                    if (curtTable == tabler.table)
                    {
                        int[] rowcol = new int[2];
                        table.GetKeyRowCol(item.Key, rowcol);
                        return rowcol;
                    }
                }
            }

            return null;
        }

        public void EnableTableLineAutoAdjustRichSize(int idx, bool isEnable, LineDir lineDir)
        {
            table.EnableTableLineAutoAdjustRichSize(idx, isEnable, lineDir);
        }

        public void EnableTableAllLineAutoAdjustRichSize(bool isEnable, LineDir lineDir)
        {
            table.EnableTableAllLineAutoAdjustRichSize(isEnable, lineDir);
        }

        public void EnableTableAllLineAutoAdjustRichSize(bool isEnable)
        {
            table.EnableTableAllLineAutoAdjustRichSize(isEnable, LineDir.HORIZONTAL);
            table.EnableTableAllLineAutoAdjustRichSize(isEnable, LineDir.VERTICAL);
        }

        public void AddChild(Control control, int row, int col)
        {
            if (row >= table.rowAmount)
            {
                for (int i = table.rowAmount; i < row + 1; i++)
                    table.AddNewAutoRow();
            }

            if (col >= table.colAmount)
            {
                for (int i = table.colAmount; i < col + 1; i++)
                    table.AddNewAutoCol();
            }

            if (control == null)
                return;

            control.transform.SetParent(transform);
            control.Parent = this;

            TableCellData celldata;
            Type type = control.GetType();
            if (type == typeof(Tabler))
            {
                Tabler tbr = control as Tabler;
                int key = table.GetRowColKey(row, col);
                ctableDict[key] = tbr;
                table.AddCellChildTable(row, col, tbr.table);
            }
            else
            {
                celldata = new TableCellData();
                celldata.type = TableCellDataType.FixedSize;
                celldata.data = control;
                celldata.rowIdx = row;
                celldata.colIdx = col;
                celldata.GetLayoutSize = GetLayoutSize;
                table.SetCellValue(row, col, celldata);
            }

        }

        public float GetLayoutSize(TableCellData cellData, LineDir lineDir)
        {
            Control ctrl = (Control)cellData.data;
            if (lineDir == LineDir.VERTICAL)
            {
                return ctrl.Width;
            }

            return ctrl.Height;
        }

        protected override void Layout()
        {
            table.ReLayout();
            SetSize((int)table.Width, (int)table.Height);
        }

    }

}
