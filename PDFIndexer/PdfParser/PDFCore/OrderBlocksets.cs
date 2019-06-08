﻿//#define DEBUG_ORDERBLOCKSET

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDFIndexer.Base;

namespace PDFIndexer.PDFCore
{
    public class OrderBlocksets : IProcessBlock
    {
        const float VERTICAL_DIFFERENCE = 4f;
        int VERTICAL_DIFFERENCE_INT = 1;

        private List<Data> Values { get; set; }
        private List<Data> ValuesY { get; set; }
        List<IBlock> OrderedBlocks = new List<IBlock>();
        public bool[] ValuesB { get; private set; }

        class Data
        {
            public int ID;
            public int X;
            public int X2;
            public int Y;
            public int Y1;
            public int W;
            public object B;
        }

        public BlockPage Process(BlockPage page)
        {
            var blocksets = page.AllBlocks.ToList();

            if (blocksets.Count == 0)
                return page;

            float x1 = page.AllBlocks.GetX();
            float x2 = page.AllBlocks.GetX() + page.AllBlocks.GetWidth();
            float dx = page.AllBlocks.GetWidth() + 2;
            float h1 = page.AllBlocks.GetH();
            float h2 = page.AllBlocks.GetH() + page.AllBlocks.GetHeight();
            float dh = page.AllBlocks.GetHeight() + 2;

            // Prepare the values order by X
            int id = 0;
            this.Values = page.AllBlocks.Select(b => new Data
            {
                ID = id++,
                X = (int)(6.0 * ((b.GetX() - x1) / dx) + 0.5),
                X2 = (int)(6.0 * ((b.GetX() + b.GetWidth() - x1) / dx) + 0.5),
                Y = (int)(1000 * (b.GetH() - h1) / (dh)),
                Y1 = (int)(1000 * (b.GetH() + b.GetHeight() - h1) / (dh)),
                W = (int)(6.0 * (b.GetWidth() / dx) + 0.5),
                B = b
            })
            .OrderBy(p => 10000 * p.X - p.Y)
            .ToList();

            VERTICAL_DIFFERENCE_INT = (int)(1000*VERTICAL_DIFFERENCE / dh);

            var checkInvalidW = Values.Where(v => v.X2 - v.X != v.W).ToList();

            // sometimes W is miscalculated - need to investigate
            // it is related to smaller size than the expected
            // check ResizeBlocksets as well
            if (checkInvalidW.Count > 0)
            {
                // warn the issue
                PdfReaderException.Warning("checkInvalidW failed");

                // workaround: recalculate W in terms of X and X2
                checkInvalidW.Select(t => { var inv = Values.Where(t1 => t1.ID == t.ID).First(); inv.W = inv.X2 - inv.X; return 0; }).ToList();
                checkInvalidW = Values.Where(v => v.X2 - v.X != v.W).ToList();

                if (checkInvalidW.Count > 0)
                    PdfReaderException.Throw("checkInvalidW failed");
            }

            var checkOverW = Values.Where(v => v.W < 0 || v.W > 6).ToList();
            if (checkOverW.Count > 0)
            {
                PdfReaderException.Warning("checkOverW failed");
                Values = Values.Where(t => t.W >= 0 && t.W <= 6)
                    .OrderBy(p => 10000 * p.X - p.Y)
                    .ToList();
            }

            // re-implement in ResizeBlocksets (column)
            //// if column is narrow (W=1 and X=2, then W <- 2)
            //int fixCount = Values.Where(v => v.W == 1 && v.X == 2).Select(v => v.W = 2).Count();

            var checkOddW = Values.Where(v => v.W == 1 || v.W == 5).ToList();
            if (checkOddW.Count > 0)
            {
                PdfReaderException.Warning("checkOddW failed");
                Values = Values.Where(t => t.W != 1 && t.W != 5)
                    .OrderBy(p => 10000 * p.X - p.Y)
                    .ToList();
            }

            // very weird bug: causes infinite loop!
            var checkZeroW = Values.Where(v => v.W == 0).ToList();
            if (checkZeroW.Count > 0)
            {
                // try to set to 2
                checkZeroW.Where(t => t.X == 4).Select(t => { var inv = Values.Where(t1 => t1.ID == t.ID).First(); inv.W = 2; inv.X2 = 6; return 0; }).ToList();

                Values = Values.OrderBy(p => 10000 * p.X - p.Y).ToList();

                checkZeroW = Values.Where(v => v.W == 0).ToList();
                if (checkZeroW.Count > 0)
                {
                    PdfReaderException.Warning("checkZeroW failed");
                    Values = Values.Where(t => t.W != 0)
                            .OrderBy(p => 10000 * p.X - p.Y)
                            .ToList();
                }
            }

            var checkOddX = Values.Where(v => v.X != 2 && v.X != 3 && v.X != 4 && v.X != 0).ToList();
            if (checkOddX.Count > 0)
            {
                PdfReaderException.Warning("check X failed");
            }
            // Prepare the values order by Y
            this.ValuesY = Values.OrderBy(p => -100 * p.Y + p.X).ToList();

            this.ValuesB = new bool[Values.Count];

            OrderedBlocks = new List<IBlock>();

            scan();

            var result = new BlockPage();

            //result.AddRange(Values.Select(p => (IBlock)p.B));

            result.AddRange(OrderedBlocks);

            return result;
        }

        void scan(int level=0, int x = 2, int min_y = -1, int max_x=6, int cur_w=-1)
        {
            int max_loop = 100000;

            // Algorithm
            // 1. Scan the values in x=2..6
            // 2. Skip processed values
            // 3. Take the parameter while x <= x2
            // 4. Save the max Y
            //Values = Values
            //    .Where(p => p != null)
            //    .OrderBy(p => 10000 * p.X - p.Y)
            //    .ToList();

            for (; x<= max_x; x+=2)
            {
                for (int k=0; k<Values.Count; k++)
                {
                    if (0 > max_loop--)
                    {
                        PdfReaderException.Throw("Infinite loop detected");
                        foreach (var val in Values)
                        {
                            if (val != null)
                            {
#if DEBUG_ORDERBLOCKSET
                                Console.WriteLine($"[{val.ID}] X={val.X} Y={val.Y} X2={val.X2} W={val.W} cur_w={cur_w} level={level}");
#endif
                                OrderedBlocks.Add((IBlock)val.B);
                            }
                        }
                        return;
                    }

                    var v = Values[k];

                    // skip if already processed
                    if (v == null)
                        continue;

                    // ignore low values
                    if (v.Y < min_y)
                        continue;

                    if (cur_w == -1)
                        cur_w = v.W;

                    bool oneColumn = (cur_w == 6);
                    bool twoColumns = (cur_w == 3);
                    bool threeColumns = (cur_w == 2) || (cur_w == 4);

                    // if W >= 3
                    if ( v.W != cur_w )
                    {
                        bool oneColumn2 = (v.W == 6);
                        bool twoColumns2 = (v.W == 3);
                        bool threeColumns2 = (v.W == 2) || (v.W == 4);

                        if (!(threeColumns && threeColumns2))
                        {
                            scan(level + 1, x: x, min_y: v.Y1+1, max_x: 6, cur_w: cur_w);

                            // reset to the new column W
                            k = -1;
                            cur_w = -1;
                            continue;
                        }
                    }

                    // consume all values < X2
                    if ( x >= v.X2 )
                    {
                        // take the parameter
                        OrderedBlocks.Add((IBlock)v.B);

                        if ((v.W != cur_w) && (!((v.W == 2 && cur_w == 4)|| (v.W == 4 && cur_w == 2))))
                            PdfReaderException.Warning($"v.W != cur_w: {v.W} != {cur_w}");

#if DEBUG_ORDERBLOCKSET 
                        Console.WriteLine($"[{v.ID}] X={v.X} Y={v.Y} X2={v.X2} W={v.W} cur_w={cur_w} level={level}");
#endif
                        Values[k] = null;
                        continue;
                    }

                    if (v.X2 > max_x)
                        continue;

                    // define a new goal
                    scan(level + 1, x: x+2, min_y: v.Y1+1, max_x: v.X2, cur_w: cur_w);

                    if ((v.W != cur_w) && (!((v.W == 2 && cur_w == 4) || (v.W == 4 && cur_w == 2))))
                        PdfReaderException.Warning($"v.W != cur_w: {v.W} != {cur_w}");

                    // consume X2
                    OrderedBlocks.Add((IBlock)v.B);

#if DEBUG_ORDERBLOCKSET
                    Console.WriteLine($"[{v.ID}] X={v.X} Y={v.Y} X2={v.X2} W={v.W} cur_w={cur_w} level={level}");
#endif

                    Values[k] = null;

                    // reset
                    k = -1;

                    if (cur_w == 6)
                    {
                        cur_w = -1;
                    }

                    if (cur_w == 3 && v.X == 3)
                    {
                        cur_w = -1;
                    }

                    if (cur_w == 2 && v.X == 4)
                    {
                        cur_w = -1;
                    }
                }
            }

        }
    }
}
