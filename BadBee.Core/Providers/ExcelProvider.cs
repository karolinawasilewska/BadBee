﻿using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;
using log4net;

namespace BadBee.Core.Providers
{
    public class ExcelProvider
    {
        //private static log4net.ILog Log { get; set; }
        //ILog log = log4net.LogManager.GetLogger(typeof(ExcelProvider));
        public static byte[] ReadFully_ItemsToCSV(string type)
        {
            //try
            //{

                var input = ItemsToCSV(type);
                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }

            }
            //catch (Exception e)
            //{
            //    ExcelProvider.Log.Error(e);
            //    throw;
            //}
        public static MemoryStream ItemsToCSV(string type)
        {

            string connectionstring;
            string query = "";
            connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            if (type == "items")
            {
                query = "SELECT [Brand],[Type],[Serie],[Model],[Kw],[Km],[DateFrom],[DateTo],[Fr],[WvaDesc],[Wva],[WvaDetailsQty],[WvaDetails],[BadBeeNumber],"
            + "[Height],[Width],[Thickness],[Wedge],[DrumDiameter],[BrakeSystem],[RivetsQuantity],[RivetsType],[ProductType] FROM ItemsDb";
            }
            else if (type == "cross")
            {
                query = "SELECT [BadBeeNumber],[CrossBrandName],[CrossBrandNumber] FROM Crosses";
            }
            else if (type == "itemsWithIds")
            {
                query = "SELECT * FROM ItemsDb";
            }
            else if (type == "crossWithIds")
            {
                query = "SELECT * From Crosses";
            }

            SqlConnection connection = new SqlConnection(connectionstring);
            connection.Open();

            SqlCommand command = new SqlCommand(query, connection);

            DataTable data = new DataTable();

            using (SqlDataAdapter a = new SqlDataAdapter(command))
            {
                a.Fill(data);
                using (XLWorkbook wb = new XLWorkbook())
                {
                    if (type == "items")
                    {
                        wb.Worksheets.Add(data, "Excel Export Products");
                    }
                    else if (type == "cross")
                    {
                        wb.Worksheets.Add(data, "Excel Export Cross");
                    }
                    else if (type == "itemsWithIds")
                    {
                        wb.Worksheets.Add(data, "Excel Export Products to Import");
                    }
                    else if (type == "crossWithIds")
                    {
                        wb.Worksheets.Add(data, "Excel Export Cross to Import");
                    }
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.Buffer = true;
                    HttpContext.Current.Response.Charset = "";
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    if (type == "items")
                    {
                        HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=Excel Export Products.xlsx");
                    }
                    else if (type == "cross")
                    {
                        HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=Excel Export Cross.xlsx");
                    }
                    else if (type == "itemsWithIds")
                    {
                        HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=Excel Export Products to Import.xlsx");
                    }
                    else if (type == "crossWithIds")
                    {
                        HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=Excel Export Cross to Import.xlsx");
                    }

                    using (MemoryStream excelMS = new MemoryStream())
                    {
                        wb.SaveAs(excelMS);
                        excelMS.WriteTo(HttpContext.Current.Response.OutputStream);
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.Response.End();

                        return excelMS;
                    }
                }
            }
            
        }

        public static DataTable ReadAsDataTable(string fileName)
        {

            DataTable dt = new DataTable();
                using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(fileName, false))
                {
                    WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                    IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                    string relationshipId = sheets.First().Id.Value;
                    WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                    Worksheet workSheet = worksheetPart.Worksheet;
                    SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                    IEnumerable<Row> rows = sheetData.Descendants<Row>();


                int cellIndex = 0;
                foreach (Cell cell in rows.ElementAt(0))
                {
                    if (cellIndex == 7)
                    {
                        dt.Columns.Add("DateFrom", typeof(DateTime));
                    }
                    else if (cellIndex == 8)
                    {
                        dt.Columns.Add("DateTo", typeof(DateTime));
                    }
                    else
                    {
                        dt.Columns.Add(GetCellValue(spreadSheetDocument, cell, cellIndex));
                    }
                    cellIndex++;
                }
                cellIndex = 0;
                int rowIndex = 0;
                foreach (Row row in rows) //this will also include your header row...
                {

                    DataRow tempRow = dt.NewRow();
                    int columnIndex = 0;
                   
                    foreach (Cell cell in row.Descendants<Cell>())
                    {
                        // Gets the column index of the cell with data
                        int cellColumnIndex = (int)GetColumnIndexFromName(GetColumnName(cell.CellReference));
                        cellColumnIndex--; //zero based index
                        if (columnIndex < cellColumnIndex)
                        {
                            do
                            {
                                if ((columnIndex == 7 || columnIndex == 8) && rowIndex > 0)
                                {

                                    tempRow[columnIndex] = DBNull.Value;
                                }
                                else
                                {
                                    tempRow[columnIndex] = ""; //Insert blank data here;
                                }
                                
                                columnIndex++;
                            }
                            while (columnIndex < cellColumnIndex);
                        }
                        if ((columnIndex == 7 || columnIndex == 8)&&rowIndex>0)
                        {
                            
                            string dateValue = GetCellValue(spreadSheetDocument, cell, cellIndex);
                            tempRow[columnIndex] = DateTime.FromOADate(double.Parse(dateValue));
                        }
                        else if(rowIndex > 0)
                        {
                            tempRow[columnIndex] = GetCellValue(spreadSheetDocument, cell, cellIndex);
                        }
                        cellIndex++;
                        columnIndex++;
                    }
                    dt.Rows.Add(tempRow);
                    rowIndex++;
                }
            }

            dt.Rows.RemoveAt(0);

            return dt;
            
        }

        /// <summary>
        /// Given a cell name, parses the specified cell to get the column name.
        /// </summary>
        /// <param name="cellReference">Address of the cell (ie. B2)</param>
        /// <returns>Column Name (ie. B)</returns>
        public static string GetColumnName(string cellReference)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);
            return match.Value;
        }
        /// <summary>
        /// Given just the column name (no row index), it will return the zero based column index.
        /// Note: This method will only handle columns with a length of up to two (ie. A to Z and AA to ZZ). 
        /// A length of three can be implemented when needed.
        /// </summary>
        /// <param name="columnName">Column Name (ie. A or AB)</param>
        /// <returns>Zero based index if the conversion was successful; otherwise null</returns>
        public static int? GetColumnIndexFromName(string columnName)
        {

            //return columnIndex;
            string name = columnName;
            int number = 0;
            int pow = 1;
            for (int i = name.Length - 1; i >= 0; i--)
            {
                number += (name[i] - 'A' + 1) * pow;
                pow *= 26;
            }
            return number;
        }
        private static string GetCellValue(SpreadsheetDocument document, Cell cell, int cellIndex)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;

            string value = cell.CellValue.InnerXml;

            //if (cellIndex==7 || cellIndex==8 && (cell.DataType != null && cell.DataType.Value == CellValues.Date))
            //{
            //    DateTime date = DateTime.Parse(stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText) ;
            //    return date
            //}

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }
    }
}
