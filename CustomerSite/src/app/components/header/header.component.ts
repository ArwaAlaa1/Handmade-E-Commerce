import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { environment } from '../../../environments/environment.development';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit {

  isLogin: boolean = false;
  userName: string = '';
  photo: string | null = null;
  baseImageUrl : string = environment.baseImageURLAPI;
  constructor(private _AuthService : AuthService) {
  }

  ngOnInit(): void {

    this._AuthService.userData.subscribe({
      next: (data) => {
        if (data) {
          this.isLogin = true;
          this.userName = data.displayName;
          this.photo = data.image;
        }
        else
        {
          this.isLogin = false;
        }
      }
    });


  }

  logout(){
    this._AuthService.signout();
  }

}
