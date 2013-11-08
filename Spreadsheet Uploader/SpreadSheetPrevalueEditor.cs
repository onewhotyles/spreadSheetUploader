using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using umbraco.DataLayer;
using umbraco.BusinessLogic;
using umbraco.editorControls;

namespace Spreadsheet_Uploader {
    public class SpreadsheetPrevalueEditor : System.Web.UI.WebControls.PlaceHolder, umbraco.interfaces.IDataPrevalue {

         #region IDataPrevalue Members

        // referenced datatype
        private umbraco.cms.businesslogic.datatype.BaseDataType _datatype;
        private TextBox _csvBox;
        private CheckBoxList checkboxList;
        
        public SpreadsheetPrevalueEditor(umbraco.cms.businesslogic.datatype.BaseDataType DataType)
        {
            _datatype = DataType;
            setupChildControls();
        }

        private void setupChildControls()
        {
            _csvBox = new TextBox();
            _csvBox.ID = "csvBox";
            _csvBox.CssClass = "umbEditorTextField";
            Controls.Add(_csvBox);

            //checkboxList = new CheckBoxList();
            //checkboxList.ID = "spreadsheetOptions";
            //checkboxList.CssClass="spreadsheetOptions";

            //checkboxList.Items.Add(new ListItem("Render As Report", "renderAsReport"));

            //Controls.Add(checkboxList);
        }

        public Control Editor
       {
            get
            {
                return this;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                if (Configuration.Length > 0)
                {
                    string[] config = Configuration.Split('|');
                    _csvBox.Text = config[0];
                    //try {
                    //    checkboxList.SelectedValue = config[1];
                    //}
                    //catch {
                    //    checkboxList.SelectedValue = "";
                    //}
                }
                else
                {
                    //default value
                    _csvBox.Text = "";
                }
            }
        }

        public void Save()
        {
            _datatype.DBType = (umbraco.cms.businesslogic.datatype.DBTypes)Enum.Parse(typeof(umbraco.cms.businesslogic.datatype.DBTypes), DBTypes.Ntext.ToString(), true);
            string data = _csvBox.Text+"|"+checkboxList.SelectedValue;

            SqlHelper.ExecuteNonQuery("delete from cmsDataTypePreValues where datatypenodeid = @dtdefid", SqlHelper.CreateParameter("@dtdefid", _datatype.DataTypeDefinitionId));
            SqlHelper.ExecuteNonQuery("insert into cmsDataTypePreValues (datatypenodeid,[value],sortorder,alias) values (@dtdefid,@value,0,'')",SqlHelper.CreateParameter("@dtdefid", _datatype.DataTypeDefinitionId), SqlHelper.CreateParameter("@value", data));
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.WriteLine("<label>CSS Style List for Dropdown</label>");
            _csvBox.RenderControl(writer);

            //checkboxList.RenderControl(writer);
        }

        public string Configuration
        {
            get
            {
                object conf = SqlHelper.ExecuteScalar<object>("select value from cmsDataTypePreValues where datatypenodeid = @datatypenodeid", SqlHelper.CreateParameter("@datatypenodeid", _datatype.DataTypeDefinitionId));

                if (conf != null)
                    return conf.ToString();
                else
                    return "";
            }
        }
        #endregion

        public static ISqlHelper SqlHelper
        {
            get
            {
                return Application.SqlHelper;
            }
        }
    }
}