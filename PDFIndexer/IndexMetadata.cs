﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer
{
    class IndexMetadata
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public List<PdfMetadata> ListOfLines { get; set; }
        public List<PdfMetadata> ListOfWords { get; set; }
        public string PDFURI { get; set; }

        public IndexMetadata(string _text, List<PdfMetadata> _listOfLines, List<PdfMetadata> _listOfWords)
        {
            Id = Guid.NewGuid();
            Text = _text;
            ListOfLines = _listOfLines;
            ListOfWords = _listOfWords;
        }

        public IndexMetadata(string _text, List<PdfMetadata> _listOfLines, List<PdfMetadata> _listOfWords, string _pdfUri)
        {
            Id = Guid.NewGuid();
            Text = _text;
            ListOfLines = _listOfLines;
            ListOfWords = _listOfWords;
            PDFURI = _pdfUri;
        }

        public IndexMetadata() { }
    }
}
