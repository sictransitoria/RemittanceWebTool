using CCURemittanceAPI.Helpers;
using CCURemittanceAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Data.SqlClient;
using System.Linq;

namespace CCURemittanceAPI.Controllers
{
    public class CCURemittanceController : ApiController
    {
        //     ||     \\
        //     ||     \\
        //    ||||    \\
        //  || ** ||  \\
        //  || ** ||  \\
        //  || ** ||  \\
        //  || ** ||  \\
        //  || ** ||  \\
        //  || ** ||  \\
        //  || ** ||  \\  
        public async Task<IHttpActionResult> PaymentProcessUpload()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Content(HttpStatusCode.BadRequest, new APIResponseModel(false, "Upload failed. UnsupportedMediaType."));
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);

                using (var dbContext = new FormViewerEntities1())
                {
                    // read the form input.
                    foreach (var key in provider.FormData.AllKeys)
                    {
                        foreach (var val in provider.FormData.GetValues(key))
                        {
                            Trace.WriteLine(string.Format("{0}: {1}", key, val));
                        }
                    }

                    // get the file names, files
                    int fileCount = 0;
                    DateTime curDate = DateTime.Now;
                    foreach (MultipartFileData file in provider.FileData)
                    {
                        if (fileCount == 1)
                        {
                            break;
                        }

                        Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                        Trace.WriteLine("Server file path: " + file.LocalFileName);

                        #region file name code

                        // get the filename
                        var theFileName = file.Headers.ContentDisposition.FileName;

                        // remove the quotations
                        theFileName = theFileName.Replace(@"""", "");

                        // get the file extension of the file being uploaded (.xlsx or .xls)
                        string fileExtension = System.IO.Path.GetExtension(theFileName);

                        // final result of the filename
                        string fileNameResult = theFileName.Substring(0, theFileName.Length - fileExtension.Length);

                        // make sure not write the records again if the user uploads the same file
                        bool checkIfFileExists = dbContext.CCURemittanceHistories.Where(a => a.FileName == fileNameResult).Any();

                        if (checkIfFileExists)
                        {
                            break;
                        }

                        #endregion

                        // member info
                        DataSet ds = MicrosoftExcelTool.ReadExcelFileMemberInfo(file.LocalFileName);
                        DataTable dtbl = ds.Tables[0];

                        // invoice info
                        DataSet dsDeux = MicrosoftExcelTool.ReadExcelFileInvoiceNum(file.LocalFileName);
                        DataTable dtblDeux = dsDeux.Tables[0];

                        if (dtbl.Rows.Count > 0 && dtblDeux.Rows.Count > 0)
                        {
                            #region get member info
                            List<CCURemittanceMemberInfo> processRecords = new List<CCURemittanceMemberInfo>();
                            foreach (DataRow row in dtbl.Rows)
                            {
                                string lastName = row[0].ToString();

                                // skip rows without a LastName
                                if (lastName.Equals(""))
                                {
                                    continue;
                                }

                                    string firstName = row[2].ToString();
                                    string clientID = row[4].ToString();
                                    string SSN = row[5].ToString();
                                    string serviceDate = row[8].ToString();
                                    string BN = row[9].ToString();
                                    string SV = row[10].ToString();
                                    string rate = row[12].ToString();
                                    string units = row[13].ToString();
                                    string gross = row[14].ToString();
                                    string fee = row[15].ToString();
                                    string net = row[16].ToString();
                                    string fileName = theFileName.ToString();

                                // save to table
                                CCURemittanceMemberInfo paymentRecords = new CCURemittanceMemberInfo()
                                {
                                    Last = lastName,
                                    First = firstName,
                                    ClientID = clientID,
                                    SSN = SSN,
                                    ServiceDate = serviceDate,
                                    BN = BN,
                                    SV = SV,
                                    Rate = rate,
                                    Units = units,
                                    Gross = gross,
                                    Fee = fee,
                                    Net = net,
                                    FileName = fileNameResult
                                };

                                processRecords.Add(paymentRecords);
                            }

                            dbContext.CCURemittanceMemberInfoes.AddRange(processRecords);
                            dbContext.SaveChanges();

                            #endregion

                            #region get Invoice Info
                            List<CCURemittanceInvoiceInfo> processRecordsDeux = new List<CCURemittanceInvoiceInfo>();
                            foreach (DataRow row2 in dtblDeux.Rows)
                            {
                                string invoiceNum = row2[1].ToString();
                                string invoiceDate = row2[4].ToString();
                                string voucherNum = row2[6].ToString();
                                string voucherDate = row2[8].ToString();
                                string warNum = row2[10].ToString();
                                string warDate = row2[12].ToString();
                                string fileName = theFileName.ToString();

                                // save to table
                                CCURemittanceInvoiceInfo paymentRecordsDeux = new CCURemittanceInvoiceInfo()
                                {
                                    InvoiceNum = invoiceNum,
                                    InvoiceDate = invoiceDate,
                                    VoucherNum = voucherNum,
                                    VoucherDate = voucherDate,
                                    WarNum = warNum,
                                    WarDate = warDate,
                                    FileName = fileNameResult
                                };

                                processRecordsDeux.Add(paymentRecordsDeux);
                            }

                            dbContext.CCURemittanceInvoiceInfoes.AddRange(processRecordsDeux);
                            dbContext.SaveChanges();

                            #endregion

                            #region history of uploads
                            var ccuRemittanceHistory = new CCURemittanceHistory()
                            {
                                UploadDate = curDate,
                                FileName = fileNameResult
                            };
                            #endregion

                            dbContext.CCURemittanceHistories.Add(ccuRemittanceHistory);
                            dbContext.SaveChanges();

                            fileCount++;
                        }
                        else
                        {
                            return Content(HttpStatusCode.BadRequest, new APIResponseModel(false, "Upload failed."));
                        }
                    }
                    CCUResponseModel res = new CCUResponseModel
                    {
                        result = "Success"
                    };
                    string result = JsonConvert.SerializeObject(res);
                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, "Submit failed. An unexpected error occured.");
            }
        }

        [HttpGet]
        public IHttpActionResult GetReport(string FileName)
        {
            using (var dbContext = new FormViewerEntities1())
            {
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@fileName", FileName);

                string sql = @"	 -- CCU REMITTANCE script
                                    IF OBJECT_ID('tempdb..#ccuRemittance') IS NOT NULL
                                    DROP TABLE #ccuRemittance

                                    SELECT 
	                                    r.ID AS 'DataID'
                                    ,	r.[Last] AS 'Last'
                                    ,	r.[First] AS 'First'
                                    ,	r.ClientID AS 'ClientID'
                                    ,	r.SSN AS 'SSN'
                                    ,	r.ServiceDate AS 'SVDT'
                                    ,	r.Net AS 'NET'
                                    ,	d.InvoiceNum AS 'InvoiceNumber'
                                    ,	d.[FileName] AS 'FileName'
                                    ,	d.InvoiceDate AS 'InvoiceDate'
                                    ,	NULL AS 'FileTotal'

	                                    INTO 
		                                    #ccuRemittance

	                                    FROM 
		                                    [FormViewer].[dbo].CCURemittanceInvoiceInfo d

	                                    JOIN 
		                                    [FormViewer].[dbo].CCURemittanceMemberInfo r ON d.[FileName] = r.[FileName]

	                                    --

	                                    SELECT
		                                    [Last]
	                                    ,	[First]
	                                    ,	[ClientID]
	                                    ,	SSN
	                                    ,	CAST(SVDT - 2 AS smallDATETIME) AS 'SVDT'
	                                    ,	SUM(CAST(NET AS FLOAT)) AS 'NET'
	                                    ,	InvoiceNumber
	                                    ,	[FileName]
	                                    ,	CAST(InvoiceDate - 2 AS smallDATETIME) AS 'InvoiceDate'
	                                    ,	SUM(SUM(CAST(NET AS FLOAT))) OVER () AS 'FileTotal'
		
		                                    FROM 
			                                    #ccuRemittance

		                                    WHERE
			                                    [FileName] = @fileName

		                                    GROUP BY
			                                    [Last]
		                                    ,	[First]
		                                    ,	[ClientID]
		                                    ,	SSN
		                                    ,	SVDT
		                                    ,	InvoiceNumber
		                                    ,	[FileName]
		                                    ,	InvoiceDate
		                                    ,   DataID

		                                    ORDER BY DataID";

                object detail = SQLDBHelper.QueryDbDataTable(dbContext.Database.Connection.DataSource, dbContext.Database.Connection.Database, sql, parameters);

                string result = JsonConvert.SerializeObject(detail);

                return Ok(result);
            }
        }
    }
}