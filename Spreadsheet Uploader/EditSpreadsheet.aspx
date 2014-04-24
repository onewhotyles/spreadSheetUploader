<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditSpreadsheet.aspx.cs" Inherits="Spreadsheet_Uploader.EditSpreadsheet" %>
<%@ Register TagPrefix="umb" Namespace="ClientDependency.Core.Controls" Assembly="ClientDependency.Core" %>
<%@ Register TagPrefix="UmbracoControls" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register TagPrefix="UmbracoControls" Namespace="umbraco.controls" Assembly="umbraco" %>
<%@ Import Namespace="umbraco.IO" %>
<html>
<head>
<link rel="stylesheet" type="text/css" href="<%=GlobalVariables.datatypePath.ToString(); %>"/css/SpreadsheetUploaderBase.css" />
<link rel="stylesheet" type="text/css" href="<%=GlobalVariables.datatypePath.ToString(); %>"/css/SpreadsheetUploader.css" />
<link rel="stylesheet" type="text/css" href="<%=GlobalVariables.datatypePath.ToString(); %>"/css/SpreadsheetUploaderModal.css" />
  <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7/jquery.min.js"></script>

  
  
  
<script type="text/javascript" src="/umbraco_client/Application/NamespaceManager.js" ></script>
<script type="text/javascript" src="/umbraco_client/Application/HistoryManager.js" ></script>
<script type="text/javascript" src="/umbraco_client/ui/jquery.js" ></script>
<script type="text/javascript" src="/umbraco_client/Tree/UmbracoTree.js" ></script>
<script src="http://code.jquery.com/ui/1.10.2/jquery-ui.js"></script>

  
<script type="text/javascript" src="/umbraco_client/Application/UmbracoClientManager.js"></script>
<script type="text/javascript" src="/umbraco/plugins/SpreadsheetUploader/js/selectTRs.js"></script>

<script type="text/javascript" src="<%=GlobalVariables.datatypePath %>"/jHtmlArea/scripts/jquery-1.3.2.js"></script>
    <script type="text/javascript" src="<%=GlobalVariables.datatypePath %>"/jHtmlArea/scripts/jquery-ui-1.7.2.custom.min.js"></script>
    <link rel="Stylesheet" type="text/css" href="<%GlobalVariables.datatypePath %>"/jHtmlArea/style/jqueryui/ui-lightness/jquery-ui-1.7.2.custom.css" />

    <script type="text/javascript" src="<%=GlobalVariables.datatypePath %>"/jHtmlArea/scripts/jHtmlArea-0.7.5.js"></script>
    <link rel="Stylesheet" type="text/css" href="<%=GlobalVariables.datatypePath %>"/jHtmlArea/style/jHtmlArea.css" />
    

	
	
    <style type="text/css">
        /* body { background: #ccc;} */
        div.jHtmlArea .ToolBar ul li a.custom_disk_button
        {
            background: url(images/disk.png) no-repeat;
            background-position: 0 0;
        }
        
        
    </style>


</head>
<body>

<script type="text/javascript">
       
    </script>



  

<form id="form1" runat="server">








<div class="HtmlAreaWrap">
    <textarea id="HtmlArea"></textarea>
</div>
	<div class="TopControls">					
		<div class="uploadWrap">	
			<span>Upload new spreadsheet:</span>
			<asp:FileUpload ID="uFilePath" CssClass="filePath" runat="server" ></asp:FileUpload>
			<asp:Label ID="lblMessage" CSSClas="error" runat="server">You must choose a .xls file to upload</asp:Label>
		</div>
	</div>

	
	<div class="resultsPane">
	<div class="header-instructions">	<img src="css/selectHeaderText.png" /></div>

        <div class="table-result" cellspacing="0" cellpadding="2">
           
		   
				
                <asp:Literal runat="server" ID="ltrlCurrentSaved"><span class='preview'>PREVIEW</span></asp:Literal>
				
			
        </div>
	</div>
	<asp:Button ID="btnUpload" CSSClass="hidden" runat="server" Text="Upload" onclick="btnUpload_Click" />
         
		 
	<div class="bottomControls"> 
		 <input type="button" id="insertButton" value="Insert Into Page" runat="server"/>
        <em> or </em><a href="#" style="color: blue" onClick="UmbClientMgr.closeModalWindow()">cancel</a> 
	</div>
    	<asp:HiddenField runat="server" id="hfBoldClass"/>
<asp:HiddenField runat="server" id="hfFilePath"/>
<asp:HiddenField runat="server" id="hfStyle"/>

</form>
</body>
</html>