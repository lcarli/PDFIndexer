using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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



        public static void _addToLuceneIndex<T>(T obj, IndexWriter writer) where T : class
        {
            var doc = new Document();

            var props = obj.GetType().GetProperties().ToList();

            foreach (var item in props)
            {
                doc.Add(new TextField(item.Name, JsonConvert.SerializeObject(item.GetValue(obj)), Field.Store.YES));
            }

            writer.AddDocument(doc);
        }

        public static void AddUpdateLuceneIndex<T>(IEnumerable<T> sampleDatas) where T : class
        {
            // remove older index
            LuceneDirStart();
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
                    _addToLuceneIndex<T>(sampleData, writer);
                }
                writer.Dispose();
            }
        }


        public static void AddUpdateLuceneIndex<T>(T sampleData) where T : class
        {
            AddUpdateLuceneIndex(new List<T> { sampleData });
        }


        //Still working on this
        private static void ClearLuceneIndexRecord(Guid record_id)
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

        private static T _mapLuceneDocumentToData<T>(Document doc) where T : class
        {
            var obj = Activator.CreateInstance(typeof(T));

            var props = obj.GetType().GetProperties().ToList();

            foreach (var item in props)
            {
                if (item.PropertyType == typeof(Guid))
                {
                    item.SetValue(obj, Guid.Parse(doc.Get(item.Name).Replace("\"", "").Replace("\\", "")));
                }
                else
                {
                    Type t = item.PropertyType;
                    var a = item.GetType();

                    item.SetValue(obj, JsonConvert.DeserializeObject(doc.Get(item.Name), t));
                }
            }

            return obj as T;
        }

        public static T GetObject<T>() where T : new()
        {
            return new T();
        }

        private static IEnumerable<T> _mapLuceneToDataList<T>(IEnumerable<Document> hits) where T : class
        {
            return hits.Select(_mapLuceneDocumentToData<T>).ToList();
        }
        private static IEnumerable<T> _mapLuceneToDataList<T>(IEnumerable<ScoreDoc> hits, IndexSearcher searcher) where T : class
        {
            return hits.Select(hit => _mapLuceneDocumentToData<T>(searcher.Doc(hit.Doc))).ToList();
        }

        private static IEnumerable<T> _search<T>(string searchQuery, string searchField) where T : class
        {
            Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            QueryParser parser = new QueryParser(LuceneVersion.LUCENE_48, searchField, analyzer);

            Query query = parser.Parse(searchQuery);


            using (IndexReader reader = DirectoryReader.Open(_directory))
            {
                // set up lucene searcher
                IndexSearcher searcher = new IndexSearcher(reader);
                var result = searcher.Search(query, null, 10);
                return _mapLuceneToDataList<T>(result.ScoreDocs.ToList(), searcher);
            }
        }

        public static IEnumerable<T> Search<T>(string input, string fieldName = "") where T : class
        {
            if (string.IsNullOrEmpty(input)) return new List<T>();

            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            return _search<T>(input, fieldName);
        }

        public static IEnumerable<T> SearchDefault<T>(string input, string fieldName = "") where T : class
        {
            return string.IsNullOrEmpty(input) ? new List<T>() : _search<T>(input, fieldName);
        }

        public static IEnumerable<T> GetAllIndexRecords<T>() where T : class
        {
            List<T> newList = new List<T>();

            using (IndexReader reader = DirectoryReader.Open(_directory))
            {
                for (int i = 0; i < reader.MaxDoc; i++)
                {
                    try
                    {
                        Document doc = reader.Document(i);
                        newList.Add(_mapLuceneDocumentToData<T>(doc));
                    }
                    catch (Exception)
                    {
                        //Do something
                    }
                }
            }
            return newList;
        }
    }
}
