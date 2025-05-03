import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environments/environment.development';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators  } from '@angular/forms';
import { response } from 'express';
import { CartService } from '../../services/cart.service';
import { CartItem } from '../../interfaces/cart';
import { v4 as uuidv4 } from 'uuid';
import { AuthService } from '../../services/auth.service';
import { CommonService } from '../../services/common.service';

@Component({
  selector: 'app-details',
  imports: [ReactiveFormsModule, CommonModule, FormsModule],
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.css']
})
export class DetailsComponent implements OnInit {
  ProductId: number = 0;
  product: any = null;
  imageBaseUrl: string = environment.baseImageURL;
  imageBaseUrlAPI: string = environment.baseImageURLAPI;
  loading: boolean = true;
  error: string | null = null;
  isLoading = true;
  selectedPhotoUrl: string = '';
  reviews: any[] = []

  selectedColor: string = '';
  selectedSizeObject: { name: string; extraCost: number } | null = null;
  additionalDetails: string = '';
  selectedQuantity: number = 0;
  showValidation = false;
  isModalOpen: boolean = false;

  // selectedImageIndex: number = 0;
  // sizes: string = ''; // To hold the formatted sizes
  // prductphotos: any = null;
  // Math: any;

  newreviewcontent:string='';
  inputrate:number=0;

  reviewForm: FormGroup = new FormGroup(
    {
      rating: new FormControl(null, { validators: [Validators.required,Validators.min(1), Validators.max(10)] }),
      reviewContent: new FormControl(null, { validators: [Validators.required, Validators.minLength(10)] }),
    },
  );

  constructor(private _service: ProductService, private _authService:AuthService, private _router:Router, private commonService:CommonService,private _route: ActivatedRoute, private _cartService:CartService) {
    this.ProductId=this._route.snapshot.params['ProductId'];
  }

  countdown: string = '';
  private timer: any;
  isOfferExpired: boolean = false;


  ngOnInit(): void {
    this.filterProducts();
    this.getreviews(this.ProductId);

  }

  compareSizes(a: any, b: any): boolean {
    return a?.name === b?.name && a?.extraCost === b?.extraCost;
  }

  openModal(product: any) {
    if (!product.stock || product.stock <= 0) {
      alert("This product is out of stock.");
      return;
    }

    this.product = product;

    this.selectedColor = product.colors?.length === 1 ? product.colors[0] : '';
    this.selectedSizeObject = product.sizes?.length === 1 ? product.sizes[0] : null;

    this.additionalDetails = '';
    this.selectedQuantity = 1;
    this.showValidation = false;
    this.isModalOpen = true;
  }

  startCountdown() {
    this.timer = setInterval(() => {
      const now = new Date();
      const endDate = new Date(this.product.offer.endDate);

      if (now > endDate) {
        this.countdown = 'Time Out';
        this.isOfferExpired = true;
        clearInterval(this.timer);
        return;
      }

      const diff = endDate.getTime() - now.getTime();
      this.countdown = this.formatCountdown(diff);
    }, 1000);
  }

  formatCountdown(diff: number): string {
    const days = Math.floor(diff / (1000 * 60 * 60 * 24));
    const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((diff % (1000 * 60)) / 1000);

    return `${days} Days ${hours} Hours ${minutes} Minutes ${seconds} Seconds`;
  }

  filterProducts() {
    this.isLoading = true;
    this._service.getProById(this.ProductId)
      .subscribe({
        next: (response) => {
          this.product = response;
          console.log('pro' , response);
          if (this.product.offer) {
            this.startCountdown();
          }
          this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
      }
    });
  }

  selectPhoto(url: string) {
    this.selectedPhotoUrl = url;
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
      this._router.navigate(['/login']);
    }

    this._service.addToFav(productId).subscribe({
    next:(response) =>
    {
      this.product.isFavorite = true;
      this.commonService.triggerRefresh();
      console.log(response);
    },
      error: (error) => {
        console.log(error);
      }
    });
  }

  deleteFromFavorite(productId : number) {
    this._service.deleteFromFav(productId).subscribe({
    next:(response) =>
    {
      this.product.isFavorite = false;
      this.commonService.triggerRefresh();
      console.log(response);
    },
    error: (error) => {
      console.log(error);
      }
    });
  }

  getStarRating(rating: number): number {
    const fiveStarRating = (rating / 10) * 5;
    return Math.round(fiveStarRating * 2) / 2;
  }

  // formatSizes(): void {
  //   if (this.product?.sizes) {
  //     this.sizes = this.product.sizes.map((s: { name: string }) => s.name).join(', ');
  //   }
  // }

// currentImageIndex: number = 1;
// prevImage() {
//   if (this.product?.photos?.length) {
//     this.currentImageIndex =
//       (this.currentImageIndex - 1 + this.product.photos.length) % this.product.photos.length;
//   }
// }

// nextImage() {
//   if (this.product?.photos?.length) {
//     this.currentImageIndex =
//       (this.currentImageIndex + 1) % this.product.photos.length;
//   }
// }


  getreviews(id:number){
    this._service.getProductReviews(id).subscribe({
      next: (response) => {
        this.reviews = response;
        console.log(response);
      },
      error: (error) => {
        console.error('Error fetching reviews:', error);
      }
    });
  }

  submitReview() {
  if (this.reviewForm.valid) {
    const reviewData = this.reviewForm.value;
    // console.log('Review Submitted:', reviewData);
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
      this._router.navigate(['/login']);
    }

    this._service.AddReview(this.product.id,this.reviewForm.controls['reviewContent'].value,this.voteValue*2).subscribe({
      next:(response)=>{
        console.log(response);
        this.getreviews(this.ProductId);
      },
      error: (error) => {
        console.error('Error cannot add review:', error);
      }

    });

    this.reviewForm.reset();
    } else {
      this.reviewForm.markAllAsTouched();
    }
  }

  voteValue = 0;
  setVote(value: number) {
    this.voteValue = value;
    this.reviewForm.controls['rating'].setValue(value * 2);
  }
  getRoundedRating(rating: number): number {
    return Math.round(rating / 2);
  }


    addToCart() {

      this.showValidation = true;
      const needsSize = this.product.sizes?.length > 1;
      const needsAdditional = !!this.product.additionalDetails;
      const quantityInvalid =
      !this.selectedQuantity ||
      this.selectedQuantity <= 0 ||
      this.selectedQuantity > this.product.stock;

      if (
        (needsSize && !this.selectedSizeObject) ||
        (needsAdditional && !this.additionalDetails) ||
        quantityInvalid
      ) {
        return;
      }

      const payload = {
        productId: this.product.id,
        color: this.selectedColor,
        size: this.selectedSizeObject?.name,
        extraCost: this.selectedSizeObject?.extraCost,
        additionalDetails: this.additionalDetails,
        quantity: this.selectedQuantity
      };

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
      this.product.stock = this.product.stock - this.selectedQuantity;

      this.isModalOpen = false
      this.showValidation = false;
    }

}
