using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spreadsheet_Uploader {
    public class SpreadsheetDataType : umbraco.cms.businesslogic.datatype.BaseDataType, umbraco.interfaces.IDataType {
        private umbraco.interfaces.IDataEditor _Editor;
        private umbraco.interfaces.IData _baseData;
        private SpreadsheetPrevalueEditor _prevalueeditor;
        public static string defaultValue = "<table class='no-class'></table>";

        public override umbraco.interfaces.IDataEditor DataEditor
        {
            get
            {
                if (_Editor == null)
                    _Editor = new SpreadsheetDataEditor(Data, ((SpreadsheetPrevalueEditor)PrevalueEditor).Configuration);
                return _Editor;
            }
        }

        //public override umbraco.interfaces.IData Data {
        //    get {
        //        if (_baseData == null)
        //            _baseData = new umbraco.cms.businesslogic.datatype.DefaultData(this);
        //        return _baseData;
        //    }
        //}


        public override umbraco.interfaces.IData Data {
            get {
                if (_baseData == null)
                    _baseData = new SpreadsheetData(this);
                return _baseData;
            }
        }
        public override Guid Id
        {
            get {
                return new Guid("CCD8F9F1-31D2-4301-975A-B901ED48BFA2");
            }
        }
 
        public override string DataTypeName
        {
            get { return "Spreadsheet Uploader"; }
        }
 
        public override umbraco.interfaces.IDataPrevalue PrevalueEditor
        {
            get
            {
                if (_prevalueeditor == null)
                    _prevalueeditor = new SpreadsheetPrevalueEditor(this);
                return _prevalueeditor;
            }
        }
    }
}