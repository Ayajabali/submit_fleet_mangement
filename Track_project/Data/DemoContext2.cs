using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Track_project.Models;

namespace Track_project.Data;

public partial class DemoContext2 : DbContext
{
    public DemoContext2()
    {
    }

    public DemoContext2(DbContextOptions<DemoContext2> options)
        : base(options)
    {
    }

    public virtual DbSet<Circlegeofence> Circlegeofences { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<Geofence> Geofences { get; set; }

    public virtual DbSet<Polygongeofence> Polygongeofences { get; set; }

    public virtual DbSet<Rectanglegeofence> Rectanglegeofences { get; set; }

    public virtual DbSet<Routehistory> Routehistories { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<Vehiclesinformation> Vehiclesinformations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Circlegeofence>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("circlegeofence_pkey");

            entity.HasOne(d => d.Geofence).WithMany(p => p.Circlegeofences).HasConstraintName("circlegeofence_geofenceid_fkey");
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Driverid).HasName("driver_pkey");
        });

        modelBuilder.Entity<Geofence>(entity =>
        {
            entity.HasKey(e => e.Geofenceid).HasName("geofences_pkey");
        });

        modelBuilder.Entity<Polygongeofence>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("polygongeofence_pkey");

            entity.HasOne(d => d.Geofence).WithMany(p => p.Polygongeofences).HasConstraintName("polygongeofence_geofenceid_fkey");
        });

        modelBuilder.Entity<Rectanglegeofence>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rectanglegeofence_pkey");

            entity.HasOne(d => d.Geofence).WithMany(p => p.Rectanglegeofences).HasConstraintName("rectanglegeofence_geofenceid_fkey");
        });

        modelBuilder.Entity<Routehistory>(entity =>
        {
            entity.HasKey(e => e.Routehistoryid).HasName("routehistory_pkey");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Routehistories).HasConstraintName("routehistory_vehicleid_fkey");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Vehicleid).HasName("vehicles_pkey");
        });

        modelBuilder.Entity<Vehiclesinformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("vehiclesinformations_pkey");

            entity.HasOne(d => d.Driver).WithMany(p => p.Vehiclesinformations).HasConstraintName("vehiclesinformations_driverid_fkey");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Vehiclesinformations).HasConstraintName("vehiclesinformations_vehicleid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
