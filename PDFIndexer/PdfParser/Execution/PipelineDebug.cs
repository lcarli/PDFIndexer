﻿using PDFIndexer.PDFCore;
using PDFIndexer.TextStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using PDFIndexer.Base;

namespace PDFIndexer.Execution
{
    public static class PipelineDebug
    {
        static public void Output(PipelineInputPdf pdf, string filename)
        {
            pdf.Output(filename);
        }

        static public void DebugBreak<T>(T instance, Func<T, bool> condition)
        {
            if (condition != null)
            {
                bool shouldBreak = condition(instance);

                if (!shouldBreak)
                    return;
            }

            System.Diagnostics.Debugger.Break();
        }

        static public void Show(PipelineInputPdf pdf, BlockPage blockPage, Color color)
        {
            var blocks = blockPage.AllBlocks;

            foreach(var b in blocks)
            {
                pdf.CurrentPage.DrawRectangle(b.GetX(), b.GetH(), b.GetWidth(), b.GetHeight(), color);
            }
        }

        static public void ShowText(PipelineInputPdf pdf, BlockPage blockPage, Color color)
        {
            var blocks = blockPage.AllBlocks;

            foreach (var b in blocks)
            {
                float diff = 2f;
                pdf.CurrentPage.DrawRectangle(b.GetX()+diff/2, b.GetH()+diff/2, b.GetWidth()-diff, b.GetHeight()-diff, color);
                pdf.CurrentPage.DrawText(b.GetX(), b.GetH()+ b.GetHeight(), b.GetText(), b.GetHeight()/2, color);
            }
        }
        static public void ShowWarnings(PipelineInputPdf pdf, IEnumerable<string> warnings)
        {
            string text = String.Join("\n", warnings);

            pdf.CurrentPage.DrawWarning(text, 6, Color.Red);
        }

        static public void ShowException(PipelineInputPdf pdf, Exception ex)
        {
            PdfReaderException pdfException = ex as PdfReaderException;

            string component = FindPdfCoreComponent(ex.StackTrace);

            if (pdfException == null)
            {
                string text = component + "\n" + ex.Message + "\n" + ex.StackTrace;

                var white = System.Drawing.Color.FromArgb(230, 250, 250, 250);

                pdf.CurrentPage.DrawBackground(white);
                pdf.CurrentPage.DrawWarning(text, 20, Color.Red);
            }
            else
            {
                string text = $"({component}) {pdfException.ShortMessage}";

                var white = System.Drawing.Color.FromArgb(100, 200, 200, 200);
                var yellow = System.Drawing.Color.FromArgb(100, 250, 250, 0);
                var blue = System.Drawing.Color.FromArgb(100, 0, 0, 250);

                pdf.CurrentPage.DrawBackground(white);
                pdf.CurrentPage.DrawWarning(text, 12, Color.Red);

                var additionalInfo = pdfException.Blocks;
                if(additionalInfo != null )
                {
                    foreach (var block in additionalInfo)
                    {
                        float width = block.GetWidth();
                        float height = block.GetHeight();
                        
                        bool invalidBoundary = false;

                        if (width <= 3f) { width = 3f; invalidBoundary=true; }
                        if (height <= 3f) { height = 3f; invalidBoundary = true; }

                        if( invalidBoundary)
                        {
                            pdf.CurrentPage.FillRectangle(block.GetX(), block.GetH(), width, height, blue);
                            pdf.CurrentPage.DrawRectangle(block.GetX(), block.GetH(), width, height, Color.DarkRed);
}
                        else
                        {
                            pdf.CurrentPage.FillRectangle(block.GetX(), block.GetH(), width, height, yellow);
                            pdf.CurrentPage.DrawRectangle(block.GetX(), block.GetH(), width, height, Color.DarkRed);
                        }
                    }
                }                    
            }
        }

        static string FindPdfCoreComponent(string stackTrace)
        {
            const string PDFCORE_NAMESPACE = "PDFIndexer.PDFCore.";
            const string PDFCORE_METHODEND = "(";
            const string UNKNOWN_COMPONENT = "Component Unknown";

            int idxPdfCore = stackTrace.IndexOf(PDFCORE_NAMESPACE) + PDFCORE_NAMESPACE.Length;

            if (idxPdfCore < PDFCORE_NAMESPACE.Length)
                return UNKNOWN_COMPONENT;
            
            int idxPdfCore2 = stackTrace.IndexOf(PDFCORE_METHODEND, idxPdfCore);
            
            if (idxPdfCore2 < 0)
                return UNKNOWN_COMPONENT;

            return stackTrace.Substring(idxPdfCore, idxPdfCore2 - idxPdfCore);
        }


        static public void Show(PipelineInputPdf pdf, System.Collections.IEnumerable objectList, Color color)
        {
            foreach (var t in objectList)
            {
                var b = (IBlock)t;
                pdf.CurrentPage.DrawRectangle(b.GetX(), b.GetH(), b.GetWidth(), b.GetHeight(), color);
            }
        }

        static public void ShowLine(PipelineInputPdf pdf, BlockPage blockPage, Color color)
        {
            var blocks = blockPage.AllBlocks;

            float x1 = float.NaN;
            float h1 = float.NaN;

            foreach (var b in blocks)
            {
                float x2 = b.GetX() + b.GetWidth() / 2;
                float h2 = b.GetH() + b.GetHeight() / 2;

                if ( (!float.IsNaN(x1)) && (!float.IsNaN(h1)) )
                {
                    pdf.CurrentPage.DrawLine(x1, h1, x2, h2, color);
                }

                x1 = x2;
                h1 = h2;
            }
        }
    }

    static class PipelineDebugExtensions
    {
        static public PipelinePage Output(this PipelinePage page, string filename)
        {
            PipelineDebug.Output((PipelineInputPdf)page.Context, filename);
            return page;
        }
        static public PipelinePage DebugBreak(this PipelinePage page, Func<PipelinePage,bool> condition = null)
        {
            PipelineDebug.DebugBreak(page, condition);
            return page;
        }

        static public PipelinePage Show(this PipelinePage page, Color color)
        {            
            PipelineDebug.Show((PipelineInputPdf)page.Context, page.LastResult, color);
            return page;
        }
        static public PipelinePage ShowText(this PipelinePage page, Color color)
        {
            PipelineDebug.ShowText((PipelineInputPdf)page.Context, page.LastResult, color);
            return page;
        }

        static public PipelinePage ShowLine(this PipelinePage page, Color color)
        {
            PipelineDebug.ShowLine((PipelineInputPdf)page.Context, page.LastResult, color);
            return page;
        }
        //static public PipelineText Output(this PipelineText page, string filename)
        //{
        //    PipelineDebug.Output(page.Context, filename);

        //    return page;
        //}

        static public PipelineText<T> DebugBreak<T>(this PipelineText<T> page, Func<PipelineText<T>, bool> condition = null)
        {
            PipelineDebug.DebugBreak(page, condition);

            return page;
        }

        //static public PipelineText Show(this PipelineText page, Color color)
        //{
        //    PipelineDebug.Show(page.LastResult, color);

        //    return page;
        //}
    }
}
