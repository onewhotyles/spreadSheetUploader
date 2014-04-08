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
using System.Data.OleDb;
using System.Data;
using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;




namespace Spreadsheet_Uploader
{
    public partial class EditSpreadsheet : umbraco.BasePages.UmbracoEnsuredPage
    {
        string strType = HttpContext.Current.Request.QueryString["type"];
        int nodeID = Convert.ToInt32(HttpContext.Current.Request.QueryString["nodeID"]);
        string strAlias = HttpContext.Current.Request.QueryString["alias"];

       
        protected void btnUpload_Click(object sender, EventArgs e) {
           
            string strFileType = System.IO.Path.GetExtension(uFilePath.FileName).ToString().ToLower();

            if (!uFilePath.HasFile) {
                lblMessage.Text = "You must Select a header file for upload.";
                lblMessage.Visible = true;
            }
            else if (strFileType == ".xlsx") {

                lblMessage.Text = "Spreadsheet Uploader does not currently support .xlsx.  Please select an .xls file.";
                insertButton.Disabled = true;

                
            }
            else if (strFileType == ".xls") {

                convertToHtml(uFilePath, ltrlCurrentSaved);
                insertButton.Disabled = false;

            }
            else {

                lblMessage.Text = "You must Select an .xls file for upload.";
                lblMessage.Visible = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e) {

            string strStyle = HttpContext.Current.Request.QueryString["style"];
            string strClientID = HttpContext.Current.Request.QueryString["clientID"];
            hfStyle.Value = strStyle;
            Document theDoc = new Document(nodeID);

            XmlDocument XMLDoc = new XmlDocument();
            string strTheDoc = "<table class='no-class'><thead></thead><tbody></tbody></table>";

            //Prior version grabbed table from property alias, but now save table in session to play nice with Widget Builder
           /* if (theDoc.getProperty(strAlias) != null && (string)theDoc.getProperty(strAlias).Value != "") {
                strTheDoc = theDoc.getProperty(strAlias).Value.ToString();
            }*/
            if (Session["sessionTable"] != null) {
                strTheDoc = Session["sessionTable"].ToString();
            }

            XMLDoc.LoadXml(strTheDoc);
            XmlNode xNodeTable;

            xNodeTable = XMLDoc.SelectSingleNode("table");
 
            ltrlCurrentSaved.Text = xNodeTable.OuterXml;
            
           
            lblMessage.Visible = false;
            lblMessage.Visible = false;

            lblMessage.Text = "Please select an excel file first";
        }

        private void convertToHtml(FileUpload txtFilePath, Literal tableOutput) {

            string strTable = "<table id=\"tblEditTable\" class='" + hfStyle.Value + "'>";
            string strSearchTable = "<spreadsheet><page></page><table id=\"tblEditTable\" class='" + hfStyle.Value + "'>";
            int formattingRunIndex;
            int toIndex = 0;
            int fromIndex = 0;
            int subStringLength = 0;
            int furthestColumn = 0;
            int rowLastColumn = 0;
            string strNewPath = "";
            if ((txtFilePath.HasFile)) {
                int numberOfMergedRegions = 0;
                int rowCount = 0;

                HSSFWorkbook hssfwb;

                string strFileType = System.IO.Path.GetExtension(txtFilePath.FileName).ToString().ToLower();



                txtFilePath.SaveAs(Server.MapPath("~/" + GlobalVariables.datatypePath + "/TempData/" + txtFilePath.FileName));
                strNewPath = Server.MapPath("~/" + GlobalVariables.datatypePath + "/TempData/" + txtFilePath.FileName);

                using (FileStream file = new FileStream(strNewPath, FileMode.Open, FileAccess.Read)) {
                    hssfwb = new HSSFWorkbook(file);
                }

                ISheet sheet = hssfwb.GetSheetAt(0);

                NPOI.SS.Util.CellRangeAddress[] cellRange;
                try {
                    numberOfMergedRegions = sheet.NumMergedRegions;
                }
                catch (SystemException e) {
                    numberOfMergedRegions = 0;
                }
                if (numberOfMergedRegions > 0) {
                    cellRange = new NPOI.SS.Util.CellRangeAddress[sheet.NumMergedRegions];

                    for (int i = 0; i < sheet.NumMergedRegions; i++) {

                        cellRange[i] = sheet.GetMergedRegion(i);
                    }

                }
                else {
                    cellRange = new NPOI.SS.Util.CellRangeAddress[0];
                }
             
                for (int row = 0; row <= sheet.LastRowNum; row++) {
                    strTable += "<tr>";
                    strSearchTable += "<tr>";
                    for (int c = 0; c < sheet.GetRow(row).Cells.Count; c++) {

                        if (sheet.GetRow(row).GetCell(c) == null) {

                            sheet.GetRow(row).CreateCell(c);
                           
                        }

                        fromIndex = 0;
                        toIndex = 0;
                        subStringLength = 0;
                        var cell = sheet.GetRow(row).Cells[c];
                        var cellmerge = sheet.GetRow(row).Cells[c];
                        var style = cell.CellStyle;
                        var font = style.GetFont(hssfwb);
                        int intColspan = 0;
                        int intRowspan = 0;
                        bool repeatedCell = false;
                        
                        

                        

                        rowLastColumn = sheet.GetRow(row).LastCellNum - 1;

                        if (furthestColumn < rowLastColumn) {
                            furthestColumn = rowLastColumn;
                        }

                        //if we are on the last cell of the row
                        if (sheet.GetRow(row).GetCell(c).ColumnIndex == rowLastColumn) {
                           
                            if (sheet.GetRow(row).GetCell(c).ColumnIndex < furthestColumn) {
                                int cellDiff = furthestColumn - sheet.GetRow(row).GetCell(c).ColumnIndex;
                              
                                for (int n = 0; n < cellDiff; n++) {
                                    sheet.GetRow(row).CreateCell(n + rowLastColumn + 1);
                                }
                            }
                         
                        }


                        strSearchTable += "<td ";
                        strSearchTable += " axis = '" + cell.ColumnIndex + "'>";
                        if (numberOfMergedRegions > 0) {
                            if (!cell.IsMergedCell) {
                                strTable += "<td ";
                                
                            }
                            foreach (NPOI.SS.Util.CellRangeAddress region in cellRange) {

                                // If the region does contain the cell you have just read from the row
                                if (region.IsInRange(cell.RowIndex, cell.ColumnIndex))
                                {
                                    // need to get the cell from the top left hand corner of this
                                    int rowNum = region.FirstRow;
                                    int colIndex = region.FirstColumn;
                                    if (sheet.GetRow(row).Cells[c].CellType == NPOI.SS.UserModel.CellType.NUMERIC)
                                    {
                                        sheet.GetRow(row).Cells[c].SetCellType(NPOI.SS.UserModel.CellType.STRING);
                                    }

                                    string theRealValue = sheet.GetRow(row).Cells[c].RichStringCellValue.ToString();
                                    intColspan = region.LastColumn - region.FirstColumn + 1;
                                    intRowspan = region.LastRow - region.FirstRow + 1;
                                    if (theRealValue != "")
                                    {
                                        strTable += "<td ";
                                        //intColspan = region.LastColumn - region.FirstColumn + 1;
                                        strTable += "colspan='" + intColspan.ToString() + "' ";
                                        //intRowspan = region.LastRow - region.FirstRow + 1;
                                        strTable += "rowspan='" + intRowspan.ToString() + "' ";

                                        strTable += " axis='" + cell.ColumnIndex + "'";

                                        strTable += ">";
                                        
                                        strSearchTable += theRealValue + "</td>";
                                    }
                                    else
                                    {
                                        repeatedCell = true;
                                        strSearchTable += sheet.GetRow(region.FirstRow).Cells[region.FirstColumn].RichStringCellValue.ToString() + "</td>";
                                    }
                                }
                                //else
                                //{
                                    //strSearchTable += sheet.GetRow(row).Cells[c].RichStringCellValue.ToString() +"</td>";
                                //}
                            }

                            if (!cell.IsMergedCell) {
                                
                                strTable += " axis='" + cell.ColumnIndex + "'>";
                                //strSearchTable += " axis = '" + cell.ColumnIndex + "'>";
                            }

                        }
                        else {

                                //strSearchTable += " axis = '" + cell.ColumnIndex + "'>";
                                strTable += "<td axis='" + cell.ColumnIndex + "'>";
                            
                        }
                        if (cell.CellType != NPOI.SS.UserModel.CellType.STRING) {
                            cell.SetCellType(NPOI.SS.UserModel.CellType.STRING);
                        }
                        var rts = cell.RichStringCellValue;

                        if (!repeatedCell) {

                            if (rts.NumFormattingRuns > 0) {
                                for (formattingRunIndex = 0; formattingRunIndex < rts.NumFormattingRuns; formattingRunIndex++) {
                                    // Get that point in the cells text where the next formatting run will begin. Use this to
                                    // display the substring the format will apply to. There is no option to recover the length of a
                                    // formatting run using HSSF and so it is necessary to always look ahead.
                                    toIndex = rts.GetIndexOfFormattingRun(formattingRunIndex);
                                    subStringLength = rts.Length - ((fromIndex - 1) + (rts.Length - toIndex + 1));
                                    string cellValue = HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength));

                                    // It was proving problematic to obtain the font object using the index number of the formatting
                                    // so, this time, the font information is obtained using the position of the starting point of the substring within the cells contents. 

                                    if (font.TypeOffset == 1) {
                                        strTable += "<sup>" + HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength)) + "</sup>";
                                        strSearchTable += "<sup>" + HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength)) + "</sup>";
                                    }
                                    else if (font.TypeOffset == 2) {
                                        strTable += "<sub>" + HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength)) + "</sub>";
                                        strSearchTable += "<sub>" + HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength)) + "</sub>";
                                    }
                                    else {
                                        strTable += HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength));
                                        strSearchTable += HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength));
                                    }
                                    font = hssfwb.GetFontAt(rts.GetFontAtIndex(rts.GetIndexOfFormattingRun(formattingRunIndex)));

                                    // Make sure that the starting point for the next
                                    // substring will be the end point of the last 
                                    fromIndex = toIndex;
                                }
                                // Handle the very final part of the cells contents.
                                toIndex = rts.Length;
                                subStringLength = rts.Length - ((fromIndex - 1) + (rts.Length - toIndex + 1));

                                font = hssfwb.GetFontAt(rts.GetFontAtIndex(fromIndex));
                                if (font.TypeOffset == 1) {
                                    strTable += "<sup>" + HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength)) + "</sup>";
                                    strSearchTable += "<sup>" + HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength)) + "</sup>";
                                }
                                else if (font.TypeOffset == 2) {
                                    strTable += "<sub>" + HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength)) + "</sub>";
                                    strSearchTable += "<sub>" + HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength)) + "</sub>";
                                }
                                else {
                                    strTable += HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength));
                                    strSearchTable += HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength));
                                }
                            }
                            else {
                                // If there are no formatting runs in the RichTextString then that string has only a single format applied
                                // to it. In this case, all information can be obtained from the font information contained within the cell style object. 

                                if (font.TypeOffset == 1) {
                                    if (rts.ToString().Substring(fromIndex, subStringLength) != "") {
                                        strTable += "<sup>" + HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength)) + "</sup>";
                                        strSearchTable += "<sup>" + HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength)) + "</sup>";
                                    }
                                    else {
                                    }
                                }
                                else if (font.TypeOffset == 2) {
                                    if (rts.ToString().Substring(fromIndex, subStringLength) != "") {
                                        strTable += "<sub>" + HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength)) + "</sub>";
                                        strSearchTable += "<sub>" + HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength)) + "</sub>";
                                    }
                                    else {
                                        strTable += "<sub>" + HttpUtility.HtmlEncode(rts.ToString()) + "</sub>";
                                        strSearchTable += "<sub>" + HttpUtility.HtmlEncode(rts.ToString()) + "</sub>";
                                    }
                                }
                                else {
                                    if (rts.ToString().Substring(fromIndex, subStringLength) != "") {
                                        strTable += HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength));
                                        strSearchTable += HttpUtility.HtmlEncode(rts.ToString().Substring(fromIndex, subStringLength));
                                    }
                                    else {
                                        strTable += HttpUtility.HtmlEncode(sheet.GetRow(row).Cells[c].RichStringCellValue.ToString().Trim());
                                        strSearchTable += HttpUtility.HtmlEncode(sheet.GetRow(row).Cells[c].RichStringCellValue);
                                    }
                                }
                            }
                            strSearchTable += "</td>";
                            strTable += "</td>";
                            repeatedCell = false;
                        }
                    }
                    strSearchTable += "</tr>";
                    strTable += "</tr>";
                    rowCount += 1;
                    strTable = strTable.Replace(Convert.ToChar(13).ToString(), "<br />");
                    strTable = strTable.Replace(Convert.ToChar(10).ToString(), "<br />");
                }
            }
            tableOutput.Text = strTable + "</table>";
            //tableOutput.Text = strSearchTable + "</table>";
            System.IO.File.Delete(strNewPath);
        }
    }
}