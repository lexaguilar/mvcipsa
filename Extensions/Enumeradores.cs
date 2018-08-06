using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Extensions
{
    enum IngresosEgresosCajaEstado { Indefinido, Registrado, Anulado }

    enum Roles { Ninguno, Administrador, Usuario, Reportes,CambiarTasa }

    public enum TipoDocumentos { Ninguno, Desposito = 1, NotaDeDebito = 3, NotaDeCredito = 4, Cheque = 2 }
    public enum TipoMovimientos { Cheques, ServicioDeCuarentenaAgropecuaria = 1, ServicioDeInocuidadAgroalimentaria = 2, ServicioDeSanidadVegetralySemillas = 3, ServicioDeSaludAnimal = 4, ServicioDeLaboratorioLNRQB = 5,
        ServicioDeLaboratorioLNDVMA = 6, NotaDeCredito = 7, ServicioDeLaboratorioLNDFCS = 8, NotaDeDebito = 9, TrasladosDeFondos = 10, CorreccionIntMenos = 11, CorreccionIntMas = 12, VariosODepositosDiversos = 13,
        DepositosDeCajaUnica = 14, TransferenciasACentros = 15, TransferenciaAnulada = 31, Servicios = 32, Laboratorios = 33, Retenciones = 34, SobrantesAnticipos = 35, Carnet = 36, Otros = 37 }
    enum IngresosEgresosBancoEstados { Indefinido, Registrado, Anulado }

    enum Monedas { Cordoba = 1, Dolares = 2 }

    enum TipoProcesos { SaldoInicial = 1, Movimientos = 2 }

    enum TipoPagos { Efectivo = 1, Cheque = 2 , Minuta = 3, Transferencia = 4}

    enum ConciliacionEstados { Pendiente = -1, EncontradoF1 = 1, EncontradoF2 = 2, EncontradoF3=3, EncontradoF4 = 4, DuplicadoF1 = 5, DuplicadoF2 = 6, EncontradoM = 7}

}
