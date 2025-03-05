using Administration.Data;
using Administration.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Administration.ViewModel
{
    public partial class RapportsVM
    {
        private readonly AdministrationContext _dbContext;


        public RapportsVM()
        {
            AdministrationContextFactory factory = new AdministrationContextFactory();
            _dbContext = factory.CreateDbContext(new string[0]);
        }
    }
}
