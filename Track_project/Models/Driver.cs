using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Track_project.Models;

[Table("driver")]
public partial class Driver
{
    [Key]
    [Column("driverid")]
    public long Driverid { get; set; }

    [Column("drivername", TypeName = "character varying")]
    public string? Drivername { get; set; }

    [Column("phonenumber")]
    public long? Phonenumber { get; set; }

    [InverseProperty("Driver")]
    public virtual ICollection<Vehiclesinformation> Vehiclesinformations { get; set; } = new List<Vehiclesinformation>();
}
