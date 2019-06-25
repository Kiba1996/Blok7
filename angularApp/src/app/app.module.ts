import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { TokenInterceptor } from 'src/app/interceptors/token.interceptor';
import { MenubarComponent } from './components/menubar/menubar.component';
import { TimetableComponent } from './components/timetable/timetable.component';
import {RouterModule, Routes} from '@angular/router';
import { BusmapsComponent } from './components/busmaps/busmaps.component';
import { PricelistComponent } from './components/pricelist/pricelist.component';
import { SigninComponent } from './components/signin/signin.component';
import { RegisterComponent } from './components/register/register.component';

import {FormsModule, ReactiveFormsModule, ControlContainer} from'@angular/forms';
import { HttpModule } from '@angular/http';
import { ProfileComponent } from './components/profile/profile.component';
import {NgxPopper} from 'angular-popper';
import { AddChangeLinesComponent } from './components/add_change/add-change-lines/add-change-lines.component';
import { AgmCoreModule } from '@agm/core';
import { AddChangeStationsComponent } from './components/add_change/add-change-stations/add-change-stations.component';
import { AddChangeTimetableComponent } from './components/add_change/add-change-timetable/add-change-timetable.component';
import { AddChangePricelistComponent } from './components/add_change/add-change-pricelist/add-change-pricelist.component';
// import { MapComponent } from './components/map/map.component';
import { BuyATicketComponent } from './components/buy-a-ticket/buy-a-ticket.component';
import { UserSignedInGuard } from './guard/userSignedIn-guar';
import { CanActivateViaAuthGuard } from './guard/auth-guard';
import { ControlorGuard } from './guard/controler-guards';
import { EditProfileComponent } from './components/profile/edit-profile/edit-profile.component';
import {
  ToastrModule,
  ToastNoAnimation,
  ToastNoAnimationModule,
} from 'ngx-toastr';
import { NotificationsComponent } from './components/notifications/notifications.component';
import { TicketValidationComponent } from './components/ticket-validation/ticket-validation.component';


import { AgmDirectionModule } from 'agm-direction';
import { AddChangeVehicleComponent } from './components/add_change/add-change-vehicle/add-change-vehicle.component';
import { CanActivateNotification } from './guard/notification-guard';
import { CanActivateUser } from './guard/user-guard';
import { UserNotSignedInGuard } from './guard/notSignedIn-guard';
import { RegAdminContComponent } from './components/register/reg-admin-cont/reg-admin-cont.component';
import { BusLocationComponent } from './components/busmaps/bus-location/bus-location.component';
import { NgxPayPalModule } from 'ngx-paypal';

const Routes = [
  {
    path: "",
    component: HomeComponent
  },
  {
    path: "home",
    component: HomeComponent
  },
  {
    path: "timetable",
    component: TimetableComponent
  },
  {
    path: "busmaps",
    component: BusmapsComponent
  },
  {
    path: "getLocation",
    component: BusLocationComponent
  },
  {
    path: "pricelist",
    component: PricelistComponent
  },
  {
    path: "signin",
    component: SigninComponent,
    canActivate:[UserNotSignedInGuard]
  },
  {
    path: "register",
    component: RegisterComponent,
    canActivate:[UserNotSignedInGuard]
    
  },
  {
    path: "regAdminController",
    component: RegAdminContComponent,
    canActivate:[UserNotSignedInGuard]
    
  },
  
  {
    path: "profile",
    component: ProfileComponent,
    canActivate: [UserSignedInGuard],
    children: [
      {
        path:'edit',
        component: EditProfileComponent,
        canActivate: [UserSignedInGuard]
      }]
  },
  {
    path: "add_change_lines",
    component: AddChangeLinesComponent,
    canActivate: [CanActivateViaAuthGuard]
  },
  {
    path: "add_change_stations",
    component: AddChangeStationsComponent,
    canActivate: [CanActivateViaAuthGuard]
  },
  {
    path: "add_change_timetable",
    component: AddChangeTimetableComponent,
    canActivate: [CanActivateViaAuthGuard]
  },
  {
    path: "add_change_pricelist",
    component: AddChangePricelistComponent,
    canActivate: [CanActivateViaAuthGuard]
  },
  {
    path: "buy_a_ticket",
    component: BuyATicketComponent,
    canActivate: [CanActivateUser]
  },
  {
    path: "notifications",
    component: NotificationsComponent,
    canActivate: [CanActivateNotification]
  },
  {
    path: "validateTicket",
    component: TicketValidationComponent,
    canActivate: [ControlorGuard]
  },
  {
    path: "add_change_vehicle",
    component: AddChangeVehicleComponent,
    canActivate: [CanActivateViaAuthGuard]
  }
]

@NgModule({
  declarations: [
    AppComponent,
    MenubarComponent,
    HomeComponent,
    TimetableComponent,
    BusmapsComponent,
    PricelistComponent,
    SigninComponent,
    RegisterComponent,
    ProfileComponent,
    AddChangeLinesComponent,
    AddChangeStationsComponent,
    AddChangeTimetableComponent,
    AddChangePricelistComponent,
   // MapComponent,
    BuyATicketComponent,
    EditProfileComponent,
    NotificationsComponent,
    TicketValidationComponent,
    AddChangeVehicleComponent,
    RegAdminContComponent,
    BusLocationComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    RouterModule.forRoot(Routes),
    HttpModule,
    HttpClientModule,
    NgxPopper,
    ReactiveFormsModule,
    AgmDirectionModule,
    AgmCoreModule.forRoot({apiKey: 'AIzaSyDnihJyw_34z5S1KZXp90pfTGAqhFszNJk'}),
    ToastNoAnimationModule,
    ToastrModule.forRoot({
      toastComponent: ToastNoAnimation,
    }),
    NgxPayPalModule,
  ],
 // {provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true}
  providers: [ CanActivateViaAuthGuard,
    CanActivateUser,
    UserSignedInGuard,
    CanActivateNotification,
    UserNotSignedInGuard,
    ControlorGuard,{ provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true}
    
  ],
  bootstrap: [AppComponent]
})



export class AppModule { }
