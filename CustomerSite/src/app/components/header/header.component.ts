import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit {

  isLogin: boolean = false;
  userName: string = ''
  constructor(private _AuthService : AuthService) {
  }

  ngOnInit(): void {
    this._AuthService.userData.subscribe({
      next: (data) => {
        if (data) {
          this.isLogin = true;
          this.userName = data.displayName
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
