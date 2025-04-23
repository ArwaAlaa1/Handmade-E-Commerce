import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {

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

    this._productService.getAllProduct(
      10,
      1,
      this.filters.categoryId,
      this.filters.maxPrice,
      this.filters.minPrice
    ).subscribe({
        next: (response) => {
          this.allProducts = response.products;
          this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
      }
    });

  }
}
