using OurTube.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.Services
{
    public class ViewService
    {
        private ApplicationDbContext _dbContext;

        public ViewService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
