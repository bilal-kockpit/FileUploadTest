using FileUpload.Models;
using FileUpload.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PTCL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FileUpload.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IHostingEnvironment _env;
        private readonly ApplicationContext _context;
        public HomeController(ILogger<HomeController> logger, IHostingEnvironment env, ApplicationContext context)
        {
            _logger = logger;
            _env = env;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var fileuploadViewModel = await LoadAllFiles();
            ViewBag.Message = TempData["Message"];
            return View(fileuploadViewModel);
        }


        private async Task<FileUploadViewModel> LoadAllFiles()
        {
            var viewModel = new FileUploadViewModel();
            //viewModel.FilesOnDatabase = await _context.FilesOnDatabase.ToListAsync();
            viewModel.FilesOnFileSystem = await _context.FileOnFileSystemModel.ToListAsync();
            return viewModel;
        }
        [HttpPost]
        public IActionResult UploadToFileSystem(List<IFormFile> files, string description)
        {
            List<string> errlist = new List<string>();
                string wwwPath = this._env.WebRootPath;
                string contentPath = this._env.ContentRootPath;
                string basePath = Path.Combine(this._env.WebRootPath, "Files");      

            foreach (var file in files)
            {               
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var filePath = Path.Combine(basePath, file.FileName);
                var extension = Path.GetExtension(file.FileName);
                if (!System.IO.File.Exists(filePath))
                {

                    if (fileEncryptionDecryption.encryptFileUpload(basePath, file).Item1)
                    {
                        var fileModel = new FileOnFileSystemModel
                        {
                            CreatedOn = DateTime.Now,
                            FileType = file.ContentType,
                            Extension = extension,
                            Name = fileName,
                            Description = description,
                            FilePath = filePath
                        };
                        _context.FileOnFileSystemModel.Add(fileModel);
                        _context.SaveChanges();
                    }
                    else
                    {
                        TempData["Message"] = fileEncryptionDecryption.encryptFileUpload(basePath, file).Item2;
                    }

                }
            }
            if (errlist.Count > 0)
            {
                foreach (var item in errlist)
                {
                    TempData["Message"] = item + " ";

                }
            }
            else
            {
                TempData["Message"] = "File successfully uploaded to File System.";

            }      

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DownloadFileFromFileSystem(int id)
        {

            var file = await _context.FileOnFileSystemModel.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null) return null;
           string downloadFilePath = fileEncryptionDecryption.decryptFileDownload(file.FilePath).Item2;
            var memory = new MemoryStream();
            using (var stream = new FileStream(downloadFilePath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, file.FileType, file.Name + file.Extension);
        }
        public async Task<IActionResult> DeleteFileFromFileSystem(int id)
        {

            var file = await _context.FileOnFileSystemModel.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null) return null;
            if (System.IO.File.Exists(file.FilePath))
            {
                System.IO.File.Delete(file.FilePath);
            }
            _context.FileOnFileSystemModel.Remove(file);
            _context.SaveChanges();
            TempData["Message"] = $"Removed {file.Name + file.Extension} successfully from File System.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> showFileDetails(string id)
        {
            int newId = int.Parse(fileEncryptionDecryption.decryptText(id, System.Text.Encoding.Unicode));
            
            var file = await _context.FileOnFileSystemModel.Where(x => x.Id == newId).FirstOrDefaultAsync();
            if (file == null) return null;
            return View(file);
        }

      



        public IActionResult Privacy()
        {
            return View();
        }    

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
