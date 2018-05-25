﻿using PDFIndexer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PDFIndexer.Base
{
    partial class VirtualFS : IVirtualFS
    {
        static IVirtualFS g_vfs = new VirtualFS();

        public Stream OpenReader(string filename)
        {
            System.Diagnostics.Debug.WriteLine($"READ: {filename}");
            return new FileStream(filename, FileMode.Open, FileAccess.Read);
        }

        public Stream OpenWriter(string filename)
        {
            System.Diagnostics.Debug.WriteLine($"WRITE: {filename}");

            string folderName = Path.GetDirectoryName(filename);
            if(!Directory.Exists(folderName))
            {
                if(Path.IsPathRooted(folderName))
                {
                    throw new NotImplementedException();
                }
                else
                {
                    DirectoryInfo directory = new DirectoryInfo(".");
                    directory.CreateSubdirectory(folderName);
                }
            }

            return new FileStream(filename, FileMode.Create);
        }        
    }
}
