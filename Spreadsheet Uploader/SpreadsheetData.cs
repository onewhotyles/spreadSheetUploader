﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;


namespace Spreadsheet_Uploader {
    public class SpreadsheetData : umbraco.cms.businesslogic.datatype.DefaultData {
        public SpreadsheetData(umbraco.cms.businesslogic.datatype.BaseDataType DataType)
            : base(DataType) {
        }

        public override System.Xml.XmlNode ToXMl(System.Xml.XmlDocument data) {
            
            XmlDocument xd = new XmlDocument();
            try {
                xd.LoadXml(this.Value.ToString());
            }
            catch (Exception e) {
                this.Value = SpreadsheetDataType.defaultValue;
                xd.LoadXml(this.Value.ToString());
            }
           
            XmlNode wrapNode = xd.CreateNode(XmlNodeType.CDATA, "spreadsheet", null);
            wrapNode.Value = xd.OuterXml;
            return data.ImportNode(xd.DocumentElement, true);
        }

    }
   
}