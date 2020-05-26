//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RentC.UI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Reservations
    {
        public int ReservationID { get; set; }
        [Required]
        [Display(Name = "Car ID")]
        public int CarID { get; set; }
        [Required]
        [Display(Name = "Car ID")]
        public int CustomerID { get; set; }
        public byte ReservStatsID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime EndDate { get; set; }
        [Required]
        public string Location { get; set; }
        public string CouponCode { get; set; }
        public string CartPlate { get; set; }
    
        public virtual Cars Cars { get; set; }
        public virtual Coupons Coupons { get; set; }
        public virtual Customers Customers { get; set; }
        public virtual ReservationStatuses ReservationStatuses { get; set; }
    }
}