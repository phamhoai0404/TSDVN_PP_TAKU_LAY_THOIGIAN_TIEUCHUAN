using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using PP1_HIEUSUAT.DTO;
using PP1_HIEUSUAT.FUNCTION;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace PP1_HIEUSUAT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public DataConfig configGet = new DataConfig();
        private void Form1_Load(object sender, EventArgs e)
        {
            //Thuc hien doc file config
            try
            {
                ReadConfig.GetConfig(this.configGet);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                //Thuc hien check config
                CheckDataConfig.CheckData(this.configGet);

                //Thuc hien doc du lieu file
                List<DataGetCSV> listDataGet = new List<DataGetCSV>();
                ActionMain.ActionGetValue(this.configGet, listDataGet);

                //Thuc hien xu ly file excel
                string textAdd = "";
                if(listDataGet.Count > 0)//Neu co du lieu thi thuc hien add
                {
                    ActionWriteExcel.WriteFileExcel(this.configGet, listDataGet, ref textAdd);
                }
                

                //Thuc hien tinh toan xem co bao nhieu file khong ket noi duoc
                var listErr= this.configGet.listMachine.Where(x => x.actionConnect == false).ToList() ;
                if(listErr.Count() == this.configGet.listMachine.Count())
                {
                    throw new Exception("Tất cả các folder Machine đều không kết nối!");
                }
                string machineError = listErr.Any() ? " - " + string.Join("@", listErr.Select(p => p.pathFolder)) : "";

                //Thuc hien lay du lieu cua cac file
                ActionFileLog.WriteResultOKNG(configGet.pathFileLog, $"OK{textAdd}{machineError}");
            }
            catch (Exception ex)
            {
                ActionFileLog.WriteResultOKNG(configGet.pathFileLog, ex.Message);
            }

            //Thuc hien dong
            Application.Exit();
        }
    }
    
}
