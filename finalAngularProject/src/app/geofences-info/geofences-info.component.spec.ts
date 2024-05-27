import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GeofencesInfoComponent } from './geofences-info.component';

describe('GeofencesInfoComponent', () => {
  let component: GeofencesInfoComponent;
  let fixture: ComponentFixture<GeofencesInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GeofencesInfoComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GeofencesInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
