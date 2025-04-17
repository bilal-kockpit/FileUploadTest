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
    public class fileEncryptionDecryption
    {
        public string extentions { get; set; } = "png,jpg,jped,gif,doc,docx,pdf,txt";
        public static bool IsValid( string filePath)
        {
            bool flag = false;
            var extention = Path.GetExtension(filePath);
            switch (extention.ToLower())
            {
                case ".png" :
                case ".jpg" :
                case ".jpeg":
                case ".doc" :
                case ".docx":
                case ".pdf" :
                case ".bmp" :
                case ".xls":
                case ".xlsx":
                case ".tif":
                case ".rtf":
                case ".txt":
                    flag = true;
                    break;                                      
                default:
                    flag = false;
                    break;
            }
            return flag;
        }
        public static bool validateFile(string filePath, IFormFile file)
        {
            bool flag = true;
            string msg = "";
            try {
                // Explicit check for .txt files
                if (Path.GetExtension(file.FileName).ToLower() == ".txt")
                {
                    return true;  // Allow text files without further checks
                }
                using (var fileStream = File.OpenRead(filePath))
                {
                    var isRecognizableType = FileTypeValidator.IsTypeRecognizable(fileStream);
                    
                    if (!isRecognizableType)
                    {

                        
                        flag = false;
                    
                    }
                    else
                    {
                        var IfiletypeExtention = Path.GetExtension(file.FileName);
                        var fileTypeExtention = FileTypeValidator.GetFileType(fileStream).Extension;
                        
                        if (IfiletypeExtention.ToLower().Contains(fileTypeExtention.ToLower()))
                        {
                            flag = true;
                            
                        }
                        else if(IfiletypeExtention== ".xlsx" || IfiletypeExtention==".xls")
                        {
                            flag = fileTypeExtention.Contains("docx") ? true : false;
                        }
                        else
                        {
                            flag = false;
                            
                        }
                    }

                }
               
            }
            catch(Exception ex)
            {
               
                flag = false;

            }
            return flag;
        }

        public static  Tuple<bool, string> encryptFileUpload(string fileUploadDirectoryPath, IFormFile file)
        {    
            //bool flag = IsValid(Path.Combine(fileUploadDirectoryPath,file.FileName)) ? true : false;
            string msg = "";
            try
            {
                string encryptedFile = fileUploadDirectoryPath;
                string actualFile = Path.Combine(fileUploadDirectoryPath, "Temp1");

                if (!Directory.Exists(encryptedFile))
                    {
                        Directory.CreateDirectory(encryptedFile);
                    }
                if (!Directory.Exists(actualFile))
                {
                    Directory.CreateDirectory(actualFile);
                }
                actualFile = Path.Combine(actualFile, file.FileName);
                encryptedFile = Path.Combine(encryptedFile, file.FileName);
                    if (!System.IO.File.Exists(encryptedFile))
                    {
                        using (var stream = new FileStream(actualFile, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        } 
                    }
                if (validateFile(actualFile, file))
                {
                    if(EncryptFile(actualFile, encryptedFile)== "encrypted")
                    {
                      
                        return new Tuple<bool, string>(true, "encrypted");
                      
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, EncryptFile(actualFile, encryptedFile).ToString());
                           
                    }
                }              
                else
                {
                    if (System.IO.File.Exists(actualFile)) { System.IO.File.Delete(actualFile); };
                    return new Tuple<bool, string>(false, "It is not a valid file");


                }                
            }                 
            catch(Exception ex)
            {               
                return new Tuple<bool, string>(false, ex.Message);
            }          
        }

        public static Tuple<bool, string>  decryptFileDownload(string fileUploadDirectoryPath)
        {

            bool flag = true;
            string msg = "";
            //only file path should be given 
            if (!File.Exists(fileUploadDirectoryPath))
            {
                msg = "File not exits";
            }
            else
            {
                try
                {
                    string filename = Path.GetFileName(fileUploadDirectoryPath);

                    string encryptedFile = fileUploadDirectoryPath;
                    FileInfo fi = new FileInfo(encryptedFile);            

                    string decryptedfile = Path.Combine(fi.DirectoryName, "Temp2");

                    if (!Directory.Exists(decryptedfile))
                    {
                        Directory.CreateDirectory(decryptedfile);
                    }
                    decryptedfile = Path.Combine(decryptedfile, filename);
                   
                    msg = DecryptFile(fileUploadDirectoryPath, decryptedfile);
                }


                catch (Exception ex)
                {
                    msg = ex.Message;
                    flag = false;
                }

            }
            
            return new Tuple<bool, string>(flag, msg);
        }
        //encrypt Text
        public static string encryptText(String input, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }
        //decrypt file
        public static string decryptText(String hexInput, System.Text.Encoding encoding)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return encoding.GetString(bytes);
        }
        //encrypt file
        private static string EncryptFile(string sInputFilename, string sOutputFilename)
        {
            const string exceptionMessage = "Action error. File encryption or decryption process is failing!";
            try
            {

                string EncryptionKey = "MAKV2SPBNI99212";
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (FileStream fs = new FileStream(sOutputFilename, FileMode.Create))
                    {
                        using (CryptoStream cs = new CryptoStream(fs, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            FileStream fsIn = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose);
                            int data;
                            while ((data = fsIn.ReadByte()) != -1)
                            {
                                cs.WriteByte((byte)data);
                            }
                        }

                    }
                }
                return "encrypted";
            }
            catch (Exception ex)
            {
                return ex.Message + "/" + exceptionMessage;

            }
        }
        //decrypt file
        private static string DecryptFile(string sInputFilename, string sOutputFilename)
        {
            const string exceptionMessage = "Action error. File encryption or decryption process is failing!";
            try
            {
                string EncryptionKey = "MAKV2SPBNI99212";
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (FileStream fs = new FileStream(sInputFilename, FileMode.Open))
                    {
                        using (CryptoStream cs = new CryptoStream(fs, encryptor.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (FileStream fsOut = new FileStream(sOutputFilename, FileMode.Create))
                            {
                                int data;
                                while ((data = cs.ReadByte()) != -1)
                                {
                                    fsOut.WriteByte((byte)data);
                                }
                            }
                        }
                    }
                }

                return sOutputFilename;
            }
            catch (Exception ex)
            {
                return ex.Message + "/" + exceptionMessage;
            }



        }

    }
}
    


