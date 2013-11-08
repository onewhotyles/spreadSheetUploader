using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Collections;
using umbraco.cms.businesslogic.web;
using umbraco.cms.businesslogic.datatype;
using umbraco;
using umbraco.NodeFactory;
using umbraco.editorControls;
using umbraco.BusinessLogic;

using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;


namespace Spreadsheet_Uploader {

    public partial class Download : System.Web.UI.Page {
        Document doc;
        HSSFWorkbook workbook; 
        XmlDocument XMLDoc;
        string tableHtml;
        XmlNodeList xNodeTRs;

        

        protected void Page_Init(object sender, EventArgs e)
        {
            doc = new Document(Convert.ToInt32(Request.QueryString["nodeID"]));
            workbook = new HSSFWorkbook();
            XMLDoc = new XmlDocument();

           
        }

        protected void Page_Load(object sender, EventArgs e) {

            string strAlias = Request.QueryString["alias"];

            exportWorkbook(createHeaderBodyWorkbook(), strAlias + ".xls");
   
        }

       
        private HSSFWorkbook createAllWorkbook()
        {
            SpreadsheetDataType ss=new SpreadsheetDataType();

            foreach (umbraco.cms.businesslogic.property.Property prop in doc.GenericProperties)
            {
                if (prop.PropertyType.DataTypeDefinition.DataType.Id == ss.Id)
                {
                    tableHtml = prop.Value.ToString();
                    XMLDoc.LoadXml(tableHtml);
                    xNodeTRs = XMLDoc.SelectNodes("table//tr");
                    addSheet(workbook, prop.PropertyType.Name, xNodeTRs);


                }
            }

            return workbook;
        }

        private HSSFWorkbook createHeaderBodyWorkbook()
        {
            string strAlias = Request.QueryString["alias"];
            string type = Request.QueryString["type"];
            string strIndex = (Convert.ToInt32(Request.QueryString["index"]) + 1).ToString();

            string tableHtml = doc.getProperty(strAlias).Value.ToString();
            
            XMLDoc.LoadXml(tableHtml);

            XmlNodeList xNodeTRs; 

            if(!String.IsNullOrEmpty(Request.QueryString["index"])){
                xNodeTRs = XMLDoc.SelectNodes("(//table)[" + strIndex + "]//tr");
                addSheet(workbook, "sheet1", xNodeTRs);
                
            } else {
                xNodeTRs = XMLDoc.SelectNodes("table//tr");
                addSheet(workbook, "sheet1", xNodeTRs);
            }
            
            return workbook;
        }

       

           
      

        private HSSFRichTextString ApplyHtmlTags(string resultXML, HSSFWorkbook workbook) {

         

            XMLDoc.LoadXml(resultXML);

            XmlNodeList TDxml = XMLDoc.SelectNodes("/td/*");
            string TDtext = XMLDoc.SelectSingleNode("/td").InnerText;
            string tempResult = XMLDoc.SelectSingleNode("/td").InnerXml;

          

            HSSFRichTextString formattedRichText = new HSSFRichTextString(TDtext);

            IFont FontSup = workbook.CreateFont();
            FontSup.TypeOffset = (short)FontSuperScript.SUPER;

            IFont FontSub = workbook.CreateFont();
            FontSub.TypeOffset = (short)FontSuperScript.SUB;

            IFont FontBold = workbook.CreateFont();
            FontBold.TypeOffset = (short)FontBold.Index;
            
  
            string tagXml = "";
            foreach (XmlNode tag in TDxml) {
                tagXml = tag.OuterXml;
                int startPosition = tempResult.IndexOf(tagXml);
                int endPosition = tempResult.IndexOf(tagXml) + tag.InnerText.Length;
                switch (tag.Name){
                    case "sup":
                        formattedRichText.ApplyFont(startPosition, endPosition, FontSup);
                        break;

                    case "sub":
                        formattedRichText.ApplyFont(startPosition, endPosition, FontSub);
                        break;

                }
  
                tempResult = StringExtensions.ReplaceFirstOccurrance(tempResult, tagXml, tag.InnerText);
             
            }




            return formattedRichText;


        }

        private void addSheet(HSSFWorkbook workbook, string sheetName, XmlNodeList xNodeTRs)
        {
            //umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 8008, "workbook: " + workbook);

            if (xNodeTRs.Count==0)
            {
                return;
            }

            ISheet sheet = workbook.CreateSheet(sheetName);

            int colspan = 0;
            int rowspan = 0;
            int rowIndex = 0;
            int columnIndex = 0;
            int rowDuration = 0;
            bool needsShifted = false;
            
            int mergeRegionCount = 0;

            var mergedRegionList = new List<mergedRegion> {
            };


            foreach (XmlNode xNodeTR in xNodeTRs)
            {
                if (xNodeTR.Name.ToLower() == "tr")         //loop through all the TRs.
                {
                    var row = sheet.CreateRow(rowIndex);

                    foreach (XmlNode xNodeTD in xNodeTR.ChildNodes)
                    {
                        if (xNodeTD.Name.ToLower() == "td")
                        {

                            var cell = row.CreateCell(Convert.ToInt32(xNodeTD.Attributes.GetNamedItem("axis").Value));
                            if (needsShifted)
                            {
                                rowDuration -= 1;
                                if (rowDuration <= 0)
                                {
                                    needsShifted = false;
                                    
                                }
                            }

                       
                            /*----get Merged cells ------*/
                            //If the cell is merged
                            if (xNodeTD.Attributes.GetNamedItem("colspan") != null || xNodeTD.Attributes.GetNamedItem("rowspan") != null)
                            {

                                mergedRegion region = new mergedRegion();

                                region.ColStart = 0;
                                region.ColEnd = 0;
                                region.RowStart = 0;
                                region.RowEnd = 0;

                               

                                if (xNodeTD.Attributes.GetNamedItem("colspan") != null)
                                {
                                    colspan = Convert.ToInt32(xNodeTD.Attributes.GetNamedItem("colspan").Value);

                                    region.ColStart = Convert.ToInt32(xNodeTD.Attributes.GetNamedItem("axis").Value);
                                    region.ColEnd = Convert.ToInt32(xNodeTD.Attributes.GetNamedItem("axis").Value) + colspan - 1;
                                }

                                if (xNodeTD.Attributes.GetNamedItem("rowspan") != null)
                                {
                                    rowspan = Convert.ToInt32(xNodeTD.Attributes.GetNamedItem("rowspan").Value);

                                    region.RowStart = rowIndex;
                                    region.RowEnd = rowIndex + rowspan - 1;
                                }

                                mergedRegionList.Add(region);
                            }


                            
                            //Enable text wrap in the cell
                            cell.CellStyle.WrapText = true;

                            //The xml string of the current <td>
                            string resultXml = xNodeTD.OuterXml;
                            
                            //entity conversion
                            resultXml = resultXml.Replace("&#216;", "Ø");
                            resultXml = resultXml.Replace("<br />", "\n");

                            //convert html tags to Excel font style calls
                            HSSFRichTextString formattedRichText = ApplyHtmlTags(resultXml, workbook);
                            
                            //set the data type of the cell
                            cell.SetCellType(CellType.STRING);

                            //save formatted value into cell
                            cell.SetCellValue(formattedRichText);

                            mergeRegionCount += 1;
                            colspan = 0;
                            rowspan = 0;
                            columnIndex++;
                        }

                    }
                    rowIndex++;
                    columnIndex = 0;
                } //end if TR exists
                if (!needsShifted)
                {
                    //indexShift = 0;
                }

            } //end for loop for TRs

            foreach (mergedRegion merge in mergedRegionList) {
                if (merge != null) {
                    NPOI.SS.Util.CellRangeAddress region = new NPOI.SS.Util.CellRangeAddress(merge.RowStart, merge.RowEnd, merge.ColStart, merge.ColEnd);
                    sheet.AddMergedRegion(region);
                }
            }
        }

        void exportWorkbook(HSSFWorkbook workbook, string fileName)
        {
            // Save the Excel spreadsheet to a MemoryStream and return it to the client
            MemoryStream exportData = new MemoryStream();
            workbook.Write(exportData);
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.BinaryWrite(exportData.GetBuffer());
            HttpContext.Current.Response.End();
        }
    }
}