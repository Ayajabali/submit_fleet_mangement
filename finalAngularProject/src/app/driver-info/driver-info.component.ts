import { Component, OnInit } from '@angular/core';
import { VehicleService } from '../vehicle.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-driver-info',
  templateUrl: './driver-info.component.html',
  styleUrls: ['./driver-info.component.css']
})
export class DriverInfoComponent implements OnInit {
  drivers: any[] = [];
  newDriverName: string = '';
  newDriverPhoneNumber: string = '';
  editDriverName: string = '';
  editDriverPhoneNumber: string = '';
  editMode: boolean = false;
  selectedDriver: any = null;

  constructor(private vehicleService: VehicleService, private _snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.loadDrivers();
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

  submitAddForm(form: any) {
    const driverInfo = {
      DicOfDic: {
        Tags: {
          DriverName: this.newDriverName,
          PhoneNumber: this.newDriverPhoneNumber
        }
      }
    };

    this.vehicleService.addDriver(driverInfo).subscribe(
      (response: any) => {
        console.log('Add driver response:', response);
        if (response.sts === 1) {
          form.reset();
          this._snackBar.open('Driver added successfully', 'Close', {
            duration: 3000,
          });
          // Reload the driver list after successfully adding a new driver
          this.loadDrivers();
        } else {
          console.error('Error adding driver: Invalid response');
        }
      },
      (error) => {
        console.error('Error adding driver:', error);
      }
    );
  }


  submitEditForm(form: any) {
    if (this.selectedDriver) {
      const driverInfo = {
        DicOfDic: {
          Tags: {
            Driverid: this.selectedDriver.driverID, // Ensure this property name matches the backend
            Drivername: this.editDriverName,
            Phonenumber: this.editDriverPhoneNumber
          }
        }
      };

      this.vehicleService.updateDriver(this.selectedDriver.driverID, driverInfo).subscribe(
        (response: any) => {
          console.log('Update driver response:', response);
          if (response.sts === 1) {
            this.selectedDriver.driverName = this.editDriverName;
            this.selectedDriver.phoneNumber = this.editDriverPhoneNumber;

            form.reset();
            this.editMode = false;
            this.selectedDriver = null;
            this._snackBar.open('Driver updated successfully', 'Close', {
              duration: 3000,
            });
          } else {
            console.error('Error updating driver: Invalid response');
          }
        },
        (error) => {
          console.error('Error updating driver:', error);
        }
      );
    }
  }


  editDriver(driver: any) {
    this.editMode = true;
    this.selectedDriver = driver;
    this.editDriverName = driver.driverName;
    this.editDriverPhoneNumber = driver.phoneNumber;
  }

  deleteDriver(driver: any) {
    if (confirm('Are you sure you want to delete this driver?')) {
      // Ensure that the driver object has a valid driverID property before proceeding
      if (driver.driverID) {
        this.vehicleService.deleteDriver(driver.driverID).subscribe(
          (response: any) => {
            console.log('Delete driver response:', response);
            if (response.sts === 1) {
              this.drivers = this.drivers.filter(d => d.driverID !== driver.driverID);
              this._snackBar.open('Driver deleted successfully', 'Close', {
                duration: 3000,
              });
            } else {
              console.error('Error deleting driver: Invalid response');
            }
          },
          (error) => {
            console.error('Error deleting driver:', error);
          }
        );
      } else {
        console.error('Error deleting driver: Invalid driverID');
      }
    }
  }
}
