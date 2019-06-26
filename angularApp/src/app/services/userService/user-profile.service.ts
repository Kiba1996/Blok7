import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserProfileService {

  base_url = "http://localhost:52295"
  constructor(private http: Http, private httpClient: HttpClient) { }

  getUserClaims() {
    return this.httpClient.get('http://localhost:52295/api/Account/UserInfo')
  }
  getUserData(email:string) {
    return this.httpClient.get('http://localhost:52295/api/Account/GetUser?email='+email)
  }
  // getUserTypes(){
  //   return this.httpClient.get('http://localhost:52295/api/Account/UserTypes')
  // }
  edit(user): Observable<any>{
    console.log(user);
    return this.httpClient.post(this.base_url+"/api/Account/Edit",user);
  }
  editPassword(pass): Observable<any>{
    console.log(pass);
    return this.httpClient.post(this.base_url+"/api/Account/ChangePassword",pass);
  }

  getUserImages(emails:any) {
    return this.httpClient.post('http://localhost:52295/api/Account/GetUserImages',emails)
  }

  resendReqest(user) : Observable<any> {
    return this.httpClient.post(this.base_url+"/api/Account/ResendRequest",user);
  }


}