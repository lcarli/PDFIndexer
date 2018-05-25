﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    interface IProcessStructure<T>
    {
        T Process(T structure);
    }

    interface IProcessStructure2<T>
    {
        IEnumerable<T> Process(IEnumerable<T> input);
    }
}
