import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { VehicleModel } from 'src/app/models/vehicleModel';
import { VehicleService } from 'src/app/services/vehicleService/vehicle.service';

@Component({
  selector: 'app-add-change-vehicle',
  templateUrl: './add-change-vehicle.component.html',
  styleUrls: ['./add-change-vehicle.component.css']
})
export class AddChangeVehicleComponent implements OnInit {
  private selected: string="";
  availableVehicles : any = [];
  NemaSobonihVozila : boolean = false;
  idVozila: string = "";

  constructor(private vehicleServ: VehicleService) {
    this.vehicleServ.GetAllAvailableVehicles().subscribe(data =>{
      this.availableVehicles = data;
      if(this.availableVehicles == null || this.availableVehicles.length == 0 || this.availableVehicles== undefined)
      {
        this.NemaSobonihVozila = true;
      }
      
    });
   }

  ngOnInit() {
  }
  onSubmit(vehicleData: VehicleModel, form: NgForm){
   
    if(this.selected == "Add")
    {
      console.log(vehicleData)
      this.vehicleServ.addVehicle(vehicleData).subscribe(data=>{
        window.alert("Vehicle successfully added!");
        form.reset();
        this.refresh();
      },
      err => {
        window.alert(err.error);
        this.refresh();
      });
    }
    else if(this.selected == "Remove"){
      
      if(this.idVozila == "" || this.idVozila == null)
      {
        vehicleData.Id = 0;
      }
      this.vehicleServ.deleteVehicle(vehicleData.Id).subscribe(data => {
        window.alert("Vehicle successfully removed!");
        form.reset();
        this.refresh();
      },
      err => {
        window.alert(err.error);
        this.refresh();
      });
      }
    else{
      console.log("lalallaa")
    }
  }

  setradio(e: string): void   
  {  
    this.selected = e;
    this.refresh();      
  }  

  isSelected(name: string): boolean   
  { 
    if (!this.selected) { 
        return false;  
    }  
    return (this.selected === name); 
  } 

  refresh(){
    this.NemaSobonihVozila = false;
    this.idVozila = "";
    this.vehicleServ.GetAllAvailableVehicles().subscribe(data =>{
      this.availableVehicles = data;
      if(this.availableVehicles == null || this.availableVehicles.length == 0 || this.availableVehicles== undefined)
      {
        this.NemaSobonihVozila = true;
      }
     });
  }

}
