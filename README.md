PDFIndexer
===============


Useful and easy way to get text from pdf (including metadata)


# Architecture #

 * Creating a Nuget pack, yet.


## How to use ##

To use:
``` CSharp
            string path = "path with my pdf"
            TextExtractor te = new TextExtractor();
            var list = te.ExtractLinesMetadata(path);
```



## Methods ##

ExtractFullText -> Extract full text as a single string

ExtractWordsMetadata -> Extract every single word with metadata (text, Point X, Poin Y, Width and Height)

ExtractLinesMetada -> Extract every single word with metadata (text, Point X, Poin Y, Width and Height)


In all cases you can use string or stream to pass the pdf document.
