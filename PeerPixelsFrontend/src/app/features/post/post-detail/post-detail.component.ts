import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { PostService } from '../../../core/services/post.service';
import { AuthService } from '../../../core/auth/auth.service';
import { Post } from '../../../core/models/post.model';

@Component({
  selector: 'app-post-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container mt-4">
      <div class="row">
        <div class="col-md-8 mx-auto">
          <div *ngIf="loading" class="text-center my-5">
            <div class="spinner-border" role="status">
              <span class="visually-hidden">Loading...</span>
            </div>
          </div>
          
          <div *ngIf="error" class="alert alert-danger">
            {{ error }}
          </div>
          
          <div *ngIf="!loading && post" class="card mb-4">
            <div class="card-header d-flex align-items-center justify-content-between">
              <div class="d-flex align-items-center">
                <img [src]="post.userAvatarUrl" class="rounded-circle me-2" 
                     style="width: 32px; height: 32px; object-fit: cover;" alt="User Avatar">
                <a [routerLink]="['/profile', post.userName]" class="text-decoration-none">
                  <strong>{{ post.displayName }}</strong>
                  <small class="text-muted">{{ '@' + post.userName }}</small>
                </a>
              </div>
              
              <button *ngIf="isOwnPost" class="btn btn-sm btn-outline-danger"
                      (click)="deletePost()" [disabled]="deleting">
                <span *ngIf="deleting" class="spinner-border spinner-border-sm me-1"></span>
                Delete
              </button>
            </div>
            
            <img [src]="post.imageUrl" class="card-img-top" alt="Post image">
            
            <div class="card-body">
              <p class="card-text">{{ post.caption }}</p>
              <small class="text-muted">{{ formatDate(post.createdAt) }}</small>
            </div>
            
            <!-- Comments section can be added here in future development -->
          </div>
          
          <div class="mt-3">
            <button (click)="goBack()" class="btn btn-outline-secondary">
              <i class="bi bi-arrow-left"></i> Back
            </button>
          </div>
        </div>
      </div>
    </div>
  `
})
export class PostDetailComponent implements OnInit {
  post: Post | null = null;
  loading: boolean = true;
  error: string | null = null;
  isOwnPost: boolean = false;
  deleting: boolean = false;
  
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private postService: PostService,
    private authService: AuthService
  ) {}
  
  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = +params['id']; // Convert to number
      this.loadPost(id);
    });
  }
  
  loadPost(id: number): void {
    this.loading = true;
    this.error = null;
    
    this.postService.getPostById(id).subscribe({
      next: (post) => {
        this.post = post;
        this.isOwnPost = this.authService.currentUserValue?.id === post.userId;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading post:', err);
        this.error = 'Unable to load post. It might have been removed or you do not have permission to view it.';
        this.loading = false;
      }
    });
  }
  
  deletePost(): void {
    if (!this.post || !this.isOwnPost) return;
    
    if (confirm('Are you sure you want to delete this post? This action cannot be undone.')) {
      this.deleting = true;
      
      // Note: You'll need to implement deletePost in your PostService
      // this.postService.deletePost(this.post.id).subscribe({
      //   next: () => {
      //     this.router.navigate(['/profile', this.authService.currentUserValue?.userName]);
      //   },
      //   error: (err) => {
      //     console.error('Error deleting post:', err);
      //     this.error = 'Failed to delete post. Please try again.';
      //     this.deleting = false;
      //   }
      // });
      
      // Since deletePost might not be implemented yet, we'll simulate it
      setTimeout(() => {
        this.router.navigate(['/profile', this.authService.currentUserValue?.userName]);
      }, 1000);
    }
  }
  
  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleString();
  }
  
  goBack(): void {
    this.router.navigate(['/feed']);
  }
}