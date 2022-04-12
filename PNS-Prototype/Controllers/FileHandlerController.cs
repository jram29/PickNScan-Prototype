using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PNS_Prototype.Models;
using ExcelDataReader;
using System.IO;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;

namespace PNS_Prototype.Controllers
{
    public class FileHandlerController : Controller
    {
        // GET: FileHandler

        public ActionResult Index()
        {



            return View(); // Unused.
        }


        public ActionResult ImportFile()
        {
            DataTable dt = new DataTable();

            try
            {
                dt = (DataTable)Session["tmpdata"];
            }
            catch (Exception ex)
            {

            }

            return View(dt); // This loads the default view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportFile(HttpPostedFileBase upload)
        {

            // TO DO: SAVE UPLOADED FILE AS EXCEL FILE 
            // NOTE: When uploading the exported file from Infusion
            // it is discovered that is not a original excel file
            // because the current code will only accept orignal .xls/.xlsx

            // This block of code below is untested.\\

            //var excelFileName = upload.ToString();
            //var app = new Microsoft.Office.Interop.Excel.Application();
            //var workbook = app.Workbooks.Open(excelFileName); 

            //if (System.IO.File.Exists(excelFileName))
            //{
            //    System.IO.File.Delete(excelFileName);
            //}

            //workbook.SaveAs(
            //    excelFileName,
            //    Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault,
            //    Type.Missing, Type.Missing,
            //    false, false,
            //    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
            //    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            //workbook.Close();
            //app.Quit();
            //app = null;
            //workbook = null;


            // This block of code is uploading the file and
            // save specific lines into the database.
            if (ModelState.IsValid)
            {
                if(upload != null && upload.ContentLength > 0)
                {
                    Stream stream = upload.InputStream;

                    IExcelDataReader reader = null;


                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateReader(stream);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }
                    int fieldcount = reader.FieldCount;
                    int rowcount = reader.RowCount;


                    DataTable dt = new DataTable();
                    DataTable itemDT = new DataTable();

                    DataRow pRow;
                    DataRow row;

                    object[] packingSlip = new object[9]; 
                    object[] toFulfill = new object[12];

                    DataTable dt_ = new DataTable();
                    try
                    {

                        PNSDbEntities db = new PNSDbEntities();
                        Order o = new Order();

                        int rowcounter = 0;
                        dt_ = reader.AsDataSet().Tables[0];


                        // TO GET PACKING SLIP NO.
                        for (int i = 0; i < 9; i++)
                        {
                            dt.Columns.Add(dt_.Rows[0][i].ToString());
                        }

                        for (int row_ = 3; row_ < 4; row_++)
                        {
                        pRow = dt.NewRow();

                            for (int col = 7; col < 9; col++)
                            {
                            pRow[col] = dt_.Rows[row_][col].ToString();
                                rowcounter++;
                            packingSlip = pRow.ItemArray;
                            
                            }
                            dt.Rows.Add(pRow);
                        }


                    o.Id = Convert.ToInt32(packingSlip[7].ToString());
                    o.PackingSlip = Convert.ToInt32(packingSlip[7].ToString());
                    o.Status = "Ready";
                    o.Created = DateTime.Now;
                    db.Orders.Add(o);
                    db.SaveChanges();



                    // TO GET ITEMS TO BE FULFILLED.
                        for (int i = 0; i < dt_.Columns.Count; i++)
                        {
                            itemDT.Columns.Add(dt_.Rows[0][i].ToString());
                        }

                        for (int row_ = 20; row_ < (dt_.Rows.Count - 9); row_++)
                        {

               
                            row = itemDT.NewRow();
                            

                            for (int col = 0; col < 11; col++)
                            {
                                row[col] = dt_.Rows[row_][col].ToString();
                                rowcounter++;
                            toFulfill = row.ItemArray;
                            
                            }
                            itemDT.Rows.Add(row);


                        if (toFulfill[0].ToString() != null && !String.IsNullOrEmpty(toFulfill[0].ToString().Trim()))
                        {

                            Fulfill f = new Fulfill();

                            f.OrderId = Convert.ToInt32(packingSlip[7].ToString());
                            f.ProCode = toFulfill[0].ToString();
                            f.Description = toFulfill[2].ToString();
                            f.SKU = Convert.ToInt32(toFulfill[4].ToString());
                            f.Order = Convert.ToInt32(toFulfill[5].ToString());
                            f.Supplied = 0;

                            db.Fulfills.Add(f);
                        }


                        db.SaveChanges();
                        }

                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("File", "Unable to Upload file!");
                        return View();
                    }



                    DataSet result = new DataSet();

                    result.Tables.Add(dt);

                    reader.Close();

                    reader.Dispose();

                    DataTable tmp = result.Tables[0];


                    Session["tmpdata"] = tmp;  // Store datatable into session
                    return RedirectToAction("ImportFile");
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            
            return View();
        }
    }
}

