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
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }              
               

            foreach (var file in files)
            {
               
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var filePath = Path.Combine(basePath, file.FileName);
                var extension = Path.GetExtension(file.FileName);
                if (!System.IO.File.Exists(filePath))
                {
                    //using (var stream = new FileStream(filePath, FileMode.Create))
                    //{
                    //    file.CopyToAsync(stream);
                    //}
                    fileEncryption.encryptFileUpload(basePath, file, "");
                  

                    var fileModel = new FileOnFileSystemModel
                    {
                        CreatedOn = DateTime.Now,
                        FileType = file.ContentType,
                        Extension = extension,
                        Name = fileName,
                        Description = description,
                        FilePath = filePath
                    };
                    //string abc = MimeTypes.GetContentType(file.FileName);
                    //var filecheck = chkContentFile.Validate(filePath, extension);
                    //if (filecheck.Item1==true)
                    //{
                    //    _context.FileOnFileSystemModel.Add(fileModel);
                    //    _context.SaveChanges();
                    //}
                    //else
                    //{
                    //    errlist.Add(filecheck.Item2);
                    //    if (System.IO.File.Exists(filePath))
                    //    {
                    //        System.IO.File.Delete(filePath);
                    //    }
                    //}
                    _context.FileOnFileSystemModel.Add(fileModel);
                    _context.SaveChanges();

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
        //http://www.dotnetawesome.com/2013/11/how-to-upload-file-with-encryption-and.html

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DownloadFileFromFileSystem(int id)
        {

            var file = await _context.FileOnFileSystemModel.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null) return null;
            var memory = new MemoryStream();
            using (var stream = new FileStream(file.FilePath, FileMode.Open))
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
        public IActionResult Privacy()
        {
            return View();
        }














        [HttpPost]
        public IActionResult Index(List<IFormFile> postedFiles)
        {
            //string wwwPath = this._env.WebRootPath;
            //string contentPath = this._env.ContentRootPath;

            //string path = Path.Combine(this._env.WebRootPath, "Uploads");
            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}

            //List<string> uploadedFiles = new List<string>();
            //foreach (IFormFile postedFile in postedFiles)
            //{
            //    string fileName = Path.GetFileName(postedFile.FileName);
            //    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            //    {
            //        postedFile.CopyTo(stream);
            //        uploadedFiles.Add(fileName);
            //        ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", fileName);
            //    }
            //}

            return View();

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
