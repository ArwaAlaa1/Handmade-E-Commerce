import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private _HttpClient: HttpClient, private _CookieService : CookieService, private _auth: AuthService) {
  }

  private getAuthHeaders(): HttpHeaders {
    const storedData = this._CookieService.get('userData');

    if (!storedData) {
      return new HttpHeaders();
    }
    const parsedData = JSON.parse(storedData);
    if (parsedData && parsedData.token) {
      return new HttpHeaders({
        Authorization: `Bearer ${parsedData.token}`,
        'Content-Type': 'application/json'
      });
    }
    return new HttpHeaders();
  }

  getAllProduct(
    pageSize: number,
    pageIndex: number,
    categoryId?: number | null,
    maxPrice?: number | null,
    minPrice?: number | null
  ): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    let url = `${environment.baseURL}Products/GetAllWithFilter?pageSize=${pageSize}&pageIndex=${pageIndex}`;

    if (categoryId !== null && categoryId !== undefined) {
      url += `&categoryId=${categoryId}`;
    }

    if (maxPrice !== null && maxPrice !== undefined) {
      url += `&maxPrice=${maxPrice}`;
    }

    if (minPrice !== null && minPrice !== undefined) {
      url += `&minPrice=${minPrice}`;
    }

    return this._HttpClient.get(url, { headers: this.getAuthHeaders() });
  }
  getProductById(id:number): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.get(`${environment.baseURL}Products/GetProductByIdWithOffer/${id}`, { headers });
  }

  // getProductById(productId: number): Observable<any> {

  //   const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

  //   let url = `${environment.baseURL}Products/GetProductByID/${productId}`;
  //   return this._HttpClient.get(url, { headers });
  // }
  getProById(id:number): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.get(`${environment.baseURL}Products/GetProductByIdWithOffer/${id}`, { headers : this.getAuthHeaders()});
  }
  getProductReviews(productId: number): Observable<any> {

    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    let url = `${environment.baseURL}Review/GetProductReviews/${productId}`;
    return this._HttpClient.get(url, { headers });
  }


  GetFavList(
    pageSize: number,
    pageIndex: number,
    categoryId?: number | null,
    maxPrice?: number | null,
    minPrice?: number | null
  ): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    let url = `${environment.baseURL}Products/GetFavList?pageSize=${pageSize}&pageIndex=${pageIndex}`;

    if (categoryId !== null && categoryId !== undefined) {
      url += `&categoryId=${categoryId}`;
    }

    if (maxPrice !== null && maxPrice !== undefined) {
      url += `&maxPrice=${maxPrice}`;
    }

    if (minPrice !== null && minPrice !== undefined) {
      url += `&minPrice=${minPrice}`;
    }

    return this._HttpClient.get(url, { headers: this.getAuthHeaders() });
  }

  getProductinOffer(pageSize: number, pageIndex: number): Observable<any>
  {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.get(`${environment.baseURL}Products/GetProductsWithActiveOffers?pageSize=${pageSize}&pageIndex=${pageIndex}`, { headers : this.getAuthHeaders()  });
  }

  getAllCategories(): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.get(`${environment.baseURL}Categories`, { headers });
  }

  addToFav(productId: number): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}Favorite/AddFavoriteToUser/${productId}`,
      {},
      { headers : this.getAuthHeaders(), responseType : 'text' });
  }

  deleteFromFav(productId: number): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.delete(`${environment.baseURL}Favorite/DeleteFavorite/${productId}`,
      { headers : this.getAuthHeaders(), responseType : 'text' });
  }

  AddReview(credentials: { productId: number; reviewContent: string; rating: number }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this._HttpClient.post(`${environment.baseURL}Review/AddReview`, credentials, { headers: this.getAuthHeaders() });
  }


}
