function updateList(clientID, rValue){

	if(rValue.outVal != null){
		
		$('#'+clientID).find('input').val()
		var $ul = $('#'+clientID).find('.textstring-row-set');
		buildXML($ul, rValue.outVal);
	}

};


$(function(){
	 $(".TextBoxListSettingsSaveBox").each(function(){
      buildXML($(this).parent().find('ul'));
	 });
	
	var indentPixels = 20;
	var minIndentLevel = 0;
	
	$('.textstring-row-field').each(function(){
			var $maxIndentLevel = $(this).parent().parent().attr("limit");
			var $row = $(this);
            var $input = $row.find('.umbEditorTextField');
            var $ul = $row.parent();
            var $li = $row;
            var indent = $row.attr("indent");
			
            var indentLevels = parseInt(indent, 10) || 0;
			
                for (i = 0; i < indentLevels; i++) {
                  
                    $(this).animate({

                        marginLeft: '+=' + indentPixels

                    }, 50);

                   $row.attr("indent", indentLevels);
				
                $input.css('width', '-=' + indentPixels);
                $li.css('width', '-=' + indentPixels);
                }
	}); 
	
	
	
	 //respond to title and blurb changes
  $('.umbEditorTextField').live('keyup', function(){
    buildXML($(this).parent().parent());
  });
	
	$('.add_row').click(function() {
	
		var $parent = $(this).parent();
            var $row = $parent.clone(true); // clone the row
			var $ul = $(this).parent().parent();
            var $input = $row.find('.umbEditorTextField');
			
			    // clear the text field
                $input.val('');

                // append the new row
                $row.insertAfter($parent);

                // set the focus
                $input.focus();

                // re-populate the hidden field
                buildXML($ul);
	});

	$('.delete_row').click(function() {
	var $ul = $(this).parent().parent();
		$inputs = $(this).parent().parent().parent().find('.umbEditorTextField');
		
	 // make sure the user wants to remove the row
            if (confirm('Are you sure you want to delete this row?')) {
				
                // check if this is the last row...
                if ($inputs.length == 1) {

                    // ... if so, just clear it.
                    $inputs.val('').focus();

                } else{

                    var $parent = $(this).parent();

                    // set the focus
                    $parent.prev().find('.umbEditorTextField').focus();

                    // remove the row
                    $parent.remove();
                }

                // re-populate the hidden field
               buildXML($ul);;
            }
	
	});
	
	$('.indent_row').click(function () {
		var $maxIndentLevel = $(this).parent().parent().parent().attr("limit");
            var $row = $(this);
            var $input = $row.parent().find('.umbEditorTextField');
            var $ul = $row.parent().parent();
            var $li = $row.parent();
            var indent = $row.parent().attr("indent");
			
            var indentLevels = parseInt(indent, 10) || 0;
			
            if (indent < $maxIndentLevel) {
                indentLevels += 1;
               
                $(this).parent().animate({

                    marginLeft: '+=' + indentPixels

                }, 50);
				$row.parent().attr("indent", indentLevels);
				
                $input.css('width', '-=' + indentPixels);
                $li.css('width', '-=' + indentPixels);
				
                buildXML($ul);
            }

            return false;
        });
	$('.unindent_row').click(function () {

            var $row = $(this);
            var $input = $row.parent().find('.umbEditorTextField');
            var $ul = $row.parent().parent();
            var $li = $row.parent();
            var $indent = $row.parent().attr("indent");
			
            var indentLevels = parseInt($indent, 10) || 0;
			
            if ($indent > minIndentLevel) {
                indentLevels -= 1;
				
				
                $indent -= 1;
                $(this).parent().animate({

                    marginLeft: '-=' + indentPixels

                }, 50);
				$row.parent().attr("indent", indentLevels);			
                $input.css('width', '+=' + indentPixels);
                $li.css('width', '+=' + indentPixels);
              
                buildXML($ul);
            }

            return false;
        });
	$('.clear-button').click(function(){
		if (confirm('Are you sure you want to clear the entire list? ')) {
      var $saveBox=$(this).siblings('.TextBoxListSettingsSaveBox');
			$saveBox.val('<list><item indent="0"></item></list>');
      $saveBox.change();
			__doPostBack();
		}
	});	
		
	$('.textstring-row-set').sortable({
			handle: ".textstring-row-sort-image",
			cursor: 'move',
			helper: fixHelper,
			update: function(){buildXML($(this).parent().parent());},
			start : function(e, ui){ui.placeholder.html('<div>Insert Here</div>')},
			placeholder: 'sort-placeholder'
  });
		
		  //supposed to help sortable widths 
  function fixHelper(e, ui){
    ui.children().each(function(){
      $(this).width($(this).width());
    });
    return ui;
  };
			
				
		
});


function buildXML($ul, addNew){
		var $saveBox = $ul.parent().parent().find('.TextBoxListSettingsSaveBox'); 
		
		var xml = "";
		xml+='<list>';        
        $ul.find('.textstring-row-field').each(function(){
          var $thisListItem=$(this);
          
		  $thisListItem.find('.umbEditorTextField').val($thisListItem.find('.umbEditorTextField').val().replace("\u2022","").replace("\u25AA","").replace("\u25CB","").replace(/^\s+/,""));
		  
          xml+='<item indent="' + $thisListItem.attr("indent") + '">';
          xml += htmlEntities($thisListItem.find('.umbEditorTextField').val());
		  xml+='</item>';
        });
		
		if(addNew){
			var $lis = $ul.find('li');
			if($lis.length < 2){
				if($lis.find('input').val() == ""){
					xml = '<list>';
				}
			}
			xml += addNew;
		}
        xml+='</list>';
		$saveBox.val(xml);
		
    $saveBox.change();
    
		if(addNew){
			__doPostBack();
		}
	};	
	
	function htmlEntities(string) {
    return String(string).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
  }

