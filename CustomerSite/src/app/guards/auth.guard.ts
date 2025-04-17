import { Injectable, inject } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { BehaviorSubject, Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  private _Router = inject(Router);

  userData = new BehaviorSubject<any>(null);

  canActivate(): Observable<boolean> {
    if (typeof window === 'undefined') {
      return of(false);
    }


    setTimeout(() => {
      this._Router.navigate(['/login']);
    }, 0);

    return of(false);
  }

}
