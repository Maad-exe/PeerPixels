import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../../core/services/user.service';
import { AuthService } from '../../../core/auth/auth.service';
import { UpdateUser, User } from '../../../core/models/user.model';

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  template: `
    <div class="container mt-4">
      <div class="row">
        <div class="col-md-6 mx-auto">
          <h2 class="mb-4">Edit Profile</h2>
          
          <div *ngIf="loading" class="text-center my-5">
            <div class="spinner-border" role="status">
              <span class="visually-hidden">Loading...</span>
            </div>
          </div>
          
          <div *ngIf="error" class="alert alert-danger">
            {{ error }}
          </div>
          
          <div *ngIf="successMessage" class="alert alert-success">
            {{ successMessage }}
          </div>
          
          <form *ngIf="!loading" [formGroup]="profileForm" (ngSubmit)="onSubmit()">
            <div class="mb-4 text-center">
              <img [src]="previewImage || currentUser?.avatarUrl" class="rounded-circle mb-3"
                   style="width: 150px; height: 150px; object-fit: cover;" alt="Profile Picture">
              
              <div class="mb-3">
                <label for="avatarFile" class="form-label">Change Profile Picture</label>
                <input class="form-control" type="file" id="avatarFile" accept="image/*" (change)="onFileSelected($event)">
              </div>
            </div>
            
            <div class="mb-3">
              <label for="displayName" class="form-label">Display Name</label>
              <input type="text" class="form-control" id="displayName" formControlName="displayName">
              <div *ngIf="profileForm.get('displayName')?.invalid && profileForm.get('displayName')?.touched" class="text-danger">
                Display Name is required
              </div>
            </div>
            
            <div class="mb-3">
              <label for="userName" class="form-label">Username</label>
              <input type="text" class="form-control" id="userName" [value]="currentUser?.userName" readonly>
              <small class="text-muted">Username cannot be changed</small>
            </div>
            
            <div class="mb-3">
              <label for="email" class="form-label">Email</label>
              <input type="email" class="form-control" id="email" [value]="currentUser?.email" readonly>
              <small class="text-muted">Email cannot be changed</small>
            </div>
            
            <div class="d-flex gap-2 mt-4">
              <button type="submit" class="btn btn-primary" [disabled]="profileForm.invalid || updating">
                <span *ngIf="updating" class="spinner-border spinner-border-sm me-2"></span>
                Save Changes
              </button>
              <button type="button" class="btn btn-outline-secondary" (click)="cancel()">Cancel</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  `
})
export class EditProfileComponent implements OnInit {
  profileForm!: FormGroup;
  currentUser: User | null = null;
  loading: boolean = true;
  updating: boolean = false;
  error: string | null = null;
  successMessage: string | null = null;
  previewImage: string | null = null;
  selectedFile: File | null = null;
  
  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private userService: UserService,
    private authService: AuthService
  ) {}
  
  ngOnInit(): void {
    this.initForm();
    this.loadUserData();
  }
  
  initForm(): void {
    this.profileForm = this.formBuilder.group({
      displayName: ['', [Validators.required]],
      avatarUrl: ['']
    });
  }
  
  loadUserData(): void {
    this.loading = true;
    
    if (!this.authService.currentUserValue) {
      this.error = 'User not authenticated';
      this.router.navigate(['/login']);
      return;
    }
    
    const userId = this.authService.currentUserValue.id;
    
    this.userService.getUserById(userId).subscribe({
      next: (user) => {
        this.currentUser = user;
        this.profileForm.patchValue({
          displayName: user.displayName,
          avatarUrl: user.avatarUrl
        });
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading user data:', err);
        this.error = 'Failed to load user data. Please try again.';
        this.loading = false;
      }
    });
  }
  
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    
    if (input.files && input.files[0]) {
      this.selectedFile = input.files[0];
      
      // Preview image
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.previewImage = e.target.result;
      };
      reader.readAsDataURL(this.selectedFile);
    }
  }
  
  onSubmit(): void {
    if (this.profileForm.invalid) return;
    
    this.updating = true;
    this.error = null;
    this.successMessage = null;
    
    const updateData: UpdateUser = {
      displayName: this.profileForm.value.displayName
    };
    
    if (this.selectedFile) {
      // In a real application, you would upload the file to a server and get a URL back
      // For now, we'll simulate this process
      
      // Simulating image upload - in a real app you would call a file upload service
      setTimeout(() => {
        // Simulate getting an avatar URL from the server
        updateData.avatarUrl = this.previewImage || this.currentUser?.avatarUrl;
        
        this.updateProfileData(updateData);
      }, 1000);
    } else {
      this.updateProfileData(updateData);
    }
  }
  
  updateProfileData(updateData: UpdateUser): void {
    this.userService.updateUser(updateData).subscribe({
      next: (updatedUser) => {
        this.successMessage = 'Profile updated successfully!';
        this.updating = false;
        
        // Update the local user data
        if (this.authService.currentUserValue) {
          this.authService.updateCurrentUser({
            ...this.authService.currentUserValue,
            displayName: updatedUser.displayName,
            avatarUrl: updatedUser.avatarUrl
          });
        }
        
        // Navigate back to profile after a short delay
        setTimeout(() => {
          this.router.navigate(['/profile', this.authService.currentUserValue?.userName]);
        }, 1500);
      },
      error: (err) => {
        console.error('Error updating profile:', err);
        this.error = 'Failed to update profile. Please try again.';
        this.updating = false;
      }
    });
  }
  
  cancel(): void {
    this.router.navigate(['/profile', this.currentUser?.userName]);
  }
}