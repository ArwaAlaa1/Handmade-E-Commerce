import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { environment } from '../../../../environments/environment.development';

@Component({
  selector: 'app-profile',
  imports: [CommonModule, RouterLink],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {

  userData : any = {};
  // imageBaseUrl: string = `https://handmadee-commerce.runasp.net/images//`;
  imageBaseUrl: string = environment.baseImageURLAPI;

  constructor(public _userService: UserService) { }

  ngOnInit(): void {
    this._userService.getUserProfile()
    .subscribe({
      next: (response) => {
        this.userData = response;
        // console.log(this.userData);
      },
      error: (error) => {
      }
    });
  }

}
