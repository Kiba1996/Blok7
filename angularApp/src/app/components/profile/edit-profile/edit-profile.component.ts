import { Component, OnInit } from '@angular/core';
import { UserProfileService } from 'src/app/services/userService/user-profile.service';
import { RegisterComponent } from '../../register/register.component';
import { RegModel } from 'src/app/models/regModel';
import { NgForm } from '@angular/forms';
import { ChangePasswordModel } from 'src/app/models/changePassModel';
import { FileUploadService } from 'src/app/services/fileUploadService/file-upload.service';
import { NotificationService } from 'src/app/services/notificationService/notification.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ProfileComponent } from '../profile.component';

@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.css']
})
export class EditProfileComponent implements OnInit {
  user : any;
  selectedImage: any;
  constructor(private router: Router, private route: ActivatedRoute, private usersService: UserProfileService, private fileServ: FileUploadService, private notificationServ: NotificationService) 
  { 
    this.requestUserInfo()
  }

  ngOnInit() {
  }
  requestUserInfo(){
    //this.usersService.getUserClaims().subscribe(claims => {
      this.usersService.getUserData(localStorage.getItem('name')).subscribe(data => {
        
          this.user = data;    
          let str = this.user.Birthday;
          this.user.Birthday = str.split('T')[0];
          console.log(this.user);    
      });
     
   // });
  }

  Button1(userr: RegModel, form: NgForm)
  {
    let errorss = [];
    userr.Id = this.user.Id;
    
    if (this.selectedImage == undefined){
      this.usersService.edit(userr).subscribe(data =>{
        if(localStorage.getItem('name') != this.user.Email)
        {
            localStorage.setItem('name', this.user.Email);
        }
        //this.router.navigateByUrl("/profile");
        window.alert("You successfully edited you account!");
        ProfileComponent.returned.next(false);
        this.router.navigate(['profile']);
        
      }, err =>
      {
        //window.alert(err.error.ModelState[""]);
        for(var key in err.error.ModelState)
        {
          for(var i = 0; i < err.error.ModelState[key].length; i++)
          {
              errorss.push(err.error.ModelState[key][i]);
          }
        }
        console.log(errorss);
        window.alert(errorss);
      });
      }else{
          this.fileServ.uploadFile(this.selectedImage)
          .subscribe(data => {      
            //alert("Image uploaded.");  
            this.usersService.edit(userr).subscribe(data =>
              {
                if(localStorage.getItem('name') != this.user.Email)
                {
                 localStorage.setItem('name', this.user.Email);
                }
                if(localStorage.getItem('role') == 'AppUser'){
                  this.notificationServ.sendNotificationToController();
                }
                //this.router.navigateByUrl("/profile");
                window.alert("You successfully edited you account!");
                ProfileComponent.returned.next(false);
                this.router.navigate(['profile']);
              }, err =>
              {
                //window.alert(err.error.ModelState[""]);
                for(var key in err.error.ModelState)
                {
                  for(var i = 0; i < err.error.ModelState[key].length; i++)
                  {
                    errorss.push(err.error.ModelState[key][i]);
                  }
                }
                console.log(errorss);
                window.alert(errorss);
              }
            );
          }, err =>
          {
            //window.alert(err.error.ModelState[""]);
            for(var key in err.error.ModelState)
            {
              for(var i = 0; i < err.error.ModelState[key].length; i++)
              {
                errorss.push(err.error.ModelState[key][i]);
              }
            }
            console.log(errorss);
            window.alert(errorss);
          });
        }
   
  }
  Button2(pass: ChangePasswordModel, form:NgForm )
  {
    let errorss = [];
    this.usersService.editPassword(pass).subscribe(data=>{
     // this.router.navigateByUrl("/profile");
     window.alert("You successfully edited you account!");
      ProfileComponent.returned.next(false);
      this.router.navigate(['profile']);
    }, err =>
    {
      //window.alert(err.error.ModelState[""]);
      for(var key in err.error.ModelState)
      {
        for(var i = 0; i < err.error.ModelState[key].length; i++)
        {
          errorss.push(err.error.ModelState[key][i]);
        }
      }
      console.log(errorss);
      window.alert(errorss);
    });
  }
  onFileSelected(event){
    this.selectedImage = event.target.files;
   
  }
}
