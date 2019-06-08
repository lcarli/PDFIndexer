﻿using PDFIndexer.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDFIndexer.PDFCore
{
    public class BasicFirstPageStats : IProcessBlock
    {
        public float MinX { get; private set; }
        public float MaxX { get; private set; }
        public float PageWidth { get; private set; }
        public float TabStop { get; private set; }

        static BasicFirstPageStats Global = null;

        public BasicFirstPageStats Stats { get
            {
                return Global;
            }
        }

        [Obsolete]
        public static void Reset()
        {
            Global = null;
        }

        public void SetTabStop(float tabstop)
        {
            TabStop = tabstop;
        }

        public void SetupPage(BlockPage page)
        {
            if (Global != null)
                return;                

            Global = this;

            var blocks = page.AllBlocks;

            MinX = blocks.Min(b => b.GetX());
            MaxX = blocks.Max(b => b.GetX() + b.GetWidth());
            PageWidth = MaxX - MinX;
        }

        public BlockPage Process(BlockPage page)
        {
            SetupPage(page);

            return page;
        }
    }
}
