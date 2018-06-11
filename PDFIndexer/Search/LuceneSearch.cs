using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PDFIndexer.Search
{
    static class LuceneSearch
    {
        static string currentDirectory = System.IO.Directory.GetCurrentDirectory();
        public static string _luceneDir = Path.Combine(currentDirectory, "lucene_index");
        private static FSDirectory _directoryTemp;
        private static string field = "Text";


        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }


        public static void LuceneDirStart()
        {
            if (!System.IO.Directory.Exists(_luceneDir))
            {
                System.IO.Directory.CreateDirectory(_luceneDir);
            }
            else
            {
                System.IO.Directory.Delete(_luceneDir, true);
            }
        }


        private static void _addToLuceneIndex(IndexMetadata data, IndexWriter writer)
        {
            // add new index entry
            var doc = new Document();

            // add lucene fields mapped to db fields
            doc.Add(new TextField("Text", data.Text, Field.Store.YES));
            doc.Add(new TextField("Id", data.Id.ToString(), Field.Store.YES));
            doc.Add(new TextField("Words", JsonConvert.SerializeObject(data.ListOfWords), Field.Store.YES));
            doc.Add(new TextField("Lines", JsonConvert.SerializeObject(data.ListOfLines), Field.Store.YES));
            writer.AddDocument(doc);
        }


        public static void AddUpdateLuceneIndex(IEnumerable<IndexMetadata> sampleDatas)
        {
            // remove older index entry
            //ClearLuceneIndexRecord(data.Id);
            ClearLuceneIndex();


            // init lucene
            Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

            IndexWriterConfig iwc = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
                // Add new documents to an existing index:
            iwc.OpenMode = OpenMode.CREATE_OR_APPEND;

            using (var writer = new IndexWriter(_directory, iwc))
            {
                // add data to lucene search index (replaces older entry if any)
                foreach (var sampleData in sampleDatas)
                {
                    _addToLuceneIndex(sampleData, writer);
                }
                writer.Dispose();
            }
        }


        public static void AddUpdateLuceneIndex(IndexMetadata sampleData)
        {
            AddUpdateLuceneIndex(new List<IndexMetadata> { sampleData });
        }

        public static void ClearLuceneIndexRecord(Guid record_id)
        {
            // init lucene
            Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

            IndexWriterConfig iwc = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);

            using (var writer = new IndexWriter(_directory, iwc))
            {
                // remove older index entry
                TermQuery searchQuery = new TermQuery(new Term("Id", record_id.ToString()));
                writer.DeleteDocuments(searchQuery);

                writer.Dispose();
            }
        }

        public static bool ClearLuceneIndex()
        {
            try
            {
                Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

                IndexWriterConfig iwc = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);

                using (var writer = new IndexWriter(_directory, iwc))
                {
                    writer.DeleteAll();

                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private static IndexMetadata _mapLuceneDocumentToData(Document doc)
        {
            return new IndexMetadata()
            {
                Id = Guid.Parse(doc.Get("Id")),
                Text = doc.Get("Text"),
                ListOfWords = JsonConvert.DeserializeObject<List<PdfMetadata>>(doc.Get("Words")),
                ListOfLines = JsonConvert.DeserializeObject<List<PdfMetadata>>(doc.Get("Lines")),
                //PDFURI = doc.Get("URI")
            };
        }

        private static IEnumerable<IndexMetadata> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }
        private static IEnumerable<IndexMetadata> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits,
            IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }

        private static IEnumerable<IndexMetadata> _search (string searchQuery, string searchField = "")
        {
            LuceneDirStart();
            AddUpdateLuceneIndex(DataForTest.GetAll());


            if (String.IsNullOrWhiteSpace(searchField))
            {
                searchField = field;
            }

            //NewTest
            Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            QueryParser parser = new QueryParser(LuceneVersion.LUCENE_48, searchField, analyzer);

            Query query = parser.Parse(searchQuery);


            using (IndexReader reader = DirectoryReader.Open(_directory))
            {
                // set up lucene searcher
                IndexSearcher searcher = new IndexSearcher(reader);
                var result = searcher.Search(query, null, 10);
                return _mapLuceneToDataList(result.ScoreDocs.ToList(), searcher);
            }
        }

        public static IEnumerable<IndexMetadata> Search(string input, string fieldName = "")
        {
            if (string.IsNullOrEmpty(input)) return new List<IndexMetadata>();

            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            return _search(input, fieldName);
        }

        public static IEnumerable<IndexMetadata> SearchDefault(string input, string fieldName = "")
        {
            return string.IsNullOrEmpty(input) ? new List<IndexMetadata>() : _search(input, fieldName);
        }

        public static IEnumerable<IndexMetadata> GetAllIndexRecords()
        {
            // validate search index
            if (!System.IO.Directory.EnumerateFiles(_luceneDir).Any()) return new List<IndexMetadata>();


            using (IndexReader reader = DirectoryReader.Open(_directoryTemp))
            {
                // set up lucene searcher
                var searcher = new IndexSearcher(reader);
                Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
                //var docs = new List<Document>();
                //var term = reader.;
                //while (term.Next()) docs.Add(searcher.Doc(term.Doc));
                //reader.Dispose();
                //searcher.Dispose();
                //return _mapLuceneToDataList(docs);
                return null;
            }
        }
    }
}
