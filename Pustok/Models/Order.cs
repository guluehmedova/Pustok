﻿using Pustok.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class Order:BaseEntity
    {
        public string AppUserId { get; set; }
        [StringLength(maximumLength: 5)]
        public string CodePrefix { get; set; }
        public int CodeNumber { get; set; }
        [StringLength(maximumLength: 50)]
        public string FullName { get; set; }
        [StringLength(maximumLength: 100)]
        public string Email { get; set; }
        [Required]
        [StringLength(maximumLength: 25)]
        public string Phone { get; set; }
        [Required]
        [StringLength(maximumLength: 250)]
        public string Address1 { get; set; }
        [StringLength(maximumLength: 250)]
        public string Address2 { get; set; }
        [Required]
        [StringLength(maximumLength: 25)]
        public string Country { get; set; }
        [Required]
        [StringLength(maximumLength: 25)]
        public string City { get; set; }
        [Required]
        [StringLength(maximumLength: 20)]
        public string Zipcode { get; set; }
        public OrderStatus Status { get; set; }
        public OrderDeliveryStatus? DeliveryStatus { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        [StringLength(maximumLength: 350)]
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public AppUser AppUser { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
