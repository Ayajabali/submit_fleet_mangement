import { Component, OnInit } from '@angular/core';
import { VehicleService } from '../vehicle.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { WebSocketService } from '../web-socket.service';

@Component({
  selector: 'app-vehicle-info',
  templateUrl: './vehicleinfo.component.html',
  styleUrls: ['./vehicleinfo.component.css']
})
export class VehicleinfoComponent implements OnInit {
  vehicleInfos: any[] = [];
  newVehicleId: string = '';
  newDriverId: string = '';
  newVehicleMake: string = '';
  newVehicleModel: string = '';
  newPurchaseDate: Date | null = null;
  editVehicleId: string = '';
  editDriverId: string = '';
  editVehicleMake: string = '';
  editVehicleModel: string = '';
  editPurchaseDate: Date | null = null;
  editMode: boolean = false;
  selectedVehicleInfo: any = null;
  editId: number = 0;

  constructor(private vehicleInfoService: VehicleService, private _snackBar: MatSnackBar, private webSocketService: WebSocketService) {

    this.editId = 0;
  }

  ngOnInit(): void {
    this.loadVehicleInfos();
    this.webSocketService.onMessage().subscribe((message: string) => {
      if (message == 'New vehicle information added' || message === 'New vehicle information updated') {
        this.loadVehicleInfos();
      }
    });
  }

  loadVehicleInfos() {
    this.vehicleInfoService.getVehicleInfos().subscribe(
      (response: any) => {
        if (response && response.gvar && response.gvar.dicOfDT && response.gvar.dicOfDT.VehiclesInformations) {
          this.vehicleInfos = response.gvar.dicOfDT.VehiclesInformations;
        } else {
          console.error('Response format is incorrect:', response);
        }
      },
      (error) => {
        console.error('Error fetching vehicle information:', error);
      }
    );
  }

  submitAddForm(form: any) {
    const vehicleId = parseInt(this.newVehicleId);
    const driverId = parseInt(this.newDriverId);

    if (isNaN(vehicleId)) {
      console.error('Invalid vehicle ID:', this.newVehicleId);
      return;
    }

    if (isNaN(driverId)) {
      console.error('Invalid driver ID:', this.newDriverId);
      return;
    }

    if (!this.newPurchaseDate) {
      console.error('Purchase date is null or undefined');
      return;
    }

    const purchaseDate = new Date(this.newPurchaseDate);

    if (!(purchaseDate instanceof Date && !isNaN(purchaseDate.getTime()))) {
      console.error('Invalid purchase date:', this.newPurchaseDate);
      return;
    }

    const purchaseDateEpoch = purchaseDate.getTime();
    console.log('Purchase date in epoch:', purchaseDateEpoch);

    const vehicleInfo = {
      DicOfDic: {
        Tags: {
          VehicleId: vehicleId,
          DriverId: driverId,
          VehicleMake: this.newVehicleMake,
          VehicleModel: this.newVehicleModel,
          PurchaseDate: purchaseDateEpoch
        }
      }
    };

    this.vehicleInfoService.addVehicleInfo(vehicleInfo).subscribe(
      (response: any) => {
        console.log('Add vehicle info response:', response);

        if (response.STS === 1) {
          form.reset();
          this._snackBar.open('Vehicle information added successfully', 'Close', {
            duration: 3000,
          });
          // Reload the vehicle information list after successfully adding a new entry
          this.loadVehicleInfos();
        } else {
          this._snackBar.open('Vehicle information added successfully', 'Close', {
            duration: 3000,
          });
        }
      },
      (error) => {
        console.error('Error adding vehicle information:', error);
        this._snackBar.open('Error adding vehicle information', 'Close', {
          duration: 3000,
        });
      }
    );
  }

  submitEditForm(form: any) {
    const vehicleId = parseInt(this.editVehicleId);
    const driverId = parseInt(this.editDriverId);

    if (isNaN(vehicleId)) {
      console.error('Invalid vehicle ID:', this.editVehicleId);
      return;
    }

    if (isNaN(driverId)) {
      console.error('Invalid driver ID:', this.editDriverId);
      return;
    }

    if (!this.editPurchaseDate) {
      console.error('Purchase date is null or undefined');
      return;
    }

    const purchaseDate = new Date(this.editPurchaseDate);

    if (!(purchaseDate instanceof Date && !isNaN(purchaseDate.getTime()))) {
      console.error('Invalid purchase date:', this.editPurchaseDate);
      return;
    }

    const purchaseDateEpoch = purchaseDate.getTime();
    console.log('Purchase date in epoch:', purchaseDateEpoch);

    const vehicleInfo = {
      DicOfDic: {
        Tags: {
          Id: this.editId,
          VehicleId: vehicleId,
          DriverId: driverId,
          VehicleMake: this.editVehicleMake,
          VehicleModel: this.editVehicleModel,
          PurchaseDate: purchaseDateEpoch
        }
      }
    };

    this.vehicleInfoService.updateVehicleInfo(this.editId, vehicleInfo).subscribe(
      (response: any) => {
        console.log('Update vehicle info response:', response);

        if (response.STS === 1) {
          form.reset();
          this.editMode = false;
          this.selectedVehicleInfo = null;
          this.loadVehicleInfos();
          this._snackBar.open('Vehicle information updated successfully', 'Close', {
            duration: 3000,
          });
          // Reload the vehicle information list after successfully updating an entry
          this.loadVehicleInfos();
        } else {
          this._snackBar.open('Vehicle information updated successfully', 'Close', {
            duration: 3000,
          });
        }
      },
      (error) => {
        console.error('Error updating vehicle information:', error);
        this._snackBar.open('Error updating vehicle information', 'Close', {
          duration: 3000,
        });
      }
    );
  }

  editVehicleInfo(vehicleInfo: any) {
    this.editMode = true;
    this.selectedVehicleInfo = vehicleInfo;
    this.editId = vehicleInfo.id;
    this.editVehicleId = vehicleInfo.vehicleId;
    this.editDriverId = vehicleInfo.driverId;
    this.editVehicleMake = vehicleInfo.vehicleMake;
    this.editVehicleModel = vehicleInfo.vehicleModel;
    this.editPurchaseDate = new Date(vehicleInfo.purchaseDate);
  }

  deleteVehicleInfo(vehicleInfo: any) {
    if (confirm('Are you sure you want to delete this vehicle information?')) {
      this.vehicleInfoService.deleteVehicleInfo(vehicleInfo.id).subscribe(
        (response: any) => {
          console.log('Delete vehicle info response:', response);
          this._snackBar.open('Vehicle information deleted successfully', 'Close', {
            duration: 3000,
          });
          // Reload the vehicle information list after successfully deleting an entry
          this.loadVehicleInfos();
        },
        (error) => {
          console.error('Error deleting vehicle information:', error);
        }
      );
    }
  }
}
