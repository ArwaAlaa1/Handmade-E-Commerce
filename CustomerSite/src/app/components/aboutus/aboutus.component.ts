import { CommonModule } from '@angular/common';
import { Component, NgModule } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterModule } from '@angular/router';

@Component({
  selector: 'app-aboutus',
  imports: [RouterLink, CommonModule,RouterLinkActive],
  templateUrl: './aboutus.component.html',
  styleUrl: './aboutus.component.css'
})
export class AboutusComponent {

}
