import { CartItem } from './../../interfaces/cart';
import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environments/environment';
import { error } from 'node:console';
import { finalize } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { v4 as uuidv4 } from 'uuid';
import { RouterLink } from '@angular/router';
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule,RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {

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

  constructor(public _productService: ProductService, private _authService: AuthService
    ,private _cartService:CartService) {}

  ngOnInit(): void {
    this.filterProducts();
    this.getCategories();
    this._authService.checktheme();
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
        console.log(response);
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  filterProducts() {
    this.isLoading = true;
    this._productService.getAllProduct(
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

  product :any = {};
  ;
  addToCart(id:number, quantity:number) {
  
    this.product= this._productService.getProductById(id)
    .subscribe((response) =>
      {
        // console.log(response);
        this.product = response;
        console.log(this.product);
        const cartItem: CartItem = {
          itemId: uuidv4().toString(),
          productId: this.product.id,
          productName: this.product.name,
          sellerName: this.product.seller.name,
          sellerId: this.product.seller.id,
          photoUrl: this.product.photos[0].url,
          category: this.product.category.name,
          customizeInfo: this.product.customizeInfo,
          price: this.product.sellingPrice,
          color: this.product.color,
          sellingPrice: this.product.sellingPrice,
          priceAfterSale: this.product.discountedPrice,
          unitPrice: this.product.sellingPrice,
          size: this.product.size,
          activeSale: this.product.salePercent,
          quantity: quantity
        };
        this._cartService.addItemToBasket(cartItem,quantity);
      });
   
    
    
  }

}