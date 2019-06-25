import { Component, OnInit, NgZone } from '@angular/core';
import { Polyline } from 'src/app/models/map/polyliner';
import { StationModel } from 'src/app/models/stationModel';
import { LineServiceService } from 'src/app/services/lineService/line-service.service';
import { GeoLocation } from 'src/app/models/map/geolocation';
import { ForBusLocationService } from 'src/app/services/for-bus-location/for-bus-location.service';
import { NotificationsForBusLocService } from 'src/app/services/for-bus-location/notifications-for-bus-loc.service';
import { GoogleMapsAPIWrapper, MapsAPILoader } from '@agm/core';
import { MarkerInfo } from 'src/app/models/map/marker-info.model';

@Component({
  selector: 'app-bus-location',
  templateUrl: './bus-location.component.html',
  styleUrls: ['./bus-location.component.css'],
  styles: ['agm-map {height: 500px; width: 700px;}']
})
export class BusLocationComponent implements OnInit {

  public polyline: Polyline;
  public polylineRT: Polyline;  
  public zoom: number = 15;
  startLat : number = 45.242268;
  startLon : number = 19.842954;

  options : string[];
  options1: any;
  stations : StationModel[] = [];
  buses : any[];
  busImgIcon : any = {url:"assets/busicon.png", scaledSize: {width: 50, height: 50}};
  autobusImgIcon : any = {url:"assets/autobus.png", scaledSize: {width: 50, height: 50}};

  isConnected: boolean;
  notifications: string[];
  time: number[] = [];

  latitude : number ;
  longitude : number;
  marker: MarkerInfo = new MarkerInfo(new GeoLocation(this.startLat,this.startLon),"","","","");

  isChanged : boolean = false;

//iconPath : any = { url:"assets/busicon.png", scaledSize: {width: 50, height: 50}};
  constructor(private mapsApiLoader : MapsAPILoader,private notifForBL : NotificationsForBusLocService, private ngZone: NgZone, private lineService : LineServiceService, private clickService : ForBusLocationService) {
    this.isConnected = false;
    this.notifications = [];
   }

  ngOnInit() {
    this.isChanged = false;
    //za combobox izlistaj sve linije
    this.lineService.getAllLines().subscribe(
      data =>{
        this.options = [];
        this.options1 = data;
        this.options1.forEach(element => {
          this.options.push(element.LineNumber);
        });
      });
    //inicijalizacija polyline
    this.polyline = new Polyline([], 'blue', { url:"assets/busicon.png", scaledSize: {width: 50, height: 50}});
    // this.marker = new MarkerInfo(new GeoLocation(45.242268, 19.842954), 
    // "assets/ftn.png",
    // "Jugodrvo" , "" ,"");
    //this.mapsApiLoader.load().then(() =>{
  //   this.marker = new google.maps.Marker({
  //     position: new google.maps.LatLng(this.latitude,this.longitude),
  //   map: new google.maps.Map(document.getElementById("mapa")),
  //     title: "Your current location!"
  // });
  //  });
    //za hub
    this.checkConnection();
    this.subscribeForTime();
  }

  getStationsByLineNumber(lineNumber : string){
    this.options1.forEach(element => {
      if(element.LineNumber == lineNumber)
      {
        this.stations = element.Stations;
        for(var i=0; i<this.stations.length; ++i){
          this.polyline.addLocation(new GeoLocation(this.stations[i].Latitude, this.stations[i].Longitude));
        }
        console.log(this.stations);
        this.clickService.click(this.stations).subscribe();
      }
    });
    // this.lineService.getAllStationsByLineNumber(lineNumber).subscribe(
    //   data =>{
    //     this.stations = data;
    //     for(var i=0; i<this.stations.length; ++i){
    //       this.polyline.addLocation(new GeoLocation(this.stations[i].Lat, this.stations[i].Lon));
    //     }
    //     console.log(this.stations);
    //     this.clickService.click(this.stations).subscribe();
    //   });
  }

  onSelectionChangeNumber(event){
    this.isChanged = true;
    this.stations = [];
    this.polyline.path = [];
    if(event.target.value == "")
    {
      this.isChanged = false;
      this.stations = [];
      this.polyline.path = [];
      this.stopTimer();
    }else
    {
      this.getStationsByLineNumber(event.target.value);   
    
      this.notifForBL.StartTimer(); 
    }
    
  }

  private checkConnection(){
    this.notifForBL.startConnection().subscribe(e => {
      this.isConnected = e; 
        if (e) {
          this.notifForBL.StartTimer()
        }
    });
  }  

 public subscribeForTime() {
    this.notifForBL.registerForTimerEvents().subscribe(e => this.onTimeEvent(e));
  }

  position1 : number;
  position2: number;

  public onTimeEvent(pos: number[]){
    this.ngZone.run(() => { 
       this.time = pos; 
       if(this.isChanged){
         this.latitude = pos[0];
          this.longitude = pos[1];
          // this.marker.location.latitude = pos[0];
          // this.marker.location.longitude = pos[0];
         //this.transition(pos);
          
       }else{
          this.latitude = 0;
          this.longitude = 0;
       }
    });      
  }  

  public startTimer() {    
    this.notifForBL.StartTimer();
  }

  public stopTimer() {
    this.notifForBL.StopTimer();
    console.log("valjda stopira timer")
    this.time = null;
  }

//   numDeltas:number = 100;
//  delay: number = 5; //milliseconds
// i:number = 0;
//  deltaLat: number;
//  deltaLng: number;
// transition(result){
//     this.i = 0;
//     //this.deltaLat = 0.2;
//     // (result[0] - this.latitude)/this.numDeltas;
//    // this.deltaLng = 0.2;
//     //(result[1] - this.longitude)/this.numDeltas;
//     this.moveMarker();
// }

//  moveMarker(){
//   this.latitude += this.deltaLat;
//   this.longitude += this.deltaLng;
//   console.log("ulazi u ovoooo");
//     var latlng = new GeoLocation(this.latitude, this.longitude);
//    //this.marker.updatePosition(latlng);
//     if(this.i!=this.numDeltas){
//         this.i++;
//         setTimeout(this.moveMarker, this.delay);
//     }
//   }
}
