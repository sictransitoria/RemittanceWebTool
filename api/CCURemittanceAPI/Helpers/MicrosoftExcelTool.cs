using System;
using System.Data;
using System.Runtime.InteropServices;

namespace CCURemittanceAPI.Helpers
{
    public class MicrosoftExcelTool
    {
        public static DataSet ReadExcelFileMemberInfo(string filepath)
        {
            DataSet ds = new DataSet();

            //Instantiate variables here to enable cleanup if an error occurs
            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Microsoft.Office.Interop.Excel.Workbook excelWorkBook = null;
            Microsoft.Office.Interop.Excel.Worksheet excelSheet = null;
            Microsoft.Office.Interop.Excel.Range excelRange = null;

            try
            {
                excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.DisplayAlerts = false;
                excelApp.Visible = false;
                excelWorkBook = excelApp.Workbooks.Open(@"" + filepath, Type.Missing, false);

                #region Copy Data from Worksheet
                excelSheet = excelWorkBook.Worksheets[1];
                excelRange = excelSheet.UsedRange;
                int totalRows = excelRange.Rows.Count;
                int totalCols = excelRange.Columns.Count;

                object[,] data = excelRange.Value2;

                System.Data.DataTable dt = new System.Data.DataTable();


                // Add columns to data table first
                for (int col = 1; col <= totalCols; col++)
                {
                    dt.Columns.Add("F" + col.ToString());
                }

                // looking for every row from 17 and on
                for (int row = 16; row <= totalRows; row++)
                {
                    DataRow newRow = dt.NewRow();
                    for (int col = 1; col <= totalCols; col++)
                    {
                        newRow[col - 1] = data[row, col] == null ? "" : data[row, col].ToString();
                    }
                    dt.Rows.Add(newRow);
                }

                ds.Tables.Add(dt);
                #endregion

                // Release interop objects
                Marshal.FinalReleaseComObject(excelRange);
                Marshal.FinalReleaseComObject(excelSheet);

                excelWorkBook.Close();
                Marshal.FinalReleaseComObject(excelWorkBook);

                excelApp.Quit();
                Marshal.FinalReleaseComObject(excelApp);
            }
            catch (Exception ex)
            {
                Marshal.FinalReleaseComObject(excelRange);
                Marshal.FinalReleaseComObject(excelSheet);

                excelWorkBook.Close();
                Marshal.FinalReleaseComObject(excelWorkBook);

                excelApp.Quit();
                Marshal.FinalReleaseComObject(excelApp);
            }

            return ds;
        }
        public static DataSet ReadExcelFileInvoiceNum(string filepath)
        {
            DataSet dsDeux = new DataSet();

            //Instantiate variables here to enable cleanup if an error occurs
            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Microsoft.Office.Interop.Excel.Workbook excelWorkBook = null;
            Microsoft.Office.Interop.Excel.Worksheet excelSheet = null;
            Microsoft.Office.Interop.Excel.Range excelRange = null;

            try
            {
                excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.DisplayAlerts = false;
                excelApp.Visible = false;
                excelWorkBook = excelApp.Workbooks.Open(@"" + filepath, Type.Missing, false);

                #region Copy Data from Worksheet
                excelSheet = excelWorkBook.Worksheets[1];
                excelRange = excelSheet.UsedRange;
                int totalRows = excelRange.Rows.Count;
                int totalCols = excelRange.Columns.Count;

                object[,] data = excelRange.Value2;

                System.Data.DataTable dtDeux = new System.Data.DataTable();


                // Add columns to data table first
                for (int col = 1; col <= totalCols; col++)
                {
                    dtDeux.Columns.Add("F" + col.ToString());
                }

                // we will only need to extract the data from the 9th row
                for (int row = 9; row <= 9; row++)
                {
                    DataRow newRow = dtDeux.NewRow();
                    for (int col = 1; col <= totalCols; col++)
                    {
                        newRow[col - 1] = data[row, col] == null ? "" : data[row, col].ToString();
                    }
                    dtDeux.Rows.Add(newRow);
                }

                dsDeux.Tables.Add(dtDeux);
                #endregion

                // Release interop objects
                Marshal.FinalReleaseComObject(excelRange);
                Marshal.FinalReleaseComObject(excelSheet);

                excelWorkBook.Close();
                Marshal.FinalReleaseComObject(excelWorkBook);

                excelApp.Quit();
                Marshal.FinalReleaseComObject(excelApp);
            }
            catch (Exception ex)
            {
                Marshal.FinalReleaseComObject(excelRange);
                Marshal.FinalReleaseComObject(excelSheet);

                excelWorkBook.Close();
                Marshal.FinalReleaseComObject(excelWorkBook);

                excelApp.Quit();
                Marshal.FinalReleaseComObject(excelApp);
            }

            return dsDeux;
        }
    }
}