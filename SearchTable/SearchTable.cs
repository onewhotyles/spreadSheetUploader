using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Collections;
using System.Data.OleDb;
using System.Data;
using System.IO;

using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;


namespace SearchTable
{
    public class SearchTable : ApplicationEventHandler
    {
        private static object _lockObj = new object();
        private static bool _ran = false; 

        public void OnApplicationInitialized(UmbracoApplication httpApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarting(UmbracoApplication httpApplication, ApplicationContext applicationContext)
        {
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {

              // lock
            if (!_ran)
            {
                lock (_lockObj)
                {
                    if (!_ran)
                    {


            base.ApplicationStarted(umbracoApplication, applicationContext);

            if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(strFilePath)))
            {
                StreamWriter sw = new StreamWriter(HttpContext.Current.Server.MapPath(strFilePath), true);
                sw.Write("<?xml version='1.0' encoding='utf-8'?><tableIndexer></tableIndexer>");
                sw.Close();
            }
               
            XmlDocument xmlDoc = new XmlDocument();
            XmlDocument tempDoc = new XmlDocument();
            
            xmlDoc.Load(HttpContext.Current.Server.MapPath(strFilePath));
            StringBuilder sb = new StringBuilder();
            
            string output = "";
            string cmsTab = "";
            
            Umbraco.Core.Services.ContentService.Published += (sender, args) =>
            {
                int itemCount = 0;
                foreach (var item in args.PublishedEntities)
                {
                    
                    HttpContext.Current.Response.Write(itemCount);
                    //HttpContext.Current.Response.Write("**" + item.ToString() + "**");
                    if (System.Web.Configuration.WebConfigurationManager.AppSettings["SST:" + item.ContentType.Alias] != null)
                    {
                        string[] dataTypes;
                        int count = 0;
                        

                        dataTypes = System.Web.Configuration.WebConfigurationManager.AppSettings["SST:" + item.ContentType.Alias].Split(',');
                        //output += System.Web.Configuration.WebConfigurationManager.AppSettings["SST:" + item.ContentType.Alias];
                        if (dataTypes[0] != "")
                        {
                            foreach (string pt in dataTypes)
                            {

                                output += "property: " + pt + "<br />";
                                foreach (PropertyGroup ptg in item.PropertyGroups)
                                {
                                    output += "property: " + pt + " - prg: " + ptg.Name + "<br />";
                                    foreach (PropertyType propType in ptg.PropertyTypes)
                                    {

                                        //output += "tab: " + ptg.Name + ", propAlias: " + propType.Alias + ", propAlias From config: " + pt + "<br />";
                                        if (propType.Alias == pt)
                                        {
                                            XmlDocument xd = new XmlDocument();
                                            xd.LoadXml(item.GetValue(ptg.PropertyTypes.OrderBy(o => o.SortOrder).First().Alias).ToString());
                                            XmlNode theNode = xd.SelectSingleNode("//names");
                                            if (theNode != null)
                                            {
                                                cmsTab = theNode.InnerText.Replace(" ", "");
                                                break;
                                            }
                                        }

                                    }
                                    count++;
                                    //HttpContext.Current.Response.Write("Count: " + count + "<br />");
                                }
                                HttpContext.Current.Response.Write(output);

                                tempDoc.LoadXml(item.GetValue(pt).ToString());
                                XmlNode xCurrentTable = tempDoc.SelectSingleNode("//table");

                                XmlAttribute xTabName = tempDoc.CreateAttribute("data-tabname");
                                xTabName.Value = cmsTab;
                                xCurrentTable.Attributes.Append(xTabName);

                                sb.Append(DeSpanTables(xmlDoc, xCurrentTable));
                            }

                //            //To get the url of the content, need to use UmbracoHelper to instantiate an object of IPublishedContent
                //            //NOTE:: for some reason, this does not work on first publish.  Must publish twice

                            UmbracoHelper UMHelper = new UmbracoHelper(UmbracoContext.Current);
                            IPublishedContent publishedContent = UMHelper.TypedContent(item.Id);

                            //XmlDocument thePage = new XmlDocument();
                            //thePage.Load("~/AppData/umbraco.config");

                           
                            
                            
                            XmlNode newPage = xmlDoc.CreateNode(XmlNodeType.Element, "page", null);

                            XmlAttribute id = xmlDoc.CreateAttribute("nodeId");
                            id.Value = item.Id.ToString();

                            XmlAttribute url = xmlDoc.CreateAttribute("url");

                            url.Value = publishedContent.Url;

                            var newUrl = new UmbracoHelper(UmbracoContext.Current).NiceUrl(item.Id);

                            url.Value = newUrl;
                            newPage.Attributes.Append(id);
                            newPage.Attributes.Append(url);
                            newPage.InnerXml = sb.ToString();

                            XmlNode pageNode = xmlDoc.SelectSingleNode("//page[@nodeId ='" + item.Id.ToString() + "']");

                            if (pageNode != null)
                            {
                                xmlDoc.SelectSingleNode("tableIndexer").RemoveChild(pageNode);
                            }
                            xmlDoc.SelectSingleNode("tableIndexer").AppendChild(newPage);
                        }
                        xmlDoc.Save(HttpContext.Current.Server.MapPath(strFilePath));
                        
                    }
                    //HttpContext.Current.Response.Write("- <br />"); 
                    itemCount++;
                }
                
                //HttpContext.Current.Response.Write(output);
                //HttpContext.Current.Response.Write(output);
            };

            Umbraco.Core.Services.ContentService.UnPublished += (sender, args) =>
            {
                foreach (var item in args.PublishedEntities)
                {
                    XmlNode newPage = xmlDoc.CreateNode(XmlNodeType.Element, "page", null);
                    XmlNode pageNode = xmlDoc.SelectSingleNode("//page[@nodeId ='" + item.Id.ToString() + "']");

                    if (pageNode != null)
                    {
                        xmlDoc.SelectSingleNode("tableIndexer").RemoveChild(pageNode);
                    }
                }
                xmlDoc.Save(HttpContext.Current.Server.MapPath(strFilePath));
            };

            _ran = true;
                    }
                }
            }

        }


        private string strFilePath = "~/config/TableIndexer.config";
        
        StringBuilder sbPageTables = new StringBuilder();

        private void RemoveTableFileEventHandler(Umbraco.Core.Publishing.IPublishingStrategy sender, Umbraco.Core.Events.PublishEventArgs<Umbraco.Core.Models.IContent> e)
        {


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(HttpContext.Current.Server.MapPath(strFilePath));
            

            foreach (var item in e.PublishedEntities)
            {
                XmlNode newPage = xmlDoc.CreateNode(XmlNodeType.Element, "page", null);
                XmlNode pageNode = xmlDoc.SelectSingleNode("//page[@nodeId ='" + item.Id.ToString() + "']");

                if (pageNode != null)
                {
                    xmlDoc.SelectSingleNode("tableIndexer").RemoveChild(pageNode);
                }
            }
            xmlDoc.Save(HttpContext.Current.Server.MapPath(strFilePath));
        }

        private string DeSpanTables(XmlDocument xmlDoc, XmlNode table)
        {

            int colspan = 0;
            int rowspan = 0;
            int rowIndex = 0;
            int columnIndex = 0;
            int rowDuration = 0;
            bool needsShifted = false;
          
            XmlNodeList xNodeTRs = table.SelectNodes("tbody/tr");
            
            foreach (XmlNode xNodeTR in xNodeTRs)
            {
                if (xNodeTR.Name.ToLower() == "tr")         //loop through all the TRs.
                {
                    foreach (XmlNode xNodeTD in xNodeTR.ChildNodes)
                    {
                        if (xNodeTD.Name.ToLower() == "td")
                        {

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
                                colspan = Convert.ToInt32(xNodeTD.Attributes.GetNamedItem("colspan").Value);

                                if (xNodeTD.Attributes.GetNamedItem("rowspan") != null)
                                {
                                    
                                    rowspan = Convert.ToInt32(xNodeTD.Attributes.GetNamedItem("rowspan").Value);
                                    if (rowspan > 1)
                                    {
                                        int calibRowIndex = rowIndex + 1;
                                        int rowLimit = rowspan - 1;
                                        int currentRow = calibRowIndex;
                                        int nextRow = 0;
                                       
                                        for (int count = 1; count <= rowLimit; count++)
                                        {
                                            nextRow = currentRow + 1;
                                            XmlNode xRow = xNodeTR.ParentNode.SelectSingleNode("tr[position() = " + nextRow + "]");
                                            int insertAxis = Convert.ToInt32(xNodeTD.Attributes["axis"].Value) - 1;
                                            XmlNode placeToInsert = xRow.SelectSingleNode("td[@axis='" + insertAxis + "']");
                                            xNodeTD.Attributes["rowspan"].Value = "1";

                                            xRow.InsertAfter(xNodeTD.Clone(), placeToInsert);
                                            currentRow += 1;
                                        }
                                    }
                                }
                            }

                            //The xml string of the current <td>
                            string resultXml = xNodeTD.OuterXml;

                            //entity conversion
                            resultXml = resultXml.Replace("&#216;", "Ø");
                            resultXml = resultXml.Replace("<br />", "\n");

                            colspan = 0;
                            rowspan = 0;
                            columnIndex++;
                        }
                    }
                    rowIndex++;
                    columnIndex = 0;
                } //end if TR exists
            } //end for loop for TRs

            //HttpContext.Current.Response.Write(table.OuterXml);
            return table.OuterXml;
        }

    }
}
