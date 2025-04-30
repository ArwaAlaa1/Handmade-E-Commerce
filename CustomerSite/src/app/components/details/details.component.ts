import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environments/environment.development';
import { FormControl, FormGroup, ReactiveFormsModule, Validators  } from '@angular/forms';

@Component({
  selector: 'app-details',
  imports: [ReactiveFormsModule, CommonModule],
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

  // selectedImageIndex: number = 0;
  // sizes: string = ''; // To hold the formatted sizes
  // prductphotos: any = null;
  // Math: any;

  reviewForm: FormGroup = new FormGroup(
    {
      rating: new FormControl(null, { validators: [Validators.required,Validators.min(1), Validators.max(10)] }),
      reviewContent: new FormControl(null, { validators: [Validators.required, Validators.minLength(10)] }),
    },
  );

  constructor(private _route: ActivatedRoute, private _service: ProductService) {
    this.ProductId=this._route.snapshot.params['ProductId'];
  }

  ngOnInit(): void {
    this.filterProducts();
    this.getreviews(this.ProductId);
  }

  filterProducts() {
    this.isLoading = true;
    this._service.getProById(this.ProductId)
      .subscribe({
        next: (response) => {
          this.product = response;
          console.log(response);
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

  addToFavorite(productId: number) {
    this._service.addToFav(productId).subscribe({
    next:(response) =>
    {
      this.product.isFavorite = true;
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
    console.log('Review Submitted:', reviewData);
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

}


