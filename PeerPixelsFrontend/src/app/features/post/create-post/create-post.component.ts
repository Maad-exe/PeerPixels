import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { PostService } from '../../../core/services/post.service';
import { CreatePost } from '../../../core/models/post.model';

@Component({
  selector: 'app-create-post',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="container mt-4">
      <div class="row">
        <div class="col-md-8 mx-auto">
          <div class="card">
            <div class="card-header">
              <h2>Create New Post</h2>
            </div>
            <div class="card-body">
              <form [formGroup]="postForm" (ngSubmit)="onSubmit()">
                <div class="mb-3">
                  <label for="image" class="form-label">Image</label>
                  <input type="file" class="form-control" id="image" (change)="onFileChange($event)" accept="image/*">
                  
                  <div *ngIf="previewUrl" class="mt-3">
                    <img [src]="previewUrl" class="img-fluid rounded" alt="Preview">
                  </div>
                </div>
                
                <div class="mb-3">
                  <label for="caption" class="form-label">Caption</label>
                  <textarea formControlName="caption" class="form-control" id="caption" rows="3"></textarea>
                </div>
                
                <button type="submit" class="btn btn-primary" [disabled]="postForm.invalid || !selectedFile || uploading">
                  <span *ngIf="uploading" class="spinner-border spinner-border-sm me-2"></span>
                  Share Post
                </button>
              </form>
              
              <div *ngIf="error" class="alert alert-danger mt-3">
                {{ error }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class CreatePostComponent {
  postForm: FormGroup;
  selectedFile: File | null = null;
  previewUrl: string | null = null;
  uploading: boolean = false;
  error: string = '';

  constructor(
    private formBuilder: FormBuilder,
    private postService: PostService,
    private router: Router
  ) {
    this.postForm = this.formBuilder.group({
      caption: ['', Validators.required]
    });
  }

  onFileChange(event: Event): void {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      this.selectedFile = fileInput.files[0];
      this.createImagePreview();
    }
  }

  createImagePreview(): void {
    if (!this.selectedFile) {
      return;
    }
    
    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl = reader.result as string;
    };
    reader.readAsDataURL(this.selectedFile);
  }

  onSubmit(): void {
    if (this.postForm.invalid || !this.selectedFile) {
      return;
    }
    
    this.uploading = true;
    this.error = '';
    
    // Upload the image first
    this.postService.uploadImage(this.selectedFile).subscribe({
      next: (imageUrl) => {
        // Then create the post with the image URL
        const createPost: CreatePost = {
          imageUrl: imageUrl,
          caption: this.postForm.value.caption
        };
        
        this.postService.createPost(createPost).subscribe({
          next: (post) => {
            this.uploading = false;
            this.router.navigate(['/feed']);
          },
          error: (error) => {
            this.uploading = false;
            this.error = error.error?.message || 'Failed to create post';
          }
        });
      },
      error: (error) => {
        this.uploading = false;
        this.error = 'Failed to upload image';
      }
    });
  }
}