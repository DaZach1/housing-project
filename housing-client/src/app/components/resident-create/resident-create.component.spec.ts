import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResidentCreateComponent } from './resident-create.component';

describe('ResidentCreateComponent', () => {
  let component: ResidentCreateComponent;
  let fixture: ComponentFixture<ResidentCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResidentCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ResidentCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
