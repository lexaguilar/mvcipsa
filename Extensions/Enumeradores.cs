using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Extensions
{
    enum IngresosEgresosCajaEstado { Indefinido, Registrado, Anulado, Cerrado }
    enum Roles { Ninguno, Administrador, Usuario, Reportes }

    public enum TipoDocumentos{Ninguno, Despositos=1 , NotaDebito=3, NotaCredito=4}

}
