import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environments/environment';
import { finalize } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-favourite',
    standalone: true,
    imports: [CommonModule, FormsModule, RouterLink],
    templateUrl: './favourite.component.html',
    styleUrl: './favourite.component.css'
})
export class FavouriteComponent implements OnInit {

  currentPage: number = 1;
  itemsPerPage: number = 4;
  totalCount: number = 0;
  totalPages: number = 0;
  isLoading = true;
  imageBaseUrl:string = environment.baseImageURL;
  allProducts: any[] = [];
  categories : any[] = [];

  filters = {
    categoryId: null,
    maxPrice: null,
    minPrice: null
  };

  constructor(public _productService: ProductService) {}

  ngOnInit(): void {
    this.filterProducts();
    this.getCategories();
  }

  applyFilters() {
    this.currentPage = 1;
    this.filterProducts();
  }

  getCategories(){
    this.isLoading = true;
    this._productService.getAllCategories(
    ).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (response) => {
        this.categories = response;
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  filterProducts() {
    this.isLoading = true;
    this._productService.GetFavList(
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
        this.totalPages = response.totalPages;
        // console.log(response);
      },
      error: (error) => {
        // console.log(error);
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
      const product = this.allProducts.find(c => c.id === productId);
      if (product) {
        product.isFavorite = true;
      }
    },
      error: (error) => {
      }
    });
  }

  deleteFromFavorite(productId: number) {
    this._productService.deleteFromFav(productId).subscribe({
      next: (response) => {
        const index = this.allProducts.findIndex(c => c.id === productId);
        if (index !== -1) {
            this.allProducts.splice(index, 1);
            this.totalCount--;
            this.totalPages--;
            if (this.allProducts.length === 0 && this.currentPage > 1) {
              this.changePage(this.currentPage - 1);
            }
        }
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  getStarRating(rating: number): number {
    const fiveStarRating = (rating / 10) * 5;
    return Math.round(fiveStarRating * 2) / 2;
  }

}
