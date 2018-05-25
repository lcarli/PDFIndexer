using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Parser
{
    class Artigo
    {
        public Metadados Metadados { get; set; }
        public Conteudo Conteudo { get; set; }
        public List<Anexo> Anexos { get; set; }
    }
}
