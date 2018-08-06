var dataConfig = context.dataConfig;
var entity = 'IngresosEgresosBanco';

dataConfig['Id'] = () => {
    return context.dataConfig.createConfig(
        {     
            ToEdit :  {
                required: true,
            },           
            id: 'Id',
            entity: entity,
        });
}

dataConfig['Referencia'] = () => {
    return context.dataConfig.createConfig(
        {
            ToSave :  {
                required: true,
            },
            ToEdit :  {
                required: true,
            },           
            id: 'Referencia',
            entity: entity,
        });
}

dataConfig['Monto'] = () => {
    return context.dataConfig.createConfig(
        {
            ToSave :  {
                required: true,
            },
            ToEdit :  {
                required: true,
            },           
            id: 'Monto',
            entity: entity,
        });
}

dataConfig['BancoCuenta'] = () => {
    return context.dataConfig.createConfig(
        {
            url : 'bancocuentas',
            ToSave :  {
                required: true,
            },
            ToEdit :  {
                required: true,
            },           
            id: 'BancoCuenta',
            display : 'Cuenta Bancaria',
            entity: entity,
            render: {
                option: function (item, escape) {
                    return '<div>' +
                        '<span class="title">' +
                        '<span class="cedulaCliente"><i class="fa fa-book"></i> ' + escape(item.Text) + '</span>' +
                        '</span>' +
                        '<p class="nombreCliente">' + escape(item.Banco) + '</p>' +
                        '<p class="nombreCliente">' + escape(item.Moneda) + '</p>' +
                        '</div>';
                },                
                item: function (data, escape) {
                    return '<div class="">' + escape(data.Text) + '</div>';
                }
            }
        });
}

dataConfig['TipoMovimientoId'] = () => {
    return context.dataConfig.createConfig(
        {
            url : 'tipoMovimientos',
            ToSave :  {
                required: true,
            },
            ToEdit :  {
                required: true,
            },           
            id: 'TipoMovimientoId',
            display : 'Tipo de movimiento',
            entity: entity,
        });
}

dataConfig['Concepto'] = () => {
    return context.dataConfig.createConfig(
        {
            ToSave :  {
                required: true,
            },
            ToEdit :  {
                required: true,
            },           
            id: 'Concepto',
            entity: entity,
        });
}

dataConfig['FechaProceso'] = () => {
    return context.dataConfig.createConfig(
        {
            ToSave :  {
                required: true,
            },
            ToEdit :  {
                required: true,
            },           
            id: 'FechaProceso',
            display : 'Fecha',
            entity: entity,
        });
}

dataConfig['TipoDocumentoId'] = () => {
    return context.dataConfig.createConfig(
        {
            url : 'tipoDocumentos',
            ToSave :  {
                required: true,
            },
            ToEdit :  {
                required: true,
            },           
            id: 'TipoDocumentoId',
            display : 'Tipo de documento',
            entity: entity,
        });
}

var banco = {
    save : function()  {
        var banco = buildEntity(entity);
        if (banco) {
            $('.wrapper').loading({message : 'Guardando...'});

            var verb = 'put'
            if (Id.value=='0') {
                verb = 'post'
            }

            var errorOnSave = response => {
                $('.wrapper').loading('stop');
                context.ajax.failResult(response);               
            }

            var okOnSave = response => {
                $('.wrapper').loading('stop');
                if (Id.value != '0')
                    $.notification(`Movimiento ${response.Id} actualizado correctamente`,'info');
                else {
                    Id.value = response.Id;
                    $('#lblId').text(`${response.Id}`);
                    $.notification(`Movimiento guardado correctamente No.:${response.Id}`,'success');
                }
            }

            var url = `api/banco/guardar`;
            context.ajax.full(url, banco, okOnSave, errorOnSave, verb);
        }
    },
    setValues : function(data){
        
        bindingData(data);      
        
    },
    load: function(id){
        $('.wrapper').loading({message : 'Cargando...'});
        context.ajax.get('api/banco/cargar/'+id,null, data =>{            
            this.setValues(data);
            $('.wrapper').loading('stop');  
        });
    },
    anular: function () {

    }

}