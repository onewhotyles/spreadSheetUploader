$(function(){
    
    //ini
    buildJson();
    
    $(".mediaNodes").keyup(function(){
        buildJson();
    });
    
    function buildJson(){
        //console.log('building...');
        
        var $saveBox=$(".saveBox");
        
        var json='';
        
        json+="{";
        json+="'mediaIDs' : '"+$(".mediaNodes").val()+"'";
        json+="}";
        
        $saveBox.val(json);
    }
});