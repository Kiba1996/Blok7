import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/auth/authentication.service';
import { Subscription } from 'rxjs';
import { NotificationService } from 'src/app/services/notificationService/notification.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-menubar',
  templateUrl: './menubar.component.html',
  styleUrls: ['./menubar.component.css']
})
export class MenubarComponent implements OnInit {
  prom: string;
  
  constructor(private notServ:NotificationService, public authService: AuthenticationService, private router: Router) { }

  ngOnInit() {
    
  }

  loggedIn():string{
    if(localStorage.jwt){
      this.prom = localStorage.getItem('name');
    }
    return localStorage.jwt;
  }

  logout() {
    this.authService.logout();
    this.router.navigate(["signin"]);
    
    
  }
  get user(): any {
    return localStorage.getItem('role');
}

}
