

$(function(){

	$('a.edit-upload').click( function() {
		storeTable($(this));
	} );
	
	$('.styleTitle').change(function(){
		$theTable = $(this).closest('.control-wrap').find('table');
		$theTable.attr('class', $(this).val());
		
		$hiddenField = $(this).closest('.control-wrap').siblings('input');
		
		//console.log($hiddenField);
		//console.log($theTable.parent().html());
		$hiddenField.val($theTable.parent().html().replace(/<br>/g,'<br />'));
	});

});


function storeTable($link){
var $tableDiv = $link.closest('.control-wrap').find('.table-wrapper');
//console.log("the table: " + $tableDiv.html());
  $.ajax({
      type: "POST",
      async: false,
      url: "/Umbraco/plugins/SpreadsheetUploader/SessionTables.asmx/StoreTable",
      //data: '{"strTable":"' + encodeURI($table.wrap('<div>').parent().html().replace(/<br>/g,'<br />')) + '"}',
	  data: '{"strTable":"' + encodeURI($tableDiv.html().replace(/<br>/g,'<br />')) + '"}',
	  //data: '{"strTable":"' + encodeURI("<table id=""tblEditable""><thead></thead><tbody></tbody></table>") + '"}',
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (returnValue){
	 
        //$table.unwrap();
		
		
      }
  });
}


function updateTable(type, clientID, rValue){
	//console.log(rValue);
	if(rValue.outVal != null){
		
		
		$('#'+clientID).html(rValue.outVal);
		$('#body_'+clientID+'HiddenTableValue').val(rValue.outVal.replace(/<br>/g,'<br />'));
	}
}

function Remove(clientID) {
	
		
	
	$('#'+clientID).html("<table class='no-class'><thead></thead><tbody></tbody></table>");
	$('#body_'+clientID+'HiddenTableValue').val("<table class='no-class'><thead></thead><tbody></tbody></table>");
	
		/*$('#'+clientID+' ' + type).prepend("<tr></tr>");
		if(type == 'thead'){
			$('#'+clientID+' ' + type + ' tr:first-child').prepend("<td colspan='20' class='headSpan'>&nbsp;</td>");
		}
		else{
		$('#'+clientID+' ' + type + ' tr:first-child').prepend("<td colspan='20' class='bodySpan'><img src='/css/images/space.gif' width='1' height='1' /></td>");
		}
		$('#'+clientID+' ' + type + ' tr:first-child').append($lastTD);
		*/


}