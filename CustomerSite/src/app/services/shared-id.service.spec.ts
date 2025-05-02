import { TestBed } from '@angular/core/testing';

import { SharedIdService } from './shared-id.service';

describe('SharedIdService', () => {
  let service: SharedIdService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SharedIdService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
