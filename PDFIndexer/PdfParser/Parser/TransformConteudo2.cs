﻿using PDFIndexer.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDFIndexer.Base;
using System.Text.RegularExpressions;

namespace PDFIndexer.Parser
{
    class TransformConteudo2 : IAggregateStructure<TextSegment, Conteudo>
    {
        int count = 0;
        public bool Aggregate(TextSegment line)
        {
            return false;
        }

        public Conteudo Create(List<TextSegment> segments)
        {
            count++;
            TextSegment segment = ProcessSingularBodyBehaviors(segments[0]);

            segment = ProcessExclusiveText(segments[0]);

            int page = -1;
            if (segment.Body.Count() > 0)
                page = segment.Body[0].Lines[0].PageInfo.PageNumber;

            string titulo = null;
            string hierarchy = null;
            string body = null;
            string caput = null;
            string possibleData = null;
            List<Autor> autores = null;
            string data = null;
            List<Anexo> anexos = null;
            //just for process results
            List<string> resultProcess = new List<string>() { null, null, null };

            // Hierarquia
            var hierarquiteTitulo = segment.Title.Select(t => CleanupBreaklinesAndHyphens(t.Text)).ToArray();

            // Texto
            string texto = String.Join("\n\n", segment.Body.Select(GenerateText));


            //Definindo Titulo e hierarquia
            int idxTitle = segment.Title.Count() - 1;

            if (idxTitle == 0)
            {
                titulo = segment.Title[0].Text;
            }
            else if (idxTitle > 0)
            {
                for (int i = 0; i < segment.Title.Count() - 1; i++)
                {
                    hierarchy = hierarchy + segment.Title[i].Text + ":";
                }
                titulo = ProcessSingularTitles(segment.Title, segment.Title[idxTitle].Text);
            }

            //Definindo Caput
            if (segment.Body.Count() > 1)
            {
                if (segment.Body[0].TextAlignment == TextAlignment.RIGHT && segment.Body[1].TextAlignment == TextAlignment.JUSTIFY)
                    caput = segment.Body[0].Text;
            }


            //Definindo Assinatura, Cargo e Data
            int idxSigna = 0;
            //Se contiver anexo...
            if (caput != null)
            {
                idxSigna = segment.Body.ToList().FindIndex(2, s => s.TextAlignment == TextAlignment.RIGHT);
            }
            else //Caso não tenha anexo
            {
                idxSigna = segment.Body.ToList().FindLastIndex(s => s.TextAlignment == TextAlignment.JUSTIFY) + 1;
            }

            autores = new List<Autor>();
            if (idxSigna > 0 && idxSigna < segment.Body.Count())
            {
                resultProcess.Clear();
                resultProcess = ProcessSignatureAndRole(segment.Body[idxSigna].Lines);
                autores.Add(new Autor() { Assinatura = resultProcess[0], Cargo = resultProcess[1] });
                data = resultProcess[2];
            }

            var startLine = (caput != null) ? segment.Body.Skip(1) : segment.Body;

            //Definindo Body
            if (caput != null && idxSigna > 0 && idxSigna < segment.Body.Count())
            {
                body = String.Join("\n", startLine.Take(idxSigna - 1).Select(GenerateText));

                possibleData = segment.Body[idxSigna - 1].Text;
            }
            else if (idxSigna > 0 && idxSigna < segment.Body.Count())
            {
                var valueToTake = idxSigna - 1;
                if (valueToTake == 0)
                    valueToTake = 1;
                body = String.Join("\n", startLine.Take(valueToTake).Select(GenerateText));

                possibleData = segment.Body[idxSigna - 1].Text;
            }
            else
            {
                if (segment.Body.Count() > 0)
                {
                    body = String.Join("\n", startLine.Take(segment.Body.Count()).Select(GenerateText));
                    possibleData = segment.Body[segment.Body.Count() - 1].Text;
                }
            }

            //Definindo o Anexo se existir e verificando se necessita juntar as assinaturas
            var resultSignAndAnexo = ProcessAnexoOrSign(segment.Body, idxSigna);
            anexos = new List<Anexo>();
            if (resultSignAndAnexo[1].Count() > 0)
            {
                anexos.Add(ConcatAnexo(resultSignAndAnexo[1]));
            }

            if (resultSignAndAnexo[0].Count() > 0)
            {
                var result = ProcessListOfSignatures(resultSignAndAnexo[0].ToList());
                foreach (Autor item in result)
                {
                    autores.Add(item);
                }
            }

            //Verificando se Data ficou no Corpo
            if (data == null)
                if (possibleData != null)
                    data = HasData(possibleData);

            return new Conteudo()
            {
                IntenalId = count,
                Page = page,
                Hierarquia = hierarchy,
                Titulo = CleanupBreaklinesAndHyphens(titulo),
                Caput = CleanupBreaklinesAndHyphens(caput),
                Corpo = ReplaceBreaklinesAndHyphensWithHtml(body),
                Autor = autores,
                Data = data,
                Anexos = anexos,
                HierarquiaTitulo = hierarquiteTitulo,
                Texto = texto
            };
        }

        string GenerateText(TextStructure s)
        {
            string prefix = "";

            if(s.TextAlignment == TextAlignment.JUSTIFY)
            {
                return s.Text.Replace("\t", "\n\t").TrimStart('\n');
            }

            if (s.TextAlignment == TextAlignment.LEFT || s.TextAlignment == TextAlignment.UNKNOWN)
            {
                PdfReaderException.Warning("s.TextAlignment == TextAlignment.LEFT || s.TextAlignment == TextAlignment.UNKNOWN");
            }

            if (s.TextAlignment == TextAlignment.CENTER)
                prefix = "\t\t";

            if (s.TextAlignment == TextAlignment.RIGHT)
                prefix = "\t\t\t\t";

            var lines = s.Text.Split('\n').Select(l => prefix + l);

            string text = String.Join("\n", lines);

            return text;
        }

        string CleanupHyphens(string body)
        {
            if (body == null) return null;

            if (!body.Contains("-"))
                return body;

            // Algumas vezes, a deteccao de texto indica que a continuacao
            // do hifen cai sobre o parágrafo seguinte. Nesse caso, aceitamos
            // que pode ser um erro e consideramos igual a uma quebra de linha
            string incluirQuebraParagrafo = body.Replace("-\n\n", "-\n");
            
            string texto = HifenUtil.ExtrairHifen(incluirQuebraParagrafo);

            return texto;
        }

        string CleanupBreaklines(string body)
        {
            if (body == null) return null;

            return body.Replace("\n", " ");
        }

        string CleanupBreaklinesAndHyphens(string body)
        {
            if (body == null) return null;

            return CleanupBreaklines(CleanupHyphens(body));
        }

        string ReplaceBreaklinesAndHyphensWithHtml(string body)
        {
            if (body == null) return null;

            var cleanBody = CleanupHyphens(body);
            var lines = cleanBody.Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            var cleanLines = lines.Select(l => CleanupBreaklines(l));

            return "<p>" + String.Join("</p><p>", cleanLines) + "</p>";
        }

        string RemoveDataFromBody(string body, string data)
        {
            return body.Replace(data, "");
        }

        string HasData(string body)
        {

            string LastLine = body.Split('\n').Last();

            string result = null;

            var match = Regex.Match(LastLine, @"(.+?[a-zA-Z]+, \d\d de [a-zA-Z]+ de \d{4})");

            if (match.Success)
                return LastLine;

            return result;
        }

        List<TextStructure[]> ProcessAnexoOrSign(TextStructure[] structures, int idxSigna)
        {
            List<TextStructure> sign = new List<TextStructure>();
            List<TextStructure> anexo = new List<TextStructure>();
            IEnumerable<TextStructure> discover;

            if (idxSigna > 0 && structures.Count() > idxSigna)
            {
                discover = structures.Skip(idxSigna + 1).Take(structures.Count() - idxSigna);

                foreach (var item in discover)
                {
                    if (item.TextAlignment == TextAlignment.JUSTIFY)
                    {
                        anexo.Add(item);
                    }
                    else
                    {
                        sign.Add(item);
                    }
                }
            }
            return new List<TextStructure[]>() { sign.ToArray(), anexo.ToArray() };
        }

        TextSegment ProcessExclusiveText(TextSegment segment)
        {
            foreach (TextStructure item in segment.Body)
            {
                if (item.Text.ToLower().Contains("o presidente da república") || item.Text.ToLower().Contains("a presidenta da república"))
                    item.TextAlignment = TextAlignment.JUSTIFY;
                if (item.Text.Contains("Parágrafo único"))
                {
                    if (item.Text.Substring(0, 15) == "Parágrafo único")
                        item.TextAlignment = TextAlignment.JUSTIFY;
                }
                if (item.Text.Contains("Art."))
                {
                    if (item.Text.Substring(0, 4) == "Art.")
                        item.TextAlignment = TextAlignment.JUSTIFY;
                }
            }
            return segment;
        }

        List<string> ProcessSignatureAndRole(List<TextLine> lines)
        {

            string signature = null;
            string role = null;
            string date = null;


            foreach (var item in lines)
            {
                if (item.Text.ToUpper() == item.Text)
                {
                    signature = signature + "\n" + item.Text;
                }
                else if (item.FontStyle == "Italic" || item.FontName.ToLower().Contains("italic"))
                {
                    signature = signature + "\n" + item.Text;
                }
                else
                {
                    var match = Regex.Match(item.Text, @"(\,? [0-9]* [a-zA-Z]* [a-zA-Z]* [a-zA-Z]* [0-9]*)");
                    var match2 = Regex.Match(item.Text, @"([0-9]*\/[0-9]*\/[0-9]*)");

                    if (match.Success)
                        date = item.Text;
                    else if (match2.Success)
                        date = item.Text;
                    else
                        role = role + "\n" + item.Text;
                }
            }

            return new List<string>() { signature, role, date };
        }

        List<Autor> ProcessListOfSignatures(List<TextStructure> signatures)
        {
            List<Autor> autores = new List<Autor>();
            foreach (TextStructure item in signatures)
            {
                Autor autor = new Autor();
                foreach (var line in item.Lines)
                {
                    if (line.Text.ToUpper() == line.Text)
                    {
                        if (!String.IsNullOrWhiteSpace(autor.Assinatura))
                        {
                            autor.Assinatura = line.Text;
                        }
                        else
                        {
                            autor = new Autor() { Assinatura = line.Text };
                            autores.Add(autor);
                            continue;
                        }
                    }
                    else
                    {
                        autor.Cargo = line.Text;
                    }

                    autores.Add(autor);
                }
            }
            return autores;
        }

        string ProcessSingularTitles(TextStructure[] segmentTitles, string title)
        {
            string newTitle = null;

            //If title was single data (e.g. "Em 25 de Dezembro de 2016")
            var match = Regex.Match(title, @"((Em|EM|DIA|dia) [0-9]*(\°?|o?|º?) (de|DE) [a-zA-Z]+ (de|DE) \d{4})");
            //Get the last position before title and concat with it.
            if (match.Success)
                newTitle = $"{segmentTitles[segmentTitles.Count() - 2].Text} - {title.ToUpper()}";


            //If title is specific like "Relação N°"
            var match2 = Regex.Match(title, @"(RELAÇÃO (No|N°|Nº)\-? [0-9]*\/[0-9]*)");
            //Get the last position before title and concat with it.
            if (match2.Success)
                newTitle = $"{segmentTitles[segmentTitles.Count() - 2].Text} - {title.ToUpper()}";

            return (newTitle == null) ? title : newTitle;
        }

        TextSegment ProcessSingularBodyBehaviors(TextSegment segment)
        {
            var match = Regex.Match(segment.Body[0].Text, @"(.*? (No|N°|Nº)\-? ([0-9]+\.?(\/)?[0-9]*), [a-zA-Z]* [0-9]* [a-zA-Z]* [a-zA-Z]* [a-zA-Z]* [0-9]*)");

            if (match.Success)
            {
                int TOLERANCE_PERIOD_CHAR = 2;
                if (segment.Body[0].Text.Length - match.Length < TOLERANCE_PERIOD_CHAR)
                {
                    segment.Body[0].TextAlignment = TextAlignment.CENTER;
                    List<TextStructure> newTitle = segment.Title.ToList();
                    newTitle.Add(segment.Body[0]);
                    segment.Title = newTitle.ToArray();
                    segment.Body = segment.Body.Where(b => b != segment.Body[0]).ToArray();
                }
            }


            var match2 = Regex.Match(segment.Body[0].Text, @"(.*? [a-zA-Z]* [0-9]* [a-zA-Z]* [a-zA-Z]* [a-zA-Z]* [0-9]*)");

            if (match2.Success)
            {
                if (match2.Length == segment.Body[0].Text.Length)
                {
                    segment.Body[0].TextAlignment = TextAlignment.CENTER;
                    List<TextStructure> newTitle = segment.Title.ToList();
                    newTitle.Add(segment.Body[0]);
                    segment.Title = newTitle.ToArray();
                    segment.Body = segment.Body.Where(b => b != segment.Body[0]).ToArray();
                }
            }

            //If doc is PAUTA DE JULGAMENTO
            if (segment.Body[0].Text.Count() > 20)
            {
                if (segment.Body[0].Text.Substring(0, 19) == "PAUTA DE JULGAMENTO")
                {
                    segment.Body[0].TextAlignment = TextAlignment.CENTER;
                    List<TextStructure> newTitle = segment.Title.ToList();
                    newTitle.Add(segment.Body[0]);
                    segment.Title = newTitle.ToArray();
                    segment.Body = segment.Body.Where(b => b != segment.Body[0]).ToArray();
                }
            }

            return segment;
        }

        Anexo ConcatAnexo(TextStructure[] structures)
        {
            return new Anexo(String.Join("\n", structures.Take(structures.Count()).Select(t => t.Text))) { };
        }

        public void Init(TextSegment line)
        {

        }
    }
}
