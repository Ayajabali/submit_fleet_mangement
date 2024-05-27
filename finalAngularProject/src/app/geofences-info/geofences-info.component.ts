// geofences-info.component.ts
import { Component, OnInit } from '@angular/core';
import { VehicleService } from '../vehicle.service';

@Component({
  selector: 'app-geofences-info',
  templateUrl: './geofences-info.component.html',
  styleUrls: ['./geofences-info.component.css']
})
export class GeofencesInfoComponent implements OnInit {
  geofences: any[] = [];

  constructor(private vehicleService: VehicleService) { }

  ngOnInit(): void {
    this.loadGeofences();
  }

  loadGeofences(): void {
    this.vehicleService.getAllGeofences().subscribe(
      (response: any) => {
        if (response && response.sts === 1 && response.gvar && response.gvar.dicOfDT) {
          this.geofences = response.gvar.dicOfDT.Geofences;
        } else {
          // Handle error or empty response
        }
      },
      (error: any) => {
        console.error('Error fetching geofences:', error);
        // Handle error
      }
    );
  }
}
