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
using Umbraco.Core.Publishing;
using Umbraco.Core.Cache;

namespace spreadsheet_Uploader
{
    public class SearchTable : ApplicationEventHandler
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlDocument tempDoc = new XmlDocument();
        string strNodePath = "";
        XmlNode newPage;

        public void OnApplicationInitialized(UmbracoApplication httpApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarting(UmbracoApplication httpApplication, ApplicationContext applicationContext)
        {
        }
       
          
        
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(strFilePath)))
            {
                StreamWriter sw = new StreamWriter(HttpContext.Current.Server.MapPath(strFilePath), true);
                sw.Write("<?xml version='1.0' encoding='utf-8'?><tableIndexer></tableIndexer>");
                sw.Close();
            }

            xmlDoc.Load(HttpContext.Current.Server.MapPath(strFilePath));
            StringBuilder sb = new StringBuilder();

            string output = "";
            string cmsTab = "";
        
            Umbraco.Core.Services.ContentService.Published += (sender, args) =>
            {
                int itemCount = 0;
                foreach (var item in args.PublishedEntities)
                {

                    
                    UmbracoHelper UMHelper = new UmbracoHelper(UmbracoContext.Current);
                    IPublishedContent publishedContent = UMHelper.TypedContent(item.Id);

                    //If current Item is a product page
                    if (System.Web.Configuration.WebConfigurationManager.AppSettings["SST:" + item.ContentType.Alias] != null)
                    {
                        
                        newPage = xmlDoc.CreateNode(XmlNodeType.Element, "page", null);

                        XmlAttribute id = xmlDoc.CreateAttribute("nodeId");
                        XmlAttribute pageName = xmlDoc.CreateAttribute("pageName");

                        pageName.Value = publishedContent.Name;
                        id.Value = item.Id.ToString();


                        XmlAttribute url = xmlDoc.CreateAttribute("url");
                        umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 9999999, "hey");
                        
                        //var newUrl = new UmbracoHelper(UmbracoContext.Current).NiceUrl(item.Id);
                        //umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 9999999, "Page: " + newUrl);

                        //var newUrl = publishedContent.Url;
                        var newUrl = "";
                        //url.Value = newUrl;
                        newPage.Attributes.Append(id);
                        newPage.Attributes.Append(url);
                        newPage.Attributes.Append(pageName);

                        //HttpContext.Current.Response.Write(itemCount);
                         //HttpContext.Current.Response.Write("**" + item.Name + "**");
                        output += item.Name + "<br/>";
                        /*using (StreamWriter _testData = new StreamWriter(HttpContext.Current.Server.MapPath("~/data.txt"), true))
                        {
                            _testData.WriteLine(item.Name); // Write the file.
                        }*/
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

                                    //output += "property: " + pt + "<br />";
                                    foreach (PropertyGroup ptg in item.PropertyGroups)
                                    {
                                        //output += "property: " + pt + " - prg: " + ptg.Name + "<br />";
                                        foreach (PropertyType propType in ptg.PropertyTypes)
                                        {

                                            //output += "tab: " + ptg.Name + ", propAlias: " + propType.Alias + ", propAlias From config: " + pt + "<br />";
                                            if (propType.Alias == pt)
                                            {
                                                //used for sites like franklinagua that use a dropdown for tab names
                                                //XmlDocument xd = new XmlDocument();
                                                //xd.LoadXml(item.GetValue(ptg.PropertyTypes.OrderBy(o => o.SortOrder).First().Alias).ToString());
                                                //XmlNode theNode = xd.SelectSingleNode("//names");
                                                //if (theNode != null)
                                                //{
                                                //    cmsTab = theNode.InnerText.Replace(" ", "");
                                                //    break;
                                                //}

                                                cmsTab = ptg.Name.Replace(" ", "");
                                            }

                                        }
                                        count++;
                                        //HttpContext.Current.Response.Write("Count: " + count + "<br />");
                                    }
                                    //HttpContext.Current.Response.Write(output);


                                            //Folowing code was replaced so that we can grab the title of the table
                                    //tempDoc.LoadXml(item.GetValue(pt).ToString());
                                    //XmlNodeList xCurrentTable = tempDoc.SelectNodes("//table");

                                    //XmlAttribute xTabName = tempDoc.CreateAttribute("data-tabname");
                                    //XmlAttribute xTableName = tempDoc.CreateAttribute("data-tablename");

                                    //foreach (XmlNode xTable in xCurrentTable)
                                    //{
                                    //    xTabName.Value = cmsTab;
                                    //    xTable.Attributes.Append(xTabName);
                                    //    XmlNode xtn = xTable.SelectSingleNode("../../title");
                                    //    xTableName.Value = xtn.InnerText;
                                    //    xTable.Attributes.Append(xTableName);

                                    //    sb.Append(DeSpanTables(xmlDoc, xTable));

                                    //}

                                    

                                    tempDoc.LoadXml(item.GetValue(pt).ToString());
                                    XmlNodeList xCurrentWidget = tempDoc.SelectNodes("//widget");
                                  
                                    XmlAttribute xTabName = tempDoc.CreateAttribute("data-tabname");
                                    XmlAttribute xTableName = tempDoc.CreateAttribute("data-tablename");

                                    foreach (XmlNode xWidget in xCurrentWidget)
                                    {
                                        umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 9999999, "AT 165");
                                        XmlNode xTable = xWidget.SelectSingleNode("spreadsheet/table");
                                       // HttpContext.Current.Response.Write("<p>widget</p>" + xTable.OuterXml);
                                        XmlNode xtn = xWidget.SelectSingleNode("title");
                                        xTabName.Value = cmsTab;
                                        xTable.Attributes.Append(xTabName);

                                        xTableName.Value = xtn.InnerText;
                                        xTable.Attributes.Append(xTableName);
                                        umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 9999999, "AT 174: ");
                                        var thePathParam = "";
                                        if (publishedContent.Path != null)
                                        {
                                            thePathParam = publishedContent.Path;
                                        }
                                        sb.Append(DeSpanTables(xmlDoc, xTable, item.Id.ToString(), thePathParam, cmsTab));

                                    }
                                }

                                //            //To get the url of the content, need to use UmbracoHelper to instantiate an object of IPublishedContent
                                //            //NOTE:: for some reason, this does not work on first publish.  Must publish twice

                                //UmbracoHelper UMHelper = new UmbracoHelper(UmbracoContext.Current);
                                //IPublishedContent publishedContent = UMHelper.TypedContent(item.Id);

                                //XmlDocument thePage = new XmlDocument();
                                //thePage.Load("~/AppData/umbraco.config");

                                //XmlNode newPage = xmlDoc.CreateNode(XmlNodeType.Element, "page", null);

                                //XmlAttribute id = xmlDoc.CreateAttribute("nodeId");
                                //id.Value = item.Id.ToString();
                                //newPage.Attributes.Append(id);

                                //using (StreamWriter _testData = new StreamWriter(HttpContext.Current.Server.MapPath("~/data.txt"), true))
                                //{
                                //    _testData.WriteLine(sb.ToString()); // Write the file.
                                //}
                                newPage.InnerXml = sb.ToString();
                                //try
                                //{
                                //    XmlAttribute url = xmlDoc.CreateAttribute("url");
                                //    url.Value = publishedContent.Url;
                                //    var newUrl = new UmbracoHelper(UmbracoContext.Current).NiceUrl(item.Id);
                                //    url.Value = newUrl;
                                //    newPage.Attributes.Append(url);
                                //}
                                //catch
                                //{

                                //}
                                umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 9999999, "AT 212");
                                XmlNode pageNode = xmlDoc.SelectSingleNode("//page[@nodeId ='" + item.Id.ToString() + "']");

                                if (pageNode != null)
                                {
                                    xmlDoc.SelectSingleNode("tableIndexer").RemoveChild(pageNode);
                                }
                                umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 9999999, "AT 219");
                                xmlDoc.SelectSingleNode("tableIndexer").AppendChild(newPage);
                            }
                            xmlDoc.Save(HttpContext.Current.Server.MapPath(strFilePath));
                            umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 9999999, "AT 222");
                        }
                        //HttpContext.Current.Response.Write("- <br />"); 
                        itemCount++;


                    }
                    sb.Clear();

                    
                }


                 

            };

            Umbraco.Core.Services.ContentService.Moved += (sender, args) =>
                    {
               
                   


                    if (System.Web.Configuration.WebConfigurationManager.AppSettings["SST:" + args.Entity.ContentType.Alias] != null)
                    {
                        XmlNode pageNode = xmlDoc.SelectSingleNode("//page[@nodeId ='" + args.Entity.Id.ToString() + "']");

                       
                            UmbracoHelper UMHelper = new UmbracoHelper(UmbracoContext.Current);
                            IPublishedContent publishedContent = UMHelper.TypedContent(args.Entity.Id);

                            pageNode.Attributes["url"].Value = publishedContent.Url;
                            xmlDoc.Save(HttpContext.Current.Server.MapPath(strFilePath));
                    }
                };

            PublishingStrategy.Published += PublishingStrategy_Published;
            PublishingStrategy.UnPublished += PublishingStrategy_Unpublished;
            

        }


        void PublishingStrategy_Published(IPublishingStrategy sender, Umbraco.Core.Events.PublishEventArgs<Umbraco.Core.Models.IContent> e)
        {
            //foreach (var item in e.PublishedEntities)
            //{
            //    if (System.Web.Configuration.WebConfigurationManager.AppSettings["SST:" + item.ContentType.Alias] != null)
            //    {
            //        XmlNode pageNode = xmlDoc.SelectSingleNode("//page[@nodeId ='" + item.Id.ToString() + "']");

            //        if (pageNode.Attributes["url"].Value == "#")
            //        {
            //            UmbracoHelper UMHelper = new UmbracoHelper(UmbracoContext.Current);
            //            IPublishedContent publishedContent = UMHelper.TypedContent(item.Id);

            //            pageNode.Attributes["url"].Value = publishedContent.Url;
            //        }
            //    }
            //}
        }

        void PublishingStrategy_Unpublished(IPublishingStrategy sender, Umbraco.Core.Events.PublishEventArgs<Umbraco.Core.Models.IContent> e)
        {
            xmlDoc.Load(HttpContext.Current.Server.MapPath(strFilePath));
            StringBuilder sb = new StringBuilder();

            foreach (var item in e.PublishedEntities)
            {
                // HttpContext.Current.Response.Write("outside if");
                XmlNode pageNode = xmlDoc.SelectSingleNode("//page[@nodeId ='" + item.Id.ToString() + "']");

                if (pageNode != null)
                {
                    xmlDoc.SelectSingleNode("tableIndexer").RemoveChild(pageNode);
                }
            }
            xmlDoc.Save(HttpContext.Current.Server.MapPath(strFilePath));

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

        private string DeSpanTables(XmlDocument xmlDoc, XmlNode table, string nodeID, string nodePath, string tabName)
        {
            umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 9999999, "AT 331");
            int colspan = 0;
            int rowspan = 0;
            int rowIndex = 0;
            int columnIndex = 0;
            int rowDuration = 0;
            bool needsShifted = false;

            XmlNodeList xNodeTRs = table.SelectNodes("tbody/tr");
            umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 9999999, "AT 340");
            foreach (XmlNode xNodeTR in xNodeTRs)
            {
                umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 9999999, "AT 343");
                if (xNodeTR.Name.ToLower() == "tr")         //loop through all the TRs.
                {
                    XmlAttribute xNodeID = tempDoc.CreateAttribute("data-nodeID");
                    XmlAttribute xNodePath = tempDoc.CreateAttribute("data-nodePath");
                    XmlAttribute xTabName = tempDoc.CreateAttribute("data-tabName");
                    umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 9999999, "AT 348");
                    xNodeID.Value = nodeID;
                    xNodePath.Value = nodePath;
                    xTabName.Value = tabName;
                    umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Custom, 9999999, "AT 352");
                    xNodeTR.Attributes.Append(xNodeID);
                    xNodeTR.Attributes.Append(xNodePath);
                    xNodeTR.Attributes.Append(xTabName);

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
