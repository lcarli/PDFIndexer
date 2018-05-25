﻿
using PDFIndexer.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using PDFIndexer.Base;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Extgstate;
using iText.IO.Font.Constants;
using PDFIndexer.ExecutionStats;

namespace PDFIndexer.Execution
{
    class PipelineInputPdf : IPipelinePdfContext, IDisposable
    {
        private readonly string _input;
        private PdfDocument _pdfDocument;
        private string _output;
        private PdfDocument _pdfOutput;
        private List<object> _statsCollection = new List<object>();
        private List<List<object>> _statsCollectionPerPage = new List<List<object>>();
        private PipelinePdfLog _pdfLog = new PipelinePdfLog();
        private TransformIndexTree _indexTree = new TransformIndexTree();
        //private PipelineSingletonAutofacFactory _documentFactory = new PipelineSingletonAutofacFactory();
        private PipelineFactory _documentFactory;

        private static bool g_continueOnException = true;

        public static void StopOnException()
        {
            g_continueOnException = false;
        }
        public static PipelineInputPdf DebugCurrent;

        public PipelineInputPdfPage CurrentPage { get; private set; }
        public TransformIndexTree Index => _indexTree;

        public PipelineInputPdf(string filename, PipelineFactory factory, PipelineInputCache<IProcessBlockData> cache = null)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var pdfDocument = new PdfDocument(VirtualFS.OpenPdfReader(filename));

            InitDocument(pdfDocument, factory);

            this._input = filename;
            this._pdfDocument = pdfDocument;
            this._documentFactory = factory;
            
            if( cache != null )
            {
                cache.SetSize(_pdfDocument.GetNumberOfPages());
                this._cache = cache;
            }

            PipelineInputPdf.DebugCurrent = this;

            PdfReaderException.ClearContext();
        }

        void InitDocument(PdfDocument document, PipelineFactory factory)
        {
            var globalStats = factory.CreateGlobalInstance<PipelineDocumentStats>();

            var page = document.GetPage(1);

            var rect = page.GetPageSize();
            
            globalStats.X = rect.GetX();
            globalStats.H = rect.GetY();
            globalStats.Height = rect.GetHeight();
            globalStats.Width = rect.GetWidth();
        }

        public PipelineInputPdfPage Page(int pageNumber)
        {
            if (CurrentPage != null)
            {
                CurrentPage.Dispose();
            }

            var page = new PipelineInputPdfPage(new PipelineFactory(this._documentFactory), this, pageNumber);
            
            CurrentPage = page;

            return page;
        }

        public void LogCheck(int pageNumber, Type component, string message)
        {
            _pdfLog.LogCheck(pageNumber, component, message);
        }
        
        public int ExtractOutputPages(string outputfile, IEnumerable<int> pages)
        {
            string inputfile = this._input;

            var pageList = pages.OrderBy(t => t).ToList();

            if (pageList.Count == 0)
                return 0;

            using (var pdfInput = new PdfDocument(VirtualFS.OpenPdfReader(_output)))
            using (var pdfOutput = new PdfDocument(VirtualFS.OpenPdfWriter(outputfile)))
            {
                pdfInput.CopyPagesTo(pageList, pdfOutput);
            }

            return pageList.Count;
        }

        public int SaveErrors(string outputfile)
        {
            string inputfile = this._input;

            var errorPages = _pdfLog.GetErrors().OrderBy(t=>t).ToList();

            if (errorPages.Count == 0)
                return 0;

            using (var pdfInput = new PdfDocument(VirtualFS.OpenPdfReader(_input)))
            using (var pdfOutput = new PdfDocument(VirtualFS.OpenPdfWriter(outputfile)))
            {
                pdfInput.CopyPagesTo(errorPages, pdfOutput);
            }

            return errorPages.Count;
        }

        public PipelineInputPdf Output(string outfile)
        {
            if( _pdfOutput != null )
            {
                ((IDisposable)_pdfOutput).Dispose();
            }

            var pdfOutput = new PdfDocument(VirtualFS.OpenPdfReader(_input), VirtualFS.OpenPdfWriter(outfile));

            this._output = outfile;
            this._pdfOutput = pdfOutput;

            return this;
        }

        public PipelineDebugContext CreatePipelineDebugContext(string outputname)
        {
            return new PipelineDebugContext(_input, outputname);
        }

        public void Dispose()
        {
            if( CurrentPage != null )
            {
                CurrentPage.Dispose();
                CurrentPage = null;
            }

            if (_pdfDocument != null)
            {
                ((IDisposable)_pdfDocument).Dispose();
                _pdfDocument = null;
            }

            if (_pdfOutput != null)
            {
                ((IDisposable)_pdfOutput).Dispose();
                _pdfOutput = null;
            }            
        }

        public void ExtractPages(string outfile, IList<int> pageNumbers)
        {
            using (var pdfInput = new PdfDocument(VirtualFS.OpenPdfReader(_input)))
            using (var pdfOutput = new PdfDocument(VirtualFS.OpenPdfWriter(outfile)))
            {
                pdfInput.CopyPagesTo(pageNumbers, pdfOutput);
            }
        }

        public PipelineText<TextLine> AllPages<T>(Action<PipelineInputPdfPage> callback)
            where T : IConvertBlock, new()
        {
            var textLines = StreamConvert<T>(callback);
            
            var pipeText = new PipelineText<TextLine>(this._documentFactory, this, textLines, _indexTree, this);
            
            return pipeText;
        }
        
        public IEnumerable<TextLine> Get<T>(Action<PipelineInputPdfPage> callback)
            where T : IConvertBlock, new()
        {
            return StreamConvert<T>(callback);
        }

        private IEnumerable<TextLine> StreamConvert<T>(Action<PipelineInputPdfPage> callback)
            where T: IConvertBlock, new()
        {
            var processor = new T();            

            int totalPages = _pdfDocument.GetNumberOfPages();

            for (int i = 1; i <= totalPages; i++)
            {
                var pdfPage = Page(i);

                if (ProtectCall(callback, pdfPage) == false)
                    continue;

                var textSet = processor.ProcessPage(i, CurrentPage.GetLastResult());

                foreach(var t in textSet)
                {
                    yield return t;
                }
            }
        }

        bool ProtectCall(Action<PipelineInputPdfPage> callback, PipelineInputPdfPage pdfPage)
        {
            if (!g_continueOnException)
            {
                callback(pdfPage);
                return true;
            }

            try
            {
                callback(pdfPage);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                bool hasOutput = (_pdfOutput != null);
                if (hasOutput)
                {
                    PipelineDebug.ShowException(this, ex);
                }

                StoreStatistics(new StatsExceptionHandled(pdfPage.GetPageNumber(), ex));
                StoreStatistics(pdfPage.GetPageNumber(), new StatsExceptionHandled(pdfPage.GetPageNumber(), ex));
            }

            return false;
        }

        public void StoreStatistics(object stats)
        {
            _statsCollection.Add(stats);
        }

        public void StoreStatistics(int page, object stats)
        {
            int index = page - 1;

            while( index >= _statsCollectionPerPage.Count )
            {
                _statsCollectionPerPage.Add(new List<object>());
            }

            var stat = _statsCollectionPerPage[index];

            stat.Add(stats);
        }

        public IEnumerable<T> RetrieveStatistics<T>()
            where T: class
        {
            var availableStats = _statsCollection
                                    .Select(s => s as T)
                                    .Where(s => s != null);

            return availableStats;
        }

        public PipelineStats Statistics => new PipelineStats(_statsCollection, _statsCollectionPerPage);

        public PipelineInputPdf Global<T>() where T : class
        {
            _documentFactory.CreateGlobalInstance<T>();

            return this;
        }

        public void StageProcess(Action<PipelineInputPdfPage> callback)
        {
            int totalPages = _pdfDocument.GetNumberOfPages();

            using(this)
            {
                for (int i = 1; i <= totalPages; i++)
                {
                    var pdfPage = Page(i);

                    if (ProtectCall(callback, pdfPage) == false)
                        continue;
                }
            }
        }

        PipelineInputCache<IProcessBlockData> _cache = null;

        PipelineInputCache<IProcessBlockData> GetCache()
        {
            if (_cache == null)
                PdfReaderException.AlwaysThrow("Cache not initialized");

            return _cache;
        }

        public IProcessBlockData FromCache<T>(int pageNumber) => GetCache().FromCache<T>(pageNumber-1);

        public void StoreCache<T>(int pageNumber, IProcessBlockData result) => GetCache().StoreCache<T>(pageNumber-1, result);

        public class PipelineInputPdfPage : IDisposable
        {
            private readonly PipelineInputPdf _pdf;
            private readonly int _pageNumber;            
            private readonly PdfPage _pdfPage;
            private PipelinePage _page;
            private PdfCanvas _outputCanvas;

            PipelineFactory _factory;

            public int GetPageNumber() => _pageNumber;
            public BlockPage GetLastResult() => _page.LastResult;

            public PipelineInputPdfPage(PipelineFactory factory, PipelineInputPdf pipelineInputContext, int pageNumber)
            {
                var pdfPage = pipelineInputContext._pdfDocument.GetPage(pageNumber);

                this._pdf = pipelineInputContext;
                this._pageNumber = pageNumber;
                this._pdfPage = pdfPage;
                this._factory = factory;

                PdfReaderException.SetContext(_pdf._input, pageNumber);
            }

            public T CreateInstance<T>() where T: class
            {
                return _factory.CreateInstance<T>();
            }

            public PipelinePage ParsePdf<T>()
                where T: class, IEventListener, IPipelineResults<BlockPage>, new()
            {
                var listener = CreateInstance<T>();

                var parser = new PdfCanvasProcessor(listener);
                parser.ProcessPageContent(_pdfPage);

                // retrieve page size. where to store?
                var pageSize = _pdfPage.GetPageSize();

                var page = new PipelinePage(_pdf,  _pageNumber);

                page.LastResult = listener.GetResults();

                if (page.LastResult == null)
                    throw new InvalidOperationException();

                if (page.LastResult.AllBlocks == null)
                    throw new InvalidOperationException();

                _page = page;

                return page;
            }

            public PipelinePage FromCache<T>()
                where T : class, IProcessBlockData
            {
                var page = new PipelinePage(_pdf, _pageNumber);

                _page = page.FromCache<T>();

                return _page;
            }

            PdfCanvas GetCanvas()
            {
                if(_outputCanvas == null)
                {
                    var page = _pdf._pdfOutput.GetPage(_pageNumber);                                        
                    var canvas = new PdfCanvas(page);

                    _outputCanvas = canvas;
                }                

                return _outputCanvas;
            }

            iText.Kernel.Colors.DeviceRgb GetColor(System.Drawing.Color color)
            {
                return new iText.Kernel.Colors.DeviceRgb(color.R, color.G, color.B);
            }

            public void DrawRectangle(double x, double h, double width, double height, System.Drawing.Color color)
            {
                var canvas = GetCanvas();

                var pdfColor = GetColor(color);

                canvas.SetStrokeColor(pdfColor);
                canvas.Rectangle(x, h, width, height);
                canvas.Stroke();
            }
            public void FillRectangle(double x, double h, double width, double height, System.Drawing.Color color)
            {
                var canvas = GetCanvas();

                var pdfColor = GetColor(color);

                int opacity = color.A;

                canvas.SaveState();
                if( opacity < 250 )
                {
                    PdfExtGState gstate = new PdfExtGState();
                    gstate.SetFillOpacity(color.A / 255f);
                    //gstate.SetBlendMode(PdfExtGState.BM_EXCLUSION);
                    canvas.SetExtGState(gstate);
                }                

                canvas.SetFillColor(pdfColor);
                canvas.Rectangle(x, h, width, height);
                canvas.Fill();
                canvas.RestoreState();
            }
            public void DrawText(double x, double h, string text, float size, System.Drawing.Color color)
            {
                var canvas = GetCanvas();

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

            public void DrawBackground(System.Drawing.Color color)
            {
                float page_width = _pdfPage.GetPageSize().GetWidth();
                float page_height = _pdfPage.GetPageSize().GetHeight();

                FillRectangle(0, 0, page_width, page_height, color);
            }

            public void DrawWarning(string message, float size, System.Drawing.Color color)
            {
                int MAXTEXTSIZE = (int)(1.2*_pdfPage.GetPageSize().GetHeight()/size);
                float margin = 10f;
                float linespace = size*1.15f;
                float paragraph = size * 2f;
                
                float x = margin;
                float h = _pdfPage.GetPageSize().GetHeight() - size - margin;
                
                string[] lines = message.Split('\n');

                foreach (var line in lines)
                {
                    string text;

                    for (text = line; text.Length > MAXTEXTSIZE; text = text.Substring(MAXTEXTSIZE))
                    {                        
                        DrawText(x, h, text.Substring(0, MAXTEXTSIZE), size, color);
                        h -= linespace;
                    }
                    DrawText(x, h, text, size, color);

                    h -= paragraph;
                }
            }

            public void DrawLine(double x1, double h1, double x2, double h2, System.Drawing.Color color)
            {
                var canvas = GetCanvas();

                var pdfColor = GetColor(color);

                canvas.SetStrokeColor(pdfColor);
                canvas.MoveTo(x1, h1);
                canvas.LineTo(x2, h2);
                canvas.Stroke();
            }
            

            public void Dispose()
            {
                PdfReaderException.ClearContext();

                if ( _outputCanvas != null )
                {
                    _outputCanvas.Release();
                    _outputCanvas = null;
                }

                if( _factory != null )
                {
                    _factory.Dispose();
                    _factory = null;
                }
            }
        }        
    }
}
