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
import { Router, RouterLink } from '@angular/router';
import { CommonService } from '../../services/common.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
categoryPhoto:string="/Images/Categories/";
  currentPage: number = 1;
  itemsPerPage: number = 4;
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

  constructor(private commonService:CommonService,public _productService: ProductService, private _authService: AuthService
    ,private _cartService:CartService,private route:Router) {}

  ngOnInit(): void {
    this.filterProducts();
    this.getCategories();
    this._authService.checktheme();
  }

  compareSizes(a: any, b: any): boolean {
    return a?.name === b?.name && a?.extraCost === b?.extraCost;
  }

  openModal(product: any) {
    if (!product.stock || product.stock <= 0) {
       Swal.fire({
        icon: 'warning',
        title: 'Out of Stock',
        text: 'This product is out of stock.',
        confirmButtonText: 'OK',
        confirmButtonColor: '#3085d6',
        customClass: {
          popup: 'animated fadeInDown'
        }
      });
      return;
    }

    this.selectedProduct = product;

    this.selectedColor = product.colors?.length === 1 ? product.colors[0] : '';
    this.selectedSizeObject = product.sizes?.length === 1 ? product.sizes[0] : null;

    this.additionalDetails = '';
    this.selectedQuantity = 1;
    this.showValidation = false;
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
        console.log('categories', this.categories);
      },
      error: (error) => {
        console.log(error);
      }
    });
  }
  get categorySlides() {
    const chunkSize = 3;
    const result = [];
    for (let i = 0; i < this.categories.length; i += chunkSize) {
      result.push(this.categories.slice(i, i + chunkSize));
    }
    return result;
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
      this.commonService.triggerRefresh();
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
      this.commonService.triggerRefresh();
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
      Swal.fire({
        icon: 'warning',
        title: 'Invalid Input',
        text: 'Please fill in all required fields.',
        confirmButtonText: 'OK',
        confirmButtonColor: '#3085d6',
        customClass: {
          popup: 'animated fadeInDown'
        }
      });
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

        const imageUrl = this.imageBaseUrl && this.product?.photos?.[0]?.url
        ? `${this.imageBaseUrl}${this.product.photos[0].url}`
        : 'https://via.placeholder.com/100'; 
        // Swal.fire({
        //   icon: 'success',
        //   title: 'Item Added Successfully',
        //   text: `${this.product.name} has been added to your cart!`,
        //   confirmButtonText: 'OK',
        //   confirmButtonColor: '#3085d6',
        //   customClass: {
        //     popup: 'animated fadeInDown'
        //   }
        // });

        Swal.fire({
          icon: 'success',
          title: 'Item Added Successfully! ',
          html:`
          <div style="text-align: center;">
            <img src="${imageUrl}" alt="${this.product.name}" style="width: 100px; height: 100px; border-radius: 10px; margin-bottom: 10px;" />
            <p><strong>${this.product.name}</strong> has been added to your cart!</p>
          </div>
        `,
          showCancelButton: true,
          confirmButtonText: 'View Cart',
          cancelButtonText: 'Continue Shopping',
          confirmButtonColor: '#3085d6',
          cancelButtonColor: '#28a745', 
          customClass: {
            popup: 'animated bounceIn', 
            confirmButton: 'swal2-confirm-button', 
            cancelButton: 'swal2-cancel-button' 
          },
          backdrop: 'rgba(0,0,0,0.8)', 
          timer: 5000, 
          timerProgressBar: true,
         
          willClose: () => {
            const popup = Swal.getPopup();
            if (popup) {
              popup.classList.add('animated', 'bounceOut'); 
            }
          }
        }).then((result) => {
          if (result.isConfirmed) {
           
            this.route.navigate(['/cart']); 
          }
        });
        this.showValidation = false;
        this.selectedProduct = null;
        this.commonService.triggerRefresh();
      });
  }

  GetProductsWithThisCategory(categoryId:number){
    this.route.navigate(['/ProductsWithCategory',categoryId]);
  }
}
