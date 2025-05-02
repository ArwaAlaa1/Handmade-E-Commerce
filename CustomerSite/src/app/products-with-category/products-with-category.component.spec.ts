import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductsWithCategoryComponent } from './products-with-category.component';

describe('ProductsWithCategoryComponent', () => {
  let component: ProductsWithCategoryComponent;
  let fixture: ComponentFixture<ProductsWithCategoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductsWithCategoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductsWithCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
