﻿using PDFIndexer.CommomModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Utils
{
    static class ProcessResult
    {

        public static void ProcessResults(List<IndexMetadata> results)
        {
            //TODO: Extract Page
            //      Convert2Image
            //      Cut
            //      Create SampleObject




            foreach (var result in results)
            {

            }
        }


        private static List<HighlightObject> HightlightWords(IndexMetadata input, string keyword)
        {
            List<PdfMetadata> words = new List<PdfMetadata>();
            List<List<PdfMetadata>> wordPerPage = new List<List<PdfMetadata>>();
            List<HighlightObject> list = new List<HighlightObject>();
            int LastPage = 1;

            foreach (var word in input.ListOfWords)
            {
                if (keyword == word.Text)
                {
                    if (word.page == LastPage)
                    {
                        words.Add(word);
                    }
                    else
                    {
                        LastPage = word.page;
                        wordPerPage.Add(words);
                        words.Clear();
                        words.Add(word);
                    }
                }
            }

            foreach (var item in wordPerPage)
            {
                list.Add(new HighlightObject
                {
                    Metadata = input,
                    HighlightedWords = ConvertWord2BoundingBox(item),
                    Keyword = keyword,
                    PageNumber = item[0].page
                });
            }

            return list;
        }

        private static List<BoundingBox> ConvertWord2BoundingBox(List<PdfMetadata> words)
        {
            List<BoundingBox> list = new List<BoundingBox>();

            foreach (var word in words)
            {
                list.Add(new BoundingBox { X = word.X, Y = word.Y, Height = word.Height, Width = word.Width });
            }

            return list;
        }

    }
}
