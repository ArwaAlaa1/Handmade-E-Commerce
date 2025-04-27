import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment.development';

@Component({
  selector: 'app-offers',
  imports: [CommonModule],
  templateUrl: './offers.component.html',
  styleUrl: './offers.component.css'
})
export class OffersComponent implements OnInit {


  currentPage: number = 1;
  itemsPerPage: number = 4;
  totalCount: number = 11;
  totalPages: number = 2;
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
    this._productService.getProductinOffer(
      this.itemsPerPage,
      this.currentPage,
      this.filters.categoryId,
      this.filters.maxPrice,
      this.filters.minPrice
    ).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (response) => {
        this.allProducts = response.products;
        this.totalCount = response.totalCount;
        this.totalPages = Math.ceil(this.totalCount / this.itemsPerPage);
        console.log(response);
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  changePage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.filterProducts();
  }

  toggleFavorite(product: any) {
    product.isFavorite = !product.isFavorite;
  }

  addToFavorite(productId: number) {
    this._productService.addToFav(productId).subscribe({
    next:(response) =>
    {
      console.log(response);
      const product = this.allProducts.find(c => c.id === productId);
      if (product) {
      product.isFavorite = true;
      }
    },
      error: (error) => {
        console.log(error);

      }
    });
  }

  deleteFromFavorite(productId : number) {
    this._productService.deleteFromFav(productId).subscribe((response) =>
    {
      console.log(response);
      const product = this.allProducts.find(c => c.id === productId);
      if (product) {
        product.isFavorite = false;
      }
    });
  }

  getStarRating(rating: number): number {
    const fiveStarRating = (rating / 10) * 5;
    return Math.round(fiveStarRating * 2) / 2;
  }

}
