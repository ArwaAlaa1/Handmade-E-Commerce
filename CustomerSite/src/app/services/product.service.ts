import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private _HttpClient: HttpClient) {
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

    return this._HttpClient.get(url, { headers });
  }


}
