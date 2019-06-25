import { Component, OnInit } from '@angular/core';
import { PricelistServiceService } from 'src/app/services/priceListService/pricelist-service.service';
import { PriceListModel } from 'src/app/models/pricelistModel';
import { NgForm } from '@angular/forms';
import { TicketPricesPomModel } from 'src/app/models/ticketPricesPomModel';
import { TicketPricessModel } from 'src/app/models/ticketPriceModel';

@Component({
  selector: 'app-add-change-pricelist',
  templateUrl: './add-change-pricelist.component.html',
  styleUrls: ['./add-change-pricelist.component.css']
})
export class AddChangePricelistComponent implements OnInit {
priceList: any;
ticketPricesPom: TicketPricesPomModel = new TicketPricesPomModel(0,0,0,0,0,new PriceListModel(null,null,0, []));
datumVazenjaBool: boolean = false;
validPrices: TicketPricesPomModel;

  constructor( private pricelistServ: PricelistServiceService) { 
    this.refresh();
     
  }

  ngOnInit() {
  }

  onSubmit(pm: PriceListModel, form: NgForm){
  let priceL : any;
  let bol : boolean = false;
  this.ticketPricesPom.PriceList = pm;
//  let a : Date = new Date(Date.now());
//   if(pm.StartOfValidity.toString() == ""  || pm.EndOfValidity.toString() == '' || pm.StartOfValidity == undefined || pm.StartOfValidity == null || pm.EndOfValidity == undefined || pm.EndOfValidity == null)
//   {
//     window.alert("Start or End of validiti can't be empty!");
//     form.reset();
//     //this.refresh();

//   }
//   else if(pm.StartOfValidity> pm.EndOfValidity)
//   {
//     window.alert("Start of validiti id bigger than End of validity!");
//     form.reset();
//     //this.refresh();
//   }
  
  // else{
    this.pricelistServ.addPricelist(this.ticketPricesPom).subscribe(data =>
      {
        window.alert("Timetable successfully added!");
        this.refresh();
      },
      err => {
        window.alert(err.error);
        this.refresh();
      });
  // }
  

  }
  onSubmit1(pm: TicketPricesPomModel, form: NgForm){
   if(pm.Daily == null){
      pm.Daily = 0;
    }
    if(pm.Hourly == null){
      pm.Hourly = 0;
    }
    if(pm.Monthly == null){
      pm.Monthly = 0;
    }
    if(pm.Yearly == null){
      pm.Yearly = 0;
    }
    this.ticketPricesPom = pm;
    this.datumVazenjaBool = true;
    
  }
  refresh(){
    this.ticketPricesPom  = new TicketPricesPomModel(0,0,0,0,0,new PriceListModel(null,null,0, []));
     this. datumVazenjaBool = false;
   
    this.pricelistServ.getPricelist().subscribe(data => {
      
      this.priceList = data; 
       console.log(data);
      
       this.validPrices = new TicketPricesPomModel(0,0,0,0,0,new PriceListModel(null,null,0, []))
       if(this.priceList){
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

}
