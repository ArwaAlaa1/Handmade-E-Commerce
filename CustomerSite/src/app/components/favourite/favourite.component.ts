import { CommonService } from './../../services/common.service';
import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environments/environment';
import { finalize } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { CartItem } from '../../interfaces/cart';
import { v4 as uuidv4 } from 'uuid';

@Component({
    selector: 'app-favourite',
    standalone: true,
    imports: [CommonModule, FormsModule, RouterLink],
    templateUrl: './favourite.component.html',
    styleUrl: './favourite.component.css'
})
export class FavouriteComponent implements OnInit {

  currentPage: number = 1;
  itemsPerPage: number = 8;
  totalCount: number = 0;
  totalPages: number = 0;
  isLoading = true;
  imageBaseUrl:string = environment.baseImageURL;
  allProducts: any[] = [];
  categories : any[] = [];

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

  constructor(public _productService: ProductService, private _cartService:CartService
  ,private commonService:CommonService) {}

  ngOnInit(): void {
    this.filterProducts();
    this.getCategories();
  }

  applyFilters() {
    this.currentPage = 1;
    this.filterProducts();
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
    this.commonService.triggerRefresh();
  }

  addToFavorite(productId: number) {
    this._productService.addToFav(productId).subscribe({
    next:(response) =>
    {
      const product = this.allProducts.find(c => c.id === productId);
      if (product) {
        product.isFavorite = true;
      }
      this.commonService.triggerRefresh();
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
            this.commonService.triggerRefresh();
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
          this.commonService.triggerRefresh();
          const product = this.allProducts.find(c => c.id === this.selectedProduct.id);
          if (product) {
          product.stock = product.stock - this.selectedQuantity;
          }

          this.showValidation = false;
          this.selectedProduct = null;

        });

    }

}
