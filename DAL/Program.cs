using System;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Fotron.DAL
{

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private readonly string CONNECTION_STRING = "server=localhost;user id=fotron.io;password=fotron.io;persistsecurityinfo=True;database=fotron.io;";

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>();

            opts.UseMySql(CONNECTION_STRING, myopts => {
                myopts.UseRelationalNulls(true);
            });

            var context = new ApplicationDbContext(opts.Options);

            context.Database.Migrate();

            return context;
        }

    }
}
