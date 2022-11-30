using FileTypeChecker;
using FileTypeChecker.Abstracts;
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

        private  Tuple<bool, string> encryptFileUpload(string inputFile, string outputFile)
        {
            bool flag = true;
            string msg = "";
            try
            {
                string password = @"myKey123"; // Your Key Here
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,  RMCrypto.CreateEncryptor(key, key), CryptoStreamMode.Write);
                FileStream fsIn = new FileStream(inputFile, FileMode.Open);
                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);
                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
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

