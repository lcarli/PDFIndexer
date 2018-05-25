﻿using PDFIndexer.Base;
using PDFIndexer.Execution;
using PDFIndexer.ExecutionStats;
using PDFIndexer.Parser;
using PDFIndexer.PDFCore;
using PDFIndexer.PDFText;
using PDFIndexer.TextStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PDFIndexer
{
    public class ExamplesAzure
    {
        public static void RunCreateArtigos(IVirtualFS virtualFS, string basename, string inputfolder, string tmpfolder, string outputfolder)
        {
            VirtualFS.ConfigureFileSystem(virtualFS);

            PdfReaderException.ContinueOnException();

            Pipeline pipeline = new Pipeline();

            var conteudo = GetTextLines(pipeline, basename, inputfolder, tmpfolder) // use temp folder
                            .ConvertText<CreateTextLineIndex, TextLine>()
                            .ConvertText<PreCreateStructures, TextLine2>()
                            .ConvertText<CreateStructures2, TextStructure>()
                            .ConvertText<PreCreateTextSegments, TextStructureAgg>()
                            .ConvertText<AggregateStructures, TextStructure>()
                            .ConvertText<CreateTextSegments, TextSegment>()
                            .ConvertText<CreateTreeSegments, TextSegment>()
                                .Log<AnalyzeSegmentTitles>($"{tmpfolder}/{basename}/segment-titles-tree.txt")
                            .ConvertText<TransformConteudo, Conteudo>()
                            .ToList();
            
            var createArticle = new TransformArtigo();
            var artigos = createArticle.Create(conteudo);
            createArticle.CreateXML(artigos, $"{outputfolder}/{basename}", basename);
        }
        public static void RunCreateArtigosJson(IVirtualFS virtualFS, string basename, string inputfolder, string tmpfolder, string outputfolder)
        {
            VirtualFS.ConfigureFileSystem(virtualFS);

            PdfReaderException.ContinueOnException();

            Pipeline pipeline = new Pipeline();

            var conteudo = GetTextLines(pipeline, basename, inputfolder, tmpfolder) // use temp folder
                            .ConvertText<CreateTextLineIndex, TextLine>()
                            .ConvertText<PreCreateStructures, TextLine2>()
                            .ConvertText<CreateStructures2, TextStructure>()
                            .ConvertText<PreCreateTextSegments, TextStructureAgg>()
                            .ConvertText<AggregateStructures, TextStructure>()
                            .ConvertText<CreateTextSegments, TextSegment>()
                            .ConvertText<CreateTreeSegments, TextSegment>()
                                .Log<AnalyzeSegmentTitles>($"{tmpfolder}/{basename}/segment-titles-tree.txt")
                            .ConvertText<TransformConteudo, Conteudo>()
                            .ToList();

            var createArticle = new TransformArtigo();
            var artigos = createArticle.Create(conteudo);
            createArticle.CreateJson(artigos, $"{outputfolder}/{basename}", basename);
        }

        static PipelineText<TextLine> GetTextLines(Pipeline pipeline, string basename, string inputfolder, string tmpfolder)
        {
            var result =
            pipeline.Input($"{inputfolder}/{basename}.pdf")
                    .Output($"{tmpfolder}/{basename}/parser-output.pdf")
                    .AllPages<CreateTextLines>(page =>
                              page.ParsePdf<PreProcessTables>()
                                  .ParseBlock<IdentifyTables>()             // 1
                              .ParsePdf<PreProcessImages>()
                                  .ParseBlock<BasicFirstPageStats>()        // 2
                                                                            //.Validate<RemoveOverlapedImages>().ShowErrors(p => p.Show(Color.Blue))
                                  .ParseBlock<RemoveOverlapedImages>()      // 3
                              .ParsePdf<ProcessPdfText>()                   // 4
                                                                            //.Validate<RemoveSmallFonts>().ShowErrors(p => p.ShowText(Color.Green))
                                  .ParseBlock<RemoveSmallFonts>()           // 5
                                                                            //.Validate<MergeTableText>().ShowErrors(p => p.Show(Color.Blue))
                                  .ParseBlock<MergeTableText>()             // 6
                                                                            //.Validate<HighlightTextTable>().ShowErrors(p => p.Show(Color.Green))
                                  .ParseBlock<HighlightTextTable>()         // 7
                                  .ParseBlock<RemoveTableText>()            // 8
                                  .ParseBlock<ReplaceCharacters>()          // 9
                                  .ParseBlock<GroupLines>()                 // 10
                                  .ParseBlock<RemoveTableDotChar>()         // 11
                                      .Show(Color.Yellow)
                                      .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                                  .ParseBlock<RemoveHeaderImage>()          // 12
                                  .ParseBlock<FindInitialBlocksetWithRewind>()  // 13
                                      .Show(Color.Gray)
                                  .ParseBlock<BreakColumnsLight>()          // 14
                                                                            //.ParseBlock<BreakColumns>()
                                  .ParseBlock<AddTableSpace>()              // 15
                                  .ParseBlock<RemoveTableOverImage>()       // 16
                                  .ParseBlock<RemoveImageTexts>()           // 17
                                  .ParseBlock<AddImageSpace>()              // 18
                                      .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                                  .ParseBlock<RemoveFooter>()               // 19
                                  .ParseBlock<AddTableHorizontalLines>()    // 20
                                  .ParseBlock<RemoveBackgroundNonText>()    // 21
                                      .ParseBlock<BreakColumnsRewrite>()    // 22

                                  .ParseBlock<BreakInlineElements>()        // 23
                                  .ParseBlock<ResizeBlocksets>()            // 24
                                  .ParseBlock<ResizeBlocksetMagins>()       // 25
                                    .ParseBlock<OrderBlocksets>()           // 26

                                  .ParseBlock<OrganizePageLayout>()         // 27
                                  .ParseBlock<MergeSequentialLayout>()      // 28
                                  .ParseBlock<ResizeSequentialLayout>()     // 29
                                      .Show(Color.Orange)
                                      .ShowLine(Color.Black)

                                  .ParseBlock<CheckOverlap>()               // 30

                                      .Validate<CheckOverlap>().ShowErrors(p => p.Show(Color.Red))
                                      .Validate<ValidatePositiveCoordinates>().ShowErrors(p => p.Show(Color.Red))
                                  .PrintWarnings()
                    );

            return result;
        }
    }
}
