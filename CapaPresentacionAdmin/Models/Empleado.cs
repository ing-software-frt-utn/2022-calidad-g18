//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CapaPresentacionAdmin.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Empleado
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Empleado()
        {
            this.OrdenProduccion = new HashSet<OrdenProduccion>();
            this.OrdenProduccion1 = new HashSet<OrdenProduccion>();
        }
    
        public int DNI { get; set; }
        public string Apellidos { get; set; }
        public string Nombres { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public string TipoEmpleado { get; set; }
        public Nullable<System.DateTime> FechaRegistro { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrdenProduccion> OrdenProduccion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrdenProduccion> OrdenProduccion1 { get; set; }
    }
}
