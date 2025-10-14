import { TestBed } from '@angular/core/testing';

import { RecclassService } from './recclass.service';

describe('RecclassService', () => {
  let service: RecclassService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RecclassService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
