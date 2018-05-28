using PDFIndexer.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDFIndexer.PdfParser.PDFCore
{
    class GroupWords : IProcessBlock
    {
        public BlockPage Process(BlockPage page)
        {
            GroupFontLineHelper groupFont = null;
            BlockLine line = null;
            IBlock last = null;
            var result = new BlockPage();


            foreach (var block in page.AllBlocks)
            {
                if (last != null)
                {
                    if (IsEndOfWord(block) || IsEndOfLine(block, line))
                    {
                        last = null;
                        result.Add(line);
                    }
                    else
                    {
                        line.Text += block.GetText();
                        line.Width = block.GetX() + block.GetWidth() - line.GetX();

                        groupFont.MergeFont((Block)block);
                    }
                }

                if ((last == null))
                {
                    var b = (Block)block;
                    string text = block.GetText();

                    line = new BlockLine()
                    {
                        Text = text,
                        X = block.GetX(),
                        H = block.GetH(),
                        Width = block.GetWidth(),
                        Height = block.GetHeight(),

                        HasBackColor = b.HasBackColor,

                        HasLargeSpace = false,

                        // might be inaccurate 
                        FontFullName = b.FontFullName,
                        FontName = b.FontName,
                        FontSize = b.FontSize, // BE CAREFUL!
                        FontStyle = b.FontStyle
                        // now the settings are done in GroupFontLineHelper
                    };

                    // TODO: validar a entrada duas vezes

                    if (groupFont != null)
                        groupFont.Done();

                    groupFont = new GroupFontLineHelper(line, b);

                    if (line.Width <= 0 || line.Height <= 0)
                        PdfReaderException.AlwaysThrow("line.Width <= 0 || line.Height <= 0");
                }
                last = block;
            }

            if (groupFont != null)
                groupFont.Done();

            return result;
        }

        bool IsEndOfWord(IBlock a)
        {
            if (a != null)
            {
                var space = a.GetText()[a.GetText().Count() - 1];
                if (space == ' ')
                {
                    return true;
                }
            }
            return false;
        }

        bool IsEndOfLine(IBlock block, BlockLine line)
        {
            float startOfBlock = block.GetX();
            float endOfBlock = block.GetX() + block.GetWidth();
            float endOfLine = line.GetX() + line.GetWidth();

            if ((block.GetX() + block.GetWidth() - line.GetX()) < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    class GroupFontLineHelper
    {
        readonly BlockLine _line;
        readonly GroupFontLineItem _currentFont;
        List<GroupFontLineItem> _conflictItems = null;

        public GroupFontLineHelper(BlockLine line, Block block)
        {
            _line = line;
            _currentFont = CreateFont(block);
        }

        // TODO: implementar a diferenca de fonte (MODEM)
        public void MergeFont(Block line)
        {
            if (HasConflict(line))
            {
                if (_conflictItems == null)
                {
                    _conflictItems = new List<GroupFontLineItem>();
                    _conflictItems.Add(_currentFont);
                }

                _conflictItems.Add(CreateFont(line));
            }
        }

        public void Done()
        {
            _line.FontFullName = _currentFont.FontFullName;
            _line.FontName = _currentFont.FontName;
            _line.FontSize = _currentFont.FontSize;
            _line.FontStyle = _currentFont.FontStyle;

            if (_conflictItems != null)
            {
                var regularItem = _conflictItems.Where(f => f.FontStyle == "Regular").FirstOrDefault();
                if (regularItem != null)
                {
                    _line.FontStyle = "Regular";
                    _line.FontName = regularItem.FontName;
                    _line.FontSize = regularItem.FontSize;

                    if (_conflictItems.Where(f => f.FontStyle == "Regular").Count() > 1)
                        PdfReaderException.Warning("Multiple regular fonts");
                }
            }
        }

        bool HasFullConflict(Block block)
        {
            return ((_currentFont.FontFullName != block.FontFullName) ||
                (_currentFont.FontName != block.FontName) ||
                (_currentFont.FontSize != block.FontSize) ||
                (_currentFont.FontStyle != block.FontStyle));
        }
        bool HasConflict(Block block)
        {
            return (_currentFont.FontStyle != block.FontStyle);
        }

        GroupFontLineItem CreateFont(Block block)
        {
            return new GroupFontLineItem
            {
                FontFullName = block.FontFullName,
                FontName = block.FontName,
                FontSize = block.FontSize,
                FontStyle = block.FontStyle
            };
        }
    }
    class GroupFontLineItem
    {
        public string FontFullName;
        public string FontName;
        public float FontSize;
        public string FontStyle;
    }
}
