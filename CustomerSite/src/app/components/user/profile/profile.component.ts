import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile',
  imports: [CommonModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {

  userData : any = {};
  imageBaseUrl: string = `https://handmadee-commerce.runasp.net/images//`;

  constructor(public _userService: UserService) { }

  ngOnInit(): void {
    this._userService.getUserProfile()
    .subscribe({
      next: (response) => {
        this.userData = response;
      },
      error: (error) => {
      }
    });
  }

}
