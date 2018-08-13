var tables = ['iecb', 'ccc'];
var formats;
var $moneda;

var estados = {
    pendiente: -1,
    found: 1,
    foundMixto: 2,
    foundOutDate: 3,
    foundOutDateMixto: 4,
    duplicated: 5,
    duplicatedOutDate: 6,
    manual: 7
}

var _estados = {
    1: "Encontrado F1",
    2: "Encontrado F2",
    3: "Encontrado F3",
    4: "Encontrado F4",
    5: "Duplicado F1",
    6: "Duplicado F2",
    7: "Encontrado M",
}

var tipoMovimientos = {
    deposito: 1,
    cheque: 2,
    notaDebito: 3,
    notaCredito: 4
}

var $gridBanco = '#dxGridBanco';
var $gridIecb = '#dxGridIecb'

var $BancoData = [];
var $IecbData = [];

var sourceBanco = new DevExpress.data.DataSource({
    store: {
        type: 'array',
        data: [],
        key: 'Referencia'
    }
});

$($gridBanco).dxDataGrid({
    dataSource: sourceBanco,
    pager: {
        showInfo: true,
    },
    columnsAutoWidth: true,
    allowColumnResizing: true,
    headerFilter: {
        visible: true
    },
    filterRow: {
        visible: true,
        applyFilter: "auto"
    },
    editing: {
        allowUpdating: true,
        mode: 'cell',
    },
    onContextMenuPreparing: function (e) {
        if (e.row.rowType === "data") {
            e.items = [{
                text: "Conciliar manual",
                onItemClick: function () {
                    _conciliarManual();
                }
            }];
        }
    },
    columns: [
        {
            dataField: "Fecha",
            alignment: "right",
            dataType: "date",
            format: "dd/MM/yyyy",
            allowEditing: false
        }, {
            dataField: "Referencia",
            allowEditing: false
        }, {
            dataField: "TipoMovimiento",
            alignment: "right",
            allowEditing: false
        }, {
            dataField: "Debito",
            alignment: "right",
            allowEditing: false,
            cellTemplate: function (container, options) {
                $("<div>")
                    .append(numeral(options.value).format('$0,0.0000'))
                    .appendTo(container);
            },
        }, {
            dataField: "Credito",
            alignment: "right",
            allowEditing: false,
            cellTemplate: function (container, options) {
                $("<div>")
                    .append(numeral(options.value).format('$0,0.0000'))
                    .appendTo(container);
            },
        }, {
            dataField: "Estado",
            alignment: "right",
            allowEditing: false
        }, {
            caption: "",
            dataField: "ck",
            dataType: "boolean",
            allowEditing: true,
            width: 80
        },
        {
            caption: "",
            dataField: "UUID",
            dataType: "string",
            visible: false,
        }
    ],
    summary: {
        totalItems: [{
            column: "Credito",
            summaryType: "sum",
            customizeText: function (data) {
                return ($moneda || '') + ' ' + numeral(data.value).format('$0,0.0000');
            }
        }, {
            column: "Debito",
            summaryType: "sum",
            customizeText: function (data) {
                return ($moneda || '') + ' ' + numeral(data.value).format('$0,0.0000');
            }
        }, {
            column: "Referencia",
            summaryType: "count",
            customizeText: function (data) {
                return data.value + ' registros';
            }
        }]
    },
    onCellClick: function (e) {
        if (e.rowType == 'data' && e.column.dataField == "ck") {
            if (e.value) {
                $BancoData.push(e.data);
            } else {
                $BancoData = $BancoData.filter(x => x.Referencia != e.data.Referencia && x.Fecha != e.data.Fecha && x.Debito != e.data.Debito && x.Credito != e.data.Credito);
            }
        }
    },
    onRowPrepared: function (info) {
        if (info.rowType != 'header' && info.rowType != 'totalFooter' && info.rowType != 'filter') {
            if (info.data.EstadoId == estados.found)
                info.rowElement.addClass('found');
            if (info.data.EstadoId == estados.foundOutDate)
                info.rowElement.addClass('foundOutDate');
            if (info.data.EstadoId == estados.foundMixto)
                info.rowElement.addClass('foundMixto');
            if (info.data.EstadoId == estados.foundOutDateMixto)
                info.rowElement.addClass('foundOutDateMixto');
            if (info.data.EstadoId == estados.duplicated)
                info.rowElement.addClass('duplicated');
            if (info.data.EstadoId == estados.duplicatedOutDate)
                info.rowElement.addClass('duplicatedOutDate');
            if (info.data.EstadoId == estados.manual)
                info.rowElement.addClass('foundManual');
        }
    }
}).dxDataGrid("instance");

var sourceIecb = new DevExpress.data.DataSource({
    store: {
        type: 'array',
        data: [],
        key: 'Referencia'
    }
});

$($gridIecb).dxDataGrid({
    dataSource: sourceIecb,
    pager: {
        showInfo: true,
    },
    columnsAutoWidth: true,
    headerFilter: {
        visible: true
    },
    editing: {
        allowUpdating: true,
        mode: 'cell',
    },
    filterRow: {
        visible: true,
        applyFilter: "auto"
    },
    onContextMenuPreparing: function (e) {
        if (e.row.rowType === "data") {
            e.items = [{
                text: "Conciliar manual",
                onItemClick: function () {
                    _conciliarManual();
                }
            }];
        }
    },
    columns: [
        {
            caption: "",
            dataField: "ck",
            dataType: "boolean",
            allowEditing: true,
            width: 80
        }, {
            dataField: "Estado",
            allowEditing: false
        },
        {
            dataField: "Fecha",
            alignment: "right",
            dataType: "date",
            format: "dd/MM/yyyy",
            allowEditing: false
        },
        {
            dataField: "Referencia",
            alignment: "right",
            allowEditing: false
        },
        {
            dataField: "TipoMovimiento",
            alignment: "right",
            allowEditing: false
        },
        {
            dataField: "Debito",
            allowEditing: false,
            cellTemplate: function (container, options) {
                $("<div>")
                    .append(numeral(options.value).format('$0,0.0000'))
                    .appendTo(container);
            },
        },
        {
            dataField: "Credito",
            allowEditing: false,
            cellTemplate: function (container, options) {
                $("<div>")
                    .append(numeral(options.value).format('$0,0.0000'))
                    .appendTo(container);
            },
        },
        {
            dataField: "TipoMovimiento",
            visible: false
        },
        {
            dataField: "CajaId",
            allowEditing: false,
            width: 50
        },
        {
            caption: "Recibo",
            dataField: "IdRef",
            cellTemplate: function (container, options) {
                $("<div>")
                    .append(`<a class="href-button text-custom-16" onclick="printReportById(${options.value},${options.data.TableInfo});"> ${options.data.NumRecibo}</a>`)
                    .appendTo(container);
            },
            allowEditing: false
        },
        {
            caption: "",
            dataField: "UUID",
            dataType: "string",
            visible: false,
        }
    ],
    summary: {
        totalItems: [{
            column: "Credito",
            summaryType: "sum",
            customizeText: function (data) {

                return ($moneda || '') + ' ' + numeral(data.value).format('$0,0.0000');
            }
        }, {
            column: "Debito",
            summaryType: "sum",
            customizeText: function (data) {

                return ($moneda || '') + ' ' + numeral(data.value).format('$0,0.0000');
            }
        }, {
            column: "Estado",
            summaryType: "count",
            customizeText: function (data) {
                return data.value + ' registros';
            }
        }]
    },
    onCellClick: function (e) {
        if (e.rowType == 'data' && e.column.dataField == "ck") {
            if (e.value) {
                $IecbData.push(e.data);
            } else {
                $IecbData = $IecbData.filter(x => x.Referencia == e.data.Referencia && x.NumRecibo !== e.data.NumRecibo);
            }
        }
    },
    onRowPrepared: function (info) {
        if (info.rowType != 'header' && info.rowType != 'totalFooter' && info.rowType != 'filter') {
            if (info.data.EstadoId == estados.found)
                info.rowElement.addClass('found');
            if (info.data.EstadoId == estados.foundOutDate)
                info.rowElement.addClass('foundOutDate');
            if (info.data.EstadoId == estados.foundMixto)
                info.rowElement.addClass('foundMixto');
            if (info.data.EstadoId == estados.foundOutDateMixto)
                info.rowElement.addClass('foundOutDateMixto');
            if (info.data.EstadoId == estados.duplicated)
                info.rowElement.addClass('duplicated');
            if (info.data.EstadoId == estados.duplicatedOutDate)
                info.rowElement.addClass('duplicatedOutDate');
            if (info.data.EstadoId == estados.manual)
                info.rowElement.addClass('foundManual');

        }
    }
}).dxDataGrid("instance");

var showButtonConciliar = () => {
    var rowsIecb = allRows($gridIecb);
    var rowsCcc = allRows($gridBanco);

    if (rowsIecb.length && rowsCcc.length)
        $('.conciliar').removeAttr('disabled');
    else
        $('.conciliar').attr('disabled', 'disabled');
}

var allRows = mytable => $(`${mytable}`).dxDataGrid("instance").option('dataSource')

var dialogLoading = document.querySelector('#dialog-loading');

var closeDialog = () => {
    dialogLoading.close();
}

var updateEstado = x => {
    x.Estado = 'Pendiente';
    x.EstadoId = estados.pendiente;
    return x;
}

var isConsiliado = x => !compareOr(x.EstadoId).with(-1, 5, 6)

var isFirtTime = true;

var _conciliarAutomatico = function () {
    if (isFirtTime) {

        isFirtTime = false;
        $('#btnCloseDialog').hide();
        $('#ready').hide();

        var iteration = 1;
        var _itemFounds = 0
        itemFounds.innerText = _itemFounds;
        if (!dialogLoading.showModal) {
            dialogPolyfill.registerDialog(dialogLoading);
        }

        dialogLoading.showModal();
        var elem_pgrs = document.querySelector('#p1');
        var pgrs = 0
        $('#loaderConciliando').show();
        $('#percentConciliando').show();

        var dataIecb = allRows($gridIecb).map(updateEstado);
        var dataBanco = allRows($gridBanco).map(updateEstado);

        var format = customFormat.value;
        var total = dataBanco.length;
        iterationTotal.innerText = total;

        var indexBanco = 0;
        function buscarBanco() {
            setTimeout(function () {

                const elementBanco = dataBanco[indexBanco];
                counting.innerText = iteration;
                pgrs = parseInt(iteration / total * 100);
                percentConciliando.innerText = pgrs + '%'
                elem_pgrs.MaterialProgress.setProgress(pgrs);
                iteration++;

                for (let indexIecb = 0; indexIecb < dataIecb.length; indexIecb++) {
                    const elementIecb = dataIecb[indexIecb];

                    var bancoReferencia = parseFloat(elementBanco.Referencia);
                    var IecbReferencia = parseFloat(elementIecb.Referencia);

                    if (bancoReferencia == IecbReferencia) {
                        var montoAux = compareOr(elementIecb.TipoDocumento)
                            .with(tipoMovimientos.deposito, tipoMovimientos.notaDebito) ? elementIecb.Debito : elementIecb.Credito;
                        var montoBanco = compareOr(elementIecb.TipoDocumento)
                            .with(tipoMovimientos.deposito, tipoMovimientos.notaDebito) ? elementBanco.Credito : elementBanco.Debito;

                        var _fechaAux = moment(elementIecb.Fecha).format('DD-MM-YYYY');
                        var _fechaBanco = moment(elementBanco.Fecha).format('DD-MM-YYYY');

                        if (parseFloat(montoAux) == parseFloat(montoBanco)) {
                            if (_fechaBanco == _fechaAux) {
                                var _count = dataIecb.filter(x => parseFloat(x.Referencia) == bancoReferencia).length;
                                if (_count == 1) {
                                    var uuid = generateUUID();
                                    dataIecb[indexIecb].Estado = _estados[estados.found];
                                    dataIecb[indexIecb].EstadoId = estados.found;
                                    dataIecb[indexIecb].UUID = uuid;

                                    dataBanco[indexBanco].Estado = _estados[estados.found];
                                    dataBanco[indexBanco].EstadoId = estados.found;
                                    dataBanco[indexBanco].UUID = uuid;

                                    _itemFounds++;
                                    itemFounds.innerText = _itemFounds;
                                } else {
                                    dataBanco[indexBanco].Estado = _estados[estados.duplicated];
                                    dataBanco[indexBanco].EstadoId = estados.duplicated;

                                    dataIecb = dataIecb.map(x => {

                                        if (parseFloat(x.Referencia) == bancoReferencia) {
                                            x.Estado = _estados[estados.duplicated];
                                            x.EstadoId = estados.duplicated;
                                        }

                                        return x;
                                    });
                                }
                                break;
                            } else {
                                var _count = dataIecb.filter(x => parseFloat(x.Referencia) == bancoReferencia).length;
                                if (_count == 1) {
                                    var uuid = generateUUID();
                                    dataBanco[indexBanco].Estado = _estados[estados.foundOutDate];
                                    dataBanco[indexBanco].EstadoId = estados.foundOutDate;
                                    dataBanco[indexBanco].UUID = uuid;

                                    dataIecb = dataIecb.map(x => {

                                        if (parseFloat(x.Referencia) == bancoReferencia) {
                                            x.Estado = _estados[estados.foundOutDate];
                                            x.EstadoId = estados.foundOutDate;
                                            x.UUID = uuid
                                        }

                                        return x;

                                    });

                                    _itemFounds++;
                                    itemFounds.innerText = _itemFounds
                                } else {
                                    dataBanco[indexBanco].Estado = _estados[estados.duplicatedOutDate];
                                    dataBanco[indexBanco].EstadoId = estados.duplicatedOutDate;

                                    dataIecb = dataIecb.map(x => {

                                        if (parseFloat(x.Referencia) == bancoReferencia) {
                                            x.Estado = _estados[estados.duplicatedOutDate];
                                            x.EstadoId = estados.duplicatedOutDate;
                                        }

                                        return x;
                                    });
                                }

                                break;
                            }
                        } else {

                            var searchDuplicate = dataIecb.filter(c => parseFloat(c.Referencia) == IecbReferencia
                                && moment(c.Fecha).format('DD-MM-YYYY') == _fechaBanco);

                            if (searchDuplicate.length > 1) {
                                var _totalBanc = compareOr(elementIecb.TipoDocumento).with(tipoMovimientos.deposito, tipoMovimientos.notaDebito) ? elementBanco.Credito : elementBanco.Debito;
                                var sumAux = compareOr(elementIecb.TipoDocumento).with(tipoMovimientos.deposito, tipoMovimientos.notaDebito) ? searchDuplicate.sum('Debito') : searchDuplicate.sum('Credito');
                                if (parseFloat(sumAux) == parseFloat(_totalBanc)) {
                                    var uuid = generateUUID();
                                    dataBanco[indexBanco].Estado = _estados[estados.foundMixto];
                                    dataBanco[indexBanco].EstadoId = estados.foundMixto;
                                    dataBanco[indexBanco].UUID = uuid;

                                    dataIecb = dataIecb.map(x => {

                                        if (parseFloat(x.Referencia) == bancoReferencia) {
                                            x.Estado = _estados[estados.foundMixto];
                                            x.EstadoId = estados.foundMixto;
                                            x.UUID = uuid
                                        }

                                        return x;

                                    });

                                    _itemFounds++;
                                    itemFounds.innerText = _itemFounds
                                    break;
                                }
                            } else {
                                searchDuplicate = dataIecb.filter(c => parseFloat(c.Referencia) == IecbReferencia);
                                var _totalBanc = compareOr(elementIecb.TipoDocumento).with(tipoMovimientos.deposito, tipoMovimientos.notaDebito) ? elementBanco.Credito : elementBanco.Debito;
                                var sumAux = compareOr(elementIecb.TipoDocumento).with(tipoMovimientos.deposito, tipoMovimientos.notaDebito) ? searchDuplicate.sum('Debito') : searchDuplicate.sum('Credito');

                                if (parseFloat(sumAux) == parseFloat(_totalBanc)) {
                                    var uuid = generateUUID();
                                    dataBanco[indexBanco].Estado = _estados[estados.foundOutDateMixto];
                                    dataBanco[indexBanco].EstadoId = estados.foundOutDateMixto;
                                    dataBanco[indexBanco].UUID = uuid;

                                    dataIecb = dataIecb.map(x => {

                                        if (parseFloat(x.Referencia) == bancoReferencia) {
                                            x.Estado = _estados[estados.foundOutDateMixto];
                                            x.EstadoId = estados.foundOutDateMixto;
                                            x.UUID = uuid
                                        }

                                        return x;

                                    });

                                    _itemFounds++;
                                    itemFounds.innerText = _itemFounds
                                    break;
                                }
                            }

                        }
                    }
                }
                indexBanco++;
                if (indexBanco < total) {
                    buscarBanco();
                } else {
                    $($gridIecb).dxDataGrid("instance").option('dataSource', dataIecb);
                    $($gridBanco).dxDataGrid("instance").option('dataSource', dataBanco);
                    $('#btnCloseDialog').show();
                    $('#loaderConciliando').hide();
                    $('#percentConciliando').hide();
                    $('#ready').show();

                }
            }, 3);
        }
        buscarBanco();
    } else {
        conciliarQuestion();
    }
}

var _conciliarManual = function () {
    var isValid = true;
    if ($IecbData.some(isConsiliado)) {
        $.notification(`Las referencias del auxiliar ${$IecbData.filter(isConsiliado).map(x => x.Referencia).join(',')} ya tienen estado conciliado`);
        isValid = false;
    }

    if ($BancoData.some(isConsiliado)) {
        $.notification(`Las referencias del estado de cuenta ${$BancoData.filter(isConsiliado).map(x => x.Referencia).join(',')} ya tienen estado conciliado`);
        isValid = false;
    }
    if (isValid) {

        var conciliar = () => {
            var dataIecb = allRows($gridIecb);
            var dataBanco = allRows($gridBanco);

            var uuid = generateUUID();
            dataBanco = dataBanco.map(x => {
                if (parseFloat(x.Referencia) == parseFloat($BancoData[0].Referencia) && x.Fecha == $BancoData[0].Fecha) {
                    x.Estado = _estados[estados.manual];
                    x.EstadoId = estados.manual;
                    x.UUID = uuid;
                }
                return x;
            });


            dataIecb = dataIecb.map(x => {
                if (parseFloat(x.Referencia) == parseFloat($IecbData[0].Referencia) && x.NumRecibo == $IecbData[0].NumRecibo) {
                    x.Estado = _estados[estados.manual];
                    x.EstadoId = estados.manual;
                    x.UUID = uuid;
                }
                return x;
            });

            $($gridIecb).dxDataGrid("instance").option('dataSource', dataIecb);
            $($gridBanco).dxDataGrid("instance").option('dataSource', dataBanco);
        }


        if ($IecbData.length && $BancoData.length) {
            var allReferenciaAreUniqueBanco = $BancoData.map(x => x.Referencia).unique();
            var allReferenciaAreUniqueIecb = $IecbData.map(x => x.Referencia).unique();
            if (allReferenciaAreUniqueIecb > 1) {
                if (allReferenciaAreUniqueBanco > 1) {
                    var _totalAux = compareOr($IecbData[0].TipoDocumento).with(tipoMovimientos.deposito, tipoMovimientos.notaDebito) ? $IecbData.sum('Debito') : $IecbData.sum('Credito');
                    var _totalBanco = compareOr($IecbData[0].TipoDocumento).with(tipoMovimientos.deposito, tipoMovimientos.notaDebito) ? $BancoData.sum('Credito') : $BancoData.sum('Debito');
                    if (_totalAux == _totalBanco) {
                        if (parseFloat($BancoData[0].Referencia) == parseFloat($IecbData[0].Referencia)) {
                            conciliar();
                        } else {
                            $.notification(`Las referencias no coinciden entre los registros seleccionados banco(${parseFloat($BancoData[0].Referencia)} auxiliar (${parseFloat($IecbData[0].Referencia)}))`);
                        }
                    } else {
                        $.notification(`Las sumas no cuadran con los items seleccionados banco(${_totalBanco} y auxiliar(${_totalAux}))`);
                    }
                } else {
                    $.notification(`Las referencias seleccionadas del auxiliar no son unicas ${$IecbData.map(x => x.Referencia).join(',')}, seleccione registros con la misma referencia`);
                }
            } else {
                $.notification(`Las referencias seleccionadas del banco no son unicas ${$BancoData.map(x => x.Referencia).join(',')}, seleccione registros con la misma referencia`);
            }
        } else {
            $.notification('Seleccione registros de ambas tablas');
        }

    }
}

var botonclick = false;
var conciliarQuestion = () => {
    if (!botonclick) {
        notif_confirm({
            'message': 'Volver a conciliar?',
            'textaccept': 'Si',
            'textcancel': 'No',
            'fullscreen': true,
            'callback': function (choice) {
                if (choice == true) {
                    isFirtTime = true;
                    _conciliarAutomatico();
                }
            }
        })
    }
    return botonclick;
}

var saveConciliacion = () => {
    var conciliacionBancariaAux = $($gridIecb).dxDataGrid("instance").option('dataSource');
    var conciliacionBancariaEC = $($gridBanco).dxDataGrid("instance").option('dataSource');

    if (conciliacionBancariaAux.length && conciliacionBancariaEC) {
        if (conciliacionBancariaAux.some(isConsiliado) && conciliacionBancariaEC.some(isConsiliado)) {
            var Parameter = findEntity('Parameter');
            $('body').loading({
                message: 'Guardando...'
            });
            $.get(pathBase + `api/catalogs/procesoBanco/bank/${Parameter.BancosCuenta}`, function (data) {
                if (!data) {
                    var text = $('#BancosCuenta').text();
                    $.notification('Por favor ingrese el saldo inicial para la cuenta ' + text);
                    $('body').loading('stop');
                } else {
                    continuar();
                }
            });
        } else {
            $.notification('Debe conciliar los datos antes de guardar');
        }
    } else {
        $.notification('Debe de cargar los datos previamente');
    }
}

function continuar() {

    var Parameter = findEntity('Parameter');
    var data = {
        BancoCuenta: Parameter.BancosCuenta,
        Year: Parameter.Year,
        Month: Parameter.Month,
        conciliacionBancariaAux: $($gridIecb).dxDataGrid("instance").option('dataSource'),
        conciliacionBancaria: $($gridBanco).dxDataGrid("instance").option('dataSource')
    }

    $.post(pathBase + 'procesosBanco/SaveAuxiliarAndIngresosEgresos', data, function () {
        $.notification("Datos guardados correctamente", "success");
    }).always(function () {
        showButtonConciliar();
        $('body').loading('stop');
    }).fail(function () {
        $.notification("Error al guardar la información");

    });
}