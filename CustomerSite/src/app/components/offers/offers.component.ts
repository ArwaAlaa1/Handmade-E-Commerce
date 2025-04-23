import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-offers',
  imports: [CommonModule],
  templateUrl: './offers.component.html',
  styleUrl: './offers.component.css'
})
export class OffersComponent implements OnInit {

  isLoading = true;

  allProducts: any[] = [];

  filters = {
    categoryId: null,
    maxPrice: null,
    minPrice: null
  };

  constructor(public _productService: ProductService) {}

  ngOnInit(): void {
    this.filterProducts();
  }

  filterProducts() {

    this.isLoading = true;

    this._productService.getProductinOffer(
      10,
      1,
      this.filters.categoryId,
      this.filters.maxPrice,
      this.filters.minPrice
    ).subscribe({
        next: (response) => {
          this.allProducts = response.products;
          console.log(response);
          this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
      }
    });

  }
}
