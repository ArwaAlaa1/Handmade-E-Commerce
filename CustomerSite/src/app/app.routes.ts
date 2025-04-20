import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { RegisterComponent } from './components/auth/register/register.component';
import { LoginComponent } from './components/auth/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { SendPinComponent } from './components/auth/send-pin/send-pin.component';
import { EnterPinComponent } from './components/auth/enter-pin/enter-pin.component';
import { ResetPasswordComponent } from './components/auth/reset-password/reset-password.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { CartComponent } from './components/cart/cart.component';

export const routes: Routes = [
  {path: '', redirectTo: 'home', pathMatch: 'full'},
  {path:'register', component:RegisterComponent, title:'Register Page'},
  {path:'login', component:LoginComponent, title:'Login Page'},
  {path:'home', component:HomeComponent, title:'Home Page'},
  
  {path:'sendpin', component:SendPinComponent, title:'Send Pin Page'},
  {path:'enterpin/:email/:expireAt', component:EnterPinComponent, title:'Enter Pin Page'},
  {path:'resetpassword/:email', component:ResetPasswordComponent, title:'Forget Password Page'},
  {path:'cart', component:CartComponent, title:'Cart Page'},
  {path:'**', canActivate:[AuthGuard],component: NotFoundComponent , title:'NotFound Page'}
];
