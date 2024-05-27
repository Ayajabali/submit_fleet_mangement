import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { VehicleListComponent } from './vehicle-list/vehicle-list.component';
import { VehicleDetailsComponent } from './vehicle-details/vehicle-details.component';
import { AppRoutingModule } from './app-routing.module';
import { MatIconModule } from '@angular/material/icon';
import { VehicleComponent } from './vehicle/vehicle.component';
import { VehicleinfoComponent } from './vehicleinfo/vehicleinfo.component';
import { GeofencesInfoComponent } from './geofences-info/geofences-info.component';
import { DriverInfoComponent } from './driver-info/driver-info.component';
import { RouteHistoryComponent } from './route-history/route-history.component';


@NgModule({
  declarations: [
    AppComponent,
    VehicleListComponent,
    VehicleDetailsComponent,
    VehicleComponent,
    VehicleinfoComponent,
    GeofencesInfoComponent,
    DriverInfoComponent,
    RouteHistoryComponent,

  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    MatTableModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule, // Import ReactiveFormsModule here
    AppRoutingModule,// Import AppRoutingModule here
    MatIconModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
