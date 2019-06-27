import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TicketService {

  base_url = "http://localhost:52295"
  constructor(private http: Http, private httpClient:HttpClient) { }

  getAllTicketTypes() {
    return this.httpClient.get(this.base_url+"/api/Tickets/GetTicketTypes");
  }
  getTypeUser(email) {
    return this.httpClient.get(this.base_url+"/api/Account/GetPassengerTypeForUser?email="+email);
  }
  addTicket(ticket): Observable<any>{
    
    return this.httpClient.post(this.base_url+"/api/Tickets/Add",ticket);
  }
  SendMail(ticket): Observable<any>{
    
    return this.httpClient.post(this.base_url+"/api/Tickets/SendMail",ticket);
  }

  getTicket(id) {
    return this.httpClient.get(this.base_url+"/api/Tickets/GetTicket?id="+id);
  }
  getTickets(id){
    return this.httpClient.get(this.base_url+"/api/Tickets/GetTickets?id="+id);
  }
}
