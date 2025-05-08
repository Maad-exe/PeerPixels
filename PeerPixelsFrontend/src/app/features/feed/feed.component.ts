import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PostService } from '../../core/services/post.service';
import { Post } from '../../core/models/post.model';
import { PostCardComponent } from '../../shared/components/post-card/post-card.component';

@Component({
  selector: 'app-feed',
  standalone: true,
  imports: [CommonModule, PostCardComponent],
  template: `
    <div class="container mt-4">
      <div class="row">
        <div class="col-md-8 mx-auto">
          <h2 class="mb-4">Your Feed</h2>
          
          <div *ngIf="loading" class="text-center">
            <div class="spinner-border" role="status">
              <span class="visually-hidden">Loading...</span>
            </div>
          </div>
          
          <ng-container *ngIf="!loading">
            <div *ngIf="posts.length === 0" class="alert alert-info">
              <p>Your feed is empty. Start following other users to see their posts!</p>
            </div>
            
            <div *ngFor="let post of posts" class="mb-4">
              <app-post-card [post]="post"></app-post-card>
            </div>
            
            <div *ngIf="hasMorePosts" class="text-center mb-4">
              <button class="btn btn-primary" (click)="loadMorePosts()" [disabled]="loadingMore">
                <span *ngIf="loadingMore" class="spinner-border spinner-border-sm me-2"></span>
                Load More
              </button>
            </div>
          </ng-container>
        </div>
      </div>
    </div>
  `
})
export class FeedComponent implements OnInit {
  posts: Post[] = [];
  loading: boolean = true;
  loadingMore: boolean = false;
  hasMorePosts: boolean = true;
  currentPage: number = 1;
  pageSize: number = 10;

  constructor(private postService: PostService) {}

  ngOnInit(): void {
    this.loadPosts();
  }

  loadPosts(): void {
    this.loading = true;
    this.postService.getFeedPosts(this.currentPage, this.pageSize).subscribe({
      next: (posts) => {
        this.posts = posts;
        this.hasMorePosts = posts.length === this.pageSize;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading feed', error);
        this.loading = false;
      }
    });
  }

  loadMorePosts(): void {
    this.loadingMore = true;
    this.currentPage++;
    
    this.postService.getFeedPosts(this.currentPage, this.pageSize).subscribe({
      next: (posts) => {
        this.posts = [...this.posts, ...posts];
        this.hasMorePosts = posts.length === this.pageSize;
        this.loadingMore = false;
      },
      error: (error) => {
        console.error('Error loading more posts', error);
        this.loadingMore = false;
      }
    });
  }
}