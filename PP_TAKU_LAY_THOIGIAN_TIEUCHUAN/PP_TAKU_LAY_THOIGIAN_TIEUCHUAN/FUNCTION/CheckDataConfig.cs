using PP1_HIEUSUAT.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP1_HIEUSUAT.FUNCTION
{
    public class CheckDataConfig
    {
        public static void CheckData(DataConfig config)
        {
            //Check su ton tai cua cac duong dan
            if (!File.Exists(config.resultPathFile))
            {
                throw new Exception($"File: {config.resultPathFile} => Không tồn tại!");
            }
        }
    }
}
