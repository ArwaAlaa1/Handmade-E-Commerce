import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { RegisterComponent } from './components/auth/register/register.component';
import { LoginComponent } from './components/auth/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { SendPinComponent } from './components/auth/send-pin/send-pin.component';
import { EnterPinComponent } from './components/auth/enter-pin/enter-pin.component';
import { ResetPasswordComponent } from './components/auth/reset-password/reset-password.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { ChangePasswordComponent } from './components/auth/change-password/change-password.component';
import { CartComponent } from './components/cart/cart.component';
import { ProfileComponent } from './components/user/profile/profile.component';
import { EditProfileComponent } from './components/user/edit-profile/edit-profile.component';
import { AddImageComponent } from './components/user/add-image/add-image.component';
import { AddAddressComponent } from './components/user/add-address/add-address.component';
import { EditAddressComponent } from './components/user/edit-address/edit-address.component';
import { OffersComponent } from './components/offers/offers.component';
import { FavouriteComponent } from './components/favourite/favourite.component';

export const routes: Routes = [
  {path: '', redirectTo: 'home', pathMatch: 'full'},
  {path:'home', component:HomeComponent, title:'Home Page'},
  {path:'offer', component:OffersComponent, title:'Offer Page'},
  {path:'favourite', component:FavouriteComponent, title:'Favourite Page'},


  {path:'profile', canActivate:[AuthGuard], component:ProfileComponent, title:'Profile Page'},
  {path:'editprofile', canActivate:[AuthGuard], component:EditProfileComponent, title:'Edit Profile Page'},
  {path:'addimage', canActivate:[AuthGuard], component:AddImageComponent, title:'Add Profile Image Page'},
  {path:'addAddrrss', canActivate:[AuthGuard], component:AddAddressComponent, title:'Add Address Page'},
  {path:'editAddress/:id', canActivate:[AuthGuard], component:EditAddressComponent, title:'Edit Address Page'},

  {path:'register', component:RegisterComponent, title:'Register Page'},
  {path:'login', component:LoginComponent, title:'Login Page'},
  {path:'sendpin', component:SendPinComponent, title:'Send Pin Page'},
  {path:'enterpin/:email/:expireAt', component:EnterPinComponent, title:'Enter Pin Page'},
  {path:'resetpassword/:email', component:ResetPasswordComponent, title:'Forget Password Page'},
  {path:'changepassword', canActivate:[AuthGuard], component:ChangePasswordComponent, title:'Change Password Page'},


  {path:'cart', component:CartComponent, title:'Cart Page'},

  {path:'**', canActivate:[AuthGuard],component: NotFoundComponent , title:'NotFound Page'}
];
