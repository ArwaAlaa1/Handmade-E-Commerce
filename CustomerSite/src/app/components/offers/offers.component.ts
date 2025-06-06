import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { Route, Router, RouterLink } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { CartItem } from '../../interfaces/cart';
import { v4 as uuidv4 } from 'uuid';
import { FormsModule } from '@angular/forms';
import { log } from 'console';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-offers',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './offers.component.html',
  styleUrl: './offers.component.css'
})
export class OffersComponent implements OnInit {


  currentPage: number = 1;
  itemsPerPage: number = 8;
  totalCount: number = 0;
  totalPages: number = 0;
  isLoading = true;
  imageBaseUrl:string = environment.baseImageURL;
  allProducts: any[] = [];

  selectedProduct: any = null;
  selectedColor: string = '';
  selectedSizeObject: { name: string; extraCost: number } | null = null;
  additionalDetails: string = '';
  selectedQuantity: number = 0;
  showValidation = false;

  filters = {
    categoryId: null,
    maxPrice: null,
    minPrice: null
  };

  constructor(public _productService: ProductService, private _cartService:CartService, private _authService: AuthService, private route : Router) {}

  ngOnInit(): void {
    this.filterProducts();
  }

  filterProducts() {
    this.isLoading = true;
    this._productService.getProductinOffer(
      this.itemsPerPage, this.currentPage).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (response) => {
         console.log(this.itemsPerPage );
         console.log(this.currentPage);

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

  compareSizes(a: any, b: any): boolean {
    return a?.name === b?.name && a?.extraCost === b?.extraCost;
  }

  openModal(product: any) {
    if (!product.stock || product.stock <= 0) {
      alert("This product is out of stock.");
      return;
    }

    this.selectedProduct = product;

    this.selectedColor = product.colors?.length === 1 ? product.colors[0] : '';
    this.selectedSizeObject = product.sizes?.length === 1 ? product.sizes[0] : null;

    this.additionalDetails = '';
    this.selectedQuantity = 1;
    this.showValidation = false;
  }

  changePage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.filterProducts();
  }

  toggleFavorite(product: any) {
    product.isFavorite = !product.isFavorite;
  }

  isLogin : boolean = false;
  addToFavorite(productId: number) {

    this._authService.userData.subscribe({
      next: (data) => {
        if (data) {
          this.isLogin = true;
        }
        else {
          this.isLogin = false;
        }
      }
    });

    if(this.isLogin == false){
      this.route.navigate(['/login']);
    }

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

    addToCart() {

      this.showValidation = true;
      const needsSize = this.selectedProduct.sizes?.length > 1;
      const needsAdditional = !!this.selectedProduct.additionalDetails;
      const quantityInvalid =
      !this.selectedQuantity ||
      this.selectedQuantity <= 0 ||
      this.selectedQuantity > this.selectedProduct.stock;

      if (
        (needsSize && !this.selectedSizeObject) ||
        (needsAdditional && !this.additionalDetails) ||
        quantityInvalid
      ) {
        return;
      }

      const payload = {
        productId: this.selectedProduct.id,
        color: this.selectedColor,
        size: this.selectedSizeObject?.name,
        extraCost: this.selectedSizeObject?.extraCost,
        additionalDetails: this.additionalDetails,
        quantity: this.selectedQuantity
      };

      this.product= this._productService.getProductById(this.selectedProduct.id)
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
            customizeInfo: this.additionalDetails,
            price: this.product.sellingPrice,
            color: this.selectedColor,
            sellingPrice: this.product.sellingPrice,
            priceAfterSale: this.product.discountedPrice,
            unitPrice: this.product.sellingPrice,
            size: this.selectedSizeObject?.name,
            extraCost: this.selectedSizeObject?.extraCost,
            activeSale: this.product.salePercent,
            quantity: this.selectedQuantity
          };
          this._cartService.addItemToBasket(cartItem, this.selectedQuantity);

          const product = this.allProducts.find(c => c.id === this.selectedProduct.id);
          if (product) {
          product.stock = product.stock - this.selectedQuantity;
          }

          this.showValidation = false;
          this.selectedProduct = null;

        });

    }

}
