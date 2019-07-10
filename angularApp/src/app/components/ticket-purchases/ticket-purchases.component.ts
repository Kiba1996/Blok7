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
  blaa: any = [];
  constructor(private ticketServ: TicketService) {
    this.userName = localStorage.getItem('name');
    if(this.userName == "" || this.userName == null)
    {
      this.boolic = false;
    }
    ticketServ.getTickets(this.userName).subscribe(data => {
      this.tickets = data;
      this.boolic = true;
      this.tickets.forEach(element => {
        let d : Date = new Date(element.PurchaseTime);
        let mesec : number = d.getMonth() + 1;
        let m : string = "";
        m = m+ d.getDate().toString() + ".";
        m = m+ mesec.toString() + ".";
        m = m + d.getFullYear().toString() + "." + "  ";
        m = m + d.getHours().toString() + ":";
        m = m + d.getMinutes().toString();

        this.blaa.push(m);
      });
      this.tickets.reverse();
      this.blaa.reverse();
    },
    err =>{
      window.alert(err.error);
      this.boolic = false;
    });
  }

  ngOnInit() {
  }

}
