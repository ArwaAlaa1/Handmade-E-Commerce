import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { environment } from '../../../environments/environment.development';
import { CookieService } from 'ngx-cookie-service';
import { CommonService } from '../../services/common.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit {

  isDarkMode = false;
  isLogin: boolean = false;
  userName: string = '';
  photo: string | null = null;
  baseImageUrl : string = environment.baseImageURLAPI;
  constructor(private _AuthService : AuthService, private _CookieService: CookieService
  ,public commonService: CommonService) {
    const theme = this._CookieService.get('theme');
    this.isDarkMode = theme === 'dark';
  }

  ngOnInit(): void {
    
    this.updateBodyClass();
    this.commonService.getCartCount();
    this.commonService.getOrderCount();
  
    this.commonService.refreshNotifier$.subscribe(() => {
      this.commonService.getCartCount();
      this.commonService.getOrderCount();
    });
    this._AuthService.userData.subscribe({
      next: (data) => {
        if (data) {
          this.isLogin = true;
          this.userName = data.displayName;
          this.photo = data.image;
        }
        else {
          this.isLogin = false;
        }
      }
    });
  }


  toggleTheme() {
    this.isDarkMode = !this.isDarkMode;
    this._CookieService.set('theme', this.isDarkMode ? 'dark' : 'light');
    this.updateBodyClass();
  }

  updateBodyClass() {
    if (typeof window !== 'undefined' && typeof document !== 'undefined') {
      if (this.isDarkMode) {
        document.body.classList.add('dark-mode');
      } else {
        document.body.classList.remove('dark-mode');
      }
    }
  }

  logout(){
    this._AuthService.signout();
  }

}
