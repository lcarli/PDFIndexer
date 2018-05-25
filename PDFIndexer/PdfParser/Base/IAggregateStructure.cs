using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Base
{
    interface IAggregateStructure<TI, TO>
    {
        void Init(TI line);

        bool Aggregate(TI line);

        TO Create(List<TI> input);
    }
}
