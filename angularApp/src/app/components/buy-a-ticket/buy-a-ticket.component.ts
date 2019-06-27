import { Component, OnInit } from '@angular/core';
import { TicketService } from 'src/app/services/ticketService/ticket.service';
import { PricelistServiceService } from 'src/app/services/priceListService/pricelist-service.service';
import { TicketPricesPomModel } from 'src/app/models/ticketPricesPomModel';
import { PriceListModel } from 'src/app/models/pricelistModel';
import { UserProfileService } from 'src/app/services/userService/user-profile.service';
import { TypeModel } from 'src/app/models/typeModel';
import { TicketModel } from 'src/app/models/ticketModel';
import { AuthenticationService } from 'src/app/services/auth/authentication.service';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
//import { PayPalConfig } from 'ngx-paypal'
//import { IPayPalConfig,ICreateOrderRequest } from 'ngx-paypal';
@Component({
  selector: 'app-buy-a-ticket',
  templateUrl: './buy-a-ticket.component.html',
  styleUrls: ['./buy-a-ticket.component.css']
})
export class BuyATicketComponent implements OnInit {
  //public payPalConfig?: IPayPalConfig;
  allTicketTypes : any = [];
  ticketTypeDetail: string = "";
  selecetTT : number;
  priceList: any;
  validPrices: TicketPricesPomModel;
  price: number;
  discount: number;
  priceWDiscount: number;
  user: any;
  neregKupVremKartu : boolean= false;
  poruka: string = "";
  prikaziButtonK : boolean = false;
   typeM : any;
   EmailForPay : string = "";

  constructor(private router: Router, private ticketServ: TicketService, private pricelistServ: PricelistServiceService, private usersService: UserProfileService) {
    ticketServ.getAllTicketTypes().subscribe( data => {
      this.allTicketTypes = data;
    });
    this.pricelistServ.getPricelist().subscribe(data => {
      this.priceList = data; 
       console.log(data);
    });

    let ro = localStorage.getItem('role');
    if(ro)
    {
      if(ro == "AppUser")
      {
        this.usersService.getUserData(localStorage.getItem('name')).subscribe(data => {
        
          this.user = data;    
          console.log(this.user); 
          
           
        });
      }
      // else {
      //   this.neregKupVremKartu = true;
      // }
    }else {
      this.neregKupVremKartu = true;
    }
   }

  ngOnInit() {
    console.log("pozvan initConfig()");
    this.initConfig(); 
  }

  SelectedTicketType(event)
  {
    if(event.target.value != "" && event.target.value != "0" && this.selecetTT !=parseInt(event.target.value, 10))
    {
     this.selecetTT = parseInt(event.target.value, 10);
     this.prikaziButtonK = true;
     this.priceList.TicketPricess.forEach(element => {
       if(element.TicketTypeId == this.selecetTT)
       {
        this.price = element.Price;
        
       }
       
     });
    //  let ro = localStorage.getItem('role');
    // if(ro)
    // {
    //   if(ro == "AppUser")
    //   {

        
    //     this.prikaziButtonK = true;
    //     this.neregKupVremKartu = false;
    if(!this.neregKupVremKartu)
    {
        this.CalculateDiscount();
       
     // }
    }else{
    //  this.prikaziButtonK = false;
      this.discount = 0;
      this.priceWDiscount = this.price;
      // if(this.selecetTT == 1)
      // {
      //   this.neregKupVremKartu = true;
      //   this.poruka = "";
      // }
      // else{
      //   this.poruka = "Only signed in users can buy this type of ticket!";
      //   this.neregKupVremKartu = false;
      // }

    }
    this.initConfig();
    }
  
  }

  CalculateDiscount(){
    let uN = localStorage.getItem('name');
    
    this.ticketServ.getTypeUser(uN).subscribe(data =>{
      this.typeM = data;
      this.discount =  this.typeM.Coefficient * 100;
      this.priceWDiscount = this.price - (this.price * this.typeM.Coefficient) ;
      // if(this.typeM.Name != "Regular")
      // {
      //   if(!this.user.Activated)
      //   {
      //     window.alert("You are not authorized for this action!");
      //     this.prikaziButtonK = false;
      //   }
      // }
      
    });
  }

  Button1(t:any,form: NgForm ){
   
    let ticketMod = new TicketModel("",new Date(),0,"",0,0);
    let b = new Date();
    b.setHours(b.getHours()+ 2);
    ticketMod.PurchaseTime = new Date(b);
    console.log(new Date().getUTCHours());
    ticketMod.TicketTypeId = this.selecetTT;
    this.priceList.TicketPricess.forEach(element => {
      if(element.TicketTypeId == this.selecetTT)
      {
        ticketMod.TicketPricesId = element.Id;
      }
    });
    let ai : any;
    let ro = localStorage.getItem('role');
    if(ro)
    {
      if(ro == "AppUser")
      {
        this.usersService.getUserData(localStorage.getItem('name')).subscribe(data =>{
          ai = data;
          ticketMod.ApplicationUserId = ai.Id;
          this.ticketServ.addTicket(ticketMod).subscribe(data => {
            window.alert("Ticket successfully bought!")
            this.router.navigate(['ticketPurchases']);
          },
          err =>{
            window.alert(err.error)
            console.log(err);
          });
        });
      }
    }else{
      if(t.Email != "" && t.Email != undefined && t.Email != null){

        // let b = new Date();
        // b.setHours(b.getHours()+ 2);
        // ticketMod.PurchaseTime = new Date(b);
        // ticketMod.TicketTypeId = this.selecetTT;
        // this.priceList.TicketPricess.forEach(element => {
        //   if(element.TicketTypeId == this.selecetTT)
        //   {
        //     ticketMod.TicketPricesId = element.Id;
        //   }
        // });
        ticketMod.Name= t.Email;
      // ticketMod.ApplicationUserId = null;
        this.ticketServ.addTicket(ticketMod).subscribe( data => {
          this.ticketServ.SendMail(ticketMod).subscribe(resp =>{
            if(resp == 'Ok'){
              window.alert("Ticket successfully bought!")
             this.router.navigateByUrl('/ticketPurchases');
            }
            else{
              alert("Something went wrong");
              this.router.navigateByUrl('/home');
            }
          },
          err =>{
            window.alert(err.error)
            console.log(err);
          });
        },
        err =>{
          window.alert(err.error)
          console.log(err);
        });
      } 
      else{
      window.alert("Email is required!");
      }
   }
  }

  private initConfig(): void {
    
   
    var diffDays =this.priceWDiscount;




    // this.payPalConfig = new PayPalConfig(PayPalIntegrationType.ClientSideREST, PayPalEnvironment.Sandbox, {
    //   commit: true,
    //   client: {
    //    // sandbox: PayPalKey,
    //   },
    //   button: {
    //     label: 'paypal',
    //   },
    //   onPaymentComplete: (data, actions) => {
    //     console.log('OnPaymentComplete');
    //   },
    //   onCancel: (data, actions) => {
    //     console.log('OnCancel');
    //   },
    //   onError: (err) => {
    //     console.log('OnError');
    //   },
    //   transactions: [{
    //     amount: {
    //       currency: 'USD',
    //       total: 1 // this.vehicle.pricePerHour * diffDays * 24
    //     }
    //   }]
    // });
  }

 
}
