import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VehicleinfoComponent } from './vehicleinfo.component';

describe('VehicleinfoComponent', () => {
  let component: VehicleinfoComponent;
  let fixture: ComponentFixture<VehicleinfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [VehicleinfoComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(VehicleinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
