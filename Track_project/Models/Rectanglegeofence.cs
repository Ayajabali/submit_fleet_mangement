using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Track_project.Models;

[Table("rectanglegeofence")]
public partial class Rectanglegeofence
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("geofenceid")]
    public long? Geofenceid { get; set; }

    [Column("north")]
    public float? North { get; set; }

    [Column("east")]
    public float? East { get; set; }

    [Column("west")]
    public float? West { get; set; }

    [Column("south")]
    public float? South { get; set; }

    [ForeignKey("Geofenceid")]
    [InverseProperty("Rectanglegeofences")]
    public virtual Geofence? Geofence { get; set; }
}
