import { Component, OnInit } from '@angular/core';
import { VehicleService } from '../vehicle.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-vehicle-list',
  templateUrl: './vehicle-list.component.html',
  styleUrls: ['./vehicle-list.component.css']
})
export class VehicleListComponent implements OnInit {
  vehicles: any[] = [];
  drivers: any[] = [];

  constructor(private vehicleService: VehicleService, private router: Router) { }

  ngOnInit(): void {
    this.loadVehicles();
    this.loadDrivers();
  }

  loadVehicles() {
    this.vehicleService.getVehicles().subscribe(
      (response: any) => {
        this.vehicles = response.gvar.dicOfDT.Vehicles;
      },
      (error) => {
        console.error('Error fetching vehicles:', error);
      }
    );
  }

  loadDrivers() {
    this.vehicleService.getDrivers().subscribe(
      (response: any) => {
        this.drivers = response.gvar.dicOfDT.Drivers;
      },
      (error) => {
        console.error('Error fetching drivers:', error);
      }
    );
  }

  showMore(vehicleId: number) {
    this.vehicleService.getVehicleDetails(vehicleId).subscribe(
      (response: any) => {
        const vehicleDetails = response.gvar.dicOfDic.VehiclesInformation;

        if (vehicleDetails) {
          const driverName = vehicleDetails.DriverName;
          const driver = this.drivers.find(d => d.name === driverName);
          const driverId = driver ? driver.id : 'Unknown';

          const details = `
            VehicleNumber: ${vehicleDetails.VehicleNumber}\n
            VehicleType: ${vehicleDetails.VehicleType}\n
            DriverName: ${driverName}\n
            PhoneNumber: ${vehicleDetails.PhoneNumber}\n
            LastPosition: ${vehicleDetails.LastPosition}\n
            VehicleMake: ${vehicleDetails.VehicleMake}\n
            VehicleModel: ${vehicleDetails.VehicleModel}\n
            LastGPSTime: ${vehicleDetails.LastGPSTime}\n
            LastGPSSpeed: ${vehicleDetails.LastGPSSpeed}\n
            LastAddress: ${vehicleDetails.LastAddress}
          `;

          alert(details);
        } else {
          alert('Vehicle details not found.');
        }
      },
      (error) => {
        console.error('Error fetching vehicle details:', error);
      }
    );
  }

  delete(vehicleId: number) {
    this.vehicleService.delete(vehicleId).subscribe(
      (response: any) => {
        // Reload vehicles after deletion
        this.loadVehicles();
      },
      (error) => {
        console.error('Error deleting vehicle:', error);
      }
    );
  }

  openNewPage(): void {
    this.router.navigate(['/empty-page']);
  }

  openGeofencesPage(): void {
    this.router.navigate(['/geofences-info']);
  }
  openDriverPage(): void {
    this.router.navigate(['/driver-info']); // Navigate to the driver information page
  }
  openRouteHistoryPage(): void {
    this.router.navigate(['/route-history']); // Navigate to the RouteHistory page
  }
}
