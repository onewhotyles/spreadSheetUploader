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
using umbraco.interfaces;
using umbraco.IO;

namespace Spreadsheet_Uploader
{
    
    public class SpreadsheetDataEditor : System.Web.UI.UpdatePanel, umbraco.interfaces.IDataEditor
    {
        
        private umbraco.interfaces.IData _data;
        private string _prevalues;
        
        public string strAlias;

        private XmlDocument savedXML = new XmlDocument();

        protected HtmlGenericControl wrapper;
        protected HtmlGenericControl tblMain;
        protected HiddenField HiddenHeadValue;
        protected TextBox HiddenTableValue;
        protected HiddenField HiddenListValue;
        protected string styleValue;
        protected string styleText;
        public HtmlGenericControl UmbracoClientManager;
        public HtmlGenericControl javascriptInclude;
        private DropDownList styleDDL;
        private Literal ltrlCurrentSavedHeader;
        private Literal ltrlCurrentSavedTable;

        public string strResultHeader;
        public string strResultBody;

        string[] config;
        bool renderTableMode = true;

        /* contructor which brings in the prevalue named Configuration COMMENT! */
        public SpreadsheetDataEditor(umbraco.interfaces.IData Data, string Configuration)
        {
            
           //ini the _data object
            _data = Data;
            _prevalues = Configuration;
         }

        public virtual bool TreatAsRichTextEditor
        {
            get { return false; }
        }

        public bool ShowLabel
        {
            get { return true; }
        }
        
        public Control Editor { get { return this; } }

        public void Save()
        {
            if (renderTableMode)
            {
        

                ltrlCurrentSavedTable.Text = HiddenTableValue.Text.Replace("<br>", "<br />");
                this._data.Value = HiddenTableValue.Text.Replace("<br>", "<br />");
            }
        }


        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            
            strAlias = this.Editor.ID.ToString();
            strAlias = strAlias.Substring(5);
            if (strAlias == "") strAlias = "0";

        }

        protected override void CreateChildControls() {
            base.CreateChildControls();

            this.EnsureChildControls();
        }

        

        protected override void OnInit(EventArgs e) {
           // umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 999999, this._data.Value.ToString());
            base.OnInit(e);

            //grab the prevalues and split up into their pieces.
            config = _prevalues.Split('|');
            try {
                if (config[1] == "renderAsReport") {
                    renderTableMode = false;
                }
            }
            catch {
                
            }

            string css = string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", "/umbraco/plugins/SpreadsheetUploader/css/SpreadsheetUploader.css");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(SpreadsheetDataEditor), "spreadsheetUploaderCSS", css, false);

            string js = string.Format("<script href=\"{0}\" type=\"text/javascript\"></script>", "/umbraco/plugins/SpreadsheetUploader/js/extensions.js");
            ScriptManager.RegisterClientScriptBlock(Page, typeof(SpreadsheetDataEditor), "spreadsheetExtensions", js, false);

            if (renderTableMode)
            {

                wrapper = new HtmlGenericControl("div")
                {
                    ID = "wrapper"
                };
                wrapper.Attributes.Add("class", "resultsPane");

                styleDDL = new DropDownList();
                styleDDL.CssClass = "styleTitle";
                base.ContentTemplateContainer.Controls.Add(styleDDL);


                ltrlCurrentSavedTable = new Literal();
                ltrlCurrentSavedTable.ID = this.ClientID + "ltrlCurrentSavedBody";
                base.ContentTemplateContainer.Controls.Add(ltrlCurrentSavedTable);

                
                javascriptInclude = new HtmlGenericControl("script");
                javascriptInclude.Attributes["type"] = "text/javascript";
                javascriptInclude.Attributes["src"] = "/umbraco/plugins/SpreadsheetUploader/js/extensions.js";
                base.ContentTemplateContainer.Controls.Add(javascriptInclude);
                

                HiddenTableValue = new TextBox();
                HiddenTableValue.ID = this.ClientID + "HiddenTableValue";
                HiddenTableValue.CssClass = "save-box";
                base.ContentTemplateContainer.Controls.Add(HiddenTableValue);



                //fill the ddl
                if (_prevalues.Contains(","))
                {

                    string[] prevaluelist;
                    prevaluelist = config[0].Split(',');

                    for (int i = 0; i < prevaluelist.Length; i++)
                    {
                        if (prevaluelist[i].Trim().Length > 0)
                        {
                            styleDDL.Items.Add(new ListItem(prevaluelist[i].Trim(), prevaluelist[i].Trim()));
                        }
                    }
                }
                else
                {
                    if (_prevalues.Trim().Length > 0)
                    {
                        string[] pv = _prevalues.Trim().Split('|');
                        styleDDL.Items.Add(new ListItem(pv[0]));
                    }
                }

                try
                {
                    savedXML.LoadXml(this._data.Value.ToString());
                }
                catch (Exception error)
                {

                    savedXML.LoadXml(SpreadsheetDataType.defaultValue);
                }
                //umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 87878787, "style: " + savedXML.Value);
                XmlNode savedTable = savedXML.SelectSingleNode("//table");
                //problem is the next few lines...


                if (savedTable == null) {
                    savedXML.LoadXml(SpreadsheetDataType.defaultValue);
                    savedTable = savedXML.SelectSingleNode("//table");
                }

                    if (savedTable.Attributes["class"] != null) {
                        if (savedTable.Attributes["class"].Value.ToString().Trim().Length > 0) {
                            styleDDL.SelectedValue = savedTable.Attributes["class"].Value.ToString().Trim();
                        }
                        else {
                            
                            styleDDL.SelectedIndex = 0;
                        }
                    }
                    
             

            }
            else
            {

            }

            this.EnsureChildControls();
        }
      
        protected override void Render(HtmlTextWriter writer) {

            if (renderTableMode)
            {
                javascriptInclude.RenderControl(writer);

                try
                {
                    savedXML.LoadXml(this._data.Value.ToString());
                }
                catch (Exception error)
                {

                    savedXML.LoadXml(SpreadsheetDataType.defaultValue);
                }



                XmlNode tableBuilder = savedXML.SelectSingleNode("//table");
           

                ltrlCurrentSavedTable.Text = tableBuilder.OuterXml;

                HiddenTableValue.Text = tableBuilder.OuterXml;
              
                writer.WriteLine("<div class='control-wrap'>");
      
                writer.WriteLine("<div class='controls-list'>");
                writer.WriteLine("<ul>");

                writer.WriteLine("<li><a class='edit-upload' href=\"javascript:UmbClientMgr.openModalWindow('plugins/SpreadSheetUploader/EditSpreadsheet.aspx?nodeID=" + HttpContext.Current.Request.QueryString["id"] + "&alias=" + strAlias + "&clientID = " + this.ClientID + "&style=" + styleDDL.SelectedValue + "&type=tbody', 'Edit Spreadsheet Body', true, 960, 630,'','','', function(returnValue){updateTable('tbody', '" + this.ClientID + "', returnValue)} );\">Edit/Upload</a></li>");


                if (HiddenTableValue.Text != "")
                {
                    writer.WriteLine("<li>");
                    writer.WriteLine("<a id='" + this.ClientID + "_DownloadBody' href=\"plugins/SpreadSheetUploader/Download.aspx?nodeID=" + HttpContext.Current.Request.QueryString["id"] + "&alias=" + strAlias + "&type=tbody\">Download</a>");
                    writer.WriteLine("</li>");
                }
                writer.WriteLine("<li class='style-select'>");
                writer.WriteLine("<label>Style:</label>");
                styleDDL.RenderControl(writer);
                writer.WriteLine("</li>");
                writer.WriteLine("<li class='remove'>");
                writer.WriteLine("<a id='" + this.ClientID + "_RemoveBody' href=\"javascript:Remove('" + this.ClientID + "','tbody')\">Clear Table</a>");
                writer.WriteLine("</li>");
                writer.WriteLine("</ul>");
                writer.WriteLine("</div>");
           
                writer.WriteLine("<div class='table-wrapper' id='" + this.ClientID + "'>");
                ltrlCurrentSavedTable.RenderControl(writer);
               
                writer.WriteLine("</div>");
                writer.WriteLine("</div>");
                HiddenTableValue.RenderControl(writer);
            }
            else
            {
                writer.WriteLine("<a href='plugins/SpreadSheetUploader/Download.aspx?nodeID=" + HttpContext.Current.Request.QueryString["id"] + "'>Download All Spreadsheets from This Document</a><br/>");
                writer.WriteLine("<a href='plugins/SpreadSheetUploader/Download.aspx?nodeID=" + HttpContext.Current.Request.QueryString["id"] + "&split=true'>Download All Spreadsheets from This Document Split</a>");
            }
        }
    }
}