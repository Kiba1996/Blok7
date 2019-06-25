import { Injectable } from '@angular/core';
import { StationModel } from 'src/app/models/stationModel';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ForBusLocationService {

  url = "http://localhost:52295/";

  constructor(public http: HttpClient) { 

  }

  click(stations : StationModel[]): Observable<any> {
    let httpOptions = {
        headers:{
          "Content-type":"application/json"
        }
      } 

    return this.http.post(this.url+"api/Stations/SendStationsToHub",stations,httpOptions);
}
}
