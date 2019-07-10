import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/auth/authentication.service';
import { RegModel } from 'src/app/models/regModel';
import { Validators, FormBuilder, NgForm } from '@angular/forms';


import { FileUploadService } from 'src/app/services/fileUploadService/file-upload.service';
import { NotificationService } from 'src/app/services/notificationService/notification.service';
import { Router } from '@angular/router';
import { ConfirmPasswordValidator } from 'src/app/models/Validation/password-validator';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

    serverErrors: string[];
    fileDisabled: boolean = true;
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
     PassengerType: ['Regular', Validators.required]
  },
  { validators: ConfirmPasswordValidator }
   );
   selectedImage: any = null;
   types:any = [];


  onFileSelected(event){
    this.selectedImage = event.target.files;
     
  }

  get f() 
  {
     return this.registerForm.controls; 
  }
  constructor(private fb: FormBuilder, private fileUploadService: FileUploadService, private accountService: AuthenticationService,private notificationServ: NotificationService, private router: Router) { 
    accountService.getTypes().subscribe(types => {
          this.types = types;
        });
  }



  ngOnInit() {
  }


  SelectedType(event: any): void{
    if(event.target.value == "Regular")
    {
    this.fileDisabled = true;
    }else{
      this.fileDisabled = false;
    }
    console.log("FILEDISABLED: ");
    console.log(this.fileDisabled);
  }

  Button1() {
    let regModel: RegModel = this.registerForm.value;
    let formData: FormData = new FormData();

    
    regModel.Role = "AppUser";

    if (this.selectedImage == undefined || this.selectedImage == null){
      regModel.Activated  = "Deactivated";
    this.accountService.register(regModel).subscribe(
      ret => {
        this.serverErrors = [];
        this.router.navigateByUrl('/signin');

      },
      err => {
        console.log(err);
        window.alert(err.error.Message);
        //this.serverErrors = err.error.ModelState[""]
       
      }
    );
    }
    else{
      regModel.Activated  = "Pending";
      this.fileUploadService.uploadFile(this.selectedImage)
         .subscribe(data => { 
          this.accountService.register(regModel).subscribe(
            ret => {
              this.serverErrors = [];
              console.log("ret", ret);
              if(ret == "sve je ok")
              {
              this.notificationServ.sendNotificationToController();
              this.router.navigateByUrl('/signin');
            }
            else
            {
              console.log("nesto nece d posalje notifikaciju");
              this.router.navigateByUrl('/signin');
            }
            },
            err => {
              console.log(err);
              window.alert(err.error.Message);
              // window.alert(err.error.ModelState[""]);
              // this.serverErrors = err.error.ModelState[""]
            }
          );
         },
         err => {
          window.alert(err.error);
         });

    }
  }


}
