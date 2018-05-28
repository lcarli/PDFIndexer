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
    }

    {
        Text = "text"
        X = 170.233
        Y = 88.45
        Width = 12.2
        Height = 11.82
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
}
```