﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using PDFIndexer.Base;

namespace PDFIndexer.Parser
{
    public class ProcessParserOriginal
    {
        int countPerPage = 1;
        string initialPage = "0001";
        public void XMLWriter(IEnumerable<Artigo> artigos, string doc)
        {

            // TODO: fix it
            // Rollback to previous name
            //string finalURL = ProcessName(artigos.FirstOrDefault(), doc);
            string finalURL = doc;

            var settings = new XmlWriterSettings()
            {
                Indent = true                
            };
            using (Stream virtualStream = VirtualFS.OpenWrite($"{finalURL}.xml"))
            using (XmlWriter writer = XmlWriter.Create(virtualStream, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Artigo");

                foreach (Artigo artigo in artigos)
                {
                    Conteudo conteudo = artigo.Conteudo;
                    Metadados metadados = artigo.Metadados;
                    List<Anexo> anexos = artigo.Anexos;


                    //Writing Metadata
                    writer.WriteStartElement("Metadados");

                    writer.WriteAttributeString("ID", conteudo.IntenalId.ToString());
                    if (metadados.Nome != null)
                        writer.WriteAttributeString("Nome", ConvertBreakline2Space(metadados.Nome));
                    if (metadados.TipoDoArtigo != null)
                        writer.WriteAttributeString("TipoDoArtigo", ConvertBreakline2Space(metadados.TipoDoArtigo));
                    if (conteudo.Hierarquia != null)
                        writer.WriteAttributeString("Hierarquia", ConvertBreakline2Space(conteudo.Hierarquia));
                    if (metadados.Grade != null)
                        writer.WriteAttributeString("Grade", ConvertBreakline2Space(metadados.Grade));
                    if (metadados.NumeroDaPagina >= 0)
                        writer.WriteAttributeString("NumPagina", metadados.NumeroDaPagina.ToString());

                    writer.WriteEndElement();

                    //Writing Body
                    writer.WriteStartElement("Conteudo");

                    if (conteudo.Titulo != null)
                        writer.WriteElementString("Titulo", ConvertBreakline2Space(conteudo.Titulo));
                    if (conteudo.Caput != null)
                        writer.WriteElementString("Caput", conteudo.Caput);
                    if (conteudo.Corpo != null)
                        writer.WriteElementString("Corpo", conteudo.Corpo);
                    if (conteudo.Autor.Count > 0)
                    {
                        writer.WriteStartElement("Autores");
                        foreach (Autor autor in conteudo.Autor)
                        {
                            writer.WriteStartElement("Autor");
                            if (autor.Cargo != null)
                            {
                                if (autor.Assinatura == null)
                                {
                                    writer.WriteString(ConvertBreakline2Space(autor.Cargo));
                                }
                                else
                                {
                                    writer.WriteAttributeString("Cargo", ConvertBreakline2Space(autor.Cargo));
                                    writer.WriteString(ConvertBreakline2Space(autor.Assinatura));
                                }
                            }
                            else
                            {
                                writer.WriteString(ConvertBreakline2Space(autor.Assinatura));
                            }

                            writer.WriteEndElement();

                            //if (autor.Assinatura != null && autor.Assinatura.Length > 3)
                            //    writer.WriteElementString("Assinatura", autor.Assinatura);
                            //if (autor.Cargo != null)
                            //    writer.WriteElementString("Cargo", ConvertBreakline2Space(autor.Cargo));
                        }
                        writer.WriteEndElement();
                    }
                    if (conteudo.Data != null)
                        writer.WriteElementString("Data", conteudo.Data);

                    writer.WriteEndElement();

                    //Writing Anexos
                    if (anexos.Count > 0)
                    {
                        writer.WriteStartElement("Anexos");
                        foreach (Anexo item in anexos)
                        {
                            writer.WriteStartElement("Anexo");
                            if (item.HierarquiaTitulo != null)
                                writer.WriteElementString("Hierarquia", item.HierarquiaTitulo);
                            if (item.Titulo != null)
                                writer.WriteElementString("Titulo", item.Titulo);
                            if (item.Texto != null)
                                writer.WriteElementString("Texto", item.Texto);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();

            }
        }

        private string ProcessName(Artigo artigo, string doc)
        {
            string numpag = artigo.Metadados.NumeroDaPagina.ToString().PadLeft(4, '0');
            string docFinalText = new DirectoryInfo(doc).Name;
            string date = docFinalText.Substring(4, 10).Replace("_", "");
            string globalId = docFinalText.Substring(21, docFinalText.Length - 21);
            string modelNameGlobal = $"{date}-{globalId}";
            string modelNameCustom = null;

            if (numpag == initialPage)
            {
                modelNameCustom = $"{date}-{numpag}-{countPerPage++.ToString().PadLeft(2, '0')}";
            }
            else
            {
                initialPage = numpag;
                countPerPage = 1;
                modelNameCustom = $"{date}-{numpag}-{countPerPage.ToString().PadLeft(2, '0')}";
            }


            return doc.Replace(docFinalText, modelNameCustom);
        }

        string ConvertBreakline2Space(string input)
        {
            string output = input;
            if (input != null && input.Length > 1)
            {
                output = input.Replace("\n", " ");
                if (output.Contains(":"))
                {
                    output = output.Substring(0, output.Length - 1);
                }
                if (output.Substring(0, 1) == " ")
                    output = output.Substring(1, output.Length - 1);
            }
            return output;
        }

        public void XMLWriterMultiple(IEnumerable<Artigo> artigos, string doc)
        {
            int i = 1;
            foreach(var artigo in artigos)
            {
                string doc_i = doc + (i++);
                var artigo_i = new Artigo[] { artigo };

                this.XMLWriter(artigo_i, doc_i);
            }
        }
    }
}
