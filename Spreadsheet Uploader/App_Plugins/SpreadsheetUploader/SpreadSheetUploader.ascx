<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpreadSheetUploader.ascx.cs" Inherits="Spreadsheet_Uploader.SpreadSheetUploader" %>

<%@ Import Namespace="umbraco.IO" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="DigibizTree" %>


<script type="text/javascript" src="/umbraco_client/Application/UmbracoClientManager.js" />

<script type="text/javascript">


	function back(strId){
		//alert('hey there');
	}
</script>



    <div>
        <a href="javascript:UmbClientMgr.openModalWindow('plugins/SpreadSheetUploader/EditSpreadsheet.aspx?dt=<%=this.ClientID%>', 'Selected Spreadsheet', true, 785, 800,'','','',alert('hey'); );">Edit...</a>
    </div>
	
	<link rel="stylesheet" type="text/css" href="/css/excelUpload.css" />




<div class="resultsPane">


        <table class="OrderInfo" cellspacing="0" cellpadding="2">
            <thead>
                <tr style="">
                    
                    <td colspan="20" style="background-color:#859db4; border:none;">&nbsp;</td>

                    <td class="headerControls" style="padding:3px; border-left:none; border-right:none; border-bottom:none;" rowspan="4">
					<div style="text-align:right; white-space:normal;">
						

						
					</div>
                    </td>
				</tr>
				
                <asp:Literal runat="server" ID="ltrlCurrentSavedHeader"></asp:Literal>
				
			<tr>	
				<td colspan="20" style="border:none; background-color:#999999; padding:0;margin:0;"><img src="/css/images/space.gif" width="1" height="1" /></td>
			</tr>	
			<tr>	
				<td colspan="20" style="border:none; background-color:#f2f9f6; padding:0;margin:0;"><img src="/css/images/space.gif" width="1" height="1" /></td>
			</tr>				
            </thead>
			
            <tbody>

				
                <tr style="border:none;">
                    
                    <td colspan="20" style="background-color:#ccc; border-bottom:none; border-right:0px; border-left:none; border-top: 5px solid #f2f9f6; padding:0;margin:0;"><img src="/css/images/space.gif" width="1" height="1" /></td>
                    <td class="bodyControls" style="padding:3px;border-left:none; border-right:none; border-bottom:none;"  rowspan="1000">
					
					<div style="text-align:right; white-space:normal;"">
						
						
					</div>
					
						
                    </td>
                    
                </tr>
              
                <asp:Literal runat="server" ID="ltrlCurrentSavedBody"></asp:Literal>
            
            </tbody>
        </table>

        

</div>
    
