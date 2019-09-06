using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ControlNS
{
    public enum LineDir
    {
        /// <summary>
        /// 水平行线
        /// </summary>
        HORIZONTAL,

        /// <summary>
        /// 垂直列线
        /// </summary>
        VERTICAL,
    }
    public enum LineComputeMode
    {
        /// <summary>
        /// 自动
        /// </summary>
        AUTO,

        /// <summary>
        /// 百分比
        /// </summary>
        PERCENTAGE,

        /// <summary>
        /// 绝对值
        /// </summary>               
        ABSOLUTE,
    }

    public enum DirectionMode
    {
        /// <summary>
        /// 上方或左方
        /// </summary>
        UP_OR_LEFT,

        /// <summary>
        /// 下方或右方
        /// </summary>               
        BOTTOM_OR_RIGHT
    }

    /// <summary>
    /// 停靠方式
    /// </summary>
    public enum DockType
    {
        None,
        LeftTop,
        Top,
        RightTop,
        Left,
        Center,
        HoriCenter,
        VertCenter,
        Right,
        LeftBottom,
        Bottom,
        RightBottom,
    }

    /// <summary>
    /// 尺寸匹配类型
    /// </summary>
    public enum MatchType
    {
        None,
        MatchParentSize,
        MatchParentWidth,
        MatchParentHeight,
    }

    public enum TableCellDataType
    {
        FixedSize,
        Table,
    }


    public struct Margin
    {
        public Margin(float left, float top, float right, float bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public float left;
        public float top;
        public float right;
        public float bottom;
    }

    public struct SpanOffset
    {
        public SpanOffset(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public int left { get; set; }
        public int top { get; set; }
        public int right { get; set; }
        public int bottom { get; set; }
    }

    public enum AdjustMode
    {
        /// <summary>
        /// 上调整或左调整
        /// </summary>
        UP_OR_LEFT_ADJUST,

        /// <summary>
        /// 下调整或右调整
        /// </summary>               
        DOWN_OR_RIGHT_ADJUST
    }

    public struct LinePos
    {
        public Vector2 start;
        public Vector2 end;
    }

    public class TableLine
    {
        public LineDir lineDir { get; set; }
        public LineComputeMode lineComputeMode { get; set; }
        public float computeParam { get; set; }

        public float computedDistance { get; set; }

        public float maxComputedValue { get; set; }

        public float minComputedValue { get; set; }

        /// <summary>
        /// 是否开启此行线自动调整富余尺寸模式
        /// </summary>
        public bool enableAutoAdjustRichSize = true;

        public TableLine(LineDir lineDir)
        {
            this.lineDir = lineDir;
        }
    }

 
    public struct CellCoord
    {
        public int rowIdx;
        public int colIdx;
        public CellCoord(int rowIdx, int colIdx)
        {
            this.rowIdx = rowIdx;
            this.colIdx = colIdx;
        }
    }

    public struct RectangleF
    {
        private float x;
        public float X
        {
            get { return x; }
            set
            {
                x = value;
                Left = x;
                Right = x + width;
            }
        }

        private float y;
        public float Y
        {
            get { return y; }
            set
            {
                y = value;
                Top = y;
                Bottom = y + height;
            }
        }

        private float width;
        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                Right = x + width;
            }
        }


        private float height;
        public float Height
        {
            get { return height; }
            set
            {
                height = value;
                Bottom = y + height;
            }
        }


        public float Left { get; private set; }
        public float Right { get; private set; }
        public float Top { get; private set; }
        public float Bottom { get; private set; }


        public RectangleF(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            Left = x;
            Top = y;
            Right = x + width;
            Bottom = y + height;
            this.width = width;
            this.height = height;
        }
    }

    public class TableCellData
    {
        public delegate float GetLayoutSizeCB(TableCellData data, LineDir lineDir);
        public GetLayoutSizeCB GetLayoutSize;

        public TableCellDataType type = TableCellDataType.FixedSize;
        public bool isLimitWidth = false;
        public object data;
        public Vector2 fixedSize;
        public int rowIdx;
        public int colIdx;
        public RectangleF rect;
    }
}
