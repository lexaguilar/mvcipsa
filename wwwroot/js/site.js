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
                                buttons += '<a href="#" class ="btn btn-danger btn-xs"   onclick="' + settings.callback + '(this);" data-controller="' + (settings.controller ? (settings.controller) : '') + '" data-action="EditRols" data-currentid="' + elem[settings.id] + '"  style="" class="tip-top" data-original-title="Eliminar"><i style="color:white" class="fa fa-list"></i></a> ';
                            }
                            if (btnAction == 'anular') {
                                buttons += '<a href="#" class ="btn btn-danger btn-xs"   onclick="anular(this);" data-controller="' + (settings.controller ? (settings.controller) : '') + '" data-action="EditRols" data-currentid="' + elem[settings.id] + '"  style="" class="tip-top" data-original-title="Anular"><i style="color:white" class="fa fa-remove"></i></a> ';
                            }
                        });
                        value += '<td>' + buttons + '</td>';
                    }
                    else
                    {
                        if (settings.columnsWithFormatMoney.includes(ed)) {
                            value += '<td>' + numeral(elem[ed]).format('$0,0.00') + '</td>';
                        }
                        else if (settings.columnsWithFormatDate.includes(ed)) {
                            value += '<td>' + moment(elem[ed]).format("DD-MM-YYYY") + '</td>';
                        }
                        else {
                            var _class = '';
                            if (ed == 'estado') {
                                if (elem[ed] == 'Registrado') {
                                    _class = 'text-primary';
                                }
                                if (elem[ed] == 'Anulado') {
                                    _class = 'text-danger';
                                }
                            } 
                            if (ed == 'numRecibo') {                         
                                _class = 'text-bold';                                
                            } 
                            value += '<td class="' + _class +'">' + elem[ed] + '</td>';
                        }
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
    $.fn.addNewReference = function (referencia) {      
        
        return this.each(function () {
            var d = new Date();
            var t = d.getTime();
            
            var table = $(this);
            $(table).find('tbody').append('<tr>' +               
                '<td id-source='+ t +'><input type="text" placeholder="dd-mm-aaaa" class="datepicker selectorFecha" value="' + (referencia ? moment(referencia.Fecha).format('DD-MM-YYYY')  : '')+'"/></td>' +
                '<td><a data-value="' + (referencia ? referencia.TipoPagoId : '') + '" class="teditable2 tipo_pago_id" href="#" data-type="select" data-name="tipo_pago_id" data-pk="1" data-title="Tipo de pago"></a></td>' +
                '<td><a data-value="' + (referencia ? referencia.IdBanco : '') + '" class="editable2 id_banco" href="#" data-type="select" data-name="id_banco" data-pk="1" data-title="Banco"></a></td>' +
                '<td><a data-value="' + (referencia ? referencia.Referencia : '') + '" class="editable2 selectorReferencia" href="#" data-name="referencia" data-original-title="Ingrese la referencia"></a></td>' + 
                '<td><a data-value="' + (referencia ? referencia.MontoEfectivo : '0.00') + '" class="editable2 monto_efectivo" href="#" data-name="monto_efectivo" data-original-title="Monto efectivo">0.00</a></td>' +              
                '<td><a data-value="' + (referencia ? referencia.MontoCheq : '0.00') + '" class="editable2 monto_cheq" href="#" data-name="monto_cheq" data-original-title="Monto del cheque">0.00</a></td>' +
                '<td><a data-value="' + (referencia ? referencia.MontoMinu : '0.00') + '" class="editable2 monto_minu" href="#" data-name="monto_minu" data-original-title="Monto de la minuta">0.00</a></td>' +
                '<td><a data-value="' + (referencia ? referencia.MontoTrans : '0.00') + '" class="editable2 monto_trans" href="#" data-name="monto_trans" data-original-title="Monto de la transferencia">0.00</a></td>' +                
                '<td>' + (referencia ? referencia.TipoCambio : '0.00') + '</td>' +        
                '<td>0.00</td>' +        
                '<td>0.00</td>' +        
                '<td><a class="fa fa-remove fa-color-red a-type-cursor" onclick="removeReferencia(this);"></a></td>' +
                '</tr>'
            );

            $('.tipo_pago_id').editable({   
                emptytext: 'Seleccione',               
                source: [
                    { value: 1, text: 'Efectivo' },
                    { value: 2, text: 'Cheque' },
                    { value: 3, text: 'Minuta' },
                    { value: 4, text: 'Transferencia' }
                ],
                validate: function (value) {
                    if ($.trim(value) == '')
                        return 'El valor es requerido';   
                    var result = validateTasa(this);
                    if (result) return result;
                },
            });

            $('.id_banco').editable({
                emptytext : 'Seleccione',             
                source: [
                    { value: 12, text: 'BANPRO' },
                    { value: 22, text: 'BANCO DE FINANZAS' },
                    { value: 1, text: 'B.A.C.' },
                    { value: 4, text: 'BANCENTRO' },
                    { value: 36, text: 'FICOSA' },
                    { value: 43, text: 'MINISTERIO DE HACIENDA Y CREDITO PUBLICO' }
                ],
                validate: function (value) {
                    if ($.trim(value) == '')
                        return 'El valor es requerido';
                    var result = validateTasaAndTipoPago(this);
                    if (result) return result;
                },
            });

            var validateTasaAndTipoPago = that => {
                var result = validateTasa(that);
                if (result) return result;

                result = validateTipoPago(that);
                if (result) return result;
             
            }

            var validateTasa = that => {
                let tasaCambio = $(that).closest('tr').find(positions.tasa_cambio).html();
                if (!isNumber($.trim(tasaCambio)) || parseInt(tasaCambio) == 0)
                    return 'La tasa de cambio no esta seleccionada, selecione una fecha';
            }

            var validateTipoPago = that => {
                let tipoPago = $(that).closest('tr').find(positions.tipo_pago).find('a.editable').editable('getValue').tipo_pago_id;
                if (!isNumber($.trim(tipoPago)))
                    return 'Seleccione un tipo de pago válido';
            }

            $('.selectorReferencia').editable({
                emptytext: 'Seleccione',
                validate: function (value) {
                    var result = validateTasaAndTipoPago(this);
                    if (result) return result;
                }
            });            
          
            $('.monto_efectivo, .monto_minu, .monto_cheq, .monto_trans').editable({
                validate: function (value) {
                    var result = validateTasaAndTipoPago(this);
                    if (result) return result;

                    if ($.trim(value) == '')
                        return 'El valor es requerido';
                    if (!isNumber($.trim(value)))
                        return 'El valor debe ser numerico';
                   
                },
                display: function (value) {
                    $(this).text(parseFloat(value).toFixed(2));
                }
            });
         
            $('.tipo_pago_id').on('save', function (e, params) {
                var excecpt = ['tipo_pago', 'tasa_cambio', 'fecha', 'totalD', 'totalC']
                for (var position in positions) {
                    if (!excecpt.includes(position)) {
                        if (position == 'referencia')
                            $(this).closest('tr').find(positions[position]).find('a.editable').editable('setValue', '', true).editable('toggleDisabled', false);
                        else
                            $(this).closest('tr').find(positions[position]).find('a.editable').editable('setValue', 0, true).editable('toggleDisabled', false);
                    }                    
                }               

                $(this).closest('tr').find(context.getPosition(params.newValue)).find('a.editable').editable('toggleDisabled', true);

                if (params.newValue != tipoPago.efectivo) {       
                    $(this).closest('tr').find(positions.referencia).find('a.editable').editable('toggleDisabled', true).editable('setValue', '', true);   
                    $(this).closest('tr').find(positions.banco).find('a.editable').editable('toggleDisabled', true).editable('setValue', 0, true).editable('show');
                } else {
                    var that = this;
                    setTimeout(function () {
                        $(that).closest('tr').find(positions.monto_efectivo).find('a.editable').editable('show');
                    }, 400);                   
                }
            });  

            $('.id_banco').on('save', function (e, params) {               
                $(this).closest('tr').find(positions.referencia).find('a.editable').editable('show');               
            });            

            $('.monto_efectivo, .monto_minu, .monto_cheq, .monto_trans').on("shown", function (e, editable) {  
                    var tipoMoneda = $('#TipoMonedaId').val();                   
                    var tipoPago = $(this).closest('tr').find(positions.tipo_pago).find('a.editable').editable('getValue').tipo_pago_id;
                    let tasaCambio = $(this).closest('tr').find(positions.tasa_cambio).html();

                    var _montodolar = detalle.reduce(function (a, b) { return (+a) + (+b.montodolar); }, 0);

                    var idSource = $(this).closest('tr').find(positions.fecha).attr('id-source');
                    
                    var montoPagado = referencias.filter(ref => ref.idSource != idSource).reduce(function (a, b) { return (+a) + (+b.totalD);}, 0);

                    var dif = _montodolar - montoPagado;

                    if (tipoMoneda == context.tipoMoneda.cordoba)
                        if (isNumber(tasaCambio)) 
                            dif = parseFloat(tasaCambio) * dif;    

                    editable.input.postrender = function () {
                        if (parseFloat(editable.input.$input[0].value) == 0)
                            editable.input.$input[0].value = parseFloat(dif).toFixed(4);
                        editable.input.$input.select();
                    };
            });

            $('.monto_efectivo, .monto_minu, .monto_cheq, .monto_trans').on('hidden', function (e, reason) {                
                if (reason === 'save')
                    calcularDetallePago();
            });    

            $('.selectorReferencia').on('save', function (e, params) {
                var tipo_pago_id = $(this).closest('tr').find(positions.tipo_pago).find('a.editable').editable('getValue').tipo_pago_id;
                $(this).closest('tr').find(context.getPosition(tipo_pago_id)).find('a.editable').editable('show');
            });      

            $('.selectorFecha').datepicker({
                autoclose: true,
                format: 'dd-mm-yyyy',
                language: 'es'
            });

            $('.selectorFecha').change(function () {
                var that = this;
                var fun = function (data) {                                 
                    $(that).closest('tr').find(positions.tasa_cambio).html(data.dolares);
                }

                var fail = function (xhr, status, error) {
                    $(that).closest('tr').find(positions.tasa_cambio).html('<span class="text-danger">Sin datos</span>');
                }
              
                findTypeOfficial(this.value, fun, fail);         
            });
           
            $('.editable2')
                .removeClass('tipo_pago_id')
                .removeClass('id_banco')
                .removeClass('monto_efectivo')
                .removeClass('monto_minu')
                .removeClass('monto_cheq')
                .removeClass('monto_trans')
                .removeClass('selectorReferencia');

            $('.datepicker').removeClass('selectorFecha');
            
        });
    }
    
    $.fn.dataSourceReference = function (data) {
        return this.each(function () {
            var table = $(this);
            $('#referencias tbody tr').remove();
            data.forEach(referencia => {
                $(table).addNewReference(referencia);
            });          
        });
    }

    $.fn.addNewServicio = function (servicio) {      
        return this.each(function () {
            var table = $(this);
            $(table).find('tbody').append('<tr>' +                         
                '<td class="w25"><select class="select40 data-ajax"><option value="">Seleccione</option></select></td>' +    
                '<td class="w40 text-center"><a href="#" data-name="precio"  data-original-title="Ingrese el precio" class="editable1 precio">' + (servicio ? servicio.Precio:"0.00") +'</a></td>' +   
                '<td class="w40"><a href="#" data-name="cantidad" data-original-title="Ingrese la cantidad" class="editable1 cantidad">' + (servicio ? servicio.Cantidad : "1") +'</a></td>' +   
                '<td class="w40">0.00</a></td>' +   
                '<td><a class="fa fa-remove fa-color-red a-type-cursor" onclick="removeServicio(this);"></a></td>' +
                '</tr>'
            );

            $('.precio, .cantidad').editable({
                validate: function (value) {
                    if ($.trim(value) == '')
                        return 'El valor es requerido';
                    if (!isNumber($.trim(value)))
                        return 'El valor debe ser numerico';
                },
                display: function (value) {
                    $(this).text(parseFloat(value).toFixed(2));
                }
            });

            $('.precio, .cantidad').on("shown", function (e, editable) {
                editable.input.postrender = function () {
                    editable.input.$input.select();
                };
            });

            $('.precio, .cantidad').on('hidden', function (e, reason) {
                if (reason === 'save')
                    calculate();
                $(this).closest('td').next('td').find('a.editable').editable('show'); 
            });

            var $select = $('.data-ajax').selectize({
                valueField: 'Cuenta',
                labelField: 'Nombre',
                searchField: 'Nombre',
                options: servicios.slice(0, 1500),
                create: false,
                render: {
                    option: function (item, escape) {

                        return '<div>' +
                            '<span class="title">' +
                            '<span class="cuenta"><i class="fa fa-book"></i> ' + escape(item.Cuenta) + '</span>' +
                            '</span>' +
                            '<p class="description"><i class="fa fa-' + (item.TipoCta != 4 ? "user user" : "money money") + '"></i> ' + escape(item.Nombre) + '</p>' +
                            '<p class="descriptionPadre">' + escape(item.padre) + '</p>' +
                            '</div>';
                    },
                    item: function (item, escape) {
                        return '<div>' +
                            ('<span class="name">' + escape(item.Nombre) + '</span>') +
                            ('<span class="account">' + escape(item.Cuenta) + '</span>') +
                            '</div>';
                    },
                },
                onDropdownClose: function ($dropdown) {
                    $($dropdown).closest('td').next('td').find('a.editable').editable('show'); 
                }
            });
          
            if (servicio) {
                var selectize = $select[0].selectize;               
                selectize.setValue(servicio.CtaContable.substring(4, servicio.CtaContable.length)); 
            }
           
            $('.editable1')
                .removeClass('precio')
                .removeClass('cantidad')              

            $('.data-ajax').removeClass('data-ajax');
        });
    }

    $.fn.dataSourceServicio = function (servicios) {
        return this.each(function () {
            var table = $(this);
            $('#servicios tbody tr').remove();
            servicios.forEach(servicio => {
                $(table).addNewServicio(servicio);
            });
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
            $(btn).html('<span class="fa fa-filter"></span> Filtrar');
        });
    }
}(jQuery));

var findTypeOfficial = (_date, fn, fail) => {
    var d = moment(_date, 'DD-MM-YYYY').utc().format()

    $.get(pathBase + "IngresosEgresosCajas/FindTipoCambio", { fecha: d }, fn, "json").fail(fail);
}

function calcularDetallePago() {
    referencias = []

    var tipoMoneda = $('#TipoMonedaId').val();
    var totalC = 0;
    var totalD = 0;

    $('#referencias tbody tr').each(function (index, element) {

        let fecha = $(this).find(positions.fecha).find('input').val();

        let idSource = $(this).find(positions.fecha).attr('id-source');

        let TipoPagoId = $(this).find(positions.tipo_pago).find('a').editable('getValue').tipo_pago_id;
        let IdBanco = $(this).find(positions.banco).find('a').editable('getValue').id_banco;
        let MontoEfectivo = $(this).find(positions.monto_efectivo).find('a').editable('getValue').monto_efectivo;      
        let MontoCheq = $(this).find(positions.monto_cheq).find('a').editable('getValue').monto_cheq;
        let MontoMinu = $(this).find(positions.monto_minu).find('a').editable('getValue').monto_minu;
        let MontoTrans = $(this).find(positions.monto_trans).find('a').editable('getValue').monto_trans;
        let referencia =    $(this).find(positions.referencia).find('a').editable('getValue').referencia;

        let tasaCambio = $(this).find(positions.tasa_cambio).html();

        var total = parseFloat(MontoEfectivo) + parseFloat(MontoMinu) + parseFloat(MontoCheq) + parseFloat(MontoTrans);
                
        if (tipoMoneda == context.tipoMoneda.cordoba) {
            totalC = total;
            totalD = parseFloat(total) / parseFloat(tasaCambio);            
           
            $(this).find(positions.totalC).html(numeral(totalC).format('$0,0.00'))
            $(this).find(positions.totalD).html(numeral(totalD).format('$0,0.00'));

        } else {
            totalC = parseFloat(total) * parseFloat(tasaCambio);
            totalD = total;            
         
            $(this).find(positions.totalC).html(numeral(totalC).format('$0,0.00'));
            $(this).find(positions.totalD).html(numeral(totalD).format('$0,0.00'));
        }    

        referencias.push({
            idSource,
            TipoPagoId,
            fecha,
            IdBanco,
            MontoEfectivo: parseFloat(MontoEfectivo),
            MontoMinu: parseFloat(MontoMinu),
            MontoCheq: parseFloat(MontoCheq),
            MontoTrans: parseFloat(MontoTrans),
            referencia,
            totalD: totalD,
            totalC: totalC
        });
    });

    var dolarNeto = referencias.reduce(function (a, b) {
        return (+a) + (+b.totalD);
    }, 0);

    $('.amountsDetalle li:eq(1)').html('<span class="pull-left">Total Dólares : </span><strong> $ ' + numeral(dolarNeto).format('$0,0.00') + '</strong>');  
    
}

var removeServicio = e => remove(e, calculate);

var removeReferencia = e => remove(e, calcularDetallePago);


var remove = (e,callback) => {
    $(e).parent().parent().addClass('animated fadeOutDown');
    function removeRow() {
        $(e).parent().parent().remove();
        callback();
    }
    setTimeout(removeRow, 500)    
}

function isNumber (n){return !isNaN(parseFloat(n)) && isFinite(n)}

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
            if (element.type != 'hidden') {
                if (element.dataset.required) {
                    if (element.value == '' || element.value == '0' || element.value == 0 || element.value == null) {
                        var hasErrorMessage = $(this).data('error-message');                     
                        if (hasErrorMessage) {
                            $.notification($(this).data('error-message'));
                            result = false;
                        }
                    }
                }                
            }            
        }
    });    
    if (detalle.length==0) {
        $.notification('Debe registrar al menos un servicio');
        result = false;
    }
    if (referencias.length == 0) {
        $.notification('Debe registrar el detalle de pago');
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
            let montodolar = parseFloat(precio) * parseFloat(cantidad);          
            
            $(this).find("td:eq(3)").html(parseFloat(montodolar).toFixed(2));          
                              
            detalle.push({
                cta_cuenta,              
                montodolar : montodolar,
                precio: parseFloat(precio),
                cantidad: parseFloat(cantidad)
            });   
        }        
    }); 
    var _montocordoba = detalle.reduce(function (a, b) {
        return (+a) + (+b.montocordoba);
    }, 0);
    var _montodolar = detalle.reduce(function (a, b) {
        return (+a) + (+b.montodolar);
    }, 0);
   
    $('.amountsServicio li:eq(1)').html('<span class="pull-left">Total Dólares : </span><strong>$ ' + numeral(_montodolar).format('$0,0.00')+'</strong>');    
   
}

var context = {
    ajax: {
        get: (url, data, callback) => {
            $.get(pathBase + url, data,callback,
                "json"
            );
        }
    },
    tipoMoneda : {
        cordoba: 1,
        dolar : 2
    },
    tipoPago: {
        efectivo: 1,
        cheque: 2,
        minuta: 3,
        trans: 4
    },
    pagoPosition: {
        fecha: 'td:eq(0)',
        tipo_pago: 'td:eq(1)',
        banco: 'td:eq(2)',
        referencia: 'td:eq(3)',
        monto_efectivo: 'td:eq(4)',
        monto_minu: 'td:eq(6)',
        monto_cheq: 'td:eq(5)',
        monto_trans: 'td:eq(7)',
        tasa_cambio: 'td:eq(8)',
        totalD: 'td:eq(9)',
        totalC: 'td:eq(10)',
    },
    getPosition: tipoPag => {
        var pos = ['','monto_efectivo', 'monto_cheq','monto_minu', 'monto_trans']
        return positions[pos[tipoPag]];
    }

}
var tipoPago = context.tipoPago;

var positions = context.pagoPosition;

var validateModel = model => {
    var isValid = true;
    if (model) {
        if (model.Idtipopago == 3 || model.Idtipopago == 4 || model.Idtipopago == 6) {
            if (!referencias.length) {
                isValid = false;             
                $('#btnShowReference').notification('Debe ingresar las referencias de deposito o transferencia', {
                    position: "left"
                });
            }
        }
    }
    return isValid;
}

$('#TipoMonedaId').change(function (e) {    
    calcularDetallePago();
});

var newRecibo = () => {
    window.location.href = pathBase + 'ingresosEgresosCajas/create';
}

var printReport = e =>{
    window.open(
        `${pathBase}ingresosEgresosCajas/print/${Id.value}`,
        '_blank' 
      );     
}