using OfficeOpenXml;
using PP1_HIEUSUAT.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PP1_HIEUSUAT.FUNCTION
{
    public class ActionWriteExcel
    {
        internal static void WriteFileExcel(DataConfig configGet, List<DataGetCSV> listDataGet_First, ref string textAdd)
        {
            //Thuc hien sap xep tang dan cua thoi gian
            var listDataGet = listDataGet_First.OrderBy(p => p.timeUpdate).ToList();

            //Thuc hien mo file excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(configGet.resultPathFile)))
            {
                //Thuc hien kiem tra su ton tai cua sheet 2 sheet name All va sheet name Ver New
                ExcelWorksheet worksheetAll = package.Workbook.Worksheets[configGet.resultSheetNameAll];
                if (worksheetAll == null)
                {
                    throw new Exception($"File: {configGet.resultPathFile} - SheetName: {configGet.resultSheetNameAll} => Không tồn tại!");
                }
                ExcelWorksheet worksheetVerNew = package.Workbook.Worksheets[configGet.resultSheetNameNew];
                if (worksheetVerNew == null)
                {
                    throw new Exception($"File: {configGet.resultPathFile} - SheetName: {configGet.resultSheetNameNew} => Không tồn tại!");
                }

                //Lay du lieu vung dang su dung
                int lastRow = worksheetAll.Dimension.End.Row;//Laydong cuoi cung
                object[,] listAll = null;
                listAll = worksheetAll.Cells[$"A1:I{lastRow}"].Value as object[,];

                //Thuc hien loc du lieu da co
                List<DataGetCSV> listDataOld = new List<DataGetCSV>();
                ReadDataOld(listAll, listDataOld, ref lastRow);

                List<DataGetCSV> listVer = new List<DataGetCSV>();
                //Thuc hien check va chuan bi add du lieu
                CheckDataAddNew(listDataOld, listDataGet, configGet, listVer);

                //Thuc hien ghi du lieu sang mang 
                var listDataWrite = listDataGet.Where(p => p.actionCheck == false).ToList();//Vi du lieu true da nam trong khoang
                if (listDataWrite.Count > 0)
                {
                    //Lam viec voi sheet du lieu All
                    lastRow = lastRow + 1;//Tinh duoc dong bat dau can ghi
                    textAdd = $"- StartRow: {lastRow} Write:{listDataWrite.Count} Record";
                    object[,] multiArray = GetValueWrite(listDataWrite);
                    worksheetAll.Cells[$"B{lastRow}:I{lastRow}"].Value = multiArray;


                    //Lam viec voi sheet du lieu Ver New
                    //Thuc hien xoa du lieu tua A I => den het
                    worksheetVerNew.Cells[$"B3:I{worksheetVerNew.Dimension.End.Row}"].ClearFormulaValues();
                    object[,] multiArrayVerNew = GetValueWriteVer(listVer);
                    worksheetVerNew.Cells[$"B3:I3"].Value = multiArrayVerNew;

                    //Thuc hien luu lai
                    package.Save();
                }
                else
                {
                    textAdd = $"- 0 Record";
                }
            }
        }

        private static void CheckDataAddNew(List<DataGetCSV> listDataOld, List<DataGetCSV> listDataGet, DataConfig configGet, List<DataGetCSV> listVer)
        {
            //Thuc hien tinh toan MinMax cua du lieu cu
            listDataOld.ForEach(p => p.SetMinMax(configGet));

            //Thuc hien lay du lieu cua cua cac model
            HashSet<string> listItemAll = new HashSet<string>(listDataOld.Select(p => p.model)
                                                                         .Concat(listDataGet.Select(p => p.model))
                                                                         .Distinct());

            int numberCurrent = 0;
            foreach (var itemCurrent in listItemAll)//Thuc hien duyet tung Item 1
            {
                //Thuc hien lay Item moi
                var listItemInNew = listDataGet.Where(p => p.model == itemCurrent).ToList();

                //Thuc hien lay Item cu
                var listItemInDataOld = listDataOld.Where(p => p.model == itemCurrent).ToList();

                //Neu ma khong co du lieu trong cai moi => thi thuc hien lay cai cu
                if (listItemInNew.Count == 0)
                {
                    listVer.Add(new DataGetCSV(listItemInDataOld[listItemInDataOld.Count - 1]));//Neu khong co du lieu thi mac dinh lay cai cuoi cung
                    continue;//Thuc hien add xong thi chuyen sang Item khac
                }

                //Chua tung ton tai truoc do thi add toan bo
                if (listItemInDataOld.Count == 0)
                {
                    //Ghi lai so cua Item vua moi tao
                    numberCurrent = 0;
                    listItemInNew.ForEach(p => p.numberSort = ++numberCurrent);
                    listVer.Add(new DataGetCSV(listItemInNew[listItemInNew.Count - 1]));
                    continue;//Neu khong data Old ma khong co thi thuc hien duyet => dung thoi
                }

                //Lay so luong hien co
                numberCurrent = listItemInDataOld.Count;

                //Kiem tra xem Model moi co giong cai cuoi cung khong
                var itemOld = listItemInDataOld[listItemInDataOld.Count - 1];//Thuc hien lay con cuoi cung
                itemOld.SetMinMax(configGet);//Thuc hien tinh toan minmax

                var itemCheck = listItemInNew[0];//Thuc hien lay con dau tien
                if (itemOld.dataMinMax.MaxNhietdoHan >= itemCheck.nhietdoHan && itemOld.dataMinMax.MinNhietdoHan <= itemCheck.nhietdoHan &&
                itemOld.dataMinMax.MaxNhietdoN2 >= itemCheck.nhietdoN2 && itemOld.dataMinMax.MinNhietdoN2 <= itemCheck.nhietdoN2 &&
                itemOld.dataMinMax.MaxNozzle >= itemCheck.nozzle && itemOld.dataMinMax.MinNozzle <= itemCheck.nozzle &&
                itemOld.dataMinMax.MaxTakt >= itemCheck.takt && itemOld.dataMinMax.MinTakt <= itemCheck.takt)
                {
                    //Thuc hien khong add nua vi no giong voi con vua them roi
                    itemCheck.actionCheck = true;

                    if (listItemInNew.Count == 1)//Neu chi co 1  thang thi thuc hien lay thang cu
                    {
                        listVer.Add(new DataGetCSV(itemOld));
                    }
                    else//Neu khong phai <>1 thi no se lay thang cuoi cung
                    {
                        //Thuc hien gan so luong tru con dau tien
                        for (int i = 1; i < listItemInNew.Count; i++)
                        {
                            listItemInNew[i].numberSort = ++numberCurrent;
                        }
                        //Neu co du lieu => thi lay du lieu cuoi cung cua file
                        listVer.Add(new DataGetCSV(listItemInNew[listItemInNew.Count - 1]));
                    }
                }
                else
                {
                    //Thuc hien gan so luong ver
                    listItemInNew.ForEach(p => p.numberSort = ++numberCurrent);
                    //Neu co du lieu => thi lay du lieu cuoi cung cua file
                    listVer.Add(new DataGetCSV(listItemInNew[listItemInNew.Count - 1]));
                }


            }

        }

        private static void ReadDataOld(object[,] listAll, List<DataGetCSV> listDataOld, ref int rowLastData)
        {
            //Thuc hien lay du lieu cu
            int indexModel = 1;
            int indexNumberSort = 2;

            int indexDate = 3;
            int indexTime = 4;

            int indexNhietDoHan = 5;
            int indexNhietDoN2 = 6;
            int indexNozzle = 7;
            int indexTakt = 8;

            int rowAll = listAll.GetLength(0);

            int tempQty = 0;

            rowLastData = 2;//Thuc hien dong dau tien co du lieu

            DataGetCSV tempValue = new DataGetCSV();//Thuc hien luu tru du lieu tam
            //Bat dau  tu 2 => vi dong 3 moi bat dau co du lieu
            for (int i = 2; i < rowAll; i++)
            {
                if (string.IsNullOrWhiteSpace(listAll[i, indexModel]?.ToString()) == true)
                {
                    continue;
                }
                rowLastData = i + 1;//Mac dinh dong cuoi la dong do

                tempValue.model = listAll[i, indexModel].ToString().Trim().ToUpper();//Thuc hien gia tri cua Model

                //Thuc hien lay gia tri cua so
                tempValue.numberSort = listAll[i, indexNumberSort];

                //Thuc hien lay gia tri cua thoi gian
                tempValue.dateUpdate = listAll[i, indexDate];
                tempValue.timeObject = listAll[i, indexTime];

                //Lay du lieu cua NhietDo
                if (!int.TryParse(listAll[i, indexNhietDoHan]?.ToString(), out tempQty))
                {
                    continue;//Neu ma loi thi khong lay dong du lieu cu nay luon
                }
                tempValue.nhietdoHan = tempQty;

                if (!int.TryParse(listAll[i, indexNhietDoN2]?.ToString(), out tempQty))
                {
                    continue;//Neu ma loi thi khong lay dong du lieu cu nay luon
                }
                tempValue.nhietdoN2 = tempQty;

                if (!int.TryParse(listAll[i, indexNozzle]?.ToString(), out tempQty))
                {
                    continue;//Neu ma loi thi khong lay dong du lieu cu nay luon
                }
                tempValue.nozzle = tempQty;

                if (!int.TryParse(listAll[i, indexTakt]?.ToString(), out tempQty))
                {
                    continue;//Neu ma loi thi khong lay dong du lieu cu nay luon
                }
                tempValue.takt = tempQty;

                listDataOld.Add(new DataGetCSV(tempValue));

            }
        }


        private static object[,] GetValueWrite(List<DataGetCSV> listDataWrite)
        {
            object[,] multiArray = new object[listDataWrite.Count, 9];
            for (int i = 0; i < listDataWrite.Count; i++)
            {
                multiArray[i, 0] = listDataWrite[i].model;
                multiArray[i, 1] = listDataWrite[i].numberSort;
                multiArray[i, 2] = listDataWrite[i].dateUpdate;
                multiArray[i, 3] = listDataWrite[i].timeUpdate.ToString("HH:mm:ss");
                multiArray[i, 4] = listDataWrite[i].nhietdoHan;
                multiArray[i, 5] = listDataWrite[i].nhietdoN2;
                multiArray[i, 6] = listDataWrite[i].nozzle;
                multiArray[i, 7] = listDataWrite[i].takt;
            }
            return multiArray;
        }
        private static object[,] GetValueWriteVer(List<DataGetCSV> listDataWrite)
        {
            object[,] multiArray = new object[listDataWrite.Count, 9];
            for (int i = 0; i < listDataWrite.Count; i++)
            {
                multiArray[i, 0] = listDataWrite[i].model;
                multiArray[i, 1] = listDataWrite[i].numberSort;
                multiArray[i, 2] = listDataWrite[i].dateUpdate;
                multiArray[i, 3] = listDataWrite[i].timeObject == null? listDataWrite[i].timeUpdate.ToString("HH:mm:ss"): listDataWrite[i].timeObject;
                multiArray[i, 4] = listDataWrite[i].nhietdoHan;
                multiArray[i, 5] = listDataWrite[i].nhietdoN2;
                multiArray[i, 6] = listDataWrite[i].nozzle;
                multiArray[i, 7] = listDataWrite[i].takt;
            }
            return multiArray;
        }
    }
}
