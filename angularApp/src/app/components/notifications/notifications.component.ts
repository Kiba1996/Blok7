import { Component, OnInit } from '@angular/core';
import { VerificationService } from 'src/app/services/verificationSerfice/verification.service';
import { UserProfileService } from 'src/app/services/userService/user-profile.service';
import { ModelHelperAuthorization } from 'src/app/models/modelHelperAuthorization';
import { Subscription } from 'rxjs';
import { NotificationService } from 'src/app/services/notificationService/notification.service';

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.css']
})
export class NotificationsComponent implements OnInit {
 user: any;
  awaitingAdmins:any = [];
  awaitingControllers:any = [];
  modelHelp: ModelHelperAuthorization = new ModelHelperAuthorization("");
  awaitingClients:any = [];
  
  userBytesImages:any = [];
  imagesLoaded:boolean = false
  wtfList:any = []

  
  constructor(private verifyService: VerificationService,private usersService: UserProfileService, private notificationServ: NotificationService) { 
    this.usersService.getUserData(localStorage.getItem('name')).subscribe(data => {
         
      this.user = data;    
      console.log("USER:");
      console.log(this.user);    
     if(this.user.Role == 'Admin')
     {
      verifyService.getAwaitingAdmins().subscribe(data => {
        this.awaitingAdmins = data;
        verifyService.getAwaitingControllers().subscribe(data => {
          this.awaitingControllers = data;
        });
      })
    }
    if(this.user.Role == 'Controller'){
      verifyService.getAwaitingClients().subscribe(data => {
        this.awaitingClients = data;
        usersService.getUserImages(this.awaitingClients).subscribe(imageBytes => {
          this.userBytesImages = imageBytes
          this.userBytesImages.forEach(element => {
            element = "data:image/png;base64," + element
            this.wtfList.push(element)
          });
          this.imagesLoaded = true
          console.log(this.userBytesImages)
        })
      })
      
    }
    });
   
    
    
  }

  ngOnInit() {
  }

  
      

  AuthorizeAdmins(id, i) {
    this.modelHelp.Id = id;
    this.verifyService.authorizeAdmin(this.modelHelp).subscribe(resp => {
      if(resp == "Ok")  {
        alert("Admin has been authorized!");
        this.awaitingAdmins.splice(i,1);
      }

      else alert("Something went wrong");
    })
  }

  AuthorizeControllers(id, i) {
    this.modelHelp.Id = id;
    this.verifyService.authorizeController(this.modelHelp).subscribe(resp => {
      if(resp == "Ok")  {
        alert("Controller has been authorized!");
        this.awaitingControllers.splice(i,1);
      }

      else alert("Something went wrong");
    })

  }

  AuthorizeUser(id, i) {
    this.modelHelp.Id = id;
    this.verifyService.authorizeUser(this.modelHelp).subscribe(resp => {
      if(resp == "Ok")  {
        alert("Client has been authorized!"); 
        this.awaitingClients.splice(i,1);
        this.wtfList.splice(i,1);
      }

      else alert("Something went wrong");
    })
  }

  DeclineAdmins(id,i)
  {
    this.modelHelp.Id = id;
    this.verifyService.declineAdmin(this.modelHelp).subscribe(resp => {
      if(resp == "Ok")  {
        alert("Admin has been declined!");
        this.awaitingAdmins.splice(i,1);
      }

      else alert("Something went wrong");
    })
  }

  DeclineControllers(id,i)
  {
    this.modelHelp.Id = id;
    this.verifyService.declineController(this.modelHelp).subscribe(resp => {
      if(resp == "Ok")  {
        alert("Controller has been declined!");
        this.awaitingControllers.splice(i,1);
      }

      else alert("Something went wrong");
    })
  }

  DeclineUser(id, i) {
    this.modelHelp.Id = id;
    this.verifyService.declineUser(this.modelHelp).subscribe(resp => {
      if(resp == "Ok")  {
        alert("Client has been declined!"); 
        this.awaitingClients.splice(i,1);
        this.wtfList.splice(i,1);
      }

      else alert("Something went wrong");
    })
  }


  
}
