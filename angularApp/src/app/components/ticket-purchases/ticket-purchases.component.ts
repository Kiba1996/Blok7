import { Component, OnInit } from '@angular/core';
import { TicketService } from 'src/app/services/ticketService/ticket.service';

@Component({
  selector: 'app-ticket-purchases',
  templateUrl: './ticket-purchases.component.html',
  styleUrls: ['./ticket-purchases.component.css']
})
export class TicketPurchasesComponent implements OnInit {

  userName : string = "";
  boolic: boolean = false;
  tickets: any  = [];
  constructor(private ticketServ: TicketService) {
    this.userName = localStorage.getItem('name');
    if(this.userName == "" || this.userName == null)
    {
      this.boolic = false;
    }
    ticketServ.getTickets(this.userName).subscribe(data => {
      this.tickets = data;
      this.boolic = true;
    },
    err =>{
      window.alert(err.error);
      this.boolic = false;
    });
  }

  ngOnInit() {
  }

}
