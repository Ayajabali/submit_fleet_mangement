import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VehicleListComponent } from './vehicle-list/vehicle-list.component';
import { VehicleDetailsComponent } from './vehicle-details/vehicle-details.component';
import { VehicleComponent } from './vehicle/vehicle.component';
import { VehicleinfoComponent } from './vehicleinfo/vehicleinfo.component';
import { EmptyPageComponent } from './empty-page/empty-page.component';
import { GeofencesInfoComponent } from './geofences-info/geofences-info.component';
import { DriverInfoComponent } from './driver-info/driver-info.component'; // Import DriverInfoComponent
import { RouteHistoryComponent } from './route-history/route-history.component'; // Import RouteHistoryComponent

const routes: Routes = [
  { path: '', redirectTo: '/vehicles', pathMatch: 'full' },
  { path: 'vehicles', component: VehicleListComponent },
  { path: 'vehicleDetails', component: VehicleDetailsComponent },
  { path: 'vehicleInfo', component: VehicleinfoComponent },
  { path: 'empty-page', component: EmptyPageComponent },
  { path: 'vehicle', component: VehicleComponent },
  { path: 'vehicleinfo', component: VehicleinfoComponent },
  { path: 'geofences-info', component: GeofencesInfoComponent },
  { path: '', redirectTo: '/empty-page', pathMatch: 'full' }, // Default route
  { path: 'driver-info', component: DriverInfoComponent }, // Add this line
  { path: 'route-history', component: RouteHistoryComponent }, // Add this line


];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
