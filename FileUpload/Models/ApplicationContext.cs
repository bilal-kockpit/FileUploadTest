using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload.Models
{
    public class ApplicationContext : DbContext
    {

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
       
        public DbSet<FileOnDatabaseModel> FileOnDatabaseModels { get; set; }
        public DbSet<FileOnFileSystemModel> FileOnFileSystemModel { get; set; }
    }
}
