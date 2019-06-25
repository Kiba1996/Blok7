import { Component, OnInit, NgZone, Output,Directive ,EventEmitter,Input, OnChanges,SimpleChanges} from '@angular/core';
import { Polyline } from 'src/app/models/map/polyliner';
import { GeoLocation } from 'src/app/models/map/geolocation';
import { MapsAPILoader, MouseEvent } from '@agm/core';
import { StationServiceService } from 'src/app/services/stationService/station-service.service';
import { MarkerInfo } from 'src/app/models/map/marker-info.model';
import { StationModel } from 'src/app/models/stationModel';
import { NgForm } from '@angular/forms';
import { LineModel } from 'src/app/models/lineModel';
import { LineServiceService } from 'src/app/services/lineService/line-service.service';
import { AddLinesValidation } from 'src/app/models/Validation/validationModels';
import { Alert } from 'selenium-webdriver';
import { element } from 'protractor';


@Component({
  selector: 'app-add-change-lines',
  templateUrl: './add-change-lines.component.html',
  styleUrls: ['./add-change-lines.component.css'],
  styles: ['agm-map {height: 500px; width: 700px;}']
})
export class AddChangeLinesComponent implements OnInit {
  public polyline: Polyline;
  sl: LineModel = new LineModel(0,"",[],"");
  selektovanaLinijaZaIzmenu: LineModel = new LineModel(0,"",[],"");
  selLine: Polyline;
  id: number;
  idForRemove: number;
  selectedL: string = "none";
  selected: string = "";
  public zoom: number;
  stati: any = [];
  drugiMarkeriStati: any = [];
  markerInfo: MarkerInfo;
  pomStat: StationModel;
  allLines: any = [];
  selectedStations: StationModel[] = [];
  public latitude: number;
  public longitude: number;
  markerZaDodavanje: StationModel;
  boolic: boolean = false;
  boolZaAktivanRadio = true;
  boolZaMarkerZaDodavanje : boolean = false;
  LineSelected : string = "none";
  iconPath : any = { url:"assets/busicon.png", scaledSize: {width: 50, height: 50}}
  validations: AddLinesValidation = new AddLinesValidation();

  errorForListStat:string = "";
  
  constructor(private ngZone: NgZone, private mapsApiLoader : MapsAPILoader , private statServ: StationServiceService, private lineServ: LineServiceService) { 
    this.statServ.getAllStations().subscribe(data => {
      this.stati = data;
      this.drugiMarkeriStati = data;
      });

      this.lineServ.getAllLines().subscribe(data => {
        this.allLines = data;
        console.log(data);
      });
  }

  ngOnInit() {
    this.markerInfo = new MarkerInfo(new GeoLocation(45.242268, 19.842954), 
    "assets/ftn.png",
    "Jugodrvo" , "" , "http://ftn.uns.ac.rs/691618389/fakultet-tehnickih-nauka");
    this.polyline = new Polyline([], 'blue', { url:"assets/busicon.png", scaledSize: {width: 50, height: 50}});
    this.selLine = new Polyline([], 'red', { url:"assets/busicon.png", scaledSize: {width: 50, height: 50}});

    this.mapsApiLoader.load().then(() =>{
      google.maps.event.addListener(this.sl, 'positionChanged', (function(selLine, i) {
        return function(event) {
          console.log(event.LatLngLiteral);
          alert("WTF");
        }
      }));
    });
  }

  stationClick( id: number){
    if(this.selected == 'Add'){
      this.stati.forEach(element => {
        if(element.Id == id){
          this.pomStat = element;
        }
      });
      let pom: boolean = false;
      this.selectedStations.forEach(element => {
        if(element.Id == this.pomStat.Id){
          pom = true;
        }
      });
      // console.log("pomStat:");
      // console.log(this.pomStat);
      if(pom){
        window.alert("Can't add a station more than once in one line!");
      }else{
        this.selectedStations.push(this.pomStat);
        this.polyline.addLocation(new GeoLocation(this.pomStat.Latitude, this.pomStat.Longitude))
        this.id = id;
      }
    }
  }

  SelectedLine(event: any): void
  {
    this.selectedL = event.target.value;
    
    if(this.selectedL == "none" || this.selectedL == "")
    {
      this.sl = new LineModel(0,"",[],"");
      this.selLine = new Polyline([], 'red', { url:"assets/busicon.png", scaledSize: {width: 50, height: 50}});
      this.selektovanaLinijaZaIzmenu = new LineModel(0,"",[],"");
      this.idForRemove=0;
    }
    else 
    {
      this.selektovanaLinijaZaIzmenu = new LineModel(0,"",[],"");
      this.selLine = new Polyline([], 'red', { url:"assets/busicon.png", scaledSize: {width: 50, height: 50}});
      this.allLines.forEach(x => {
        if(x.LineNumber == this.selectedL)
        {
          this.selektovanaLinijaZaIzmenu = x;
          this.sl = x;
          this.idForRemove = x.Id;
          x.Stations.forEach(stat => {
            this.selLine.addLocation(new GeoLocation(stat.Longitude, stat.Latitude));
          });
          console.log(this.selLine);
        }
      });

    }
  }

  isSelectedLine(name: string): boolean
  {
    if (!this.selectedL) { 
      return false;  
    }  
     return (this.selectedL === name); 
  }

  setradio(e: string): void   
  {  
    this.selektovanaLinijaZaIzmenu = new LineModel(0,"",[],"");
    this.sl = new LineModel(0,"",[],"");
    this.LineSelected = "none";
    this.refresh();
        this.selected = e;  
        if(this.selected != "Add")
        {
          this.boolZaAktivanRadio = false;
        }
       
  }  

  isSelected(name: string): boolean   
  {  
    if (!this.selected) {
        return false;  
    }  
    return (this.selected === name);   
  } 

  onSubmit(lineData: LineModel, form: NgForm){
    if(this.selected == "Add")
      {
        lineData.Stations = this.selectedStations;
        if(lineData.ColorLine == "" || lineData.ColorLine == null)
        {
          lineData.ColorLine = "#000000";
        }
        // console.log(lineData)
        // if(this.validations.validate(lineData)) {
        //   this.refresh();
        //   return;
        // }

        // this.allLines.forEach(element => {
        //   if(element.LineNumber == lineData.LineNumber)
        //   {
        //     window.alert("Line number already exits!");
        //       form.reset();
        //       this.refresh();
        //   }
          
        // });

        this.lineServ.addLine(lineData).subscribe(data => {
          this.boolic = data;
          window.alert("Line successfully added!");
          form.reset();
          // ponovo kupi sve linije, osvezavanje
          this.refresh();
        },
        err => {
          window.alert(err.error);
          this.refresh();
        });
        
      }
      else if(this.selected == "Change"){
        if(this.selectedL != "none")
    {
       
        lineData.Stations = this.selektovanaLinijaZaIzmenu.Stations;
        lineData.Id = this.selektovanaLinijaZaIzmenu.Id;
        lineData.ColorLine = this.selektovanaLinijaZaIzmenu.ColorLine;
        lineData.LineNumber = this.selektovanaLinijaZaIzmenu.LineNumber;
        lineData.Version = this.selektovanaLinijaZaIzmenu.Version;
        console.log(lineData);
        // if(this.validations.validate(lineData)) {
        //   this.refresh();
        //   //form.reset();
        //   return;
        // }
        this.lineServ.changeLine(this.selektovanaLinijaZaIzmenu.Id,lineData).subscribe(data =>
          {
            window.alert("Line successfully changed!");
            this.LineSelected = "none";
            form.reset();
            this.refresh();
            
          },
        err => {
          window.alert(err.error);
          this.refresh();
         });
  
        }
      }
      else if(this.selected == "Remove"){
        if(this.idForRemove == undefined)
        {
          this.idForRemove = 0;
        }
        this.lineServ.deleteLine(this.idForRemove).subscribe(data =>
          {
            window.alert("Line successfully removed!");
            // ponovo kupi sve linije, osvezavanje
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

  removeFromLine(stationId,i)
  {
    this.selektovanaLinijaZaIzmenu.Stations.splice(i,1);
  }

  addStationIntoLine(i: any, form: NgForm)
  {
    {
      let pom: boolean = false;
      this.selektovanaLinijaZaIzmenu.Stations.forEach(element=>{
        if(element.Id == this.markerZaDodavanje.Id){
          pom = true;
        }
      });
      if(pom){
        window.alert("Can't add a station more than once in one line!");
      }
      else{
        if(i<=0 || i > this.selektovanaLinijaZaIzmenu.Stations.length)
        {
          this.errorForListStat = "Index out of range!";
          form.reset();
        }
        else{
          this.errorForListStat = "";
          this.selektovanaLinijaZaIzmenu.Stations.splice(i.rBr-1,0,this.markerZaDodavanje);
          console.log(this.selektovanaLinijaZaIzmenu.Stations);
          this.boolZaMarkerZaDodavanje = false;
        }
      }  
    }
  }

  stationClick1( id: number){
    this.stati.forEach(element => {
     if(element.Id == id){
         this.markerZaDodavanje = element;
         this.boolZaMarkerZaDodavanje = true;
      }
    });
  
    console.log("marker za dodavanje:");
    console.log(this.markerZaDodavanje);
   }

   refresh()
   {
    this.polyline = new Polyline([], 'blue', { url:"assets/busicon.png", scaledSize: {width: 50, height: 50}});
    this.sl = new LineModel(0,"",[],"");
    this.selektovanaLinijaZaIzmenu = new LineModel(0,"",[],"");
    this.selLine = new Polyline([], 'red', { url:"assets/busicon.png", scaledSize: {width: 50, height: 50}});
    this.selectedL = "none";
    this.markerZaDodavanje = new StationModel("","", 0,0,0);
    this.allLines = [];
    this.selectedStations = [];
    this.boolZaMarkerZaDodavanje = false;
    this.idForRemove = 0;
    this.LineSelected = "none";
    this.lineServ.getAllLines().subscribe(data => {
    this.allLines = data;
      console.log(data);
    });
    this.statServ.getAllStations().subscribe(data => {
      this.stati = data;
      this.drugiMarkeriStati = data;
      });
   }
  
}
