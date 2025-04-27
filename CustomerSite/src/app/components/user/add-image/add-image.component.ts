import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-add-image',
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './add-image.component.html',
  styleUrl: './add-image.component.css'
})
export class AddImageComponent implements OnInit {
  addImageForm: FormGroup = new FormGroup(
    {
      photo : new FormControl(null, { validators: [Validators.required, this.imageExtensionValidator()] }),
    },
  );

  errorMessage: string = '';
  isLoading :boolean = false;

  constructor(private _userService: UserService,
    private _Router : Router
  ) {}

  ngOnInit(): void {}

  imageExtensionValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const file = control.value;
      if (!file) return null;

      if (file instanceof FileList && file.length > 0) {
        const fileName = file[0].name.toLowerCase();
        return fileName.endsWith('.jpg') || fileName.endsWith('.png') ? null : { invalidImageType: true };
      }

      return null;
    };
  }

  get f() {
    return this.addImageForm.controls;
  }

  onSubmit() {
    if (this.addImageForm.invalid) {
      this.addImageForm.markAllAsTouched();
      return;
    }
    this.submitAddImageForm();
  }

  onFileSelected(event: Event) {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      const file = fileInput.files[0];
      this.addImageForm.patchValue({ photo: file });
      this.addImageForm.get('photo')?.updateValueAndValidity();
    }
  }

  submitAddImageForm()
  {
    this.isLoading = true;
    const formData = new FormData();

    const imageFile = this.addImageForm.get('photo')?.value;

    if (imageFile instanceof File) {
      formData.append('photo', imageFile);
    }

    this._userService.AddImage(formData).subscribe({
      next: (response) => {
          this.isLoading = false;
          console.log(response);

          window.alert('Your Photo added Successfully.');
          this._Router.navigate(['/profile']);
      },
      error: (error) => {
        if (error.error) {
          console.log(error);
          this.isLoading = false;
          if (typeof error.error === 'string') {
            this.errorMessage = error.error;
          } else if (error.error.errors) {
            this.errorMessage = Object.values(error.error.errors).flat().join(' ');
          }
        }
      }
    });
  }

}

