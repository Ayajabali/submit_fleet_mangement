import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { VehicleService } from '../vehicle.service';

@Component({
  selector: 'app-route-history',
  templateUrl: './route-history.component.html',
  styleUrls: ['./route-history.component.css']
})
export class RouteHistoryComponent implements OnInit {
  vehicleNumbers: any[] = [];
  selectedVehicleId: number = 0;
  routeHistories: any[] = [];
  newRouteHistory: any = {
    VehicleID: null,
    VehicleDirection: 0,
    Status: '0',
    VehicleSpeed: '',
    Epoch: null,
    Address: '',
    Latitude: null,
    Longitude: null
  };

  constructor(private vehicleService: VehicleService, private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.loadVehicleNumbers();
  }

  loadVehicleNumbers(): void {
    this.vehicleService.getVehicles().subscribe(response => {
      console.log('Vehicle numbers response:', response); // Log the response for debugging
      if (response.sts === 1 && response.gvar && response.gvar.dicOfDT && Array.isArray(response.gvar.dicOfDT.Vehicles)) {
        this.vehicleNumbers = response.gvar.dicOfDT.Vehicles; // Access the nested array
      } else {
        console.error('Unexpected response structure', response);
      }
    }, error => {
      console.error('Error loading vehicle numbers', error); // Handle error
    });
  }

  loadRouteHistories(): void {
    if (this.selectedVehicleId) {
      const startTimeEpoch = 0; // Replace with actual start time epoch
      const endTimeEpoch = Math.floor(Date.now() / 1000); // Replace with actual end time epoch (current time in seconds)

      this.vehicleService.getRouteHistoryByVehicle(this.selectedVehicleId, startTimeEpoch, endTimeEpoch).subscribe(response => {
        console.log('Route histories response:', response); // Log the response for debugging
        if (response.sts === 1 && response.gvar && response.gvar.dicOfDT && response.gvar.dicOfDT.RouteHistory) {
          this.routeHistories = response.gvar.dicOfDT.RouteHistory; // Assign route histories from the response
        } else {
          console.error('Unexpected response structure', response);
        }
      }, error => {
        console.error('Error loading route histories', error); // Handle error
      });
    }
  }




  addRouteHistory(): void {
    this.newRouteHistory.VehicleID = this.selectedVehicleId;
    this.vehicleService.addRouteHistory(this.newRouteHistory).subscribe(response => {
      console.log('Add route history response:', response); // Log the response for debugging
      if (response.sts === 1) {
        this.loadRouteHistories();
        this.snackBar.open('Route history added successfully', 'Close', { duration: 3000 });
      } else {
        console.error('Unexpected response structure', response);
        this.snackBar.open('Failed to add route history', 'Close', { duration: 3000 });
      }
    }, error => {
      console.error('Error adding route history', error); // Handle error
      this.snackBar.open('Error adding route history', 'Close', { duration: 3000 });
    });
  }

  formatDate(epoch: number): string {
    const date = new Date(epoch * 1000); // Convert epoch to milliseconds
    return date.toLocaleString(); // Adjust format as needed
  }
}
