﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Helper
{
    public static class FileManager
    {
        public static string Save(string rootPath, string folder, IFormFile file)
        {
            string filename = file.FileName;
            filename = filename.Length <= 64 ? filename : (filename.Substring(filename.Length - 64, 64));
            filename = Guid.NewGuid().ToString() + filename;

            string path = Path.Combine(rootPath, folder, filename);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return filename;
        }
    }
}
