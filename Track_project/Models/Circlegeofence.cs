using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Track_project.Models;

[Table("circlegeofence")]
public partial class Circlegeofence
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("geofenceid")]
    public long? Geofenceid { get; set; }

    [Column("radius")]
    public long? Radius { get; set; }

    [Column("latitude")]
    public float? Latitude { get; set; }

    [Column("longitude")]
    public float? Longitude { get; set; }

    [ForeignKey("Geofenceid")]
    [InverseProperty("Circlegeofences")]
    public virtual Geofence? Geofence { get; set; }
}
