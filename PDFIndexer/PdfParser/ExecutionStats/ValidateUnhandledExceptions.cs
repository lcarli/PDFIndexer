﻿using PDFIndexer.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.ExecutionStats
{
    public class ValidateUnhandledExceptions : ICalculateStats<StatsExceptionHandled>
    {
        public IList<StatsExceptionHandled> Results { get; private set; }

        public IEnumerable<int> GetPageErrors()
        {
            for (int i = 0; i < Results.Count; i++)
            {
                if (Results[i] == null)
                    continue;

                yield return Results[i].PageNumber;
            }
        }

        public object Calculate(IEnumerable<StatsExceptionHandled> stats)
        {
            var result = new List<StatsExceptionHandled>();

            foreach (var s in stats)
            {
                result.Add(s);
            }

            Results = result;

            return this;
        }
    }
}
