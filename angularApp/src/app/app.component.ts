import { Component, NgZone, Input, AfterViewInit, ElementRef } from '@angular/core';
import { NotificationService } from './services/notificationService/notification.service';
import { NotificationMessage } from './models/notificationMessage';
import { ToastrService } from 'ngx-toastr';
import { decode } from 'punycode';
import { NotificationsForBusLocService } from './services/for-bus-location/notifications-for-bus-loc.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [NotificationService],
  
})
export class AppComponent implements AfterViewInit{
  ngAfterViewInit(): void {
    this.elementRef.nativeElement.ownerDocument.body.style.backgroundColor = '#26313f';
   // this.elementRef.nativeElement.ownerDocument.body.style.backgroundImage = 'url(assets/logo.png)';
  }
  title = 'angularApp';
  public currentMessage: NotificationMessage;  
  public allMessages: any = "";  
  public canSendMessage: Boolean; 
  

  constructor(private notifForBL: NotificationsForBusLocService, private elementRef: ElementRef,private notificationServ: NotificationService, private _ngZone: NgZone, private toastr: ToastrService) {    
    this.subscribeToEvents();  
    this.canSendMessage = notificationServ.connectionExists; 
    //this.notifForBL.startConnection().subscribe(data => {});
    //this.sendMessage();
}

showMessage(message: string) {
  this.toastr.success(message, 'New notification!');
}  

// sendMessage(): void {
//   // send message to subscribers via observable subject
// this.notificationServ.sendMessage('Message from app Component to message Component!');   
// }

// clearMessage():void{
//   this.notificationServ.clearMessage();
// }

private subscribeToEvents(): void {   
  this.notificationServ.connectionEstablished.subscribe(() => {  
      this.canSendMessage = true;  
  });  
  this.notificationServ.messageReceived.subscribe((message: NotificationMessage) => {  
      this._ngZone.run(() => {  
          this.allMessages = message;

          if(!localStorage.jwt) return;
          if (localStorage.role == "Admin" || localStorage.role == "Controller"){
            //this.sendMessage();
            this.showMessage(this.allMessages)
          }
      });  
  });  
}  

// public subscribeForTime() {
//   this.notifForBL.registerForTimerEvents().subscribe(e => this.onTimeEvent(e));
// }


// public onTimeEvent(pos: number[]){
//   this._ngZone.run(() => { 
//      this.time = pos; 
//      if(this.isChanged){
//        this.latitude = pos[0];
//         this.longitude = pos[1];
//         console.log("pos: ", this.latitude, this.longitude);
//         //this.isChanged = false;
//      }else{
//         this.latitude = 0;
//         this.longitude = 0;
//      }
//   });      
// }  

}