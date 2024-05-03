using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP1_HIEUSUAT.DTO
{
    internal class MdlCommon
    {
        public static string PATH_FILE_XML = ConfigurationManager.AppSettings["PathFileConfig"];
    }
}
