import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { environment } from '../../../../environments/environment.development';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-profile',
  imports: [CommonModule, RouterLink],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {

  userData : any = {};
  isLoading: boolean = true;
  errorMessage: string = '';
  // imageBaseUrl: string = `https://handmadee-commerce.runasp.net/images//`;
  imageBaseUrl: string = environment.baseImageURLAPI;

  constructor(public _userService: UserService, private router: Router) {}

  ngOnInit(): void {
    this.loadUserData();
  }

  loadUserData(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this._userService.getUserProfile().subscribe({
      next: (response) => {
        this.userData = response;
        this.isLoading = false;
        console.log('user', this.userData);
      },
      error: (error) => {
        this.errorMessage = 'Failed to load profile data. Please try again later.';
        this.isLoading = false;
        console.error('Error loading user data:', error);
      }
    });
  }

  retry(): void {
    this.loadUserData();
  }

  logout(): void {
    Swal.fire({
      title: 'Are you sure?',
      text: 'You will be logged out of your account.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#6c7fd8',
      cancelButtonColor: '#dc3545',
      confirmButtonText: 'Yes, logout',
      cancelButtonText: 'Cancel'
    }).then((result) => {
      if (result.isConfirmed) {
        localStorage.removeItem('token');
        this.router.navigate(['/login']);
        Swal.fire({
          title: 'Logged out!',
          text: 'You have been successfully logged out.',
          icon: 'success',
          confirmButtonColor: '#6c7fd8',
          timer: 1500,
          showConfirmButton: false
        });
      }
    });
  }
}