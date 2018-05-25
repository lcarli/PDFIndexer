﻿using PDFIndexer.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDFIndexer.Base;

namespace PDFIndexer.Parser
{
    //
    //
    // TODO: rewrite using IFilterStructure
    //
    //
    class TransformExemplo : IAggregateStructure<TextStructure, TextStructure>
    {
        public bool Aggregate(TextStructure line)
        {
            // never aggregate multiple lines
            return false;
        }

        public TextStructure Create(List<TextStructure> textStructureList)
        {
            if (textStructureList.Count != 1)
                throw new InvalidOperationException("impossible");

            var textStruct = textStructureList[0];

            // filter password out
            if (textStruct.Text.Contains("password"))
                return null;

            return textStruct;
        }

        public void Init(TextStructure line)
        {
        }
    }
}
