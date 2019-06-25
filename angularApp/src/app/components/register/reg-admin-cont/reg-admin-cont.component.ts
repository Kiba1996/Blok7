import { Component, OnInit } from '@angular/core';
import { Validators, FormBuilder } from '@angular/forms';
import { ConfirmPasswordValidator } from 'src/app/models/Validation/password-validator';
import { AuthenticationService } from 'src/app/services/auth/authentication.service';
import { NotificationService } from 'src/app/services/notificationService/notification.service';
import { Router } from '@angular/router';
import { RegModel } from 'src/app/models/regModel';

@Component({
  selector: 'app-reg-admin-cont',
  templateUrl: './reg-admin-cont.component.html',
  styleUrls: ['./reg-admin-cont.component.css']
})
export class RegAdminContComponent implements OnInit {

  serverErrors: string[];
  registerForm = this.fb.group({
   
    Password: ['',
      [Validators.required,
      Validators.minLength(6),
      Validators.pattern(/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\W])/)]],
    ConfirmPassword: ['',
      Validators.required],
    Email: ['',
      [Validators.email]],
    Name: ['',
      Validators.required],
    Surname: ['',
      Validators.required],
      Address: ['',
      Validators.required],
    Birthday: ['', Validators.required],
     Role: ['Admin', Validators.required]
  },
  { validators: ConfirmPasswordValidator }
   );


  get f() 
  {
     return this.registerForm.controls; 
  }
  constructor(private fb: FormBuilder, private accountService: AuthenticationService,private notificationServ: NotificationService, private router: Router) { 
 
  }



  ngOnInit() {
  }

  Button1() {
    let regModel: RegModel = this.registerForm.value;
    let formData: FormData = new FormData();

    regModel.Activated  = false;
   

   
    this.accountService.register(regModel).subscribe(
      ret => {
        this.serverErrors = [];
        this.notificationServ.sendNotification();
        this.router.navigateByUrl('/signin');

      },
      err => {
        console.log(err);
        window.alert(err.error.ModelState[""]);
        this.serverErrors = err.error.ModelState[""]

      }
    );
    
   

    
  }


}
