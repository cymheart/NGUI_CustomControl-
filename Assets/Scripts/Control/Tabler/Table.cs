using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ControlNS
{
    public class Table
    {
        public string name;

        Vector2 tbPos;

        float tbWidth;
        public float Width
        {
            get { return tbWidth; }
            set { tbWidth = value; }
        }

        float tbHeight;
        public float Height
        {
            get { return tbHeight; }
            set { tbHeight = value; }
        }

        public DockType dockType;
        public MatchType matchType;

        public int rowAmount { get; private set; }
        public int colAmount { get; private set; }

        Margin defaultCellMargin = new Margin(0, 0, 0, 0);
        Dictionary<int, Margin> cellMarginDict = new Dictionary<int, Margin>();
        Dictionary<int, SpanOffset> cellSpanDict = new Dictionary<int, SpanOffset>();
        public Dictionary<int, TableCellData> cellValueInfoDict = new Dictionary<int, TableCellData>();

        public Table parentTable;
        CellCoord inParentCellCoord;

        List<TableLine> rowLineList = new List<TableLine>();
        List<TableLine> colLineList = new List<TableLine>();

        public Table(float x, float y, float tbWidth, float tbHeight, int rowAmount = 0, int colAmount = 0)
        {
            this.tbPos.x = x;
            this.tbPos.y = y;
            this.tbHeight = tbHeight;
            this.tbWidth = tbWidth;

            this.rowAmount = rowAmount;
            this.colAmount = colAmount;

            //
            TableLine tableLine;
            float rowComputedDist = tbHeight / rowAmount;
            float rowPercent = 1f / rowAmount;

            for (int i = 0; i < rowAmount; i++)
            {
                tableLine = new TableLine(LineDir.HORIZONTAL);
                tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;
                tableLine.computeParam = rowPercent;
                tableLine.computedDistance = rowComputedDist;
                tableLine.maxComputedValue = -1;
                tableLine.minComputedValue = -1;
                rowLineList.Add(tableLine);
            }

            //
            float colComputedDist = tbWidth / colAmount;
            float colPercent = 1f / colAmount;

            for (int i = 0; i < colAmount; i++)
            {
                tableLine = new TableLine(LineDir.VERTICAL);
                tableLine.lineComputeMode = LineComputeMode.PERCENTAGE;
                tableLine.computeParam = colPercent;
                tableLine.computedDistance = colComputedDist;
                tableLine.maxComputedValue = -1;
                tableLine.minComputedValue = -1;
                colLineList.Add(tableLine);
            }
        }

        public Table(RectangleF tbRect, int rowAmount = 0, int colAmount = 0)
           : this(tbRect.X, tbRect.Y, tbRect.Width, tbRect.Height, rowAmount, colAmount)
        {

        }

        public Table(int rowAmount = 0, int colAmount = 0)
        {
            tbPos.x = 0;
            tbPos.y = 0;
            tbHeight = 0;
            tbWidth = 0;

            this.rowAmount = rowAmount;
            this.colAmount = colAmount;

            matchType = MatchType.MatchParentSize;

            TableLine tableLine;

            for (int i = 0; i < rowAmount; i++)
            {
                tableLine = new TableLine(LineDir.HORIZONTAL);
                tableLine.lineComputeMode = LineComputeMode.AUTO;
                tableLine.maxComputedValue = -1;
                tableLine.minComputedValue = -1;
                rowLineList.Add(tableLine);
            }

            //
            for (int i = 0; i < colAmount; i++)
            {
                tableLine = new TableLine(LineDir.VERTICAL);
                tableLine.lineComputeMode = LineComputeMode.AUTO;
                tableLine.maxComputedValue = -1;
                tableLine.minComputedValue = -1;
                colLineList.Add(tableLine);
            }
        }


        public TableLine GetTableLine(int idx, LineDir lineDir)
        {
            List<TableLine> lineList;

            if (lineDir == LineDir.HORIZONTAL)
                lineList = rowLineList;
            else
                lineList = colLineList;

            if (idx >= lineList.Count)
                return null;

            return lineList[idx];
        }

        public void EnableTableLineAutoAdjustRichSize(int idx, bool isEnable, LineDir lineDir)
        {
            List<TableLine> lineList;
            int amount;

            if (lineDir == LineDir.HORIZONTAL)
            {
                lineList = rowLineList;
                amount = rowAmount;
            }
            else
            {
                lineList = colLineList;
                amount = colAmount;
            }

            if (idx < lineList.Count)
            { 
                lineList[idx].enableAutoAdjustRichSize = isEnable;
            }
        }

        public void EnableTableAllLineAutoAdjustRichSize(bool isEnable, LineDir lineDir)
        {
            List<TableLine> lineList;
            int amount;

            if (lineDir == LineDir.HORIZONTAL)
            {
                lineList = rowLineList;
                amount = rowAmount;
            }
            else
            {
                lineList = colLineList;
                amount = colAmount;
            }

            for(int i=0; i<lineList.Count; i++)
                lineList[i].enableAutoAdjustRichSize = isEnable;
        }


        public void SetTableLine(int idx, TableLine tableLine)
        {
            List<TableLine> lineList;
            int amount;

            if (tableLine.lineDir == LineDir.HORIZONTAL)
            {
                lineList = rowLineList;
                amount = rowAmount;
            }
            else
            {
                lineList = colLineList;
                amount = colAmount;
            }

            if (idx < lineList.Count)
            {
                TableLine tl = new TableLine(tableLine.lineDir);
                tl.computeParam = tableLine.computeParam;
                tl.computedDistance = tableLine.computedDistance;
                tl.lineComputeMode = tableLine.lineComputeMode;
                tl.minComputedValue = tableLine.minComputedValue;
                tl.maxComputedValue = tableLine.maxComputedValue;
                lineList[idx] = tl;
            }
            else
            {
                int n = idx - lineList.Count;
                for (int i = 0; i < n; i++)
                {
                    TableLine tl = new TableLine(tableLine.lineDir);
                    tl.computeParam = tableLine.computeParam;
                    tl.computedDistance = tableLine.computedDistance;
                    tl.lineComputeMode = tableLine.lineComputeMode;
                    tl.minComputedValue = tableLine.minComputedValue;
                    tl.maxComputedValue = tableLine.maxComputedValue;
                    lineList.Add(tl);
                    amount++;
                }

                if (tableLine.lineDir == LineDir.HORIZONTAL)
                {
                    rowAmount = amount;
                }
                else
                {
                    colAmount = amount;
                }
            }
        }

        public void SetAutoTableLine(int idx, LineDir lineDir)
        {
            TableLine tableLine = new TableLine(lineDir);
            tableLine.lineComputeMode = LineComputeMode.AUTO;
            tableLine.maxComputedValue = -1;
            tableLine.minComputedValue = -1;
            SetTableLine(idx, tableLine);
        }

        public Vector2 GetTablePosition()
        {
            return tbPos;
        }
        public void SetTablePosition(float x, float y)
        {
            this.tbPos.x = x;
            this.tbPos.y = y;
        }

        public void SetTableSize(float w, float h)
        {
            tbWidth = w;
            tbHeight = h;
        }

        public int GetRowColKey(int rowIdx, int colIdx)
        {
            return (rowIdx << 12) | colIdx;
        }

        public int[] GetKeyRowCol(int key)
        {
            int[] rowcol = new int[2];
            rowcol[0] = key >> 12;
            rowcol[1] = key & 0xfff;
            return rowcol;
        }

        public void GetKeyRowCol(int key, int[] outRowCol)
        {
            outRowCol[0] = key >> 12;
            outRowCol[1] = key & 0xfff;
        }

        public void SetDefaultCellMargin(Margin cellMargin)
        {
            defaultCellMargin.left = cellMargin.left;
            defaultCellMargin.top = cellMargin.top;
            defaultCellMargin.right = cellMargin.right;
            defaultCellMargin.bottom = cellMargin.bottom;
        }

        public void SetCellMargin(int rowIdx, int colIdx, Margin margin)
        {
            int key = GetRowColKey(rowIdx, colIdx);

            if (cellMarginDict.ContainsKey(key) == false)
            {
                cellMarginDict.Add(key, margin);
            }

            cellMarginDict[key] = margin;
        }

        public Margin GetCellMargin(int rowIdx, int colIdx)
        {
            int key = GetRowColKey(rowIdx, colIdx);

            if (cellMarginDict.ContainsKey(key) == false)
            {
                return defaultCellMargin;
            }

            return cellMarginDict[key];
        }


        public void SetCellSpan(int rowIdx, int colIdx, SpanOffset span)
        {
            int key = GetRowColKey(rowIdx, colIdx);

            if (cellSpanDict.ContainsKey(key) == false)
            {
                cellSpanDict.Add(key, span);
            }

            cellSpanDict[key] = span;

        }

        public SpanOffset? GetCellSpan(int rowIdx, int colIdx)
        {
            int key = GetRowColKey(rowIdx, colIdx);

            if (cellSpanDict.ContainsKey(key) == false)
            {
                return null;
            }
            return cellSpanDict[key];
        }

        public void SetLinesComputedValueRange(int idx, float minValue, float maxValue, LineDir lineDir)
        {
            if (lineDir == LineDir.HORIZONTAL)
            {
                rowLineList[idx].minComputedValue = minValue;
                rowLineList[idx].maxComputedValue = maxValue;
            }
            else
            {
                colLineList[idx].minComputedValue = minValue;
                colLineList[idx].maxComputedValue = maxValue;
            }
        }

        public void SetCellValue(int rowIdx, int colIdx, TableCellData cellData)
        {
            int key = GetRowColKey(rowIdx, colIdx);

            if (cellValueInfoDict.ContainsKey(key) == false)
            {
                cellValueInfoDict.Add(key, cellData);
            }
            else
            {
                cellValueInfoDict[key] = cellData;
            }
        }

        public void SetCellFixedSizeValue(int rowIdx, int colIdx, Vector2 fixedSize)
        {
            TableCellData cellData;
            cellData = new TableCellData();
            cellData.type = TableCellDataType.FixedSize;
            cellData.fixedSize = fixedSize;

            int key = GetRowColKey(rowIdx, colIdx);

            if (cellValueInfoDict.ContainsKey(key) == false)
            {
                cellValueInfoDict.Add(key, cellData);
            }
            else
            {
                cellValueInfoDict[key] = cellData;
            }
        }

        public void AddCellChildTable(int rowIdx, int colIdx, Table childTable)
        {
            TableCellData cellData;
            cellData = new TableCellData();
            cellData.type = TableCellDataType.Table;
            cellData.data = childTable;
            cellData.rowIdx = rowIdx;
            cellData.colIdx = colIdx;

            int key = GetRowColKey(rowIdx, colIdx);

            if (cellValueInfoDict.ContainsKey(key) == false)
            {
                cellValueInfoDict.Add(key, cellData);
            }
            else
            {
                cellValueInfoDict[key] = cellData;
            }

            childTable.parentTable = this;
            childTable.inParentCellCoord = new CellCoord(rowIdx, colIdx);
        }

        public Table GetCellChildTable(int rowIdx, int colIdx)
        {
            int key = GetRowColKey(rowIdx, colIdx);

            if (cellValueInfoDict.ContainsKey(key) == false)
            {
                return null;
            }
            else
            {
                TableCellData cellData;
                cellData = cellValueInfoDict[key];

                if (cellData.type == TableCellDataType.Table)
                    return (Table)cellData.data;

                return null;
            }
        }


        public TableCellData GetCellValueInfo(int rowIdx, int colIdx)
        {
            int key = GetRowColKey(rowIdx, colIdx);

            if (cellValueInfoDict.ContainsKey(key) == false)
            {
                return null;
            }
            else
            {
                return cellValueInfoDict[key];
            }
        }


        public void ReLayout()
        {
            ModifyTableDockAndMatch(dockType);
            LayoutLinesArea(LineDir.HORIZONTAL);
            LayoutLinesArea(LineDir.VERTICAL);

            Table childTable;
            foreach (var item in cellValueInfoDict)
            {
                if (item.Value.type == TableCellDataType.Table)
                {
                    childTable = (Table)item.Value.data;
                    childTable.ReLayout();
                }
            }
        }

        private void LayoutLinesArea(LineDir lineDir)
        {
            TableLine line;
            List<TableLine> lineList = null;

            //表行或列方向上的长度
            float tbLength;  

            if (lineDir == LineDir.HORIZONTAL)
            {
                lineList = rowLineList;
                tbLength = tbHeight;
            }
            else
            {
                lineList = colLineList;
                tbLength = tbWidth;
            }

            
            float usedLength = 0;
            float percent = 0;
            float tbRichHeight;
            float tbPerRichPercent;
            float tbPerRichHeight;
            int percentCount = 0;

            //自动调整富余尺寸的lines个数
            int autoCount = 0;

          
            for (int i = 0; i < lineList.Count; i++)
            {
                line = lineList[i];

                switch (line.lineComputeMode)
                {
                    case LineComputeMode.AUTO:
                        line.computeParam = ComputeLineAreaSize(i, lineDir);
                        usedLength += line.computeParam;
                        line.computedDistance = line.computeParam;

                        if (line.enableAutoAdjustRichSize)
                            autoCount++;

                        break;

                    case LineComputeMode.ABSOLUTE:
                        usedLength += line.computeParam;
                        line.computedDistance = line.computeParam;
                        ModifyTableSize(lineDir, line, ref usedLength, ref tbLength);
                        break;

                    case LineComputeMode.PERCENTAGE:
                        percentCount++;
                        float n = percent + line.computeParam;

                        if (n > 1f + 0.001f)
                        {
                            line.computeParam = 1f - percent;
                            percent = 1f;

                            for (int j = i + 1; j < lineList.Count; j++)
                            {
                                percentCount++;
                                line = lineList[j];
                                line.computeParam = 0;
                                line.computedDistance = 0;
                            }

                            break;
                        }
                        else
                        {
                            percent = n;
                        }
                        break;
                }
            }


            //计算表尺寸
            //根据使用的长度即为表的长度
            if (tbLength == 0 &&
                (matchType == MatchType.MatchParentSize ||
                (matchType == MatchType.MatchParentHeight && lineDir == LineDir.HORIZONTAL) ||
                (matchType == MatchType.MatchParentWidth && lineDir == LineDir.VERTICAL)))
            {
                tbLength = usedLength;
                if (lineDir == LineDir.HORIZONTAL)
                    tbHeight = usedLength;
                else
                    tbWidth = usedLength;
            }

            //分配未使用的余量长度
            //如果存在有百分比的lines,将把余量(tbRichHeight)平均分配到这些lines上
            //否则如果存在自动调整尺寸的lines,将把余量(tbRichHeight)平均分配到这些lines上
            if (percentCount > 0)
            {
                tbRichHeight = tbLength - usedLength;
                tbPerRichPercent = (1f - percent) / percentCount;

                for (int i = 0; i < lineList.Count; i++)
                {
                    line = lineList[i];

                    if (line.lineComputeMode == LineComputeMode.PERCENTAGE)
                    {
                        line.computeParam += tbPerRichPercent;
                        line.computedDistance = tbRichHeight * line.computeParam;
                    }
                }
            }
            else if (autoCount > 0)
            {
                tbRichHeight = tbLength - usedLength;
                tbPerRichHeight = tbRichHeight / autoCount;

                float totalScaleHeight = 0;
                float totalParamHeight = 0;

                for (int i = 0; i < lineList.Count; i++)
                {
                    line = lineList[i];

                    if (line.lineComputeMode == LineComputeMode.AUTO && line.enableAutoAdjustRichSize == true)
                    {
                        if (line.computeParam == 0)
                        {
                            line.computeParam = tbPerRichHeight;
                            totalScaleHeight += tbPerRichHeight;
                        }
                        else
                        {
                            totalScaleHeight += line.computeParam;
                            totalParamHeight += line.computeParam;
                        }
                    }
                }

                float realTotalHeight = totalParamHeight + tbRichHeight;

                for (int i = 0; i < lineList.Count; i++)
                {
                    line = lineList[i];

                    if (line.lineComputeMode == LineComputeMode.AUTO &&
                        line.enableAutoAdjustRichSize == true)
                    {
                        line.computeParam = line.computeParam / totalScaleHeight * realTotalHeight;
                        line.computedDistance = line.computeParam;
                    }
                }
            }
        }

        void ModifyTableDockAndMatch(DockType dockType)
        {
            RectangleF rect = new RectangleF(tbPos.x, tbPos.y, tbWidth, tbHeight);
            if (parentTable != null)
            {
                rect = parentTable.GetCellContentRect(inParentCellCoord.rowIdx, inParentCellCoord.colIdx);
            }

            switch (matchType)
            {
                case MatchType.MatchParentSize:
                    tbHeight = rect.Height;
                    tbWidth = rect.Width;
                    break;

                case MatchType.MatchParentWidth:
                    tbWidth = rect.Width;
                    break;

                case MatchType.MatchParentHeight:
                    tbHeight = rect.Height;
                    break;
            }

            switch (dockType)
            {
                case DockType.Center:
                    tbPos.x = rect.Width / 2 - tbWidth / 2;
                    tbPos.y = rect.Height / 2 - tbHeight / 2;
                    return;

                case DockType.HoriCenter:
                    tbPos.x = rect.Width / 2 - tbWidth / 2;
                    return;

                case DockType.VertCenter:
                    tbPos.y = rect.Height / 2 - tbHeight / 2;
                    return;

                case DockType.Left:
                    tbPos.x = 0;
                    tbPos.y = rect.Height / 2 - tbHeight / 2;
                    return;

                case DockType.LeftTop:
                    tbPos.x = 0;
                    tbPos.y = 0;
                    return;

                case DockType.Top:
                    tbPos.x = rect.Width / 2 - tbWidth / 2;
                    tbPos.y = 0;
                    return;

                case DockType.RightTop:
                    tbPos.x = rect.Width - tbWidth;
                    tbPos.y = 0;
                    return;

                case DockType.Right:
                    tbPos.x = rect.Width - tbWidth;
                    tbPos.y = rect.Height / 2 - tbHeight / 2;
                    return;

                case DockType.RightBottom:
                    tbPos.x = rect.Width - tbWidth;
                    tbPos.y = rect.Height - tbHeight;
                    return;

                case DockType.Bottom:
                    tbPos.x = rect.Width / 2 - tbWidth / 2;
                    tbPos.y = rect.Height - tbHeight;
                    return;

                case DockType.LeftBottom:
                    tbPos.x = 0;
                    tbPos.y = rect.Height - tbHeight;
                    return;
            }
        }


        void ModifyTableSize(LineDir lineDir, TableLine line, ref float usedLength, ref float tbLength)
        {
            if (matchType == MatchType.MatchParentSize ||
               (matchType == MatchType.MatchParentHeight && lineDir == LineDir.HORIZONTAL) ||
               (matchType == MatchType.MatchParentWidth && lineDir == LineDir.VERTICAL))
            {
                return;
            }
            else if (usedLength > tbLength)
            {
                usedLength -= line.computeParam;
                line.computeParam = tbLength - usedLength;
                usedLength += line.computeParam;
                line.computedDistance = line.computeParam;
            }
        }

        float ComputeLineAreaSize(int idx, LineDir lineDir)
        {
            Margin margin;
            Table ctable;
            float maxSize = 0;
            float size = 0;
            TableCellData cellData;
            int amount = 0;
            int rowIdx, colIdx;
            Vector2 fixedSize;

            if (lineDir == LineDir.HORIZONTAL)
                amount = colAmount;
            else
                amount = rowAmount;

            for (int i = 0; i < amount; i++)
            {
                if (lineDir == LineDir.HORIZONTAL)
                {
                    rowIdx = idx;
                    colIdx = i;
                }
                else
                {
                    rowIdx = i;
                    colIdx = idx;
                }

                margin = GetCellMargin(rowIdx, colIdx);
                cellData = GetCellValueInfo(rowIdx, colIdx);

                if (cellData == null)
                {
                    if (lineDir == LineDir.HORIZONTAL)
                        size = margin.top + margin.bottom;
                    else
                        size = margin.left + margin.right;
                }
                else
                {
                    switch (cellData.type)
                    {    
                        case TableCellDataType.FixedSize:

                            if (cellData.GetLayoutSize != null)
                            {
                                size = cellData.GetLayoutSize(cellData, lineDir);

                                if (lineDir == LineDir.HORIZONTAL)
                                    size += margin.top + margin.bottom;
                                else
                                    size += margin.left + margin.right;
                            }
                            else
                            {
                                fixedSize = cellData.fixedSize;

                                if (lineDir == LineDir.HORIZONTAL)
                                    size = fixedSize.y + margin.top + margin.bottom;
                                else
                                    size = fixedSize.x + margin.left + margin.right;
                            }
                            break;

                        case TableCellDataType.Table:
                            ctable = (Table)cellData.data;

                            if (ctable == null)
                                continue;

                            if (cellData.GetLayoutSize != null)
                            {
                                size = cellData.GetLayoutSize(cellData, lineDir);
                                if (lineDir == LineDir.HORIZONTAL)
                                    size += margin.top + margin.bottom;
                                else
                                    size += margin.left + margin.right;
                            }
                            else
                            {
                                if (lineDir == LineDir.HORIZONTAL)
                                {
                                    if (ctable.tbHeight == 0)
                                        ctable.ReLayout();
                                    size = ctable.tbHeight + margin.top + margin.bottom;
                                }
                                else
                                {
                                    if (ctable.tbWidth == 0)
                                        ctable.ReLayout();
                                    size = ctable.tbWidth + margin.left + margin.right;
                                }
                            }
                            break;
                    }
                }

                maxSize = Math.Max(maxSize, size);
            }

            //
            return GetLimitLength(maxSize, idx, lineDir);
        }

        float GetLimitLength(float orgLen, int idx, LineDir lineDir)
        {
            //
            float len = orgLen;
            float min, max;

            if (lineDir == LineDir.HORIZONTAL)
            {
                min = rowLineList[idx].minComputedValue;
                max = rowLineList[idx].maxComputedValue;
            }
            else
            {
                min = colLineList[idx].minComputedValue;
                max = colLineList[idx].maxComputedValue;
            }

            if (orgLen < min && min != -1)
                len = min;
            else if (orgLen > max && max != -1)
                len = max;

            return len;
        }


        public void SetLineArea(int lineIdx, TableLine setLine)
        {
            List<TableLine> lineList = null;
            float tbLength;

            if (setLine.lineDir == LineDir.HORIZONTAL)
            {
                lineList = rowLineList;
                tbLength = tbHeight;
            }
            else
            {
                lineList = colLineList;
                tbLength = tbWidth;
            }

            TableLine adjustLine = lineList[lineIdx];
            adjustLine.lineComputeMode = setLine.lineComputeMode;
            adjustLine.computeParam = setLine.computeParam;
            adjustLine.enableAutoAdjustRichSize = setLine.enableAutoAdjustRichSize;
            adjustLine.lineDir = setLine.lineDir;
            adjustLine.minComputedValue = setLine.minComputedValue;
            adjustLine.maxComputedValue = setLine.maxComputedValue;
        }

        public TableLine GetRowTableLine(int idx)
        {
            if (idx >= rowLineList.Count)
                idx = rowLineList.Count - 1;
            else if (idx < 0)
                idx = 0;

            return rowLineList[idx];
        }

        public TableLine GetColTableLine(int idx)
        {
            if (idx >= colLineList.Count)
                idx = colLineList.Count - 1;
            else if (idx < 0)
                idx = 0;

            return colLineList[idx];
        }

        public Vector2 TransToGlobalPoint(Vector2 pt)
        {
            RectangleF rect;
            float x = pt.x, y = pt.y;
            CellCoord inCellCoord = inParentCellCoord;

            x += tbPos.x;
            y += tbPos.y;

            for (Table tb = parentTable; tb != null; tb = tb.parentTable)
            {
                rect = tb.GetCellContentRect(inCellCoord.rowIdx, inCellCoord.colIdx);
                x += rect.X + tb.tbPos.x;
                y += rect.Y + tb.tbPos.y;
                inCellCoord = tb.inParentCellCoord;
            }

            return new Vector2(x, y);
        }

        public RectangleF TransToGlobalRect(RectangleF rect)
        {
            RectangleF r;
            CellCoord inCellCoord = inParentCellCoord;
            float x = rect.X, y = rect.Y;

            x += tbPos.x;
            y += tbPos.y;

            for (Table tb = parentTable; tb != null; tb = tb.parentTable)
            {
                r = tb.GetCellContentRect(inCellCoord.rowIdx, inCellCoord.colIdx);
                x += r.X + tb.tbPos.x;
                y += r.Y + tb.tbPos.y;
                inCellCoord = tb.inParentCellCoord;
            }

            RectangleF dstRect = new RectangleF(x, y, rect.Width, rect.Height);

            return dstRect;
        }

        public LinePos GetRowLinePos(int rowIdx, DirectionMode dir = DirectionMode.BOTTOM_OR_RIGHT)
        {
            RectangleF rect = GetOriginalCellRect(rowIdx, 0);

            LinePos linePos = new LinePos();
            Vector2 start, end;

            if (dir == DirectionMode.UP_OR_LEFT)
            {
                start = new Vector2(rect.Left, rect.Top);
                end = new Vector2(rect.Left + tbWidth, rect.Top);
            }
            else
            {
                start = new Vector2(rect.Left, rect.Bottom);
                end = new Vector2(rect.Left + tbWidth, rect.Bottom);
            }

            linePos.start = start;
            linePos.end = end;
            return linePos;
        }

        public LinePos GetColLinePos(int colIdx, DirectionMode dir = DirectionMode.BOTTOM_OR_RIGHT)
        {
            RectangleF rect = GetOriginalCellRect(0, colIdx);

            LinePos linePos = new LinePos();
            Vector2 start, end;

            if (dir == DirectionMode.UP_OR_LEFT)
            {
                start = new Vector2(rect.Left, rect.Top);
                end = new Vector2(rect.Left, rect.Top + tbHeight);
            }
            else
            {
                start = new Vector2(rect.Right, rect.Top);
                end = new Vector2(rect.Right, rect.Top + tbHeight);
            }

            linePos.start = start;
            linePos.end = end;
            return linePos;
        }

        public RectangleF GetRect(int startRowIdx, int startColIdx, int endRowIdx, int endColIdx)
        {
            RectangleF r1 = GetOriginalCellRect(startRowIdx, startColIdx);
            RectangleF r2 = GetOriginalCellRect(endRowIdx, endColIdx);

            float left = r2.Left, top = r2.Top, right = r2.Right, bottom = r2.Bottom;

            if (r1.Left < r2.Left)
                left = r1.Left;

            if (r1.Top < r2.Top)
                top = r1.Top;

            if (r1.Right > r2.Right)
                right = r1.Right;

            if (r1.Bottom > r2.Bottom)
                bottom = r1.Bottom;

            RectangleF rect = new RectangleF(left, top, right - left, bottom - top);
            return rect;
        }

        public LinePos[] GetRectLinePos(int startRowIdx, int startColIdx, int endRowIdx, int endColIdx)
        {
            RectangleF rect = GetRect(startRowIdx, startColIdx, endRowIdx, endColIdx);

            LinePos[] linePosList = new LinePos[4];

            //
            LinePos linePos = new LinePos();
            linePos.start = new Vector2(rect.Left, rect.Top);
            linePos.end = new Vector2(rect.Left, rect.Bottom);
            linePosList[0] = linePos;

            //
            linePos = new LinePos();
            linePos.start = new Vector2(rect.Left, rect.Top);
            linePos.end = new Vector2(rect.Right, rect.Top);
            linePosList[1] = linePos;

            //
            linePos = new LinePos();
            linePos.start = new Vector2(rect.Right, rect.Top);
            linePos.end = new Vector2(rect.Right, rect.Bottom);
            linePosList[2] = linePos;


            //
            linePos = new LinePos();
            linePos.start = new Vector2(rect.Left, rect.Bottom);
            linePos.end = new Vector2(rect.Right, rect.Bottom);
            linePosList[3] = linePos;

            return linePosList;

        }

        public RectangleF GetTableRect()
        {
            RectangleF rect = new RectangleF(0, 0, tbWidth, tbHeight);
            return rect;
        }

        public RectangleF GetRowRect(int rowIdx)
        {
            RectangleF rect = GetOriginalCellRect(rowIdx, 0);
            rect.Width = tbWidth;
            return rect;
        }

        public RectangleF GetColRect(int colIdx)
        {
            RectangleF rect = GetOriginalCellRect(0, colIdx);
            rect.Height = tbHeight;
            return rect;
        }

        public RectangleF GetCellContentGlobalRect(int rowIdx, int colIdx)
        {
            RectangleF rect = GetCellContentRect(rowIdx, colIdx);
            return TransToGlobalRect(rect);
        }

        public RectangleF GetCellContentRect(int rowIdx, int colIdx)
        {
            return GetCellContentRect(new CellCoord(rowIdx, colIdx));
        }

        public RectangleF GetCellContentRect(CellCoord cellCoord)
        {
            RectangleF rect = GetCellRect(cellCoord);
            Margin cellMargin = GetCellMargin(cellCoord.rowIdx, cellCoord.colIdx);

            rect.X += cellMargin.left;
            rect.Y += cellMargin.top;

            rect.Width -= cellMargin.left + cellMargin.right;
            rect.Height -= cellMargin.top + cellMargin.bottom;

            return rect;
        }

        public RectangleF GetCellGlobalRect(int rowIdx, int colIdx)
        {
            RectangleF rect = GetCellRect(rowIdx, colIdx);
            return TransToGlobalRect(rect);
        }
        public RectangleF GetCellRect(int rowIdx, int colIdx)
        {
            return GetCellRect(new CellCoord(rowIdx, colIdx));
        }

        public RectangleF GetCellRect(CellCoord cellCoord)
        {
            SpanOffset? span = GetCellSpan(cellCoord.rowIdx, cellCoord.colIdx);

            if (span == null)
                return GetOriginalCellRect(cellCoord);

            SpanOffset _span = span.Value;


            int rowIdx = cellCoord.rowIdx - _span.top;
            int colIdx = cellCoord.colIdx - _span.left;
            RectangleF r1 = GetOriginalCellRect(rowIdx, colIdx);

            rowIdx = cellCoord.rowIdx + _span.bottom;
            colIdx = cellCoord.colIdx + _span.right;
            RectangleF r2 = GetOriginalCellRect(rowIdx, colIdx);


            RectangleF rect = new RectangleF(r1.Left, r1.Top, r2.Right - r1.Left, r2.Bottom - r1.Top);
            return rect;
        }

        public RectangleF GetOriginalCellRect(int rowIdx, int colIdx)
        {
            return GetOriginalCellRect(new CellCoord(rowIdx, colIdx));
        }

        public RectangleF GetOriginalCellRect(CellCoord cellCoord)
        {
            TableLine line;
            RectangleF rect = new RectangleF();
            float xpos = 0;
            float ypos = 0;

            for (int i = 0; i < colLineList.Count; i++)
            {
                line = colLineList[i];

                if (i == cellCoord.colIdx)
                    break;

                xpos += line.computedDistance;
            }

            rect.X = xpos;
            rect.Width = colLineList[cellCoord.colIdx].computedDistance;


            //
            for (int i = 0; i < rowLineList.Count; i++)
            {
                line = rowLineList[i];

                if (i == cellCoord.rowIdx)
                    break;

                ypos += line.computedDistance;
            }

            rect.Y = ypos;
            rect.Height = rowLineList[cellCoord.rowIdx].computedDistance;

            return rect;
        }

        
        public void  AddNewAutoRow(TableCellData[] cellValueInfos = null)
        {
            AddNewRow(cellValueInfos);
        }

        public void AddNewRow(TableCellData[] cellValueInfos,
            bool enableAutoAdjustRichSize = false,
            LineComputeMode lineComputeMode = LineComputeMode.AUTO,
            float computeParam = 0,
            float computedDistance = 0)
        {
            AddNewRow(enableAutoAdjustRichSize, lineComputeMode, computeParam, computedDistance);

            if (cellValueInfos != null)
            {
                for (int i = 0; i < colAmount; i++)
                {
                    if (i < cellValueInfos.Length)
                        SetCellValue(rowAmount - 1, i, cellValueInfos[i]);
                }
            }
        }

        void AddNewRow(
            bool enableAutoAdjustRichSize = false,
            LineComputeMode lineComputeMode = LineComputeMode.AUTO,
            float computeParam = 0,
            float computedDistance = 0)
        {
            rowAmount += 1;

            TableLine tableLine = new TableLine(LineDir.HORIZONTAL);
            tableLine.enableAutoAdjustRichSize = enableAutoAdjustRichSize;
            tableLine.lineComputeMode = lineComputeMode;
            tableLine.computeParam = computeParam;
            tableLine.computedDistance = computedDistance;
            tableLine.maxComputedValue = -1;
            tableLine.minComputedValue = -1;
            rowLineList.Add(tableLine);
        }


        public void AddNewAutoCol(TableCellData[] cellValueInfos = null)
        {
            AddNewCol(cellValueInfos);
        }

        public void AddNewCol(TableCellData[] cellValueInfos,
            bool enableAutoAdjustRichSize = false,
            LineComputeMode lineComputeMode = LineComputeMode.AUTO,
            float computeParam = 0,
            float computedDistance = 0)
        {
            AddNewCol(enableAutoAdjustRichSize, lineComputeMode, computeParam, computedDistance);

            if (cellValueInfos != null)
            {
                for (int i = 0; i < rowAmount; i++)
                {
                    if (i < cellValueInfos.Length)
                        SetCellValue(i, colAmount - 1, cellValueInfos[i]);

                }
            }
        }

        void AddNewCol(
            bool enableAutoAdjustRichSize = false,
            LineComputeMode lineComputeMode = LineComputeMode.AUTO,
            float computeParam = 0,
            float computedDistance = 0)
        {
            colAmount += 1;

            TableLine tableLine = new TableLine(LineDir.VERTICAL);
            tableLine.enableAutoAdjustRichSize = enableAutoAdjustRichSize;
            tableLine.lineComputeMode = lineComputeMode;
            tableLine.computeParam = computeParam;
            tableLine.computedDistance = computedDistance;
            tableLine.maxComputedValue = -1;
            tableLine.minComputedValue = -1;
            colLineList.Add(tableLine);
        }
    }
}
