$(function(){

  var $limitBox = $('.indLimitBox');
  
  //ini
  buildJSON();

  $limitBox.change(function(){

    buildJSON();
  });
  
  function buildJSON(){
   
    var json="";
    json+='{"indentedLimit":"' + $limitBox.val() + '"}';
    $('.indentedListSaveBox').val(json);
  }
});