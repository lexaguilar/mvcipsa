(function($) {
    $.fn.datatable = function (options) {
        // Establish our default settings
        var settings = $.extend({
            columnsHead: null,
            columnsDisplay: null,
            data: null,
            pag: 0,
            rows: 0,
            totalRows: 0,
            controller: null,            
            id: null,
            callback:'redirect'
        }, options);
        return this.each(function () {
            var table = $(this);
            var thead = '<thead><tr>';
            $.each(settings.columnsHead, function (i, e) {
                thead += '<th>' + e + '</th>';
            });
            thead += '</tr></thead>';
            var tbody = '<tbody>';
            $.each(settings.data, function (index, elem) {
                var value = '';
                $.each(settings.columnsDisplay, function (i, ed) {
                    if (ed.type) {
                        var buttons = '';
                        $.each(ed.buttons, function (i, btnAction) {
                            if (btnAction == 'edit') {
                                buttons += '<a href="#" class ="btn btn-primary btn-xs"  onclick="' + settings.callback+'(this);" data-controller="' + (settings.controller ? (settings.controller) : '') + '" data-action="edit"   data-currentid="' + elem[settings.id] + '"  style="" class="tip-top" data-original-title="Editar">  <i style="color:white" class="fa fa-pencil"></i></a> ';
                            }
                            if (btnAction == 'remove') {
                                buttons += '<a href="#" class ="btn btn-danger btn-xs"   onclick="' + settings.callback +'(this);" data-controller="' + (settings.controller ? (settings.controller) : '') + '" data-action="delete" data-currentid="' + elem[settings.id] + '"  style="" class="tip-top" data-original-title="Eliminar"><i style="color:white" class="fa fa-remove"></i></a> ';
                            }
                        });                       
                        value += '<td>' + buttons + '</td>';
                    }
                    else
                        value += '<td>' + elem[ed] + '</td>';
                });
                tbody += '<tr>' + value + '</tr>';
            });
            tbody += '</tbody>';
            $(table).html(thead + tbody);
        });
    }
}(jQuery));


function createPaging(nButtons, totalPages, btnActive) {
    if (totalPages > nButtons) {
        var btnBefore = 1;
        var btnAfter = nButtons;
        var nButtonTemp = Math.floor(nButtons / 2);
        if (btnActive > 2 && btnActive <= (totalPages - nButtonTemp)) {
            btnBefore = parseInt(btnActive) - nButtonTemp;
            btnAfter = parseInt(btnActive) + nButtonTemp;
        }
        if (btnActive > totalPages - nButtonTemp) {
            btnBefore = totalPages - nBotones + 1
            btnAfter = totalPages;
        }
        $('#paging').html('');
        $('#paging').append((1 != btnActive ? '<button type="button" class="btn" onclick="paginar(' + 1 + ')"><span class="fa fa-angle-double-left fa-color-black"></span></button>' : ''));
        $('#paging').append((1 != btnActive ? '<button type="button" class="btn" onclick="paginar(' + (parseInt(btnActive) - 1) + ')"><span class="fa fa-angle-left fa-color-black"></span></button>' : ''));
        for (var i = btnBefore; i <= btnAfter; i++) {
            $('#paging').append('<button type="button" class="btn ' + (i == btnActive ? " active" : "") + '" onclick="paginar(' + i + ')">' + i + ' </button>');
        }
        $('#paging').append((totalPages != btnActive ? '<button type="button" class="btn" onclick="paginar(' + (parseInt(btnActive) + 1) + ')"><span class="fa fa-angle-right fa-color-black"></span></button>' : ''));
        $('#paging').append((totalPages != btnActive ? '<button type="button"  class="btn" onclick="paginar(' + totalPages + ')"><span class="fa fa-angle-double-right fa-color-black"></button>' : ''));
    }
    else {
        $('#paging').html('');
        for (var i = 1; i <= totalPages; i++) {
            $('#paging').append('<button type="button" class="btn' + (i == btnActive ? " active" : "") + '" onclick="paginar(' + 6 + ')">' + i + ' </button>');
        }
    }
}


function redirect(elemt) {
    var controller = $(elemt).data('controller');
    var action = $(elemt).data('action');
    var currentId = $(elemt).data('currentid');
    window.location.href = pathBase + getURL([controller, action, currentId]);
}

var  getURL = (a) => a.join('/');



(function ($) {
    $.fn.addNewRow = function (t) {
        var tick = 'selector' + t;
        return this.each(function () {
            var table = $(this);
            $(table).find('tbody').append('<tr>' +
                '<td class="w10"><select class="select20 fuente" data-selector="' + tick + '"><option value="">Seleccione</option></select></td>' +                               
                '<td class="w25"><select class="select40 data-ajax" data-selector="' + tick + '"><option value="">Seleccione</option></select></td>' +    
                '<td class="w40"><a href="#" data-original-title="Ingrese el precio" class="editable dolares ' + tick + '" customData="' + tick + '">0.00</a></td>' +
                '<td class="w40"></td>' +
                '<td class="w40"><a href="#" data-original-title="Ingrese la cantidad" class="editable cantidad ' + tick + '" customData="' + tick + '">1</a></td>' +
                '<td class="w40"></td>' +
                '<td class="w40"></td>' +
                '<td><a class="fa fa-remove fa-color-red a-type-cursor" onclick="remove(this);"></a></td>' +
                '</tr>'
            );
        });
    }
}(jQuery));

function remove(e) {
    $(e).parent().parent().addClass('animated fadeOutDown');
    function removeRow() {
        $(e).parent().parent().remove();
    }
    setTimeout(removeRow, 500)    
}

function isNumber (n){return !isNaN(parseFloat(n)) && isFinite(n)}

var Monto = {
    hidden: function () {
        $('.monto').hide();
    },
    tipo1: function () {//efec
        Monto.hidden();
        $('.Montoefectivo').show();
    },
    tipo2: function(){ //cheque
        Monto.hidden();
        $('.Montocheque').show();
        Monto.showLable('No. cheque:');
    },
    tipo3: function () {//minuta
        Monto.hidden();
        $('.Montominuta').show();
        Monto.showLable();
    },
    tipo4: function () {//transf
        Monto.hidden();
        $('.Montotransferencia').show();
        Monto.showLable();
    },
    tipo5: function () {//efec + cheq
        Monto.hidden();
        $('.Montoefectivo').show();
        $('.Montocheque').show();
        Monto.showLable('No.cheque:');
    },
    tipo6: function () {//efec + transf
        Monto.hidden();
        $('.Montocheque').show();
        $('.Montotransferencia').show();
        Monto.showLable();
    },
    showLable: function (desc) {
        $('.lblNoreferencia').text(desc||'Id Referencia:'); 
        $('.Cuentabanco').show(); 
        $('.Noreferencia').show(); 
    }
}