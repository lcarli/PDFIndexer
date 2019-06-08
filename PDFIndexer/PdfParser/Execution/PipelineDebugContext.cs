﻿using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using PDFIndexer.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Execution
{
    public class PipelineDebugContext : IPipelineDebug, IDisposable
    {
        private PdfDocument _pdf;
        private int _lastPageNumber = -1;
        private PdfCanvas _currentCanvas = null;

        public PipelineDebugContext(string filename, string outputname)
        {
            this._pdf = new PdfDocument(VirtualFS.OpenPdfReader(filename), VirtualFS.OpenPdfWriter(outputname));
        }

        public void Dispose()
        {
            if ( _pdf != null )
            {
                ((IDisposable)_pdf).Dispose();
                _pdf = null;
            }
        }

        public void ShowLine(TextLine line, System.Drawing.Color color)
        {
            int pageNumber = line.PageInfo.PageNumber;

            if (pageNumber == -1)
                PdfReaderException.AlwaysThrow("invalid page number");

            if( pageNumber != _lastPageNumber )
            {
                var page = _pdf.GetPage(pageNumber);
                var canvas = new PdfCanvas(page);

                _currentCanvas = canvas;
            }

            DrawRectangle(_currentCanvas, line.Block, color);
        }

        public void ShowText(string text, TextLine line, System.Drawing.Color color)
        {
            int pageNumber = line.PageInfo.PageNumber;

            if (pageNumber == -1)
                PdfReaderException.AlwaysThrow("invalid page number");

            if (pageNumber != _lastPageNumber)
            {
                var page = _pdf.GetPage(pageNumber);
                var canvas = new PdfCanvas(page);

                _currentCanvas = canvas;
            }

            DrawText(_currentCanvas, text, line.Block, color);
        }

        public void ShowLine(IEnumerable<TextLine> lines, System.Drawing.Color color)
        {
            foreach(var line in lines)
            {
                ShowLine(line, color);
            }
        }

        void DrawRectangle(PdfCanvas canvas, IBlock block, System.Drawing.Color color)
        {
            DrawRectangle(canvas, block.GetX(), block.GetH(), block.GetWidth(), block.GetHeight(), color);
        }

        void DrawRectangle(PdfCanvas canvas, double x, double h, double width, double height, System.Drawing.Color color)
        {
            var pdfColor = GetColor(color);

            canvas.SetStrokeColor(pdfColor);
            canvas.Rectangle(x, h, width, height);
            canvas.Stroke();
        }

        void DrawText(PdfCanvas canvas, string text, IBlock block, System.Drawing.Color color)
        {
            const float FONT_SIZE = 8f;
            const float FONT_DISTANCE = 2f;
            DrawText(canvas, block.GetX() + block.GetWidth() + FONT_DISTANCE, block.GetH() , text, FONT_SIZE, color);
        }

        void DrawText(PdfCanvas canvas, double x, double h, string text, float size, System.Drawing.Color color)
        {
            var pdfColor = GetColor(color);

            canvas.SetColor(pdfColor, true);
            //canvas.Rectangle(x, h, width, height);
            canvas.BeginText();
            canvas.MoveText(x, h);
            var font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            canvas.SetFontAndSize(font, size);
            canvas.ShowText(text);
            canvas.EndText();
            //canvas.Stroke();
        }
        iText.Kernel.Colors.DeviceRgb GetColor(System.Drawing.Color color)
        {
            return new iText.Kernel.Colors.DeviceRgb(color.R, color.G, color.B);
        }
    }
}
