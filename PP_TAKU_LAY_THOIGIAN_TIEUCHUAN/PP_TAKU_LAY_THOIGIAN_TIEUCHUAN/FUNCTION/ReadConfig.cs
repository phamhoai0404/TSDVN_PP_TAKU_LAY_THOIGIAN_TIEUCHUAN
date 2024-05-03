using PP1_HIEUSUAT.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PP1_HIEUSUAT.FUNCTION
{
    internal class ReadConfig
    {
        public static void GetConfig(DataConfig config)
        {


            //Thuc hien doc file
            XmlDocument doc = new XmlDocument();
            string pathFileConfig = MdlCommon.PATH_FILE_XML;
            doc.Load(pathFileConfig);

            //Thong tin ghi ket qua, log
            config.pathFileLog = doc.SelectSingleNode("/Database/Infor/PathFileLog").InnerText?.Trim();
            config.resultPathFile = doc.SelectSingleNode("/Database/Infor/PathFileResult/Path").InnerText?.Trim();
            config.resultSheetNameAll = doc.SelectSingleNode("/Database/Infor/PathFileResult/SheetNameAll").InnerText?.Trim();
            config.resultSheetNameNew = doc.SelectSingleNode("/Database/Infor/PathFileResult/SheetNameNew").InnerText?.Trim();
            
            config.dateStart = int.Parse(doc.SelectSingleNode("/Database/Infor/DateGet/DateStart").InnerText?.Trim());
            config.dateEnd = int.Parse(doc.SelectSingleNode("/Database/Infor/DateGet/DateEnd").InnerText?.Trim());

            //Lay du lieu sai so
            config.wnNhietDoHan = int.Parse(doc.SelectSingleNode("/Database/Infor/WrongNumber/NhietDoHan").InnerText?.Trim());
            config.wnNhietDoN2 = int.Parse(doc.SelectSingleNode("/Database/Infor/WrongNumber/NhietDoN2").InnerText?.Trim());
            config.wnNozzle = int.Parse(doc.SelectSingleNode("/Database/Infor/WrongNumber/Nozzle").InnerText?.Trim());
            config.wnTaskt = int.Parse(doc.SelectSingleNode("/Database/Infor/WrongNumber/Takt").InnerText?.Trim());

            XmlNodeList machineNodes = doc.SelectNodes("/Database/ListMachine/Machine");

            config.listMachine = new List<Machine>();//Khoi tao moi
            foreach (XmlNode machineNode in machineNodes)
            {
                config.listMachine.Add(new Machine(machineNode.SelectSingleNode("FolderGet").InnerText?.Trim()));
            }
        }
    }
}
