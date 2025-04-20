import { Injectable, inject } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  private _Router = inject(Router);
  private _CookieService = inject(CookieService);

  userData = new BehaviorSubject<any>(null);

  canActivate(): Observable<boolean> {
    if (typeof window === 'undefined') {
      return of(false);
    }

    const storedData = this._CookieService.get('userData');
    if (storedData) {
      const parsedData = JSON.parse(storedData);
      if (parsedData.token) {
        this.userData.next(parsedData);
        return of(true);
      }
    }

    setTimeout(() => {
      // this._Router.navigate(['/login']);
      window.location.href = '/login';
    }, 0);

    return of(false);
  }
}
