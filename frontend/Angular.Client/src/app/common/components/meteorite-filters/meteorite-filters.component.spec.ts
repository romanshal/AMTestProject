import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MeteoriteFiltersComponent } from './meteorite-filters.component';

describe('MeteoriteFiltersComponent', () => {
  let component: MeteoriteFiltersComponent;
  let fixture: ComponentFixture<MeteoriteFiltersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MeteoriteFiltersComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MeteoriteFiltersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
