import { Component, Input } from '@angular/core';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { VehicleService } from '../vehicle.service';

@Component({
  selector: 'app-vehicle-details',
  templateUrl: './vehicle-details.component.html',
  styleUrls: ['./vehicle-details.component.css']
})
export class VehicleDetailsComponent {
  @Input() vehicleId!: number;

  vehicleDetails: any = {};
  drivers: any[] = [];

  constructor(private vehicleService: VehicleService, private modalService: NgbModal) { }

  open(content: NgbModalRef) {
    this.loadVehicleDetails();
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  loadVehicleDetails() {
    this.vehicleService.getVehicleDetails(this.vehicleId).subscribe(
      (response: any) => {
        this.vehicleDetails = response.gvar.dicOfDic.VehiclesInformation;
      },
      (error: any) => {
        console.error('Error fetching vehicle details:', error);
      }
    );

    this.vehicleService.getDrivers().subscribe(
      (response: any) => {
        this.drivers = response.gvar.dicOfDT.Drivers;
      },
      (error: any) => {
        console.error('Error fetching drivers:', error);
      }
    );
  }
}
