import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MeteoriteTableComponent } from './meteorite-table.component';

describe('MeteoriteTableComponent', () => {
  let component: MeteoriteTableComponent;
  let fixture: ComponentFixture<MeteoriteTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MeteoriteTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MeteoriteTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
