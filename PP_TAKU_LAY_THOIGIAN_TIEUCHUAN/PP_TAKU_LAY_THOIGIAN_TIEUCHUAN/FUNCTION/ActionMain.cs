using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using PP1_HIEUSUAT.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace PP1_HIEUSUAT.FUNCTION
{
    internal class ActionMain
    {

        public static void ActionGetValue(DataConfig configGet, List<DataGetCSV> listDataOut)
        {
            //Thuc hien lay thoi gian
            DateTime dateStart = DateTime.Now.AddDays(configGet.dateStart);
            DateTime dateEnd = DateTime.Now.AddDays(configGet.dateEnd);

            List<DataGetCSV> listDataGet = new List<DataGetCSV>();
            foreach (var machineCurrent in configGet.listMachine)
            {
                //Thuc hien kiem tra xem file co xem khong
                if (!Directory.Exists(machineCurrent.pathFolder))
                {
                    machineCurrent.actionConnect = false;
                    continue;//Thuc hien chuyen check folder khac
                }

                //Thuc hien lay du lieu cua folderDate
                var listFolderGet = Directory.GetDirectories(machineCurrent.pathFolder)
                                           .Where(p => Directory.GetLastWriteTime(p).Date >= dateStart.Date &&
                                                     Directory.GetLastWriteTime(p).Date <= dateEnd.Date)
                                           .ToList();

                //Neu ma khong co du lieu ngay thi duyet sang folder khac
                if (listFolderGet.Count == 0)
                    continue;
                foreach (var folderDateChild in listFolderGet)
                {
                    //Thuc hien lay string date 
                    string dateGet = Directory.GetLastWriteTime(folderDateChild).ToString("yyyy/MM/dd");

                    //Thuc hien check xem folder DIP co ton tai hay khong
                    string pathFolderDip = System.IO.Path.Combine(folderDateChild, "DIP_1");
                    if (Directory.Exists(pathFolderDip) == false)
                    {
                        continue; //Neu ma khong ton tai thi thuc hien lay cai moi
                    }

                    //Thuc hien check du lieu trong
                    var allFile = new DirectoryInfo(pathFolderDip).GetFiles().ToArray();
                    foreach (var itemFile in allFile)
                    {
                        GetFileCSV2(itemFile.FullName, listDataGet, dateGet);
                    }
                }

            }

            //Thuc hien check du lieu de loai bo nhung con tuong doi giong nhau

            var sortedDateTimeList = listDataGet.OrderBy(p=> p.timeUpdate);
            FilterData(configGet, ref listDataOut, listDataGet);
        }

        private static void GetFileCSV2(string pathFile, List<DataGetCSV> listDataGet, string dateString)
        {
            try
            {
                using (FileStream fs = new FileStream(pathFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string all = sr.ReadToEnd();
                        var lines = all.Split('\n');

                        int countAll = lines.Count();//Danh sach tat ca


                        for (int i = countAll - 1; i >= 0; i--)
                        {
                            var delimitedLine = lines[i].Split(',');
                            if (delimitedLine.Length >= 18)
                            {
                                if (delimitedLine[16].ToString() == "OK")
                                {
                                    listDataGet.Add(new DataGetCSV(
                                        dateString,
                                        delimitedLine[0],
                                        delimitedLine[1], delimitedLine[2],
                                        delimitedLine[3], delimitedLine[4], delimitedLine[10], delimitedLine[17]
                                    ));
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                //Neu ma chuyen co loi thi thoi khong sao ca
            }
        }

        private static void FilterData(DataConfig configGet, ref List<DataGetCSV> listDataOut, List<DataGetCSV> listDataFirst)
        {
            int countListAll = listDataFirst.Count;
            for (int i = 0; i < countListAll; i++)
            {
                //Neu giong voi 1 cai nao do thi thuc hien bo qua
                if (listDataFirst[i].actionCheck == true)
                {
                    continue;
                }



                //Thuc hien them du lieu add vao
                listDataOut.Add(new DataGetCSV(listDataFirst[i]));

                int maxHan = listDataFirst[i].nhietdoHan + configGet.wnNhietDoHan;
                int minHan = listDataFirst[i].nhietdoHan - configGet.wnNhietDoHan;
                int maxN2 = listDataFirst[i].nhietdoN2 + configGet.wnNhietDoN2;
                int minN2 = listDataFirst[i].nhietdoN2 - configGet.wnNhietDoN2;
                int maxNozzle = listDataFirst[i].nozzle + configGet.wnNozzle;
                int minNozzle = listDataFirst[i].nozzle - configGet.wnNozzle;
                int maxTakt = listDataFirst[i].takt + configGet.wnTaskt;
                int minTakt = listDataFirst[i].takt - configGet.wnTaskt;

                for (int j = 0; j < countListAll; j++)
                {
                    //Neu da duyet roi hoac o nhung cai da co thi thuc hien khong check nua
                    if (j == i || listDataFirst[j].actionCheck == true ||
                       listDataFirst[i].model != listDataFirst[j].model)
                    {
                        continue;
                    }

                    //Kiem tra xem nhung du lieu khac co giong nhau khong
                    if (maxHan >= listDataFirst[j].nhietdoHan && minHan <= listDataFirst[j].nhietdoHan &&
                     maxN2 >= listDataFirst[j].nhietdoN2 && minN2 <= listDataFirst[j].nhietdoN2 &&
                     maxNozzle >= listDataFirst[j].nozzle && minNozzle <= listDataFirst[j].nozzle &&
                     maxTakt >= listDataFirst[j].takt && minTakt <= listDataFirst[j].takt)
                    {
                        listDataFirst[j].actionCheck = true;//Thuc hien danh dau la da duyet
                    }

                }
            }





        }
    }
}
