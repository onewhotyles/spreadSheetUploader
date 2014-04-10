$(function () {
	

        //alternating row fills on Order Tables
        $(".OrderInfoData tr:even").addClass("AlternatingRowStyle");
        $(".OrderInfoData tr:odd").addClass("RowStyle");
		
        $(".SpecInfoData tr:even").addClass("AlternatingRowStyle");
        $(".SpecInfoData tr:odd").addClass("RowStyle");
		
		
        $('input[type="file"]').attr('size', '1');
		
		$('#tblEditTable').find('tr').each(function(){
			$("<td class='headerClicker'></td><td class='spacer'></td>").prependTo(this);
		});
		
		var $theTable = $('#tblEditTable');
		var $allTRs = $theTable.find('tr');
		var resetTRs = $allTRs.clone(true).appendTo("tbody");
		$allTRs.remove();
		
		
		$('#insertButton').click(function(){
						
			//save remaining value from editor to highlighted cell.
			$('.currently-editing').html($("#HtmlArea").val());
			
			//remove all unnecessary classes
			$('.currently-editing').removeClass('currently-editing');
			$('.defocus').removeClass('defocus');
			
			$('td.headerClicker').remove();
			$('td.spacer').remove();
			
			//find highlighted rows and move them under <thead>
			var $headerTRs = $theTable.find('tr.highlighted');
			
			if ($theTable.find("thead").length===0){ 
				$("<thead></thead>").prependTo($theTable);
			}
			
			var copy = $headerTRs.clone(true).appendTo("thead");
			$headerTRs.remove();
			
			
			
			

			if($theTable.find('div').length != 0){
				$theTable.find('div').each(function(){
					var $div = $(this);
					$div.parent().append("<br>" + $div.html());
					$div.remove();
				});
			}

			if($theTable.find('p').length != 0){
				$theTable.find('p').each(function(){
					var $paragraph = $(this);
					$paragraph.parent().append("<br>" + $paragraph.html());
					$paragraph.remove();
				});
			}



			//UmbClientMgr.closeModalWindow($theTable.html().replace("", ""));
			//console.log("test: " + $theTable.wrap('div').html());
			UmbClientMgr.closeModalWindow($('.table-result').html().replace("", ""));
			
		});
		
		$('#uFilePath').change(function(){ $('#btnUpload').click(); });
		
		
		
    });
	
	
	$(function () {
		var isMouseDown = false, isHighlighted;
		$("td.headerClicker").mousedown(function () {
			isMouseDown = true;
	  
			var $selectedTR = $(this).parent();
		
			if($selectedTR.hasClass("highlighted")){  //if it is already highlighted
		
				var largestRowSpanPrev = 1;
			
				$selectedTR.prevAll().children().each(function(i) { 
					if(this.rowSpan > largestRowSpanPrev){
						largestRowSpanPrev = this.rowSpan;
					}
				});
		
				var $prevTR = $(this).parent();
				for(var x = 1; x<largestRowSpanPrev;x++){
					$prevTR.prev().removeClass("highlighted");
					$prevTR = $prevTR.prev();
				}
			
				$selectedTR.removeClass("highlighted");
				$selectedTR.nextAll().removeClass("highlighted");
				isHighlighted = $(this).parent().hasClass("highlighted");
			}else{
			
				var largestRowSpan = 1;
				$selectedTR.children().each(function(i) {
					if(this.rowSpan > largestRowSpan){
						largestRowSpan = this.rowSpan;
					}
				});
		
				var $nextTR = $(this).parent();
				for(var x = 1; x<largestRowSpan;x++){
					$nextTR.next().addClass("highlighted");
					$nextTR = $nextTR.next();
				}
				
				$selectedTR.addClass("highlighted");
				$selectedTR.prevAll().addClass("highlighted");
				isHighlighted = $(this).parent().hasClass("highlighted");
			}
		
		return false; // prevent text selection
    });
   
   

	$(function() {

		$("#HtmlArea").htmlarea({

			// Override/Specify the Toolbar buttons to show
			toolbar: [
				["superScript", "subScript"],                  
				[{
					// This is how to add a completely custom Toolbar Button
					css: "custom_disk_button",
					text: "Undo",
					action: function(btn) {
						
						this.html($('.currently-editing').html());
						
					}
				}]
			],
		  
			// Specify a specific CSS file to use for the Editor
			css: "css/iframe.css",

			// Do something once the editor has finished loading
			loaded: function() {

				$('iframe').attr(' horizontalscrolling','no');
				$('iframe').attr(' verticalscrolling','no');
				$('iframe').attr(' scrolling', 'no');
				/*$.myControl = { jhtmlarea: this };
				$($.myControl.jhtmlarea.editor.body).keypress(function (e) {
					if ((e.keyCode || e.which) == 13) { //override ENTER KEY to always add a line break
						if ($.browser.msie) {
							$('#HtmlArea').htmlarea('pasteHTML', "<br>");
							return false;
						}
					}
					else {
						if (e.which != 8 && e.which != 0) { //8 is the backspace key
							var c = String.fromCharCode(e.which);
							if (e.which == 32) { //32 is the space key
								$('#HtmlArea').htmlarea('pasteHTML', '&nbsp;');
							}
							else {
								$('#HtmlArea').htmlarea('pasteHTML', c);
							}
							return false;
						} 
					}
				}); */
			}
		});
				
	});


	$("td:not(.headerClicker)").not(".spacer").click(function () {

		if($('#HtmlArea').val() != ''){
			
			//var strHtml = ('#HtmlArea').val();
			var accom = $('#HtmlArea').clone()
                .find('span').replaceWith(function() { return this.innerHTML; })
                .end().html(); 
				//console.log($('#HtmlArea').val());
			$('.currently-editing').html($("#HtmlArea").val());
			

			//$('.currently-editing').html($('.currently-editing').html().replace(/<\/?([a-z]+)[^>]*>/gi, function(match, tag) {
			//	return (tag.toLowerCase() === "sup" || tag.toLowerCase() === "sub" || tag.toLowerCase() === "br") ? match : "";
			//}));
			
		}
		
		
		$('.currently-editing').removeClass('currently-editing');
		$('.defocus').removeClass('defocus');
		
		var $cell = $(this);
		$cell.addClass('currently-editing');
		
		$('#tblEditTable td:not(.currently-editing)').each(function(){
			$(this).addClass('defocus');
		});
		
		$('#tblEditTable td.spacer').each(function(){
			$(this).removeClass('defocus');
		});
		
		if($cell.html() != ''){
			$("#HtmlArea").htmlarea('html', $cell.html());
		}
		else{
			$("#HtmlArea").htmlarea('html', '<body></body>');
		}
		setTimeout(setFocusThickboxIframe, 100);
		
		
		
		return false;

	});

	function setFocusThickboxIframe() {
		var iframe = $("iframe")[0];
		iframe.contentWindow.focus();

	}
	 
});


	function removeTextareas(){
		
		$('textarea').each(function(){
			var $td = $(this).parent();
			
			var inputValue = $(this).val();
			$(this).remove();
			//$td.html(inputValue.replace(/([^>\r\n]?)(\r\n|\n\r|\r|\n)/g, "<br />"));
			$td.html(inputValue.replace(/\n/, "<br />"));
			
			
		});
		
	}
