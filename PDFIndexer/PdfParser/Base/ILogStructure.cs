﻿using PDFIndexer.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFIndexer.Base
{
    interface ILogStructure<T>
    {
        void StartLog(TextWriter input);
        void Log(TextWriter input, T data);
        void EndLog(TextWriter input);
    }

    interface ILogStructure2<T> : ILogStructure<T>
    {
        void Init(ITransformIndexTree index);
    }
}
