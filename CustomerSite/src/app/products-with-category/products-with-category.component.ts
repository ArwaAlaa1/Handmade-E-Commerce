import { ProductService } from './../services/product.service';
import { Component, OnInit } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { finalize } from 'rxjs';
import { CommonModule } from '@angular/common';
import { CartService } from '../services/cart.service';
import { AuthService } from '../services/auth.service';
import { CartItem } from '../interfaces/cart';
import { v4 as uuidv4 } from 'uuid';
import { FormsModule, NgModel } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonService } from '../services/common.service';
@Component({
  selector: 'app-products-with-category',
  imports: [CommonModule, RouterModule,FormsModule],
  templateUrl: './products-with-category.component.html',
  styleUrl: './products-with-category.component.css'
})
export class ProductsWithCategoryComponent {
  // currentPage: number = 1;
  // itemsPerPage: number = 4;
  // totalCount: number = 0;
  // totalPages: number = 0;
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

 
  categoryId: number = 0;

  constructor(private commonService:CommonService,public _productService: ProductService, private _authService: AuthService
      ,private _cartService:CartService, private activatedRoute: ActivatedRoute, private route: Router) {}
  
  ngOnInit(): void {
      
     
      this._authService.checktheme();
      this.activatedRoute.paramMap.subscribe(params => {
      const idParam = params.get('id');
      this.isLoading = true;
      if (idParam) {
        this.categoryId = +idParam; 
        console.log('Received Category ID:', this.categoryId);
      this._productService.getAllProductsByCategory(this.categoryId).subscribe({
        next: (data) => {
       
          this.allProducts = data.products;
        
          console.log("allProducts", this.allProducts.length);
          console.log("products",this.allProducts);
          this.isLoading = false;
        }
        , error: (err) => {
          console.log(err);
        }
      });
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


 
  


  // filterProducts() {
  //   this.isLoading = true;
  //   this._productService.getAllProduct(
  //     this.itemsPerPage,
  //     this.currentPage,
  //     this.filters.categoryId,
  //     this.filters.maxPrice,
  //     this.filters.minPrice
  //   ).pipe(
  //     finalize(() => this.isLoading = false)
  //   ).subscribe({
  //     next: (response) => {
  //       this.allProducts = response.products;
  //       this.totalCount = response.totalCount;
  //       this.totalPages = Math.ceil(this.totalCount / this.itemsPerPage);
  //       // console.log(response);
  //     },
  //     error: (error) => {
  //       // console.log(error);
  //     }
  //   });
  // }

  // changePage(page: number) {
  //   if (page < 1 || page > this.totalPages) return;
  //   this.currentPage = page;
  //   this.filterProducts();
  // }

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
      this.commonService.triggerRefresh();
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
      const product = this.allProducts.find(c => c.id === productId);
      if (product) {
        product.isFavorite = false;
        this.commonService.triggerRefresh();
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
        this.commonService.triggerRefresh();
      });
  }

  GetProductsWithThisCategory(categoryId:number){
    this.route.navigate(['/ProductsWithCategory',categoryId]);
  }
 
}


