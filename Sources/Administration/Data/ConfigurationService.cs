using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Administration.Data
{
    internal class ConfigurationService
    {
        public static IConfigurationRoot GetConfiguration()
        {
            //Look if the build is debug or release to load the right appsettings.json
            IConfigurationBuilder builder;
#if DEBUG
            builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.dev.json", optional: false, reloadOnChange: true);
            return builder.Build();
#elif RELEASE
            builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            return builder.Build();
#endif
            throw new Exception("Configuration file not found");

        }
    }
}
