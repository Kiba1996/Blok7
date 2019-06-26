import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Http } from '@angular/http';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class VerificationService {

  constructor(private http: Http, private httpClient: HttpClient) { }
  getAwaitingAdmins(): Observable<any> {
    return this.httpClient.get("http://localhost:52295/api/Account/GetAwaitingAdmins");
  }

  authorizeAdmin(adminId): Observable<any> {
    return this.httpClient.post("http://localhost:52295/api/Account/AuthorizeAdmin", adminId);
  }

  getAwaitingControllers(): Observable<any> {
    return this.httpClient.get("http://localhost:52295/api/Account/GetAwaitingAControllers");
  }

  authorizeController(controllerId): Observable<any> {
  //  let data = `${controllerId}`;
    let headers = new HttpHeaders();
    headers = headers.append( "Content-type","application/json");
    return this.httpClient.post("http://localhost:52295/api/Account/AuthorizeControll", controllerId);
  }

  getAwaitingClients(): Observable<any> {
    return this.httpClient.get("http://localhost:52295/api/Account/GetAwaitingClients");
  }

  authorizeUser(userId): Observable<any> {
    return this.httpClient.post("http://localhost:52295/api/Account/AuthorizeUser",userId);
  }

  declineAdmin(adminId): Observable<any> {
    return this.httpClient.post("http://localhost:52295/api/Account/DeclineAdmin", adminId);
  }

  declineController(controllerId): Observable<any> {
    return this.httpClient.post("http://localhost:52295/api/Account/DeclineController", controllerId);
  }
  declineUser(userId): Observable<any> {
    return this.httpClient.post("http://localhost:52295/api/Account/DeclineUser",userId);
  }

}
