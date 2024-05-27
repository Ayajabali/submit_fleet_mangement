import { Component, OnInit } from '@angular/core';
import { VehicleService } from '../vehicle.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-vehicle',
  templateUrl: './vehicle.component.html',
  styleUrls: ['./vehicle.component.css']
})
export class VehicleComponent implements OnInit {
  vehicles: any[] = [];
  newVehicleNumber: string = '';
  newVehicleType: string = '';
  editVehicleNumber: string = '';
  editVehicleType: string = '';
  editMode: boolean = false;
  selectedVehicle: any = null;

  constructor(private vehicleService: VehicleService, private _snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.loadVehicles();
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

  submitAddForm(form: any) {
    const vehicleInfo = {
      DicOfDic: {
        Tags: {
          VehicleNumber: this.newVehicleNumber,
          VehicleType: this.newVehicleType
        }
      }
    };

    this.vehicleService.addVehicle(vehicleInfo).subscribe(
      (response: any) => {
        console.log('Add vehicle response:', response);
        form.reset();
        this._snackBar.open('Vehicle added successfully', 'Close', {
          duration: 3000,
        });
        // Reload the vehicle list after successfully adding a new vehicle
        this.loadVehicles();
      },
      (error) => {
        console.error('Error adding vehicle:', error);
      }
    );
  }

  submitEditForm(form: any) {
    if (this.selectedVehicle) {
      const vehicleInfo = {
        DicOfDic: {
          Tags: {
            vehicleID: this.selectedVehicle.vehicleID,
            vehicleNumber: this.editVehicleNumber,
            vehicleType: this.editVehicleType
          }
        }
      };

      this.vehicleService.updateVehicle(this.selectedVehicle.vehicleID, vehicleInfo).subscribe(
        (response: any) => {
          console.log('Update vehicle response:', response);
          form.reset();
          this.editMode = false;
          this.selectedVehicle = null;
          this._snackBar.open('Vehicle updated successfully', 'Close', {
            duration: 3000,
          });
          // Reload the vehicle list after successfully updating a vehicle
          this.loadVehicles();
        },
        (error) => {
          console.error('Error updating vehicle:', error);
        }
      );
    }
  }

  editVehicle(vehicle: any) {
    this.editMode = true;
    this.selectedVehicle = vehicle;
    this.editVehicleNumber = vehicle.vehicleNumber;
    this.editVehicleType = vehicle.vehicleType;
  }

  deleteVehicle(vehicle: any) {
    if (confirm('Are you sure you want to delete this vehicle?')) {
      this.vehicleService.deleteVehicle(vehicle.vehicleID).subscribe(
        (response: any) => {
          console.log('Delete vehicle response:', response);
          this._snackBar.open('Vehicle deleted successfully', 'Close', {
            duration: 3000,
          });
          // Reload the vehicle list after successfully deleting a vehicle
          this.loadVehicles();
        },
        (error) => {
          console.error('Error deleting vehicle:', error);
        }
      );
    }
  }
}
