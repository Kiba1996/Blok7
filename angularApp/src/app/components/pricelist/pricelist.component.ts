import { Component, OnInit } from '@angular/core';
import { PricelistServiceService } from 'src/app/services/priceListService/pricelist-service.service';
import { TicketPricesPomModel } from 'src/app/models/ticketPricesPomModel';
import { PriceListModel } from 'src/app/models/pricelistModel';

@Component({
  selector: 'app-pricelist',
  templateUrl: './pricelist.component.html',
  styleUrls: ['./pricelist.component.css']
})
export class PricelistComponent implements OnInit {
  priceList: any;
  endOfvalidity: string = "";
  ticketPricesPom: TicketPricesPomModel = new TicketPricesPomModel(0,0,0,0,0,new PriceListModel(new Date(),new Date(),0, []));
  datumVazenjaBool: boolean = false;
  validPrices: TicketPricesPomModel;
    constructor( private pricelistServ: PricelistServiceService) { 
      this.pricelistServ.getPricelist().subscribe(data => {
        
        this.priceList = data; 
         console.log(data);
        
         this.validPrices = new TicketPricesPomModel(0,0,0,0,0,new PriceListModel(new Date(),new Date(),0, []))
         if(this.priceList){
          this.endOfvalidity = this.priceList.EndOfValidity.split('T')[0];
         this.priceList.TicketPricess.forEach(element => {
          if(element.TicketTypeId == 2)
          {
            this.validPrices.Daily = element.Price;
          }
          if(element.TicketTypeId == 1)
          {
            this.validPrices.Hourly = element.Price;
          }
          if(element.TicketTypeId == 3)
          {
            this.validPrices.Monthly = element.Price;
          }
          if(element.TicketTypeId == 4)
          {
            this.validPrices.Yearly = element.Price;
          }
          
        });
      }
      });
       
    }

  ngOnInit() {
  }

}
