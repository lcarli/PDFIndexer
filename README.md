# PDFIndexer

Useful and easy way to get text from pdf (including metadata)

To use:
'''
            TextExtractor te = new TextExtractor();
            var list = te.ExtractLinesMetadata(path, true);
'''



Methods:

ExtractFullText -> Extract full text as a single string

ExtractWordsMetadata -> Extract every single word with metadata (text, Point X, Poin Y, Width and Height)

ExtractLinesMetada -> Extract every single word with metadata (text, Point X, Poin Y, Width and Height)


In all cases you can use string or stream to pass the pdf document.
