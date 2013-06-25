using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.presentation.nodeFactory;
using umbraco.BasePages;
using umbraco.cms.businesslogic.datatype;

namespace Spreadsheet_Uploader {
    public class TableType {
        public string TypeName;
    }
    //create prevalues
    public class tableTypePicker : DataEditorSettingType {
        private DropDownList ddl = new DropDownList();

        private string _val = string.Empty;
        public override string Value {
            get {
                return ddl.SelectedValue;
            }
            set {
                if (!string.IsNullOrEmpty(value))
                    _val = value;
            }
        }

        public override System.Web.UI.Control RenderControl(DataEditorSetting setting) {
            ddl.ID = setting.GetName();
            ddl.Items.Clear();
            ddl.Items.Insert(0, new ListItem(String.Empty, String.Empty));
            ddl.SelectedValue = _val;

            return ddl;
        } 
    }

    public class docProperty {

        private int _id;
        private string _name;
        private string _value;


        public int Id {
            get {
                return _id;
            }
            set {
                _id = value;
            }
        }
        public string Name {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }
        public string Value {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }

    }

    public class htmlTag {
        private string _name;
        private int _startPosition;
        private int _endPosition;
        private string _value;

        public string Name {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }
        public int StartPosition {
            get {
                return _startPosition;
            }
            set {
                _startPosition = value;
            }
        }
        public int EndPosition {
            get {
                return _endPosition;
            }
            set {
                _endPosition = value;
            }
        }
        public string Value {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }
    }
    public class mergedRegion {
        private int _rowStart;
        private int _rowEnd;
        private int _colStart;
        private int _colEnd;

        public int RowStart {
            get {
                return _rowStart;
            }
            set {
                _rowStart = value;
            }
        }
        public int RowEnd {
            get {
                return _rowEnd;
            }
            set {
                _rowEnd = value;
            }
        }
        public int ColStart {
            get {
                return _colStart;
            }
            set {
                _colStart = value;
            }
        }
        public int ColEnd {
            get {
                return _colEnd;
            }
            set {
                _colEnd = value;
            }
        }
    }
    public static class StringExtensions {
        public static string ReplaceFirstOccurrance(this string original, string oldValue, string newValue) {
            if (String.IsNullOrEmpty(original))
                return String.Empty;
            if (String.IsNullOrEmpty(oldValue))
                return original;
            if (String.IsNullOrEmpty(newValue))
                newValue = String.Empty;
            int loc = original.IndexOf(oldValue);
            return original.Remove(loc, oldValue.Length).Insert(loc, newValue);
        }
    }
}





