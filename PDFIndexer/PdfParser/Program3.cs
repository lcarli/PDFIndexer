﻿using System;
using System.Collections.Generic;
using System.Text;
using PDFIndexer.Base;
using PDFIndexer.Parser;
using PDFIndexer.ExecutionStats;
using PDFIndexer.TextStructures;
using PDFIndexer.Execution;
using PDFIndexer.PDFText;
using System.Drawing;
using PDFIndexer.PDFCore;
using System.Linq;

namespace PDFIndexer
{
    class Program3
    {
        // public static void CreateLayout(string basename)
        // {
        //     //PipelineInputPdf.StopOnException();
        //     //PdfReaderException.ContinueOnException();

        //     Console.WriteLine();
        //     Console.WriteLine("Program3 - CreateLayout");
        //     Console.WriteLine();

        //     var textLines = GetTextLines(basename, out Execution.Pipeline pipeline)
        //                     .ConvertText<CreateStructures, TextStructure>()
        //                     .ConvertText<CreateTextSegments, TextSegment>()
        //                     .ToList();

        //     Console.WriteLine($"FILENAME: {pipeline.Filename}");

        //     var statistics = pipeline.Statistics;
        //     //var layout = (ValidateLayout)statistics.Calculate<ValidateLayout, StatsPageLayout>();
        //     var layout = statistics.RetrieveStatistics<StatsPageLayout>();

        //     // pipeline.Statistics.SaveStats<StatsPageLayout>($"bin/{basename}-pagelayout.txt");

        //     pipeline.ExtractOutput<ShowParserWarnings>($"bin/{basename}-parser-errors.pdf");
        // }


        public static void ProcessStage(string basename, int page=-1)
        {
            Console.WriteLine();
            Console.WriteLine("ProcessStage");
            Console.WriteLine();

            // PipelineInputPdf.StopOnException();
            
            if ( page != -1 )
            {
                basename = ExtractPage(basename, page);
            }

            ExampleStages.RunParserPDF(new VirtualFS(), basename, "input", "output");
        }
        
        static void ExtractPages(string basename, string outputname, IList<int> pages)
        {
            using (var pipeline = new Execution.Pipeline())
            {
                pipeline.Input($"input/{basename}.pdf")
                        .ExtractPages($"input/{outputname}.pdf", pages);
            }
        }

        static void ExtractPage(string basename, IList<int> pages)
        {
            string outputname = $"{basename}-pages";

            ExtractPages(basename, outputname, pages);
        }

        static string ExtractPage(string basename, int p, bool create = true)
        {
            string outputname = $"{basename}-p{p}";

            if(create)
                ExtractPages(basename, outputname, new int[] { p });

            return outputname;
        }
    }
}
