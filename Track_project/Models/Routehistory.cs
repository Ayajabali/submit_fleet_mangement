using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Track_project.Models;

[Table("routehistory")]
public partial class Routehistory
{
    [Key]
    [Column("routehistoryid")]
    public long Routehistoryid { get; set; }

    [Column("vehicleid")]
    public long? Vehicleid { get; set; }

    [Column("vehicledirection")]
    public int? Vehicledirection { get; set; }

    [Column("status")]
    [MaxLength(1)]
    public char? Status { get; set; }

    [Column("vehiclespeed", TypeName = "character varying")]
    public string? Vehiclespeed { get; set; }

    [Column("epoch")]
    public long? Epoch { get; set; }

    [Column("address", TypeName = "character varying")]
    public string? Address { get; set; }

    [Column("latitude")]
    public float? Latitude { get; set; }

    [Column("longitude")]
    public float? Longitude { get; set; }

    [ForeignKey("Vehicleid")]
    [InverseProperty("Routehistories")]
    public virtual Vehicle Vehicle { get; set; }

}
