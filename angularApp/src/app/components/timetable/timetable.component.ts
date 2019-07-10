import { Component, OnInit } from '@angular/core';
import { LineServiceService } from 'src/app/services/lineService/line-service.service';
import { TimetableService } from 'src/app/services/timetableService/timetable.service';

@Component({
  selector: 'app-timetable',
  templateUrl: './timetable.component.html',
  styleUrls: ['./timetable.component.css']
})
export class TimetableComponent implements OnInit {
  allLines: any = [];
  allDayTypes: any = [];
  allTimetables: any = [];
  showList: any = [];
  dt: any;
  selectedDay : boolean  = false;
  lineIdChoosen: number;
  boolic: boolean = false;
  stringovi1 : string[] = [];
  constructor(private lineServ: LineServiceService, private timetableServ: TimetableService) { 
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

  SelectedDaytype(event: any)
  {
    this.dt = event.target.value;
    console.log(this.dt);
    if(this.dt == 0){
      this.selectedDay = false;
    }else{
      this.selectedDay = true;
      
        this.GetLineIds();
      
     
    }
  }

  GetLineIds()
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

  SelectedLine(event: any): void
{
  this.lineIdChoosen = event.target.value;
  if(event.target.value != 0){
 
    
    this.boolic = true;
    
      this.SplitDepartures();
  }
  
}

SplitDepartures(){

  this.stringovi1 = [];
  
  this.allTimetables.forEach(element => {
    if(element.LineId == this.lineIdChoosen && element.DayTypeId == this.dt)
    {
      this.stringovi1= element.Departures.split(";");
      this.stringovi1.splice(this.stringovi1.length-1,1);
      
    }
    
  });

}

}
