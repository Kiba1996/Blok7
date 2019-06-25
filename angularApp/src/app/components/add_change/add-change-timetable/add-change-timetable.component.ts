import { Component, OnInit } from '@angular/core';
import { LineServiceService } from 'src/app/services/lineService/line-service.service';
import { TimetableService } from 'src/app/services/timetableService/timetable.service';
import { TimetableModel } from 'src/app/models/timetableModel';
import { DayTypeModel } from 'src/app/models/dayTypeModel';
import { VehicleModel } from 'src/app/models/vehicleModel';
import { VehicleService } from 'src/app/services/vehicleService/vehicle.service';
import { Time } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-change-timetable',
  templateUrl: './add-change-timetable.component.html',
  styleUrls: ['./add-change-timetable.component.css']
})
export class AddChangeTimetableComponent implements OnInit {
selected: string = "";
selectedDay: boolean = false;
allLines: any = [];
allDayTypes: any = [];
allTimetables: any = [];
showList: any = [];
dt: any;

br: number = 0;
dayTypeChosen : number;
lineIdChoosen: number;
boolic: boolean = false;
duzinaStringovi: boolean = false;
stringovi: string[]  = [];
stringovi1: string[] = [];
depart: string = "";
vehicleId: any;
availableVehicles: any = [];
chooseVehicle: boolean = false;
ttZaDodavanje : TimetableModel = new TimetableModel("", 0, 0,0);
daySelected : string = "0";
lineSelected: string = "0";
MyInput: Time;


  constructor(private router: Router,private lineServ: LineServiceService, private timetableServ: TimetableService,private vehicleServ: VehicleService) { 
    this.lineServ.getAllLines().subscribe(data => {
      this.allLines = data;
      console.log(data);
    });

    this.timetableServ.getAllDayTypes().subscribe(data => {
      this.allDayTypes = data;
      console.log(data);
    });
    this.timetableServ.getAllTimetables().subscribe(data => {
      this.allTimetables = data;
      console.log(data);
    });


  }

  ngOnInit() {
  }


  setradio(e: string): void   
  {  
    this.selected = e;  
    this.boolic = false;
    
    this.refresh();
       
  }  
   
  isSelected(name: string): boolean   
  {  
    if (!this.selected) { 
        return false;  
    }  
    return (this.selected === name); 
  } 

  SelectedDaytype(event: any)
  {
    this.dt = event.target.value;
    console.log(this.dt);
    if(this.dt == 0){
      this.selectedDay = false;
      this.boolic = false;
    }else{
      this.selectedDay = true;
      if(this.selected == "Add")
      {
        this.getLineIds();
      }
      if(this.selected == "Change")
      {
        this.getLineIdsForChange();
      }
     
    }
  }

getLineIdsForChange()
{
  let ll: any =[];
  let k : boolean = false;
  this.showList = [];
  if(this.allTimetables === void 0){

    this.showList = [];
  }
  else{
   
    this.allTimetables.forEach(element => {
     
      if(element.DayTypeId == this.dt)
      {
        ll.push(element);
      }
    });

    if(ll === void 0 || ll == [])
    {
      this.showList = [];
    }
    else {
      this.allLines.forEach(element => {
        k = false;
        
        ll.forEach(d => {
          if(d.LineId == element.Id)
          {
            k = true;
          }
        });
    
       if(k ) {
          this.showList.push(element);
       }
      
      });
    }

  }
}
  
getLineIds(){
  let list: any = [];
  let k: boolean = false;
  let ll: any =[];
  
  if(this.allTimetables === void 0){
    this.showList = this.allLines;
  }
  else{
    this.allTimetables.forEach(element => {
      if(element.DayTypeId == this.dt)
      {
        ll.push(element);
      }
    });
   
    if(ll === void 0 || ll == [])
    {
      this.showList = this.allLines;
    }
    else {
      this.showList = [];
      this.allLines.forEach(element => {
        k = false;

        ll.forEach(d => {
          if(d.LineId == element.Id)
          {
            k = true;
          }
        });
    
      if(!k ) {
          this.showList.push(element);
      }
      
      });
    }
  }
  
}

SelectedLine(event: any): void
{
  this.lineIdChoosen = event.target.value;
  if(event.target.value != 0){
    let k = parseInt(event.target.value,10);
    this.lineServ.FindVehicleId(k).subscribe(data1 =>{
    
        this.vehicleId = data1;
        if(this.vehicleId != -1){
          this.chooseVehicle = false;
        }
        else{
          this.vehicleServ.GetAllAvailableVehicles().subscribe(data =>{
            this.availableVehicles = data;
            if(this.availableVehicles == null || this.availableVehicles.length == 0 || this.availableVehicles== undefined)
            {
              window.alert("No available vehicles! You will be redirected to add vehicle page!");
              this.router.navigateByUrl("/add_change_vehicle");
            }
            this.chooseVehicle = true;
          });
        }
      
    });
   
    this.boolic = true;
    if(this.selected == "Change")
    {
      this.SplitDepartures();
    }
  }
    else{
      this.boolic = false;
    }
  }
  


SelectedVehicle(event: any): void
{
  this.vehicleId = event.target.value;
}
SplitDepartures(){

  this.stringovi1 = [];
  this.ttZaDodavanje = new TimetableModel("",0,0,0);
  this.allTimetables.forEach(element => {
    if(element.LineId == this.lineIdChoosen && element.DayTypeId == this.dt)
    {
      this.ttZaDodavanje = element;
    }
    
  });

  this.stringovi1= this.ttZaDodavanje.Departures.split(";");
  this.stringovi1.splice(this.stringovi1.length-1,1);
  this.stringovi = this.stringovi1;

}
addTime(n:any){
 let dodajIsto = false;
  if(n != undefined && n!= ""){
  console.log(n);
  if(this.stringovi.length > 0){
    this.stringovi.forEach(element =>{
      if(element == n)
      {
        dodajIsto = true;
        window.alert("Cant add same time!");
        return;
      }
    })
  }
  if(!dodajIsto){
    this.stringovi.push(n);
    this.stringovi.sort((a,b)=> a.localeCompare(b));
    this.stringovi1 = this.stringovi;
  }
  
  }else{
    window.alert("You have to choose time!");
  }
}

ChangeTimetable()
{
  this.ttZaDodavanje.DayTypeId = this.dt;
  this.ttZaDodavanje.LineId = this.lineIdChoosen;
  let stringZaDodavanje : string = "";
  this.stringovi1.forEach(x => {
    stringZaDodavanje = stringZaDodavanje + x + ";";
  });
  this.ttZaDodavanje.Departures = stringZaDodavanje;
 
  this.timetableServ.changeTimetable(this.ttZaDodavanje.Id,this.ttZaDodavanje).subscribe(data=>
    {
      window.alert("Timetable successfully changed!");
      this.refresh();
    },
    err => {
      window.alert(err.error);
      this.refresh();
      });
  
}

AddTimetable(){
  this.ttZaDodavanje.DayTypeId = this.dt;
  this.ttZaDodavanje.LineId = this.lineIdChoosen;
  let stringZaDodavanje : string = "";
  this.stringovi1.forEach(x => {
    stringZaDodavanje = stringZaDodavanje + x + ";";
  });
  this.ttZaDodavanje.Departures = stringZaDodavanje;
  
  this.ttZaDodavanje.Vehicles.push(new VehicleModel(this.vehicleId));

  this.timetableServ.addTimetable(this.ttZaDodavanje).subscribe(data => {
    window.alert("Timetable successfully added!");
    this.refresh();
  },
  err => {
    window.alert(err.error);
    this.refresh();
    });
}
removeFromTimes(st,i){
  this.stringovi1.splice(i,1);
  this.stringovi = this.stringovi1;
  
}
DeleteTimetable()
{
  
  this.timetableServ.deleteTimetable(this.ttZaDodavanje.Id).subscribe(data =>{
    window.alert("Timetable successfully deleted!");
    this.refresh();
  },
  err => {
    window.alert(err.error);
    this.refresh();
   

  });
}

refresh(){
  this.ttZaDodavanje = new TimetableModel("",0,0,0);
  this.selectedDay = false;
  this.daySelected = "0";
  this.lineSelected = "0";
  this.boolic = false;
  this.stringovi = [];
  this.stringovi1 = [];
  this.allTimetables= [];
  this.vehicleId =  0;
  this.chooseVehicle = false;
  this.timetableServ.getAllTimetables().subscribe(data => {
    this.allTimetables = data;
    console.log(data);
  });
}

}
