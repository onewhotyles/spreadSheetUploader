<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportList.aspx.cs" Inherits="TextBoxList.ImportList" %>
<%@ Register TagPrefix="umb" Namespace="ClientDependency.Core.Controls" Assembly="ClientDependency.Core" %>
<%@ Register TagPrefix="UmbracoControls" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register TagPrefix="UmbracoControls" Namespace="umbraco.controls" Assembly="umbraco" %>
<%@ Import Namespace="umbraco.IO" %>
<html>
<head>

  <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7/jquery.min.js"></script>
 
 
 
<script type="text/javascript" src="/umbraco_client/Application/NamespaceManager.js" ></script>
<script type="text/javascript" src="/umbraco_client/Application/HistoryManager.js" ></script>
<script type="text/javascript" src="/umbraco_client/ui/jquery.js" ></script>
<script type="text/javascript" src="/umbraco_client/Tree/UmbracoTree.js" ></script>
  
<script type="text/javascript" src="/umbraco_client/Application/UmbracoClientManager.js"></script>
<script type="text/javascript">
function htmlEntities(string) {
    return String(string).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
  }
  
$(function(){	
	$('#TB_1').live('keyup', function(){
		var $saveBox = $('#TB_Values'); 
		var xml = "";
		var values = $('#TB_1').val().split("\n");
	
		for(count = 0; count < values.length; count++){
			if(values[count] != ""){
			xml += '<item indent="0">';
			xml += htmlEntities(values[count]);
			xml += '</item>';
			}
		}
		$saveBox.val(xml);
	});


	$('#insertButton').click(function(){
	
			UmbClientMgr.closeModalWindow($('#TB_Values').val());
			
	});
		
});

	


</script>
	<style type="text/css">
	*{
		margin:0px;
		padding:0px;
		font-family: Trebuchet MS, Lucida Grande, verdana, arial;
		
	}
	#TB_1{
		width:417px;
		height:430px;
	}
		#TB_Values{
			display:none;
		}
	
		h1{
			font-size: 18px;
			font-weight: bold;
			
		}
		p{
			font-size: 12px;
	
		}
	div{
		border:1px solid #ffffff;
	}
	.import-wrapper{
		width:417px;
		margin:10px auto 0 auto;

		

	}
	
	.bottomControls{
		margin-top:10px;
		height:40px;
	
	}
	
	</style>
</head>
<body>
    <form id="form1" runat="server">
		<div class="import-wrapper">
			<h1>Import List</h1>
			<p>Enter a list of items into the box below. <br />Each row of text will be one item in the list.</p>
			<asp:TextBox ID="TB_Values" runat="server"></asp:TextBox>
			<br />
			<asp:TextBox ID="TB_1" runat="server" TextMode="MultiLine"></asp:TextBox>
			<br />
			<div class="bottomControls"> 
				<input type="button" ID="insertButton" value="Insert Into Page" runat="server" />
				<em> or </em><a href="#" style="color: blue" onClick="UmbClientMgr.closeModalWindow()">cancel</a> 
			</div>
		</div>
    </form>
</body>
</html>