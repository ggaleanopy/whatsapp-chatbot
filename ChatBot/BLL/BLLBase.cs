using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatBot.BLL
{
    /// <summary>
    /// Clase base para la logica de negocios de GERP.
    /// </summary>
    public class BLLBase : IDisposable
    {
        /// <summary>
        /// Indica los posibles valores de navegación o recorrido de registros.
        /// </summary>
        public enum NavigationType
        {
            First = 0,
            Back = 1,
            Next = 2,
            Last = 3
        }

        /// <summary>
        /// Libera recursos innecesarios de la memoria.
        /// </summary>
        public void Dispose()
        {
            GC.Collect();
        }
    }
}