import type { User } from "./user";

export type AuthState = {
  user: User;
  isAuthenticated: boolean;
  isLoading: boolean;
};
