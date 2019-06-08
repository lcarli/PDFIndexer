using System;
using System.Collections.Generic;
using System.Text;
using PDFIndexer.Base;

namespace PDFIndexer.Parser
{
    public class Content : TextStructure
    {
        public TipoDoConteudo ContentType { get; set; }

        public Content() { }

        public Content(TextStructure structure, TipoDoConteudo type)
        {
            this.FontName = structure.FontName;
            this.FontSize = structure.FontSize;
            this.FontStyle = structure.FontStyle;
            this.Text = structure.Text;
            this.TextAlignment = structure.TextAlignment;
            this.ContentType = type;
        }
    }
}
