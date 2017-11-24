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
            callback: 'redirect',
            columnsWithFormatDate: [],
            columnsWithFormatMoney: [],
        }, options);
        return this.each(function () {
            var table = $(this);
            let rows = parseInt(settings.rows);
            let pag = parseInt(settings.pag);
            var thead = '<thead><tr>';
            $.each(settings.columnsHead, function (i, e) {
                thead += '<th>' + e + '</th>';
            });
            thead += '</tr></thead>';
            var tbody = '<tbody>';
   
            var value = '';
            if (!settings.data.length) {
                value = '<td colspan = "' + settings.columnsDisplay.length +'"><div class="alert alert-warning text-center" role="alert">No se encontraron registros</div><td>';
                tbody += '<tr>' + value + '</tr>';
            }

            $.each(settings.data, function (index, elem) {
                value = ''                
                $.each(settings.columnsDisplay, function (i, ed) {
                    if (ed.type) {
                        var buttons = '';
                        $.each(ed.buttons, function (i, btnAction) {
                            if (btnAction == 'edit') {
                                buttons += '<a href="#" class ="btn btn-primary btn-xs"  onclick="' + settings.callback + '(this);" data-controller="' + (settings.controller ? (settings.controller) : '') + '" data-action="edit"   data-currentid="' + elem[settings.id] + '"  style="" class="tip-top" data-original-title="Editar">  <i style="color:white" class="fa fa-pencil"></i></a> ';
                            }
                            if (btnAction == 'remove') {
                                buttons += '<a href="#" class ="btn btn-danger btn-xs"   onclick="' + settings.callback + '(this);" data-controller="' + (settings.controller ? (settings.controller) : '') + '" data-action="delete" data-currentid="' + elem[settings.id] + '"  style="" class="tip-top" data-original-title="Eliminar"><i style="color:white" class="fa fa-remove"></i></a> ';
                            }
                            if (btnAction == 'otherAction') {
                                buttons += '<a href="#" class ="btn btn-success btn-xs"   onclick="' + settings.callback + '(this);" data-controller="' + (settings.controller ? (settings.controller) : '') + '" data-action="EditRols" data-currentid="' + elem[settings.id] + '"  style="" class="tip-top" data-original-title="Eliminar"><i style="color:white" class="fa fa-list"></i></a> ';
                            }
                        });
                        value += '<td>' + buttons + '</td>';
                    }
                    else {
                        if (settings.columnsWithFormatMoney.includes(ed)) {
                            value += '<td>' + numeral(elem[ed]).format('$0,0.00') + '</td>'; 
                        }
                        else if (settings.columnsWithFormatDate.includes(ed)) {
                            value += '<td>' + moment(elem[ed]).format("DD-MM-YYYY") + '</td>';
                        }
                        else
                            value += '<td>' + elem[ed] + '</td>'; 
                    }
                                                
                });
                tbody += '<tr>' + value + '</tr>';
            });
            tbody += '</tbody>';

            var allRows = settings.totalRows;
            if (allRows % rows > 0)
                totalPages = Math.floor(allRows / rows) + 1;
            else
                totalPages = (allRows / rows);

            var tfoot = '';
            if (allRows > rows) {
                var RegistroInicio = (pag * rows) - (rows - 1);
                var RegistroHasta = (pag * rows);
                var info = '<p style="float:left;">Montrando de ' + RegistroInicio + ' a ' + (allRows < RegistroHasta ? allRows : RegistroHasta) + " de unos " + allRows + '</p>';

                tfoot += '<tfoot><tr><td colspan="' + settings.columnsHead.length + '">' + info + '<div class="btn-group" role="group" aria-label="..." style="float:right;">';
                tfoot += createPaging(5, totalPages, settings.pag);
            }
            tfoot += '</div></td></tr></tfoot>';


            $(table).html(thead + tbody + tfoot);
            
        });
    }
}(jQuery));

function createPaging(nButtons, Pages, btnAct) {
    var paging = '';
    if (Pages > nButtons) {
        var btnBefore = 1;
        var btnAfter = nButtons;
        var nButtonTemp = Math.floor(nButtons / 2);

        if (btnAct > 2 && btnAct <= (Pages - nButtonTemp)) {
            btnBefore = parseInt(btnAct) - nButtonTemp;
            btnAfter = parseInt(btnAct) + nButtonTemp;
        }
        if (btnAct > Pages - nButtonTemp) {
            btnBefore = Pages - nButtons + 1
            btnAfter = Pages;
        }

        paging += createLeftButton(i, btnAct, 'double-left');
        paging += createLeftButton(i, btnAct, 'left');
        for (var i = btnBefore; i <= btnAfter; i++) {
            paging += createButton(i, btnAct);
        }
        paging += createRightButton(i, btnAct, 'right', Pages);
        paging += createRightButton(i, btnAct, 'double-right', Pages);

    }
    else
        for (var i = 1; i <= Pages; i++)
            paging += createButton(i, btnAct);

    return paging;
}

function createLeftButton(i, btnAct, orientation) {
    var button = ''

    if (1 != btnAct)
        button = getBase()
            .replace('{act?}', '')
            .replace('{pag}', (orientation == 'double-left' ? 1 : (parseInt(btnAct) - 1)))
            .replace('{pag}', '<span class="fa fa-angle-' + orientation + ' fa-color-black"></span>');

    return button
}

function createRightButton(i, btnAct, orientation, pages) {
    var button = ''
    if (pages != btnAct)
        button = getBase()
            .replace('{act?}', '')
            .replace('{pag}', (orientation == 'double-right' ? pages : (parseInt(btnAct) + 1)))
            .replace('{pag}', '<span class="fa fa-angle-' + orientation + ' fa-color-black"></span>');

    return button
}

function createButton(i, btnAct) {
    return getBase()
        .replace('{act?}', (i == btnAct ? " active" : ""))
        .replace('{pag}', i)
        .replace('{pag}', i);
}

function getBase() {
    return '<button type="button" class="btn {act?}" onclick="init({pag})">{pag}</button>';;
}

function redirect(elemt) {
    var controller = $(elemt).data('controller');
    var action = $(elemt).data('action');
    var currentId = $(elemt).data('currentid');
    window.location.href = pathBase + getURL([controller, action, currentId]);
}

var  getURL = (a) => a.join('/');


(function ($) {
    $.fn.addNewRow = function (t,model) {
        var tick = 'selector' + t;
        return this.each(function () {
            var table = $(this);
            $(table).find('tbody').append('<tr>' +                         
                '<td class="w25"><select class="select40 data-ajax" data-selector="' + tick + '"><option value="">Seleccione</option></select></td>' +    
                '<td class="w40 text-center"><a data-name="precio" href="#" data-original-title="Ingrese el precio" class="editable precio ' + tick + '" customData="' + tick + '">' + (model ? model.Precio:"0.00") +'</a></td>' +   
                '<td class="w40"><a href="#" data-name="cantidad" data-original-title="Ingrese la cantidad" class="editable cantidad ' + tick + '" customData="' + tick + '">' + (model ? model.Cantidad : "1") +'</a></td>' +   
                '<td class="w40"><a href="#" data-name="montodolar" data-original-title="cantidad en dolares" class="editable dolares ' + tick + '" customData="' + tick + '">0.00</a></td>' +   
                '<td class="w40"><a href="#" data-name="montocordoba" data-original-title="Cantidad en cordobas" class="editable cordobas ' + tick + '" customData="' + tick + '">0.00</a></td>' +   
                '<td><a class="fa fa-remove fa-color-red a-type-cursor" onclick="remove(this);"></a></td>' +
                '</tr>'
            );
        });
    }
    $.fn.saving = function () {        
        return this.each(function () {
            var btn = $(this);
            $(btn).data('original', $(btn).html());
            $(btn).html('Guardando... <span class="fa fa-spinner fa-pulse"></span>');
        });
    }
    $.fn.searching = function () {
        return this.each(function () {
            var btn = $(this);
            $(btn).data('original', $(btn).html());
            $(btn).html('Buscando... <span class="fa fa-spinner fa-pulse"></span>');
        });
    }
    $.fn.reset = function () {      
        return this.each(function () {
            var btn = $(this);
            $(btn).html($(btn).data('original'));
        });
    }
}(jQuery));

function remove(e) {
    $(e).parent().parent().addClass('animated fadeOutDown');
    function removeRow() {
        $(e).parent().parent().remove();
        calculate();
    }
    setTimeout(removeRow, 500)    
}

function isNumber (n){return !isNaN(parseFloat(n)) && isFinite(n)}

var Monto = {
    total: function () {
        var _total = 0;
        $('.clear-monto').each(function (index, element) {
            _total += element.value;
        });  
        return _total;
    },
    hidden: function (change) {
        $('.monto').hide();
        $('.clear-monto').each(function (index, element) {
            if (!change) {
                element.value = 0;   
            }                    
        });          
    },
    tipo1: function (c) {//efec
        Monto.hidden(c);
        $('.Montoefectivo').show();
    },
    tipo2: function(c){ //cheque
        Monto.hidden(c);
        $('.Montocheque').show();
        Monto.showLable('No. cheque:');
    },
    tipo3: function (c) {//minuta
        Monto.hidden(c);
        $('.Montominuta').show();
        Monto.showLable();
    },
    tipo4: function (c) {//transf
        Monto.hidden(c);
        $('.Montotransferencia').show();
        Monto.showLable();
    },
    tipo5: function (c) {//efec + cheq
        Monto.hidden(c);
        $('.Montoefectivo').show();
        $('.Montocheque').show();
        Monto.showLable('No.cheque:');
    },
    tipo6: function (c) {//efec + minuta
        Monto.hidden(c);
        $('.Montoefectivo').show();
        $('.Montominuta').show();
        Monto.showLable();
    },
    showLable: function (desc) {
        $('.lblNoreferencia').text(desc||'Id Referencia:'); 
        $('.Cuentabanco').show(); 
        $('.Noreferencia').show(); 
    }
}

var findEntity = (entity) => {
    var NewEntity = {};
    $('.' + entity).each(function (index, element) {
        if (element.tagName == 'INPUT' || element.tagName == 'SELECT') {
            if (element.classList.contains('datepicker')) {
                NewEntity[$(element).attr('id')] = moment($(element).val(), 'DD-MM-YYYY').utc().format();
            } else
            NewEntity[$(element).attr('id')] = $(element).val();
        }    
    });  
    return NewEntity;
}

var fieldsIsValid = (c) => {
    var result = true;
    $('.' + c).each(function (index, element) {
        if (element.tagName == 'INPUT' || element.tagName == 'SELECT') {
            if (element.type !='hidden') {
                if (element.value == '' || element.value == '0' || element.value == 0 || element.value == null) {
                    var hasErrorMessage = $(this).data('error-message');
                    console.log(element.id);
                    console.log(hasErrorMessage);
                    if (hasErrorMessage) {
                        $.notification($(this).data('error-message'));
                        result = false;
                    }
                }
            }            
        }
    });    
    if (detalle.length==0) {
        $.notification('Debe registrar el menos un servicio');
        result = false;
    }
    return result;
}

var calculate = () => {
    detalle = [];
    $('#servicios tbody tr').each(function (index, element) {
        var cta_cuenta = $(this).find("td:eq(0)").find('select').val();
        
        if (cta_cuenta.length) {           
            let precio = $(this).find("td:eq(1)").find('a').editable('getValue').precio;
            let cantidad = $(this).find("td:eq(2)").find('a').editable('getValue').cantidad;
            let montodolar = precio * cantidad;
            let montocordoba = montodolar * tasaCambio;


            $(this).find("td:eq(3)").find('a').editable('setValue', montodolar, true);
            $(this).find("td:eq(4)").find('a').editable('setValue', montocordoba, true);
                              
            detalle.push({
                cta_cuenta,
                montocordoba,
                montodolar,
                precio,
                cantidad
            });           
        }        
    }); 
    var _montocordoba = detalle.reduce(function (a, b) {
        return (+a) + (+b.montocordoba);
    }, 0);
    var _montodolar = detalle.reduce(function (a, b) {
        return (+a) + (+b.montodolar);
    }, 0);

    $('.amounts li:eq(0)').html('<span class="pull-left"> Total Dólares : </span><strong>$ ' + numeral(_montodolar).format('$0,0.00') + '</strong>');
    $('.amounts li:eq(1)').html('<span class="pull-left">Total Córdobas : </span><strong>C$ ' + numeral(_montocordoba).format('$0,0.00')+'</strong>');    
   
}

var context = {
    ajax: {
        get: (url, data, callback) => {
            $.get(pathBase + url, data,callback,
                "json"
            );
        }
    }
}