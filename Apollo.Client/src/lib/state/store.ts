import { Store } from "@tanstack/react-store";
import type { User } from "../types/user";
import type { AuthState } from "../types/auth";

// Create the store with initial state
export const store = new Store({
	authState: {
		user: {} as User,
		isAuthenticated: false,
		isLoading: true,
	} as AuthState,
});

// Auth actions
export const authActions = {
	setUser: (user: User) => {
		store.setState((state) => ({
			...state,
			authState: {
				...state.authState,
				user,
				isAuthenticated: true,
			},
		}));
	},

	clearUser: () => {
		store.setState((state) => ({
			...state,
			authState: {
				...state.authState,
				user: {} as User,
				isAuthenticated: false,
			},
		}));
	},

	setLoading: (isLoading: boolean) => {
		store.setState((state) => ({
			...state,
			authState: {
				...state.authState,
				isLoading,
			},
		}));
	},
};
