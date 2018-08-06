using System;

namespace mvcIpsa.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public int CajaId { get; set; }
        public int ReciboId { get; set; }
        public string Descripcion { get; set; }
    }
}
