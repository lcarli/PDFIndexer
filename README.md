PDFIndexer
===============


Useful and easy way to get text from pdf (including metadata)


# Architecture #

 * https://www.nuget.org/packages/PDFParser-CSharp/

 To Install using Nuget PM
 ```
 Install-Package PDFParser-CSharp -Version 1.2.1
 ```


## How to use ##

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