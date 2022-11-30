using FileTypeChecker;
using FileTypeChecker.Abstracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PTCL
{
    public class fileEncryption
    {
        public static Tuple<bool, string> validateFile(string filePath, string fileExtn)
        {
            bool flag = true;
            string msg = "";
            try {
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
                        if (fileExtn.Contains(fileType.Extension))
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
               
            }
            catch(Exception ex)
            {
                msg = ex.Message;
                flag = false;

            }
            return new Tuple<bool, string>(flag, msg);
        }

        public static  Tuple<bool, string> encryptFileUpload(string fileUploadDirectoryPath, IFormFile file, string encrytionKey)
        {    

            bool flag = true;
            string msg = "";
            try
            {
                if (!Directory.Exists(fileUploadDirectoryPath))
                {
                    Directory.CreateDirectory(fileUploadDirectoryPath);
                }
                byte[] bFile = new byte[file.Length];
                var filePath = Path.Combine(fileUploadDirectoryPath, file.FileName);
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(encrytionKey);
                byte[] Key = Encoding.UTF8.GetBytes("asdf!@#$1234ASDF");
                if (!System.IO.File.Exists(filePath))                {
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    RijndaelManaged rmCryp = new RijndaelManaged();
                    CryptoStream cs = new CryptoStream(fs, rmCryp.CreateEncryptor(Key, Key), CryptoStreamMode.Write);


                    foreach (var data in bFile)
                    {
                        cs.WriteByte((byte)data);
                    }
                    cs.Close();
                    fs.Close();


                }
            }

            
            catch(Exception ex)
            {
                msg = ex.Message;
                flag = false;
            }
            return new Tuple<bool, string>(flag, msg);
        }
    }
    
}

