/***************************************************************************************
*    Title: Inventory Database Entity
*    Author: Maseeha17
*    Date: 3 October 2026
*    Code version: Version 1.0
*    Availability: FMCGEnterpriseManagementSystem/Models/Inventory.cs
***************************************************************************************/

using System.ComponentModel.DataAnnotations;
﻿using FMCGEnterpriseManagementSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Inventory
{
    [Key]
    public int InventoryId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;

   
    public int QuantityOnHand { get; set; }

    public int ReorderLevel { get; set; }
    public string? Notes { get; set; }


      
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<StockBatch> StockBatches { get; set; } = new List<StockBatch>();
}