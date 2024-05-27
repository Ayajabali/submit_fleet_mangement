import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  private apiUrl = 'https://localhost:7116/api'; // Replace with your actual API URL

  constructor(private http: HttpClient) { }
  delete(vehicleId: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/vehicle/${vehicleId}`);
  }
  getVehicles(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/vehicle`);
  }

  getVehicleDetails(vehicleId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/Vehiclesinformation/${vehicleId}`);
  }

  getDrivers(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/Driver`);
  }

  addVehicle(vehicle: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/vehicle`, vehicle);
  }

  updateVehicle(vehicleId: number, vehicle: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/vehicle/${vehicleId}`, vehicle);
  }

  deleteVehicle(vehicleId: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/vehicle/${vehicleId}`);
  }

  getVehicleInfos(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/Vehiclesinformation`);
  }

  addVehicleInfo(vehicleInfo: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/Vehiclesinformation`, vehicleInfo);
  }

  updateVehicleInfo(id: number, vehicleInfo: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/Vehiclesinformation/${id}`, vehicleInfo);
  }

  deleteVehicleInfo(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/Vehiclesinformation/${id}`);
  }
  getAllGeofences(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/geofence`);
  }


  addDriver(driverInfo: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/driver`, driverInfo);
  }

  updateDriver(driverId: number, driverInfo: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/driver/${driverId}`, driverInfo);
  }

  deleteDriver(driverId: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/driver/${driverId}`);
  }


  // Add RouteHistory specific methods
  getAllRouteHistories(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/Routehistory`);
  }

  getRouteHistoryByVehicle(vehicleId: number, startTimeEpoch: number, endTimeEpoch: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/Routehistory/${vehicleId}/${startTimeEpoch}/${endTimeEpoch}`);
  }

  addRouteHistory(routeHistory: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/Routehistory`, { DicOfDic: { Tags: routeHistory } });
  }
}
