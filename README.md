PDFIndexer
===============


Useful and easy way to get text from pdf (including metadata)

Which a single line you can add a batch of PDF's
And with other single line you can search exactly where she is on the text (or more!)


# Architecture #

 * https://www.nuget.org/packages/PDFParser-CSharp/

 To Install using Nuget PM
 ```
 Install-Package PDFParser-CSharp -Version 1.2.2
 ```


## How to use ##

General use:
``` CSharp
    //TO ADD A BATCH OF PDF'S
    ProcessPDF.AddPDFs(new List<string>() { path });
    
    //TO SEARCH OVER THEM
    var result = ProcessPDF.GetVisualResults("{your search word}");
```


To use:
``` CSharp
            string path = "path with my pdf"
            TextExtractor te = new TextExtractor();
            var list = te.ExtractLinesMetadata(path);
```



## Methods ##

ExtractFullText -> Extract full text as a single string

ExtractWordsMetadata -> Extract every single word with metadata (text, Point X, Point Y, Width and Height)

ExtractLinesMetada -> Extract every single word with metadata (text, Point X, Point Y, Width and Height)

GeIndexMetadata -> To create a hOCR or other xml pattern page, we have this class with all text and points of every line and word.


In all cases you can use string or stream to pass the pdf document.


## Main Methods ##

* AddPDFs -> receive a list of strings to process and save
* GetVisualResults -> Recieve a string and search on the metadata database
** The result should be a list of SampleObject with the word, the position and others metadatas for each word found
    ``` CSharp
    {
        HighlightObject = {
            IndexMetadata Metadata
            List<BoundingBox> HighlightedWords
            string Keyword
            int PageNumber
        },
        Metadata = {
            string Text
            List<PdfMetadata> ListOfLines
            List<PdfMetadata> ListOfWords
            string PDFURI
        },
        ImageUri = "https://{uri_image_path}"
    };
    ```


## Expected Results ##

### ExtractFullText ###
``` CSharp
"some text of entire page (or pages)"
```

### ExtractWordsMetadata ###
``` CSharp
[
    {
        Text = "some"
        X = 150.233
        Y = 88.45
        Width = 12.2
        Height = 11.82
        PageInfo =  {
                        PageNumber = 1,
                        BlobkId = 0
                    }
}

    {
        Text = "text"
        X = 170.233
        Y = 88.45
        Width = 12.2
        Height = 11.82
        PageInfo =  {
                        PageNumber = 1,
                        BlobkId = 1
                    }
    }
]
```

### ExtractLinesMetada ###
``` CSharp
{
    Text = "some text of entire line"
    X = 150.233
    Y = 88.45
    Width = 12.2
    Height = 11.82
    PageInfo =  {
                    PageNumber = 1,
                    BlobkId = 0
                }
}
```

### GetIndexMetadata ###
``` CSharp
{
    Text = "some text of entire line"
    ListOfLines = 
    [ 
        {
            Text = "some text of entire line"
            X = 150.233
            Y = 88.45
            Width = 12.2
            Height = 11.82
            PageInfo =  {
                            PageNumber = 2,
                            BlobkId = 12
                        }
        },
        ... 
    ]
    ListOfWords = 
    [
        {
            Text = "some"
            X = 150.233
            Y = 88.45
            Width = 12.2
            Height = 11.82
            PageInfo =  {
                            PageNumber = 1,
                            BlobkId = 0
                        }
        }

        {
            Text = "text"
            X = 170.233
            Y = 88.45
            Width = 12.2
            Height = 11.82
            PageInfo =  {
                            PageNumber = 1,
                            BlobkId = 1
                        }
        }
    ]
}
```
