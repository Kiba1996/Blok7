import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserProfileService } from 'src/app/services/userService/user-profile.service';
import { Router, ActivatedRoute,NavigationEnd } from '@angular/router';
import { Subject } from 'rxjs';
import { ModelHelperAuthorization } from 'src/app/models/modelHelperAuthorization';
@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
  providers: [UserProfileService]
})
export class ProfileComponent implements OnInit, OnDestroy  {
 
  user: any;
  otvorenEdit: boolean = false;
  accBool : boolean = false;
  nijeUser : boolean = false;
  navigationSubscription;
  public static returned: Subject<any> = new Subject();

  constructor(private usersService: UserProfileService, private router: Router, private route: ActivatedRoute) {
    this.requestUserInfo()
    ProfileComponent.returned.subscribe(res => {
      this.otvorenEdit = false;
      this.accBool = false;
      this.nijeUser = false;
      this.provera();
     // this.requestUserInfo(); // this populates an array

   });
   this.navigationSubscription = this.router.events.subscribe((e: any) => {
    // If it is a NavigationEnd event re-initalise the component
    if (e instanceof NavigationEnd) {
      this.otvorenEdit = false;
      this.accBool = false;
      this.nijeUser = false;
      this.provera();
      this.requestUserInfo();
    }
  });
   }

  ngOnInit() {
    this.provera();
  }
  ngOnDestroy() {
    // avoid memory leaks here by cleaning up after ourselves. If we  
    // don't then we will continue to run our initialiseInvites()   
    // method on every navigationEnd event.
    if (this.navigationSubscription) {  
       this.navigationSubscription.unsubscribe();
    }
  }
  provera() {
    var _activeChild = this.route.children.length;
      if (_activeChild!=0) {
        console.log("uslo ovdeee")
         this.otvorenEdit = true;
         this.accBool  = false;
         this.nijeUser = false;
      }
      else
      {
        this.otvorenEdit = false;
        this.accBool  = false;
        this.nijeUser = false;
      }
  }
  requestUserInfo(){
   // this.usersService.getUserClaims().subscribe(claims => {
      this.usersService.getUserData(localStorage.getItem('name')).subscribe(data => {
        
          this.user = data;   
          if(localStorage.getItem('role') == 'AppUser')
          {

            this.nijeUser = false;
            if(this.user.PassengerTypeId == 3)
            {
              this.nijeUser = true;
            }
          }
          else{
            this.nijeUser = true;
          }
          if(this.user.Activated == "Declined")  
          {
            this.accBool = true;
          } 
          else
          {
            this.accBool = false;
          }  
          console.log(this.user);    
        },
        err =>
        {
          window.alert(err.error);
          console.log(err);
        });
  }

  Resend(){
    let kor = localStorage.getItem('name');
    let m : ModelHelperAuthorization = new ModelHelperAuthorization("");
    m.Id = kor;
    this.usersService.resendReqest(m).subscribe(data =>
      {
        window.alert("Request successfully sent.");
        this.requestUserInfo();
      });
  }

  Edit(){
    this.otvorenEdit = true;
    this.router.navigate(['../profile/edit'], {relativeTo: this.route});
  }

}
