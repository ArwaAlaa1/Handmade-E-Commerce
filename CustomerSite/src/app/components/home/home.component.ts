import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {

  isLoading = true;
  imageBaseUrl:string = environment.baseImageURL;
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
          console.log(response);

          this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
      }
    });
  }

  toggleFavorite(product: any) {
    product.IsFavorite = !product.IsFavorite;
  }

  addToFavorite(productId: number) {
    this._productService.addToFav(productId).subscribe(() => {
      const product = this.allProducts.find(c => c.id === productId);
      if (product) {
        product.IsFavorite = true;
      }
    });
  }

  deleteFromFavorite(productId : number) {
    this._productService.deleteFromFav(productId).subscribe(() => {
      const product = this.allProducts.find(c => c.id === productId);
      if (product) {
        product.IsFavorite = false;
      }
    });
  }

}
