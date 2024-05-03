using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP1_HIEUSUAT.DTO
{
    public class DataConfig
    {
        /// <summary>
        /// Duong dan cua file ghi ket qua
        /// </summary>
        public string resultPathFile { get; set; }
        /// <summary>
        /// Sheet name cua file ghi ket qua All
        /// </summary>
        public string resultSheetNameAll { get; set; }

        /// <summary>
        /// SheetName cua file ghi ket qua VerNew
        /// </summary>
        public string resultSheetNameNew { get; set; }



        /// <summary>
        /// Duong dan ghi log
        /// </summary>
        public string pathFileLog { get; set; }
        
        
        /// <summary>
        /// Thoi gian lay ket qua so voi thoi diem chay
        /// </summary>
        public int dateStart { get;set; }
        public int dateEnd { get;set; }



        /// <summary>
        /// Danh sach cac machine lay du lieu
        /// </summary>
        public List<Machine> listMachine { get; set; }


        /// <summary>
        /// Sai so cua Nhiet do han
        /// </summary>
        public int wnNhietDoHan { get; set; }

        /// <summary>
        /// Sai so cua nhiet do N2
        /// </summary>
        public int wnNhietDoN2 { get; set; }

        /// <summary>
        /// Sai so cua Nozzle
        /// </summary>
        public int wnNozzle { get; set; }

        /// <summary>
        /// Sai so cua Taskt
        /// </summary>
        public int wnTaskt { get; set; }


        public DataConfig() { }
    }
    public class Machine
    {

        /// <summary>
        /// Duong dan folder cua Machine
        /// </summary>
        public string pathFolder { get; set; }

        /// <summary>
        /// Trang thai ket noi voi duong dan
        /// </summary>
        public bool actionConnect { get; set; }
        public Machine()
        {
            this.actionConnect = true;
        }
        public Machine(string pathFolder)
        {
            this.pathFolder = pathFolder;
            this.actionConnect = true;
        }

    }
}
