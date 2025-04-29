import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { CommonModule } from '@angular/common';
import { environment } from '../../../environments/environment.development';
import { get } from 'node:http';
import { FormBuilder, FormGroup, Validators  } from '@angular/forms';

@Component({
  selector: 'app-details',
  imports: [CommonModule],
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.css']
})
export class DetailsComponent implements OnInit {
  ProductId: number = 0;
  product: any = null;
  imageBaseUrl: string = environment.baseImageURL;
  loading: boolean = true;
  error: string | null = null;
  sizes: string = ''; // To hold the formatted sizes
  selectedImageIndex: number = 0;
  isLoading = true;
  prductphotos: any = null;
  
  reviews: any[] =[]
  reviewForm!: FormGroup;
  constructor(private _route: ActivatedRoute, private _service: ProductService,private fb: FormBuilder) { 
    this.ProductId=this._route.snapshot.params['ProductId'];
  }
  ngOnInit(): void {
    this.filterProducts();
    this.getreviews(this.ProductId);
    this.reviewForm = this.fb.group({
      rating: [null, [Validators.required, Validators.min(1), Validators.max(10)]],
      reviewContent: ['', [Validators.required, Validators.minLength(10)]]
    });
  }
  selectImage(index: number): void {
    this.selectedImageIndex = index;
  }
  filterProducts() {

    this.isLoading = true;

    this._service.getProductById(this.ProductId)
      .subscribe({
        next: (response) => {
          this.product = response;
          console.log(response);
          this.prductphotos=this.product.photos;
          this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
      }
    });
  }

 
  // Format sizes as a comma-separated string
  formatSizes(): void {
    if (this.product?.sizes) {
      this.sizes = this.product.sizes.map((s: { name: string }) => s.name).join(', ');
    }
  }
  addToFavorite(id:number){

  }
  deleteFromFavorite(id:number){

  }

  roundToFive(rating: number): number {
    return Math.round(rating / 2);
  }
  currentImageIndex: number = 1;

prevImage() {
  if (this.product?.photos?.length) {
    this.currentImageIndex =
      (this.currentImageIndex - 1 + this.product.photos.length) % this.product.photos.length;
  }
}

nextImage() {
  if (this.product?.photos?.length) {
    this.currentImageIndex =
      (this.currentImageIndex + 1) % this.product.photos.length;
  }
}


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

    // تقدر تبعتها لـ API هنا:
    console.log('Review Submitted:', reviewData);

    // بعد الإرسال الناجح:
    this.reviewForm.reset();
  } else {
    this.reviewForm.markAllAsTouched(); // تظهر الأخطاء لو المستخدم ضغط قبل ما يكمّل
  }
}
}

  
  