﻿using System;
using System.Collections.Generic;
using System.Text;
using PDFIndexer.Base;

namespace PDFIndexer
{
    class Block : IBlock
    {
        public string Text { get; set; }
        public float X { get; set; }
        public float B { get; set; }
        public float H { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Lower { get; set; }
        public string FontName { get; set; }
        public string FontFullName { get; set; }
        public string FontStyle { get; set; }
        public float FontSize { get; set; }
        public float WordSpacing { get; set; }
        public bool HasBackColor { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public string GetText() => Text;
        public float GetX() => X;
        public float GetH() => H;
        public float GetWidth() => Width;
        public float GetHeight() => Height;
        public float GetWordSpacing() => WordSpacing;

        public Block()
        {
        }

        public Block(Block block)
        {
            Text = block.Text;
            X = block.X;
            B = block.B;
            H = block.H;
            Width = block.Width;
            Height = block.Height;
            Lower = block.Lower;
            FontName = block.FontName;
            FontFullName = block.FontFullName;
            FontStyle = block.FontStyle;
            FontSize = block.FontSize;
            WordSpacing = block.WordSpacing;
            HasBackColor = block.HasBackColor;
            IsBold = block.IsBold;
            IsItalic = block.IsItalic;
        }

        public static bool HasOverlap(IBlock a, IBlock b)
        {
            float a_x1 = a.GetX();
            float a_x2 = a.GetX() + a.GetWidth();
            float a_y1 = a.GetH();
            float a_y2 = a.GetH() + a.GetHeight();

            float b_x1 = b.GetX();
            float b_x2 = b.GetX() + b.GetWidth();
            float b_y1 = b.GetH();
            float b_y2 = b.GetH() + b.GetHeight();

            return (HasOverlap(a_x1, a_x2, b_x1, b_x2) && HasOverlap(a_y1, a_y2, b_y1, b_y2));
        }

        public static bool HasOverlapWithY(IBlock a, IBlock b, float errY)
        {
            float a_x1 = a.GetX();
            float a_x2 = a.GetX() + a.GetWidth();
            float a_y1 = a.GetH() - errY;
            float a_y2 = a.GetH() + a.GetHeight() + errY;

            float b_x1 = b.GetX();
            float b_x2 = b.GetX() + b.GetWidth();
            float b_y1 = b.GetH() - errY;
            float b_y2 = b.GetH() + b.GetHeight() + errY;

            return (HasOverlap(a_x1, a_x2, b_x1, b_x2) && HasOverlap(a_y1, a_y2, b_y1, b_y2));
        }


        static bool HasOverlap(float a1, float a2, float b1, float b2)
        {
            if ((b1 > b2) || (a1 > a2))
                throw new InvalidOperationException();

            bool separated = ((a1 < b1) && (a2 < b1)) ||
                             ((a1 > b2) && (a2 > b2));
            return !separated;
        }

        public static bool HasOverlapY(IBlock block, float b1, float b2)
        {
            if (b1 > b2)
                throw new InvalidOperationException();

            float a1 = block.GetH();
            float a2 = block.GetH() + block.GetHeight();

            bool separated = ((a1 < b1) && (a2 < b1)) ||
                             ((a1 > b2) && (a2 > b2));
            return !separated;
        }
        public static bool AreSameLine(IBlock a, IBlock b)
        {
            float err = 0.01f;

            float diff_h1 = Math.Abs(a.GetH() - b.GetH());
            float diff_h2 = Math.Abs(a.GetHeight() - b.GetHeight());

            return ( diff_h1 < err && diff_h2 < err );
        }
        public static bool SameHeight(IBlock a, IBlock b)
        {
            float err = 0.5f;

            float ah = a.GetH() + a.GetHeight();
            float bh = b.GetH() + b.GetHeight();

            float diff = Math.Abs(ah - bh);

            return (diff < err);
        }
        
        public static bool IsSuperscriptFont(Block a, Block b)
        {
            float a_y1 = a.GetH();
            float a_y2 = a.GetH() + a.FontSize;
            float b_y1 = b.GetH();

            return ((a_y2 > b_y1) && (a_y1 < b_y1)) && (a.FontSize > b.FontSize);
        }

        public static bool Contains(IBlock a, IBlock b)
        {
            float a_y1 = a.GetH();
            float a_y2 = a.GetH() + a.GetHeight();
            float b_y1 = b.GetH();
            float b_y2 = b.GetH() + b.GetHeight();

            return ((a_y2 > b_y2) && (a_y1 < b_y1));
        }
    }

}
