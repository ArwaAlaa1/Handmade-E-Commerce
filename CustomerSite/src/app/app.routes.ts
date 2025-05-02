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
import { DetailsComponent } from './components/details/details.component';
import { FavouriteComponent } from './components/favourite/favourite.component';
import { OrderConfirmationComponent } from './components/order-confirmation/order-confirmation.component';
import { OrderComponent } from './components/order/order.component';
import { OrderDetailsComponent } from './components/order-details/order-details.component';
import { ProductsWithCategoryComponent } from './products-with-category/products-with-category.component';
import { AboutusComponent } from './components/aboutus/aboutus.component';
import { ContactusComponent } from './components/contactus/contactus.component';
import { FaqComponent } from './components/faq/faq.component';


export const routes: Routes = [
  {path: '', redirectTo: 'home', pathMatch: 'full'},
  {path:'home', component:HomeComponent, title:'Home Page'},
  {path:'offer', component:OffersComponent, title:'Offer Page'},

  {path:'ProductDetails/:ProductId', component:DetailsComponent, title:'Details Page'},

  {path:'favourite', component:FavouriteComponent, title:'Favourite Page'},



  {path:'profile', canActivate:[AuthGuard], component:ProfileComponent, title:'Profile Page'},
  {path:'editprofile', canActivate:[AuthGuard], component:EditProfileComponent, title:'Edit Profile Page'},
  {path:'addimage', canActivate:[AuthGuard], component:AddImageComponent, title:'Add Profile Image Page'},
  {path:'addAddrrss', canActivate:[AuthGuard], component:AddAddressComponent, title:'Add Address Page'},
  {path:'editAddress/:id', canActivate:[AuthGuard], component:EditAddressComponent, title:'Edit Address Page'},

  {path:'register', component:RegisterComponent, title:'Register Page'},
  {path:'orders', component:OrderComponent, title:'Orders Page'},
  {path:'orderdetails/:id', component:OrderDetailsComponent, title:'OrdrrsDetails Page'},

  {path:'login', component:LoginComponent, title:'Login Page'},
  {path:'Aboutus', component:AboutusComponent, title:'AboutUs Page'},
  {path:'Contactus', component:ContactusComponent, title:'ContactUs Page'},
  {path:'FAQ', component:FaqComponent, title:'FAQ Page'},
  {path:'ProductsWithCategory/:id', component:ProductsWithCategoryComponent, title:'Product Category Page'},

  {path:'sendpin', component:SendPinComponent, title:'Send Pin Page'},
  {path:'enterpin/:email/:expireAt', component:EnterPinComponent, title:'Enter Pin Page'},
  {path:'resetpassword/:email', component:ResetPasswordComponent, title:'Forget Password Page'},
  {path:'changepassword', canActivate:[AuthGuard], component:ChangePasswordComponent, title:'Change Password Page'},

  // {path:'addresspop', component:AddressPopUpComponent, title:'Address Page'},

  {path:'cart', component:CartComponent, title:'Cart Page'},
  {path:'order-confirmation', component:OrderConfirmationComponent, title:'order-confirmation Page'},


  {path:'**', canActivate:[AuthGuard],component: NotFoundComponent , title:'NotFound Page'}
];
