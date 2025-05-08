import { User } from './user.model';

export interface Login {
  email: string;
  password: string;
}

export interface Register {
  userName: string;
  email: string;
  password: string;
  displayName: string;
}

export interface AuthResponse {
  succeeded: boolean;
  token: string;
  user: User;
  message: string;
}

export interface GoogleAuth {
  idToken: string;
}