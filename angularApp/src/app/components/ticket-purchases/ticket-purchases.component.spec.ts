import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketPurchasesComponent } from './ticket-purchases.component';

describe('TicketPurchasesComponent', () => {
  let component: TicketPurchasesComponent;
  let fixture: ComponentFixture<TicketPurchasesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TicketPurchasesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TicketPurchasesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
