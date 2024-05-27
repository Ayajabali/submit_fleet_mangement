import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-empty-page',
  templateUrl: './empty-page.component.html',
  styleUrls: ['./empty-page.component.css']
})
export class EmptyPageComponent {
  constructor(private router: Router) { }

  navigateToVehicle(): void {
    this.router.navigate(['/vehicle']);
  }

  navigateToVehicleInfo(): void {
    this.router.navigate(['/vehicleinfo']);
  }
}
