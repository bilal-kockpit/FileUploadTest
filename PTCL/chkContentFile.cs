using FileTypeChecker;
using FileTypeChecker.Abstracts;
using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PTCL
{
    public class chkContentFile
    {
        public static Tuple<bool,string> Validate(string filePath, string fileExtn)
        { 
            bool flag = true;
            string msg = "";
            using (var fileStream = File.OpenRead(filePath))
            {
                var isRecognizableType = FileTypeValidator.IsTypeRecognizable(fileStream);

                if (!isRecognizableType)
                {

                    msg = "Invalid file type!";
                    flag = false;
                }
                else
                {
                    IFileType fileType = FileTypeValidator.GetFileType(fileStream);
                    if (fileExtn.ToLower().Contains(fileType.Extension))
                    {
                        flag = true;
                        msg = fileType.Extension;

                    }                    
                    else
                    {                        
                        flag = false;
                        msg = "Invalid file type!";
                    }
                }              

            }
            return new Tuple<bool, string>(flag, msg);
        }
    }
}

