import { Component, OnInit, NgZone } from '@angular/core';
import { GeoLocation } from 'src/app/models/map/geolocation';
import { MarkerInfo } from 'src/app/models/map/marker-info.model';
import { MapsAPILoader, MouseEvent } from '@agm/core';
import { StationModel } from 'src/app/models/stationModel';
import { NgForm } from '@angular/forms';
import { StationServiceService } from 'src/app/services/stationService/station-service.service';
import { Observable } from 'rxjs/internal/Observable';
//import { AddStationValidation } from 'src/app/models/Validation/validationModels';

@Component({
  selector: 'app-add-change-stations',
  templateUrl: './add-change-stations.component.html',
  styleUrls: ['./add-change-stations.component.css'],
  styles: ['agm-map {height: 500px; width: 700px;}']
})
export class AddChangeStationsComponent implements OnInit {
  private selected: string="";
  coordinates: GeoLocation = new GeoLocation(0,0); 
  markerInfo: MarkerInfo;
  private geocoder;
  name: string = "";
  address: string;
  stati: any = [];
  id: number;
  version: number;
  public allStations: any = [];
  iconPath : any = { url:"assets/busicon.png", scaledSize: {width: 50, height: 50}}
  //validations : AddStationValidation = new AddStationValidation();
  constructor(private ngZone: NgZone, private mapsApiLoader : MapsAPILoader, private statServ: StationServiceService) { 
    this.statServ.getAllStations().subscribe(data => {
    this.stati = data;
    });
  }

  ngOnInit() {
   this.markerInfo = new MarkerInfo(new GeoLocation(45.242268, 19.842954), 
    "assets/ftn.png",
    "Jugodrvo" , "" , "http://ftn.uns.ac.rs/691618389/fakultet-tehnickih-nauka");
      this.mapsApiLoader.load().then(() =>{
      this.geocoder = new google.maps.Geocoder();
    });
  }

  onSubmit(stationData: StationModel, form: NgForm){
    if(this.selected == "Add")
    {
      stationData.Latitude = this.coordinates.latitude;
      stationData.Longitude = this.coordinates.longitude;
      stationData.Address = this.address;
      console.log(stationData)
      // if(this.validations.validate(stationData)) {
      //   this.refresh();
      //   return;
      // }
      this.statServ.addStation(stationData).subscribe(data => 
        {
          window.alert("Station successfully added!");
          form.reset();
          this.refresh();
        },
        err => {
          window.alert(err.error);
          this.refresh();
        });
    }
    else if(this.selected == "Change"){
      stationData.Latitude = this.coordinates.latitude;
      stationData.Longitude = this.coordinates.longitude;
      stationData.Address = this.address;
      if(stationData.Name == "" || stationData.Name == null)
      {
        stationData.Name = this.name;
      }
      stationData.Id = this.id;
      stationData.Version = this.version;
      // if(this.validations.validate(stationData))
      // {
      //   this.refresh();
      //   return;
      // } 
      console.log("stationData")
      console.log(stationData)
      this.statServ.changeStation(stationData).subscribe(data =>
        {
          window.alert("Station successfully changed!");
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

  RemoveStation()
  {
    if(this.id == undefined)
    {
      this.id = 0;
    }
    this.statServ.deleteStation(this.id).subscribe(data =>
      {
        window.alert("Station successfully removed!");
        this.refresh();
      },
      err => {
        window.alert(err.error);
        this.refresh();
       });
    }

  setradio(e: string): void   
  {  
        this.selected = e; 
        this.stati = [];
        this.name = "";
        this.address = "";
        this.statServ.getAllStations().subscribe(data => {
          this.stati = data;
          });     
  }  

  isSelected(name: string): boolean   
  { 
    if (!this.selected) {   
        return false;  
    }  
    return (this.selected === name); 
  } 

  placeMarker1($event){
    this.coordinates = new GeoLocation($event.coords.lat, $event.coords.lng);
    this.getAddress(this.coordinates.latitude,this.coordinates.longitude);
  }

  getAddress(latitude: number,longitude:number){
    this.geocoder.geocode({'location': {lat: latitude, lng: longitude}}, (results,status) =>{
      console.log(results);
      if(status === 'OK'){
          if(results[0]){
            this.address = results[0].formatted_address;
          }
          else{
            window.alert('no results found');
          }
      }
    });
    
  }

  markerDragEnd($event: MouseEvent, name:string, id: number, version: number) {
    console.log($event);
     this.coordinates.latitude = $event.coords.lat;
     this.coordinates.longitude = $event.coords.lng;
     this.getAddress(this.coordinates.latitude, this.coordinates.longitude);
     this.name = name;
     this.id = id;
     this.version = version;
     console.log(id);
  }

  stationClick(id: number){
    this.id = id;
  }

  refresh(){
    this.stati = [];
    this.address = "";
    this.name = "";
    
    this.coordinates = new GeoLocation(0,0); 
    this.statServ.getAllStations().subscribe(data => {
      this.stati = data;
      });
  }

}
