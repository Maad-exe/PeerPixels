import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreatePost, Post } from '../models/post.model';

@Injectable({
  providedIn: 'root'
})
export class PostService {

  constructor(private http: HttpClient) {}

  createPost(post: CreatePost): Observable<Post> {
    return this.http.post<Post>(`${environment.apiUrl}/posts`, post);
  }

  getPostById(id: number): Observable<Post> {
    return this.http.get<Post>(`${environment.apiUrl}/posts/${id}`);
  }

  getUserPosts(userId: string): Observable<Post[]> {
    return this.http.get<Post[]>(`${environment.apiUrl}/posts/user/${userId}`);
  }

  getFeedPosts(page: number = 1, pageSize: number = 10): Observable<Post[]> {
    return this.http.get<Post[]>(`${environment.apiUrl}/posts/feed?page=${page}&pageSize=${pageSize}`);
  }

  // This would be implemented with a real file upload service
  uploadImage(file: File): Observable<string> {
    // For this MVP, we'll just return a placeholder image URL
    return new Observable(observer => {
      setTimeout(() => {
        observer.next(`https://picsum.photos/id/${Math.floor(Math.random() * 1000)}/500/500`);
        observer.complete();
      }, 1000);
    });
  }
}