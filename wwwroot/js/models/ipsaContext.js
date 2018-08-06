var baseModel ={
    entity : null,
    url : null,
    verb : 'post',
    onError : response => {
        $('.wrapper').loading('stop');
        context.ajax.failResult(response); 
    },
    onSuccess : function(){

    },
    save : function(){

    }
}