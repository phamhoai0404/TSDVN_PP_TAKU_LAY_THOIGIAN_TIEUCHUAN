using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP1_HIEUSUAT.DTO
{
    internal class DataGetCSV
    {
        public string model { get; set; }
        public object dateUpdate { get; set; }

        public DateTime timeUpdate { get; set; }//Day la du lieu ngay thang
        public object timeObject { get; set; }//Con day la du lieu cua time object

        public int nhietdoHan { get; set; }
        public int nhietdoN2 { get; set; }
        public int nozzle { get; set; }
        public int takt { get; set; }

        public bool actionCheck { get; set; }

        /// <summary>
        /// Thuc hien lay so luong cua ngay
        /// </summary>
        public object numberSort { get;set; }

        

        public MinMaxData dataMinMax { get; set; }  

        public DataGetCSV() { }

        public DataGetCSV(string dateString,
            string modelTemp, 
            object dateUpdate, object timeGet, 
            object nhietdoHan, object nhietdoN2, object nozzle, object takt)
        {
            this.model = modelTemp;
            this.dateUpdate = dateUpdate;

            //Thuc hien lay thoi gian khong muon lay truc tiep tu dageUpdate co thi bi sai hoac khac khi thay doi dinh dang
            string dateTimeTemp = $"{dateString} {timeGet.ToString()}";
            DateTime timeUpdates = new DateTime();
            DateTime.TryParseExact(dateTimeTemp, "yyyy/MM/dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out timeUpdates);
            this.timeUpdate = timeUpdates;

            this.nhietdoHan = int.Parse(nhietdoHan.ToString());
            this.nhietdoN2 = int.Parse(nhietdoN2.ToString());
            this.nozzle = (int)double.Parse(nozzle.ToString());
            this.takt = int.Parse(takt.ToString().Replace("\r", ""));

            this.actionCheck = false;//Mac dinh la chua duoc check
        }

        
        public DataGetCSV(DataGetCSV s)
        {
            this.model = s.model;
            this.dateUpdate = s.dateUpdate;

            this.timeUpdate= s.timeUpdate;//Thoi gian lay kieu ngay thang
            this.timeObject= s.timeObject;//Thoi gian lay kieu object

            this.nhietdoHan= s.nhietdoHan;
            this.nhietdoN2= s.nhietdoN2;
            this.nozzle= s.nozzle;
            this.takt = s.takt;

            //Thuc hien lay ver cua no khong
            this.numberSort = s.numberSort;
        }


        public void SetMinMax(DataConfig config)
        {
            this.dataMinMax = new MinMaxData();
            this.dataMinMax.MaxNhietdoHan = this.nhietdoHan + config.wnNhietDoHan;
            this.dataMinMax.MinNhietdoHan = this.nhietdoHan - config.wnNhietDoHan;
            this.dataMinMax.MaxNhietdoN2 = this.nhietdoN2 + config.wnNhietDoN2;
            this.dataMinMax.MinNhietdoN2 = this.nhietdoN2 - config.wnNhietDoN2;
            this.dataMinMax.MaxNozzle = this.nozzle + config.wnNozzle;
            this.dataMinMax.MinNozzle = this.nozzle - config.wnNozzle;
            this.dataMinMax.MaxTakt = this.takt + config.wnTaskt ;
            this.dataMinMax.MinTakt = this.takt - config.wnTaskt ;
        }

    }
    public class MinMaxData
    {
        public int MinNhietdoHan { get; set; }
        public int MaxNhietdoHan { get; set; }
        public int MinNhietdoN2 { get; set; }
        public int MaxNhietdoN2 { get; set; }
        public int MinNozzle { get; set; }
        public int MaxNozzle { get; set; }
        public int MinTakt { get; set; }
        public int MaxTakt { get; set; }

        public MinMaxData() { }
    }
}
